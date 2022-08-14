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
    public GameObject PrevoiousCard;//上一张被移动的卡
    public int CardToRoleID;//当前卡移动到哪个角色下
    public CurrentCardPoolModel CurrentCardModel;//当前卡数据
    public CurrentCardPoolModel PrevoiousCardModel;//上一张卡数据
    public bool CrtCardChange = false;//当前卡已变化
    public bool CardUseBeforeResult = true;//卡牌使用前
    public bool UseCopyCard = false;//使用了复制卡牌
    public bool CopyBoxCardExist = false;//复制框是否有物体
    public float SuperPositionEffect = 0;//叠加的攻击力
    public bool ActivateSuperPos = false;//激活叠加攻击力
    public bool HasNumberValueChange = false;//数值变化
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
            Destroy(CurrentCard);
            if (CurrentCardModel.Consume > 0)
            {
                Common.EnergyImgChange(ownRole.Energy, CurrentCardModel.Consume, 0, ownRole.EnergyMax);
                ownRole.Energy -= CurrentCardModel.Consume;
            }
            gameView.obj_RemoveCard.SetActive(true);
        }
        #endregion
        #region AI每次攻击手牌攻击力+1
        else if (CurrentCardModel.EffectType == 13)//AI每次攻击手牌攻击力+1
        {
            hasUseCard = true;
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
                        //攻击结束后、更新一遍buffUI操作
                        gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref CurrentCardModel);
                    }
                    #endregion
                    #region 卡牌攻击力叠加
                    else if (CurrentCardModel.EffectType == 14)//卡牌攻击力叠加
                    {
                        #region 攻击
                        Destroy(CurrentCard);
                        if (CurrentCardModel.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CurrentCardModel.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CurrentCardModel.Consume;
                        }
                        if (SuperPositionEffect < CurrentCardModel.InitEffect)
                        {
                            SuperPositionEffect = CurrentCardModel.InitEffect;
                        }
                        int DeductionHp = Convert.ToInt32(SuperPositionEffect);
                        if (AiRole.Armor > 0)
                        {
                            if (AiRole.Armor >= SuperPositionEffect)
                            {
                                DeductionHp = 0;
                                AiRole.Armor -= Convert.ToInt32(SuperPositionEffect);
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
                        //攻击结束后、更新一遍buffUI操作
                        gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref CurrentCardModel);
                        #endregion

                        var cardData = ownRole.handCardList.Find(a => a.SingleID == CurrentCardModel.SingleID);
                        ActivateSuperPos = true;
                        SuperPositionEffect = cardData.InitEffect;
                        cardData.Effect = cardData.InitEffect;
                        hasUseCard = true;
                    }
                    #endregion
                    #region 连续攻击
                    else if (CurrentCardModel.EffectType == 5)//连续攻击
                    {
                        hasUseCard = true;
                        Destroy(CurrentCard);
                        if (CurrentCardModel.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CurrentCardModel.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CurrentCardModel.Consume;
                        }
                        EffectOn = "0";
                    }
                    #endregion
                    #region 攻击无视防御
                    else if (CurrentCardModel.EffectType == 7)//攻击无视防御
                    {
                        hasUseCard = true;
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

    }

    /// <summary>
    /// 卡牌使用前的触发事件
    /// </summary>
    /// <param name="model">所使用的的卡牌数据</param>
    /// <returns>是否触发成功</returns>
    public bool CardUseBeforeTriggerEvent(CurrentCardPoolModel model)
    {
        bool result = true;
        var ownRole = BattleManager.instance.OwnPlayerData[0];
        var AiRole = BattleManager.instance.EnemyPlayerData[0];
        var list = model.UsingBeforeTriggerList;
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
                    if (AiRole.bloodNow == AiRole.bloodMax)
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
                    switch (item.TriggerState)
                    {
                        #region 防御
                        case 2:
                            gameView.Eimg_Armor.transform.localScale = Vector3.one;
                            AiRole.Armor += Convert.ToInt32(CurrentCardModel.Effect);
                            if (ownRole.Armor > ownRole.bloodMax)
                            {
                                ownRole.Armor = Convert.ToInt32(ownRole.bloodMax);
                            }
                            gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                            break;
                        #endregion
                        #region 血量变化
                        case 3:
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
                            break;
                        #endregion
                        #region 能量变化
                        case 4:
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
                            break;
                        #endregion
                        #region 摧毁防御
                        case 6:
                            if (AiRole.Armor > 0)
                            {
                                AiRole.Armor = 0;
                                gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                                gameView.Eimg_Armor.transform.localScale = Vector3.zero;
                            }
                            break;
                        #endregion
                        #region 无视防御
                        case 7:
                            CurrentCardModel.EffectType = 7;
                            break;
                        #endregion
                        #region 伤害翻倍
                        case 10:
                        case 18:
                        case 19:
                            HasNumberValueChange = true;
                            CurrentCardModel.Effect = CurrentCardModel.Effect * 2;
                            break;
                        #endregion
                        #region 移除一张手牌
                        case 17:
                            break;
                        #endregion
                        default:
                            break;
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 卡牌使用后的触发事件
    /// </summary>
    /// <param name="model"></param>
    /// <param name="DrawACard">所抽的卡列表</param>
    /// <returns>0没有动画、1卡牌回收、2攻击和卡牌回收、3防御和卡牌回收、4血量恢复和卡牌回收、5能量恢复和卡牌回收、6移除卡牌,7抽卡动画</returns>
    public int CardUseAfterTriggerEvent(CurrentCardPoolModel model, ref string EffectOn, ref List<CurrentCardPoolModel> DrawACard)
    {
        int result = 1;
        PlayerData nowPlayer = BattleManager.instance.GetCurrentPlayerData();//当前出手的角色
        int PlayerOrAI = 0;
        switch (nowPlayer.playerType)
        {
            case PlayerType.OwnHuman:
                break;
            case PlayerType.NormalRobot:
            case PlayerType.AiRobot:
                PlayerOrAI = 1;
                break;
            case PlayerType.OtherHuman:
                break;
        }
        bool hasPutInUseCards = true;//是否放入已使用的牌堆
        var list = model.TriggerAfterUsingList;
        var ownRole = BattleManager.instance.OwnPlayerData[0];
        var AiRole = BattleManager.instance.EnemyPlayerData[0];
        var unUseCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUnUsedCardPoolsFileName);
        var PlayerCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName);
        var PlayerHandCardList = BattleManager.instance.OwnPlayerData[0].handCardList;
        var atkBarCard = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentATKBarCardPoolsFileName);
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
                                Num = item.TriggerValue,
                                EffectType = item.TriggerState,
                                BUFFType = 3
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
                                    Num = item.TriggerValue,
                                    EffectType = item.TriggerState,
                                    BUFFType = 3
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
                            DrawACard.Add(PlayerCardList[i]);
                        }
                        gameView.txt_Left_Count.text = unUseCardList.Count.ToString();
                        Common.SaveTxtFile(unUseCardList.ListToJson(), GlobalAttr.CurrentUnUsedCardPoolsFileName);
                        result = 7;
                    }
                    #endregion
                    #region 抽攻击卡到未使用卡池中
                    else if (item.TriggerState == 14)//抽攻击卡
                    {
                        var aktCardList = PlayerCardList.FindAll(a => a.CardType == 0).ListRandom();
                        for (int i = 0; i < item.TriggerValue; i++)
                        {
                            aktCardList[i].SingleID = 100001 + PlayerHandCardList.Count;
                            unUseCardList.Add(aktCardList[i]);
                            PlayerHandCardList.Add(aktCardList[i]);
                            DrawACard.Add(aktCardList[i]);
                        }
                        gameView.txt_Left_Count.text = unUseCardList.Count.ToString();
                        Common.SaveTxtFile(unUseCardList.ListToJson(), GlobalAttr.CurrentUnUsedCardPoolsFileName);
                        result = 7;
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
                            DrawACard.Add(CardList[i]);
                        }
                        gameView.txt_Left_Count.text = unUseCardList.Count.ToString();
                        Common.SaveTxtFile(unUseCardList.ListToJson(), GlobalAttr.CurrentUnUsedCardPoolsFileName);
                        result = 7;
                    }
                    #endregion
                    #region 移除当前卡
                    else if (item.TriggerState == 16)//移除当前卡
                    {
                        result = 6;
                        //当局手牌变化
                        hasPutInUseCards = false;
                        PlayerHandCardList.Remove(CurrentCardModel);
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

        if (PlayerOrAI == 0)
        {
            if (CrtCardChange)
            {
                DestroyImmediate(PrevoiousCard);
                CrtCardChange = false;
                //放入已使用牌堆
                if (hasPutInUseCards)
                {
                    var useCards = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUsedCardPoolsFileName) ?? new List<CurrentCardPoolModel>();
                    useCards.Add(PrevoiousCardModel);
                    Common.SaveTxtFile(useCards.ListToJson(), GlobalAttr.CurrentUsedCardPoolsFileName);
                    gameView.txt_Right_Count.text = useCards == null ? "0" : useCards.Count.ToString();
                }
                //移除当前手牌
                atkBarCard.Remove(atkBarCard.Find(a => a.SingleID == PrevoiousCardModel.SingleID));
                Common.SaveTxtFile(atkBarCard.ListToJson(), GlobalAttr.CurrentATKBarCardPoolsFileName);
            }
            else
            {
                DestroyImmediate(CurrentCard);
                //放入已使用牌堆
                if (hasPutInUseCards)
                {
                    var useCards = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUsedCardPoolsFileName) ?? new List<CurrentCardPoolModel>();
                    useCards.Add(CurrentCardModel);
                    Common.SaveTxtFile(useCards.ListToJson(), GlobalAttr.CurrentUsedCardPoolsFileName);
                    gameView.txt_Right_Count.text = useCards == null ? "0" : useCards.Count.ToString();
                }
                //移除当前手牌
                var removeModel = atkBarCard.Find(a => a.SingleID == CurrentCardModel.SingleID);
                atkBarCard.Remove(removeModel);
                Common.SaveTxtFile(atkBarCard.ListToJson(), GlobalAttr.CurrentATKBarCardPoolsFileName);
            }
        }
        else
        {
            result = 0;
        }

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
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
        return result;
    }

    /// <summary>
    /// 连续攻击数值效果体现
    /// </summary>
    /// <param name="model">当前攻击卡数据</param>
    /// <param name="ownRole">玩家数据</param>
    /// <param name="AiRole">AI数据</param>
    /// <param name="crtCard">当前被使用的卡</param>
    /// <param name="PlayerOrAI">谁发起的攻击。0玩家，1AI</param>
    public void ComboATK(CurrentCardPoolModel model, PlayerData ownRole, PlayerData AiRole, int PlayerOrAI = 0)
    {
        if (PlayerOrAI == 0)
        {
            var buffRes = BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref CurrentCardModel, ref ownRole.handCardList);
            if (buffRes?.Count > 0)
            {
                if (buffRes.Exists(a => a.EffectType == 2 && a.HasValid == false))
                {
                    //无效果
                    return;
                }
            }
            #region 卡牌效果
            int DeductionHp = Convert.ToInt32(model.Effect);
            if (AiRole.Armor > 0)
            {
                if (AiRole.Armor >= model.Effect)
                {
                    DeductionHp = 0;
                    AiRole.Armor -= Convert.ToInt32(model.Effect);
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
        else
        {
            #region 卡牌效果
            int DeductionHp = Convert.ToInt32(model.Effect);
            if (ownRole.Armor > 0)
            {
                if (ownRole.Armor >= model.Effect)
                {
                    DeductionHp = 0;
                    ownRole.Armor -= Convert.ToInt32(model.Effect);
                    gameView.txt_P_Armor.text = ownRole.Armor.ToString();
                }
                else
                {
                    DeductionHp -= ownRole.Armor;
                    ownRole.Armor = 0;
                }
                if (ownRole.Armor == 0)
                {
                    gameView.Pimg_Armor.transform.localScale = Vector3.zero;
                }
            }
            Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, DeductionHp, 0);
            ownRole.bloodNow -= DeductionHp;
            if (ownRole.bloodNow <= 0)
            {
                ownRole.bloodNow = 0;
                //玩家死亡
                gameView.PlayerDie();
            }
            gameView.txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
            #endregion
        }

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
    }

}
