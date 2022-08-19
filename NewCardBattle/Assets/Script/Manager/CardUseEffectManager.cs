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
    public bool HasNumberValueChange = false;//是否数值恢复
    public int BeRemoveCardNum = 0;//被移除的卡数量
    public bool HasCrit = false;//暴击BUFF是否已被使用
    #endregion

    /// <summary>
    /// 卡牌使用前的触发事件
    /// </summary>
    /// <param name="model">所使用的的卡牌数据</param>
    /// <param name="PlayerOrAI">谁使用的卡牌0角色1AI</param>
    /// <param name="EffectOn">动画作用在谁身上，0AI，1角色</param>
    /// <returns>0没有动画、1卡牌回收、2攻击、3防御、4血量恢复、5能量恢复、6移除卡牌、7抽卡动画、8血量扣除、9摧毁防御
    /// 24移除所有手牌
    /// </returns>
    public int CardUseBeforeTriggerEvent(CurrentCardPoolModel model, int PlayerOrAI, ref string EffectOn)
    {
        var ownRole = BattleManager.instance.OwnPlayerData[0];
        var AiRole = BattleManager.instance.EnemyPlayerData[0];
        //能量不足无法使用卡牌
        if (model.EffectType != 35)
        {
            if (ownRole.Energy < model.Consume)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
                return 0;
            }
        }
        #region 判断卡牌拖动位置
        switch (model.EffectType)
        {
            #region 可以拖到任意位置
            case 3:
            case 4:
            case 12:
            case 30:
            case 27:
            case 0:
                //任意位置
                break;
            #endregion
            #region 只能拖动角色身上
            case 2:
            case 13:
            case 26:
            case 33:
            case 28://防御只能在角色身上
                if (ownRole.playerID != CardToRoleID)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
                    return 0;
                }
                break;
            #endregion
            #region 只能拖到AI身上
            case 1:
            case 14:
            case 5:
            case 7:
            case 32:
            case 31:
            case 35:
            case 6:
                if (AiRole.playerID != CardToRoleID)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
                    return 0;
                }
                break;
            #endregion
            #region 只能拖到AI或角色身上
            case 8:
            case 11:
            case 10:
            case 20:
            case 21:
            case 9:
            case 29:
            case 22:
                if (CardToRoleID == 0)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
                    return 0;
                }
                break;
                #endregion
        }
        #endregion
        if (CurrentCard != null)
        {
            CurrentCard.transform.localScale = Vector3.zero;
        }
        int result = 0;
        var list = model.UsingBeforeTriggerList;
        #region BUFF限制卡牌使用
        List<BUFFEffect> buffResult = null;
        if (PlayerOrAI == 0)
        {
            buffResult = BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref model, ref ownRole.handCardList);
        }
        else
        {
            buffResult = BUFFManager.instance.BUFFApply(ref AiRole.buffList, ownRole.buffList, ref model, ref AiRole.handCardList, false, 1);
        }
        if (buffResult?.Count > 0)
        {
            if (buffResult.Exists(a => a.EffectType == 1 && a.HasValid == false))//卡无效果未被使用
            {
                return result;
            }
            if (buffResult.Exists(a => a.EffectType == 2 && a.HasValid == false))//卡无效果已被使用
            {
                return result;
            }
        }
        #endregion
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
                    if (PlayerOrAI == 0)
                    {
                        if (AiRole.Armor > 0)
                        {
                            hasReachCondition = true;
                        }
                    }
                    else
                    {
                        if (ownRole.Armor > 0)
                        {
                            hasReachCondition = true;
                        }
                    }
                }
                else if (item.TriggerCondition == 3)//满血
                {
                    if (PlayerOrAI == 0)
                    {
                        if (AiRole.bloodNow == AiRole.bloodMax)
                        {
                            hasReachCondition = true;
                        }
                    }
                    else
                    {
                        if (ownRole.bloodNow == ownRole.bloodMax)
                        {
                            hasReachCondition = true;
                        }
                    }
                }
                else if (item.TriggerCondition == 4)//攻击力*2大与血量
                {
                    if (PlayerOrAI == 0)
                    {
                        if (model.Effect * 2 > AiRole.bloodNow)
                        {
                            hasReachCondition = true;
                        }
                    }
                    else
                    {
                        if (model.Effect * 2 > ownRole.bloodNow)
                        {
                            hasReachCondition = true;
                        }
                    }
                }
                else if (item.TriggerCondition == 5)//手牌大于1
                {
                    if (PlayerOrAI == 0)
                    {
                        if (gameView.ATKBarCardList?.Count > 1)
                        {
                            hasReachCondition = true;
                        }
                    }
                }
                #endregion
                if (hasReachCondition)
                {
                    switch (item.TriggerState)
                    {
                        #region 防御
                        case 2:
                            if (PlayerOrAI == 0)
                            {
                                gameView.Pimg_Armor.transform.localScale = Vector3.one;
                                ownRole.Armor += Convert.ToInt32(item.TriggerValue);
                                if (ownRole.Armor > ownRole.bloodMax)
                                {
                                    ownRole.Armor = Convert.ToInt32(ownRole.bloodMax);
                                }
                                gameView.txt_P_Armor.text = ownRole.Armor.ToString();
                                EffectOn = "1";
                            }
                            else
                            {
                                gameView.Eimg_Armor.transform.localScale = Vector3.one;
                                AiRole.Armor += Convert.ToInt32(item.TriggerValue);
                                if (AiRole.Armor > AiRole.bloodMax)
                                {
                                    AiRole.Armor = Convert.ToInt32(AiRole.bloodMax);
                                }
                                gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                                EffectOn = "0";
                            }
                            result = 3;
                            break;
                        #endregion
                        #region 血量变化
                        case 3:
                            if (PlayerOrAI == 0)
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
                                    result = 8;

                                }
                                else
                                {
                                    Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, item.TriggerValue, 1);
                                    ownRole.bloodNow += Convert.ToInt32(item.TriggerValue);
                                    if (ownRole.bloodNow > ownRole.bloodMax)
                                    {
                                        ownRole.bloodNow = ownRole.bloodMax;
                                    }
                                    result = 4;
                                }
                                gameView.txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
                                EffectOn = "1";
                            }
                            else
                            {
                                if (item.TriggerValue < 0)
                                {
                                    var effect = AiRole.bloodNow + Convert.ToInt32(item.TriggerValue);
                                    if (effect < 0)
                                    {
                                        AiRole.bloodNow = 0;
                                        //玩家死亡
                                        gameView.AiDie();
                                    }
                                    else
                                    {
                                        Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, item.TriggerValue, 0);
                                        AiRole.bloodNow += Convert.ToInt32(item.TriggerValue);
                                    }
                                    result = 8;

                                }
                                else
                                {
                                    Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, item.TriggerValue, 1);
                                    AiRole.bloodNow += Convert.ToInt32(item.TriggerValue);
                                    if (AiRole.bloodNow > AiRole.bloodMax)
                                    {
                                        AiRole.bloodNow = AiRole.bloodMax;
                                    }
                                    result = 4;
                                }
                                gameView.txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                                EffectOn = "0";
                            }
                            break;
                        #endregion
                        #region 能量变化
                        case 4:
                            if (PlayerOrAI == 0)
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
                                        Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(item.TriggerValue * -1), 0, ownRole.EnergyMax);
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
                                EffectOn = "1";
                                result = 5;
                            }
                            break;
                        #endregion
                        #region 摧毁防御
                        case 6:
                            if (PlayerOrAI == 0)
                            {
                                if (AiRole.Armor > 0)
                                {
                                    AiRole.Armor = 0;
                                    gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                                    gameView.Eimg_Armor.transform.localScale = Vector3.zero;
                                }
                                EffectOn = "0";
                            }
                            else
                            {
                                if (ownRole.Armor > 0)
                                {
                                    ownRole.Armor = 0;
                                    gameView.txt_P_Armor.text = ownRole.Armor.ToString();
                                    gameView.Pimg_Armor.transform.localScale = Vector3.zero;
                                }
                                EffectOn = "1";
                            }
                            result = 9;
                            break;
                        #endregion
                        #region 无视防御
                        case 7:
                            model.EffectType = 7;
                            break;
                        #endregion
                        #region 伤害翻倍
                        case 10:
                        case 18:
                            HasNumberValueChange = true;
                            model.Effect = model.Effect * 2;
                            break;
                        #endregion
                        #region 斩杀
                        case 19:
                            model.EffectType = 7;
                            HasNumberValueChange = true;
                            model.Effect = model.Effect * 2;
                            break;
                        #endregion
                        #region 移除一张手牌
                        case 17:
                            result = 17;
                            break;
                        #endregion
                        #region 移除所有手牌
                        case 24:
                            BeRemoveCardNum = gameView.ATKBarCardList.Count - 1;
                            result = 24;
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
    /// 卡牌使用效果
    /// </summary>
    /// <param name="hasUseCard">卡牌是否使用成功</param>
    /// <param name="EffectOn">效果作用在。0AI,1角色</param>
    /// <param name="hasEffect">是否有效果</param>
    /// <param name="PlayAnim">播放什么动画0无、1防御、2摧毁防御、3闪避动画、4扣血、5回血</param>
    public void CardUseEffect(CurrentCardPoolModel CardData, ref bool hasUseCard, ref string EffectOn, ref bool hasEffect, ref int PlayAnim)
    {
        var ownRole = BattleManager.instance.OwnPlayerData[0];
        var AiRole = BattleManager.instance.EnemyPlayerData[0];
        Dictionary<string, string> dic = new Dictionary<string, string>();
        Common.DicDataRead(ref dic, GlobalAttr.EffectTypeFileName, "Card");
        //能量不足无法使用卡牌
        if (CardData.EffectType != 35)
        {
            if (ownRole.Energy < CardData.Consume)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
                return;
            }
        }
        #region BUFF限制卡牌使用
        var buffResult = BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref CardData, ref ownRole.handCardList);
        if (buffResult?.Count > 0)
        {
            if (buffResult.Exists(a => a.EffectType == 1 && a.HasValid == false))//卡无效果未被使用
            {
                hasUseCard = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
                return;
            }
            else if (buffResult.Exists(a => a.EffectType == 2 && a.HasValid == false))//卡无效果已被使用
            {
                hasUseCard = true;
                hasEffect = false;
                PlayAnim = 3;
                LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
                return;
            }
            else if (buffResult.Exists(a => a.HPChange > 0))//狂暴回血
            {
                var buff = buffResult.Find(a => a.HPChange != 0);
                if (ownRole.bloodNow != ownRole.bloodMax)
                {
                    var changeHP = buff.HPChange;
                    if (ownRole.bloodNow + buff.HPChange > ownRole.bloodMax)
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
                PlayAnim = 5;
                gameView.txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
            }
        }
        #endregion
        #region 卡牌拖动到任意位置
        #region 血量变化
        if (CardData.EffectType == 3)//血量变化
        {
            hasUseCard = true;
            if (CardData.Consume > 0)
            {
                Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                ownRole.Energy -= CardData.Consume;
            }
            if (CardData.Effect < 0)
            {
                var effect = ownRole.bloodNow + Convert.ToInt32(CardData.Effect);
                if (effect < 0)//血量不足以使用此卡
                {
                    hasUseCard = false;
                }
                else
                {
                    Destroy(CurrentCard);
                    Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, CardData.Effect, 0);
                    ownRole.bloodNow += Convert.ToInt32(CardData.Effect);
                }
            }
            else
            {
                Destroy(CurrentCard);
                if (ownRole.bloodNow != ownRole.bloodMax)
                {
                    var changeHP = CardData.Effect;
                    if (ownRole.bloodNow + CardData.Effect > ownRole.bloodMax)
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
        else if (CardData.EffectType == 4)//能量变化
        {
            hasUseCard = true;
            if (CardData.Effect < 0)//扣能量
            {
                var effect = ownRole.Energy + Convert.ToInt32(CardData.Effect);
                if (effect < 0)//能量不足以使用此卡
                {
                    hasUseCard = false;
                }
                else
                {
                    Destroy(CurrentCard);
                    Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(CardData.Effect * -1), 0, ownRole.EnergyMax);
                    ownRole.Energy += Convert.ToInt32(CardData.Effect);
                }
            }
            else
            {
                Destroy(CurrentCard);
                int effect = Convert.ToInt32(CardData.Effect);
                if (ownRole.Energy + effect > ownRole.EnergyMax)
                {
                    effect = ownRole.EnergyMax - ownRole.Energy;
                }
                Common.EnergyImgChange(ownRole.Energy, effect, 1, ownRole.EnergyMax);
                ownRole.Energy += effect;
            }
            EffectOn = "1";
        }
        #endregion
        #region 复制卡
        else if (CardData.EffectType == 12)//复制卡
        {
            hasUseCard = true;
            Destroy(CurrentCard);
            if (CardData.Consume > 0)
            {
                Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                ownRole.Energy -= CardData.Consume;
            }
            gameView.obj_RemoveCard.SetActive(true);
        }
        #endregion
        #region 混乱杀机
        else if (CardData.EffectType == 30)//每有一层混乱手牌攻击力+1
        {
            hasUseCard = true;
            Destroy(CurrentCard);
            if (CardData.Consume > 0)
            {
                Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                ownRole.Energy -= CardData.Consume;
            }
            if (ownRole.buffList?.Count > 0)
            {
                var chaos = ownRole.buffList.Find(a => a.EffectType == 29);
                if (chaos != null)
                {
                    foreach (var item in gameView.ATKBarCardList)
                    {
                        if (item.CardType == 0)
                        {
                            item.Effect += chaos.Num;
                        }
                    }
                    gameView.RealTimeChangeCardData(0);
                    HasNumberValueChange = true;
                }
            }
        }
        #endregion
        #region 混乱杀机
        else if (CardData.EffectType == 27)//当前护甲翻倍
        {
            hasUseCard = true;
            Destroy(CurrentCard);
            if (CardData.Consume > 0)
            {
                Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                ownRole.Energy -= CardData.Consume;
            }
            if (ownRole.Armor > 0)
            {
                gameView.Pimg_Armor.transform.localScale = Vector3.one;
                ownRole.Armor *= 2;
                if (ownRole.Armor > ownRole.bloodMax)
                {
                    ownRole.Armor = Convert.ToInt32(ownRole.bloodMax);
                }
                gameView.txt_P_Armor.text = ownRole.Armor.ToString();
                EffectOn = "1";
            }
        }
        #endregion
        #region 无效果
        else if (CardData.EffectType == 0)
        {
            hasUseCard = true;
            PlayAnim = 0;
            Destroy(CurrentCard);
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
                if (CardData.EffectType == 8 || CardData.EffectType == 11)//愤怒
                {
                    hasUseCard = true;
                    Destroy(CurrentCard);
                    if (CardData.Consume > 0)
                    {
                        Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                        ownRole.Energy -= CardData.Consume;
                    }
                    if (CardToRoleID == ownRole.playerID)//加到角色上
                    {
                        AddBUFF(0, 0, CardData.EffectType, Convert.ToInt32(CardData.Effect), ref CardData);
                    }
                    else if (CardToRoleID == AiRole.playerID)//加到AI上
                    {
                        AddBUFF(1, 0, CardData.EffectType, Convert.ToInt32(CardData.Effect), ref CardData);
                    }
                }
                #endregion
                #region 暴击等作用在攻击的Buff
                else if (CardData.EffectType == 10)//暴击
                {
                    hasUseCard = true;
                    Destroy(CurrentCard);
                    if (CardData.Consume > 0)
                    {
                        Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                        ownRole.Energy -= CardData.Consume;
                    }
                    if (CardToRoleID == ownRole.playerID)//加到角色上
                    {
                        AddBUFF(0, 4, CardData.EffectType, Convert.ToInt32(CardData.Effect), ref CardData);
                    }
                    else if (CardToRoleID == AiRole.playerID)//加到AI上
                    {
                        AddBUFF(1, 4, CardData.EffectType, Convert.ToInt32(CardData.Effect), ref CardData);
                    }
                }
                #endregion
                #region 束缚、缠绕
                else if (CardData.EffectType == 20 || CardData.EffectType == 21)
                {
                    hasUseCard = true;
                    Destroy(CurrentCard);
                    if (CardData.Consume > 0)
                    {
                        Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                        ownRole.Energy -= CardData.Consume;
                    }
                    if (CardToRoleID == ownRole.playerID)//加到角色上
                    {
                        AddBUFF(0, 1, CardData.EffectType, Convert.ToInt32(CardData.Effect), ref CardData);
                    }
                    else if (CardToRoleID == AiRole.playerID)//加到AI上
                    {
                        AddBUFF(1, 1, CardData.EffectType, Convert.ToInt32(CardData.Effect), ref CardData);
                    }
                }
                #endregion
                #region 免疫
                else if (CardData.EffectType == 9)//免疫
                {
                    hasUseCard = true;
                    Destroy(CurrentCard);
                    if (CardData.Consume > 0)
                    {
                        Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                        ownRole.Energy -= CardData.Consume;
                    }
                    if (CardToRoleID == ownRole.playerID)//加到角色上
                    {
                        AddBUFF(0, 3, CardData.EffectType, Convert.ToInt32(CardData.Effect), ref CardData);
                    }
                    else if (CardToRoleID == AiRole.playerID)//加到AI上
                    {
                        AddBUFF(1, 3, CardData.EffectType, Convert.ToInt32(CardData.Effect), ref CardData);
                    }
                }
                #endregion
                #region 混乱
                else if (CardData.EffectType == 29)
                {
                    hasUseCard = true;
                    Destroy(CurrentCard);
                    if (CardData.Consume > 0)
                    {
                        Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                        ownRole.Energy -= CardData.Consume;
                    }
                    if (CardToRoleID == ownRole.playerID)//加到角色上
                    {
                        AddBUFF(0, 4, CardData.EffectType, Convert.ToInt32(CardData.Effect), ref CardData);
                    }
                    else if (CardToRoleID == AiRole.playerID)//加到AI上
                    {
                        AddBUFF(1, 4, CardData.EffectType, Convert.ToInt32(CardData.Effect), ref CardData);
                    }
                }
                #endregion
                #region 重伤
                else if (CardData.EffectType == 22)
                {
                    hasUseCard = true;
                    Destroy(CurrentCard);
                    if (CardData.Consume > 0)
                    {
                        Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                        ownRole.Energy -= CardData.Consume;
                    }
                    int effectValue = Convert.ToInt32(CardData.Effect);
                    if (BeRemoveCardNum > 0)
                    {
                        effectValue *= BeRemoveCardNum;
                    }
                    Debug.Log(effectValue);
                    if (CardToRoleID == ownRole.playerID)//加到角色上
                    {
                        AddBUFF(0, 2, CardData.EffectType, effectValue, ref CardData);
                    }
                    else if (CardToRoleID == AiRole.playerID)//加到AI上
                    {
                        AddBUFF(1, 2, CardData.EffectType, effectValue, ref CardData);
                    }
                }
                #endregion
                #region 卡牌拖动到角色位置
                else if (BattleManager.instance.OwnPlayerData.Find(a => a.playerID == CardToRoleID) != null)//卡牌拖动到角色位置
                {
                    #region 防御
                    if (CardData.EffectType == 2)//防御
                    {
                        hasUseCard = true;
                        Destroy(CurrentCard);
                        if (CardData.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CardData.Consume;
                        }
                        gameView.Pimg_Armor.transform.localScale = Vector3.one;
                        ownRole.Armor += Convert.ToInt32(CardData.Effect);
                        if (ownRole.Armor > ownRole.bloodMax)
                        {
                            ownRole.Armor = Convert.ToInt32(ownRole.bloodMax);
                        }
                        gameView.txt_P_Armor.text = ownRole.Armor.ToString();
                        EffectOn = "1";
                    }
                    #endregion
                    #region 概率防御
                    else if (CardData.EffectType == 28)
                    {
                        hasUseCard = true;
                        Destroy(CurrentCard);
                        if (CardData.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CardData.Consume;
                        }
                        int random = UnityEngine.Random.Range(0, 4);
                        if (random == 0)//销毁防御
                        {
                            if (ownRole.Armor > 0)
                            {
                                ownRole.Armor = 0;
                                gameView.txt_P_Armor.text = AiRole.Armor.ToString();
                                gameView.Pimg_Armor.transform.localScale = Vector3.zero;
                            }
                            PlayAnim = 2;
                        }
                        else//加护甲
                        {
                            gameView.Pimg_Armor.transform.localScale = Vector3.one;
                            ownRole.Armor += Convert.ToInt32(CardData.Effect);
                            if (ownRole.Armor > ownRole.bloodMax)
                            {
                                ownRole.Armor = Convert.ToInt32(ownRole.bloodMax);
                            }
                            gameView.txt_P_Armor.text = ownRole.Armor.ToString();
                            PlayAnim = 1;
                        }
                        EffectOn = "1";
                    }
                    #endregion
                    #region 蓄力
                    else if (CardData.EffectType == 13)//AI每次攻击手牌攻击力+1
                    {
                        hasUseCard = true;
                        Destroy(CurrentCard);
                        if (CardData.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CardData.Consume;
                        }
                        AddBUFF(0, 0, CardData.EffectType, Convert.ToInt32(CardData.Effect), ref CardData);
                    }
                    #endregion
                    #region 闪避
                    else if (CardData.EffectType == 26)//百分之五十的闪避
                    {
                        hasUseCard = true;
                        Destroy(CurrentCard);
                        if (CardData.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CardData.Consume;
                        }
                        AddBUFF(0, 4, CardData.EffectType, Convert.ToInt32(CardData.Effect), ref CardData);
                    }
                    #endregion
                    #region 狂暴
                    else if (CardData.EffectType == 33)//下次攻击全能吸血
                    {
                        hasUseCard = true;
                        Destroy(CurrentCard);
                        if (CardData.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CardData.Consume;
                        }
                        AddBUFF(0, 4, CardData.EffectType, Convert.ToInt32(CardData.Effect), ref CardData);
                    }
                    #endregion
                }
                #endregion
                #region 卡牌拖动到敌人位置
                else if (BattleManager.instance.EnemyPlayerData.Find(a => a.playerID == CardToRoleID) != null)//卡牌拖动到敌人位置
                {
                    #region 攻击
                    if (CardData.EffectType == 1)//攻击
                    {
                        hasUseCard = true;
                        Destroy(CurrentCard);
                        if (CardData.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CardData.Consume;
                        }
                        int DeductionHp = Convert.ToInt32(CardData.Effect);
                        if (AiRole.Armor > 0)
                        {
                            if (AiRole.Armor >= CardData.Effect)
                            {
                                DeductionHp = 0;
                                AiRole.Armor -= Convert.ToInt32(CardData.Effect);
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
                        gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref CardData);
                    }
                    #endregion
                    #region 攻击后卡牌攻击力叠加
                    else if (CardData.EffectType == 14)//卡牌攻击力叠加
                    {
                        #region 攻击
                        Destroy(CurrentCard);
                        if (CardData.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CardData.Consume;
                        }
                        if (SuperPositionEffect < CardData.InitEffect)
                        {
                            SuperPositionEffect = CardData.InitEffect;
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
                        gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref CardData);
                        #endregion

                        var cardData = ownRole.handCardList.Find(a => a.SingleID == CardData.SingleID);
                        ActivateSuperPos = true;
                        SuperPositionEffect = cardData.InitEffect;
                        cardData.Effect = cardData.InitEffect;
                        hasUseCard = true;
                    }
                    #endregion
                    #region 连续攻击
                    else if (CardData.EffectType == 5)//连续攻击
                    {
                        hasUseCard = true;
                        Destroy(CurrentCard);
                        if (CardData.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CardData.Consume;
                        }
                        EffectOn = "0";
                    }
                    #endregion
                    #region 攻击无视防御
                    else if (CardData.EffectType == 7)//攻击无视防御
                    {
                        hasUseCard = true;
                        Destroy(CurrentCard);
                        if (CardData.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CardData.Consume;
                        }
                        int DeductionHp = Convert.ToInt32(CardData.Effect);
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
                    else if (CardData.EffectType == 6)//销毁防御
                    {
                        hasUseCard = true;
                        Destroy(CurrentCard);
                        if (CardData.Consume > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, CardData.Consume, 0, ownRole.EnergyMax);
                            ownRole.Energy -= CardData.Consume;
                        }
                        if (AiRole.Armor > 0)
                        {
                            AiRole.Armor = 0;
                            gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                            gameView.Eimg_Armor.transform.localScale = Vector3.zero;
                        }
                    }
                    #endregion
                    #region 火力全开
                    if (CardData.EffectType == 31)//攻击
                    {
                        hasUseCard = true;
                        Destroy(CurrentCard);
                        int DeductionHp = Convert.ToInt32(CardData.Effect * ownRole.Energy);
                        if (ownRole.Energy > 0)
                        {
                            Common.EnergyImgChange(ownRole.Energy, ownRole.Energy, 0, ownRole.EnergyMax);
                            ownRole.Energy = 0;
                        }
                        if (AiRole.Armor > 0)
                        {
                            if (AiRole.Armor >= CardData.Effect)
                            {
                                DeductionHp = 0;
                                AiRole.Armor -= Convert.ToInt32(CardData.Effect);
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
                        gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref CardData);
                    }
                    #endregion
                    #region 耗血攻击
                    if (CardData.EffectType == 35)//攻击
                    {
                        if (ownRole.bloodNow > CardData.Consume)
                        {
                            hasUseCard = true;
                            Destroy(CurrentCard);
                            #region 血量消耗
                            Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, CardData.Consume, 0);
                            ownRole.bloodNow -= Convert.ToInt32(CardData.Consume);
                            gameView.txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
                            #endregion

                            int DeductionHp = Convert.ToInt32(CardData.Effect);
                            if (AiRole.Armor > 0)
                            {
                                if (AiRole.Armor >= CardData.Effect)
                                {
                                    DeductionHp = 0;
                                    AiRole.Armor -= Convert.ToInt32(CardData.Effect);
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
                            gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref CardData);
                        }
                        else
                        {
                            hasUseCard = false;
                        }
                    }
                    #endregion

                }
                #endregion
            }
        }
    }

    /// <summary>
    /// 卡牌使用后的触发事件
    /// </summary>
    /// <param name="model"></param>
    /// <param name="DrawACard">所抽的卡列表</param>
    /// <param name="EffectOn">动画作用在谁身上，0AI，1角色</param>
    /// <returns>0没有动画、1卡牌回收、2攻击和卡牌回收、3防御和卡牌回收、4血量恢复和卡牌回收、5能量恢复和卡牌回收、6移除卡牌,7抽卡动画；
    /// 8攻击动画、9防御动画、10血量扣减和卡牌回收、11血量恢复动画、12血量扣减动画、13销毁防御、14销毁防御和卡牌回收、
    /// 25抽BeRemoveCardNum数量的手牌
    /// </returns>
    public int CardUseAfterTriggerEvent(CurrentCardPoolModel model, ref string EffectOn, ref List<CurrentCardPoolModel> DrawACard)
    {
        var ownRole = BattleManager.instance.OwnPlayerData[0];
        var AiRole = BattleManager.instance.EnemyPlayerData[0];
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
        bool hasExecuteOperation = true;//是否执行操作
        var unUseCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUnUsedCardPoolsFileName);
        var PlayerCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName);
        var PlayerHandCardList = BattleManager.instance.OwnPlayerData[0].handCardList;

        #region BUFF限制卡牌使用
        List<BUFFEffect> buffResult = null;
        if (PlayerOrAI == 0)
        {
            buffResult = BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref model, ref ownRole.handCardList);
        }
        else
        {
            buffResult = BUFFManager.instance.BUFFApply(ref AiRole.buffList, ownRole.buffList, ref model, ref AiRole.handCardList, false, 1);
        }
        if (buffResult?.Count > 0)
        {
            if (buffResult.Exists(a => a.EffectType == 1 && a.HasValid == false))//卡无效果未被使用
            {
                hasExecuteOperation = false;
            }
            if (buffResult.Exists(a => a.EffectType == 2 && a.HasValid == false))//卡无效果已被使用
            {
                hasExecuteOperation = false;
            }
        }
        #endregion

        if (hasExecuteOperation)
        {
            #region 卡牌使用后操作
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Common.DicDataRead(ref dic, GlobalAttr.EffectTypeFileName, "Card");
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
                        if (PlayerOrAI == 0)
                        {
                            if (AiRole.Armor > 0)
                            {
                                hasReachCondition = true;
                            }
                        }
                        else
                        {
                            if (ownRole.Armor > 0)
                            {
                                hasReachCondition = true;
                            }
                        }
                    }
                    else if (HasCrit)//暴击
                    {
                        hasReachCondition = true;
                    }
                    #endregion
                    if (hasReachCondition)
                    {
                        #region 攻击
                        if (item.TriggerState == 1)//攻击
                        {
                            if (PlayerOrAI == 0)
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
                                EffectOn = "0";
                                result = 2;
                                //攻击结束后、更新一遍buffUI操作
                                gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref model);
                            }
                            else
                            {
                                int DeductionHp = Convert.ToInt32(item.TriggerValue);
                                if (ownRole.Armor > 0)
                                {
                                    if (ownRole.Armor >= item.TriggerValue)
                                    {
                                        DeductionHp = 0;
                                        ownRole.Armor -= Convert.ToInt32(item.TriggerValue);
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
                                EffectOn = "1";
                                result = 8;
                                //攻击结束后、更新一遍buffUI操作
                                gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref model, 1);
                            }
                        }
                        #endregion
                        #region 防御
                        else if (item.TriggerState == 2)//防御
                        {
                            if (PlayerOrAI == 0)
                            {
                                gameView.Pimg_Armor.transform.localScale = Vector3.one;
                                ownRole.Armor += Convert.ToInt32(item.TriggerValue);
                                if (ownRole.Armor > ownRole.bloodMax)
                                {
                                    ownRole.Armor = Convert.ToInt32(ownRole.bloodMax);
                                }
                                gameView.txt_P_Armor.text = ownRole.Armor.ToString();
                                result = 3;
                                EffectOn = "1";
                            }
                            else
                            {
                                gameView.Eimg_Armor.transform.localScale = Vector3.one;
                                AiRole.Armor += Convert.ToInt32(item.TriggerValue);
                                if (AiRole.Armor > AiRole.bloodMax)
                                {
                                    AiRole.Armor = Convert.ToInt32(AiRole.bloodMax);
                                }
                                gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                                result = 9;
                                EffectOn = "0";
                            }
                        }
                        #endregion
                        #region 血量变化
                        else if (item.TriggerState == 3)//血量变化
                        {
                            if (PlayerOrAI == 0)
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
                                    result = 10;
                                }
                                else
                                {
                                    Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, item.TriggerValue, 1);
                                    ownRole.bloodNow += Convert.ToInt32(item.TriggerValue);
                                    if (ownRole.bloodNow > ownRole.bloodMax)
                                    {
                                        ownRole.bloodNow = ownRole.bloodMax;
                                    }
                                    result = 4;
                                }
                                gameView.txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
                                EffectOn = "1";
                            }
                            else
                            {
                                if (item.TriggerValue < 0)
                                {
                                    var effect = AiRole.bloodNow + Convert.ToInt32(item.TriggerValue);
                                    if (effect < 0)
                                    {
                                        AiRole.bloodNow = 0;
                                        //玩家死亡
                                        gameView.AiDie();
                                    }
                                    else
                                    {
                                        Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, item.TriggerValue, 0);
                                        AiRole.bloodNow += Convert.ToInt32(item.TriggerValue);
                                    }
                                    result = 12;
                                }
                                else
                                {
                                    Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, item.TriggerValue, 1);
                                    AiRole.bloodNow += Convert.ToInt32(item.TriggerValue);
                                    if (AiRole.bloodNow > AiRole.bloodMax)
                                    {
                                        AiRole.bloodNow = AiRole.bloodMax;
                                    }
                                    result = 11;
                                }
                                gameView.txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
                                EffectOn = "0";
                            }
                        }
                        #endregion
                        #region 能量变化
                        else if (item.TriggerState == 4)//能量变化
                        {
                            if (PlayerOrAI == 0)
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
                                        Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(item.TriggerValue * -1), 0, ownRole.EnergyMax);
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
                        }
                        #endregion
                        #region 销毁防御
                        else if (item.TriggerState == 6)//销毁防御
                        {
                            if (PlayerOrAI == 0)
                            {
                                if (AiRole.Armor > 0)
                                {
                                    AiRole.Armor = 0;
                                    gameView.txt_E_Armor.text = AiRole.Armor.ToString();
                                    gameView.Eimg_Armor.transform.localScale = Vector3.zero;
                                }
                                result = 14;
                                EffectOn = "0";
                            }
                            else
                            {
                                if (ownRole.Armor > 0)
                                {
                                    ownRole.Armor = 0;
                                    gameView.txt_P_Armor.text = AiRole.Armor.ToString();
                                    gameView.Pimg_Armor.transform.localScale = Vector3.zero;
                                }
                                result = 13;
                                EffectOn = "1";
                            }
                        }
                        #endregion
                        #region 无视防御攻击
                        else if (item.TriggerState == 7)//无视防御攻击
                        {
                            if (PlayerOrAI == 0)
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
                                EffectOn = "0";
                                result = 2;
                            }
                            else
                            {
                                int DeductionHp = Convert.ToInt32(item.TriggerValue);
                                Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, DeductionHp, 0);
                                ownRole.bloodNow -= DeductionHp;
                                if (ownRole.bloodNow <= 0)
                                {
                                    ownRole.bloodNow = 0;
                                    //AI死亡
                                    gameView.PlayerDie();
                                }
                                gameView.txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
                                EffectOn = "1";
                                result = 8;
                            }
                        }
                        #endregion
                        #region 愤怒、虚弱、暴击等buff
                        else if (item.TriggerState == 8)//愤怒
                        {
                            AddBUFF(0, 0, item.TriggerState, item.TriggerValue, ref model);
                        }
                        else if (item.TriggerState == 10)//暴击
                        {
                            AddBUFF(0, 4, item.TriggerState, item.TriggerValue, ref model);
                        }
                        else if (item.TriggerState == 11)//虚弱
                        {
                            AddBUFF(1, 0, item.TriggerState, item.TriggerValue, ref model);
                        }
                        #endregion
                        #region 免疫BUFF
                        else if (item.TriggerState == 9)//免疫
                        {
                            AddBUFF(0, 3, item.TriggerState, item.TriggerValue, ref model);
                        }
                        #endregion
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
                        #region 致盲
                        else if (item.TriggerState == 23)
                        {
                            AddBUFF(1, 4, item.TriggerState, item.TriggerValue, ref model);
                        }
                        #endregion
                        #region 抽同等手牌数量
                        else if (item.TriggerState == 25)
                        {
                            return 25;
                        }
                        #endregion
                        #region 混乱
                        else if (item.TriggerState == 29)
                        {
                            AddBUFF(0, 4, item.TriggerState, item.TriggerValue, ref model);
                        }
                        #endregion
                        #region 攻击力成长
                        else if (item.TriggerState == 32)
                        {
                            var handModel = ownRole.handCardList.Find(a => a.SingleID == model.SingleID);
                            handModel.Effect += item.TriggerValue;
                            handModel.InitEffect += item.TriggerValue;
                        }
                        #endregion
                        #region 束缚
                        else if (item.TriggerState == 20)
                        {
                            AddBUFF(0, 1, item.TriggerState, item.TriggerValue, ref model);
                        }
                        #endregion
                        #region 朝夕不保
                        else if (item.TriggerState == 34)
                        {
                            AddBUFF(0, 2, item.TriggerState, item.TriggerValue, ref model);
                        }
                        #endregion
                    }
                }
            }
            #endregion 
        }

        if (PlayerOrAI == 0)
        {
            if (CrtCardChange)
            {
                if (BeRemoveCardNum == 0)
                {
                    DestroyImmediate(PrevoiousCard);
                    CrtCardChange = false;
                    //放入已使用牌堆
                    if (hasPutInUseCards)
                    {
                        gameView.UsedCardList.Add(PrevoiousCardModel);
                        Common.SaveTxtFile(gameView.UsedCardList.ListToJson(), GlobalAttr.CurrentUsedCardPoolsFileName);
                        gameView.txt_Right_Count.text = gameView.UsedCardList == null ? "0" : gameView.UsedCardList.Count.ToString();
                    }
                    //移除当前手牌
                    gameView.ATKBarCardList.Remove(gameView.ATKBarCardList.Find(a => a.SingleID == PrevoiousCardModel.SingleID));
                }
            }
            else
            {
                DestroyImmediate(CurrentCard);
                if (BeRemoveCardNum == 0)//未有被移除的卡牌
                {
                    //放入已使用牌堆
                    if (hasPutInUseCards)
                    {
                        gameView.UsedCardList.Add(CurrentCardModel);
                        Common.SaveTxtFile(gameView.UsedCardList.ListToJson(), GlobalAttr.CurrentUsedCardPoolsFileName);
                        gameView.txt_Right_Count.text = gameView.UsedCardList == null ? "0" : gameView.UsedCardList.Count.ToString();
                    }
                    //移除当前手牌
                    var removeModel = gameView.ATKBarCardList.Find(a => a.SingleID == CurrentCardModel.SingleID);
                    gameView.ATKBarCardList.Remove(removeModel);
                }
            }
        }
        else
        {
            result = 0;
        }
        HasCrit = false;//恢复初始值
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
            var buffRes = BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref model, ref ownRole.handCardList);
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
            var buffRes = BUFFManager.instance.BUFFApply(ref AiRole.buffList, ownRole.buffList, ref model, ref AiRole.handCardList, false, 1);
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

    /// <summary>
    /// 增加BUFF
    /// </summary>
    /// <param name="EffectOn">buff作用在谁身上0角色1AI</param>
    /// <param name="buffType">buff类型0应用在卡牌数据上；(如愤怒) 1应用在卡上；（如束缚） 2应用在角色身上；（如重伤） 3免疫；（如免疫）4应用在攻击上；（如混乱）</param>
    /// <param name="buffEffectType">buff效果类型</param>
    /// <param name="buffNum">buff数量</param>
    /// <param name="buffCardData">buff卡数据</param>
    public void AddBUFF(int EffectOn, int buffType, int buffEffectType, int buffNum, ref CurrentCardPoolModel buffCardData)
    {
        var ownRole = BattleManager.instance.OwnPlayerData[0];
        var AiRole = BattleManager.instance.EnemyPlayerData[0];
        Dictionary<string, string> dic = new Dictionary<string, string>();
        Common.DicDataRead(ref dic, GlobalAttr.EffectTypeFileName, "Card");
        if (EffectOn == 0)//加到角色上
        {
            if (buffType == 3)//免疫
            {
                if (ownRole.buffList == null)
                {
                    var buffDatas = new List<BuffData>();
                    buffDatas.Add(new BuffData
                    {
                        Name = dic[buffEffectType.ToString()],
                        Num = buffNum,
                        EffectType = buffEffectType,
                        BUFFType = buffType
                    });
                    ownRole.buffList = buffDatas;
                    GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                    tempBuff = Common.AddChild(gameView.P_buffObj.transform, tempBuff);
                    tempBuff.name = "img_Buff_" + buffEffectType;
                    var img = tempBuff.GetComponent<Image>();
                    Common.ImageBind(buffCardData.CardUrl, img);
                    var txt_buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                    txt_buffNum.text = (Convert.ToInt32(txt_buffNum.text) + buffNum).ToString();
                    BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref buffCardData, ref ownRole.handCardList);
                }
                else
                {
                    var aa = ownRole.buffList.Find(a => a.Name == dic[buffEffectType.ToString()]);//是否以存在此buff
                    if (aa != null)
                    {
                        aa.Num += buffNum;
                    }
                    else
                    {
                        foreach (var item in ownRole.buffList)
                        {
                            item.Num = 0;
                        }
                        ownRole.buffList.Add(new BuffData
                        {
                            Name = dic[buffEffectType.ToString()],
                            Num = buffNum,
                            EffectType = buffEffectType,
                            BUFFType = 3
                        });
                        GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                        tempBuff = Common.AddChild(gameView.P_buffObj.transform, tempBuff);
                        tempBuff.name = "img_Buff_" + buffEffectType;
                        var img = tempBuff.GetComponent<Image>();
                        Common.ImageBind(buffCardData.CardUrl, img);
                        var txt_buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                        txt_buffNum.text = (Convert.ToUInt32(txt_buffNum.text) + buffNum).ToString();
                        //UI操作
                        gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref buffCardData);
                        BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref buffCardData, ref ownRole.handCardList);
                    }
                }
            }
            else
            {
                #region BUff添加
                if (ownRole.buffList == null)
                {
                    var buffDatas = new List<BuffData>();
                    buffDatas.Add(new BuffData
                    {
                        Name = dic[buffEffectType.ToString()],
                        Num = buffNum,
                        EffectType = buffEffectType,
                        BUFFType = buffType
                    });
                    ownRole.buffList = buffDatas;
                    GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                    tempBuff = Common.AddChild(gameView.P_buffObj.transform, tempBuff);
                    tempBuff.name = "img_Buff_" + buffEffectType;
                    var img = tempBuff.GetComponent<Image>();
                    Common.ImageBind(buffCardData.CardUrl, img);
                    var txt_buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                    txt_buffNum.text = (Convert.ToInt32(txt_buffNum.text) + buffNum).ToString();
                    bool dataChange = false;
                    if (buffEffectType == 8 || buffEffectType == 11)
                    {
                        dataChange = true;
                    }
                    BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref buffCardData, ref ownRole.handCardList, dataChange);
                }
                else
                {
                    if (!ownRole.buffList.Exists(a => a.EffectType == 9))//免疫状态下所有buff卡不可使用
                    {
                        string buffName = dic[buffEffectType.ToString()];
                        var aa = ownRole.buffList.Find(a => a.Name == buffName);//是否以存在此buff
                        if (aa != null)
                        {
                            aa.Num += buffNum;
                            //UI操作
                            gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref buffCardData);
                        }
                        else
                        {
                            //一种类型只能存在一条。比如虚弱和愤怒不能共存
                            var buffs = ownRole.buffList.Find(a => a.BUFFType == buffType);
                            if (buffs != null)
                            {
                                ownRole.buffList.Find(a => a.BUFFType == buffType).Num = 0;
                            }
                            ownRole.buffList.Add(new BuffData
                            {
                                Name = dic[buffEffectType.ToString()],
                                Num = buffNum,
                                EffectType = buffEffectType,
                                BUFFType = buffType
                            });
                            GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                            tempBuff = Common.AddChild(gameView.P_buffObj.transform, tempBuff);
                            tempBuff.name = "img_Buff_" + buffEffectType;
                            var img = tempBuff.GetComponent<Image>();
                            Common.ImageBind(buffCardData.CardUrl, img);
                            var txt_buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                            txt_buffNum.text = (Convert.ToUInt32(txt_buffNum.text) + buffNum).ToString();
                            //UI操作
                            gameView.BUFFUIChange(ownRole.buffList, ref ownRole.handCardList, ref buffCardData);
                            bool dataChange = false;
                            if (buffEffectType == 8 || buffEffectType == 11)
                            {
                                dataChange = true;
                            }
                            BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref buffCardData, ref ownRole.handCardList, dataChange);
                        }
                    }
                }
                #endregion
            }
        }
        else if (EffectOn == 1)//加到AI上
        {
            if (buffType == 3)//免疫
            {
                var aa = AiRole.buffList.Find(a => a.Name == dic[buffEffectType.ToString()]);//是否以存在此buff
                if (aa != null)
                {
                    aa.Num += buffNum;
                }
                else
                {
                    AiRole.buffList = new List<BuffData>();
                    AiRole.buffList.Add(new BuffData
                    {
                        Name = dic[buffEffectType.ToString()],
                        Num = buffNum,
                        EffectType = buffEffectType,
                        BUFFType = 3
                    });
                    GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                    tempBuff = Common.AddChild(gameView.E_buffObj.transform, tempBuff);
                    tempBuff.name = "img_Buff_" + buffEffectType;
                    var img = tempBuff.GetComponent<Image>();
                    Common.ImageBind(buffCardData.CardUrl, img);
                    var txt_buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                    txt_buffNum.text = (Convert.ToUInt32(txt_buffNum.text) + buffNum).ToString();
                    //UI操作
                    gameView.BUFFUIChange(AiRole.buffList, ref AiRole.handCardList, ref buffCardData);
                    BUFFManager.instance.BUFFApply(ref AiRole.buffList, ownRole.buffList, ref buffCardData, ref AiRole.handCardList, false, 1);
                }
            }
            else
            {
                #region BUff添加

                if (AiRole.buffList == null)
                {
                    var buffDatas = new List<BuffData>();
                    buffDatas.Add(new BuffData
                    {
                        Name = dic[buffEffectType.ToString()],
                        Num = buffNum,
                        EffectType = buffEffectType,
                        BUFFType = buffType
                    });
                    AiRole.buffList = buffDatas;
                    GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                    tempBuff = Common.AddChild(gameView.E_buffObj.transform, tempBuff);
                    tempBuff.name = "img_Buff_" + buffEffectType;
                    var img = tempBuff.GetComponent<Image>();
                    Common.ImageBind(buffCardData.CardUrl, img);
                    var txt_buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                    txt_buffNum.text = (Convert.ToUInt32(txt_buffNum.text) + buffNum).ToString();
                    bool dataChange = false;
                    if (buffEffectType == 8 || buffEffectType == 11)
                    {
                        dataChange = true;
                    }
                    BUFFManager.instance.BUFFApply(ref AiRole.buffList, ownRole.buffList, ref buffCardData, ref AiRole.handCardList, dataChange, 1);
                }
                else
                {
                    if (!AiRole.buffList.Exists(a => a.EffectType == 9))//免疫状态下所有buff卡不可使用
                    {
                        string buffName = dic[buffEffectType.ToString()];
                        var aa = AiRole.buffList.Find(a => a.Name == buffName);//是否以存在此buff
                        if (aa != null)
                        {
                            aa.Num += buffNum;
                            //UI操作
                            gameView.BUFFUIChange(AiRole.buffList, ref AiRole.handCardList, ref buffCardData, 1);
                        }
                        else
                        {
                            //一种类型只能存在一条。比如虚弱和愤怒不能共存
                            var buffs = AiRole.buffList.Find(a => a.BUFFType == buffType);
                            if (buffs != null)
                            {
                                AiRole.buffList.Find(a => a.BUFFType == 0).Num = buffType;
                            }
                            AiRole.buffList.Add(new BuffData
                            {
                                Name = dic[buffEffectType.ToString()],
                                Num = buffNum,
                                EffectType = buffEffectType,
                                BUFFType = buffType
                            });
                            GameObject tempBuff = ResourcesManager.instance.Load("img_Buff") as GameObject;
                            tempBuff = Common.AddChild(gameView.E_buffObj.transform, tempBuff);
                            tempBuff.name = "img_Buff_" + buffEffectType;
                            var img = tempBuff.GetComponent<Image>();
                            Common.ImageBind(buffCardData.CardUrl, img);
                            var txt_buffNum = tempBuff.transform.Find("Text").GetComponent<Text>();
                            txt_buffNum.text = (Convert.ToUInt32(txt_buffNum.text) + buffNum).ToString();
                            //UI操作
                            gameView.BUFFUIChange(AiRole.buffList, ref AiRole.handCardList, ref buffCardData, 1);
                            bool dataChange = false;
                            if (buffEffectType == 8 || buffEffectType == 11)
                            {
                                dataChange = true;
                            }
                            BUFFManager.instance.BUFFApply(ref AiRole.buffList, ownRole.buffList, ref buffCardData, ref AiRole.handCardList, dataChange, 1);
                        }
                    }
                }
                #endregion
            }
        }
    }

    /// <summary>
    /// 血量变化
    /// </summary>
    /// <param name="Effect">变化的血量</param>
    /// <param name="PlayerOrAI">作用到玩家或AI身上，0玩家1AI</param>
    public void HPChange(float EffectHP, int PlayerOrAI)
    {
        var ownRole = BattleManager.instance.OwnPlayerData[0];
        var AiRole = BattleManager.instance.EnemyPlayerData[0];
        #region 血量变化
        if (PlayerOrAI == 0)
        {
            if (EffectHP < 0)
            {
                var effect = ownRole.bloodNow + Convert.ToInt32(EffectHP);
                if (effect < 0)
                {
                    ownRole.bloodNow = 0;
                    //玩家死亡
                    gameView.PlayerDie();
                }
                else
                {
                    Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, EffectHP, 0);
                    ownRole.bloodNow += Convert.ToInt32(EffectHP);
                }
            }
            else
            {
                Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, EffectHP, 1);
                ownRole.bloodNow += Convert.ToInt32(EffectHP);
                if (ownRole.bloodNow > ownRole.bloodMax)
                {
                    ownRole.bloodNow = ownRole.bloodMax;
                }
            }
            gameView.txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
        }
        else
        {
            if (EffectHP < 0)
            {
                var effect = AiRole.bloodNow + Convert.ToInt32(EffectHP);
                if (effect < 0)
                {
                    AiRole.bloodNow = 0;
                    //玩家死亡
                    gameView.AiDie();
                }
                else
                {
                    Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, EffectHP, 0);
                    AiRole.bloodNow += Convert.ToInt32(EffectHP);
                }
            }
            else
            {
                Common.HPImageChange(gameView.Eimg_HP, AiRole.bloodMax, EffectHP, 1);
                AiRole.bloodNow += Convert.ToInt32(EffectHP);
                if (AiRole.bloodNow > AiRole.bloodMax)
                {
                    AiRole.bloodNow = AiRole.bloodMax;
                }
            }
            gameView.txt_E_HP.text = $"{AiRole.bloodMax}/{AiRole.bloodNow}";
        }
        #endregion
    }

}
