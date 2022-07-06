using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShoppingView : BaseUI
{
    Button btn_Setting, btn_AllSkills, btn_CardPools, btn_CardUpgrade, btn_Return;
    GameObject LeftGoods, RightGoods, CardDetails, Confirms;
    Text txt_Silver;
    Text txt_CardPoolsCount, txt_ReturnView, txt_CardType, txt_SettingHasBtn, txt_ReturnView1, txt_HasClickSetting;

    List<CurrentCardPoolModel> GlobalPlayerCardPools = new List<CurrentCardPoolModel>();//角色全局卡池
    List<CurrentCardPoolModel> RightCarPools = new List<CurrentCardPoolModel>();//右边卡池
    List<CurrentCardPoolModel> CurrentCardList = new List<CurrentCardPoolModel>();

    CurrentRoleModel PlayerRole;

    #region OnInit
    public override void OnInit()
    {
        //throw new System.NotImplementedException();
        //因为获取组件以及绑定事件一般只需要做一次，所以放在OnInit
        InitComponent();
        InitUIevent();
    }

    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitComponent()
    {
        btn_Setting = transform.Find("UI/TopBar/Setting").GetComponent<Button>();
        btn_AllSkills = transform.Find("UI/btn_CardList").GetComponent<Button>();
        btn_CardPools = transform.Find("UI/CardPools_Obj").GetComponent<Button>();
        btn_CardUpgrade = transform.Find("UI/btn_CardUpgrade").GetComponent<Button>();
        btn_Return = transform.Find("UI/btn_Return").GetComponent<Button>();
        LeftGoods = transform.Find("UI/LeftGoods").gameObject;
        RightGoods = transform.Find("UI/RightGoods").gameObject;
        CardDetails = transform.Find("UI/CardDetails").gameObject;
        Confirms = transform.Find("UI/Confirms").gameObject;
        txt_Silver = transform.Find("UI/TopBar/img_Silver/txt_Silver").GetComponent<Text>();
        txt_CardPoolsCount = transform.Find("UI/CardPools_Obj/Image/Text").GetComponent<Text>();

        txt_ReturnView = GameObject.Find("MainCanvas/txt_ReturnView").GetComponent<Text>();
        txt_ReturnView1 = GameObject.Find("MainCanvas/txt_ReturnView1").GetComponent<Text>();
        txt_CardType = GameObject.Find("MainCanvas/txt_CardType").GetComponent<Text>();
        txt_SettingHasBtn = GameObject.Find("MainCanvas/txt_SettingHasBtn").GetComponent<Text>();
        txt_HasClickSetting = GameObject.Find("MainCanvas/txt_HasClickSetting").GetComponent<Text>();
    }

    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {
        #region 给画布添加隐藏技能详细事件

        EventTrigger trigger = transform.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = transform.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener(delegate { HideCardDetail(); });
        trigger.triggers.Add(entry);
        #endregion
        btn_AllSkills.onClick.AddListener(AllSkillsClick);
        btn_Setting.onClick.AddListener(SettingClick);
        btn_CardPools.onClick.AddListener(CardPoolsClick);
        btn_CardUpgrade.onClick.AddListener(CardUpgradeClick);
        btn_Return.onClick.AddListener(ReturnClick);
    }

    #region 按钮点击事件

    public void ReturnClick()
    {
        //txt_HasClickSetting.text = "1";
        UIManager.instance.OpenView(txt_ReturnView.text);
        UIManager.instance.CloseView("ShoppingView");
    }

    private void SettingClick()
    {
        HideCardDetail();
        txt_ReturnView.text = "ShoppingView";
        txt_ReturnView1.text = "ShoppingView";
        txt_SettingHasBtn.text = "1";
        txt_HasClickSetting.text = "1";
        UIManager.instance.OpenView("SettingView");
        UIManager.instance.CloseView("ShoppingView");
    }

    private void AllSkillsClick()
    {
        HideCardDetail();
        txt_ReturnView.text = "ShoppingView";
        txt_HasClickSetting.text = "1";
        UIManager.instance.OpenView("AllSkillView");
        UIManager.instance.CloseView("ShoppingView");
    }

    private void CardPoolsClick()
    {
        HideCardDetail();
        txt_ReturnView.text = "ShoppingView";
        txt_CardType.text = "0";
        txt_HasClickSetting.text = "1";
        UIManager.instance.OpenView("CardPoolsView");
        UIManager.instance.CloseView("ShoppingView");
    }

    private void CardUpgradeClick()
    {
        HideCardDetail();
        txt_ReturnView.text = "ShoppingView";
        UIManager.instance.OpenView("UpgradeView");
        UIManager.instance.CloseView("ShoppingView");
    }

    #endregion 
    #endregion

    public override void OnOpen()
    {
        //数据需要每次打开都要刷新，UI状态也是要每次打开都进行刷新，因此放在OnOpen
        InitUIData();
        InitUIState();
        InitSetting();
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void InitUIData()
    {
        if (txt_HasClickSetting.text == "0")
        {
            GlobalPlayerCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName);
            GlobalPlayerCardPools = GlobalPlayerCardPools.FindAll(a => a.HasShoppingShow == 1).ListRandom();
            RightCarPools = GlobalPlayerCardPools.FindAll(a => a.HasShoppingShow == 1).ListRandom();
            PlayerRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName);
            CurrentCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentCardPoolsFileName);
        }
    }

    /// <summary>
    /// 更新UI状态
    /// </summary>
    private void InitUIState()
    {
        if (txt_HasClickSetting.text == "0")
        {
            AddLeftGoods();
            AddRightGoods();
            txt_CardPoolsCount.text = CurrentCardList.Count.ToString();
        }
        else
        {
            txt_HasClickSetting.text = "0";
        }
    }

    /// <summary>
    /// 初始化其余设置
    /// </summary>
    private void InitSetting()
    {
        //SoundManager.instance.PlayOnlyOneSound("BGM_1", (int)TrackType.BGM, true);
    }

    #region 购物栏添加卡牌


    private void AddLeftGoods()
    {
        int count = LeftGoods.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            DestroyImmediate(LeftGoods.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
        }
        for (int i = 0; i < 3; i++)
        {
            CreateAwardCrad(GlobalPlayerCardPools[i], i, "LeftGoods");
        }
    }
    private void AddRightGoods()
    {
        int count = RightGoods.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            DestroyImmediate(RightGoods.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
        }
        for (int i = 0; i < 2; i++)
        {
            CreateAwardCrad(RightCarPools[i], i + 3, "RightGoods");
        }
    }


    /// <summary>
    /// 创建卡
    /// </summary>
    /// <param name="model"></param>
    /// <param name="i"></param>
    private void CreateAwardCrad(CurrentCardPoolModel model, int i, string type)
    {
        GameObject tempObject = ResourcesManager.instance.Load("img_CardShop") as GameObject;
        tempObject = Common.AddChild(type == "LeftGoods" ? LeftGoods.transform : RightGoods.transform, tempObject);
        tempObject.name = "img_Card" + i;
        EventTrigger trigger = tempObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = tempObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener(delegate { ShowDetail(model, i, type); });
        trigger.triggers.Add(entry);

        Common.CardDataBind(tempObject, model);
        var Card_Price = tempObject.transform.Find("Text").GetComponent<Text>();
        Card_Price.text = model.CardPrice.ToString();
        if (PlayerRole.Wealth < model.CardPrice)
        {
            Card_Price.color = Color.red;
        }
    }


    /// <summary>
    /// 显示详情
    /// </summary>
    /// <param name="model"></param>
    /// <param name="i"></param>
    /// <param name="type"></param>
    public void ShowDetail(CurrentCardPoolModel model, int i, string type)
    {
        HideCardDetail();
        var Card_Detail = transform.Find($"UI/CardDetails/img_Detail{i}")?.GetComponent<Image>();
        if (Card_Detail != null)
        {
            Card_Detail.transform.localScale = Vector3.one;
            var btn_Confirm = transform.Find($"UI/Confirms/btn_ShopConfirm{i}")?.GetComponent<Button>();
            if (btn_Confirm != null)
            {
                btn_Confirm.transform.localScale = Vector3.one;
                if (PlayerRole.Wealth < model.CardPrice)
                {
                    var txt = transform.Find($"UI/{type}/img_Card{i}/Text").GetComponent<Text>();
                    txt.color = Color.red;
                    DestroyImmediate(btn_Confirm.gameObject);
                }
            }
        }
        else
        {
            GameObject Card_img;
            if (type == "LeftGoods")
            {
                Card_img = LeftGoods.transform.Find($"img_Card{i}").gameObject;
            }
            else
            {
                Card_img = RightGoods.transform.Find($"img_Card{i}").gameObject;
            }

            GameObject tempImg = ResourcesManager.instance.Load("img_CardDetail") as GameObject;
            tempImg = Common.AddChild(CardDetails.transform, tempImg);
            tempImg.name = "img_Detail" + i;
            tempImg.transform.position = new Vector2(Card_img.transform.position.x, Card_img.transform.position.y + Screen.height / 4);

            GameObject temp = tempImg.transform.Find("Text").gameObject;
            temp.GetComponent<Text>().text = $"{model.CardName}\n{model.CardDetail}";

            if (PlayerRole.Wealth >= model.CardPrice)
            {
                var tempConfirm = ResourcesManager.instance.Load("btn_ShopConfirm") as GameObject;
                tempConfirm = Common.AddChild(Confirms.transform, tempConfirm);
                tempConfirm.name = "btn_ShopConfirm" + i;
                tempConfirm.transform.position = new Vector2(Card_img.transform.position.x, Card_img.transform.position.y - Screen.height / 5.76f);
                var btn = tempConfirm.GetComponent<Button>();
                btn.onClick.AddListener(delegate { ConfirmPurchase(model, i, type); });
            }
            else
            {
                var txt = Card_img.transform.Find("Text").GetComponent<Text>();
                txt.color = Color.red;
            }
        }
    }

    /// <summary>
    /// 隐藏详情
    /// </summary>
    public void HideCardDetail()
    {
        for (int i = 0; i < 5; i++)
        {
            var Card_img = transform.Find($"UI/CardDetails/img_Detail{i}")?.GetComponent<Image>();
            if (Card_img != null)
            {
                Card_img.transform.localScale = Vector3.zero;
            }
            var btn_Confirm = transform.Find($"UI/Confirms/btn_ShopConfirm{i}")?.GetComponent<Button>();
            if (btn_Confirm != null)
            {
                btn_Confirm.transform.localScale = Vector3.zero;
            }
        }
    }

    public void ConfirmPurchase(CurrentCardPoolModel model, int i, string type)
    {
        PlayerRole.Wealth -= model.CardPrice;
        txt_Silver.text = PlayerRole.Wealth.ToString();
        txt_CardPoolsCount.text = (Convert.ToInt32(txt_CardPoolsCount.text) + 1).ToString();
        CurrentCardList.Add(model);
        var txt = transform.Find($"UI/{type}/img_Card{i}/Text").GetComponent<Text>();
        var img = transform.Find($"UI/{type}/img_Card{i}/Image").GetComponent<Image>();
        txt.text = "已售";
        img.transform.localScale = Vector3.zero;
        Common.SaveTxtFile(CurrentCardList.ListToJson(), GlobalAttr.CurrentCardPoolsFileName);
        Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
        HideCardDetail();
        var btn = transform.Find($"UI/Confirms/btn_ShopConfirm{i}")?.gameObject;
        if (btn != null)
        {
            DestroyImmediate(btn);//如不是删除后马上要使用则用Destroy方法
        }
    }
    #endregion

    public override void OnClose()
    {

    }

}
