
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateMachine
{
    public Dictionary<int, State> dictionary = new Dictionary<int, State>();
    public State CurrentState { set; get; }

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
    public const int Ready = 1000;//准备
    public const int TurnStart = 2000;//回合开始阶段
    public const int DrawCard = 3000;//回合抽卡阶段
    public const int Control = 4000;//出牌
    public const int PlayEffect = 5000;//播放效果结算
    public const int TurnEnd = 6000;//回合结束
    public const int GameEnd = 7000;//游戏结束
}