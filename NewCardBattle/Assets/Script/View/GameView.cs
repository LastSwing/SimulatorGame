using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameView : BaseUI
{
    Image img_Background;//背景图片
    Text txt_ReturnView, txt_SettingHasBtn, txt_ReturnView1, txt_CardType, txt_HasClickSetting;
    Button btn_Setting, btn_RoundOver, btn_leftCards, btn_rightCards;
    Image img_Player, img_Enemy, Pimg_HP, Eimg_HP, Pimg_Armor, Eimg_Armor;
    GameObject obj_CardPools;
    Text txt_P_HP, txt_E_HP, txt_P_Armor, txt_E_Armor, txt_Left_Count, txt_Right_Count;
    #region 字段声明

    int chpterID;//关卡ID
    private CurrentRoleModel PlayerRole;    //玩家角色
    private CurrentRoleModel AiRole;        //Ai角色

    /// <summary>
    /// 玩家牌库
    /// </summary>
    private List<CurrentCardPoolModel> PlayerCardList = new List<CurrentCardPoolModel>();
    /// <summary>
    /// 玩家手牌
    /// </summary>
    private List<CurrentCardPoolModel> PlayerHandCardList = new List<CurrentCardPoolModel>();
    /// <summary>
    /// 未使用的卡池
    /// </summary>
    private List<CurrentCardPoolModel> UnusedCardList;
    /// <summary>
    /// 当前攻击栏卡池
    /// </summary>
    private List<CurrentCardPoolModel> ATKBarCardList = new List<CurrentCardPoolModel>();
    /// <summary>
    /// 已使用的卡池
    /// </summary>
    private List<CurrentCardPoolModel> UsedCardList = new List<CurrentCardPoolModel>();
    /// <summary>
    /// AI卡池
    /// </summary>
    private List<CurrentCardPoolModel> AiCardList = new List<CurrentCardPoolModel>();
    /// <summary>
    /// AI攻击牌池
    /// </summary>
    private List<CurrentCardPoolModel> AiATKCardList = new List<CurrentCardPoolModel>();

    #endregion
    public override void OnInit()
    {
        //因为获取组件以及绑定事件一般只需要做一次，所以放在OnInit
        InitComponent();
        InitUIevent();
    }

    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitComponent()
    {
        txt_ReturnView = GameObject.Find("MainCanvas/txt_ReturnView").GetComponent<Text>();
        txt_ReturnView1 = GameObject.Find("MainCanvas/txt_ReturnView1").GetComponent<Text>();
        txt_SettingHasBtn = GameObject.Find("MainCanvas/txt_SettingHasBtn").GetComponent<Text>();
        txt_CardType = GameObject.Find("MainCanvas/txt_CardType").GetComponent<Text>();
        txt_HasClickSetting = GameObject.Find("MainCanvas/txt_HasClickSetting").GetComponent<Text>();

        img_Background = transform.Find("BG").GetComponent<Image>();
        btn_Setting = transform.Find("UI/Setting").GetComponent<Button>();
        btn_RoundOver = transform.Find("UI/CardPools/btn_RoundOver").GetComponent<Button>();
        btn_leftCards = transform.Find("UI/CardPools/left_Card").GetComponent<Button>();
        btn_rightCards = transform.Find("UI/CardPools/right_Card").GetComponent<Button>();

        img_Player = transform.Find("UI/Player/Player").GetComponent<Image>();
        img_Enemy = transform.Find("UI/Enemy/Enemy").GetComponent<Image>();
        Pimg_HP = transform.Find("UI/Player/Pimg_HP").GetComponent<Image>();
        Eimg_HP = transform.Find("UI/Enemy/Eimg_HP").GetComponent<Image>();
        Pimg_Armor = transform.Find("UI/Player/img_Armor").GetComponent<Image>();
        Eimg_Armor = transform.Find("UI/Enemy/img_Armor").GetComponent<Image>();

        obj_CardPools = transform.Find("UI/CardPools/Card").gameObject;

        txt_P_HP = transform.Find("UI/Player/Text").GetComponent<Text>();
        txt_E_HP = transform.Find("UI/Enemy/Text").GetComponent<Text>();
        txt_P_Armor = transform.Find("UI/Player/img_Armor/Text").GetComponent<Text>();
        txt_E_Armor = transform.Find("UI/Enemy/img_Armor/Text").GetComponent<Text>();
        txt_Left_Count = transform.Find("UI/CardPools/left_Card/txt_StartCardCount").GetComponent<Text>();
        txt_Right_Count = transform.Find("UI/CardPools/right_Card/txt_EndCardCount").GetComponent<Text>();

    }


    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {
        btn_Setting.onClick.AddListener(SettingClick);
        btn_leftCards.onClick.AddListener(LeftCardsClick);
        btn_rightCards.onClick.AddListener(RightCardsClick);
    }

    #region 点击事件
    public void SettingClick()
    {
        txt_ReturnView.text = "GameView";
        txt_ReturnView1.text = "GameView";
        txt_SettingHasBtn.text = "1";
        txt_HasClickSetting.text = "1";
        UIManager.instance.OpenView("SettingView");
        UIManager.instance.CloseView("GameView");
    }

    public void LeftCardsClick()
    {
        txt_ReturnView.text = "GameView";
        txt_CardType.text = "1";
        txt_HasClickSetting.text = "1";
        UIManager.instance.OpenView("CardPoolsView");
        UIManager.instance.CloseView("GameView");
    }
    public void RightCardsClick()
    {
        txt_ReturnView.text = "GameView";
        txt_CardType.text = "2";
        txt_HasClickSetting.text = "1";
        UIManager.instance.OpenView("CardPoolsView");
        UIManager.instance.CloseView("GameView");
    }

    /// <summary>
    /// 卡牌点击事件
    /// </summary>
    public void CardClick(GameObject thisObj)
    {
        if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.TurnStart)
        {
            int i = Convert.ToInt32(thisObj.name.Substring(8, 1));
            var MagnifyObj = transform.Find("UI/CardPools/obj_Magnify").gameObject;
            int childCount = MagnifyObj.transform.childCount;
            for (int x = 0; x < childCount; x++)
            {
                DestroyImmediate(MagnifyObj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
            //显示详情
            var img_Detail = transform.Find("UI/CardPools/obj_CardDetails/img_Detail" + i)?.GetComponent<Image>();
            if (img_Detail != null)
            {
                img_Detail.transform.localScale = Vector3.one;
            }
            else
            {
                var parentObj = transform.Find("UI/CardPools/obj_CardDetails").gameObject;
                GameObject tempObject = ResourcesManager.instance.Load("img_CardDetail") as GameObject;
                tempObject = Common.AddChild(parentObj.transform, tempObject);
                tempObject.name = "img_Detail" + i;

                GameObject temp = tempObject.transform.Find("Text").gameObject;
                temp.GetComponent<Text>().text = $"{ATKBarCardList[i].CardName}\n{ATKBarCardList[i].CardDetail}";
            }
        }
    }

    #endregion

    public override void OnOpen()
    {
        //数据需要每次打开都要刷新，UI状态也是要每次打开都进行刷新，因此放在OnOpen
        if (txt_HasClickSetting.text == "0")
        {
            InitUIData();
            InitUIState();
            InitSetting();
            StartGame();
        }
        else
        {
            txt_HasClickSetting.text = "0";
        }
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void InitUIData()
    {
        chpterID = 1;//目前使用测试数据，固定为第一关

        #region 数据源初始化
        Common.SaveTxtFile(null, GlobalAttr.CurrentUsedCardPoolsFileName);
        PlayerRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName);
        AiRole = Common.GetTxtFileToList<CurrentRoleModel>(GlobalAttr.GlobalAIRolePoolFileName).Find(a => a.RoleID == 1004);//由等级随机一个AI
        Common.SaveTxtFile(AiRole.ObjectToJson(), GlobalAttr.CurrentAIRoleFileName);
        var GlobalCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalCardPoolFileName) ?? new List<CurrentCardPoolModel>();//全局卡池
        //UnusedCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentCardPoolsFileName);
        PlayerHandCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentCardPoolsFileName);
        PlayerCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName);
        UnusedCardList = PlayerHandCardList.ListRandom();
        Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);

        if (GlobalCardPools != null && GlobalCardPools?.Count > 0)
        {
            #region AI牌池
            if (!string.IsNullOrEmpty(AiRole.CardListStr))
            {
                var arr = AiRole.CardListStr.Split(';');
                for (int i = 0; i < arr.Length; i++)
                {
                    CurrentCardPoolModel cardModel = new CurrentCardPoolModel();
                    var id = arr[i].Split('|')[0].ToString().Trim();
                    if (!string.IsNullOrEmpty(id))
                    {
                        cardModel = GlobalCardPools.Find(a => a.ID == Convert.ToInt32(id));
                        AiCardList.Add(cardModel);
                    }
                }
                AiCardList.ListRandom();
                Common.SaveTxtFile(AiCardList.ListToJson(), GlobalAttr.CurrentAiCardPoolsFileName);
                AiCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentAiCardPoolsFileName);//进行深拷贝，防止对象的引用未改变。在赋唯一ID值时会出问题

                #region AI攻击栏
                //攻击栏最大五张牌
                var AtkCardNum = AiRole.AILevel + 1;
                if (AtkCardNum > 5) AtkCardNum = 5;
                for (int i = 0; i < AtkCardNum; i++)
                {
                    AiATKCardList.Add(AiCardList[i]);
                }
                Common.SaveTxtFile(AiATKCardList.ListToJson(), GlobalAttr.CurrentAIATKCardPoolsFileName);
                #endregion
            }
            #endregion

            BattleManager.instance.InitCardPools(GetCardPoolsID(CardPoolType.OwnCards), GetCardPoolsID(CardPoolType.EnemyCards), GetCardPoolsID(CardPoolType.OwnHandCards), GetCardPoolsID(CardPoolType.EnemHandCards));
            #region 攻击栏卡池
            for (int i = 0; i < 5; i++)
            {
                ATKBarCardList.Add(UnusedCardList[i]);
                UnusedCardList.Remove(UnusedCardList[i]);
            }
            Common.SaveTxtFile(ATKBarCardList.ListToJson(), GlobalAttr.CurrentATKBarCardPoolsFileName);
            Common.SaveTxtFile(UnusedCardList.ListToJson(), GlobalAttr.CurrentUnUsedCardPoolsFileName);
            #endregion 
        }
        #endregion
        #region 数据初始化

        //能量恢复最大值
        PlayerRole.Energy = PlayerRole.MaxEnergy;
        //防御值清空
        PlayerRole.Armor = 0;
        #endregion

    }

    /// <summary>
    /// 更新UI状态
    /// </summary>
    private void InitUIState()
    {
        txt_Right_Count.text = UsedCardList == null ? "0" : UsedCardList.Count.ToString();
        txt_Left_Count.text = UnusedCardList == null ? "0" : UsedCardList.Count.ToString();//有动画后再逐一减少
        txt_P_HP.text = $"{PlayerRole.MaxHP}/{PlayerRole.HP}";
        Common.HPImageChange(Pimg_HP, PlayerRole.MaxHP, PlayerRole.MaxHP - PlayerRole.HP, 0);
        txt_E_HP.text = $"{AiRole.MaxHP}/{AiRole.HP}";
        Common.ImageBind(PlayerRole.RoleImgUrl, img_Player);
        Common.ImageBind(AiRole.RoleImgUrl, img_Enemy);
        CreateEnergyImage(PlayerRole.Energy);
        AIATKCardPoolsBind();
        CreateAtkBarCard();

    }

    #region Method
    /// <summary>
    /// 创建攻击栏卡牌
    /// </summary>
    public void CreateAtkBarCard()
    {
        for (int i = 0; i < ATKBarCardList.Count; i++)
        {
            GameObject tempObject = ResourcesManager.instance.Load("img_Card200") as GameObject;
            tempObject = Common.AddChild(obj_CardPools.transform, tempObject);
            tempObject.name = "imgCard_" + ATKBarCardList[i].SingleID;

            EventTrigger trigger = tempObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = tempObject.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.callback.AddListener(delegate { CardClick(tempObject); });
            trigger.triggers.Add(entry);

            Common.CardDataBind(tempObject, ATKBarCardList[i]);
        }
    }

    /// <summary>
    /// 创建能量图片
    /// </summary>
    public void CreateEnergyImage(int count)
    {
        GameObject parentObjectBG = transform.Find("UI/CardPools/obj_EnergyBG").gameObject;
        GameObject parentObject = transform.Find("UI/CardPools/obj_Energy").gameObject;
        //Debug.Log(parentObject.transform.localPosition);
        for (int i = 0; i < count; i++)
        {
            GameObject tempBgObject = ResourcesManager.instance.Load("img_EnergyBG") as GameObject;
            tempBgObject = Common.AddChild(parentObjectBG.transform, tempBgObject);
            tempBgObject.name = "img_EnergyBg" + i;

            GameObject tempObject = ResourcesManager.instance.Load("img_Energy") as GameObject;
            tempObject = Common.AddChild(parentObject.transform, tempObject);
            tempObject.name = "img_Energy" + i;
        }
    }

    /// <summary>
    /// AI攻击牌池绑定
    /// </summary>
    public void AIATKCardPoolsBind()
    {
        if (AiATKCardList != null && AiATKCardList?.Count > 0)
        {
            GameObject parentObject = transform.Find("UI/Enemy/ATKBar").gameObject;
            for (int i = 0; i < AiATKCardList.Count; i++)
            {
                var item = AiATKCardList[i];
                GameObject tempObj = ResourcesManager.instance.Load("AI_ATKimg_Prefab") as GameObject;
                tempObj.name = item.ID + "_" + i;
                tempObj = Common.AddChild(parentObject.transform, tempObj);
                var tempImg = parentObject.transform.Find($"{item.ID}_{i}(Clone)").GetComponent<Image>();
                Common.ImageBind(item.CardUrl, tempImg);
            }
        }
    }
    #endregion

    /// <summary>
    /// 初始化其余设置
    /// </summary>
    private void InitSetting()
    {
        //SoundManager.instance.PlayOnlyOneSound("BGM_1", (int)TrackType.BGM, true);
    }

    public override void OnClose()
    {
        //throw new System.NotImplementedException();
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void StartGame()
    {
        BattleManager.instance.Init(GetPlayerDatas(PlayerType.OwnHuman), GetPlayerDatas(PlayerType.NormalRobot, chpterID));
    }

    /// <summary>
    /// 获取战斗双方数据
    /// </summary>
    /// <param name="playerType"></param>
    private List<PlayerData> GetPlayerDatas(PlayerType playerType, int other = -1)//视情况提供第二个参数
    {
        List<PlayerData> result = new List<PlayerData>();
        PlayerData data = new PlayerData();
        var dic = new Dictionary<int, int>();
        List<CurrentCardPoolModel> list = new List<CurrentCardPoolModel>();
        switch (playerType)
        {
            case PlayerType.NormalRobot:
                //todo
                foreach (var item in AiCardList)
                {
                    dic.Add(item.SingleID, item.ID);
                    list.Add(item);
                }
                data.playerID = AiRole.RoleID;
                data.playerName = AiRole.Name;
                data.playerPos = img_Enemy.transform.position;
                data.playerType = playerType;
                data.speed = 2;
                data.bloodMax = Convert.ToInt32(AiRole.MaxHP);
                data.bloodNow = Convert.ToInt32(AiRole.HP);
                data.cardDic = dic;
                data.handCardList = list;
                data.Energy = PlayerRole.Energy;
                data.EnergyMax = PlayerRole.MaxEnergy;
                result.Add(data);
                break;
            case PlayerType.AiRobot:
                //todo
                break;
            case PlayerType.OwnHuman:
                //todo
                foreach (var item in UnusedCardList)
                {
                    dic.Add(item.SingleID, item.ID);
                    list.Add(item);
                }
                foreach (var item in ATKBarCardList)
                {
                    dic.Add(item.SingleID, item.ID);
                    list.Add(item);
                }
                data.playerID = PlayerRole.RoleID;
                data.playerName = PlayerRole.Name;
                data.playerPos = img_Player.transform.position;
                data.playerType = playerType;
                data.speed = 1;
                data.bloodMax = Convert.ToInt32(PlayerRole.MaxHP);
                data.bloodNow = Convert.ToInt32(PlayerRole.HP);
                data.cardDic = dic;
                data.handCardList = list;
                data.Energy = PlayerRole.Energy;
                data.EnergyMax = PlayerRole.MaxEnergy;
                data.Wealth = PlayerRole.Wealth;
                result.Add(data);
                break;
            case PlayerType.OtherHuman:
                //todo
                break;
        }
        return result;
    }
    /// <summary>
    /// 获取设置卡唯一ID
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private List<int> GetCardPoolsID(CardPoolType type)
    {
        List<int> list = new List<int>();
        switch (type)
        {
            case CardPoolType.OwnCards:
                foreach (var item in PlayerCardList)
                {
                    if (item.SingleID > 0)
                    {
                        list.Add(item.SingleID);
                    }
                    else
                    {
                        var id = PlayerCardList.Max(a => a.SingleID);
                        if (id < 1)
                        {
                            id = 10000;
                        }
                        else
                        {
                            id += 1;
                            list.Add(id);
                        }
                        item.SingleID = id;
                    }
                }
                break;
            case CardPoolType.OwnHandCards:
                foreach (var item in PlayerHandCardList)
                {
                    if (item.SingleID > 0)
                    {
                        list.Add(item.SingleID);
                    }
                    else
                    {
                        var id = PlayerHandCardList.Max(a => a.SingleID);
                        if (id < 1)
                        {
                            id = 100000;
                        }
                        else
                        {
                            id += 1;
                            list.Add(id);
                        }
                        item.SingleID = id;
                    }
                }
                break;
            case CardPoolType.EnemyCards:
                foreach (var item in AiCardList)
                {
                    if (item.SingleID > 0)
                    {
                        list.Add(item.SingleID);
                    }
                    else
                    {
                        var id = AiCardList.Max(a => a.SingleID);
                        if (id < 1)
                        {
                            id = 100;
                        }
                        else
                        {
                            id += 1;
                            list.Add(id);
                        }
                        item.SingleID = id;
                    }
                }
                break;
            case CardPoolType.EnemHandCards:
                foreach (var item in AiCardList)
                {
                    if (item.SingleID > 0)
                    {
                        list.Add(item.SingleID);
                    }
                    else
                    {
                        var id = AiCardList.Max(a => a.SingleID);
                        if (id < 1)
                        {
                            id = 100;
                        }
                        else
                        {
                            id += 1;
                            list.Add(id);
                        }
                        item.SingleID = id;
                    }
                }
                break;
        }
        return list;
    }
}
/// <summary>
/// 卡堆类型
/// </summary>
public enum CardPoolType
{
    /// <summary>
    /// 玩家所有牌堆
    /// </summary>
    OwnCards,
    /// <summary>
    /// 玩家手牌
    /// </summary>
    OwnHandCards,
    /// <summary>
    /// AI牌堆
    /// </summary>
    EnemyCards,
    /// <summary>
    /// AI手牌
    /// </summary>
    EnemHandCards
}

/// <summary>
/// 对战者类型
/// </summary>
public enum PlayerType
{
    /// <summary>
    /// 固定设置的机器人
    /// </summary>
    NormalRobot,
    /// <summary>
    /// rouge机器人
    /// </summary>
    AiRobot,
    /// <summary>
    /// 玩家自己
    /// </summary>
    OwnHuman,
    /// <summary>
    /// 联机玩家
    /// </summary>
    OtherHuman,
}

/// <summary>
/// 玩家数据
/// </summary>
public class PlayerData
{
    /// <summary>
    /// 对战者拥有的卡牌池[唯一ID,卡牌ID]
    /// </summary>
    public Dictionary<int, int> cardDic;
    /// <summary>
    /// 对站者当前的buff列表
    /// </summary>
    public List<BuffData> buffList;
    /// <summary>
    /// 玩家类型
    /// </summary>
    public PlayerType playerType;
    /// <summary>
    /// 当前血量
    /// </summary>
    public int bloodNow;
    /// <summary>
    /// 血量上限
    /// </summary>
    public int bloodMax;
    /// <summary>
    /// 玩家名字
    /// </summary>
    public string playerName;
    /// <summary>
    /// 玩家ID
    /// </summary>
    public int playerID;
    /// <summary>
    /// 玩家位置
    /// </summary>
    public Vector3 playerPos;
    /// <summary>
    /// 先手速度权重
    /// </summary>
    public int speed;
    /// <summary>
    /// 手牌
    /// </summary>
    public List<CurrentCardPoolModel> handCardList;
    /// <summary>
    /// 当前能量
    /// </summary>
    public int Energy;
    /// <summary>
    /// 最大能量
    /// </summary>
    public int EnergyMax;
    /// <summary>
    /// 防御值
    /// </summary>
    public int Armor;
    /// <summary>
    /// 财富值
    /// </summary>
    public float Wealth;
}

/// <summary>
/// buff数据
/// </summary>
public class BuffData
{
    
    /// <summary>
    /// buff名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// buff次数
    /// </summary>
    public float Num { get; set; }

    /// <summary>
    /// 同CardPoolModel 的 EffectType
    /// </summary>
    public int EffectType { get; set; }
}
