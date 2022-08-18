using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUFFManager : SingletonMonoBehaviour<BUFFManager>
{

    /// <summary>
    /// BUFF应用，在回合开始时作用在游戏中
    /// </summary>
    /// <param name="buffDatas">当前角色BUFF列表</param>
    /// <param name="RivalBuff">对手BUFF</param>
    /// <param name="handCards">手牌</param>
    /// <param name="model">攻击牌</param>
    /// <param name="DataChange">是否数据变化</param>
    /// <param name="PlayerOrAI">作用在角色或AI。0角色，1AI</param>
    /// <returns>作用在卡和作用在角色身上时返回</returns>
    public List<BUFFEffect> BUFFApply(ref List<BuffData> buffDatas, List<BuffData> RivalBuff, ref CurrentCardPoolModel model, ref List<CurrentCardPoolModel> handCards, bool DataChange = false, int PlayerOrAI = 0)
    {
        GameView gameView = UIManager.instance.GetView("GameView") as GameView;
        List<BUFFEffect> list = new List<BUFFEffect>();
        if (buffDatas?.Count > 0)
        {
            foreach (var item in buffDatas)
            {
                BUFFEffect bEffect = new BUFFEffect();
                if (item.Num > 0)
                {
                    switch (item.EffectType)
                    {
                        case 8://愤怒
                            if (DataChange)
                            {
                                foreach (var card in handCards)
                                {
                                    if (card.CardType == 0)
                                    {
                                        card.Effect = Convert.ToInt32(card.Effect + Math.Ceiling(card.InitEffect * 0.2f));
                                    }
                                }
                                gameView.RealTimeChangeCardData(PlayerOrAI);
                            }
                            break;
                        case 11://虚弱
                            if (DataChange)
                            {
                                foreach (var card in handCards)
                                {
                                    if (card.CardType == 0)
                                    {
                                        card.Effect = Convert.ToInt32(card.Effect - Math.Ceiling(card.InitEffect * 0.2f));
                                    }
                                }
                                gameView.RealTimeChangeCardData(PlayerOrAI);
                            }
                            break;
                        case 9://免疫
                            return new List<BUFFEffect>();
                        case 10://暴击
                            if (model.CardType == 0)
                            {
                                if (model.EffectType == 1 || model.EffectType == 5 || model.EffectType == 7 || model.EffectType == 14 || model.EffectType == 31 || model.EffectType == 32)
                                {
                                    CardUseEffectManager.instance.HasNumberValueChange = true;
                                    model.Effect *= 2;
                                    item.Num -= 1;
                                    CardUseEffectManager.instance.HasCrit = true;
                                }
                            }
                            break;
                        case 20://束缚
                            if (model.CardType == 0)
                            {
                                bEffect.ID = list.Count + 1;
                                bEffect.HasValid = false;
                                bEffect.UnableUse = 0;
                                bEffect.Sort = list.Count;
                                bEffect.EffectType = 1;
                                list.Add(bEffect);
                            }
                            break;
                        case 21://缠绕
                            if (model.CardType == 1)
                            {
                                bEffect.ID = list.Count + 1;
                                bEffect.HasValid = false;
                                bEffect.UnableUse = 1;
                                bEffect.Sort = list.Count;
                                bEffect.EffectType = 1;
                                list.Add(bEffect);
                            }
                            break;
                        case 22://重伤
                            bEffect.BUFFType = 2;
                            bEffect.ID = list.Count + 1;
                            bEffect.HPChange = Convert.ToInt32(item.Num) * -1;
                            bEffect.Sort = list.Count;
                            list.Add(bEffect);
                            break;
                        case 23://致盲
                            if (model.CardType == 0)
                            {
                                int random = UnityEngine.Random.Range(0, 2);
                                if (random == 0)
                                {
                                    bEffect.ID = list.Count + 1;
                                    bEffect.HasValid = false;
                                    bEffect.UnableUse = 0;
                                    bEffect.Sort = list.Count;
                                    bEffect.EffectType = 2;
                                    list.Add(bEffect);
                                }
                            }
                            break;
                        case 29://混乱 50%暴击，50混乱
                            if (model.CardType == 0)
                            {
                                int random = UnityEngine.Random.Range(0, 2);
                                if (random == 0)
                                {
                                    bEffect.ID = list.Count + 1;
                                    bEffect.HasValid = false;
                                    bEffect.UnableUse = 0;
                                    bEffect.Sort = list.Count;
                                    bEffect.EffectType = 2;
                                    list.Add(bEffect);
                                }
                                else
                                {
                                    CardUseEffectManager.instance.HasNumberValueChange = true;
                                    model.Effect *= 2;
                                    CardUseEffectManager.instance.HasCrit = true;
                                }
                            }
                            break;
                        case 33://狂暴 只作用一次
                                //每次使用攻击卡都直接扣减狂暴次数
                            if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.EffectSettlement)
                            {
                                if (model.CardType == 0)
                                {
                                    int changeHp = Convert.ToInt32(model.Effect);
                                    if (model.EffectType == 31)
                                    {
                                        changeHp = Convert.ToInt32(model.Effect * model.Consume);
                                    }
                                    else if (model.EffectType == 5)
                                    {
                                        changeHp = Convert.ToInt32(model.Effect * model.AtkNumber);
                                    }
                                    bEffect.ID = list.Count + 1;
                                    bEffect.HPChange = changeHp;
                                    bEffect.Sort = list.Count;
                                    list.Add(bEffect);
                                    item.Num -= 1;
                                }
                            }
                            break;
                    }
                }
            }
        }
        if (RivalBuff?.Count > 0 && model != null)
        {
            #region 闪避
            //如果对手存在闪避buff此攻击有50%无效
            if (RivalBuff.Exists(a => a.EffectType == 26))
            {
                if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.EffectSettlement)//在效果结算状态下
                {
                    if (model.CardType == 0)
                    {
                        var buff = RivalBuff.Find(a => a.EffectType == 26);
                        if (buff.Num > 0)
                        {
                            int random = UnityEngine.Random.Range(0, 2);
                            if (random == 0)
                            {
                                BUFFEffect bEffect = new BUFFEffect();
                                bEffect.ID = list.Count + 1;
                                bEffect.HasValid = false;
                                bEffect.UnableUse = 0;
                                bEffect.Sort = list.Count;
                                bEffect.EffectType = 2;
                                list.Add(bEffect);
                            }
                        }
                    }
                }
                else if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.AfterCardUse && model.TriggerAfterUsingList?.Count > 0)//在卡牌使用后状态下要是攻击状态
                {
                    var afterAtkList = model.TriggerAfterUsingList;
                    foreach (var item in afterAtkList)
                    {
                        switch (item.TriggerState)
                        {
                            case 2:
                            case 7:
                                var buff = RivalBuff.Find(a => a.EffectType == 26);
                                if (buff.Num > 0)
                                {
                                    int random = UnityEngine.Random.Range(0, 2);
                                    if (random == 0)
                                    {
                                        BUFFEffect bEffect = new BUFFEffect();
                                        bEffect.ID = list.Count + 1;
                                        bEffect.HasValid = false;
                                        bEffect.UnableUse = 0;
                                        bEffect.Sort = list.Count;
                                        bEffect.EffectType = 2;
                                        list.Add(bEffect);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            #endregion

            #region 对手每次攻击自己手牌攻击力加1
            if (RivalBuff.Exists(a => a.EffectType == 13))
            {
                if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.EffectSettlement)//在效果结算状态下
                {
                    if (model.CardType == 0)
                    {
                        RivalATKHandCardEffectChange(PlayerOrAI);
                    }
                }
                else if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.AfterCardUse && model.TriggerAfterUsingList?.Count > 0)//在卡牌使用后状态下要是攻击状态
                {
                    var afterAtkList = model.TriggerAfterUsingList;
                    foreach (var item in afterAtkList)
                    {
                        switch (item.TriggerState)
                        {
                            case 2:
                            case 7:
                                RivalATKHandCardEffectChange(PlayerOrAI);
                                break;
                        }
                    }
                }
            }
            #endregion

            #region 朝夕不保
            //敌人存在此标记，攻击照成暴击
            if (RivalBuff.Exists(a => a.EffectType == 34))
            {
                if (model.CardType == 0)
                {
                    CardUseEffectManager.instance.HasNumberValueChange = true;
                    model.Effect *= 2;
                    CardUseEffectManager.instance.HasCrit = true;
                }
            }
            #endregion
        }
        return list;
    }

    /// <summary>
    /// 对手攻击手牌数值变化
    /// </summary>
    /// <param name="PlayerOrAI">0玩家攻击1AI攻击</param>
    public void RivalATKHandCardEffectChange(int PlayerOrAI)
    {
        GameView gameView = UIManager.instance.GetView("GameView") as GameView;
        if (PlayerOrAI == 1)
        {
            foreach (var data in BattleManager.instance.OwnPlayerData[0].handCardList)
            {
                if (data.CardType == 0)
                {
                    data.Effect += 1;
                }
            }
            gameView.RealTimeChangeCardData(0);
        }
        else
        {
            foreach (var data in BattleManager.instance.EnemyPlayerData[0].handCardList)
            {
                if (data.CardType == 0)
                {
                    data.Effect += 1;
                }
            }
            gameView.RealTimeChangeCardData(1);
        }
    }
}

/// <summary>
/// BUFF效果
/// </summary>
public class BUFFEffect
{
    public int ID { get; set; }
    /// <summary>
    /// 卡牌是否有效
    /// </summary>
    public bool HasValid { get; set; } = true;
    /// <summary>
    /// 不能使用的卡
    /// 0攻击、1功能
    /// </summary>
    public int UnableUse { get; set; }

    /// <summary>
    /// 效果类型
    /// 1卡未被消耗
    /// 2卡已被消耗
    /// </summary>
    public int EffectType { get; set; }

    /// <summary>
    /// 血量变化
    /// </summary>
    public int HPChange { get; set; }

    /// <summary>
    /// 0应用在卡牌数据上；(如愤怒)
    /// 1应用在卡上；（如束缚）
    /// 2应用在角色身上；（如重伤）
    /// 3免疫；（如免疫）
    /// 4应用在攻击上；（如混乱）
    /// </summary>
    public int BUFFType { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

}