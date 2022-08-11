using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardUseEffectManager : SingletonMonoBehaviour<CardUseEffectManager>
{

    GameView gameView = UIManager.instance.GetView("GameView") as GameView;

    #region 当前所使用的的卡
    public GameObject CurrentCard;//当前被移动的卡
    public int CardToRoleID;//当前卡移动到哪个角色下
    public CurrentCardPoolModel CurrentCardModel;
    #endregion

    /// <summary>
    /// 卡牌使用效果
    /// </summary>
    /// <param name="hasUseCard">卡牌是否使用成功</param>
    /// <param name="EffectOn">效果作用在。0AI,1角色</param>
    public void CardUseEffect(ref bool hasUseCard, ref string EffectOn)
    {
        var ownRole = BattleManager.instance.OwnPlayerData[0];
        var AiRole = BattleManager.instance.EnemyPlayerData[0];
        Dictionary<string, string> dic = new Dictionary<string, string>();
        Common.DicDataRead(ref dic, GlobalAttr.EffectTypeFileName, "Card");
        //能量不足无法使用卡牌
        if (ownRole.Energy < CurrentCardModel.Consume)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
            return;
        }
        #region BUFF限制卡牌使用
        var buffResult = BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref CurrentCardModel, ref ownRole.handCardList);
        if (buffResult?.Count > 0)
        {
            if (buffResult.Exists(a => a.EffectType == 1 && a.HasValid == false))
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
                return;
            }
        }
        #endregion

        #region 卡牌拖动到任意位置
        #region 血量变化
        if (CurrentCardModel.EffectType == 3)//血量变化
        {
            hasUseCard = true;
            CardUseBeforeTriggerEvent(CurrentCardModel.UsingBeforeTriggerList);
            if (CurrentCardModel.Consume > 0)
            {
                Common.EnergyImgChange(ownRole.Energy, CurrentCardModel.Consume, 0, ownRole.EnergyMax);
                ownRole.Energy -= CurrentCardModel.Consume;
            }
            if (CurrentCardModel.Effect < 0)
            {
                var effect = ownRole.bloodNow + Convert.ToInt32(CurrentCardModel.Effect);
                if (effect < 0)//血量不足以使用此卡
                {
                    hasUseCard = false;
                }
                else
                {
                    Destroy(CurrentCard);
                    Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, CurrentCardModel.Effect, 0);
                    ownRole.bloodNow += Convert.ToInt32(CurrentCardModel.Effect);
                }
            }
            else
            {
                Destroy(CurrentCard);
                if (ownRole.bloodNow != ownRole.bloodMax)
                {
                    var changeHP = CurrentCardModel.Effect;
                    if (ownRole.bloodNow + CurrentCardModel.Effect > ownRole.bloodMax)
                    {
                        changeHP = ownRole.bloodMax - ownRole.bloodNow;
                        ownRole.bloodNow = ownRole.bloodMax;
                    }
                    else
                    {
                        ownRole.bloodNow += Convert.ToInt32(changeHP);
                    }
                    Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, changeHP, 1);
                }
            }
            EffectOn = "1";
            gameView.txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
        }
        #endregion
        #region 能量变化
        else if (CurrentCardModel.EffectType == 4)//能量变化
        {
            hasUseCard = true;
            CardUseBeforeTriggerEvent(CurrentCardModel.UsingBeforeTriggerList);
            if (CurrentCardModel.Effect < 0)//扣能量
            {
                var effect = ownRole.Energy + Convert.ToInt32(CurrentCardModel.Effect);
                if (effect < 0)//能量不足以使用此卡
                {
                    hasUseCard = false;
                }
                else
                {
                    Destroy(CurrentCard);
                    Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(CurrentCardModel.Effect), 0, ownRole.EnergyMax);
                    ownRole.Energy += Convert.ToInt32(CurrentCardModel.Effect);
                }
            }
            else
            {
                Destroy(CurrentCard);
                Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(CurrentCardModel.Effect), 1, ownRole.EnergyMax);
                ownRole.Energy += Convert.ToInt32(CurrentCardModel.Effect);
                if (ownRole.Energy > ownRole.EnergyMax)
                {
                    ownRole.Energy = ownRole.EnergyMax;
                }
            }
            EffectOn = "1";
        }
        #endregion
        #region 复制卡
        else if (CurrentCardModel.EffectType == 12)//复制卡
        {
            hasUseCard = true;
            CardUseBeforeTriggerEvent(CurrentCardModel.UsingBeforeTriggerList);
            Destroy(CurrentCard);
            gameView.obj_RemoveCard.SetActive(true);
        }
        #endregion
        #region AI每次攻击手牌攻击力+1
        else if (CurrentCardModel.EffectType == 13)//AI每次攻击手牌攻击力+1
        {
            hasUseCard = true;
            CardUseBeforeTriggerEvent(CurrentCardModel.UsingBeforeTriggerList);
        }
        #endregion
        #region 卡牌攻击力叠加
        else if (CurrentCardModel.EffectType == 14)//卡牌攻击力叠加
        {
            hasUseCard = true;
            CardUseBeforeTriggerEvent(CurrentCardModel.UsingBeforeTriggerList);
        }
        #endregion
        #endregion
        if (!hasUseCard)
        {
            if (CardToRoleID == 0)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
                return;
            }
            else
            {
                #region 愤怒、虚弱等作用在数值的Buff
                if (CurrentCardModel.EffectType == 8 || CurrentCardModel.EffectType == 11)//愤怒
                {
                    hasUseCard = true;
                    CardUseBeforeTriggerEvent(CurrentCardModel.UsingBeforeTriggerList);
                    Destroy(CurrentCard);
                    if (CurrentCardModel.Consume > 0)
                    {
                        Common.EnergyImgChange(ownRole.Energy, CurrentCardModel.Consume, 0, ownRole.EnergyMax);
                        ownRole.Energy -= CurrentCardModel.Consume;
                    }
                    if (CardToRoleID == ownRole.playerID)//加到角色上
                    {
                        #region BUff添加
                        if (ownRole.buffList == null)
                        {
                            var buffDatas = new List<BuffData>();
                            buffDatas.Add(new BuffData
                            {
                                Name = dic[CurrentCardModel.EffectType.ToString()],
                                Num = CurrentCardModel.Effect,
                                EffectType = CurrentCardModel.EffectType,
                                BUFFType = 0
                            });
                            ownRole.buffList = buffDatas;
                            GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                            tempBuff = Common.AddChild(gameView.P_buffObj.transform, tempBuff);
                            tempBuff.name = "img_Buff_" + CurrentCardModel.EffectType;
                            var img = tempBuff.GetComponent<Image>();
                            Common.ImageBind(CurrentCardModel.CardUrl, img);
                            var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                            buffNum.text = (Convert.ToUInt32(buffNum.text) + CurrentCardModel.Effect).ToString();
                            BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref CurrentCardModel, ref ownRole.handCardList, true);
                        }
                        else
                        {
                            if (!ownRole.buffList.Exists(a => a.EffectType == 9))//免疫状态下所有buff卡不可使用
                            {
                                var aa = ownRole.buffList.Find(a => a.Name == dic[CurrentCardModel.EffectType.ToString()]);//是否以存在此buff
                                if (aa != null)
                                {
                                    aa.Num += CurrentCardModel.Effect;
                                    //UI操作
                                    gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref CurrentCardModel);
                                }
                                else
                                {
                                    //一种类型只能存在一条。比如虚弱和愤怒不能共存
                                    var buffs = ownRole.buffList.Find(a => a.BUFFType == 0);
                                    if (buffs != null)
                                    {
                                        ownRole.buffList.Find(a => a.BUFFType == 0).Num = 0;
                                    }
                                    ownRole.buffList.Add(new BuffData
                                    {
                                        Name = dic[CurrentCardModel.EffectType.ToString()],
                                        Num = CurrentCardModel.Effect,
                                        EffectType = CurrentCardModel.EffectType,
                                        BUFFType = 0
                                    });
                                    GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                    tempBuff = Common.AddChild(gameView.P_buffObj.transform, tempBuff);
                                    tempBuff.name = "img_Buff_" + CurrentCardModel.EffectType;
                                    var img = tempBuff.GetComponent<Image>();
                                    Common.ImageBind(CurrentCardModel.CardUrl, img);
                                    var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                    buffNum.text = (Convert.ToUInt32(buffNum.text) + CurrentCardModel.Effect).ToString();
                                    //UI操作
                                    gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref CurrentCardModel);
                                    BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref CurrentCardModel, ref ownRole.handCardList, true);
                                }
                            }
                        }
                        #endregion
                    }
                    else if (CardToRoleID == AiRole.playerID)//加到AI上
                    {
                        #region BUff添加

                        if (AiRole.buffList == null)
                        {
                            var buffDatas = new List<BuffData>();
                            buffDatas.Add(new BuffData
                            {
                                Name = dic[CurrentCardModel.EffectType.ToString()],
                                Num = CurrentCardModel.Effect,
                                EffectType = CurrentCardModel.EffectType,
                                BUFFType = 0
                            });
                            AiRole.buffList = buffDatas;
                            GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                            tempBuff = Common.AddChild(gameView.E_buffObj.transform, tempBuff);
                            tempBuff.name = "img_Buff_" + CurrentCardModel.EffectType;
                            var img = tempBuff.GetComponent<Image>();
                            Common.ImageBind(CurrentCardModel.CardUrl, img);
                            var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                            buffNum.text = (Convert.ToUInt32(buffNum.text) + CurrentCardModel.Effect).ToString();
                            BUFFManager.instance.BUFFApply(ref AiRole.buffList, ownRole.buffList, ref CurrentCardModel, ref AiRole.handCardList, true, 1);
                        }
                        else
                        {
                            if (!AiRole.buffList.Exists(a => a.EffectType == 9))//免疫状态下所有buff卡不可使用
                            {
                                var aa = AiRole.buffList.Find(a => a.Name == dic[CurrentCardModel.EffectType.ToString()]);//是否以存在此buff
                                if (aa != null)
                                {
                                    aa.Num += CurrentCardModel.Effect;
                                    //UI操作
                                    gameView.BUFFUIChange(AiRole.buffList, ref AiRole.handCardList, ref CurrentCardModel, 1);
                                }
                                else
                                {
                                    //一种类型只能存在一条。比如虚弱和愤怒不能共存
                                    var buffs = AiRole.buffList.Find(a => a.BUFFType == 0);
                                    if (buffs != null)
                                    {
                                        AiRole.buffList.Find(a => a.BUFFType == 0).Num = 0;
                                    }
                                    AiRole.buffList.Add(new BuffData
                                    {
                                        Name = dic[CurrentCardModel.EffectType.ToString()],
                                        Num = CurrentCardModel.Effect,
                                        EffectType = CurrentCardModel.EffectType,
                                        BUFFType = 0
                                    });
                                    GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                    tempBuff = Common.AddChild(gameView.E_buffObj.transform, tempBuff);
                                    tempBuff.name = "img_Buff_" + CurrentCardModel.EffectType;
                                    var img = tempBuff.GetComponent<Image>();
                                    Common.ImageBind(CurrentCardModel.CardUrl, img);
                                    var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                    buffNum.text = (Convert.ToUInt32(buffNum.text) + CurrentCardModel.Effect).ToString();
                                    //UI操作
                                    gameView.BUFFUIChange(AiRole.buffList, ref AiRole.handCardList, ref CurrentCardModel, 1);
                                    BUFFManager.instance.BUFFApply(ref AiRole.buffList, ownRole.buffList, ref CurrentCardModel, ref AiRole.handCardList, true, 1);
                                }
                            }
                        }
                        #endregion
                    }
                }
                #endregion
                #region 暴击等作用在攻击的Buff
                else if (CurrentCardModel.EffectType == 10)//暴击
                {
                    hasUseCard = true;
                    CardUseBeforeTriggerEvent(CurrentCardModel.UsingBeforeTriggerList);
                    Destroy(CurrentCard);
                    if (CurrentCardModel.Consume > 0)
                    {
                        Common.EnergyImgChange(ownRole.Energy, CurrentCardModel.Consume, 0, ownRole.EnergyMax);
                        ownRole.Energy -= CurrentCardModel.Consume;
                    }
                    if (CardToRoleID == ownRole.playerID)//加到角色上
                    {
                        #region BUff添加
                        if (ownRole.buffList == null)
                        {
                            var buffDatas = new List<BuffData>();
                            buffDatas.Add(new BuffData
                            {
                                Name = dic[CurrentCardModel.EffectType.ToString()],
                                Num = CurrentCardModel.Effect,
                                EffectType = CurrentCardModel.EffectType,
                                BUFFType = 0
                            });
                            ownRole.buffList = buffDatas;
                            GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                            tempBuff = Common.AddChild(gameView.P_buffObj.transform, tempBuff);
                            tempBuff.name = "img_Buff_" + CurrentCardModel.EffectType;
                            var img = tempBuff.GetComponent<Image>();
                            Common.ImageBind(CurrentCardModel.CardUrl, img);
                            var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                            buffNum.text = (Convert.ToUInt32(buffNum.text) + CurrentCardModel.Effect).ToString();
                        }
                        else
                        {
                            if (!ownRole.buffList.Exists(a => a.EffectType == 9))//免疫状态下所有buff卡不可使用
                            {
                                var aa = ownRole.buffList.Find(a => a.Name == dic[CurrentCardModel.EffectType.ToString()]);//是否以存在此buff
                                if (aa != null)
                                {
                                    aa.Num += CurrentCardModel.Effect;
                                    //UI操作
                                    gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref CurrentCardModel);
                                }
                                else
                                {
                                    var buffs = ownRole.buffList.Find(a => a.BUFFType == 0);
                                    if (buffs != null)
                                    {
                                        ownRole.buffList.Find(a => a.BUFFType == 0).Num = 0;
                                    }
                                    //一种类型只能存在一条。比如虚弱和愤怒不能共存
                                    ownRole.buffList.Add(new BuffData
                                    {
                                        Name = dic[CurrentCardModel.EffectType.ToString()],
                                        Num = CurrentCardModel.Effect,
                                        EffectType = CurrentCardModel.EffectType,
                                        BUFFType = 0
                                    });
                                    GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                    tempBuff = Common.AddChild(gameView.P_buffObj.transform, tempBuff);
                                    tempBuff.name = "img_Buff_" + CurrentCardModel.EffectType;
                                    var img = tempBuff.GetComponent<Image>();
                                    Common.ImageBind(CurrentCardModel.CardUrl, img);
                                    var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                    buffNum.text = (Convert.ToUInt32(buffNum.text) + CurrentCardModel.Effect).ToString();
                                }
                            }
                        }
                        #endregion
                    }
                    else if (CardToRoleID == AiRole.playerID)//加到AI上
                    {
                        #region BUff添加

                        if (AiRole.buffList == null)
                        {
                            var buffDatas = new List<BuffData>();
                            buffDatas.Add(new BuffData
                            {
                                Name = dic[CurrentCardModel.EffectType.ToString()],
                                Num = CurrentCardModel.Effect,
                                EffectType = CurrentCardModel.EffectType,
                                BUFFType = 0
                            });
                            AiRole.buffList = buffDatas;
                            GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                            tempBuff = Common.AddChild(gameView.E_buffObj.transform, tempBuff);
                            tempBuff.name = "img_Buff_" + CurrentCardModel.EffectType;
                            var img = tempBuff.GetComponent<Image>();
                            Common.ImageBind(CurrentCardModel.CardUrl, img);
                            var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                            buffNum.text = (Convert.ToUInt32(buffNum.text) + CurrentCardModel.Effect).ToString();
                        }
                        else
                        {
                            if (!AiRole.buffList.Exists(a => a.EffectType == 9))//免疫状态下所有buff卡不可使用
                            {
                                var aa = AiRole.buffList.Find(a => a.Name == dic[CurrentCardModel.EffectType.ToString()]);//是否以存在此buff
                                if (aa != null)
                                {
                                    aa.Num += CurrentCardModel.Effect;
                                    //UI操作
                                    gameView.BUFFUIChange(AiRole.buffList, ref AiRole.handCardList, ref CurrentCardModel, 1);
                                }
                                else
                                {
                                    var buffs = AiRole.buffList.Find(a => a.BUFFType == 0);
                                    if (buffs != null)
                                    {
                                        AiRole.buffList.Find(a => a.BUFFType == 0).Num = 0;
                                    }
                                    //一种类型只能存在一条。比如虚弱和愤怒不能共存
                                    AiRole.buffList.Add(new BuffData
                                    {
                                        Name = dic[CurrentCardModel.EffectType.ToString()],
                                        Num = CurrentCardModel.Effect,
                                        EffectType = CurrentCardModel.EffectType,
                                        BUFFType = 0
                                    });
                                    GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                    tempBuff = Common.AddChild(gameView.E_buffObj.transform, tempBuff);
                                    tempBuff.name = "img_Buff_" + CurrentCardModel.EffectType;
                                    var img = tempBuff.GetComponent<Image>();
                                    Common.ImageBind(CurrentCardModel.CardUrl, img);
                                    var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                    buffNum.text = (Convert.ToUInt32(buffNum.text) + CurrentCardModel.Effect).ToString();
                                }
                            }
                        }
                        #endregion
                    }
                }
                #endregion
                #region 免疫
                else if (CurrentCardModel.EffectType == 9)//免疫
                {
                    hasUseCard = true;
                    CardUseBeforeTriggerEvent(CurrentCardModel.UsingBeforeTriggerList);
                    Destroy(CurrentCard);
                    if (CurrentCardModel.Consume > 0)
                    {
                        Common.EnergyImgChange(ownRole.Energy, CurrentCardModel.Consume, 0, ownRole.EnergyMax);
                        ownRole.Energy -= CurrentCardModel.Consume;
                    }
                    if (CardToRoleID == ownRole.playerID)//加到角色上
                    {
                        #region BUFF添加
                        var aa = ownRole.buffList?.Find(a => a.Name == dic[CurrentCardModel.EffectType.ToString()]);//是否以存在此buff
                        if (aa != null)
                        {
                            aa.Num += CurrentCardModel.Effect;
                            //UI操作
                            gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref CurrentCardModel);
                        }
                        else
                        {
                            //除了免疫BUFF其他都BUFF都为0
                            ownRole.buffList?.ForEach(item =>
                            {
                                item.Num = 0;
                            });
                            ownRole.buffList.Add(new BuffData
                            {
                                Name = dic[CurrentCardModel.EffectType.ToString()],
                                Num = CurrentCardModel.Effect,
                                EffectType = CurrentCardModel.EffectType,
                                BUFFType = 3
                            });
                            GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                            tempBuff = Common.AddChild(gameView.P_buffObj.transform, tempBuff);
                            tempBuff.name = "img_Buff_" + CurrentCardModel.EffectType;
                            var img = tempBuff.GetComponent<Image>();
                            Common.ImageBind(CurrentCardModel.CardUrl, img);
                            var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                            buffNum.text = (Convert.ToUInt32(buffNum.text) + CurrentCardModel.Effect).ToString();
                            //UI操作
                            gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref CurrentCardModel);
                        }

                        #endregion
                    }
                    else if (CardToRoleID == AiRole.playerID)//加到AI上
                    {
                        #region BUFF添加
                        var aa = AiRole.buffList.Find(a => a.Name == dic[CurrentCardModel.EffectType.ToString()]);//是否以存在此buff
                        if (aa != null)
                        {
                            aa.Num += CurrentCardModel.Effect;
                            //UI操作
                            gameView.BUFFUIChange(AiRole.buffList, ref AiRole.handCardList, ref CurrentCardModel, 1);
                        }
                        else
                        {
                            //除了免疫BUFF其他都BUFF都为0
                            AiRole.buffList?.ForEach(item =>
                            {
                                item.Num = 0;
                            });
                            AiRole.buffList.Add(new BuffData
                            {
                                Name = dic[CurrentCardModel.EffectType.ToString()],
                                Num = CurrentCardModel.Effect,
                                EffectType = CurrentCardModel.EffectType,
                                BUFFType = 3
                            });
                            GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                            tempBuff = Common.AddChild(gameView.E_buffObj.transform, tempBuff);
                            tempBuff.name = "img_Buff_" + CurrentCardModel.EffectType;
                            var img = tempBuff.GetComponent<Image>();
                            Common.ImageBind(CurrentCardModel.CardUrl, img);
                            var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                            buffNum.text = (Convert.ToUInt32(buffNum.text) + CurrentCardModel.Effect).ToString();
                            //UI操作
                            gameView.BUFFUIChange(AiRole.buffList, ref AiRole.handCardList, ref CurrentCardModel, 1);
                        }
                        #endregion
                    }
                }
                #endregion
                #region 卡牌拖动到角色位置
                else if (BattleManager.instance.OwnPlayerData.Find(a => a.playerID == CardToRoleID) != null)//卡牌拖动到角色位置
                {
                    #region 防御
                    if (CurrentCardModel.EffectType == 2)//防御
                    {
                        hasUseCard = true;
                        CardUseBeforeTriggerEvent(CurrentCardModel.UsingBeforeTriggerList);
                        Destroy(CurrentCard);
                        if (CurrentCardModel.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CurrentCardModel.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CurrentCardModel.Consume;
                        }
                        gameView.Pimg_Armor.transform.localScale = Vector3.one;
                        ownRole.Armor += Convert.ToInt32(CurrentCardModel.Effect);
                        if (ownRole.Armor > ownRole.bloodMax)
                        {
                            ownRole.Armor = Convert.ToInt32(ownRole.bloodMax);
                        }
                        gameView.txt_P_Armor.text = ownRole.Armor.ToString();
                        EffectOn = "1";
                    }
                    #endregion
                }
                #endregion
                #region 卡牌拖动到敌人位置
                else if (BattleManager.instance.EnemyPlayerData.Find(a => a.playerID == CardToRoleID) != null)//卡牌拖动到敌人位置
                {
                    #region 攻击
                    if (CurrentCardModel.EffectType == 1)//攻击
                    {
                        hasUseCard = true;
                        CardUseBeforeTriggerEvent(CurrentCardModel.UsingBeforeTriggerList);
                        Destroy(CurrentCard);
                        if (CurrentCardModel.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CurrentCardModel.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CurrentCardModel.Consume;
                        }
                        int DeductionHp = Convert.ToInt32(CurrentCardModel.Effect);
                        if (AiRole.Armor > 0)
                        {
                            if (AiRole.Armor >= CurrentCardModel.Effect)
                            {
                                DeductionHp = 0;
                                AiRole.Armor -= Convert.ToInt32(CurrentCardModel.Effect);
                                gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                            }
                            else
                            {
                                DeductionHp -= AiRole.Armor;
                                AiRole.Armor = 0;
                            }
                            if (AiRole.Armor == 0)
                            {
                                gameView.Eimg_Armor.transform.localScale = Vector3.zero;
                            }
                        }
                        Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, DeductionHp, 0);
                        AiRole.bloodNow -= DeductionHp;
                        if (AiRole.bloodNow <= 0)
                        {
                            AiRole.bloodNow = 0;
                            //AI死亡
                            gameView.AiDie();
                        }
                        gameView.txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                        EffectOn = "0";
                    }
                    #endregion
                    #region 连续攻击
                    else if (CurrentCardModel.EffectType == 5)//连续攻击
                    {
                        hasUseCard = true;
                        CardUseBeforeTriggerEvent(CurrentCardModel.UsingBeforeTriggerList);
                        Destroy(CurrentCard);
                        if (CurrentCardModel.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CurrentCardModel.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CurrentCardModel.Consume;
                        }
                        for (int i = 0; i < CurrentCardModel.AtkNumber; i++)
                        {
                            var buffRes = BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref CurrentCardModel, ref ownRole.handCardList);
                            if (buffRes?.Count > 0)
                            {
                                if (buffResult.Exists(a => a.EffectType == 2 && a.HasValid == false))
                                {
                                    //无效果
                                }
                                else
                                {
                                    #region 卡牌效果
                                    int DeductionHp = Convert.ToInt32(CurrentCardModel.Effect);
                                    if (AiRole.Armor > 0)
                                    {
                                        if (AiRole.Armor >= CurrentCardModel.Effect)
                                        {
                                            DeductionHp = 0;
                                            AiRole.Armor -= Convert.ToInt32(CurrentCardModel.Effect);
                                            gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                                        }
                                        else
                                        {
                                            DeductionHp -= AiRole.Armor;
                                            AiRole.Armor = 0;
                                        }
                                        if (AiRole.Armor == 0)
                                        {
                                            gameView.Eimg_Armor.transform.localScale = Vector3.zero;
                                        }
                                    }
                                    Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, DeductionHp, 0);
                                    AiRole.bloodNow -= DeductionHp;
                                    if (AiRole.bloodNow <= 0)
                                    {
                                        AiRole.bloodNow = 0;
                                        //AI死亡
                                        gameView.AiDie();
                                    }
                                    gameView.txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";

                                    #endregion
                                }
                            }
                            else
                            {
                                #region 卡牌效果
                                int DeductionHp = Convert.ToInt32(CurrentCardModel.Effect);
                                if (AiRole.Armor > 0)
                                {
                                    if (AiRole.Armor >= CurrentCardModel.Effect)
                                    {
                                        DeductionHp = 0;
                                        AiRole.Armor -= Convert.ToInt32(CurrentCardModel.Effect);
                                        gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                                    }
                                    else
                                    {
                                        DeductionHp -= AiRole.Armor;
                                        AiRole.Armor = 0;
                                    }
                                    if (AiRole.Armor == 0)
                                    {
                                        gameView.Eimg_Armor.transform.localScale = Vector3.zero;
                                    }
                                }
                                Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, DeductionHp, 0);
                                AiRole.bloodNow -= DeductionHp;
                                if (AiRole.bloodNow <= 0)
                                {
                                    AiRole.bloodNow = 0;
                                    //AI死亡
                                    gameView.AiDie();
                                }
                                gameView.txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                                #endregion
                            }

                            EffectOn = "0";
                        }
                    }
                    #endregion
                    #region 攻击无视防御
                    else if (CurrentCardModel.EffectType == 7)//攻击无视防御
                    {
                        hasUseCard = true;
                        CardUseBeforeTriggerEvent(CurrentCardModel.UsingBeforeTriggerList);
                        Destroy(CurrentCard);
                        if (CurrentCardModel.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CurrentCardModel.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CurrentCardModel.Consume;
                        }
                        int DeductionHp = Convert.ToInt32(CurrentCardModel.Effect);
                        Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, DeductionHp, 0);
                        AiRole.bloodNow -= DeductionHp;
                        if (AiRole.bloodNow <= 0)
                        {
                            AiRole.bloodNow = 0;
                            //AI死亡
                            gameView.AiDie();
                        }
                        gameView.txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                        EffectOn = "0";
                    }
                    #endregion
                    #region 销毁防御
                    else if (CurrentCardModel.EffectType == 6)//销毁防御
                    {
                        hasUseCard = true;
                        CardUseBeforeTriggerEvent(CurrentCardModel.UsingBeforeTriggerList);
                        Destroy(CurrentCard);
                        if (CurrentCardModel.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CurrentCardModel.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CurrentCardModel.Consume;
                        }
                        if (AiRole.Armor > 0)
                        {
                            AiRole.Armor = 0;
                            gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                            gameView.Eimg_Armor.transform.localScale = Vector3.zero;
                        }
                    }
                    #endregion
                }
                #endregion
            }
        }
        if (hasUseCard)
        {
            CardUseAfterTriggerEvent(CurrentCardModel.TriggerAfterUsingList);
            DestroyImmediate(CurrentCard);

            //放入已使用牌堆
            var useCards = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUsedCardPoolsFileName) ?? new List<CurrentCardPoolModel>();
            useCards.Add(CurrentCardModel);
            Common.SaveTxtFile(useCards.ListToJson(), GlobalAttr.CurrentUsedCardPoolsFileName);
            //移除当前手牌
            var atkCards = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentATKBarCardPoolsFileName);
            atkCards.Remove(atkCards.Find(a => a.SingleID == CurrentCardModel.SingleID));
            Common.SaveTxtFile(atkCards.ListToJson(), GlobalAttr.CurrentATKBarCardPoolsFileName);

            #region 存储角色数据
            CurrentRoleModel playerRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName);
            playerRole.Armor = ownRole.Armor;
            playerRole.Energy = ownRole.Energy;
            playerRole.MaxEnergy = ownRole.EnergyMax;
            playerRole.HP = ownRole.bloodNow;
            playerRole.Wealth = ownRole.Wealth;
            Common.SaveTxtFile(playerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
            #endregion

            #region 存储AI数据
            CurrentRoleModel aiData = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentAIRoleFileName);
            aiData.Armor = AiRole.Armor;
            aiData.HP = AiRole.bloodNow;
            aiData.MaxHP = AiRole.bloodMax;
            Common.SaveTxtFile(aiData.ObjectToJson(), GlobalAttr.CurrentAIRoleFileName);
            #endregion
            gameView.txt_Right_Count.text = useCards == null ? "0" : useCards.Count.ToString();
        }
        else
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
        }
    }

    /// <summary>
    /// 卡牌使用前的触发事件
    /// </summary>
    public void CardUseBeforeTriggerEvent(List<TriggerAfterUsing> list)
    {
        var ownRole = BattleManager.instance.OwnPlayerData[0];
        var AiRole = BattleManager.instance.EnemyPlayerData[0];
        if (list != null && list?.Count > 0)
        {
            list = list.OrderBy(a => a.Sort).ToList();
            foreach (var item in list)
            {
                bool hasReachCondition = false;
                #region 是否达到触发条件
                if (item.TriggerCondition == 0)//无触发条件
                {
                    hasReachCondition = true;
                }
                else if (item.TriggerCondition == 2)//有护甲
                {
                    if (AiRole.Armor > 0)
                    {
                        hasReachCondition = true;
                    }
                }
                else if (item.TriggerCondition == 3)//满血
                {
                    //已经执行过了一次
                    if (AiRole.bloodNow + CurrentCardModel.Effect >= AiRole.bloodMax)
                    {
                        hasReachCondition = true;
                    }
                }
                else if (item.TriggerCondition == 4)//攻击力*2大与血量
                {
                    if (CurrentCardModel.Effect * 2 > AiRole.bloodNow)
                    {
                        hasReachCondition = true;
                    }
                }
                #endregion
                if (hasReachCondition)
                {
                    #region 防御
                    if (item.TriggerState == 2)//防御
                    {
                        gameView.Eimg_Armor.transform.localScale = Vector3.one;
                        AiRole.Armor += Convert.ToInt32(CurrentCardModel.Effect);
                        if (ownRole.Armor > ownRole.bloodMax)
                        {
                            ownRole.Armor = Convert.ToInt32(ownRole.bloodMax);
                        }
                        gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                    }
                    #endregion
                    #region 血量变化
                    else if (item.TriggerState == 3)//血量变化
                    {
                        if (CurrentCardModel.Effect < 0)
                        {
                            var effect = ownRole.bloodNow + Convert.ToInt32(CurrentCardModel.Effect);
                            if (effect < 0)
                            {
                                ownRole.bloodNow = 0;
                                //玩家死亡
                                gameView.PlayerDie();
                            }
                            else
                            {
                                Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, CurrentCardModel.Effect, 0);
                                ownRole.bloodNow += Convert.ToInt32(CurrentCardModel.Effect);
                            }
                        }
                        else
                        {
                            Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, CurrentCardModel.Effect, 1);
                            ownRole.bloodNow += Convert.ToInt32(CurrentCardModel.Effect);
                            if (ownRole.bloodNow > ownRole.bloodMax)
                            {
                                ownRole.bloodNow = ownRole.bloodMax;
                            }
                        }
                        gameView.txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
                    }
                    #endregion
                    #region 能量变化
                    else if (item.TriggerState == 4)//能量变化
                    {
                        if (CurrentCardModel.Effect < 0)//扣能量
                        {
                            var effect = ownRole.Energy + Convert.ToInt32(CurrentCardModel.Effect);
                            if (effect < 0)
                            {
                                Common.EnergyImgChange(ownRole.Energy, ownRole.Energy, 0, ownRole.EnergyMax);
                                ownRole.Energy = 0;
                            }
                            else
                            {
                                Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(CurrentCardModel.Effect), 0, ownRole.EnergyMax);
                                ownRole.Energy += Convert.ToInt32(CurrentCardModel.Effect);
                            }
                        }
                        else
                        {
                            Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(CurrentCardModel.Effect), 1, ownRole.EnergyMax);
                            ownRole.Energy += Convert.ToInt32(CurrentCardModel.Effect);
                            if (ownRole.Energy > ownRole.EnergyMax)
                            {
                                ownRole.Energy = ownRole.EnergyMax;
                            }
                        }
                    }
                    #endregion
                    #region 销毁防御
                    else if (item.TriggerState == 6)//销毁防御
                    {
                        if (AiRole.Armor > 0)
                        {
                            AiRole.Armor = 0;
                            gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                            gameView.Eimg_Armor.transform.localScale = Vector3.zero;
                        }
                    }
                    #endregion
                    #region 无视防御
                    else if (item.TriggerState == 7)//无视防御
                    {
                        CurrentCardModel.EffectType = 7;
                    }
                    #endregion
                    #region 攻击暴击
                    else if (item.TriggerState == 10)//暴击
                    {
                        CurrentCardModel.Effect = CurrentCardModel.Effect * 2;
                    }
                    #endregion
                    else if (item.TriggerState == 17)//移除一张手牌
                    {

                    }
                    #region 双倍伤害
                    else if (item.TriggerState == 18)//双倍伤害
                    {
                        CurrentCardModel.Effect = CurrentCardModel.Effect * 2;
                    }
                    #endregion
                }
            }
        }
    }

    /// <summary>
    /// 卡牌使用后的触发事件
    /// </summary>
    public void CardUseAfterTriggerEvent(List<TriggerAfterUsing> list)
    {
        var ownRole = BattleManager.instance.OwnPlayerData[0];
        var AiRole = BattleManager.instance.EnemyPlayerData[0];
        var unUseCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUnUsedCardPoolsFileName);
        var PlayerCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName);
        var PlayerHandCardList = BattleManager.instance.OwnPlayerData[0].handCardList;
        var atkBarCard = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentAIATKCardPoolsFileName);
        if (list != null && list?.Count > 0)
        {
            list = list.OrderBy(a => a.Sort).ToList();
            foreach (var item in list)
            {
                bool hasReachCondition = false;
                #region 是否达到触发条件
                if (item.TriggerCondition == 0)//无触发条件
                {
                    hasReachCondition = true;
                }
                else if (item.TriggerCondition == 1)//斩杀
                {
                    if (AiRole.bloodNow <= 0)
                    {
                        hasReachCondition = true;
                    }
                }
                else if (item.TriggerCondition == 2)//有护甲
                {
                    if (AiRole.Armor > 0)
                    {
                        hasReachCondition = true;
                    }
                }
                #endregion
                if (hasReachCondition)
                {
                    #region 攻击
                    if (item.TriggerState == 1)//攻击
                    {
                        int DeductionHp = Convert.ToInt32(item.TriggerValue);
                        if (AiRole.Armor > 0)
                        {
                            if (AiRole.Armor >= item.TriggerValue)
                            {
                                DeductionHp = 0;
                                AiRole.Armor -= Convert.ToInt32(item.TriggerValue);
                                gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                            }
                            else
                            {
                                DeductionHp -= AiRole.Armor;
                                AiRole.Armor = 0;
                            }
                            if (AiRole.Armor == 0)
                            {
                                gameView.Eimg_Armor.transform.localScale = Vector3.zero;
                            }
                        }
                        Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, DeductionHp, 0);
                        AiRole.bloodNow -= DeductionHp;
                        if (AiRole.bloodNow <= 0)
                        {
                            AiRole.bloodNow = 0;
                            //AI死亡
                            gameView.AiDie();
                        }
                        gameView.txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                    }
                    #endregion
                    #region 防御
                    else if (item.TriggerState == 2)//防御
                    {
                        gameView.Eimg_Armor.transform.localScale = Vector3.one;
                        AiRole.Armor += Convert.ToInt32(item.TriggerValue);
                        if (ownRole.Armor > ownRole.bloodMax)
                        {
                            ownRole.Armor = Convert.ToInt32(ownRole.bloodMax);
                        }
                        gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                    }
                    #endregion
                    #region 血量变化
                    else if (item.TriggerState == 3)//血量变化
                    {
                        if (item.TriggerValue < 0)
                        {
                            var effect = ownRole.bloodNow + Convert.ToInt32(item.TriggerValue);
                            if (effect < 0)
                            {
                                ownRole.bloodNow = 0;
                                //玩家死亡
                                gameView.PlayerDie();
                            }
                            else
                            {
                                Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, item.TriggerValue, 0);
                                ownRole.bloodNow += Convert.ToInt32(item.TriggerValue);
                            }
                        }
                        else
                        {
                            Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, item.TriggerValue, 1);
                            ownRole.bloodNow += Convert.ToInt32(item.TriggerValue);
                            if (ownRole.bloodNow > ownRole.bloodMax)
                            {
                                ownRole.bloodNow = ownRole.bloodMax;
                            }
                        }
                        gameView.txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
                    }
                    #endregion
                    #region 能量变化
                    else if (item.TriggerState == 4)//能量变化
                    {
                        if (item.TriggerValue < 0)//扣能量
                        {
                            var effect = ownRole.Energy + Convert.ToInt32(item.TriggerValue);
                            if (effect < 0)
                            {
                                Common.EnergyImgChange(ownRole.Energy, ownRole.Energy, 0, ownRole.EnergyMax);
                                ownRole.Energy = 0;
                            }
                            else
                            {
                                Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(item.TriggerValue), 0, ownRole.EnergyMax);
                                ownRole.Energy += Convert.ToInt32(item.TriggerValue);
                            }
                        }
                        else
                        {
                            Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(item.TriggerValue), 1, ownRole.EnergyMax);
                            ownRole.Energy += Convert.ToInt32(item.TriggerValue);
                            if (ownRole.Energy > ownRole.EnergyMax)
                            {
                                ownRole.Energy = ownRole.EnergyMax;
                            }
                        }
                    }
                    #endregion
                    #region 连续攻击
                    else if (item.TriggerState == 5)//连续攻击
                    {
                        for (int i = 0; i < CurrentCardModel.AtkNumber; i++)
                        {
                            int DeductionHp = Convert.ToInt32(item.TriggerValue);
                            if (AiRole.Armor > 0)
                            {
                                if (AiRole.Armor >= item.TriggerValue)
                                {
                                    DeductionHp = 0;
                                    AiRole.Armor -= Convert.ToInt32(item.TriggerValue);
                                    gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                                }
                                else
                                {
                                    DeductionHp -= AiRole.Armor;
                                    AiRole.Armor = 0;
                                }
                                if (AiRole.Armor == 0)
                                {
                                    gameView.Eimg_Armor.transform.localScale = Vector3.zero;
                                }
                            }
                            Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, DeductionHp, 0);
                            AiRole.bloodNow -= DeductionHp;
                            if (AiRole.bloodNow <= 0)
                            {
                                AiRole.bloodNow = 0;
                                //AI死亡
                                gameView.AiDie();
                            }
                            gameView.txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                        }
                    }
                    #endregion
                    #region 销毁防御
                    else if (item.TriggerState == 6)//销毁防御
                    {
                        if (AiRole.Armor > 0)
                        {
                            AiRole.Armor = 0;
                            gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                            gameView.Eimg_Armor.transform.localScale = Vector3.zero;
                        }
                    }
                    #endregion
                    #region 无视防御攻击
                    else if (item.TriggerState == 7)//无视防御攻击
                    {
                        int DeductionHp = Convert.ToInt32(item.TriggerValue);
                        Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, DeductionHp, 0);
                        AiRole.bloodNow -= DeductionHp;
                        if (AiRole.bloodNow <= 0)
                        {
                            AiRole.bloodNow = 0;
                            //AI死亡
                            gameView.AiDie();
                        }
                        gameView.txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                    }
                    #endregion
                    #region 愤怒、虚弱、暴击等buff
                    else if (item.TriggerState == 8 || item.TriggerState == 10 || item.TriggerState == 11)//愤怒
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        Common.DicDataRead(ref dic, GlobalAttr.EffectTypeFileName, "Card");
                        #region BUff添加
                        if (ownRole.buffList == null)
                        {
                            var buffDatas = new List<BuffData>();
                            buffDatas.Add(new BuffData
                            {
                                Name = dic[item.TriggerState.ToString()],
                                Num = item.TriggerValue
                            });
                            ownRole.buffList = buffDatas;
                            GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                            tempBuff = Common.AddChild(gameView.P_buffObj.transform, tempBuff);
                            tempBuff.name = "img_Buff_" + item.TriggerState;
                            var img = tempBuff.GetComponent<Image>();
                            Common.ImageBind(CurrentCardModel.CardUrl, img);
                            var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                            buffNum.text = (Convert.ToUInt32(buffNum.text) + item.TriggerValue).ToString();
                        }
                        else
                        {
                            if (!ownRole.buffList.Exists(a => a.EffectType == 9))//免疫状态下所有buff卡不可使用
                            {
                                var aa = ownRole.buffList.Find(a => a.Name == dic[item.TriggerState.ToString()]);//是否以存在此buff
                                if (aa != null)
                                {
                                    aa.Num += item.TriggerValue;
                                }
                                else
                                {
                                    ownRole.buffList.Add(new BuffData
                                    {
                                        Name = dic[item.TriggerState.ToString()],
                                        Num = item.TriggerValue
                                    });
                                    GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                    tempBuff = Common.AddChild(gameView.P_buffObj.transform, tempBuff);
                                    tempBuff.name = "img_Buff_" + item.TriggerState;
                                    var img = tempBuff.GetComponent<Image>();
                                    Common.ImageBind(CurrentCardModel.CardUrl, img);
                                    var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                    buffNum.text = (Convert.ToUInt32(buffNum.text) + item.TriggerValue).ToString();
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion
                    #region 免疫BUFF
                    else if (item.TriggerState == 9)//免疫
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        Common.DicDataRead(ref dic, GlobalAttr.EffectTypeFileName, "Card");
                        #region BUff添加
                        if (ownRole.buffList == null)
                        {
                            var buffDatas = new List<BuffData>();
                            buffDatas.Add(new BuffData
                            {
                                Name = dic[item.TriggerState.ToString()],
                                Num = item.TriggerValue
                            });
                            ownRole.buffList = buffDatas;
                            GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                            tempBuff = Common.AddChild(gameView.P_buffObj.transform, tempBuff);
                            tempBuff.name = "img_Buff_" + item.TriggerState;
                            var img = tempBuff.GetComponent<Image>();
                            Common.ImageBind(CurrentCardModel.CardUrl, img);
                            var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                            buffNum.text = (Convert.ToUInt32(buffNum.text) + item.TriggerValue).ToString();
                        }
                        else
                        {
                            var aa = ownRole.buffList.Find(a => a.Name == dic[item.TriggerState.ToString()]);//是否以存在此buff
                            if (aa != null)
                            {
                                aa.Num += item.TriggerValue;
                            }
                            else
                            {
                                ownRole.buffList.Add(new BuffData
                                {
                                    Name = dic[item.TriggerState.ToString()],
                                    Num = item.TriggerValue
                                });
                                GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                tempBuff = Common.AddChild(gameView.P_buffObj.transform, tempBuff);
                                tempBuff.name = "img_Buff_" + item.TriggerState;
                                var img = tempBuff.GetComponent<Image>();
                                Common.ImageBind(CurrentCardModel.CardUrl, img);
                                var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                buffNum.text = (Convert.ToUInt32(buffNum.text) + item.TriggerValue).ToString();
                            }
                        }
                        #endregion
                    }
                    #endregion
                    else if (item.TriggerState == 12)//卡牌攻击力叠加
                    {

                    }
                    #region 抽所有卡
                    else if (item.TriggerState == 13)//抽所有卡
                    {
                        PlayerCardList.ListRandom();
                        for (int i = 0; i < item.TriggerValue; i++)
                        {
                            PlayerCardList[i].SingleID = 100001 + PlayerHandCardList.Count;
                            unUseCardList.Add(PlayerCardList[i]);
                            PlayerHandCardList.Add(PlayerCardList[i]);
                        }
                        Common.SaveTxtFile(unUseCardList.ListToJson(), GlobalAttr.CurrentUnUsedCardPoolsFileName);
                        Common.SaveTxtFile(PlayerHandCardList.ListToJson(), GlobalAttr.CurrentCardPoolsFileName);
                    }
                    #endregion
                    #region 抽攻击卡
                    else if (item.TriggerState == 14)//抽攻击卡
                    {
                        var aktCardList = PlayerCardList.FindAll(a => a.CardType == 0).ListRandom();
                        for (int i = 0; i < item.TriggerValue; i++)
                        {
                            aktCardList[i].SingleID = 100001 + PlayerHandCardList.Count;
                            unUseCardList.Add(aktCardList[i]);
                            PlayerHandCardList.Add(aktCardList[i]);
                        }
                        Common.SaveTxtFile(unUseCardList.ListToJson(), GlobalAttr.CurrentUnUsedCardPoolsFileName);
                        Common.SaveTxtFile(PlayerHandCardList.ListToJson(), GlobalAttr.CurrentCardPoolsFileName);

                    }
                    #endregion
                    #region 抽功能卡
                    else if (item.TriggerState == 15)//抽功能卡
                    {
                        var CardList = PlayerCardList.FindAll(a => a.CardType == 1).ListRandom();
                        for (int i = 0; i < item.TriggerValue; i++)
                        {
                            CardList[i].SingleID = 100001 + PlayerHandCardList.Count;
                            unUseCardList.Add(CardList[i]);
                            PlayerHandCardList.Add(CardList[i]);
                        }
                        Common.SaveTxtFile(unUseCardList.ListToJson(), GlobalAttr.CurrentUnUsedCardPoolsFileName);
                        Common.SaveTxtFile(PlayerHandCardList.ListToJson(), GlobalAttr.CurrentCardPoolsFileName);
                    }
                    #endregion
                    #region 移除当前卡
                    else if (item.TriggerState == 16)//移除当前卡
                    {
                        PlayerHandCardList.Remove(CurrentCardModel);
                        atkBarCard.Remove(CurrentCardModel);
                        Common.SaveTxtFile(unUseCardList.ListToJson(), GlobalAttr.CurrentUnUsedCardPoolsFileName);
                        Common.SaveTxtFile(PlayerHandCardList.ListToJson(), GlobalAttr.CurrentCardPoolsFileName);
                    }
                    #endregion
                    else if (item.TriggerState == 17)//移除一张手牌
                    {

                    }
                    #region 斩杀
                    else if (item.TriggerState == 19)//斩杀
                    {
                        AiRole.bloodNow = 0;
                        Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, AiRole.bloodNow, 0);
                        //AI死亡
                        gameView.AiDie();
                        gameView.txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                    }
                    #endregion
                }
            }
        }
    }

}
