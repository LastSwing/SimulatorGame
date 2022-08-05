using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUFFManager : SingletonMonoBehaviour<BUFFManager>
{

    GameView gameView = UIManager.instance.GetView("GameView") as GameView;
    /// <summary>
    /// BUFF应用，在回合开始时作用在游戏中
    /// </summary>
    /// <param name="buffDatas">当前角色BUFF列表</param>
    /// <param name="RivalBuff">对手BUFF</param>
    /// <param name="handCards">手牌</param>
    /// <param name="model">攻击牌</param>
    /// <param name="DataChange">是否数据变化</param>
    /// <param name="PlayerOrAI">角色或AI。0角色，1AI</param>
    /// <returns>作用在卡和作用在角色身上时返回</returns>
    public List<BUFFEffect> BUFFApply(ref List<BuffData> buffDatas, List<BuffData> RivalBuff, ref CurrentCardPoolModel model, ref List<CurrentCardPoolModel> handCards, bool DataChange = false, int PlayerOrAI = 0)
    {
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
                                gameView.RealTimeChangeCardData(handCards, PlayerOrAI);
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
                                gameView.RealTimeChangeCardData(handCards, PlayerOrAI);
                            }
                            break;
                        case 9://免疫
                            return new List<BUFFEffect>();
                        case 10://暴击
                            if (DataChange)
                            {
                                if (model.CardType == 0)
                                {
                                    model.Effect *= 2;
                                    item.Num -= 1;
                                    //UI操作
                                    gameView.BUFFUIChange(buffDatas, ref handCards, ref model, PlayerOrAI);
                                }
                            }
                            break;
                        case 20://束缚
                            bEffect.ID = list.Count + 1;
                            bEffect.HasValid = false;
                            bEffect.UnableUse = 0;
                            bEffect.Sort = list.Count;
                            bEffect.EffectType = 1;
                            list.Add(bEffect);
                            break;
                        case 21://缠绕
                            bEffect.ID = list.Count + 1;
                            bEffect.HasValid = false;
                            bEffect.UnableUse = 1;
                            bEffect.Sort = list.Count;
                            bEffect.EffectType = 1;
                            list.Add(bEffect);
                            break;
                        case 22://重伤
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
                                    list.Add(bEffect);
                                }
                                else
                                {
                                    model.Effect *= 2;
                                }
                            }
                            break;
                        case 33://狂暴 只作用一次
                                //每次使用攻击卡都直接扣减狂暴次数
                            if (model.CardType == 0)
                            {
                                bEffect.ID = list.Count + 1;
                                bEffect.HPChange = Convert.ToInt32(model.Effect);
                                bEffect.Sort = list.Count;
                                list.Add(bEffect);
                                item.Num -= 1;
                                //UI操作
                                gameView.BUFFUIChange(buffDatas, ref handCards, ref model, PlayerOrAI);
                            }
                            break;
                    }
                }
            }
        }
        if (RivalBuff?.Count > 0)
        {
            //如果对手存在闪避buff此攻击有50%无效
            if (RivalBuff.Exists(a => a.EffectType == 26))
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
        }
        return list;
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
    /// 排序
    /// </summary>
    public int Sort { get; set; }

}