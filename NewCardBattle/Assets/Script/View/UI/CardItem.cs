using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 卡牌触发事件
/// </summary>
public class CardItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Vector3 InitVector;
    GameObject MagnifyObj, thisObj, gameViewObj, P_buffObj, E_buffObj, obj_RemoveCard;
    CurrentCardPoolModel model = new CurrentCardPoolModel();//卡牌数据
    Image Pimg_HP, Eimg_HP, Pimg_Armor, Eimg_Armor;
    Text txt_P_HP, txt_E_HP, txt_P_Armor, txt_E_Armor, txt_Right_Count;
    public CurrentCardPoolModel BasisData;
    RectTransform thisParent;
    Vector3 CardPos;//卡牌当前位置
    private void Start()
    {
        gameViewObj = GameObject.Find("GameView(Clone)");
        Pimg_HP = gameViewObj.transform.Find("UI/Player/Pimg_HP").GetComponent<Image>();
        Eimg_HP = gameViewObj.transform.Find("UI/Enemy/Eimg_HP").GetComponent<Image>();
        Pimg_Armor = gameViewObj.transform.Find("UI/Player/img_Armor").GetComponent<Image>();
        Eimg_Armor = gameViewObj.transform.Find("UI/Enemy/img_Armor").GetComponent<Image>();
        txt_P_HP = gameViewObj.transform.Find("UI/Player/Text").GetComponent<Text>();
        txt_E_HP = gameViewObj.transform.Find("UI/Enemy/Text").GetComponent<Text>();
        txt_P_Armor = gameViewObj.transform.Find("UI/Player/img_Armor/Text").GetComponent<Text>();
        txt_E_Armor = gameViewObj.transform.Find("UI/Enemy/img_Armor/Text").GetComponent<Text>();
        txt_Right_Count = gameViewObj.transform.Find("UI/CardPools/right_Card/txt_EndCardCount").GetComponent<Text>();
        MagnifyObj = gameViewObj.transform.Find("UI/CardPools/obj_Magnify").gameObject;
        P_buffObj = gameViewObj.transform.Find("UI/Player/BuffBar").gameObject;
        E_buffObj = gameViewObj.transform.Find("UI/Enemy/BuffBar").gameObject;
        obj_RemoveCard = gameViewObj.transform.Find("UI/CardPools/obj_RemoveCard").gameObject;

        InitVector = transform.position;
        thisObj = transform.parent.Find(name).gameObject;
        thisParent = thisObj.transform.parent.GetComponent<RectTransform>();
        var singleID = name.Split('_')[1];
        if (singleID.Contains("Clone"))
        {
            singleID = singleID.Remove(singleID.IndexOf("(Clone)"));
        }
        model = BattleManager.instance.OwnPlayerData[0].handCardList.Find(a => a.SingleID == Convert.ToInt32(singleID));

    }
    /// <summary>
    /// 鼠标进入时触发
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.TurnStart)
        {
            HideCardDetail();
            thisObj = transform.parent.Find(name).gameObject;
            thisObj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            MagnifyObj = GameObject.Find("GameView(Clone)/UI/CardPools/obj_Magnify");
            thisObj = Common.AddChild(MagnifyObj.transform, thisObj);
            MagnifyObj.transform.GetChild(0).transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
    }

    /// <summary>
    /// 鼠标离开时触发
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.TurnStart)
        {
            HideCardDetail();
            thisObj = transform.parent.Find(name).gameObject;
            thisObj.transform.localScale = Vector3.one;
            HideMagnifyCard();
        }
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.Control);
        //if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.TurnStart)
        //{
        //    HideCardDetail();
        //    HideMagnifyCard();
        //    if (model.EffectType != 0)//无效果。黑卡
        //    {
        //        BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.Control);
        //    }
        //}
    }
    /// <summary>
    /// 拖拽中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.Control)
        {
            thisObj = transform.parent.Find(name).gameObject;
            thisObj.transform.position = Input.mousePosition;
        }
    }
    /// <summary>
    /// 拖拽结束
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.Control)
        {
            bool hasUseCard = false;//卡牌是否使用了
            var roleID = CardInRolePosition(eventData.position);
            var ownRole = BattleManager.instance.OwnPlayerData[0];
            var AiRole = BattleManager.instance.EnemyPlayerData[0];
            BUFFApplyToCardData(ownRole.buffList);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Common.DicDataRead(ref dic, GlobalAttr.EffectTypeFileName, "Card");
            //能量不足无法使用卡牌
            if (ownRole.Energy < model.Consume)
            {
                BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                transform.position = InitVector;
                LayoutRebuilder.ForceRebuildLayoutImmediate(thisParent);
                return;
            }
            #region 卡牌拖动到任意位置
            #region 血量变化
            if (model.EffectType == 3)//血量变化
            {
                hasUseCard = true;
                CardUseBeforeTriggerEvent(model.UsingBeforeTriggerList);
                if (model.Consume > 0)
                {
                    Common.EnergyImgChange(ownRole.Energy, model.Consume, 0, ownRole.EnergyMax);
                    ownRole.Energy -= model.Consume;
                }
                if (model.Effect < 0)
                {
                    var effect = ownRole.bloodNow + Convert.ToInt32(model.Effect);
                    if (effect < 0)//血量不足以使用此卡
                    {
                        hasUseCard = false;
                    }
                    else
                    {
                        Destroy(thisObj);
                        Common.HPImageChange(Pimg_HP, ownRole.bloodMax, model.Effect, 0);
                        ownRole.bloodNow += Convert.ToInt32(model.Effect);
                    }
                }
                else
                {
                    Destroy(thisObj);
                    if (ownRole.bloodNow != ownRole.bloodMax)
                    {
                        var changeHP = model.Effect;
                        if (ownRole.bloodNow + model.Effect > ownRole.bloodMax)
                        {
                            changeHP = ownRole.bloodMax - ownRole.bloodNow;
                            ownRole.bloodNow = ownRole.bloodMax;
                        }
                        else
                        {
                            ownRole.bloodNow += Convert.ToInt32(changeHP);
                        }
                        Common.HPImageChange(Pimg_HP, ownRole.bloodMax, changeHP, 1);
                    }
                }
                txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
            }
            #endregion
            #region 能量变化
            else if (model.EffectType == 4)//能量变化
            {
                hasUseCard = true;
                CardUseBeforeTriggerEvent(model.UsingBeforeTriggerList);
                if (model.Effect < 0)//扣能量
                {
                    var effect = ownRole.Energy + Convert.ToInt32(model.Effect);
                    if (effect < 0)//能量不足以使用此卡
                    {
                        hasUseCard = false;
                    }
                    else
                    {
                        Destroy(thisObj);
                        Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(model.Effect), 0, ownRole.EnergyMax);
                        ownRole.Energy += Convert.ToInt32(model.Effect);
                    }
                }
                else
                {
                    Destroy(thisObj);
                    Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(model.Effect), 1, ownRole.EnergyMax);
                    ownRole.Energy += Convert.ToInt32(model.Effect);
                    if (ownRole.Energy > ownRole.EnergyMax)
                    {
                        ownRole.Energy = ownRole.EnergyMax;
                    }
                }
            }
            #endregion
            #region 复制卡
            else if (model.EffectType == 12)//复制卡
            {
                hasUseCard = true;
                CardUseBeforeTriggerEvent(model.UsingBeforeTriggerList);
                Destroy(thisObj);
                obj_RemoveCard.SetActive(true);
            }
            #endregion
            #region AI每次攻击手牌攻击力+1
            else if (model.EffectType == 13)//AI每次攻击手牌攻击力+1
            {
                hasUseCard = true;
                CardUseBeforeTriggerEvent(model.UsingBeforeTriggerList);
            }
            #endregion
            #region 卡牌攻击力叠加
            else if (model.EffectType == 14)//卡牌攻击力叠加
            {
                hasUseCard = true;
                CardUseBeforeTriggerEvent(model.UsingBeforeTriggerList);
            }
            #endregion
            #endregion
            if (!hasUseCard)
            {
                if (roleID == 0)
                {
                    BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                    transform.position = InitVector;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(thisParent);
                    return;
                }
                else
                {
                    #region 愤怒、虚弱等作用在数值的Buff
                    if (model.EffectType == 8 || model.EffectType == 11)//愤怒
                    {
                        hasUseCard = true;
                        CardUseBeforeTriggerEvent(model.UsingBeforeTriggerList);
                        Destroy(thisObj);
                        if (model.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, model.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= model.Consume;
                        }
                        if (roleID == ownRole.playerID)//加到角色上
                        {
                            #region BUff添加
                            if (ownRole.buffList == null)
                            {
                                var buffDatas = new List<BuffData>();
                                buffDatas.Add(new BuffData
                                {
                                    Name = dic[model.EffectType.ToString()],
                                    Num = model.Effect,
                                    EffectType = model.EffectType,
                                    BUFFType = 0
                                });
                                ownRole.buffList = buffDatas;
                                GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                                tempBuff.name = "img_Buff_" + model.EffectType;
                                var img = tempBuff.GetComponent<Image>();
                                Common.ImageBind(model.CardUrl, img);
                                var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                buffNum.text = (Convert.ToUInt32(buffNum.text) + model.Effect).ToString();
                            }
                            else
                            {
                                if (!ownRole.buffList.Exists(a => a.EffectType == 9))//免疫状态下所有buff卡不可使用
                                {
                                    var aa = ownRole.buffList.Find(a => a.Name == dic[model.EffectType.ToString()]);//是否以存在此buff
                                    if (aa != null)
                                    {
                                        aa.Num += model.Effect;
                                    }
                                    else
                                    {
                                        var buffs = ownRole.buffList.FindAll(a => a.BUFFType == 0);
                                        if (buffs != null && buffs?.Count > 0)
                                        {
                                            foreach (var item in buffs)
                                            {
                                                ownRole.buffList.Remove(item);
                                            }
                                        }
                                        //一种类型只能存在一条。比如虚弱和愤怒不能共存
                                        ownRole.buffList.Add(new BuffData
                                        {
                                            Name = dic[model.EffectType.ToString()],
                                            Num = model.Effect,
                                            EffectType = model.EffectType,
                                            BUFFType = 0
                                        });
                                        GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                        tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                                        tempBuff.name = "img_Buff_" + model.EffectType;
                                        var img = tempBuff.GetComponent<Image>();
                                        Common.ImageBind(model.CardUrl, img);
                                        var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                        buffNum.text = (Convert.ToUInt32(buffNum.text) + model.Effect).ToString();
                                    }
                                }
                            }
                            #endregion
                        }
                        else if (roleID == AiRole.playerID)//加到AI上
                        {
                            #region BUff添加

                            if (AiRole.buffList == null)
                            {
                                var buffDatas = new List<BuffData>();
                                buffDatas.Add(new BuffData
                                {
                                    Name = dic[model.EffectType.ToString()],
                                    Num = model.Effect,
                                    EffectType = model.EffectType,
                                    BUFFType = 0
                                });
                                AiRole.buffList = buffDatas;
                                GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                                tempBuff.name = "img_Buff_" + model.EffectType;
                                var img = tempBuff.GetComponent<Image>();
                                Common.ImageBind(model.CardUrl, img);
                                var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                buffNum.text = (Convert.ToUInt32(buffNum.text) + model.Effect).ToString();
                            }
                            else
                            {
                                if (!AiRole.buffList.Exists(a => a.EffectType == 9))//免疫状态下所有buff卡不可使用
                                {
                                    var aa = AiRole.buffList.Find(a => a.Name == dic[model.EffectType.ToString()]);//是否以存在此buff
                                    if (aa != null)
                                    {
                                        aa.Num += model.Effect;
                                    }
                                    else
                                    {
                                        var buffs = AiRole.buffList.FindAll(a => a.BUFFType == 0);
                                        if (buffs != null && buffs?.Count > 0)
                                        {
                                            foreach (var item in buffs)
                                            {
                                                AiRole.buffList.Remove(item);
                                            }
                                        }
                                        //一种类型只能存在一条。比如虚弱和愤怒不能共存
                                        AiRole.buffList.Add(new BuffData
                                        {
                                            Name = dic[model.EffectType.ToString()],
                                            Num = model.Effect,
                                            EffectType = model.EffectType,
                                            BUFFType = 0
                                        });
                                        GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                        tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                                        tempBuff.name = "img_Buff_" + model.EffectType;
                                        var img = tempBuff.GetComponent<Image>();
                                        Common.ImageBind(model.CardUrl, img);
                                        var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                        buffNum.text = (Convert.ToUInt32(buffNum.text) + model.Effect).ToString();
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion
                    #region 暴击等作用在攻击的Buff
                    else if (model.EffectType == 10)//暴击
                    {
                        hasUseCard = true;
                        CardUseBeforeTriggerEvent(model.UsingBeforeTriggerList);
                        Destroy(thisObj);
                        if (model.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, model.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= model.Consume;
                        }
                        if (roleID == ownRole.playerID)//加到角色上
                        {
                            #region BUff添加
                            if (ownRole.buffList == null)
                            {
                                var buffDatas = new List<BuffData>();
                                buffDatas.Add(new BuffData
                                {
                                    Name = dic[model.EffectType.ToString()],
                                    Num = model.Effect,
                                    EffectType = model.EffectType,
                                    BUFFType = 0
                                });
                                ownRole.buffList = buffDatas;
                                GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                                tempBuff.name = "img_Buff_" + model.EffectType;
                                var img = tempBuff.GetComponent<Image>();
                                Common.ImageBind(model.CardUrl, img);
                                var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                buffNum.text = (Convert.ToUInt32(buffNum.text) + model.Effect).ToString();
                            }
                            else
                            {
                                if (!ownRole.buffList.Exists(a => a.EffectType == 9))//免疫状态下所有buff卡不可使用
                                {
                                    var aa = ownRole.buffList.Find(a => a.Name == dic[model.EffectType.ToString()]);//是否以存在此buff
                                    if (aa != null)
                                    {
                                        aa.Num += model.Effect;
                                    }
                                    else
                                    {
                                        var buffs = ownRole.buffList.FindAll(a => a.BUFFType == 0);
                                        if (buffs != null && buffs?.Count > 0)
                                        {
                                            foreach (var item in buffs)
                                            {
                                                ownRole.buffList.Remove(item);
                                            }
                                        }
                                        //一种类型只能存在一条。比如虚弱和愤怒不能共存
                                        ownRole.buffList.Add(new BuffData
                                        {
                                            Name = dic[model.EffectType.ToString()],
                                            Num = model.Effect,
                                            EffectType = model.EffectType,
                                            BUFFType = 0
                                        });
                                        GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                        tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                                        tempBuff.name = "img_Buff_" + model.EffectType;
                                        var img = tempBuff.GetComponent<Image>();
                                        Common.ImageBind(model.CardUrl, img);
                                        var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                        buffNum.text = (Convert.ToUInt32(buffNum.text) + model.Effect).ToString();
                                    }
                                }
                            }
                            #endregion
                        }
                        else if (roleID == AiRole.playerID)//加到AI上
                        {
                            #region BUff添加

                            if (AiRole.buffList == null)
                            {
                                var buffDatas = new List<BuffData>();
                                buffDatas.Add(new BuffData
                                {
                                    Name = dic[model.EffectType.ToString()],
                                    Num = model.Effect,
                                    EffectType = model.EffectType,
                                    BUFFType = 0
                                });
                                AiRole.buffList = buffDatas;
                                GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                                tempBuff.name = "img_Buff_" + model.EffectType;
                                var img = tempBuff.GetComponent<Image>();
                                Common.ImageBind(model.CardUrl, img);
                                var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                buffNum.text = (Convert.ToUInt32(buffNum.text) + model.Effect).ToString();
                            }
                            else
                            {
                                if (!AiRole.buffList.Exists(a => a.EffectType == 9))//免疫状态下所有buff卡不可使用
                                {
                                    var aa = AiRole.buffList.Find(a => a.Name == dic[model.EffectType.ToString()]);//是否以存在此buff
                                    if (aa != null)
                                    {
                                        aa.Num += model.Effect;
                                    }
                                    else
                                    {
                                        var buffs = AiRole.buffList.FindAll(a => a.BUFFType == 0);
                                        if (buffs != null && buffs?.Count > 0)
                                        {
                                            foreach (var item in buffs)
                                            {
                                                AiRole.buffList.Remove(item);
                                            }
                                        }
                                        //一种类型只能存在一条。比如虚弱和愤怒不能共存
                                        AiRole.buffList.Add(new BuffData
                                        {
                                            Name = dic[model.EffectType.ToString()],
                                            Num = model.Effect,
                                            EffectType = model.EffectType,
                                            BUFFType = 0
                                        });
                                        GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                        tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                                        tempBuff.name = "img_Buff_" + model.EffectType;
                                        var img = tempBuff.GetComponent<Image>();
                                        Common.ImageBind(model.CardUrl, img);
                                        var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                        buffNum.text = (Convert.ToUInt32(buffNum.text) + model.Effect).ToString();
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion
                    #region 免疫
                    else if (model.EffectType == 9)//免疫
                    {
                        hasUseCard = true;
                        CardUseBeforeTriggerEvent(model.UsingBeforeTriggerList);
                        Destroy(thisObj);
                        if (model.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, model.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= model.Consume;
                        }
                        if (roleID == ownRole.playerID)//加到角色上
                        {
                            var aa = ownRole.buffList.Find(a => a.Name == dic[model.EffectType.ToString()]);//是否以存在此buff
                            if (aa != null)
                            {
                                aa.Num += model.Effect;
                            }
                            else
                            {
                                //除了免疫BUFF其他都删掉
                                var buffDatas = new List<BuffData>();
                                buffDatas.Add(new BuffData
                                {
                                    Name = dic[model.EffectType.ToString()],
                                    Num = model.Effect,
                                    EffectType = model.EffectType,
                                    BUFFType = 3
                                });
                                ownRole.buffList = buffDatas;
                                GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                                tempBuff.name = "img_Buff_" + model.EffectType;
                                var img = tempBuff.GetComponent<Image>();
                                Common.ImageBind(model.CardUrl, img);
                                var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                buffNum.text = (Convert.ToUInt32(buffNum.text) + model.Effect).ToString();
                            }
                        }
                        else if (roleID == AiRole.playerID)//加到AI上
                        {
                            var aa = AiRole.buffList.Find(a => a.Name == dic[model.EffectType.ToString()]);//是否以存在此buff
                            if (aa != null)
                            {
                                aa.Num += model.Effect;
                            }
                            else
                            {
                                //除了免疫BUFF其他都删掉
                                var buffDatas = new List<BuffData>();
                                buffDatas.Add(new BuffData
                                {
                                    Name = dic[model.EffectType.ToString()],
                                    Num = model.Effect,
                                    EffectType = model.EffectType,
                                    BUFFType = 3
                                });
                                AiRole.buffList = buffDatas;
                                GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                                tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                                tempBuff.name = "img_Buff_" + model.EffectType;
                                var img = tempBuff.GetComponent<Image>();
                                Common.ImageBind(model.CardUrl, img);
                                var buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                                buffNum.text = (Convert.ToUInt32(buffNum.text) + model.Effect).ToString();
                            }
                        }
                    }
                    #endregion
                    #region 卡牌拖动到角色位置
                    else if (BattleManager.instance.OwnPlayerData.Find(a => a.playerID == roleID) != null)//卡牌拖动到角色位置
                    {
                        #region 防御
                        if (model.EffectType == 2)//防御
                        {
                            hasUseCard = true;
                            CardUseBeforeTriggerEvent(model.UsingBeforeTriggerList);
                            Destroy(thisObj);
                            if (model.Consume > 0)
                            {
                                Common.EnergyImgChange(ownRole.Energy, model.Consume, 0, ownRole.EnergyMax);
                                ownRole.Energy -= model.Consume;
                            }
                            Pimg_Armor.transform.localScale = Vector3.one;
                            ownRole.Armor += Convert.ToInt32(model.Effect);
                            if (ownRole.Armor > ownRole.bloodMax)
                            {
                                ownRole.Armor = Convert.ToInt32(ownRole.bloodMax);
                            }
                            txt_P_Armor.text = ownRole.Armor.ToString();
                        }
                        #endregion
                    }
                    #endregion
                    #region 卡牌拖动到敌人位置
                    else if (BattleManager.instance.EnemyPlayerData.Find(a => a.playerID == roleID) != null)//卡牌拖动到敌人位置
                    {
                        #region 攻击
                        if (model.EffectType == 1)//攻击
                        {
                            hasUseCard = true;
                            CardUseBeforeTriggerEvent(model.UsingBeforeTriggerList);
                            Destroy(thisObj);
                            if (model.Consume > 0)
                            {
                                Common.EnergyImgChange(ownRole.Energy, model.Consume, 0, ownRole.EnergyMax);
                                ownRole.Energy -= model.Consume;
                            }
                            int DeductionHp = Convert.ToInt32(model.Effect);
                            if (AiRole.Armor > 0)
                            {
                                if (AiRole.Armor >= model.Effect)
                                {
                                    DeductionHp = 0;
                                    AiRole.Armor -= Convert.ToInt32(model.Effect);
                                    txt_E_Armor.text = AiRole.Armor.ToString();
                                }
                                else
                                {
                                    DeductionHp -= AiRole.Armor;
                                    AiRole.Armor = 0;
                                }
                                if (AiRole.Armor == 0)
                                {
                                    Eimg_Armor.transform.localScale = Vector3.zero;
                                }
                            }
                            Common.HPImageChange(Eimg_HP, AiRole.bloodMax, DeductionHp, 0);
                            AiRole.bloodNow -= DeductionHp;
                            if (AiRole.bloodNow <= 0)
                            {
                                AiRole.bloodNow = 0;
                                //AI死亡
                                AiDie();
                            }
                            txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                        }
                        #endregion
                        #region 连续攻击
                        else if (model.EffectType == 5)//连续攻击
                        {
                            hasUseCard = true;
                            CardUseBeforeTriggerEvent(model.UsingBeforeTriggerList);
                            Destroy(thisObj);
                            if (model.Consume > 0)
                            {
                                Common.EnergyImgChange(ownRole.Energy, model.Consume, 0, ownRole.EnergyMax);
                                ownRole.Energy -= model.Consume;
                            }
                            for (int i = 0; i < model.AtkNumber; i++)
                            {
                                int DeductionHp = Convert.ToInt32(model.Effect);
                                if (AiRole.Armor > 0)
                                {
                                    if (AiRole.Armor >= model.Effect)
                                    {
                                        DeductionHp = 0;
                                        AiRole.Armor -= Convert.ToInt32(model.Effect);
                                        txt_E_Armor.text = AiRole.Armor.ToString();
                                    }
                                    else
                                    {
                                        DeductionHp -= AiRole.Armor;
                                        AiRole.Armor = 0;
                                    }
                                    if (AiRole.Armor == 0)
                                    {
                                        Eimg_Armor.transform.localScale = Vector3.zero;
                                    }
                                }
                                Common.HPImageChange(Eimg_HP, AiRole.bloodMax, DeductionHp, 0);
                                AiRole.bloodNow -= DeductionHp;
                                if (AiRole.bloodNow <= 0)
                                {
                                    AiRole.bloodNow = 0;
                                    //AI死亡
                                    AiDie();
                                }
                                txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                            }
                        }
                        #endregion
                        #region 攻击无视防御
                        else if (model.EffectType == 7)//攻击无视防御
                        {
                            hasUseCard = true;
                            CardUseBeforeTriggerEvent(model.UsingBeforeTriggerList);
                            Destroy(thisObj);
                            if (model.Consume > 0)
                            {
                                Common.EnergyImgChange(ownRole.Energy, model.Consume, 0, ownRole.EnergyMax);
                                ownRole.Energy -= model.Consume;
                            }
                            int DeductionHp = Convert.ToInt32(model.Effect);
                            Common.HPImageChange(Eimg_HP, AiRole.bloodMax, DeductionHp, 0);
                            AiRole.bloodNow -= DeductionHp;
                            if (AiRole.bloodNow <= 0)
                            {
                                AiRole.bloodNow = 0;
                                //AI死亡
                                AiDie();
                            }
                            txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                        }
                        #endregion
                        #region 销毁防御
                        else if (model.EffectType == 6)//销毁防御
                        {
                            hasUseCard = true;
                            CardUseBeforeTriggerEvent(model.UsingBeforeTriggerList);
                            Destroy(thisObj);
                            if (model.Consume > 0)
                            {
                                Common.EnergyImgChange(ownRole.Energy, model.Consume, 0, ownRole.EnergyMax);
                                ownRole.Energy -= model.Consume;
                            }
                            if (AiRole.Armor > 0)
                            {
                                AiRole.Armor = 0;
                                txt_E_Armor.text = AiRole.Armor.ToString();
                                Eimg_Armor.transform.localScale = Vector3.zero;
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
            }
            if (hasUseCard)
            {
                CardUseAfterTriggerEvent(model.TriggerAfterUsingList);
                DestroyImmediate(thisObj);

                //放入已使用牌堆
                var useCards = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUsedCardPoolsFileName) ?? new List<CurrentCardPoolModel>();
                useCards.Add(model);
                Common.SaveTxtFile(useCards.ListToJson(), GlobalAttr.CurrentUsedCardPoolsFileName);
                //移除当前手牌
                var atkCards = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentATKBarCardPoolsFileName);
                atkCards.Remove(atkCards.Find(a => a.SingleID == model.SingleID));
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
                BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                txt_Right_Count.text = useCards == null ? "0" : useCards.Count.ToString();
            }
            else
            {
                BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                transform.position = InitVector;
                LayoutRebuilder.ForceRebuildLayoutImmediate(thisParent);
            }
        }
    }
    
    /// <summary>
    /// BUFF数据应用到卡牌攻击中
    /// </summary>
    public void BUFFApplyToCardData(List<BuffData> buffDatas)
    {
        buffDatas?.ForEach(item =>
        {
            if (item.EffectType == 10)//暴击
            {
                model.Effect = model.Effect * 2;
            }
        });
    }

    /// <summary>
    /// AI死亡
    /// </summary>
    public void AiDie()
    {
        UIManager.instance.OpenView("AiDieView");
        UIManager.instance.CloseView("GameView");
    }
    /// <summary>
    /// 玩家死亡
    /// </summary>
    public void PlayerDie()
    {
        UIManager.instance.OpenView("PlayerDieView");
        UIManager.instance.CloseView("GameView");
    }

    /// <summary>
    /// 卡牌所在哪个角色的范围里
    /// </summary>
    /// <param name="CardPos">卡牌所在位置</param>
    /// <returns>角色ID</returns>
    public int CardInRolePosition(Vector3 CardPos)
    {
        int result = 0;
        //var playerPos = BattleManager.instance.OwnPlayerData
        foreach (var item in BattleManager.instance.OwnPlayerData)
        {
            if (CardPos.x < item.playerPos.x + 200 && CardPos.x > item.playerPos.x - 200 && CardPos.y < item.playerPos.y + 180 && CardPos.y > item.playerPos.y - 180)
            {
                result = item.playerID;
            }
        }
        foreach (var item in BattleManager.instance.EnemyPlayerData)
        {
            if (CardPos.x < item.playerPos.x + 200 && CardPos.x > item.playerPos.x - 200 && CardPos.y < item.playerPos.y + 180 && CardPos.y > item.playerPos.y - 180)
            {
                result = item.playerID;
            }
        }
        return result;
    }

    /// <summary>
    /// 隐藏卡牌详情
    /// </summary>
    public void HideCardDetail()
    {
        var parentObj = GameObject.Find("GameView(Clone)/UI/CardPools/obj_CardDetails")?.gameObject;
        int childCount = parentObj.transform.childCount;
        for (int x = 0; x < childCount; x++)
        {
            parentObj.transform.GetChild(x).gameObject.transform.localScale = Vector3.zero;
        }
    }

    /// <summary>
    /// 隐藏放大后的图片
    /// </summary>
    public void HideMagnifyCard()
    {
        MagnifyObj = GameObject.Find("GameView(Clone)/UI/CardPools/obj_Magnify");

        int childCount = MagnifyObj.transform.childCount;
        for (int x = 0; x < childCount; x++)
        {
            Destroy(MagnifyObj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
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
                    if (AiRole.bloodNow + model.Effect >= AiRole.bloodMax)
                    {
                        hasReachCondition = true;
                    }
                }
                else if (item.TriggerCondition == 4)//攻击力*2大与血量
                {
                    if (model.Effect * 2 > AiRole.bloodNow)
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
                        Eimg_Armor.transform.localScale = Vector3.one;
                        AiRole.Armor += Convert.ToInt32(model.Effect);
                        if (ownRole.Armor > ownRole.bloodMax)
                        {
                            ownRole.Armor = Convert.ToInt32(ownRole.bloodMax);
                        }
                        txt_E_Armor.text = AiRole.Armor.ToString();
                    }
                    #endregion
                    #region 血量变化
                    else if (item.TriggerState == 3)//血量变化
                    {
                        if (model.Effect < 0)
                        {
                            var effect = ownRole.bloodNow + Convert.ToInt32(model.Effect);
                            if (effect < 0)
                            {
                                ownRole.bloodNow = 0;
                                //玩家死亡
                                PlayerDie();
                            }
                            else
                            {
                                Common.HPImageChange(Pimg_HP, ownRole.bloodMax, model.Effect, 0);
                                ownRole.bloodNow += Convert.ToInt32(model.Effect);
                            }
                        }
                        else
                        {
                            Common.HPImageChange(Pimg_HP, ownRole.bloodMax, model.Effect, 1);
                            ownRole.bloodNow += Convert.ToInt32(model.Effect);
                            if (ownRole.bloodNow > ownRole.bloodMax)
                            {
                                ownRole.bloodNow = ownRole.bloodMax;
                            }
                        }
                        txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
                    }
                    #endregion
                    #region 能量变化
                    else if (item.TriggerState == 4)//能量变化
                    {
                        if (model.Effect < 0)//扣能量
                        {
                            var effect = ownRole.Energy + Convert.ToInt32(model.Effect);
                            if (effect < 0)
                            {
                                Common.EnergyImgChange(ownRole.Energy, ownRole.Energy, 0, ownRole.EnergyMax);
                                ownRole.Energy = 0;
                            }
                            else
                            {
                                Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(model.Effect), 0, ownRole.EnergyMax);
                                ownRole.Energy += Convert.ToInt32(model.Effect);
                            }
                        }
                        else
                        {
                            Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(model.Effect), 1, ownRole.EnergyMax);
                            ownRole.Energy += Convert.ToInt32(model.Effect);
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
                            txt_E_Armor.text = AiRole.Armor.ToString();
                            Eimg_Armor.transform.localScale = Vector3.zero;
                        }
                    }
                    #endregion
                    #region 无视防御
                    else if (item.TriggerState == 7)//无视防御
                    {
                        model.EffectType = 7;
                    }
                    #endregion
                    #region 攻击暴击
                    else if (item.TriggerState == 10)//暴击
                    {
                        model.Effect = model.Effect * 2;
                    }
                    #endregion
                    else if (item.TriggerState == 17)//移除一张手牌
                    {

                    }
                    #region 双倍伤害
                    else if (item.TriggerState == 18)//双倍伤害
                    {
                        model.Effect = model.Effect * 2;
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
                                txt_E_Armor.text = AiRole.Armor.ToString();
                            }
                            else
                            {
                                DeductionHp -= AiRole.Armor;
                                AiRole.Armor = 0;
                            }
                            if (AiRole.Armor == 0)
                            {
                                Eimg_Armor.transform.localScale = Vector3.zero;
                            }
                        }
                        Common.HPImageChange(Eimg_HP, AiRole.bloodMax, DeductionHp, 0);
                        AiRole.bloodNow -= DeductionHp;
                        if (AiRole.bloodNow <= 0)
                        {
                            AiRole.bloodNow = 0;
                            //AI死亡
                            AiDie();
                        }
                        txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                    }
                    #endregion
                    #region 防御
                    else if (item.TriggerState == 2)//防御
                    {
                        Eimg_Armor.transform.localScale = Vector3.one;
                        AiRole.Armor += Convert.ToInt32(item.TriggerValue);
                        if (ownRole.Armor > ownRole.bloodMax)
                        {
                            ownRole.Armor = Convert.ToInt32(ownRole.bloodMax);
                        }
                        txt_E_Armor.text = AiRole.Armor.ToString();
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
                                PlayerDie();
                            }
                            else
                            {
                                Common.HPImageChange(Pimg_HP, ownRole.bloodMax, item.TriggerValue, 0);
                                ownRole.bloodNow += Convert.ToInt32(item.TriggerValue);
                            }
                        }
                        else
                        {
                            Common.HPImageChange(Pimg_HP, ownRole.bloodMax, item.TriggerValue, 1);
                            ownRole.bloodNow += Convert.ToInt32(item.TriggerValue);
                            if (ownRole.bloodNow > ownRole.bloodMax)
                            {
                                ownRole.bloodNow = ownRole.bloodMax;
                            }
                        }
                        txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
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
                        for (int i = 0; i < model.AtkNumber; i++)
                        {
                            int DeductionHp = Convert.ToInt32(item.TriggerValue);
                            if (AiRole.Armor > 0)
                            {
                                if (AiRole.Armor >= item.TriggerValue)
                                {
                                    DeductionHp = 0;
                                    AiRole.Armor -= Convert.ToInt32(item.TriggerValue);
                                    txt_E_Armor.text = AiRole.Armor.ToString();
                                }
                                else
                                {
                                    DeductionHp -= AiRole.Armor;
                                    AiRole.Armor = 0;
                                }
                                if (AiRole.Armor == 0)
                                {
                                    Eimg_Armor.transform.localScale = Vector3.zero;
                                }
                            }
                            Common.HPImageChange(Eimg_HP, AiRole.bloodMax, DeductionHp, 0);
                            AiRole.bloodNow -= DeductionHp;
                            if (AiRole.bloodNow <= 0)
                            {
                                AiRole.bloodNow = 0;
                                //AI死亡
                                AiDie();
                            }
                            txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                        }
                    }
                    #endregion
                    #region 销毁防御
                    else if (item.TriggerState == 6)//销毁防御
                    {
                        if (AiRole.Armor > 0)
                        {
                            AiRole.Armor = 0;
                            txt_E_Armor.text = AiRole.Armor.ToString();
                            Eimg_Armor.transform.localScale = Vector3.zero;
                        }
                    }
                    #endregion
                    #region 无视防御攻击
                    else if (item.TriggerState == 7)//无视防御攻击
                    {
                        int DeductionHp = Convert.ToInt32(item.TriggerValue);
                        Common.HPImageChange(Eimg_HP, AiRole.bloodMax, DeductionHp, 0);
                        AiRole.bloodNow -= DeductionHp;
                        if (AiRole.bloodNow <= 0)
                        {
                            AiRole.bloodNow = 0;
                            //AI死亡
                            AiDie();
                        }
                        txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
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
                            tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                            tempBuff.name = "img_Buff_" + item.TriggerState;
                            var img = tempBuff.GetComponent<Image>();
                            Common.ImageBind(model.CardUrl, img);
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
                                    tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                                    tempBuff.name = "img_Buff_" + item.TriggerState;
                                    var img = tempBuff.GetComponent<Image>();
                                    Common.ImageBind(model.CardUrl, img);
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
                            tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                            tempBuff.name = "img_Buff_" + item.TriggerState;
                            var img = tempBuff.GetComponent<Image>();
                            Common.ImageBind(model.CardUrl, img);
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
                                tempBuff = Common.AddChild(P_buffObj.transform, tempBuff);
                                tempBuff.name = "img_Buff_" + item.TriggerState;
                                var img = tempBuff.GetComponent<Image>();
                                Common.ImageBind(model.CardUrl, img);
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
                        PlayerHandCardList.Remove(model);
                        atkBarCard.Remove(model);
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
                        Common.HPImageChange(Eimg_HP, AiRole.bloodMax, AiRole.bloodNow, 0);
                        //AI死亡
                        AiDie();
                        txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                    }
                    #endregion
                }
            }
        }
    }

}
