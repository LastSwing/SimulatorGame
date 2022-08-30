using Assets.Script.Models;
using Assets.Script.Models.Map;
using Assets.Script.Tools;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameView : BaseUI
{
    #region 控件字段
    Image img_Background;//背景图片
    public Text txt_ReturnView, txt_SettingHasBtn, txt_ReturnView1, txt_CardType, txt_HasClickSetting, txt_ClickPlayerOrAI;
    public Button btn_Setting, btn_RoundOver, btn_leftCards, btn_rightCards, btn_CopyCard, btn_Player, btn_Enemy;
    public Image img_Player, img_Enemy, Pimg_HP, Eimg_HP, Pimg_Armor, Eimg_Armor;
    public GameObject obj_CardPools, P_buffObj, E_buffObj, MagnifyObj, CardDetailObj, obj_RemoveCard, AiATKBar_obj;
    public Text txt_P_HP, txt_E_HP, txt_P_Armor, txt_E_Armor, txt_Left_Count, txt_Right_Count;
    public RectTransform thisParent;
    #endregion
    #region 数据字段声明
    public int chpterID, AIAtkNum;//关卡ID,AI攻击次数
    private CurrentRoleModel PlayerRole;    //玩家角色
    private CurrentRoleModel AiRole;        //Ai角色
    private List<CurrentRoleModel> AIPools;//AI池

    /// <summary>
    /// 玩家手牌
    /// </summary>
    private List<CurrentCardPoolModel> PlayerHandCardList = new List<CurrentCardPoolModel>();
    /// <summary>
    /// 未使用的卡池
    /// </summary>
    public List<CurrentCardPoolModel> UnusedCardList;
    /// <summary>
    /// 当前攻击栏卡池
    /// </summary>
    public List<CurrentCardPoolModel> ATKBarCardList = new List<CurrentCardPoolModel>();
    /// <summary>
    /// 已使用的卡池
    /// </summary>
    public List<CurrentCardPoolModel> UsedCardList = new List<CurrentCardPoolModel>();
    /// <summary>
    /// AI手牌
    /// </summary>
    public List<CurrentCardPoolModel> AiCardList = new List<CurrentCardPoolModel>();
    /// <summary>
    /// AI攻击牌池
    /// </summary>
    public List<CurrentCardPoolModel> AiATKCardList = new List<CurrentCardPoolModel>();

    #endregion
    #region 卡牌长按事件
    bool hasDown = false;
    float downTime = 0;
    #endregion
    #region 动画声明
    public GameObject AnimObj;
    public Animation Anim_Shuffle, Anim_ShowTitle, Anim_DealCard, Anim_RecycleCard, Anim_ATK, Anim_HPDeduction, Anim_HPRestore, Anim_Armor, Anim_EnergyRestore, Anim_PlayerDie, Anim_RemoveCard, Anim_DrawACard, Anim_ArmorMelting, Anim_Elude;
    //int cardRotationNum = 18;
    #endregion
    public string hasPlayerOrAIDie;//0AI，1角色
    public CurrentCardPoolModel CrtAIATKModel;//当前AI攻击卡
    #region OnInit
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
        #region 动画  
        AnimObj = transform.Find("UI/Animation").gameObject;
        Anim_Shuffle = transform.Find("UI/Animation/Anim_Shuffle").GetComponent<Animation>();
        Anim_ShowTitle = transform.Find("UI/Animation/Anim_ShowTitle").GetComponent<Animation>();
        Anim_DealCard = transform.Find("UI/Animation/Anim_DealCard").GetComponent<Animation>();
        Anim_RecycleCard = transform.Find("UI/Animation/Anim_RecycleCard").GetComponent<Animation>();
        Anim_ATK = transform.Find("UI/Animation/Anim_ATK").GetComponent<Animation>();
        Anim_HPDeduction = transform.Find("UI/Animation/Anim_HPDeduction").GetComponent<Animation>();
        Anim_HPRestore = transform.Find("UI/Animation/Anim_HPRestore").GetComponent<Animation>();
        Anim_Armor = transform.Find("UI/Animation/Anim_Armor").GetComponent<Animation>();
        Anim_EnergyRestore = transform.Find("UI/Animation/Anim_EnergyRestore").GetComponent<Animation>();
        Anim_PlayerDie = transform.Find("UI/Animation/Anim_PlayerDie").GetComponent<Animation>();
        Anim_RemoveCard = transform.Find("UI/Animation/Anim_RemoveCard").GetComponent<Animation>();
        Anim_DrawACard = transform.Find("UI/Animation/Anim_DrawACard").GetComponent<Animation>();
        Anim_ArmorMelting = transform.Find("UI/Animation/Anim_ArmorMelting").GetComponent<Animation>();
        Anim_Elude = transform.Find("UI/Animation/Anim_Elude").GetComponent<Animation>();
        //AnimObj.SetActive(false);
        #endregion

        //Common.SaveTablesStruct<TriggerAfterUsing>("TriggerAfterUsing");
        txt_ReturnView = GameObject.Find("MainCanvas/txt_ReturnView").GetComponent<Text>();
        txt_ReturnView1 = GameObject.Find("MainCanvas/txt_ReturnView1").GetComponent<Text>();
        txt_SettingHasBtn = GameObject.Find("MainCanvas/txt_SettingHasBtn").GetComponent<Text>();
        txt_CardType = GameObject.Find("MainCanvas/txt_CardType").GetComponent<Text>();
        txt_HasClickSetting = GameObject.Find("MainCanvas/txt_HasClickSetting").GetComponent<Text>();
        txt_ClickPlayerOrAI = GameObject.Find("MainCanvas/txt_ClickPlayerOrAI").GetComponent<Text>();

        img_Background = transform.Find("BG").GetComponent<Image>();
        btn_Setting = transform.Find("UI/Setting").GetComponent<Button>();
        btn_RoundOver = transform.Find("UI/CardPools/btn_RoundOver").GetComponent<Button>();
        btn_leftCards = transform.Find("UI/CardPools/left_Card").GetComponent<Button>();
        btn_rightCards = transform.Find("UI/CardPools/right_Card").GetComponent<Button>();
        btn_CopyCard = transform.Find("UI/CardPools/obj_RemoveCard/Button").GetComponent<Button>();
        btn_Player = transform.Find("UI/Player/Player").GetComponent<Button>();
        btn_Enemy = transform.Find("UI/Enemy/Enemy").GetComponent<Button>();

        img_Player = transform.Find("UI/Player/Player").GetComponent<Image>();
        img_Enemy = transform.Find("UI/Enemy/Enemy").GetComponent<Image>();
        Pimg_HP = transform.Find("UI/Player/Pimg_HP").GetComponent<Image>();
        Eimg_HP = transform.Find("UI/Enemy/Eimg_HP").GetComponent<Image>();
        Pimg_Armor = transform.Find("UI/Player/img_Armor").GetComponent<Image>();
        Eimg_Armor = transform.Find("UI/Enemy/img_Armor").GetComponent<Image>();

        obj_CardPools = transform.Find("UI/CardPools/Card").gameObject;
        P_buffObj = transform.Find("UI/Player/BuffBar").gameObject;
        E_buffObj = transform.Find("UI/Enemy/BuffBar").gameObject;
        MagnifyObj = transform.Find("UI/CardPools/obj_Magnify").gameObject;
        CardDetailObj = transform.Find("UI/CardPools/obj_CardDetails").gameObject;
        obj_RemoveCard = transform.Find("UI/CardPools/obj_RemoveCard").gameObject;
        AiATKBar_obj = transform.Find("UI/Enemy/ATKBar").gameObject;

        txt_P_HP = transform.Find("UI/Player/Text").GetComponent<Text>();
        txt_E_HP = transform.Find("UI/Enemy/Text").GetComponent<Text>();
        txt_P_Armor = transform.Find("UI/Player/img_Armor/Text").GetComponent<Text>();
        txt_E_Armor = transform.Find("UI/Enemy/img_Armor/Text").GetComponent<Text>();
        txt_Left_Count = transform.Find("UI/CardPools/left_Card/txt_StartCardCount").GetComponent<Text>();
        txt_Right_Count = transform.Find("UI/CardPools/right_Card/txt_EndCardCount").GetComponent<Text>();

        thisParent = obj_CardPools.GetComponent<RectTransform>();
        //Common.UpdateColumnData<CardPoolModel>(1026, 1093, GlobalAttr.GlobalPlayerCardPoolFileName);
    }

    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {
        btn_Setting.onClick.AddListener(SettingClick);
        btn_leftCards.onClick.AddListener(LeftCardsClick);
        btn_rightCards.onClick.AddListener(RightCardsClick);
        btn_RoundOver.onClick.AddListener(RoundOverClick);
        btn_Player.onClick.AddListener(delegate { ShowBuffDetails("0"); });
        btn_Enemy.onClick.AddListener(delegate { ShowBuffDetails("1"); });
    }

    #endregion

    #region 点击事件
    /// <summary>
    /// 点击展示角色所拥有的的buff详情
    /// </summary>
    /// <param name="playerOrAI"></param>
    public void ShowBuffDetails(string playerOrAI)
    {
        txt_ReturnView.text = "GameView";
        txt_CardType.text = "1";
        txt_HasClickSetting.text = "1";
        txt_ClickPlayerOrAI.text = playerOrAI;
        UIManager.instance.OpenView("BUFFDetailsView");
        UIManager.instance.CloseView("GameView");
    }

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
    /// 鼠标按下
    /// </summary>
    public void CardPointerDown()
    {
        hasDown = true;
    }

    /// <summary>
    /// 卡牌点击事件
    /// 鼠标抬起
    /// </summary>
    public void CardClick(GameObject thisObj)
    {
        //hasDown = false;
        //if (downTime > 1)
        //{
        if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.Control)
        {
            var model = thisObj.GetComponent<CardItem>().BasisData;
            var MagnifyObj = transform.Find("UI/CardPools/obj_Magnify").gameObject;
            int childCount = MagnifyObj.transform.childCount;
            for (int x = 0; x < childCount; x++)
            {
                DestroyImmediate(MagnifyObj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
            //显示详情
            var img_Detail = transform.Find("UI/CardPools/obj_CardDetails/img_Detail" + model.SingleID)?.GetComponent<Image>();
            if (img_Detail != null)
            {
                img_Detail.transform.localScale = Vector3.one;
            }
            else
            {
                GameObject tempObject = ResourcesManager.instance.Load("img_CardDetail") as GameObject;
                tempObject = Common.AddChild(CardDetailObj.transform, tempObject);
                tempObject.name = "img_Detail" + model.SingleID;
                Common.CardDetailDataBind(tempObject, model);
            }
        }
        //}
        //downTime = 0;
    }

    /// <summary>
    /// 复制卡牌
    /// </summary>
    /// <param name="thisObj">被复制的物体</param>
    public void CopyCard(GameObject thisObj)
    {
        if (CardUseEffectManager.instance.CopyBoxCardExist)
        {
            CardItem cardItem = thisObj.GetComponent<CardItem>();
            //不进行深拷贝会修改到原来的数据。导致有两个相同的唯一ID
            var newModel = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentATKBarCardPoolsFileName).Find(a => a.SingleID == cardItem.BasisData.SingleID);
            newModel.SingleID = BattleManager.instance.OwnPlayerData[0].handCardList.Max(a => a.SingleID) + 1;
            ATKBarCardList.Add(newModel);
            BattleManager.instance.OwnPlayerData[0].handCardList.Add(newModel);
            //Common.SaveTxtFile(ATKBarCardList.ListToJson(), GlobalAttr.CurrentATKBarCardPoolsFileName);
            CreateAtkBarCard(newModel, true);

            LayoutRebuilder.ForceRebuildLayoutImmediate(thisParent);
            CardUseEffectManager.instance.UseCopyCard = false;
            CardUseEffectManager.instance.CopyBoxCardExist = false;
            BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.AfterCardUse);
            obj_RemoveCard.SetActive(false);
        }
    }

    /// <summary>
    /// 移除一张卡牌
    /// </summary>
    /// <param name="thisObj"></param>
    public void RemoveCard(GameObject thisObj, ref float animDuration)
    {
        if (CardUseEffectManager.instance.CopyBoxCardExist)
        {
            CardItem cardItem = thisObj.GetComponent<CardItem>();
            var model = cardItem.BasisData;//移除当前卡
            ATKBarCardList.Remove(ATKBarCardList.Find(a => a.SingleID == model.SingleID));
            BattleManager.instance.OwnPlayerData[0].handCardList.Remove(BattleManager.instance.OwnPlayerData[0].handCardList.Find(a => a.SingleID == model.SingleID));

            DeleteGameObj(thisObj);
            LayoutRebuilder.ForceRebuildLayoutImmediate(thisParent);
            CardUseEffectManager.instance.UseCopyCard = false;
            CardUseEffectManager.instance.CopyBoxCardExist = false;
            CardUseEffectManager.instance.hasExecuteCardEffect = true;
            animDuration = AnimationManager.instance.DoAnimation("Anim_RemoveCard", null);
            obj_RemoveCard.SetActive(false);
        }
    }

    /// <summary>
    /// 丢弃一张手牌
    /// </summary>
    /// <param name="thisObj"></param>
    public void DiscardOneCard(GameObject thisObj, ref float animDuration)
    {
        if (CardUseEffectManager.instance.CopyBoxCardExist)
        {
            CardItem cardItem = thisObj.GetComponent<CardItem>();
            var model = cardItem.BasisData;//移除当前卡
            var discard = ATKBarCardList.Find(a => a.SingleID == model.SingleID);
            ATKBarCardList.Remove(discard);
            UsedCardList.Add(discard);

            DeleteGameObj(thisObj);
            animDuration = AnimationManager.instance.DoAnimation("Anim_RecycleCard", null);
            LayoutRebuilder.ForceRebuildLayoutImmediate(thisParent);
            CardUseEffectManager.instance.UseCopyCard = false;
            CardUseEffectManager.instance.CopyBoxCardExist = false;
            CardUseEffectManager.instance.hasExecuteCardEffect = true;
            obj_RemoveCard.SetActive(false);
        }
    }
    #endregion

    #region OnOpen
    public override void OnOpen()
    {
        //数据需要每次打开都要刷新，UI状态也是要每次打开都进行刷新，因此放在OnOpen
        if (txt_HasClickSetting.text == "0")
        {
            InitUIData();
            StartGame();
            InitUIState();
            InitSetting();
            InitBUFF();
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
        AiCardList = new List<CurrentCardPoolModel>();
        AiATKCardList = new List<CurrentCardPoolModel>();
        UsedCardList = new List<CurrentCardPoolModel>();
        Common.SaveTxtFile(null, GlobalAttr.CurrentUsedCardPoolsFileName);
        PlayerRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName);
        AIPools = Common.GetTxtFileToList<CurrentRoleModel>(GlobalAttr.GlobalAIRolePoolFileName);
        #region 按AI等级生成AI
        //MapView mapView = UIManager.instance.GetView("MapView") as MapView;
        //if (mapView.AILevel == 100)
        //{
        //    AiRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentAIRoleFileName);
        //}
        //else
        //{
        //    AiRole = Common.GetTxtFileToList<CurrentRoleModel>(GlobalAttr.GlobalAIRolePoolFileName).FindAll(a => a.AILevel == mapView.AILevel).ListRandom()[0];//由等级随机一个AI
        //    //AiRole = Common.GetTxtFileToList<CurrentRoleModel>(GlobalAttr.GlobalAIRolePoolFileName).Find(a => a.RoleID == 1016);//指定AI
        //} 
        #endregion
        #region 按当前地图生成AI
        CurrentMapLocation mapLocation = Common.GetTxtFileToModel<CurrentMapLocation>(GlobalAttr.CurrentMapLocationFileName, "Map");
        switch (mapLocation.Row)
        {
            case 1:
            case 2:
            case 3:
                AiRole = AIPools.FindAll(a => a.AILevel == 1).ListRandom()[0];//由等级随机一个AI
                break;
            case 4:
            case 5:
            case 6:
                int randomLevel = UnityEngine.Random.Range(1, 3);
                AiRole = AIPools.FindAll(a => a.AILevel == randomLevel).ListRandom()[0];//由等级随机一个AI
                break;
            case 7:
            case 8:
            case 9:
                randomLevel = UnityEngine.Random.Range(1, 4);
                AiRole = AIPools.FindAll(a => a.AILevel == randomLevel).ListRandom()[0];//由等级随机一个AI
                break;
            case 10:
            case 11:
            case 12:
                randomLevel = UnityEngine.Random.Range(1, 5);
                AiRole = AIPools.FindAll(a => a.AILevel == randomLevel).ListRandom()[0];//由等级随机一个AI
                break;
            case 13:
                AiRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentAIRoleFileName);
                break;
            default:
                break;
        }
        if (mapLocation.Row < 4)
        {
            AiRole = Common.GetTxtFileToList<CurrentRoleModel>(GlobalAttr.GlobalAIRolePoolFileName).FindAll(a => a.AILevel == 1).ListRandom()[0];//由等级随机一个AI
        }
        #endregion
        var GlobalCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalCardPoolFileName) ?? new List<CurrentCardPoolModel>();//全局卡池
        PlayerHandCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentCardPoolsFileName);
        UnusedCardList = PlayerHandCardList.ListRandom();
        txt_Left_Count.text = UnusedCardList == null ? "0" : UnusedCardList.Count.ToString();//有动画后再逐一减少
        Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);

        //AI攻击栏最大五张牌
        AIAtkNum = AiRole.AILevel;
        if (AIAtkNum > 5) AIAtkNum = 5;
        if (AIAtkNum < 2) AIAtkNum = 2;
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
                //先赋值唯一ID再存数据
                AiCardList.ListRandom();
                Common.SaveTxtFile(AiCardList.ListToJson(), GlobalAttr.CurrentAiCardPoolsFileName);
                AiCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentAiCardPoolsFileName);//进行深拷贝，防止对象的引用路径未改变。在赋唯一ID值时会出问题

                #region AI攻击栏
                for (int i = 0; i < AIAtkNum; i++)
                {
                    AiATKCardList.Add(AiCardList[i]);
                }
                #endregion
            }
            #endregion
            //AiATKCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentAIATKCardPoolsFileName);//进行深拷贝，防止对象的引用路径未改变。在赋唯一ID值时会出问题
            BattleManager.instance.InitCardPools(GetCardPoolsID(CardPoolType.OwnCards), GetCardPoolsID(CardPoolType.EnemyCards), GetCardPoolsID(CardPoolType.OwnHandCards), GetCardPoolsID(CardPoolType.EnemHandCards));
            Common.SaveTxtFile(AiCardList.ListToJson(), GlobalAttr.CurrentAiCardPoolsFileName);
            Common.SaveTxtFile(AiATKCardList.ListToJson(), GlobalAttr.CurrentAIATKCardPoolsFileName);
            Common.SaveTxtFile(UnusedCardList.ListToJson(), GlobalAttr.CurrentUnUsedCardPoolsFileName);
            Common.SaveTxtFile(PlayerHandCardList.ListToJson(), GlobalAttr.CurrentCardPoolsFileName);
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
        #region 血量初始化
        RectTransform pRect = Pimg_HP.GetComponent<RectTransform>();
        pRect.sizeDelta = new Vector2(400, 45);
        Pimg_HP.transform.localPosition = new Vector3(0, Pimg_HP.transform.localPosition.y);
        RectTransform eRect = Eimg_HP.GetComponent<RectTransform>();
        eRect.sizeDelta = new Vector2(400, 45);
        Eimg_HP.transform.localPosition = new Vector3(0, Pimg_HP.transform.localPosition.y);
        #endregion
        #region 护甲初始化
        txt_P_Armor.text = "0";
        Pimg_Armor.transform.localScale = Vector3.zero;
        txt_E_Armor.text = "0";
        Eimg_Armor.transform.localScale = Vector3.zero;
        #endregion
        #region 清空状态栏
        if (P_buffObj != null)
        {
            int childCount = P_buffObj.transform.childCount;
            for (int x = 0; x < childCount; x++)
            {
                DestroyImmediate(P_buffObj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
        }
        if (E_buffObj != null)
        {
            int childCount = E_buffObj.transform.childCount;
            for (int x = 0; x < childCount; x++)
            {
                DestroyImmediate(E_buffObj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
        }
        #endregion
        #region 清空攻击栏的对象
        if (obj_CardPools != null)
        {
            int childCount = obj_CardPools.transform.childCount;
            for (int x = 0; x < childCount; x++)
            {
                DestroyImmediate(obj_CardPools.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
        }
        #endregion
        #region 清空详情栏对象
        if (CardDetailObj != null)
        {
            int childCount = CardDetailObj.transform.childCount;
            for (int x = 0; x < childCount; x++)
            {
                DestroyImmediate(CardDetailObj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
        }
        #endregion
        txt_Right_Count.text = UsedCardList == null ? "0" : UsedCardList.Count.ToString();
        txt_P_HP.text = $"{PlayerRole.MaxHP}/{PlayerRole.HP}";
        Common.HPImageChange(Pimg_HP, PlayerRole.MaxHP, PlayerRole.MaxHP - PlayerRole.HP, 0);
        txt_E_HP.text = $"{AiRole.MaxHP}/{AiRole.HP}";
        Common.ImageBind(PlayerRole.RoleImgUrl, img_Player);
        Common.ImageBind(AiRole.RoleImgUrl, img_Enemy);
        CreateEnergyImage(PlayerRole.Energy);
        AIATKCardPoolsBind();
    }

    private void InitBUFF()
    {
        if (BattleManager.instance.OwnPlayerData[0].buffList != null)
        {
            BUFFManager.instance.BUFFApply(ref BattleManager.instance.OwnPlayerData[0].buffList, BattleManager.instance.EnemyPlayerData[0].buffList, ref CardUseEffectManager.instance.CurrentCardModel, ref BattleManager.instance.OwnPlayerData[0].handCardList);
        }
    }


    /// <summary>
    /// 初始化其余设置
    /// </summary>
    private void InitSetting()
    {
        SoundManager.instance.PlayOnlyOneSound("GameBGM", (int)TrackType.BGM, true);
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void StartGame()
    {
        BattleManager.instance.Init(GetPlayerDatas(PlayerType.OwnHuman), GetPlayerDatas(PlayerType.NormalRobot, chpterID));
    }

    public override void OnClose()
    {
        //throw new System.NotImplementedException();
    }
    #endregion

    #region DataMethod

    /// <summary>
    /// 同步游戏数据。以BattleManager.instance.OwnPlayerData[0].handCardList为准
    /// </summary>
    /// <param name="list"></param>
    /// <param name="PlayerOrAI">获取AI或玩家的数据;0玩家1AI</param>
    /// <returns></returns>
    public List<CurrentCardPoolModel> SyncGameData(List<CurrentCardPoolModel> list, int PlayerOrAI = 0)
    {
        List<CurrentCardPoolModel> result = new List<CurrentCardPoolModel>();
        if (PlayerOrAI == 0)
        {
            foreach (var item in list)
            {
                var temp = BattleManager.instance.OwnPlayerData[0].handCardList.Find(a => a.SingleID == item.SingleID);
                if (temp != null)
                {
                    result.Add(temp);
                }
            }
        }
        else
        {
            foreach (var item in list)
            {
                var temp = BattleManager.instance.EnemyPlayerData[0].handCardList.Find(a => a.SingleID == item.SingleID);
                if (temp != null)
                {
                    result.Add(temp);
                }
            }
        }
        return result;
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
            case PlayerType.AiRobot:
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
                data.Armor = 0;
                result.Add(data);
                break;
            case PlayerType.OwnHuman:
                //todo
                foreach (var item in UnusedCardList)
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
                data.Armor = 0;
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
                break;
            case CardPoolType.OwnHandCards:
                foreach (var item in PlayerHandCardList)
                {
                    var id = PlayerHandCardList.Max(a => a.SingleID);
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
                break;
            case CardPoolType.EnemyCards:
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
    #endregion

    #region 回合结束

    public void RoundOverClick()
    {
        if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.Control)
        {
            HideCardDetail();
            HideMagnifyCard();
            BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.TurnEnd);
        }
    }

    #endregion

    #region AI攻击
    /// <summary>
    /// AI攻击效果结算
    /// </summary>
    /// <param name="AIAtk">攻击卡数据</param>
    /// <param name="hasUseCard">卡牌是否使用成功</param>
    /// <param name="EffectOn">效果作用在。0AI,1角色</param>
    /// <param name="hasEffect">卡牌是否有效果</param>
    /// <param name="PlayAnim">播放动画、3被闪避</param>
    public void AiAtk(CurrentCardPoolModel AIAtk, ref bool hasUseCard, ref string EffectOn, ref bool hasEffect, ref int PlayAnim)
    {
        CrtAIATKModel = AIAtk;
        var ownRole = BattleManager.instance.OwnPlayerData[0];
        var enemyRole = BattleManager.instance.EnemyPlayerData[0];
        Dictionary<string, string> dic = new Dictionary<string, string>();
        Common.DicDataRead(ref dic, GlobalAttr.EffectTypeFileName, "Card");
        var atkObj = AiATKBar_obj.transform.Find("AiAtkimg_" + AIAtk.SingleID + "(Clone)").gameObject;
        DestroyImmediate(atkObj);
        #region BUFF限制卡牌使用
        var buffResult = BUFFManager.instance.BUFFApply(ref enemyRole.buffList, ownRole.buffList, ref AIAtk, ref enemyRole.handCardList, false, 1);
        if (buffResult?.Count > 0)
        {
            if (buffResult.Exists(a => a.EffectType == 1 && a.HasValid == false))//卡无效果未被使用
            {
                hasUseCard = false;
                AiATKCardList.Remove(AIAtk);
                return;
            }
            if (buffResult.Exists(a => a.EffectType == 2 && a.HasValid == false))//卡无效果已被使用
            {
                AiATKCardList.Remove(AIAtk);
                hasUseCard = true;
                hasEffect = false;
                PlayAnim = 3;
                return;
            }
        }
        #endregion
        #region 攻击
        if (AIAtk.EffectType == 1)//攻击
        {
            int DeductionHp = Convert.ToInt32(AIAtk.Effect);
            if (ownRole.Armor > 0)
            {
                if (ownRole.Armor >= AIAtk.Effect)
                {
                    DeductionHp = 0;
                    ownRole.Armor -= Convert.ToInt32(AIAtk.Effect);
                    txt_P_Armor.text = ownRole.Armor.ToString();
                }
                else
                {
                    DeductionHp -= ownRole.Armor;
                    ownRole.Armor = 0;
                }
                if (ownRole.Armor == 0)
                {
                    Pimg_Armor.transform.localScale = Vector3.zero;
                }
            }
            Common.HPImageChange(Pimg_HP, ownRole.bloodMax, DeductionHp, 0);
            ownRole.bloodNow -= DeductionHp;
            if (ownRole.bloodNow <= 0)
            {
                ownRole.bloodNow = 0;
                //玩家死亡
                PlayerDie();
            }
            txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
            hasUseCard = true;
            EffectOn = "1";
            BUFFUIChange(enemyRole.buffList, ref enemyRole.handCardList, ref AIAtk, 1);
        }
        #endregion
        #region 防御
        else if (AIAtk.EffectType == 2)//防御
        {

            Eimg_Armor.transform.localScale = Vector3.one;
            enemyRole.Armor += Convert.ToInt32(AIAtk.Effect);
            if (enemyRole.Armor > enemyRole.bloodMax)
            {
                enemyRole.Armor = Convert.ToInt32(enemyRole.bloodMax);
            }
            txt_E_Armor.text = enemyRole.Armor.ToString();
            hasUseCard = true;
            EffectOn = "0";
        }
        #endregion
        #region 血量变化
        else if (AIAtk.EffectType == 3)//血量变化
        {
            if (AIAtk.Effect < 0)
            {
                var effect = enemyRole.bloodNow + Convert.ToInt32(AIAtk.Effect);
                if (effect < 0)//血量不足以使用此卡
                {
                    hasUseCard = false;
                    return;
                }
                else
                {
                    Common.HPImageChange(Eimg_HP, enemyRole.bloodMax, AIAtk.Effect, 0);
                    enemyRole.bloodNow += Convert.ToInt32(AIAtk.Effect);
                }
            }
            else
            {
                if (enemyRole.bloodNow != enemyRole.bloodMax)
                {
                    var changeHP = AIAtk.Effect;
                    if (enemyRole.bloodNow + AIAtk.Effect > enemyRole.bloodMax)
                    {
                        changeHP = enemyRole.bloodMax - enemyRole.bloodNow;
                        enemyRole.bloodNow = enemyRole.bloodMax;
                    }
                    else
                    {
                        enemyRole.bloodNow += Convert.ToInt32(changeHP);
                    }
                    Common.HPImageChange(Eimg_HP, enemyRole.bloodMax, changeHP, 1);
                }
            }
            hasUseCard = true;
            EffectOn = "0";
            txt_E_HP.text = $"{enemyRole.bloodMax}/{enemyRole.bloodNow}";
        }
        #endregion
        #region 连续攻击
        else if (AIAtk.EffectType == 5)//连续攻击
        {
            hasUseCard = true;
            EffectOn = "1";
            return;
        }
        #endregion
        #region 销毁防御
        else if (AIAtk.EffectType == 6)//销毁防御
        {
            if (ownRole.Armor > 0)
            {
                ownRole.Armor = 0;
                txt_P_Armor.text = ownRole.Armor.ToString();
                Pimg_Armor.transform.localScale = Vector3.zero;
            }
            hasUseCard = true;
            EffectOn = "1";
        }
        #endregion
        #region 无视防御攻击
        else if (AIAtk.EffectType == 7)//无视防御攻击
        {

            int DeductionHp = Convert.ToInt32(AIAtk.Effect);
            Common.HPImageChange(Pimg_HP, ownRole.bloodMax, DeductionHp, 0);
            ownRole.bloodNow -= DeductionHp;
            if (ownRole.bloodNow <= 0)
            {
                ownRole.bloodNow = 0;
                //玩家死亡
                PlayerDie();
            }
            hasUseCard = true;
            EffectOn = "1";
            txt_P_HP.text = $"{ownRole.bloodMax}/{ownRole.bloodNow}";
        }
        #endregion
        #region 愤怒
        else if (AIAtk.EffectType == 8)//愤怒
        {
            CardUseEffectManager.instance.AddBUFF(1, 1, AIAtk.EffectType, Convert.ToInt32(AIAtk.Effect), ref AIAtk);
            hasUseCard = true;
            EffectOn = "0";
        }
        #endregion
        #region 虚弱
        else if (AIAtk.EffectType == 11)//虚弱
        {
            CardUseEffectManager.instance.AddBUFF(0, 1, AIAtk.EffectType, Convert.ToInt32(AIAtk.Effect), ref AIAtk);
            hasUseCard = true;
            EffectOn = "1";
        }
        #endregion
        #region 暴击等作用在攻击的Buff
        else if (AIAtk.EffectType == 10)//暴击
        {
            CardUseEffectManager.instance.AddBUFF(1, 4, AIAtk.EffectType, Convert.ToInt32(AIAtk.Effect), ref AIAtk);
            hasUseCard = true;
            EffectOn = "0";
        }
        #endregion
        #region 免疫
        else if (AIAtk.EffectType == 9)//免疫
        {
            CardUseEffectManager.instance.AddBUFF(1, 3, AIAtk.EffectType, Convert.ToInt32(AIAtk.Effect), ref AIAtk);
            hasUseCard = true;
            EffectOn = "0";
        }
        #endregion

        AiATKCardList.Remove(AIAtk);

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
        //CurrentRoleModel aiData = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentAIRoleFileName);
        //aiData.Armor = enemyRole.Armor;
        //aiData.HP = enemyRole.bloodNow;
        //aiData.MaxHP = enemyRole.bloodMax;
        //Common.SaveTxtFile(aiData.ObjectToJson(), GlobalAttr.CurrentAIRoleFileName);
        #endregion

    }
    #endregion

    #region GameMethod
    /// <summary>
    /// 玩家死亡
    /// </summary>
    public void PlayerDie()
    {
        hasPlayerOrAIDie = "1";
        BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.GameEnd);
    }

    /// <summary>
    /// AI死亡
    /// </summary>
    public void AiDie()
    {
        hasPlayerOrAIDie = "0";
        BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.GameEnd);
    }
    #endregion

    #region 卡牌效果Method

    /// <summary>
    /// 创建攻击栏卡牌
    /// </summary>
    /// <param name="model"></param>
    /// <param name="hasShow">卡牌是否需要直接展示</param>
    public void CreateAtkBarCard(CurrentCardPoolModel model, bool hasShow = false)
    {
        GameObject tempObject = ResourcesManager.instance.Load("img_Card200") as GameObject;
        tempObject = Common.AddChild(obj_CardPools.transform, tempObject);
        tempObject.name = "imgCard_" + model.SingleID;

        CardItem cardData = tempObject.GetComponent<CardItem>();
        cardData.BasisData = model;
        EventTrigger trigger = tempObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = tempObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(delegate { CardPointerDown(); });
        trigger.triggers.Add(entry);

        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerUp;
        entry1.callback.AddListener(delegate { CardClick(tempObject); });
        trigger.triggers.Add(entry1);
        if (!hasShow)
        {
            tempObject.transform.localScale = Vector3.zero;
            tempObject.transform.localRotation = new Quaternion(0, 180, 0, 0);
        }
        Common.CardDataBind(tempObject, model, BattleManager.instance.OwnPlayerData[0].buffList);
    }

    /// <summary>
    /// 创建能量图片
    /// </summary>
    public void CreateEnergyImage(int count)
    {
        GameObject parentObjectBG = transform.Find("UI/CardPools/obj_EnergyBG").gameObject;
        GameObject parentObject = transform.Find("UI/CardPools/obj_Energy").gameObject;
        if (parentObjectBG != null)
        {
            int childCount = parentObjectBG.transform.childCount;
            for (int x = 0; x < childCount; x++)
            {
                DestroyImmediate(parentObjectBG.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
                DestroyImmediate(parentObject.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
        }
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
        if (AiATKBar_obj != null)
        {
            int childCount = AiATKBar_obj.transform.childCount;
            for (int x = 0; x < childCount; x++)
            {
                DestroyImmediate(AiATKBar_obj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
        }
        if (AiATKCardList != null && AiATKCardList?.Count > 0)
        {
            for (int i = 0; i < AiATKCardList.Count; i++)
            {
                var item = AiATKCardList[i];
                GameObject tempObj = ResourcesManager.instance.Load("AI_ATKimg_Prefab") as GameObject;
                tempObj.name = "AiAtkimg_" + item.SingleID;
                tempObj = Common.AddChild(AiATKBar_obj.transform, tempObj);
                Common.AICardDataBind(tempObj, item);
            }
        }
    }
    /// <summary>
    /// BUFFUI变化
    /// 只在此方法里删除BUFF列表，用于数值恢复
    /// </summary>
    /// <param name="buffDatas">BUFF列表</param>
    /// <param name="handCards">手牌列表</param>
    /// <param name="PlayerOrAI">角色或AI。0角色，1AI</param>
    public void BUFFUIChange(List<BuffData> buffDatas, ref List<CurrentCardPoolModel> handCards, ref CurrentCardPoolModel model, int PlayerOrAI = 0)
    {
        if (PlayerOrAI == 0)
        {
            if (CardUseEffectManager.instance.HasNumberValueChange)
            {
                #region 数值恢复
                foreach (var card in handCards)
                {
                    if (card.CardType == 0)
                    {
                        card.Effect = card.InitEffect;
                    }
                }
                #endregion
                CardUseEffectManager.instance.HasNumberValueChange = false;
            }
            for (int i = 0; i < buffDatas?.Count; i++)
            {
                var item = buffDatas[i];
                var cBuff = P_buffObj.transform.Find("img_Buff_" + item.EffectType).gameObject;
                if (item.Num < 1)
                {
                    DestroyImmediate(cBuff);
                    BattleManager.instance.OwnPlayerData[0].buffList.Remove(item);
                }
                else
                {
                    var txt = cBuff.transform.Find("Text").GetComponent<Text>();
                    txt.text = item.Num.ToString();
                }
            }
            ATKBarCardList = SyncGameData(ATKBarCardList);
            UnusedCardList = SyncGameData(UnusedCardList);
            UsedCardList = SyncGameData(UsedCardList);
            RealTimeChangeCardData(PlayerOrAI);
        }
        else
        {
            if (CardUseEffectManager.instance.HasNumberValueChange)
            {
                #region 数值恢复
                foreach (var card in handCards)
                {
                    if (card.CardType == 0)
                    {
                        card.Effect = card.InitEffect;
                    }
                }
                foreach (var item in AiATKCardList)
                {
                    if (item.CardType == 0)
                    {
                        item.Effect = item.InitEffect;
                    }
                }
                #endregion
                CardUseEffectManager.instance.HasNumberValueChange = false;
                //Common.SaveTxtFile(handCards.ListToJson(), GlobalAttr.CurrentAiCardPoolsFileName);
            }
            for (int i = 0; i < buffDatas?.Count; i++)
            {
                var item = buffDatas[i];
                var cBuff = E_buffObj.transform.Find("img_Buff_" + item.EffectType).gameObject;
                if (item.Num < 1)
                {
                    DestroyImmediate(cBuff);
                    BattleManager.instance.EnemyPlayerData[0].buffList.Remove(item);
                }
                else
                {
                    var txt = cBuff.transform.Find("Text").GetComponent<Text>();
                    txt.text = item.Num.ToString();
                }
            }
            AiATKCardList = SyncGameData(AiATKCardList, 1);
        }
    }

    public void DeleteGameObj(GameObject obj)
    {
        DestroyImmediate(obj);
    }

    /// <summary>
    /// 实时变化攻击栏数据
    /// </summary>
    /// <param name="handCards">手牌</param>
    /// <param name="PlayerOrAI">角色或AI。0角色，1AI</param>
    public void RealTimeChangeCardData(int PlayerOrAI = 0)
    {
        if (PlayerOrAI == 0)
        {
            if (ATKBarCardList?.Count > 0)
            {
                ATKBarCardList = SyncGameData(ATKBarCardList);
                foreach (var item in ATKBarCardList)
                {
                    var tempObj = obj_CardPools.transform.Find("imgCard_" + item.SingleID)?.gameObject;
                    if (tempObj != null)
                    {
                        CardItem cardItem = tempObj.GetComponent<CardItem>();
                        cardItem.BasisData = item;
                        Common.CardDataBind(tempObj, item, BattleManager.instance.OwnPlayerData[0].buffList);
                    }
                }
            }
        }
        else
        {
            if (AiATKCardList?.Count > 0)
            {
                AiATKCardList = SyncGameData(AiATKCardList, 1);
                foreach (var item in AiATKCardList)
                {
                    Debug.Log("AiAtkimg_" + item.SingleID + "(Clone)");
                    var tempObj = AiATKBar_obj.transform.Find("AiAtkimg_" + item.SingleID + "(Clone)")?.gameObject;
                    if (tempObj != null)
                    {
                        Common.AICardDataBind(tempObj, item, BattleManager.instance.EnemyPlayerData[0].buffList);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 隐藏卡牌详情
    /// </summary>
    public void HideCardDetail()
    {
        int childCount = CardDetailObj.transform.childCount;
        for (int x = 0; x < childCount; x++)
        {
            CardDetailObj.transform.GetChild(x).gameObject.transform.localScale = Vector3.zero;
        }
    }

    /// <summary>
    /// 隐藏放大后的图片
    /// </summary>
    public void HideMagnifyCard()
    {
        int childCount = MagnifyObj.transform.childCount;
        for (int x = 0; x < childCount; x++)
        {
            DestroyImmediate(MagnifyObj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
        }
    }

    #endregion

    #region Unity函数
    private void Update()
    {
        if (hasDown)
        {
            downTime += Time.deltaTime;
        }
    }
    #endregion

    #region Animaition
    /// <summary>
    /// 卡牌旋转
    /// </summary>
    /// <param name="cardObj"></param>
    /// <param name="deltaTime"></param>
    public void CardRotating(GameObject cardObj, ref float deltaTime, ref int CardRotationNum)
    {
        if (CardRotationNum != 18)
        {
            if (deltaTime > 0.007f)
            {
                cardObj.transform.localScale = Vector3.one;
                cardObj.transform.rotation = Quaternion.Euler(new Vector3(0, cardObj.transform.rotation.eulerAngles.y - 10, 0));
                deltaTime = 0;
                CardRotationNum++;
            }
        }
    }
    #endregion
}

#region 相关类
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
/// buff数据只作用在全局的
/// </summary>
public class BuffData
{
    /// <summary>
    /// 唯一主键
    /// </summary>
    public int ID { get; set; }
    /// <summary>
    /// buff名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// buff次数
    /// </summary>
    public float Num { get; set; }

    /// <summary>
    /// 0应用在卡牌数据上；(如愤怒)
    /// 1应用在卡上；（如束缚）
    /// 2应用在角色身上；（如重伤）
    /// 3免疫；（如免疫）
    /// 4应用在攻击上；（如混乱）
    /// </summary>
    public int BUFFType { get; set; }

    /// <summary>
    /// 同CardPoolModel 的 EffectType
    /// </summary>
    public int EffectType { get; set; }

    /// <summary>
    /// 预留字段
    /// </summary>
    public string ReservedPara { get; set; }
}
#endregion
