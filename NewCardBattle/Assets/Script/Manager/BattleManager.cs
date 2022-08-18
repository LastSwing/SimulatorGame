using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
        BattleStateMachine.AddState(new Battle_BeforeCardUse());
        BattleStateMachine.AddState(new Battle_PlayEffect());
        BattleStateMachine.AddState(new Battle_AfterCardUse());
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
        var ownRole = BattleManager.instance.OwnPlayerData[0];
        var AiRole = BattleManager.instance.EnemyPlayerData[0];
        List<BUFFEffect> buffResult = null;
        List<BUFFEffect> bUFFEffects = null;
        switch (nowPlayer.playerType)
        {
            case PlayerType.OwnHuman:
                if (BattleManager.instance.BattleStateMachine.PreviousState.ID != BattleStateID.Ready)
                {
                    TitleDuration = AnimationManager.instance.DoAnimation("Anim_ShowTitle", new object[] { "你的回合" });
                }
                #region 回合开始BUFF作用
                buffResult = BUFFManager.instance.BUFFApply(ref ownRole.buffList, AiRole.buffList, ref CardUseEffectManager.instance.CurrentCardModel, ref ownRole.handCardList);
                bUFFEffects = buffResult?.FindAll(a => a.BUFFType == 2);
                if (bUFFEffects?.Count > 0)
                {
                    foreach (var item in bUFFEffects)
                    {
                        CardUseEffectManager.instance.HPChange(item.HPChange, 0);
                        AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { "1" });
                    }
                }
                #endregion
                #region PlayerBUFF变化
                if (BattleManager.instance.OwnPlayerData[0].buffList != null)
                {
                    foreach (var item in BattleManager.instance.OwnPlayerData[0].buffList)
                    {
                        //除了暴击、吸血BUFF都-1
                        if (item.EffectType != 10 && item.EffectType != 33)
                        {
                            item.Num -= 1;
                        }
                    }
                    gameView.BUFFUIChange(BattleManager.instance.OwnPlayerData[0].buffList, ref BattleManager.instance.OwnPlayerData[0].handCardList, ref CardUseEffectManager.instance.CurrentCardModel);
                    //存储上一回合数据
                    Common.SaveTxtFile(gameView.ATKBarCardList.ListToJson(), GlobalAttr.CurrentATKBarCardPoolsFileName);
                    Common.SaveTxtFile(gameView.UnusedCardList.ListToJson(), GlobalAttr.CurrentUnUsedCardPoolsFileName);
                    Common.SaveTxtFile(gameView.UsedCardList.ListToJson(), GlobalAttr.CurrentUsedCardPoolsFileName);

                }
                #endregion
                break;
            case PlayerType.NormalRobot:
            case PlayerType.AiRobot:
                TitleDuration = AnimationManager.instance.DoAnimation("Anim_ShowTitle", new object[] { "AI回合" });
                #region 回合开始BUFF作用
                buffResult = BUFFManager.instance.BUFFApply(ref AiRole.buffList, ownRole.buffList, ref gameView.CrtAIATKModel, ref AiRole.handCardList, false, 1);
                bUFFEffects = buffResult?.FindAll(a => a.BUFFType == 2);
                if (bUFFEffects?.Count > 0)
                {
                    foreach (var item in bUFFEffects)
                    {
                        CardUseEffectManager.instance.HPChange(item.HPChange, 1);
                        AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { "0" });
                    }
                }
                #endregion
                #region AI BUFF次数-1
                if (BattleManager.instance.EnemyPlayerData[0].buffList != null)
                {
                    foreach (var item in BattleManager.instance.EnemyPlayerData[0].buffList)
                    {
                        //除了暴击、吸血BUFF都-1
                        if (item.EffectType != 10 || item.EffectType != 33)
                        {
                            item.Num -= 1;
                        }
                    }
                    gameView.BUFFUIChange(BattleManager.instance.EnemyPlayerData[0].buffList, ref BattleManager.instance.EnemyPlayerData[0].handCardList, ref CardUseEffectManager.instance.CurrentCardModel, 1);
                    //存储上一回合数据
                    Common.SaveTxtFile(gameView.AiATKCardList.ListToJson(), GlobalAttr.CurrentAIATKCardPoolsFileName);
                }
                #endregion

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
    bool BlackCardEffectExecute = false;
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
                    var crtModel = gameView.ATKBarCardList[CardNum];
                    gameView.CardRotating(gameView.obj_CardPools.transform.Find("imgCard_" + crtModel.SingleID).gameObject, ref rotationTime, ref cardRotaionNum);
                    #region 抽到了有效果的黑卡
                    if (crtModel.CardType == 2 && !BlackCardEffectExecute)//
                    {
                        var ownRole = BattleManager.instance.OwnPlayerData[0];
                        switch (crtModel.EffectType)//血量变化
                        {
                            case 3:
                                if (crtModel.Effect < 0)
                                {
                                    var effect = ownRole.bloodNow + Convert.ToInt32(crtModel.Effect);
                                    if (effect < 0)
                                    {
                                        ownRole.bloodNow = 0;
                                        //玩家死亡
                                        gameView.PlayerDie();
                                    }
                                    else
                                    {
                                        Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, crtModel.Effect, 0);
                                        ownRole.bloodNow += Convert.ToInt32(crtModel.Effect);
                                    }
                                }
                                else
                                {
                                    Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, crtModel.Effect, 1);
                                    ownRole.bloodNow += Convert.ToInt32(crtModel.Effect);
                                    if (ownRole.bloodNow > ownRole.bloodMax)
                                    {
                                        ownRole.bloodNow = ownRole.bloodMax;
                                    }
                                }
                                gameView.txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
                                AnimationManager.instance.DoAnimation("Anim_HPDeduction", null);
                                break;
                            case 4:
                                if (crtModel.Effect < 0)//扣能量
                                {
                                    var effect = ownRole.Energy + Convert.ToInt32(crtModel.Effect);
                                    if (effect < 0)
                                    {
                                        Common.EnergyImgChange(ownRole.Energy, ownRole.Energy, 0, ownRole.EnergyMax);
                                        ownRole.Energy = 0;
                                    }
                                    else
                                    {
                                        Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(crtModel.Effect * -1), 0, ownRole.EnergyMax);
                                        ownRole.Energy += Convert.ToInt32(crtModel.Effect);
                                    }
                                }
                                else
                                {
                                    Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(crtModel.Effect), 1, ownRole.EnergyMax);
                                    ownRole.Energy += Convert.ToInt32(crtModel.Effect);
                                    if (ownRole.Energy > ownRole.EnergyMax)
                                    {
                                        ownRole.Energy = ownRole.EnergyMax;
                                    }
                                }
                                AnimationManager.instance.DoAnimation("Anim_EnergyRestore", null);
                                break;
                        }
                        BlackCardEffectExecute = true;
                    }
                    #endregion
                    if (cardRotaionNum == 18)
                    {
                        gameView.txt_Left_Count.text = gameView.txt_Left_Count.text == "0" ? "0" : (Convert.ToInt32(gameView.txt_Left_Count.text) - 1).ToString();
                        CardNum++;
                        if (CardNum < gameView.ATKBarCardList.Count)
                        {
                            AnimationManager.instance.DoAnimation("Anim_DealCard", null);
                        }
                        BlackCardEffectExecute = false;
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
                    var crtModel = gameView.ATKBarCardList[CardNum];
                    gameView.CardRotating(gameView.obj_CardPools.transform.Find("imgCard_" + crtModel.SingleID).gameObject, ref rotationTime, ref cardRotaionNum);
                    #region 抽到了有效果的黑卡
                    if (crtModel.CardType == 2 && !BlackCardEffectExecute)//
                    {
                        var ownRole = BattleManager.instance.OwnPlayerData[0];
                        switch (crtModel.EffectType)//血量变化
                        {
                            case 3:
                                if (crtModel.Effect < 0)
                                {
                                    var effect = ownRole.bloodNow + Convert.ToInt32(crtModel.Effect);
                                    if (effect < 0)
                                    {
                                        ownRole.bloodNow = 0;
                                        //玩家死亡
                                        gameView.PlayerDie();
                                    }
                                    else
                                    {
                                        Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, crtModel.Effect, 0);
                                        ownRole.bloodNow += Convert.ToInt32(crtModel.Effect);
                                    }
                                }
                                else
                                {
                                    Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, crtModel.Effect, 1);
                                    ownRole.bloodNow += Convert.ToInt32(crtModel.Effect);
                                    if (ownRole.bloodNow > ownRole.bloodMax)
                                    {
                                        ownRole.bloodNow = ownRole.bloodMax;
                                    }
                                }
                                gameView.txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
                                AnimationManager.instance.DoAnimation("Anim_HPDeduction", null);
                                break;
                            case 4:
                                if (crtModel.Effect < 0)//扣能量
                                {
                                    var effect = ownRole.Energy + Convert.ToInt32(crtModel.Effect);
                                    if (effect < 0)
                                    {
                                        Common.EnergyImgChange(ownRole.Energy, ownRole.Energy, 0, ownRole.EnergyMax);
                                        ownRole.Energy = 0;
                                    }
                                    else
                                    {
                                        Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(crtModel.Effect * -1), 0, ownRole.EnergyMax);
                                        ownRole.Energy += Convert.ToInt32(crtModel.Effect);
                                    }
                                }
                                else
                                {
                                    Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(crtModel.Effect), 1, ownRole.EnergyMax);
                                    ownRole.Energy += Convert.ToInt32(crtModel.Effect);
                                    if (ownRole.Energy > ownRole.EnergyMax)
                                    {
                                        ownRole.Energy = ownRole.EnergyMax;
                                    }
                                }
                                AnimationManager.instance.DoAnimation("Anim_EnergyRestore", null);
                                break;
                        }
                        BlackCardEffectExecute = true;
                    }
                    #endregion
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
                        BlackCardEffectExecute = false;
                        cardRecycleBeforeNum--;
                    }
                }
            }
            if (CardNum == gameView.ATKBarCardList.Count)
            {
                if (DealCardDuration == 0)
                {
                    CardNum = 0;
                    BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.Control);
                }
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
            BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.BeforeCardUse);
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
/// 卡牌使用前状态
/// </summary>
public class Battle_BeforeCardUse : State
{
    float deltaTime = 0;
    float AnimDuration = 0;
    GameView gameView = UIManager.instance.GetView("GameView") as GameView;
    int result;
    bool RemoveOldCard = false;//清除手牌
    public Battle_BeforeCardUse()
    {
        ID = BattleStateID.BeforeCardUse;
    }
    public override void Enter()
    {
        gameView.AnimObj.SetActive(true);
        int PlayerOrAI = 0;
        PlayerData nowPlayer = BattleManager.instance.GetCurrentPlayerData();
        CurrentCardPoolModel model = null;
        switch (nowPlayer.playerType)
        {
            case PlayerType.OwnHuman:
                model = CardUseEffectManager.instance.CurrentCardModel;
                break;
            case PlayerType.NormalRobot:
            case PlayerType.AiRobot:
                if (gameView.AiATKCardList?.Count > 0)
                {
                    model = gameView.AiATKCardList[0];
                }
                else
                {
                    BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnEnd);
                    return;
                }
                PlayerOrAI = 1;
                break;
            case PlayerType.OtherHuman:
                break;
        }
        string EffectOn = "";
        result = CardUseEffectManager.instance.CardUseBeforeTriggerEvent(model, PlayerOrAI, ref EffectOn);
        switch (result)
        {
            case 1:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_RecycleCard", null);
                break;
            case 2:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { EffectOn });
                AnimationManager.instance.DoAnimation("Anim_ATK", null);
                break;
            case 3:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_Armor", new object[] { EffectOn });
                break;
            case 4:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_HPRestore", new object[] { EffectOn });
                break;
            case 5:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_EnergyRestore", new object[] { EffectOn });
                break;
            case 6:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_RemoveCard", null);
                break;
            case 7:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_Shuffle", null);
                break;
            case 8:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { EffectOn });
                break;
            case 9:
                //摧毁防御动画
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_ArmorMelting", new object[] { EffectOn });
                break;
            case 24://移除所有手牌
                RemoveOldCard = true;
                break;
            case 17://移除一张手牌
                gameView.obj_RemoveCard.SetActive(true);
                CardUseEffectManager.instance.UseCopyCard = true;
                gameView.btn_CopyCard.onClick.AddListener(delegate { gameView.RemoveCard(CardUseEffectManager.instance.CurrentCard, ref AnimDuration); });
                break;
        }
    }
    public override void Execute()
    {
        if (!CardUseEffectManager.instance.UseCopyCard)
        {
            deltaTime += Time.deltaTime;
            if (RemoveOldCard)
            {
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
                        RemoveOldCard = false;
                        BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.EffectSettlement);
                    }
                }
            }
            else
            {
                if (deltaTime > AnimDuration)
                {
                    deltaTime = 0;
                    AnimDuration = 0;
                    BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.EffectSettlement);
                }
            }
        }
    }
    public override void Exit()
    {
        gameView.AnimObj.SetActive(false);
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
    int comboATKNum = 1;
    public Battle_PlayEffect()
    {
        ID = BattleStateID.EffectSettlement;
    }
    public override void Enter()
    {
        string EffectOn = "";
        gameView.AnimObj.SetActive(true);
        bool hasUseCard = false;
        bool hasEffect = true;
        bool playerUseCard = false;
        int playAnim = 0;
        PlayerData nowPlayer = BattleManager.instance.GetCurrentPlayerData();
        CurrentCardPoolModel model = null;
        switch (nowPlayer.playerType)
        {
            case PlayerType.OwnHuman:
                if (CardUseEffectManager.instance.CrtCardChange)
                {
                    model = CardUseEffectManager.instance.PrevoiousCardModel;
                }
                else
                {
                    model = CardUseEffectManager.instance.CurrentCardModel;
                }
                CardUseEffectManager.instance.CardUseEffect(model, ref hasUseCard, ref EffectOn, ref hasEffect, ref playAnim);
                playerUseCard = true;
                break;
            case PlayerType.NormalRobot:
            case PlayerType.AiRobot:
                if (gameView.AiATKCardList?.Count > 0)
                {
                    model = gameView.AiATKCardList[0];
                    AIManager.instance.AIDo(1, model, ref hasUseCard, ref EffectOn, ref hasEffect, ref playAnim);
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
            #region 激活攻击力叠加
            if (CardUseEffectManager.instance.ActivateSuperPos)
            {
                if (playerUseCard)
                {
                    CardUseEffectManager.instance.SuperPositionEffect++;
                }
            }
            #endregion
            if (hasEffect)
            {
                #region 执行动画
                //执行动画
                switch (model.EffectType)
                {
                    #region 攻击
                    case 1:
                    case 7:
                    case 14:
                    case 31:
                    case 32:
                        AnimDuration = AnimationManager.instance.DoAnimation("Anim_ATK", new object[] { EffectOn });
                        AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { EffectOn });
                        break;
                    #endregion
                    #region 连续攻击
                    case 5://连续攻击
                        comboATKNum = model.AtkNumber;
                        if (nowPlayer.playerType == PlayerType.OwnHuman)
                        {
                            CardUseEffectManager.instance.ComboATK(model, BattleManager.instance.OwnPlayerData[0], BattleManager.instance.EnemyPlayerData[0]);
                        }
                        else
                        {
                            CardUseEffectManager.instance.ComboATK(model, BattleManager.instance.OwnPlayerData[0], BattleManager.instance.EnemyPlayerData[0], 1);
                        }
                        AnimDuration = AnimationManager.instance.DoAnimation("Anim_ATK", new object[] { EffectOn });
                        AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { EffectOn });
                        break;
                    #endregion
                    #region 防御
                    case 2:
                    case 27:
                        AnimDuration = AnimationManager.instance.DoAnimation("Anim_Armor", new object[] { EffectOn });
                        break;
                    #endregion
                    #region 血量变化
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
                    #endregion
                    #region 能量恢复
                    case 4:
                        if (model.Effect > 0)
                        {
                            AnimDuration = AnimationManager.instance.DoAnimation("Anim_EnergyRestore", new object[] { EffectOn });
                        }
                        break;
                    #endregion
                    #region 复制卡
                    case 12:
                        CardUseEffectManager.instance.UseCopyCard = true;
                        gameView.btn_CopyCard.onClick.AddListener(delegate { gameView.CopyCard(CardUseEffectManager.instance.CurrentCard); });
                        break;
                    #endregion
                    #region 销毁防御
                    case 6:
                        AnimDuration = AnimationManager.instance.DoAnimation("Anim_ArmorMelting", new object[] { EffectOn });
                        break;
                    #endregion
                    #region 概率防御
                    case 28:
                        if (playAnim == 1)
                        {
                            AnimDuration = AnimationManager.instance.DoAnimation("Anim_Armor", new object[] { EffectOn });
                        }
                        else
                        {
                            AnimDuration = AnimationManager.instance.DoAnimation("Anim_ArmorMelting", new object[] { EffectOn });
                        }
                        break;
                    #endregion
                    default:
                        BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.AfterCardUse);
                        break;
                }
                #endregion 
            }
            else
            {
                if (playAnim == 4)//扣血
                {
                    AnimDuration = AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { EffectOn });
                }
                else if (playAnim == 5)//回血
                {
                    AnimDuration = AnimationManager.instance.DoAnimation("Anim_HPRestore", new object[] { EffectOn });
                }
                else if (playAnim == 3)//闪避动画
                {
                    AnimDuration = AnimationManager.instance.DoAnimation("Anim_Elude", new object[] { EffectOn });
                }
                else
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
                    BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.Control);
                }
            }
        }
        #endregion
        else
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
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
                PlayerData nowPlayer = BattleManager.instance.GetCurrentPlayerData();
                if (comboATKNum == 1)
                {
                    BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.AfterCardUse);
                }
                else
                {
                    string EffectOn = "";
                    if (nowPlayer.playerType == PlayerType.OwnHuman)
                    {
                        var model = CardUseEffectManager.instance.CurrentCardModel;
                        CardUseEffectManager.instance.ComboATK(model, BattleManager.instance.OwnPlayerData[0], BattleManager.instance.EnemyPlayerData[0]);
                        EffectOn = "0";
                    }
                    else
                    {
                        var model = gameView.CrtAIATKModel;
                        CardUseEffectManager.instance.ComboATK(model, BattleManager.instance.OwnPlayerData[0], BattleManager.instance.EnemyPlayerData[0], 1);
                        EffectOn = "1";
                    }
                    AnimDuration = AnimationManager.instance.DoAnimation("Anim_ATK", new object[] { EffectOn });
                    AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { EffectOn });
                    comboATKNum--;
                }
            }
        }
    }
    public override void Exit()
    {
        gameView.AnimObj.SetActive(false);
    }
}

/// <summary>
/// 卡牌使用后状态
/// </summary>
public class Battle_AfterCardUse : State
{
    GameView gameView = UIManager.instance.GetView("GameView") as GameView;
    float deltaTime = 0;
    float AnimDuration = 0;
    float DealCardDuration = 0;
    float rotationTime = 0;
    float ShuffleDuration = 0;
    int cardRotaionNum = 0;
    int result;
    bool hasCardRecycle = false;
    bool AgainDealCard = false;//抽卡
    int AnimNum;
    int CardNum = 0;
    bool BlackCardEffectExecute = false;
    List<CurrentCardPoolModel> DrawACard;
    public Battle_AfterCardUse()
    {
        ID = BattleStateID.AfterCardUse;
    }
    public override void Enter()
    {
        gameView.AnimObj.SetActive(true);
        AnimNum = 0;
        DrawACard = new List<CurrentCardPoolModel>();
        PlayerData nowPlayer = BattleManager.instance.GetCurrentPlayerData();
        CurrentCardPoolModel model = null;
        switch (nowPlayer.playerType)
        {
            case PlayerType.OwnHuman:
                if (CardUseEffectManager.instance.CrtCardChange)
                {
                    model = CardUseEffectManager.instance.PrevoiousCardModel;
                }
                else
                {
                    model = CardUseEffectManager.instance.CurrentCardModel;
                }
                break;
            case PlayerType.NormalRobot:
            case PlayerType.AiRobot:
                model = gameView.CrtAIATKModel;
                break;
            case PlayerType.OtherHuman:
                break;
        }
        string EffectOn = "";
        result = CardUseEffectManager.instance.CardUseAfterTriggerEvent(model, ref EffectOn, ref DrawACard);
        switch (result)
        {
            #region 卡牌回收
            case 1:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_RecycleCard", null);
                break;
            #endregion
            #region 攻击和卡牌回收
            case 2:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { EffectOn });
                AnimationManager.instance.DoAnimation("Anim_ATK", new object[] { EffectOn });
                AnimationManager.instance.DoAnimation("Anim_RecycleCard", null);
                break;
            #endregion
            #region 防御和卡牌回收
            case 3:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_Armor", new object[] { EffectOn });
                AnimationManager.instance.DoAnimation("Anim_RecycleCard", null);
                break;
            #endregion
            #region HP恢复和卡牌回收
            case 4:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_HPRestore", new object[] { EffectOn });
                AnimationManager.instance.DoAnimation("Anim_RecycleCard", null);
                break;
            #endregion
            #region 能量恢复和卡牌回收
            case 5:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_EnergyRestore", new object[] { EffectOn });
                AnimationManager.instance.DoAnimation("Anim_RecycleCard", null);
                break;
            #endregion
            #region 移除卡牌
            case 6:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_RemoveCard", null);
                break;
            #endregion
            #region 抽卡
            case 7:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_Shuffle", null);
                break;
            #endregion
            #region 攻击
            case 8:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { EffectOn });
                AnimationManager.instance.DoAnimation("Anim_ATK", new object[] { EffectOn });
                break;
            #endregion
            #region 防御
            case 9:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_Armor", new object[] { EffectOn });
                break;
            #endregion
            #region 扣血和卡牌回收
            case 10:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { EffectOn });
                AnimationManager.instance.DoAnimation("Anim_RecycleCard", null);
                break;
            #endregion
            #region 血量恢复
            case 11:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_HPRestore", new object[] { EffectOn });
                break;
            #endregion
            #region 血量扣减
            case 12:
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_HPDeduction", new object[] { EffectOn });
                break;
            #endregion
            #region 销毁防御
            case 13:
                //销毁防御
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_ArmorMelting", new object[] { EffectOn });
                break;
            #endregion
            #region 销毁防御和卡牌回收
            case 14:
                //销毁防御和卡牌回收
                AnimDuration = AnimationManager.instance.DoAnimation("Anim_ArmorMelting", new object[] { EffectOn });
                AnimationManager.instance.DoAnimation("Anim_RecycleCard", null);
                break;
            #endregion
            #region 抽同等手牌数量
            case 25://抽同等手牌数量
                #region 攻击栏卡牌装填
                hasCardRecycle = true;
                if (CardUseEffectManager.instance.BeRemoveCardNum > 0)
                {
                    AgainDealCard = true;
                    //重新洗牌
                    if (gameView.UnusedCardList?.Count > 0)
                    {
                        var newUseCard = gameView.UsedCardList.FindAll(a => a.SingleID != CardUseEffectManager.instance.CurrentCardModel.SingleID);
                        foreach (var item in newUseCard)
                        {
                            gameView.UnusedCardList.Add(item);
                        }
                        gameView.UsedCardList = new List<CurrentCardPoolModel>()
                    {
                        CardUseEffectManager.instance.CurrentCardModel
                    };
                        gameView.UnusedCardList.ListRandom();
                    }
                    else
                    {
                        gameView.UnusedCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUsedCardPoolsFileName).ListRandom();
                        gameView.UsedCardList = new List<CurrentCardPoolModel>();
                        gameView.UsedCardList.Add(CardUseEffectManager.instance.CurrentCardModel);
                        gameView.UnusedCardList.Remove(gameView.UnusedCardList.Find(a => a.SingleID == CardUseEffectManager.instance.CurrentCardModel.SingleID));
                    }
                    if (gameView.UnusedCardList.Count >= CardUseEffectManager.instance.BeRemoveCardNum)
                    {
                        for (int i = 0; i < CardUseEffectManager.instance.BeRemoveCardNum; i++)
                        {
                            gameView.CreateAtkBarCard(gameView.UnusedCardList[0]);
                            gameView.ATKBarCardList.Add(gameView.UnusedCardList[0]);
                            gameView.UnusedCardList.Remove(gameView.UnusedCardList[0]);
                        }
                    }
                    Common.SaveTxtFile(gameView.ATKBarCardList.ListToJson(), GlobalAttr.CurrentATKBarCardPoolsFileName);
                    Common.SaveTxtFile(gameView.UnusedCardList.ListToJson(), GlobalAttr.CurrentUnUsedCardPoolsFileName);
                    Common.SaveTxtFile(gameView.UsedCardList.ListToJson(), GlobalAttr.CurrentUsedCardPoolsFileName);
                }
                else
                {
                    gameView.DeleteGameObj(CardUseEffectManager.instance.CurrentCard);
                    //放入已使用牌堆
                    var useCards = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUsedCardPoolsFileName) ?? new List<CurrentCardPoolModel>();
                    useCards.Add(CardUseEffectManager.instance.CurrentCardModel);
                    Common.SaveTxtFile(useCards.ListToJson(), GlobalAttr.CurrentUsedCardPoolsFileName);
                    gameView.txt_Right_Count.text = useCards == null ? "0" : useCards.Count.ToString();
                    //移除当前手牌
                    var removeModel = gameView.ATKBarCardList.Find(a => a.SingleID == CardUseEffectManager.instance.CurrentCardModel.SingleID);
                    gameView.ATKBarCardList.Remove(removeModel);
                    Common.SaveTxtFile(gameView.ATKBarCardList.ListToJson(), GlobalAttr.CurrentATKBarCardPoolsFileName);
                    AnimDuration = AnimationManager.instance.DoAnimation("Anim_RecycleCard", null);
                }
                #endregion
                break;
                #endregion
        }

    }
    public override void Execute()
    {
        deltaTime += Time.deltaTime;
        if (AgainDealCard)
        {
            #region 装填攻击栏
            if (ShuffleDuration == 0 && hasCardRecycle)
            {
                ShuffleDuration = AnimationManager.instance.DoAnimation("Anim_Shuffle", null);
                deltaTime = 0;
                gameView.txt_Right_Count.text = gameView.UsedCardList.Count.ToString();
            }
            if (ShuffleDuration > 0 && deltaTime > ShuffleDuration)
            {
                hasCardRecycle = false;
                ShuffleDuration = 0;
                gameView.txt_Left_Count.text = (gameView.UnusedCardList.Count + gameView.ATKBarCardList.Count).ToString();
            }
            if (ShuffleDuration == 0 && DealCardDuration == 0 && CardNum < gameView.ATKBarCardList.Count)
            {
                DealCardDuration = AnimationManager.instance.DoAnimation("Anim_DealCard", null);
            }
            if (ShuffleDuration == 0 && deltaTime > DealCardDuration && CardNum < gameView.ATKBarCardList.Count)
            {
                rotationTime += Time.deltaTime;
                var crtModel = gameView.ATKBarCardList[CardNum];
                gameView.CardRotating(gameView.obj_CardPools.transform.Find("imgCard_" + crtModel.SingleID).gameObject, ref rotationTime, ref cardRotaionNum);
                #region 抽到了有效果的黑卡
                if (crtModel.CardType == 2 && !BlackCardEffectExecute)//
                {
                    var ownRole = BattleManager.instance.OwnPlayerData[0];
                    switch (crtModel.EffectType)//血量变化
                    {
                        case 3:
                            if (crtModel.Effect < 0)
                            {
                                var effect = ownRole.bloodNow + Convert.ToInt32(crtModel.Effect);
                                if (effect < 0)
                                {
                                    ownRole.bloodNow = 0;
                                    //玩家死亡
                                    gameView.PlayerDie();
                                }
                                else
                                {
                                    Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, crtModel.Effect, 0);
                                    ownRole.bloodNow += Convert.ToInt32(crtModel.Effect);
                                }
                            }
                            else
                            {
                                Common.HPImageChange(gameView.Pimg_HP, ownRole.bloodMax, crtModel.Effect, 1);
                                ownRole.bloodNow += Convert.ToInt32(crtModel.Effect);
                                if (ownRole.bloodNow > ownRole.bloodMax)
                                {
                                    ownRole.bloodNow = ownRole.bloodMax;
                                }
                            }
                            gameView.txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
                            AnimationManager.instance.DoAnimation("Anim_HPDeduction", null);
                            break;
                        case 4:
                            if (crtModel.Effect < 0)//扣能量
                            {
                                var effect = ownRole.Energy + Convert.ToInt32(crtModel.Effect);
                                if (effect < 0)
                                {
                                    Common.EnergyImgChange(ownRole.Energy, ownRole.Energy, 0, ownRole.EnergyMax);
                                    ownRole.Energy = 0;
                                }
                                else
                                {
                                    Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(crtModel.Effect * -1), 0, ownRole.EnergyMax);
                                    ownRole.Energy += Convert.ToInt32(crtModel.Effect);
                                }
                            }
                            else
                            {
                                Common.EnergyImgChange(ownRole.Energy, Convert.ToInt32(crtModel.Effect), 1, ownRole.EnergyMax);
                                ownRole.Energy += Convert.ToInt32(crtModel.Effect);
                                if (ownRole.Energy > ownRole.EnergyMax)
                                {
                                    ownRole.Energy = ownRole.EnergyMax;
                                }
                            }
                            AnimationManager.instance.DoAnimation("Anim_EnergyRestore", null);
                            break;
                    }
                    BlackCardEffectExecute = true;
                }
                #endregion
                if (cardRotaionNum == 18)
                {
                    gameView.txt_Left_Count.text = gameView.txt_Left_Count.text == "0" ? "0" : (Convert.ToInt32(gameView.txt_Left_Count.text) - 1).ToString();
                    CardNum++;
                    if (CardNum < gameView.ATKBarCardList.Count)
                    {
                        AnimationManager.instance.DoAnimation("Anim_DealCard", null);
                    }
                    BlackCardEffectExecute = false;
                    cardRotaionNum = 0;
                    deltaTime = 0;
                    DealCardDuration = 0;
                }
            }
            if (CardNum == gameView.ATKBarCardList.Count)
            {
                if (DealCardDuration == 0)
                {
                    CardNum = 0;
                    AgainDealCard = false;
                    BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.Control);
                }
            }
            #endregion 
        }
        else
        {
            if (deltaTime > AnimDuration)
            {
                deltaTime = 0;
                AnimDuration = 0;
                if (result == 7)
                {
                    if (AnimNum < DrawACard.Count)
                    {
                        gameView.txt_Left_Count.text = (Convert.ToInt32(gameView.txt_Left_Count.text) + 1).ToString();
                        AnimDuration = AnimationManager.instance.DoAnimation("Anim_DrawACard", new object[] { DrawACard[AnimNum] });
                        AnimNum++;
                    }
                    if (AnimNum == DrawACard.Count && AnimDuration == 0)
                    {
                        BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.Control);
                    }
                }
                else
                {
                    BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.Control);
                }
            }
        }
    }
    public override void Exit()
    {
        CardUseEffectManager.instance.BeRemoveCardNum = 0;//恢复初始值
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
                break;
            case PlayerType.NormalRobot:
            case PlayerType.AiRobot:

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
