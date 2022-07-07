using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗管理器
/// </summary>
public class BattleManager : SingletonMonoBehaviour<BattleManager>
{
    public List<PlayerData> OwnPlayerData = new List<PlayerData>();
    public List<PlayerData> EnemyPlayerData = new List<PlayerData>();
    public StateMachine BattleStateMachine;
    public List<int> OwnCards = new List<int>();//这里的卡牌ID不用表ID，而是唯一ID，保证唯一性
    public List<int> EnemyCards = new List<int>();//这里的卡牌ID不用表ID，而是唯一ID，保证唯一性
    public List<int> OwnHandCards = new List<int>();//这里的卡牌ID不用表ID，而是唯一ID，保证唯一性
    public List<int> EnemyHandCards = new List<int>();//这里的卡牌ID不用表ID，而是唯一ID，保证唯一性
    public List<int> PlayControlIndexList = new List<int>();
    public int controlIndex;//当前出手

    private Action UpdateAction;
    public void Init(List<PlayerData> ownPlayerData, List<PlayerData> enemyPlayerData)
    {
        ResetBattle();
        this.OwnPlayerData = ownPlayerData;
        this.EnemyPlayerData = enemyPlayerData;
        if (BattleStateMachine == null)
        {
            InitBattleMachine();
        }
        BattleStateMachine.InitState(BattleStateID.Ready);
        StartBattle();
    }

    /// <summary>
    /// 初始化状态机
    /// </summary>
    private void InitBattleMachine()
    {
        BattleStateMachine = new StateMachine();
        BattleStateMachine.AddState(new Battle_Ready());
        BattleStateMachine.AddState(new Battle_DrawCard());
        BattleStateMachine.AddState(new Battle_TurnStart());
        BattleStateMachine.AddState(new Battle_Control());
        BattleStateMachine.AddState(new Battle_PlayEffect());
        BattleStateMachine.AddState(new Battle_TurnEnd());
        BattleStateMachine.AddState(new Battle_GameEnd());
        UpdateAction = () => { BattleStateMachine.ExecuteState(); };
    }

    /// <summary>
    /// 开始战斗
    /// </summary>
    public void StartBattle()
    {
        InitControlIndex();
        controlIndex = 0;
        BattleStateMachine.ChangeState(BattleStateID.TurnStart);
    }
    

    /// <summary>
    /// 确定出手顺序
    /// </summary>
    private void InitControlIndex()
    {
        List<int> speedList = new List<int>();
        List<int> idList = new List<int>();
        for (int i = 0; i < OwnPlayerData.Count; i++)
        {
            idList.Add(OwnPlayerData[i].playerID);
            speedList.Add(OwnPlayerData[i].speed);
        }
        for (int i = 0; i < EnemyPlayerData.Count; i++)
        {
            idList.Add(EnemyPlayerData[i].playerID);
            speedList.Add(EnemyPlayerData[i].speed);
        }
        for (int i = 0; i < speedList.Count - 1; i++)
        {
            if (speedList[i] > speedList[i + 1])
            {
                int tempSpeed = speedList[i];
                speedList[i] = speedList[i + 1];
                speedList[i + 1] = tempSpeed;
                int tempID = idList[i];
                idList[i] = idList[i + 1];
                idList[i + 1] = tempID;
            }
        }
        PlayControlIndexList = idList;
    }

    /// <summary>
    /// 重置战斗
    /// </summary>
    public void ResetBattle()
    {
        UpdateAction = null;
    }
    private void Update()
    {
        if (UpdateAction != null)
        {
            UpdateAction();
        }
    }
}

/// <summary>
/// 准备
/// </summary>
public class Battle_Ready : State
{
    public Battle_Ready()
    {
        ID = BattleStateID.Ready;
    }
    public override void Enter() { }
    public override void Execute() { }
    public override void Exit() { }
}

/// <summary>
/// 回合开始
/// </summary>
public class Battle_TurnStart : State
{
    public Battle_TurnStart()
    {
        ID = BattleStateID.TurnStart;
    }
    public override void Enter() { }
    public override void Execute() { }
    public override void Exit() { }
}

/// <summary>
/// 抽卡
/// </summary>
public class Battle_DrawCard : State
{
    public Battle_DrawCard()
    {
        ID = BattleStateID.DrawCard;
    }
    public override void Enter() { }
    public override void Execute() { }
    public override void Exit() { }
}

/// <summary>
/// 出牌
/// </summary>
public class Battle_Control : State
{
    public Battle_Control()
    {
        ID = BattleStateID.Control;
    }
    public override void Enter() { }
    public override void Execute() { }
    public override void Exit() { }
}

/// <summary>
/// 结算/处理效果
/// </summary>
public class Battle_PlayEffect : State
{
    public Battle_PlayEffect()
    {
        ID = BattleStateID.PlayEffect;
    }
    public override void Enter() { }
    public override void Execute() { }
    public override void Exit() { }
}

/// <summary>
/// 回合结束
/// </summary>
public class Battle_TurnEnd : State
{
    public Battle_TurnEnd()
    {
        ID = BattleStateID.TurnEnd;
    }
    public override void Enter() { }
    public override void Execute() { }
    public override void Exit() { }
}

/// <summary>
/// 游戏结束
/// </summary>
public class Battle_GameEnd : State
{
    public Battle_GameEnd()
    {
        ID = BattleStateID.GameEnd;
    }
    public override void Enter() { }
    public override void Execute() { }
    public override void Exit() { }
}
