﻿
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateMachine
{
    public Dictionary<int, State> dictionary = new Dictionary<int, State>();
    public State CurrentState { set; get; }
    /// <summary>
    /// 上一个状态
    /// </summary>
    public State PreviousState { get; set; }

    public State GetState(int id)
    {
        if (dictionary.ContainsKey(id))
            return dictionary[id];
        else
            return null;
    }

    public void AddState(State state)
    {
        int id = state.ID;
        if (!dictionary.ContainsKey(id))
            dictionary.Add(id, state);
    }

    public void RemoveState(int id)
    {
        if (dictionary.ContainsKey(id))
            dictionary.Remove(id);
    }

    public void InitState(int id)
    {
        if (CurrentState == null)
        {
            CurrentState = dictionary[id];
            CurrentState.Enter();
        }
    }

    public void ChangeState(int id)
    {
        if (CurrentState.ID != id)
        {
            CurrentState.Exit();
            PreviousState = CurrentState;
            CurrentState = dictionary[id];
            CurrentState.Enter();
        }
    }

    public void ExecuteState()
    {
        CurrentState.Execute();
    }

    public bool CheckState(int id)
    {
        return CurrentState.ID == id;
    }
}
public static class BattleStateID
{
    /// <summary>
    /// 准备
    /// </summary>
    public const int Ready = 1000;
    /// <summary>
    /// 回合开始阶段
    /// </summary>
    public const int TurnStart = 2000;
    /// <summary>
    /// 回合抽卡阶段
    /// </summary>
    public const int DrawCard = 3000;
    /// <summary>
    /// 出牌
    /// </summary>
    public const int Control = 4000;
    /// <summary>
    /// 卡牌使用前状态
    /// </summary>
    public const int BeforeCardUse = 5000;
    /// <summary>
    /// 结算效果阶段只用于结算
    /// </summary>
    public const int EffectSettlement = 6000;
    /// <summary>
    /// 卡牌使用后状态
    /// </summary>
    public const int AfterCardUse = 7000;
    /// <summary>
    /// 回合结束
    /// </summary>
    public const int TurnEnd = 8000;
    /// <summary>
    /// 游戏结束
    /// </summary>
    public const int GameEnd = 9000;
}