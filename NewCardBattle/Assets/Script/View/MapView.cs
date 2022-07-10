using Assets.Script.Models;
using Assets.Script.Models.Map;
using Assets.Script.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapView : BaseUI
{
    Image img_map;//背景图片
    Image img_Boss_Head, img_Player_Head, img_Init;
    Text txt_CardPoolsCount, txt_ReturnView, txt_CardType, txt_SettingHasBtn, txt_ReturnView1;
    Button btn_GameStart, btn_Suspension_Boss, btn_Suspension_Player, btn_CardPools, btn_Setting;
    GameObject UI_Obj;

    int OneUnitCount, MoveType;
    int MapRow = 1;
    int OneRowY = -1640;
    int OnePathY = -1520;
    bool MoveUpState, MoveTopState, MoveTopOneUnitState, CreateMapState;
    List<int> ListPreviousRow = new List<int>() { 1 };//上一行模块的数量
    CurrentMapLocation mapLocation;
    /// <summary>
    /// 当前地图AiBoss
    /// </summary>
    CurrentRoleModel CurrentAiModel;

    CurrentRoleModel PlayerRole;
    GlobalPlayerModel GlobalRole;

    List<MapCombatPoint> listCombatPoint;
    List<MapPath> listPath;
    #region 地图移动
    private Vector2 first = Vector2.zero;//鼠标第一次落下点
    private Vector2 second = Vector2.zero;//鼠标第二次位置（拖拽位置）
    private Vector3 vecPos = Vector3.zero;//鼠标移动了多少
    private Vector2 InitPos = Vector2.zero;//初始位置
    private bool IsNeedMove, HasMouseDown, HasMouseUp = false;//是否需要移动 
    #endregion
    float bottomY, topY, lineSpacing;//地图底部Y,地图顶部Y,每个战斗点间距
    #region OnInit
    public override void OnInit()
    {
        //因为获取组件以及绑定事件一般只需要做一次，所以放在OnInit
        InitComponent();
        InitUIevent();
        //Screen.height; //屏幕高度
    }

    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitComponent()
    {
        img_map = transform.Find("BG").GetComponent<Image>();
        img_Init = transform.Find("BG/Map_Row0/Atk_img0").GetComponent<Image>();
        InitPos = img_map.transform.position;
        btn_GameStart = transform.Find("UI/btn_Map_StartGame").GetComponent<Button>();
        btn_Suspension_Boss = transform.Find("UI/Suspension_Boss").GetComponent<Button>();
        btn_Suspension_Player = transform.Find("UI/Suspension_Player").GetComponent<Button>();
        btn_CardPools = transform.Find("UI/CardPools_Obj").GetComponent<Button>();

        img_Boss_Head = transform.Find("UI/Suspension_Boss/img_Round/img_Head").GetComponent<Image>();
        img_Player_Head = transform.Find("UI/Suspension_Player/img_Round/img_Head").GetComponent<Image>();
        UI_Obj = transform.Find("UI").gameObject;
        txt_CardPoolsCount = transform.Find("UI/CardPools_Obj/Image/Text").GetComponent<Text>();

        txt_ReturnView = GameObject.Find("MainCanvas/txt_ReturnView").GetComponent<Text>();
        txt_ReturnView1 = GameObject.Find("MainCanvas/txt_ReturnView1").GetComponent<Text>();
        txt_CardType = GameObject.Find("MainCanvas/txt_CardType").GetComponent<Text>();
        txt_SettingHasBtn = GameObject.Find("MainCanvas/txt_SettingHasBtn").GetComponent<Text>();
    }

    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {
        #region 地图点击隐藏title
        EventTrigger trigger2 = img_map.GetComponent<EventTrigger>();
        if (trigger2 == null)
        {
            trigger2 = img_map.transform.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.callback.AddListener(delegate { HideTitle(); });
        trigger2.triggers.Add(entry2);
        #endregion
        btn_Suspension_Boss.onClick.AddListener(Suspension_BossClick);
        btn_Suspension_Player.onClick.AddListener(Suspension_PayerClick);
        btn_CardPools.onClick.AddListener(CardPoolsClick);
    }

    #region 按钮点击事件
    /// <summary>
    /// 隐藏Title
    /// </summary>
    public void HideTitle()
    {
        //路线颜色初始化
        if (mapLocation.Row != 13)
        {
            var CurrentPathObj = transform.Find($"BG/Path_{mapLocation.Row + 1}")?.gameObject;
            for (int i = 0; i < CurrentPathObj.transform.childCount; i++)
            {
                var child = CurrentPathObj.transform.GetChild(i).GetComponent<Image>();
                child.color = Color.gray;
            }

        }
        btn_GameStart.transform.localScale = Vector3.zero;
        for (int i = 0; i < 13; i++)
        {
            var obj = transform.Find($"BG/Map_Row{i}").gameObject;
            for (int j = 0; j < obj.transform.childCount; j++)
            {
                var child = obj.transform.GetChild(j).gameObject;
                var childTitle = child.transform.Find("Map_Title_img")?.GetComponent<Image>();
                if (childTitle != null)
                {
                    childTitle.transform.localScale = Vector3.zero;
                }
            }
        }
        //Boss
        var boss_obj = transform.Find($"BG/Atk_imgBoss/Map_Title_img")?.GetComponent<Image>();
        if (boss_obj != null)
        {
            boss_obj.transform.localScale = Vector3.zero;
        }

    }

    /// <summary>
    /// 动画去到顶部
    /// </summary>
    public void Suspension_BossClick()
    {
        MoveTopState = true;
    }
    /// <summary>
    /// 动画去到角色位置
    /// </summary>
    public void Suspension_PayerClick()
    {
        MoveUpState = true;
    }

    public void CardPoolsClick()
    {
        txt_ReturnView.text = "MapView";
        txt_CardType.text = "0";
        UIManager.instance.OpenView("CardPoolsView");
        UIManager.instance.CloseView("MapView");
    }

    public void SettingClick()
    {
        txt_ReturnView.text = "MapView";
        txt_ReturnView1.text = "MapView";
        txt_SettingHasBtn.text = "1";
        UIManager.instance.OpenView("SettingView");
        UIManager.instance.CloseView("MapView");
    }
    #endregion
    #endregion

    #region OnOpen
    public override void OnOpen()
    {
        //数据需要每次打开都要刷新，UI状态也是要每次打开都进行刷新，因此放在OnOpen
        InitUIData();
        InitUIState();
        InitSetting();
    }

    /// <summary>
    /// 初始化其余设置
    /// </summary>
    private void InitSetting()
    {
        //SoundManager.instance.PlayOnlyOneSound("BGM_1", (int)TrackType.BGM, true);
    }

    /// <summary>
    /// 更新UI状态
    /// </summary>
    private void InitUIState()
    {
        btn_GameStart.transform.localScale = Vector3.zero;

    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void InitUIData()
    {
        bottomY = Screen.height * 2.5f;
        topY = Screen.height * -1.5f;
        lineSpacing = bottomY / 9;
        var tempBar = transform.Find("UI/TopBar")?.gameObject;
        #region 数据初始化
        var listAi = Common.GetTxtFileToList<CurrentRoleModel>(GlobalAttr.GlobalAIRolePoolFileName).FindAll(a => a.AILevel == 1).ListRandom();//ailevel==boss等级
        CurrentAiModel = listAi[0];
        GlobalRole = Common.GetTxtFileToModel<GlobalPlayerModel>(GlobalAttr.GlobalRoleFileName);
        PlayerRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName);
        if (PlayerRole == null)
        {
            PlayerRole = Common.GetTxtFileToList<CurrentRoleModel>(GlobalAttr.GlobalPlayerRolePoolFileName).Find(a => a.RoleID == GlobalRole.CurrentRoleID);
            //TopBar也删除
            if (tempBar != null)
            {
                DestroyImmediate(tempBar);
            }
        }
        Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
        Common.ImageBind(CurrentAiModel.HeadPortraitUrl, img_Boss_Head);
        Common.ImageBind(PlayerRole.HeadPortraitUrl, img_Player_Head);
        #region 玩家卡池
        var cardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentCardPoolsFileName);
        if (cardPools?.Count > 0)
        {
            txt_CardPoolsCount.text = cardPools?.Count.ToString();
        }
        else
        {
            cardPools = new List<CurrentCardPoolModel>();
            var GlobalCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName) ?? new List<CurrentCardPoolModel>();//全局卡池
            if (!string.IsNullOrEmpty(PlayerRole.CardListStr))
            {
                var arr = PlayerRole.CardListStr.Split(';');
                for (int i = 0; i < arr.Length; i++)
                {
                    CurrentCardPoolModel cardModel = new CurrentCardPoolModel();
                    var id = arr[i].Split('|')[0].ToString().Trim();
                    if (!string.IsNullOrEmpty(id))
                    {
                        cardModel = GlobalCardPools?.Find(a => a.ID == System.Convert.ToInt32(id));
                        cardPools.Add(cardModel);
                    }
                }
                Common.SaveTxtFile(cardPools.ListToJson(), GlobalAttr.CurrentCardPoolsFileName);
                txt_CardPoolsCount.text = cardPools?.Count.ToString();
            }
            else
            {
                txt_CardPoolsCount.text = "0";
            }
        }
        #endregion
        #endregion

        #region 加载TopBar预制件

        //加载TopBar预制件
        if (tempBar == null)
        {
            GameObject topBar = ResourcesManager.instance.Load("TopBar") as GameObject;
            topBar = Common.AddChild(UI_Obj.transform, topBar);
            topBar.name = "TopBar";
        }
        #endregion
        btn_Setting = transform.Find("UI/TopBar/Setting")?.GetComponent<Button>();
        if (btn_Setting != null)
        {
            btn_Setting.onClick.AddListener(SettingClick);
        }
        MapInit();
    } 
    #endregion

    #region 地图事件

    /// <summary>
    /// 地图初始化
    /// </summary>
    void MapInit()
    {
        listCombatPoint = Common.GetTxtFileToList<MapCombatPoint>(GlobalAttr.CurrentMapCombatPointFileName, "Map");
        listPath = Common.GetTxtFileToList<MapPath>(GlobalAttr.CurrentMapPathFileName, "Map");
        mapLocation = Common.GetTxtFileToModel<CurrentMapLocation>(GlobalAttr.CurrentMapLocationFileName, "Map");

        if (listCombatPoint?.Count > 0)
        {
            int BGcount = img_map.transform.childCount;
            for (int i = 0; i < BGcount; i++)
            {
                DestroyImmediate(img_map.transform.GetChild(0).gameObject);
            }
            //读取地图
            ReadMap();
            ReadPath();
            //头像绑定
            var map_Player_Head = transform.Find($"BG/Map_Row{mapLocation.Row}").gameObject;
            for (int i = 0; i < map_Player_Head?.transform.childCount; i++)
            {
                if (i == mapLocation.Column)
                {
                    var child = map_Player_Head.transform.GetChild(mapLocation.Column).GetComponent<Image>();
                    Common.ImageBind(PlayerRole.HeadPortraitUrl, child);
                }
            }
            //地图初始位置
            img_map.transform.position = new Vector3(InitPos.x, InitPos.y - (mapLocation.Row) * 240, 0);
        }
        else
        {
            int BGcount = img_map.transform.childCount;
            if (BGcount > 0)
            {
                for (int i = 0; i < BGcount; i++)
                {
                    if (img_map.transform.GetChild(0).gameObject.name == "Map_Row0")
                    {
                        img_map.transform.GetChild(0).gameObject.transform.SetAsLastSibling();
                    }
                    else
                    {
                        DestroyImmediate(img_map.transform.GetChild(0).gameObject);
                    }
                }
            }
            listCombatPoint = new List<MapCombatPoint>();
            listPath = new List<MapPath>();
            mapLocation = new CurrentMapLocation();//生成地图
            mapLocation.Column = 0;
            mapLocation.Row = 0;
            mapLocation.CurrentImgUrl = $"Images/Map_Atk{Random.Range(0, 4)}";
            Common.SaveTxtFile(mapLocation.ObjectToJson(), GlobalAttr.CurrentMapLocationFileName, "Map");
            listCombatPoint.Add(new MapCombatPoint
            {
                Row = 0,
                Column = 0,
                Name = "Atk_img0",
                Url = mapLocation.CurrentImgUrl,
                Type = 1
            });
            Common.ImageBind(PlayerRole.HeadPortraitUrl, img_Init);
            CreateMapState = true;
            MapRow = 1;
            ListPreviousRow = new List<int>() { 1 };
        }
    }

    #region 地图读取

    /// <summary>
    /// 读取地图
    /// </summary>
    public void ReadMap()
    {
        var listRow = listCombatPoint.GroupBy(a => a.Row).ToList();
        foreach (var item in listRow)
        {
            if (item.Key == 13)
            {
                //图片生成
                GameObject Atkimg = ResourcesManager.instance.Load("Map_Atk_img") as GameObject;
                Atkimg = Common.AddChild(img_map.transform, Atkimg);
                var column = item.ToList()[0];
                Atkimg.name = column.Name;
                Atkimg.transform.localPosition = new Vector2(0, OneRowY + item.Key * 240 + 70);
                var tempImg = img_map.transform.Find(column.Name).GetComponent<Image>();
                var rect = img_map.transform.Find(column.Name).GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(500, 250);
                Common.ImageBind(column.Url, tempImg);

                #region 添加头像
                GameObject img_head = ResourcesManager.instance.Load("Map_Head_Box") as GameObject;
                img_head = Common.AddChild(Atkimg.transform, img_head);
                img_head.name = "Map_Head_Box";
                var tempHead = Atkimg.transform.Find($"Map_Head_Box/img_Round/img_Head").GetComponent<Image>();

                Common.ImageBind(CurrentAiModel.HeadPortraitUrl, tempHead);
                #endregion

                #region 点击事件
                var btn_Head = img_head.GetComponent<Button>();
                EventTrigger trigger2 = Atkimg.GetComponent<EventTrigger>();
                if (trigger2 == null)
                {
                    trigger2 = Atkimg.AddComponent<EventTrigger>();
                }
                EventTrigger.Entry entry2 = new EventTrigger.Entry();
                int currentRow = item.Key;
                entry2.callback.AddListener(delegate { MapAtkImgClick(column.Type, 0, Atkimg, column.Row, column.Url); });
                trigger2.triggers.Add(entry2);
                #endregion
            }
            else
            {
                GameObject tempObject = ResourcesManager.instance.Load("Map_Row") as GameObject;
                tempObject = Common.AddChild(img_map.transform, tempObject);
                tempObject.name = "Map_Row" + item.Key;
                tempObject.transform.localPosition = new Vector2(0, OneRowY + item.Key * 240);
                var listColumn = item.ToList();
                foreach (var column in listColumn)
                {
                    //图片生成
                    GameObject Atkimg = ResourcesManager.instance.Load("Map_Atk_img") as GameObject;
                    Atkimg = Common.AddChild(tempObject.transform, Atkimg);
                    Atkimg.name = column.Name;
                    var tempImg = tempObject.transform.Find(column.Name).GetComponent<Image>();
                    Common.ImageBind(column.Url, tempImg);
                    #region 点击事件
                    EventTrigger trigger2 = Atkimg.GetComponent<EventTrigger>();
                    if (trigger2 == null)
                    {
                        trigger2 = Atkimg.AddComponent<EventTrigger>();
                    }
                    EventTrigger.Entry entry2 = new EventTrigger.Entry();
                    entry2.callback.AddListener(delegate { MapAtkImgClick(column.Type, column.Type == 1 ? System.Convert.ToInt32(column.Name.Substring(7)) : 0, Atkimg, column.Row, column.Url); });
                    trigger2.triggers.Add(entry2);
                    #endregion
                }
            }
        }
    }

    /// <summary>
    /// 路线读取
    /// </summary>
    public void ReadPath()
    {
        foreach (var row in listPath)
        {
            GameObject tempObject = ResourcesManager.instance.Load("Path_NullObj") as GameObject;
            tempObject = Common.AddChild(img_map.transform, tempObject);
            tempObject.name = $"Path_{row.Row}";
            tempObject.transform.localPosition = new Vector2(0, OnePathY + (row.Row - 1) * 240);
            tempObject.transform.SetAsFirstSibling();

            //row.Row _初始位置_线对应的模块
            if (row.PreviousRow == 1)
            {
                if (row.CurrentRow == 2)
                {
                    var obj1 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj1.transform.localPosition = new Vector2(-62, 0);
                    obj1.transform.name = $"img_{row.Row}_0_0";
                    obj2.transform.localPosition = new Vector2(62, 0);
                    obj2.transform.name = $"img_{row.Row}_0_1";
                }
                if (row.CurrentRow == 3)
                {
                    var obj1 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj1.transform.localPosition = new Vector2(-124, -24);
                    obj1.transform.name = $"img_{row.Row}_0_0";
                    obj2.transform.localPosition = new Vector2(0, -6);
                    obj2.transform.name = $"img_{row.Row}_0_1";
                    obj3.transform.localPosition = new Vector2(124, -24);
                    obj3.transform.name = $"img_{row.Row}_0_2";
                }
                if (row.CurrentRow == 4)
                {
                    var obj1 = ResourcesManager.instance.Load("Path_img_3") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                    var obj4 = ResourcesManager.instance.Load("Path_img_3_y") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj4 = Common.AddChild(tempObject.transform, obj4);
                    obj1.transform.localPosition = new Vector2(-186, -24);
                    obj2.transform.localPosition = new Vector2(-62, 0);
                    obj3.transform.localPosition = new Vector2(62, 0);
                    obj4.transform.localPosition = new Vector2(186, -24);
                    obj1.transform.name = $"img_{row.Row}_0_0";
                    obj2.transform.name = $"img_{row.Row}_0_1";
                    obj3.transform.name = $"img_{row.Row}_0_2";
                    obj4.transform.name = $"img_{row.Row}_0_3";
                }
            }
            else if (row.PreviousRow == 2)
            {
                if (row.CurrentRow == 1)
                {
                    var obj1 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_1_xy") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj1.transform.localPosition = new Vector2(-62, 0);
                    obj2.transform.localPosition = new Vector2(62, 0);
                    obj1.transform.name = $"img_{row.Row}_0_0";
                    obj2.transform.name = $"img_{row.Row}_1_0";
                }
                if (row.CurrentRow == 2)
                {
                    var obj1 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_0_y") as GameObject;
                    var random = row.RandomNum;
                    var obj3 = ResourcesManager.instance.Load($"Path_img_2{(random == 0 ? "_y" : "")}") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj1.transform.localPosition = new Vector2(-100, 0);
                    obj2.transform.localPosition = new Vector2(100, 0);
                    obj3.transform.localPosition = new Vector2(0, 0);
                    obj1.transform.name = $"img_{row.Row}_0_0";
                    obj2.transform.name = $"img_{row.Row}_1_1";
                    obj3.transform.name = $"img_{row.Row}_{(random == 0 ? "1" : "0")}_{random}";
                }
                if (row.CurrentRow == 3)
                {
                    var random = row.RandomNum;//1不带y，0带y
                    var obj1 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                    var obj2 = ResourcesManager.instance.Load($"Path_img_1{(random == 0 ? "_y" : "")}") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj1.transform.localPosition = new Vector2(-176, 0);
                    obj2.transform.localPosition = new Vector2(random == 0 ? -40 : 40, 0);
                    obj3.transform.localPosition = new Vector2(176, 0);
                    obj1.transform.name = $"img_{row.Row}_0_0";
                    obj3.transform.name = $"img_{row.Row}_1_2";
                    obj2.transform.name = $"img_{row.Row}_{random}_1";
                }
                if (row.CurrentRow == 4)
                {
                    int random = row.RandomNum;
                    if (random == 0)
                    {
                        var obj1 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                        var obj2 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                        var obj3 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                        var obj4 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                        obj1 = Common.AddChild(tempObject.transform, obj1);
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj3 = Common.AddChild(tempObject.transform, obj3);
                        obj4 = Common.AddChild(tempObject.transform, obj4);
                        obj1.transform.localPosition = new Vector2(-230, -5);
                        obj2.transform.localPosition = new Vector2(-105, -5);
                        obj3.transform.localPosition = new Vector2(0, -5);
                        obj4.transform.localPosition = new Vector2(230, -5);
                        obj1.transform.name = $"img_{row.Row}_0_0";
                        obj2.transform.name = $"img_{row.Row}_0_1";
                        obj3.transform.name = $"img_{row.Row}_0_2";
                        obj4.transform.name = $"img_{row.Row}_1_3";
                    }
                    else if (random == 1)
                    {
                        var obj1 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                        var obj2 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                        var obj3 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                        var obj4 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                        obj1 = Common.AddChild(tempObject.transform, obj1);
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj3 = Common.AddChild(tempObject.transform, obj3);
                        obj4 = Common.AddChild(tempObject.transform, obj4);
                        obj1.transform.localPosition = new Vector2(-230, -5);
                        obj2.transform.localPosition = new Vector2(0, -5);
                        obj3.transform.localPosition = new Vector2(120, -5);
                        obj4.transform.localPosition = new Vector2(230, -5);
                        obj1.transform.name = $"img_{row.Row}_0_0";
                        obj2.transform.name = $"img_{row.Row}_1_1";
                        obj3.transform.name = $"img_{row.Row}_1_2";
                        obj4.transform.name = $"img_{row.Row}_1_3";
                    }
                    else if (random == 2)
                    {
                        var obj1 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                        var obj2 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                        var obj3 = ResourcesManager.instance.Load("Path_img_0_y") as GameObject;
                        var obj4 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                        obj1 = Common.AddChild(tempObject.transform, obj1);
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj3 = Common.AddChild(tempObject.transform, obj3);
                        obj4 = Common.AddChild(tempObject.transform, obj4);
                        obj1.transform.localPosition = new Vector2(-230, -5);
                        obj2.transform.localPosition = new Vector2(-105, -5);
                        obj3.transform.localPosition = new Vector2(105, -5);
                        obj4.transform.localPosition = new Vector2(230, -5);
                        obj1.transform.name = $"img_{row.Row}_0_0";
                        obj2.transform.name = $"img_{row.Row}_0_1";
                        obj3.transform.name = $"img_{row.Row}_1_2";
                        obj4.transform.name = $"img_{row.Row}_1_3";
                    }
                }
            }
            else if (row.PreviousRow == 3)
            {
                if (row.CurrentRow == 1)
                {
                    var obj1 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj1.transform.localPosition = new Vector2(-133, 14);
                    obj2.transform.localPosition = new Vector2(18, 0);
                    obj3.transform.localPosition = new Vector2(133, 14);
                    obj1.transform.name = $"img_{row.Row}_0_0";
                    obj2.transform.name = $"img_{row.Row}_1_0";
                    obj3.transform.name = $"img_{row.Row}_2_0";
                }
                if (row.CurrentRow == 2)
                {
                    var random = row.RandomNum;
                    var obj1 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                    var obj4 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                    switch (random)
                    {
                        case 1:
                            obj2 = Common.AddChild(tempObject.transform, obj2);
                            obj2.transform.localPosition = new Vector2(-45, 6);
                            obj2.transform.name = $"img_{row.Row}_1_0";
                            break;
                        case 2:
                            obj3 = Common.AddChild(tempObject.transform, obj3);
                            obj3.transform.localPosition = new Vector2(66, 6);
                            obj3.transform.name = $"img_{row.Row}_1_1";
                            break;
                        default:
                            obj2 = Common.AddChild(tempObject.transform, obj2);
                            obj3 = Common.AddChild(tempObject.transform, obj3);
                            obj2.transform.localPosition = new Vector2(-45, 6);
                            obj3.transform.localPosition = new Vector2(66, 6);
                            obj2.transform.name = $"img_{row.Row}_1_0";
                            obj3.transform.name = $"img_{row.Row}_1_1";
                            break;
                    }
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj4 = Common.AddChild(tempObject.transform, obj4);
                    obj1.transform.localPosition = new Vector2(-156, 6);
                    obj4.transform.localPosition = new Vector2(174, 6);
                    obj1.transform.name = $"img_{row.Row}_0_0";
                    obj4.transform.name = $"img_{row.Row}_2_1";
                }
                if (row.CurrentRow == 3)
                {
                    var random = row.RandomNum;
                    var obj2 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                    var obj1 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                    var obj4 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                    switch (random)
                    {
                        case 0:
                            obj3 = Common.AddChild(tempObject.transform, obj3);
                            obj3.transform.localPosition = new Vector2(107, 0);
                            obj3.transform.name = $"img_{row.Row}_1_2";
                            obj2 = Common.AddChild(tempObject.transform, obj2);
                            obj2.transform.localPosition = new Vector2(-105, 0);
                            obj2.transform.name = $"img_{row.Row}_0_1";
                            break;
                        case 1:
                            obj2 = Common.AddChild(tempObject.transform, obj2);
                            obj2.transform.localPosition = new Vector2(-105, 0);
                            obj2.transform.name = $"img_{row.Row}_0_1";

                            var obj6 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                            obj6 = Common.AddChild(tempObject.transform, obj6);
                            obj6.transform.localPosition = new Vector2(20, 0);
                            obj6.transform.name = $"img_{row.Row}_1_1";
                            break;
                        case 2:
                            var obj5 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                            obj5 = Common.AddChild(tempObject.transform, obj5);
                            obj5.transform.localPosition = new Vector2(20, 0);
                            obj5.transform.name = $"img_{row.Row}_1_1";
                            obj2 = Common.AddChild(tempObject.transform, obj2);
                            obj2.transform.localPosition = new Vector2(-105, 0);
                            obj2.transform.name = $"img_{row.Row}_0_1";
                            obj3 = Common.AddChild(tempObject.transform, obj3);
                            obj3.transform.localPosition = new Vector2(107, 0);
                            obj3.transform.name = $"img_{row.Row}_1_2";
                            break;
                        default:
                            obj2 = Common.AddChild(tempObject.transform, obj2);
                            obj2.transform.localPosition = new Vector2(-105, 0);
                            obj2.transform.name = $"img_{row.Row}_0_1";
                            obj3 = Common.AddChild(tempObject.transform, obj3);
                            obj3.transform.localPosition = new Vector2(107, 0);
                            obj3.transform.name = $"img_{row.Row}_1_2";
                            break;
                    }
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj4 = Common.AddChild(tempObject.transform, obj4);
                    obj1.transform.localPosition = new Vector2(-214, 0);
                    obj4.transform.localPosition = new Vector2(237, 0);
                    obj1.transform.name = $"img_{row.Row}_0_0";
                    obj4.transform.name = $"img_{row.Row}_2_2";
                }
                if (row.CurrentRow == 4)
                {
                    int random = row.RandomNum;
                    if (random == 0)
                    {
                        var obj1 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                        var obj2 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                        var obj3 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                        var obj4 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                        obj1 = Common.AddChild(tempObject.transform, obj1);
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj3 = Common.AddChild(tempObject.transform, obj3);
                        obj4 = Common.AddChild(tempObject.transform, obj4);
                        obj1.transform.localPosition = new Vector2(-285, 0);
                        obj2.transform.localPosition = new Vector2(-143, 0);
                        obj3.transform.localPosition = new Vector2(63, 0);
                        obj4.transform.localPosition = new Vector2(280, 0);
                        obj1.transform.name = $"img_{row.Row}_0_0";
                        obj2.transform.name = $"img_{row.Row}_0_1";
                        obj3.transform.name = $"img_{row.Row}_1_2";
                        obj4.transform.name = $"img_{row.Row}_2_3";
                    }
                    else if (random == 1)
                    {
                        var obj1 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                        var obj2 = ResourcesManager.instance.Load("Path_img_1_xy") as GameObject;
                        var obj3 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                        var obj4 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                        obj1 = Common.AddChild(tempObject.transform, obj1);
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj3 = Common.AddChild(tempObject.transform, obj3);
                        obj4 = Common.AddChild(tempObject.transform, obj4);
                        obj1.transform.localPosition = new Vector2(-285, 0);
                        obj2.transform.localPosition = new Vector2(-63, 0);
                        obj3.transform.localPosition = new Vector2(63, 0);
                        obj4.transform.localPosition = new Vector2(280, 0);
                        obj1.transform.name = $"img_{row.Row}_0_0";
                        obj2.transform.name = $"img_{row.Row}_1_1";
                        obj3.transform.name = $"img_{row.Row}_1_2";
                        obj4.transform.name = $"img_{row.Row}_2_3";
                    }
                    else if (random == 2)
                    {
                        var obj1 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                        var obj2 = ResourcesManager.instance.Load("Path_img_1_xy") as GameObject;
                        var obj3 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                        var obj4 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                        var obj5 = ResourcesManager.instance.Load("Path_img_1_xy") as GameObject;
                        obj1 = Common.AddChild(tempObject.transform, obj1);
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj3 = Common.AddChild(tempObject.transform, obj3);
                        obj4 = Common.AddChild(tempObject.transform, obj4);
                        obj5 = Common.AddChild(tempObject.transform, obj5);
                        obj1.transform.localPosition = new Vector2(-285, 0);
                        obj2.transform.localPosition = new Vector2(-63, 0);
                        obj3.transform.localPosition = new Vector2(63, 0);
                        obj4.transform.localPosition = new Vector2(280, 0);
                        obj5.transform.localPosition = new Vector2(175, 0);
                        obj1.transform.name = $"img_{row.Row}_0_0";
                        obj2.transform.name = $"img_{row.Row}_1_1";
                        obj3.transform.name = $"img_{row.Row}_1_2";
                        obj4.transform.name = $"img_{row.Row}_2_3";
                        obj5.transform.name = $"img_{row.Row}_2_2";
                    }
                }
            }
            else if (row.PreviousRow == 4)
            {
                if (row.CurrentRow == 1)
                {
                    var obj1 = ResourcesManager.instance.Load("Path_img_3_y") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                    var obj4 = ResourcesManager.instance.Load("Path_img_3") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj4 = Common.AddChild(tempObject.transform, obj4);
                    obj1.transform.localPosition = new Vector2(-205, -6);
                    obj2.transform.localPosition = new Vector2(-65, 0);
                    obj3.transform.localPosition = new Vector2(47, 14);
                    obj4.transform.localPosition = new Vector2(219, -6);
                    obj1.transform.name = $"img_{row.Row}_0_0";
                    obj2.transform.name = $"img_{row.Row}_1_0";
                    obj3.transform.name = $"img_{row.Row}_2_0";
                    obj4.transform.name = $"img_{row.Row}_3_0";
                }
                if (row.CurrentRow == 2)
                {
                    var random = row.RandomNum;
                    var obj1 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                    var obj4 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                    var obj5 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                    var obj6 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                    switch (random)
                    {
                        case 0:
                            obj2 = Common.AddChild(tempObject.transform, obj2);
                            obj3 = Common.AddChild(tempObject.transform, obj3);
                            obj2.transform.localPosition = new Vector2(-100, 0);
                            obj3.transform.localPosition = new Vector2(0, 0);
                            obj2.transform.name = $"img_{row.Row}_1_0";
                            obj3.transform.name = $"img_{row.Row}_2_0";
                            break;
                        case 1:
                            obj4 = Common.AddChild(tempObject.transform, obj4);
                            obj5 = Common.AddChild(tempObject.transform, obj5);
                            obj4.transform.localPosition = new Vector2(0, 0);
                            obj5.transform.localPosition = new Vector2(124, 0);
                            obj4.transform.name = $"img_{row.Row}_1_1";
                            obj5.transform.name = $"img_{row.Row}_2_1";
                            break;
                        case 2:
                            obj2 = Common.AddChild(tempObject.transform, obj2);
                            obj3 = Common.AddChild(tempObject.transform, obj3);
                            obj5 = Common.AddChild(tempObject.transform, obj5);
                            obj2.transform.localPosition = new Vector2(-100, 0);
                            obj3.transform.localPosition = new Vector2(0, 0);
                            obj5.transform.localPosition = new Vector2(124, 0);
                            obj2.transform.name = $"img_{row.Row}_1_0";
                            obj3.transform.name = $"img_{row.Row}_2_0";
                            obj5.transform.name = $"img_{row.Row}_2_1";
                            break;
                        case 3:
                            obj2 = Common.AddChild(tempObject.transform, obj2);
                            obj4 = Common.AddChild(tempObject.transform, obj4);
                            obj5 = Common.AddChild(tempObject.transform, obj5);
                            obj2.transform.localPosition = new Vector2(-100, 0);
                            obj4.transform.localPosition = new Vector2(0, 0);
                            obj5.transform.localPosition = new Vector2(124, 0);
                            obj2.transform.name = $"img_{row.Row}_1_0";
                            obj4.transform.name = $"img_{row.Row}_1_1";
                            obj5.transform.name = $"img_{row.Row}_2_1";
                            break;
                    }
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj6 = Common.AddChild(tempObject.transform, obj6);
                    obj1.transform.localPosition = new Vector2(-222, 0);
                    obj6.transform.localPosition = new Vector2(222, 0);
                    obj1.transform.name = $"img_{row.Row}_0_0";
                    obj6.transform.name = $"img_{row.Row}_3_1";
                }
                if (row.CurrentRow == 3)
                {
                    var random = row.RandomNum;
                    var obj1 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                    var obj4 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                    var obj5 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                    if (random == 1)
                    {
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj2.transform.localPosition = new Vector2(-162, 10);
                        obj2.transform.name = $"img_{row.Row}_1_0";
                    }
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj4 = Common.AddChild(tempObject.transform, obj4);
                    obj5 = Common.AddChild(tempObject.transform, obj5);
                    obj1.transform.localPosition = new Vector2(-274, 10);
                    obj3.transform.localPosition = new Vector2(-40, 10);
                    obj4.transform.localPosition = new Vector2(79, 10);
                    obj5.transform.localPosition = new Vector2(259, 10);
                    obj1.transform.name = $"img_{row.Row}_0_0";
                    obj3.transform.name = $"img_{row.Row}_1_1";
                    obj4.transform.name = $"img_{row.Row}_2_1";
                    obj5.transform.name = $"img_{row.Row}_3_2";
                }
                if (row.CurrentRow == 4)
                {
                    int random = row.RandomNum;
                    var obj1 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                    var obj2 = ResourcesManager.instance.Load($"Path_img_2{(random == 0 ? "_y" : "")}") as GameObject;
                    var obj3 = ResourcesManager.instance.Load($"Path_img_2{(random == 0 ? "_y" : "")}") as GameObject;
                    var obj4 = ResourcesManager.instance.Load($"Path_img_2{(random == 0 ? "_y" : "")}") as GameObject;
                    var obj5 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj4 = Common.AddChild(tempObject.transform, obj4);
                    obj5 = Common.AddChild(tempObject.transform, obj5);
                    obj1.transform.localPosition = new Vector2(-330, 0);
                    obj2.transform.localPosition = new Vector2(-210, 9);
                    obj3.transform.localPosition = new Vector2(0, 9);
                    obj4.transform.localPosition = new Vector2(222, 9);
                    obj5.transform.localPosition = new Vector2(340, 0);
                    obj1.transform.name = $"img_{row.Row}_0_0";
                    if (random == 1)
                    {
                        obj2.transform.name = $"img_{row.Row}_0_1";
                        obj3.transform.name = $"img_{row.Row}_1_2";
                        obj4.transform.name = $"img_{row.Row}_2_3";
                    }
                    else if (random == 0)
                    {
                        obj2.transform.name = $"img_{row.Row}_1_0";
                        obj3.transform.name = $"img_{row.Row}_2_1";
                        obj4.transform.name = $"img_{row.Row}_3_2";
                    }
                    obj5.transform.name = $"img_{row.Row}_3_3";
                }
            }
        }
    }

    #endregion


    #region 地图生成
    /// <summary>
    /// 生成地图
    /// </summary>
    /// <param name="type">0普通地图，99boss,1商店</param>
    public void CreateMap(int type)
    {
        //地图长度13*220
        //宽度最大4，最小2
        //商店2-4,商店在3-5行遇到
        //奇遇2-4
        //一行一行进行渲染

        #region 普通地图
        if (type == 0)
        {
            //4-6必出商店
            GameObject tempObject = ResourcesManager.instance.Load("Map_Row") as GameObject;
            tempObject = Common.AddChild(img_map.transform, tempObject);
            tempObject.name = "Map_Row" + MapRow;
            tempObject.transform.localPosition = new Vector2(0, OneRowY + MapRow * 240);
            //当前行战斗数量
            int rowCount = Random.Range(2, 5);
            for (int i = 0; i < rowCount; i++)
            {
                int atkType = Random.Range(1, 11);
                string tempImgName = "";
                string tempImgUrl = "";
                int tempType = 0;
                if (MapRow == 1)
                {
                    atkType = 1;
                }
                else if (MapRow > 4 && MapRow < 6)
                {
                    atkType = Random.Range(6, 16);
                }
                else if (MapRow > 4 && MapRow < 11)
                {
                    atkType = Random.Range(1, 12);
                }
                //普通地图
                if (atkType < 8)
                {
                    //图片生成
                    int imgIndex = Random.Range(0, 4);
                    GameObject Atkimg = ResourcesManager.instance.Load("Map_Atk_img") as GameObject;
                    Atkimg = Common.AddChild(tempObject.transform, Atkimg);
                    tempImgName = "Atk_img" + i;
                    Atkimg.name = tempImgName;
                    tempImgUrl = "Images/Map_Atk" + imgIndex;
                    var tempImg = tempObject.transform.Find(tempImgName).GetComponent<Image>();
                    Common.ImageBind(tempImgUrl, tempImg);
                    #region 点击事件
                    EventTrigger trigger2 = Atkimg.GetComponent<EventTrigger>();
                    if (trigger2 == null)
                    {
                        trigger2 = Atkimg.AddComponent<EventTrigger>();
                    }
                    EventTrigger.Entry entry2 = new EventTrigger.Entry();
                    int currentRow = MapRow;
                    tempType = 1;
                    entry2.callback.AddListener(delegate { MapAtkImgClick(tempType, imgIndex, Atkimg, currentRow, tempImgUrl); });
                    trigger2.triggers.Add(entry2);
                    #endregion
                }
                //秘境
                else if (atkType < 11)
                {
                    //图片生成
                    int imgIndex = Random.Range(0, 2);
                    GameObject Atkimg = ResourcesManager.instance.Load("Map_Atk_img") as GameObject;
                    Atkimg = Common.AddChild(tempObject.transform, Atkimg);
                    tempImgName = "Adventure_img" + i;
                    Atkimg.name = tempImgName;
                    tempImgUrl = "Images/Map_Adventrue" + imgIndex;
                    var tempImg = tempObject.transform.Find(tempImgName).GetComponent<Image>();
                    Common.ImageBind(tempImgUrl, tempImg);
                    #region 点击事件
                    EventTrigger trigger2 = Atkimg.GetComponent<EventTrigger>();
                    if (trigger2 == null)
                    {
                        trigger2 = Atkimg.AddComponent<EventTrigger>();
                    }
                    EventTrigger.Entry entry2 = new EventTrigger.Entry();
                    int currentRow = MapRow;
                    tempType = 4;
                    entry2.callback.AddListener(delegate { MapAtkImgClick(tempType, imgIndex, Atkimg, currentRow, tempImgUrl); });
                    trigger2.triggers.Add(entry2);
                    #endregion
                }
                //商城
                else
                {
                    //图片生成
                    GameObject Atkimg = ResourcesManager.instance.Load("Map_Atk_img") as GameObject;
                    Atkimg = Common.AddChild(tempObject.transform, Atkimg);
                    tempImgName = "Atk_imgShop" + i;
                    Atkimg.name = tempImgName;
                    tempImgUrl = "Images/Map_Shop";
                    var tempImg = tempObject.transform.Find(tempImgName).GetComponent<Image>();
                    Common.ImageBind(tempImgUrl, tempImg);
                    #region 点击事件
                    EventTrigger trigger2 = Atkimg.GetComponent<EventTrigger>();
                    if (trigger2 == null)
                    {
                        trigger2 = Atkimg.AddComponent<EventTrigger>();
                    }
                    EventTrigger.Entry entry2 = new EventTrigger.Entry();
                    int currentRow = MapRow;
                    tempType = 2;
                    entry2.callback.AddListener(delegate { MapAtkImgClick(tempType, 0, Atkimg, currentRow, tempImgUrl); });
                    trigger2.triggers.Add(entry2);
                    #endregion
                }
                listCombatPoint.Add(new MapCombatPoint
                {
                    Row = MapRow,
                    Column = i,
                    Name = tempImgName,
                    Type = tempType,
                    Url = tempImgUrl
                });
            }
            ListPreviousRow.Add(rowCount);
            CreatePath(ListPreviousRow[MapRow - 1], rowCount);
            MapRow++;
        }
        #endregion
        #region Boss
        else if (type == 99)
        {
            //图片生成
            GameObject Atkimg = ResourcesManager.instance.Load("Map_Atk_img") as GameObject;
            Atkimg = Common.AddChild(img_map.transform, Atkimg);
            Atkimg.name = "Atk_imgBoss";
            Atkimg.transform.localPosition = new Vector2(0, OneRowY + MapRow * 240 + 70);
            var tempImg = img_map.transform.Find($"Atk_imgBoss").GetComponent<Image>();
            var rect = img_map.transform.Find($"Atk_imgBoss").GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(500, 250);
            Common.ImageBind("Images/Map_AtkBoss", tempImg);
            CreatePath(ListPreviousRow[MapRow - 1], 1);

            #region 添加头像
            GameObject img_head = ResourcesManager.instance.Load("Map_Head_Box") as GameObject;
            img_head = Common.AddChild(Atkimg.transform, img_head);
            img_head.name = "Map_Head_Box";
            var tempHead = Atkimg.transform.Find($"Map_Head_Box/img_Round/img_Head").GetComponent<Image>();

            Common.ImageBind(CurrentAiModel.HeadPortraitUrl, tempHead);
            #endregion

            #region 点击事件
            EventTrigger trigger2 = Atkimg.GetComponent<EventTrigger>();
            if (trigger2 == null)
            {
                trigger2 = Atkimg.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            int currentRow = MapRow;
            entry2.callback.AddListener(delegate { MapAtkImgClick(3, 0, Atkimg, currentRow, "Images/Map_AtkBoss"); });
            trigger2.triggers.Add(entry2);
            #endregion

            listCombatPoint.Add(new MapCombatPoint
            {
                Row = MapRow,
                Column = 0,
                Name = "Atk_imgBoss",
                Type = 3,
                Url = "Images/Map_AtkBoss"
            });
            MapRow++;
            Common.SaveTxtFile(listCombatPoint.ListToJson(), GlobalAttr.CurrentMapCombatPointFileName, "Map");
        }
        #endregion
        #region 商店
        else if (type == 1)
        {
            GameObject tempObject = ResourcesManager.instance.Load("Map_Row") as GameObject;
            tempObject = Common.AddChild(img_map.transform, tempObject);
            tempObject.name = "Map_Row" + MapRow; ;
            tempObject.transform.localPosition = new Vector2(0, OneRowY + MapRow * 240);
            //当前行战斗数量
            int rowCount = Random.Range(2, 5);
            for (int i = 0; i < rowCount; i++)
            {
                //图片生成
                GameObject Atkimg = ResourcesManager.instance.Load("Map_Atk_img") as GameObject;
                Atkimg = Common.AddChild(tempObject.transform, Atkimg);
                Atkimg.name = "Atk_imgShop" + i;
                var tempImg = tempObject.transform.Find($"Atk_imgShop{i}").GetComponent<Image>();
                Common.ImageBind("Images/Map_Shop", tempImg);

                #region 点击事件
                EventTrigger trigger2 = Atkimg.GetComponent<EventTrigger>();
                if (trigger2 == null)
                {
                    trigger2 = Atkimg.AddComponent<EventTrigger>();
                }
                EventTrigger.Entry entry2 = new EventTrigger.Entry();
                int currentRow = MapRow;
                entry2.callback.AddListener(delegate { MapAtkImgClick(2, 0, Atkimg, currentRow, "Images/Map_Shop"); });
                trigger2.triggers.Add(entry2);
                #endregion

                listCombatPoint.Add(new MapCombatPoint
                {
                    Row = MapRow,
                    Column = i,
                    Name = "Atk_imgShop" + i,
                    Type = 2,
                    Url = "Images/Map_Shop"
                });
            }
            ListPreviousRow.Add(rowCount);
            CreatePath(ListPreviousRow[MapRow - 1], rowCount);

            MapRow++;
        }
        #endregion
    }

    /// <summary>
    /// 创建路线
    /// </summary>
    /// <param name="PreviousRow">上一行数量</param>
    /// <param name="CurrentRow">当前行数量</param>
    public void CreatePath(int PreviousRow, int CurrentRow)
    {
        //1-4、1-3、1-2
        //2-4、2-3、2-2
        //3-4、3-3、3-2
        //4-4、4-3、4-2
        GameObject tempObject = ResourcesManager.instance.Load("Path_NullObj") as GameObject;
        tempObject = Common.AddChild(img_map.transform, tempObject);
        tempObject.name = $"Path_{MapRow}";
        tempObject.transform.localPosition = new Vector2(0, OnePathY + (MapRow - 1) * 240);
        tempObject.transform.SetAsFirstSibling();

        //MapRow _初始位置_线对应的模块
        int tempRandom = -1;
        if (PreviousRow == 1)
        {
            if (CurrentRow == 2)
            {
                var obj1 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                var obj2 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                obj1 = Common.AddChild(tempObject.transform, obj1);
                obj2 = Common.AddChild(tempObject.transform, obj2);
                obj1.transform.localPosition = new Vector2(-62, 0);
                obj1.transform.name = $"img_{MapRow}_0_0";
                obj2.transform.localPosition = new Vector2(62, 0);
                obj2.transform.name = $"img_{MapRow}_0_1";
            }
            if (CurrentRow == 3)
            {
                var obj1 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                var obj2 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                var obj3 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                obj1 = Common.AddChild(tempObject.transform, obj1);
                obj2 = Common.AddChild(tempObject.transform, obj2);
                obj3 = Common.AddChild(tempObject.transform, obj3);
                obj1.transform.localPosition = new Vector2(-124, -24);
                obj1.transform.name = $"img_{MapRow}_0_0";
                obj2.transform.localPosition = new Vector2(0, -6);
                obj2.transform.name = $"img_{MapRow}_0_1";
                obj3.transform.localPosition = new Vector2(124, -24);
                obj3.transform.name = $"img_{MapRow}_0_2";
            }
            if (CurrentRow == 4)
            {
                var obj1 = ResourcesManager.instance.Load("Path_img_3") as GameObject;
                var obj2 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                var obj3 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                var obj4 = ResourcesManager.instance.Load("Path_img_3_y") as GameObject;
                obj1 = Common.AddChild(tempObject.transform, obj1);
                obj2 = Common.AddChild(tempObject.transform, obj2);
                obj3 = Common.AddChild(tempObject.transform, obj3);
                obj4 = Common.AddChild(tempObject.transform, obj4);
                obj1.transform.localPosition = new Vector2(-186, -24);
                obj2.transform.localPosition = new Vector2(-62, 0);
                obj3.transform.localPosition = new Vector2(62, 0);
                obj4.transform.localPosition = new Vector2(186, -24);
                obj1.transform.name = $"img_{MapRow}_0_0";
                obj2.transform.name = $"img_{MapRow}_0_1";
                obj3.transform.name = $"img_{MapRow}_0_2";
                obj4.transform.name = $"img_{MapRow}_0_3";
            }
        }
        else if (PreviousRow == 2)
        {
            if (CurrentRow == 1)
            {
                var obj1 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                var obj2 = ResourcesManager.instance.Load("Path_img_1_xy") as GameObject;
                obj1 = Common.AddChild(tempObject.transform, obj1);
                obj2 = Common.AddChild(tempObject.transform, obj2);
                obj1.transform.localPosition = new Vector2(-62, 0);
                obj2.transform.localPosition = new Vector2(62, 0);
                obj1.transform.name = $"img_{MapRow}_0_0";
                obj2.transform.name = $"img_{MapRow}_1_0";
            }
            if (CurrentRow == 2)
            {
                var obj1 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                var obj2 = ResourcesManager.instance.Load("Path_img_0_y") as GameObject;
                var random = Random.Range(0, 2);
                tempRandom = random;
                var obj3 = ResourcesManager.instance.Load($"Path_img_2{(random == 0 ? "_y" : "")}") as GameObject;
                obj1 = Common.AddChild(tempObject.transform, obj1);
                obj2 = Common.AddChild(tempObject.transform, obj2);
                obj3 = Common.AddChild(tempObject.transform, obj3);
                obj1.transform.localPosition = new Vector2(-100, 0);
                obj2.transform.localPosition = new Vector2(100, 0);
                obj3.transform.localPosition = new Vector2(0, 0);
                obj1.transform.name = $"img_{MapRow}_0_0";
                obj2.transform.name = $"img_{MapRow}_1_1";
                obj3.transform.name = $"img_{MapRow}_{(random == 0 ? "1" : "0")}_{random}";
            }
            if (CurrentRow == 3)
            {
                var random = Random.Range(0, 2);//1不带y，0带y
                tempRandom = random;
                var obj1 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                var obj2 = ResourcesManager.instance.Load($"Path_img_1{(random == 0 ? "_y" : "")}") as GameObject;
                var obj3 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                obj1 = Common.AddChild(tempObject.transform, obj1);
                obj2 = Common.AddChild(tempObject.transform, obj2);
                obj3 = Common.AddChild(tempObject.transform, obj3);
                obj1.transform.localPosition = new Vector2(-176, 0);
                obj2.transform.localPosition = new Vector2(random == 0 ? -40 : 40, 0);
                obj3.transform.localPosition = new Vector2(176, 0);
                obj1.transform.name = $"img_{MapRow}_0_0";
                obj3.transform.name = $"img_{MapRow}_1_2";
                obj2.transform.name = $"img_{MapRow}_{random}_1";
            }
            if (CurrentRow == 4)
            {
                int random = Random.Range(0, 3);
                tempRandom = random;
                if (random == 0)
                {
                    var obj1 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                    var obj4 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj4 = Common.AddChild(tempObject.transform, obj4);
                    obj1.transform.localPosition = new Vector2(-230, -5);
                    obj2.transform.localPosition = new Vector2(-105, -5);
                    obj3.transform.localPosition = new Vector2(0, -5);
                    obj4.transform.localPosition = new Vector2(230, -5);
                    obj1.transform.name = $"img_{MapRow}_0_0";
                    obj2.transform.name = $"img_{MapRow}_0_1";
                    obj3.transform.name = $"img_{MapRow}_0_2";
                    obj4.transform.name = $"img_{MapRow}_1_3";
                }
                else if (random == 1)
                {
                    var obj1 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                    var obj4 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj4 = Common.AddChild(tempObject.transform, obj4);
                    obj1.transform.localPosition = new Vector2(-230, -5);
                    obj2.transform.localPosition = new Vector2(0, -5);
                    obj3.transform.localPosition = new Vector2(120, -5);
                    obj4.transform.localPosition = new Vector2(230, -5);
                    obj1.transform.name = $"img_{MapRow}_0_0";
                    obj2.transform.name = $"img_{MapRow}_1_1";
                    obj3.transform.name = $"img_{MapRow}_1_2";
                    obj4.transform.name = $"img_{MapRow}_1_3";
                }
                else if (random == 2)
                {
                    var obj1 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_0_y") as GameObject;
                    var obj4 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj4 = Common.AddChild(tempObject.transform, obj4);
                    obj1.transform.localPosition = new Vector2(-230, -5);
                    obj2.transform.localPosition = new Vector2(-105, -5);
                    obj3.transform.localPosition = new Vector2(105, -5);
                    obj4.transform.localPosition = new Vector2(230, -5);
                    obj1.transform.name = $"img_{MapRow}_0_0";
                    obj2.transform.name = $"img_{MapRow}_0_1";
                    obj3.transform.name = $"img_{MapRow}_1_2";
                    obj4.transform.name = $"img_{MapRow}_1_3";
                }
            }
        }
        else if (PreviousRow == 3)
        {
            if (CurrentRow == 1)
            {
                var obj1 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                var obj2 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                var obj3 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                obj1 = Common.AddChild(tempObject.transform, obj1);
                obj2 = Common.AddChild(tempObject.transform, obj2);
                obj3 = Common.AddChild(tempObject.transform, obj3);
                obj1.transform.localPosition = new Vector2(-133, 14);
                obj2.transform.localPosition = new Vector2(18, 0);
                obj3.transform.localPosition = new Vector2(133, 14);
                obj1.transform.name = $"img_{MapRow}_0_0";
                obj2.transform.name = $"img_{MapRow}_1_0";
                obj3.transform.name = $"img_{MapRow}_2_0";
            }
            if (CurrentRow == 2)
            {
                var random = Random.Range(0, 3);
                tempRandom = random;
                var obj1 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                var obj2 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                var obj3 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                var obj4 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                switch (random)
                {
                    case 1:
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj2.transform.localPosition = new Vector2(-45, 6);
                        obj2.transform.name = $"img_{MapRow}_1_0";
                        break;
                    case 2:
                        obj3 = Common.AddChild(tempObject.transform, obj3);
                        obj3.transform.localPosition = new Vector2(66, 6);
                        obj3.transform.name = $"img_{MapRow}_1_1";
                        break;
                    default:
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj3 = Common.AddChild(tempObject.transform, obj3);
                        obj2.transform.localPosition = new Vector2(-45, 6);
                        obj3.transform.localPosition = new Vector2(66, 6);
                        obj2.transform.name = $"img_{MapRow}_1_0";
                        obj3.transform.name = $"img_{MapRow}_1_1";
                        break;
                }
                obj1 = Common.AddChild(tempObject.transform, obj1);
                obj4 = Common.AddChild(tempObject.transform, obj4);
                obj1.transform.localPosition = new Vector2(-156, 6);
                obj4.transform.localPosition = new Vector2(174, 6);
                obj1.transform.name = $"img_{MapRow}_0_0";
                obj4.transform.name = $"img_{MapRow}_2_1";
            }
            if (CurrentRow == 3)
            {
                var random = Random.Range(0, 3);
                tempRandom = random;
                var obj2 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                var obj3 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                var obj1 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                var obj4 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                switch (random)
                {
                    case 0:
                        obj3 = Common.AddChild(tempObject.transform, obj3);
                        obj3.transform.localPosition = new Vector2(107, 0);
                        obj3.transform.name = $"img_{MapRow}_1_2";
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj2.transform.localPosition = new Vector2(-105, 0);
                        obj2.transform.name = $"img_{MapRow}_0_1";
                        break;
                    case 1:
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj2.transform.localPosition = new Vector2(-105, 0);
                        obj2.transform.name = $"img_{MapRow}_0_1";

                        var obj6 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                        obj6 = Common.AddChild(tempObject.transform, obj6);
                        obj6.transform.localPosition = new Vector2(20, 0);
                        obj6.transform.name = $"img_{MapRow}_1_1";
                        break;
                    case 2:
                        var obj5 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                        obj5 = Common.AddChild(tempObject.transform, obj5);
                        obj5.transform.localPosition = new Vector2(20, 0);
                        obj5.transform.name = $"img_{MapRow}_1_1";
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj2.transform.localPosition = new Vector2(-105, 0);
                        obj2.transform.name = $"img_{MapRow}_0_1";
                        obj3 = Common.AddChild(tempObject.transform, obj3);
                        obj3.transform.localPosition = new Vector2(107, 0);
                        obj3.transform.name = $"img_{MapRow}_1_2";
                        break;
                    default:
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj2.transform.localPosition = new Vector2(-105, 0);
                        obj2.transform.name = $"img_{MapRow}_0_1";
                        obj3 = Common.AddChild(tempObject.transform, obj3);
                        obj3.transform.localPosition = new Vector2(107, 0);
                        obj3.transform.name = $"img_{MapRow}_1_2";
                        break;
                }
                obj1 = Common.AddChild(tempObject.transform, obj1);
                obj4 = Common.AddChild(tempObject.transform, obj4);
                obj1.transform.localPosition = new Vector2(-214, 0);
                obj4.transform.localPosition = new Vector2(237, 0);
                obj1.transform.name = $"img_{MapRow}_0_0";
                obj4.transform.name = $"img_{MapRow}_2_2";
            }
            if (CurrentRow == 4)
            {
                int random = Random.Range(0, 3);
                tempRandom = random;
                if (random == 0)
                {
                    var obj1 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                    var obj4 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj4 = Common.AddChild(tempObject.transform, obj4);
                    obj1.transform.localPosition = new Vector2(-285, 0);
                    obj2.transform.localPosition = new Vector2(-143, 0);
                    obj3.transform.localPosition = new Vector2(63, 0);
                    obj4.transform.localPosition = new Vector2(280, 0);
                    obj1.transform.name = $"img_{MapRow}_0_0";
                    obj2.transform.name = $"img_{MapRow}_0_1";
                    obj3.transform.name = $"img_{MapRow}_1_2";
                    obj4.transform.name = $"img_{MapRow}_2_3";
                }
                else if (random == 1)
                {
                    var obj1 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_1_xy") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                    var obj4 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj4 = Common.AddChild(tempObject.transform, obj4);
                    obj1.transform.localPosition = new Vector2(-285, 0);
                    obj2.transform.localPosition = new Vector2(-63, 0);
                    obj3.transform.localPosition = new Vector2(63, 0);
                    obj4.transform.localPosition = new Vector2(280, 0);
                    obj1.transform.name = $"img_{MapRow}_0_0";
                    obj2.transform.name = $"img_{MapRow}_1_1";
                    obj3.transform.name = $"img_{MapRow}_1_2";
                    obj4.transform.name = $"img_{MapRow}_2_3";
                }
                else if (random == 2)
                {
                    var obj1 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                    var obj2 = ResourcesManager.instance.Load("Path_img_1_xy") as GameObject;
                    var obj3 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                    var obj4 = ResourcesManager.instance.Load("Path_img_1_x") as GameObject;
                    var obj5 = ResourcesManager.instance.Load("Path_img_1_xy") as GameObject;
                    obj1 = Common.AddChild(tempObject.transform, obj1);
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj3 = Common.AddChild(tempObject.transform, obj3);
                    obj4 = Common.AddChild(tempObject.transform, obj4);
                    obj5 = Common.AddChild(tempObject.transform, obj5);
                    obj1.transform.localPosition = new Vector2(-285, 0);
                    obj2.transform.localPosition = new Vector2(-63, 0);
                    obj3.transform.localPosition = new Vector2(63, 0);
                    obj4.transform.localPosition = new Vector2(280, 0);
                    obj5.transform.localPosition = new Vector2(175, 0);
                    obj1.transform.name = $"img_{MapRow}_0_0";
                    obj2.transform.name = $"img_{MapRow}_1_1";
                    obj3.transform.name = $"img_{MapRow}_1_2";
                    obj4.transform.name = $"img_{MapRow}_2_3";
                    obj5.transform.name = $"img_{MapRow}_2_2";
                }
            }
        }
        else if (PreviousRow == 4)
        {
            if (CurrentRow == 1)
            {
                var obj1 = ResourcesManager.instance.Load("Path_img_3_y") as GameObject;
                var obj2 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                var obj3 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                var obj4 = ResourcesManager.instance.Load("Path_img_3") as GameObject;
                obj1 = Common.AddChild(tempObject.transform, obj1);
                obj2 = Common.AddChild(tempObject.transform, obj2);
                obj3 = Common.AddChild(tempObject.transform, obj3);
                obj4 = Common.AddChild(tempObject.transform, obj4);
                obj1.transform.localPosition = new Vector2(-205, -6);
                obj2.transform.localPosition = new Vector2(-65, 0);
                obj3.transform.localPosition = new Vector2(47, 14);
                obj4.transform.localPosition = new Vector2(219, -6);
                obj1.transform.name = $"img_{MapRow}_0_0";
                obj2.transform.name = $"img_{MapRow}_1_0";
                obj3.transform.name = $"img_{MapRow}_2_0";
                obj4.transform.name = $"img_{MapRow}_3_0";
            }
            if (CurrentRow == 2)
            {
                var random = Random.Range(0, 4);
                tempRandom = random;
                var obj1 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                var obj2 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                var obj3 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                var obj4 = ResourcesManager.instance.Load("Path_img_2") as GameObject;
                var obj5 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                var obj6 = ResourcesManager.instance.Load("Path_img_2_y") as GameObject;
                switch (random)
                {
                    case 0:
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj3 = Common.AddChild(tempObject.transform, obj3);
                        obj2.transform.localPosition = new Vector2(-100, 0);
                        obj3.transform.localPosition = new Vector2(0, 0);
                        obj2.transform.name = $"img_{MapRow}_1_0";
                        obj3.transform.name = $"img_{MapRow}_2_0";
                        break;
                    case 1:
                        obj4 = Common.AddChild(tempObject.transform, obj4);
                        obj5 = Common.AddChild(tempObject.transform, obj5);
                        obj4.transform.localPosition = new Vector2(0, 0);
                        obj5.transform.localPosition = new Vector2(124, 0);
                        obj4.transform.name = $"img_{MapRow}_1_1";
                        obj5.transform.name = $"img_{MapRow}_2_1";
                        break;
                    case 2:
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj3 = Common.AddChild(tempObject.transform, obj3);
                        obj5 = Common.AddChild(tempObject.transform, obj5);
                        obj2.transform.localPosition = new Vector2(-100, 0);
                        obj3.transform.localPosition = new Vector2(0, 0);
                        obj5.transform.localPosition = new Vector2(124, 0);
                        obj2.transform.name = $"img_{MapRow}_1_0";
                        obj3.transform.name = $"img_{MapRow}_2_0";
                        obj5.transform.name = $"img_{MapRow}_2_1";
                        break;
                    case 3:
                        obj2 = Common.AddChild(tempObject.transform, obj2);
                        obj4 = Common.AddChild(tempObject.transform, obj4);
                        obj5 = Common.AddChild(tempObject.transform, obj5);
                        obj2.transform.localPosition = new Vector2(-100, 0);
                        obj4.transform.localPosition = new Vector2(0, 0);
                        obj5.transform.localPosition = new Vector2(124, 0);
                        obj2.transform.name = $"img_{MapRow}_1_0";
                        obj4.transform.name = $"img_{MapRow}_1_1";
                        obj5.transform.name = $"img_{MapRow}_2_1";
                        break;
                }
                obj1 = Common.AddChild(tempObject.transform, obj1);
                obj6 = Common.AddChild(tempObject.transform, obj6);
                obj1.transform.localPosition = new Vector2(-222, 0);
                obj6.transform.localPosition = new Vector2(222, 0);
                obj1.transform.name = $"img_{MapRow}_0_0";
                obj6.transform.name = $"img_{MapRow}_3_1";
            }
            if (CurrentRow == 3)
            {
                var random = Random.Range(0, 2);
                tempRandom = random;
                var obj1 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                var obj2 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                var obj3 = ResourcesManager.instance.Load("Path_img_1_y") as GameObject;
                var obj4 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                var obj5 = ResourcesManager.instance.Load("Path_img_1") as GameObject;
                if (random == 1)
                {
                    obj2 = Common.AddChild(tempObject.transform, obj2);
                    obj2.transform.localPosition = new Vector2(-162, 10);
                    obj2.transform.name = $"img_{MapRow}_1_0";
                }
                obj1 = Common.AddChild(tempObject.transform, obj1);
                obj3 = Common.AddChild(tempObject.transform, obj3);
                obj4 = Common.AddChild(tempObject.transform, obj4);
                obj5 = Common.AddChild(tempObject.transform, obj5);
                obj1.transform.localPosition = new Vector2(-274, 10);
                obj3.transform.localPosition = new Vector2(-40, 10);
                obj4.transform.localPosition = new Vector2(79, 10);
                obj5.transform.localPosition = new Vector2(259, 10);
                obj1.transform.name = $"img_{MapRow}_0_0";
                obj3.transform.name = $"img_{MapRow}_1_1";
                obj4.transform.name = $"img_{MapRow}_2_1";
                obj5.transform.name = $"img_{MapRow}_3_2";
            }
            if (CurrentRow == 4)
            {
                int random = Random.Range(0, 2);
                tempRandom = random;
                var obj1 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                var obj2 = ResourcesManager.instance.Load($"Path_img_2{(random == 0 ? "_y" : "")}") as GameObject;
                var obj3 = ResourcesManager.instance.Load($"Path_img_2{(random == 0 ? "_y" : "")}") as GameObject;
                var obj4 = ResourcesManager.instance.Load($"Path_img_2{(random == 0 ? "_y" : "")}") as GameObject;
                var obj5 = ResourcesManager.instance.Load("Path_img_0") as GameObject;
                obj1 = Common.AddChild(tempObject.transform, obj1);
                obj2 = Common.AddChild(tempObject.transform, obj2);
                obj3 = Common.AddChild(tempObject.transform, obj3);
                obj4 = Common.AddChild(tempObject.transform, obj4);
                obj5 = Common.AddChild(tempObject.transform, obj5);
                obj1.transform.localPosition = new Vector2(-330, 0);
                obj2.transform.localPosition = new Vector2(-210, 9);
                obj3.transform.localPosition = new Vector2(0, 9);
                obj4.transform.localPosition = new Vector2(222, 9);
                obj5.transform.localPosition = new Vector2(340, 0);
                obj1.transform.name = $"img_{MapRow}_0_0";
                if (random == 1)
                {
                    obj2.transform.name = $"img_{MapRow}_0_1";
                    obj3.transform.name = $"img_{MapRow}_1_2";
                    obj4.transform.name = $"img_{MapRow}_2_3";
                }
                else if (random == 0)
                {
                    obj2.transform.name = $"img_{MapRow}_1_0";
                    obj3.transform.name = $"img_{MapRow}_2_1";
                    obj4.transform.name = $"img_{MapRow}_3_2";
                }
                obj5.transform.name = $"img_{MapRow}_3_3";
            }
        }

        listPath.Add(new MapPath
        {
            Row = MapRow,
            PreviousRow = PreviousRow,
            CurrentRow = CurrentRow,
            RandomNum = tempRandom
        });
        if (MapRow == 13)
        {
            Common.SaveTxtFile(listPath.ListToJson(), GlobalAttr.CurrentMapPathFileName, "Map");
        }
    }
    #endregion

    /// <summary>
    /// 战斗点击事件
    /// </summary>
    /// <param name="type">1普通战斗、2商店、3Boss、4冒险</param>
    /// <param name="level">等级</param>
    /// <param name="thisObj">当前组件</param>
    /// <param name="CurrentRow">当前行</param>
    public void MapAtkImgClick(int type, int level, GameObject thisObj, int CurrentRow, string currentImgUrl)
    {
        HideTitle();
        //点击后展示title
        var title = thisObj.transform.Find("Map_Title_img")?.GetComponent<Image>();
        Text title_txt = null;
        if (title == null)
        {
            GameObject Atkimg = ResourcesManager.instance.Load("Map_Title_img") as GameObject;
            Atkimg = Common.AddChild(thisObj.transform, Atkimg);
            Atkimg.name = "Map_Title_img";
            title_txt = Atkimg.transform.GetChild(0).GetComponent<Text>();
            switch (type)
            {
                case 2:
                    title_txt.text = "商  店";
                    break;
                case 4:
                    title_txt.text = "冒  险";
                    break;
                case 3:
                    title_txt.text = CurrentAiModel.Name;
                    break;
                default:
                    title_txt.text = "战  斗";
                    break;
            }
        }
        else
        {
            title.transform.localScale = Vector3.one;
        }

        //如果是按路线点击、则改变路线颜色、且显示按钮
        string thisIndex = null;
        if (type == 2)
        {
            thisIndex = thisObj.name.Split('_')[1].Substring(7);

        }
        else if (type == 3)
        {
            thisIndex = "0";
        }
        else
        {
            thisIndex = thisObj.name.Split('_')[1].Substring(3);
        }
        if (mapLocation.Row + 1 == CurrentRow)
        {

            var thisPath = transform.Find($"BG/Path_{CurrentRow}/img_{CurrentRow}_{mapLocation.Column}_{thisIndex}")?.GetComponent<Image>();
            if (thisPath != null)
            {
                thisPath.color = Color.yellow;
                btn_GameStart.transform.localScale = Vector3.one;
                btn_GameStart.onClick.RemoveAllListeners();
                btn_GameStart.onClick.AddListener(delegate { GameStartClick(type, level, CurrentRow, System.Convert.ToInt32(thisIndex), thisObj, currentImgUrl); });
            }
        }
        //如果点击的是角色头像，展示角色名称
        if (mapLocation.Row == CurrentRow && mapLocation.Column == System.Convert.ToInt32(thisIndex))
        {
            if (title_txt == null)
            {
                title.transform.localScale = Vector3.one;
            }
            else
            {
                title_txt.text = PlayerRole.Name;
            }
        }
    }


    /// <summary>
    /// 游戏开始
    /// </summary>
    /// <param name="type">当前地图类型</param>
    /// <param name="level"></param>
    /// <param name="currentRow"></param>
    public void GameStartClick(int type, int level, int currentRow, int currentColumn, GameObject obj, string currentImgUrl)
    {
        //位置移动
        MoveTopOneUnitState = true;
        OneUnitCount = 0;
        string imgColumnName = "";
        string SceneName = "";
        switch (mapLocation.MapType)
        {
            case 2:
                imgColumnName = $"Atk_imgShop{mapLocation.Column}";
                SceneName = "ShoppingView";
                break;
            //case 3:
            //    imgColumnName = "Atk_imgBoss";
            //    break;
            case 4:
                imgColumnName = $"Adventure_img{mapLocation.Column}";
                SceneName = "AdventureView";
                break;
            default:
                imgColumnName = $"Atk_img{mapLocation.Column}";
                SceneName = "GameView";
                break;
        }
        string imgRowName = "";
        if (currentRow == 13)
        {
            imgRowName = $"Map_RowShop";
        }
        else
        {
            imgRowName = $"Map_Row{mapLocation.Row}";
        }
        var aa = $"BG/Map_Row{mapLocation.Row}/{imgColumnName}";
        var currentImg = GameObject.Find($"BG/{imgRowName}/{imgColumnName}")?.GetComponent<Image>();
        Common.ImageBind(mapLocation.CurrentImgUrl, currentImg);

        mapLocation.Column = currentColumn;
        mapLocation.Row = currentRow;
        mapLocation.CurrentImgUrl = currentImgUrl;
        mapLocation.MapType = type;
        Common.SaveTxtFile(mapLocation.ObjectToJson(), GlobalAttr.CurrentMapLocationFileName, "Map");
        var thisImg = obj.GetComponent<Image>();
        if (currentRow == 13)
        {
            GameObject tempObj = ResourcesManager.instance.Load("Map_Atk_img") as GameObject;
            tempObj = Common.AddChild(thisImg.transform, tempObj);
            tempObj.name = "img_Player_Head";
            tempObj.transform.localPosition = new Vector3(-100, -30);
            var tempImg = thisImg.transform.Find($"img_Player_Head").GetComponent<Image>();
            Common.ImageBind(PlayerRole.HeadPortraitUrl, tempImg);
        }
        else
        {
            Common.ImageBind(PlayerRole.HeadPortraitUrl, thisImg);
        }
        //进入游戏
        UIManager.instance.OpenView(SceneName);
        UIManager.instance.CloseView("MapView");
    }
    #endregion

    public override void OnClose()
    {
        HasMouseDown = false;
    }

    #region Uinty事件

    void Update()
    {
        #region 获取事件实行地图移动
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            //如果位置在topbar上则不移动，topBar y=90
            var currentPosition = Input.mousePosition;
            if (currentPosition.y < Screen.height - 90)
            {
                //记录鼠标按下的位置 　　
                first = Input.mousePosition;
                HasMouseDown = true;
            }
        }
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            HasMouseUp = true;
        }
        if (HasMouseDown && !HasMouseUp)
        {
            second = Input.mousePosition;
            vecPos = second - first;//需要移动的 向量
            first = second;
            IsNeedMove = true;
        }
        else
        {
            HasMouseUp = false;
            HasMouseDown = false;
            IsNeedMove = false;
        }
        #endregion

        #region 判断当前地图位置展示悬浮框
        //判断角色悬浮框 初始==1800+80初始位置 1660
        //图长3600,如果是1280*720，则最底部为1800
        //如果高是1080，则最底部为2700
        //底部是屏幕高度的2.5倍
        //Debug.Log(img_map.transform.position.y);//2700/2400 1800/1600--角色头像
        //                                        2700/          1800/-1070
        //var bottomY = Screen.height * 2.5;
        //var topY = Screen.height * -1.5;
        //var lineSpacing = bottomY / 9;
        if (img_map.transform.position.y < bottomY - (mapLocation.Row + 1) * lineSpacing)
        {
            btn_Suspension_Player.transform.localScale = Vector3.one;
            //加
            MoveType = 0;
        }
        else if (img_map.transform.position.y > bottomY - (mapLocation.Row - 2) * lineSpacing)
        {
            btn_Suspension_Player.transform.localScale = Vector3.one;
            //减
            MoveType = 1;
        }
        else
        {
            btn_Suspension_Player.transform.localScale = Vector3.zero;
        }
        //判断boss悬浮框-1050
        if (img_map.transform.position.y < topY - 30)
        {
            btn_Suspension_Boss.transform.localScale = Vector3.zero;
        }
        else
        {
            btn_Suspension_Boss.transform.localScale = Vector3.one;
        }
        #endregion

        #region 点击按钮地图移动
        if (MoveTopState)
        {
            float y = img_map.transform.position.y - 10;
            if (y < InitPos.y && y > topY)
            {
                img_map.transform.position = new Vector3(InitPos.x, y, 0);
            }
            else
            {
                MoveTopState = false;
            }
        }
        if (MoveUpState)
        {
            float y = 0;
            if (MoveType == 0)
            {
                y = img_map.transform.position.y + 10;
                if (y < InitPos.y && y < bottomY - mapLocation.Row * lineSpacing)
                {
                    img_map.transform.position = new Vector3(InitPos.x, y, 0);
                }
                else
                {
                    MoveUpState = false;
                }
            }
            else
            {
                y = img_map.transform.position.y - 10;
                if (y < InitPos.y && y > bottomY - mapLocation.Row * lineSpacing)
                {
                    img_map.transform.position = new Vector3(InitPos.x, y, 0);
                }
                else
                {
                    MoveUpState = false;
                }
            }

        }
        #endregion

        #region 地图移动一个单位
        if (MoveTopOneUnitState)
        {
            if (OneUnitCount < 24)
            {
                OneUnitCount++;
                float y = img_map.transform.position.y - 10;
                img_map.transform.position = new Vector3(InitPos.x, y, 0);
            }
            else
            {
                OneUnitCount = 0;
                MoveTopOneUnitState = false;
            }
        }
        #endregion

        #region 地图移动
        if (IsNeedMove)
        {
            float y = img_map.transform.position.y + vecPos.y;
            if (y < InitPos.y && y > topY)
            {
                img_map.transform.position = new Vector3(InitPos.x, y, 0);
            }
        }
        #endregion

        #region 地图生成
        if (CreateMapState)
        {
            //第一行玩家
            if (MapRow < 13)
            {
                CreateMap(0);
            }
            //boss
            if (MapRow == 13)
            {
                CreateMap(99);
            }
            //boss
            if (MapRow == 12)
            {
                CreateMap(1);
            }
        }
        #endregion

    }
    #endregion

}
