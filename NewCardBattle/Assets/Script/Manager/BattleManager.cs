using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 战斗管理器
/// </summary>
public class BattleManager : SingletonMonoBehaviour<BattleManager>
{
    public List<PlayerData> OwnPlayerData = new List<PlayerData>();
    public List<PlayerData> EnemyPlayerData = new List<PlayerData>();
    public StateMachine BattleStateMachine;
    public List<int> OwnCards = new List<int>();//这里的卡牌ID不用表ID，而是唯一ID，保证唯一性;玩家牌库ID
    public List<int> EnemyCards = new List<int>();//这里的卡牌ID不用表ID，而是唯一ID，保证唯一性
    public List<int> OwnHandCards = new List<int>();//这里的卡牌ID不用表ID，而是唯一ID，保证唯一性;当前玩家牌库牌堆ID
    public List<int> EnemyHandCards = new List<int>();//这里的卡牌ID不用表ID，而是唯一ID，保证唯一性
    public List<int> PlayControlIndexList = new List<int>();//出手顺序，存角色ID
    public int controlIndex;//当前出手

    private Action UpdateAction;

    public void InitCardPools(List<int> ownCards, List<int> enemyCards, List<int> ownHandCards, List<int> enemyHandCards)
    {
        this.OwnCards = ownCards;
        this.EnemyCards = enemyCards;
        this.OwnHandCards = ownHandCards;
        this.EnemyHandCards = enemyHandCards;
    }

    public void Init(List<PlayerData> ownPlayerData, List<PlayerData> enemyPlayerData)
    {
        ResetBattle();
        this.OwnPlayerData = ownPlayerData;
        this.EnemyPlayerData = enemyPlayerData;
        if (BattleStateMachine == null)
        {
            InitBattleMachine();
        }
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
        BattleStateMachine.InitState(BattleStateID.Ready);
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

    /// <summary>
    /// 获取当前出手数据
    /// </summary>
    /// <returns></returns>
    public PlayerData GetCurrentPlayerData()
    {
        List<PlayerData> Result = OwnPlayerData.Concat(EnemyPlayerData).ToList();
        int num = PlayControlIndexList[controlIndex];
        PlayerData nowPlayer = Result.Find(p => p.playerID == num);
        return nowPlayer;
    }
}

/// <summary>
/// 准备
/// </summary>
public class Battle_Ready : State
{
    GameView gameView = UIManager.instance.GetView("GameView") as GameView;
    float deltaTime = 0;
    float StartAnimDutation;
    public Battle_Ready()
    {
        ID = BattleStateID.Ready;
    }
    public override void Enter()
    {
        AnimationManager.instance.DoAnimation("Anim_Shuffle", null);
        StartAnimDutation = AnimationManager.instance.DoAnimation("Anim_ShowTitle", null);
    }
    public override void Execute()
    {
        deltaTime += Time.deltaTime;
        if (deltaTime > StartAnimDutation && StartAnimDutation > 0)
        {
            //数据初始化完成后，应转到抽卡状态
            BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
            deltaTime = 0;
            StartAnimDutation = 0;
        }
    }
    public override void Exit()
    {
        gameView.AnimObj.SetActive(false);
    }
}

/// <summary>
/// 回合开始
/// </summary>
public class Battle_TurnStart : State
{
    GameView gameView = UIManager.instance.GetView("GameView") as GameView;
    float deltaTime = 0;
    float TitleDuration;
    public Battle_TurnStart()
    {
        ID = BattleStateID.TurnStart;
    }
    public override void Enter()
    {
        gameView.AnimObj.SetActive(true);

        PlayerData nowPlayer = BattleManager.instance.GetCurrentPlayerData();
        switch (nowPlayer.playerType)
        {
            case PlayerType.OwnHuman:
                if (BattleManager.instance.BattleStateMachine.PreviousState.ID != BattleStateID.Ready)
                {
                    TitleDuration = AnimationManager.instance.DoAnimation("Anim_ShowTitle", new object[] { "你的回合" });
                }
                break;
            case PlayerType.NormalRobot:
            case PlayerType.AiRobot:
                TitleDuration = AnimationManager.instance.DoAnimation("Anim_ShowTitle", new object[] { "AI回合" });
                break;
            case PlayerType.OtherHuman:
                break;
        }

    }
    public override void Execute()
    {
        deltaTime += Time.deltaTime;
        if (deltaTime > TitleDuration)
        {
            deltaTime = 0;
            TitleDuration = 0;
            BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.DrawCard);
        }
    }
    public override void Exit()
    {
        gameView.AnimObj.SetActive(false);
    }
}

/// <summary>
/// 抽卡
/// </summary>
public class Battle_DrawCard : State
{
    GameView gameView = UIManager.instance.GetView("GameView") as GameView;
    int CardNum = 0;
    float deltaTime = 0;
    float DealCardDuration, ShuffleDuration;
    float rotationTime = 0;
    int cardRotaionNum = 0;
    bool hasCardRecycle, hasPlayer;
    int cardRecycleBeforeNum = 0;
    public Battle_DrawCard()
    {
        ID = BattleStateID.DrawCard;
    }
    public override void Enter()
    {
        gameView.AnimObj.SetActive(true);
        cardRecycleBeforeNum = 0;

        PlayerData nowPlayer = BattleManager.instance.GetCurrentPlayerData();
        switch (nowPlayer.playerType)
        {
            case PlayerType.OwnHuman:
                hasPlayer = true;
                #region 攻击栏卡牌装填
                //发牌动画放在CreateAtkBarCard中
                gameView.ATKBarCardList = new List<CurrentCardPoolModel>();
                if (gameView.UnusedCardList != null && gameView.UnusedCardList?.Count > 0)
                {
                    if (gameView.UnusedCardList.Count >= 5)
                    {
                        hasCardRecycle = false;
                        for (int i = 0; i < 5; i++)
                        {
                            gameView.CreateAtkBarCard(gameView.UnusedCardList[0]);
                            gameView.ATKBarCardList.Add(gameView.UnusedCardList[0]);
                            gameView.UnusedCardList.Remove(gameView.UnusedCardList[0]);

                        }
                    }
                    else
                    {
                        hasCardRecycle = true;
                        cardRecycleBeforeNum = gameView.UnusedCardList.Count;
                        for (int i = 0; i < cardRecycleBeforeNum; i++)
                        {
                            gameView.CreateAtkBarCard(gameView.UnusedCardList[0]);
                            gameView.ATKBarCardList.Add(gameView.UnusedCardList[0]);
                            gameView.UnusedCardList.Remove(gameView.UnusedCardList[0]);

                        }
                        //卡牌回收动画
                        gameView.UnusedCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUsedCardPoolsFileName).ListRandom();
                        for (int y = 0; y < 5 - cardRecycleBeforeNum; y++)
                        {
                            gameView.CreateAtkBarCard(gameView.UnusedCardList[0]);
                            gameView.ATKBarCardList.Add(gameView.UnusedCardList[0]);
                            gameView.UnusedCardList.Remove(gameView.UnusedCardList[0]);
                        }
                    }
                }
                else
                {
                    hasCardRecycle = true;
                    //洗牌动画
                    gameView.UnusedCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUsedCardPoolsFileName).ListRandom();
                    if (gameView.UnusedCardList.Count >= 5)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            gameView.CreateAtkBarCard(gameView.UnusedCardList[0]);
                            gameView.ATKBarCardList.Add(gameView.UnusedCardList[0]);
                            gameView.UnusedCardList.Remove(gameView.UnusedCardList[0]);

                        }
                    }
                }

                Common.SaveTxtFile(gameView.ATKBarCardList.ListToJson(), GlobalAttr.CurrentATKBarCardPoolsFileName);
                Common.SaveTxtFile(gameView.UnusedCardList.ListToJson(), GlobalAttr.CurrentUnUsedCardPoolsFileName);
                #endregion
                break;
            case PlayerType.NormalRobot:
            case PlayerType.AiRobot:
                hasPlayer = false;
                BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.Control);
                break;
            case PlayerType.OtherHuman:
                break;
        }
    }
    public override void Execute()
    {
        if (hasPlayer)
        {
            #region 装填攻击栏
            if (!hasCardRecycle)
            {
                deltaTime += Time.deltaTime;
                if (DealCardDuration == 0 && CardNum < gameView.ATKBarCardList.Count)
                {
                    DealCardDuration = AnimationManager.instance.DoAnimation("Anim_DealCard", null);
                }
                if (deltaTime > DealCardDuration && CardNum < gameView.ATKBarCardList.Count)
                {
                    //初次进入deltaTime=0.22
                    //Debug.Log("动画执行完成旋转卡牌");
                    rotationTime += Time.deltaTime;
                    gameView.CardRotating(gameView.obj_CardPools.transform.Find("imgCard_" + gameView.ATKBarCardList[CardNum].SingleID).gameObject, ref rotationTime, ref cardRotaionNum);
                    if (cardRotaionNum == 18)
                    {
                        gameView.txt_Left_Count.text = gameView.txt_Left_Count.text == "0" ? "0" : (Convert.ToInt32(gameView.txt_Left_Count.text) - 1).ToString();
                        CardNum++;
                        if (CardNum < gameView.ATKBarCardList.Count)
                        {
                            AnimationManager.instance.DoAnimation("Anim_DealCard", null);
                        }
                        cardRotaionNum = 0;
                        deltaTime = 0;
                        DealCardDuration = 0;
                    }
                }
            }
            else
            {
                deltaTime += Time.deltaTime;
                if (cardRecycleBeforeNum == 0 && ShuffleDuration == 0)
                {
                    if (deltaTime > DealCardDuration)
                    {
                        ShuffleDuration = AnimationManager.instance.DoAnimation("Anim_Shuffle", null);
                        deltaTime = 0;
                        gameView.txt_Right_Count.text = "0";
                    }
                }
                if (ShuffleDuration > 0 && deltaTime > ShuffleDuration)
                {
                    cardRecycleBeforeNum = -1;
                    ShuffleDuration = 0;
                    gameView.txt_Left_Count.text = gameView.UsedCardList.Count.ToString();
                    gameView.UsedCardList = new List<CurrentCardPoolModel>();
                    Common.SaveTxtFile(gameView.UsedCardList.ListToJson(), GlobalAttr.CurrentUsedCardPoolsFileName);
                }
                if (ShuffleDuration == 0 && DealCardDuration == 0 && CardNum < gameView.ATKBarCardList.Count)
                {
                    DealCardDuration = AnimationManager.instance.DoAnimation("Anim_DealCard", null);
                }
                if (ShuffleDuration == 0 && deltaTime > DealCardDuration && CardNum < gameView.ATKBarCardList.Count)
                {
                    //初次进入deltaTime=0.22
                    //Debug.Log("动画执行完成旋转卡牌");
                    rotationTime += Time.deltaTime;
                    gameView.CardRotating(gameView.obj_CardPools.transform.Find("imgCard_" + gameView.ATKBarCardList[CardNum].SingleID).gameObject, ref rotationTime, ref cardRotaionNum);
                    if (cardRotaionNum == 18)
                    {
                        gameView.txt_Left_Count.text = gameView.txt_Left_Count.text == "0" ? "0" : (Convert.ToInt32(gameView.txt_Left_Count.text) - 1).ToString();
                        CardNum++;
                        if (CardNum < gameView.ATKBarCardList.Count)
                        {
                            AnimationManager.instance.DoAnimation("Anim_DealCard", null);
                        }
                        cardRotaionNum = 0;
                        deltaTime = 0;
                        DealCardDuration = 0;
                        cardRecycleBeforeNum--;
                    }
                }
            }
            if (CardNum == gameView.ATKBarCardList.Count)
            {
                CardNum = 0;
                BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.Control);
            }
            #endregion 
        }
    }
    public override void Exit()
    {
        gameView.AnimObj.SetActive(false);
    }
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
    public override void Enter()
    {
        PlayerData nowPlayer = BattleManager.instance.GetCurrentPlayerData();
        if (nowPlayer.playerType == PlayerType.AiRobot || nowPlayer.playerType == PlayerType.NormalRobot)
        {
            BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.EffectSettlement);
        }
    }
    public override void Execute()
    {
        //一直执行

    }
    public override void Exit()
    {
        //退出时执行
        //Debug.Log("退出出牌状态");
    }

}

/// <summary>
/// 结算/处理效果
/// </summary>
public class Battle_PlayEffect : State
{
    GameView gameView = UIManager.instance.GetView("GameView") as GameView;
    float deltaTime = 0;
    float AnimDuration = 0;
    public Battle_PlayEffect()
    {
        ID = BattleStateID.EffectSettlement;
    }
    public override void Enter()
    {
        string EffectOn = "";
        gameView.AnimObj.SetActive(true);
        bool hasUseCard = false;
        bool hasPlayer = false;
        PlayerData nowPlayer = BattleManager.instance.GetCurrentPlayerData();
        CurrentCardPoolModel model = null;
        switch (nowPlayer.playerType)
        {
            case PlayerType.OwnHuman:
                CardUseEffectManager.instance.CardUseEffect(ref hasUseCard, ref EffectOn);
                model = CardUseEffectManager.instance.CurrentCardModel;
                hasPlayer = true;
                break;
            case PlayerType.NormalRobot:
            case PlayerType.AiRobot:
                hasPlayer = false;
                if (gameView.AiATKCardList?.Count > 0)
                {
                    model = gameView.AiATKCardList[0];
                    AIManager.instance.AIDo(1, model, ref hasUseCard, ref EffectOn);
                }
                else
                {
                    BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnEnd);
                    return;
                }
                break;
            case PlayerType.OtherHuman:
                break;
        }

        #region 卡牌已被使用
        if (hasUseCard)
        {
            if (hasPlayer)
            {
                AnimationManager.instance.DoAnimation("Anim_RecycleCard", null);
            }
            //问题：卡牌使用前后动画怎么操作;连续攻击怎么操作;第一次使用动画生效，第二次使用动画未生效
            //执行动画
            switch (model.EffectType)
            {
                case 1:
                case 7:
                case 31:
                    AnimDuration = AnimationManager.instance.DoAnimation("Anim_ATK", new object[] { EffectOn });
                    AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { EffectOn });
                    break;
                case 2:

                    AnimDuration = AnimationManager.instance.DoAnimation("Anim_Armor", new object[] { EffectOn });
                    break;
                case 3:
                    if (model.Effect > 0)
                    {
                        AnimDuration = AnimationManager.instance.DoAnimation("Anim_HPRestore", new object[] { EffectOn });
                    }
                    else
                    {
                        AnimDuration = AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { EffectOn });
                    }
                    break;
                case 4:
                    if (model.Effect > 0)
                    {
                        AnimDuration = AnimationManager.instance.DoAnimation("Anim_EnergyRestore", new object[] { EffectOn });
                    }
                    break;
                default:
                    BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.Control);
                    break;
            }
            //动画执行完毕进行状态调整
        }
        #endregion
        else
        {
            BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.Control);
        }
    }
    public override void Execute()
    {
        if (AnimDuration > 0)
        {
            deltaTime += Time.deltaTime;
            if (deltaTime > AnimDuration)//动画执行完毕
            {
                deltaTime = 0;
                AnimDuration = 0;
                BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.Control);
            }
        }
    }
    public override void Exit()
    {
        gameView.AnimObj.SetActive(false);
    }
}

/// <summary>
/// 回合结束
/// </summary>
public class Battle_TurnEnd : State
{
    GameView gameView = UIManager.instance.GetView("GameView") as GameView;
    float deltaTime = 0;
    float AnimDuration = 0;
    bool RemoveOldCard = true;//清除老的手牌
    public Battle_TurnEnd()
    {
        ID = BattleStateID.TurnEnd;
    }
    public override void Enter()
    {
        gameView.AnimObj.SetActive(true);
        PlayerData nowPlayer = BattleManager.instance.GetCurrentPlayerData();
        switch (nowPlayer.playerType)
        {
            case PlayerType.OwnHuman:
                #region Player
                gameView.ATKBarCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentATKBarCardPoolsFileName);
                gameView.UsedCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUsedCardPoolsFileName) ?? new List<CurrentCardPoolModel>();
                gameView.UnusedCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUnUsedCardPoolsFileName);
                #region PlayerBUFF变化
                if (BattleManager.instance.OwnPlayerData[0].buffList != null)
                {
                    //所有BUFF-1
                    foreach (var item in BattleManager.instance.OwnPlayerData[0].buffList)
                    {
                        item.Num -= 1;
                    }
                    //如果BUFF愤怒或虚弱。相对于的数值进行保存
                    #region 数值保存
                    if (BattleManager.instance.OwnPlayerData[0].buffList.Exists(a => a.Num > 0 && a.EffectType == 8) ||
                            BattleManager.instance.OwnPlayerData[0].buffList.Exists(a => a.Num > 0 && a.EffectType == 11))
                    {
                        BattleManager.instance.OwnPlayerData[0].handCardList?.ForEach(item =>
                        {
                            var atkTemp = gameView.ATKBarCardList.Find(a => a.SingleID == item.SingleID);
                            if (atkTemp != null)
                            {
                                gameView.ATKBarCardList.Remove(atkTemp);
                                gameView.ATKBarCardList.Add(item);
                            }
                            var useTemp = gameView.UsedCardList.Find(a => a.SingleID == item.SingleID);
                            if (useTemp != null)
                            {
                                gameView.UsedCardList.Remove(useTemp);
                                gameView.UsedCardList.Add(item);
                            }
                            var unUseTemp = gameView.UnusedCardList.Find(a => a.SingleID == item.SingleID);
                            if (unUseTemp != null)
                            {
                                gameView.UnusedCardList.Remove(unUseTemp);
                                gameView.UnusedCardList.Add(item);
                            }
                        });
                    }
                    #endregion
                    gameView.BUFFUIChange(BattleManager.instance.OwnPlayerData[0].buffList, ref BattleManager.instance.OwnPlayerData[0].handCardList, ref CardUseEffectManager.instance.CurrentCardModel);

                }
                #endregion
                #endregion
                break;
            case PlayerType.NormalRobot:
            case PlayerType.AiRobot:

                #region AI BUFF次数-1
                if (BattleManager.instance.EnemyPlayerData[0].buffList != null)
                {
                    //所有BUFF-1
                    foreach (var item in BattleManager.instance.EnemyPlayerData[0].buffList)
                    {
                        item.Num -= 1;
                    }
                    gameView.BUFFUIChange(BattleManager.instance.EnemyPlayerData[0].buffList, ref BattleManager.instance.EnemyPlayerData[0].handCardList, ref CardUseEffectManager.instance.CurrentCardModel, 1);
                }
                #endregion

                #region AiAtkBar
                gameView.AiATKCardList = new List<CurrentCardPoolModel>();
                gameView.AiCardList.ListRandom();
                for (int i = 0; i < gameView.AIAtkNum; i++)
                {
                    gameView.AiATKCardList.Add(gameView.AiCardList[i]);
                }
                Common.SaveTxtFile(gameView.AiATKCardList.ListToJson(), GlobalAttr.CurrentAIATKCardPoolsFileName);
                gameView.AIATKCardPoolsBind();
                #endregion

                //Player攻击
                BattleManager.instance.controlIndex = 0;
                //装填攻击栏/洗牌
                BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                break;
            case PlayerType.OtherHuman:
                break;
        }

    }
    public override void Execute()
    {
        #region 手牌放入已使用的已使用的类中
        //手牌放入已使用的已使用的类中
        if (RemoveOldCard)
        {
            deltaTime += Time.deltaTime;
            if (deltaTime > AnimDuration)
            {
                AnimDuration = 0;
                deltaTime = 0;
            }
            if (gameView.ATKBarCardList?.Count > 0)
            {
                if (AnimDuration == 0)
                {
                    int index = gameView.ATKBarCardList.Count - 1;
                    gameView.UsedCardList.Add(gameView.ATKBarCardList[index]);
                    GameObject obj = gameView.obj_CardPools.transform.Find("imgCard_" + gameView.ATKBarCardList[index].SingleID).gameObject;
                    gameView.DeleteGameObj(obj);
                    gameView.ATKBarCardList.Remove(gameView.ATKBarCardList[index]);
                    AnimDuration = AnimationManager.instance.DoAnimation("Anim_RecycleCard", null);
                    Common.SaveTxtFile(gameView.UsedCardList.ListToJson(), GlobalAttr.CurrentUsedCardPoolsFileName);
                    gameView.txt_Right_Count.text = gameView.UsedCardList.Count.ToString();
                }
            }
            else
            {
                if (AnimDuration == 0)
                {
                    //AI攻击
                    BattleManager.instance.controlIndex = 1;
                    //装填攻击栏/洗牌
                    BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnStart);
                }
            }
        }
        #endregion
    }
    public override void Exit()
    {
        gameView.AnimObj.SetActive(false);
    }
}

/// <summary>
/// 游戏结束
/// </summary>
public class Battle_GameEnd : State
{
    GameView gameView = UIManager.instance.GetView("GameView") as GameView;
    float deltaTime = 0;
    float AnimDuration = 0;
    public Battle_GameEnd()
    {
        ID = BattleStateID.GameEnd;
    }
    public override void Enter()
    {
        gameView.AnimObj.SetActive(true);
        if (gameView.hasPlayerOrAIDie == "0")//AI死亡
        {
            UIManager.instance.OpenView("AiDieView");
            UIManager.instance.CloseView("GameView");
        }
        else
        {
            AnimDuration = AnimationManager.instance.DoAnimation("Anim_PlayerDie", null);
        }
    }
    public override void Execute()
    {
        if (AnimDuration != 0)
        {
            deltaTime += Time.deltaTime;
            if (deltaTime > AnimDuration)
            {
                deltaTime = 0;
                AnimDuration = 0;
                UIManager.instance.OpenView("PlayerDieView");
                UIManager.instance.CloseView("GameView");
            }
        }
    }
    public override void Exit()
    {
        gameView.AnimObj.SetActive(false);
    }
}
