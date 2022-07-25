using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardTriggerEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Vector3 InitVector;
    GameObject MagnifyObj, thisObj, gameViewObj, P_buffObj, E_buffObj;
    CurrentCardPoolModel model = new CurrentCardPoolModel();//卡牌数据
    Image Pimg_HP, Eimg_HP, Pimg_Armor, Eimg_Armor;
    Text txt_P_HP, txt_E_HP, txt_P_Armor, txt_E_Armor;
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
        MagnifyObj = gameViewObj.transform.Find("UI/CardPools/obj_Magnify").gameObject;
        P_buffObj = gameViewObj.transform.Find("UI/Player/BuffBar").gameObject;
        E_buffObj = gameViewObj.transform.Find("UI/Enemy/BuffBar").gameObject;

        InitVector = transform.position;
        thisObj = transform.parent.Find(name).gameObject;

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
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.TurnStart)
        {
            HideCardDetail();
            HideMagnifyCard();
            if (model.EffectType != 0)//无效果。黑卡
            {
                BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.Control);
            }
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
            var roleID = CardInRolePosition(eventData.position);
            var ownRole = BattleManager.instance.OwnPlayerData[0];
            var AiRole = BattleManager.instance.OwnPlayerData[0];
            #region 卡牌拖动到任意位置
            #region 血量变化
            if (model.EffectType == 3)//血量变化
            {
                if (model.Effect < 0)
                {
                    var effect = ownRole.bloodNow + Convert.ToInt32(model.Effect);
                    if (effect < 0)//血量不足以使用此卡
                    {
                        BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                        transform.position = InitVector;
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
            }
            #endregion
            #region 能量变化
            else if (model.EffectType == 4)//能量变化
            {
                if (model.Effect < 0)//扣能量
                {
                    var effect = ownRole.Energy + Convert.ToInt32(model.Effect);
                    if (effect < 0)//能量不足以使用此卡
                    {
                        BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                        transform.position = InitVector;
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
            #region 复制卡
            else if (model.EffectType == 12)//复制卡
            {

            }
            #endregion
            #region AI每次攻击手牌攻击力+1
            else if (model.EffectType == 13)//AI每次攻击手牌攻击力+1
            {

            }
            #endregion
            #region 卡牌攻击力叠加
            else if (model.EffectType == 14)//卡牌攻击力叠加
            {

            }
            #endregion
            #endregion
            if (roleID == 0)
            {
                BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                transform.position = InitVector;
            }
            else
            {

                #region 愤怒、虚弱、暴击等Buff
                if (model.EffectType == 8 || model.EffectType == 11 || model.EffectType == 10)//愤怒
                {
                    if (ownRole.Energy >= model.Consume)
                    {
                        DestroyImmediate(thisObj);
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        Common.DicDataRead(ref dic, GlobalAttr.EffectTypeFileName, "Card");
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
                                    Num = model.Effect
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
                                        ownRole.buffList.Add(new BuffData
                                        {
                                            Name = dic[model.EffectType.ToString()],
                                            Num = model.Effect
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
                                    Num = model.Effect
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
                                        AiRole.buffList.Add(new BuffData
                                        {
                                            Name = dic[model.EffectType.ToString()],
                                            Num = model.Effect
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
                    else
                    {
                        BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                        transform.position = InitVector;
                    }
                }
                #endregion
                #region 免疫
                else if (model.EffectType == 9)//免疫
                {
                    if (ownRole.Energy >= model.Consume)
                    {
                        DestroyImmediate(thisObj);
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        Common.DicDataRead(ref dic, GlobalAttr.EffectTypeFileName, "Card");
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
                                    Num = model.Effect
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
                                var aa = ownRole.buffList.Find(a => a.Name == dic[model.EffectType.ToString()]);//是否以存在此buff
                                if (aa != null)
                                {
                                    aa.Num += model.Effect;
                                }
                                else
                                {
                                    ownRole.buffList.Add(new BuffData
                                    {
                                        Name = dic[model.EffectType.ToString()],
                                        Num = model.Effect
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
                                    Num = model.Effect
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
                                var aa = AiRole.buffList.Find(a => a.Name == dic[model.EffectType.ToString()]);//是否以存在此buff
                                if (aa != null)
                                {
                                    aa.Num += model.Effect;
                                }
                                else
                                {
                                    AiRole.buffList.Add(new BuffData
                                    {
                                        Name = dic[model.EffectType.ToString()],
                                        Num = model.Effect
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
                            #endregion
                        }
                    }
                    else
                    {
                        BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                        transform.position = InitVector;
                    }
                }
                #endregion
                #region 卡牌拖动到角色位置
                else if (BattleManager.instance.OwnPlayerData.Find(a => a.playerID == roleID) != null)//卡牌拖动到角色位置
                {
                    #region 防御
                    if (model.EffectType == 2)//防御
                    {
                        if (ownRole.Energy >= model.Consume)
                        {
                            DestroyImmediate(thisObj);
                            if (model.Consume > 0)
                            {
                                Common.EnergyImgChange(ownRole.Energy, model.Consume, 0, ownRole.EnergyMax);
                                ownRole.Energy -= model.Consume;
                            }
                            Eimg_Armor.transform.localScale = Vector3.one;
                            AiRole.Armor += Convert.ToInt32(model.Effect);
                            if (ownRole.Armor > ownRole.bloodMax)
                            {
                                ownRole.Armor = Convert.ToInt32(ownRole.bloodMax);
                            }
                            txt_E_Armor.text = AiRole.Armor.ToString();
                        }
                        else
                        {
                            BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                            transform.position = InitVector;
                        }
                    }
                    #endregion
                    else
                    {
                        BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                        transform.position = InitVector;
                    }
                }
                #endregion
                #region 卡牌拖动到敌人位置
                else if (BattleManager.instance.EnemyPlayerData.Find(a => a.playerID == roleID) != null)//卡牌拖动到敌人位置
                {
                    #region 攻击
                    if (model.EffectType == 1)//攻击
                    {
                        if (ownRole.Energy >= model.Consume)
                        {
                            DestroyImmediate(thisObj);
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
                            }
                            txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                        }
                        else
                        {
                            BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                            transform.position = InitVector;
                        }
                    }
                    #endregion
                    #region 连续攻击
                    else if (model.EffectType == 5)//连续攻击
                    {
                        if (ownRole.Energy >= model.Consume)
                        {
                            DestroyImmediate(thisObj);
                            for (int i = 0; i < model.AtkNumber; i++)
                            {
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
                                }
                                txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                            }
                        }
                        else
                        {
                            BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                            transform.position = InitVector;
                        }
                    }
                    #endregion
                    #region 攻击无视防御
                    else if (model.EffectType == 7)//攻击无视防御
                    {
                        if (ownRole.Energy >= model.Consume)
                        {
                            DestroyImmediate(thisObj);
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
                            }
                            txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                        }
                        else
                        {
                            BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                            transform.position = InitVector;
                        }
                    }
                    #endregion
                    #region 销毁防御
                    else if (model.EffectType == 6)//销毁防御
                    {
                        if (ownRole.Energy >= model.Consume)
                        {
                            DestroyImmediate(thisObj);
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
                        else
                        {
                            BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                            transform.position = InitVector;
                        }
                    }
                    #endregion
                    else
                    {
                        BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                        transform.position = InitVector;
                    }
                }
                #endregion
                else
                {
                    BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                    transform.position = InitVector;
                }
            }
            CurrentRoleModel playerRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName);
            playerRole.Armor = ownRole.Armor;
            playerRole.Energy = ownRole.Energy;
            playerRole.MaxEnergy = ownRole.EnergyMax;
            playerRole.HP = ownRole.bloodNow;
            playerRole.Wealth = ownRole.Wealth;
            Common.SaveTxtFile(playerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);

            CurrentRoleModel aiData = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName);
            aiData.Armor = ownRole.Armor;
            aiData.HP = ownRole.bloodNow;
            aiData.MaxHP = ownRole.bloodMax;
            Common.SaveTxtFile(playerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);

            LayoutRebuilder.ForceRebuildLayoutImmediate(thisObj.transform.parent.GetComponent<RectTransform>());
        }
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

    public void HideCardDetail()
    {
        var parentObj = GameObject.Find("GameView(Clone)/UI/CardPools/obj_CardDetails")?.gameObject;
        int childCount = parentObj.transform.childCount;
        for (int x = 0; x < childCount; x++)
        {
            parentObj.transform.GetChild(x).gameObject.transform.localScale = Vector3.zero;
        }
    }

    public void HideMagnifyCard()
    {
        MagnifyObj = GameObject.Find("GameView(Clone)/UI/CardPools/obj_Magnify");

        int childCount = MagnifyObj.transform.childCount;
        for (int x = 0; x < childCount; x++)
        {
            DestroyImmediate(MagnifyObj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
        }
    }

}
