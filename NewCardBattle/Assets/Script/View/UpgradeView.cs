using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeView : BaseUI
{
    List<CurrentCardPoolModel> CardList = new List<CurrentCardPoolModel>();
    List<CurrentCardPoolModel> UpgradeCardList = new List<CurrentCardPoolModel>();//升级后得卡池
    GameObject Content_Obj;
    Text txt_ReturnView, txt_SettingHasBtn, txt_ReturnView1, txt_HasClickSetting;
    Text txt_UpgradePrice, txt_SelectedCardID, txt_Silver;
    RectTransform Content_Rect;
    Image img_Background;//背景图片
    Button btn_Return, btn_Upgrade, btn_Setting;
    CurrentRoleModel PlayerRole;
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
        img_Background = transform.Find("BG").GetComponent<Image>();
        Content_Obj = transform.Find("UI/CardPoolsArea/CardPools/Content").gameObject;
        Content_Rect = transform.Find("UI/CardPoolsArea/CardPools/Content").GetComponent<RectTransform>();
        btn_Return = transform.Find("UI/btn_Return").GetComponent<Button>();
        btn_Upgrade = transform.Find("UI/btn_Upgrade").GetComponent<Button>();
        btn_Setting = transform.Find("UI/TopBar/Setting").GetComponent<Button>();
        txt_UpgradePrice = transform.Find("UI/btn_Upgrade/Text").GetComponent<Text>();
        txt_SelectedCardID = transform.Find("UI/txt_SelectedCardID").GetComponent<Text>();
        txt_Silver = transform.Find("UI/TopBar/img_Silver/txt_Silver").GetComponent<Text>();

        txt_ReturnView = GameObject.Find("MainCanvas/txt_ReturnView").GetComponent<Text>();
        txt_ReturnView1 = GameObject.Find("MainCanvas/txt_ReturnView1").GetComponent<Text>();
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
        btn_Return.onClick.AddListener(ReturnClick);
        btn_Upgrade.onClick.AddListener(UpgradeClick);
        btn_Setting.onClick.AddListener(SettingClick);
    }

    public void ReturnClick()
    {
        txt_HasClickSetting.text = "1";
        UIManager.instance.OpenView(txt_ReturnView.text);
        UIManager.instance.CloseView("UpgradeView");
    }

    private void SettingClick()
    {
        txt_ReturnView.text = "UpgradeView";
        txt_ReturnView1.text = "UpgradeView";
        txt_SettingHasBtn.text = "1";
        txt_HasClickSetting.text = "1";
        UIManager.instance.OpenView("SettingView");
        UIManager.instance.CloseView("UpgradeView");
    }

    public void UpgradeClick()
    {
        CurrentCardPoolModel model = CardList.Find(a => a.ID == Convert.ToInt32(txt_SelectedCardID.text));
        var UpModel = UpgradeCardList.Find(a => a.ID == Convert.ToInt32(txt_SelectedCardID.text));
        int price = Convert.ToInt32(txt_UpgradePrice.text);
        if (PlayerRole.Wealth >= price)
        {
            PlayerRole.Wealth -= price;
            UpModel.Proficiency = 0;
            CardList.Remove(model);
            CardList.Add(UpModel);
            txt_Silver.text = PlayerRole.Wealth.ToString();
            InitUIState();
            Common.SaveTxtFile(CardList.ListToJson(), GlobalAttr.CurrentCardPoolsFileName);
            Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
        }
    }

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
    /// 更新数据
    /// </summary>
    private void InitUIData()
    {
        if (txt_HasClickSetting.text == "0")
        {
            CardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentCardPoolsFileName).FindAll(a => a.UpgradeCount == 0);
            UpgradeCardList = Common.CardUpgrade();
            PlayerRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName);
        }
    }

    /// <summary>
    /// 更新UI状态
    /// </summary>
    private void InitUIState()
    {
        if (txt_HasClickSetting.text == "0")
        {
            btn_Upgrade.transform.localScale = Vector3.zero;
            CreateCardPools();
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

    #endregion

    #region 卡池创建

    public void CreateCardPools()
    {
        if (Content_Obj != null)
        {
            int childCount = Content_Obj.transform.childCount;
            for (int x = 0; x < childCount; x++)
            {
                DestroyImmediate(Content_Obj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
            if (CardList != null && CardList?.Count > 0)
            {
                float height = (float)(290 * System.Math.Ceiling((float)CardList.Count / 3f));
                for (int i = 0; i < CardList?.Count; i++)
                {
                    CreateAwardCrad(CardList[i], i);
                }
                Content_Rect.sizeDelta = new Vector2(0, height);
            }
        }
    }

    /// <summary>
    /// 创建卡
    /// </summary>
    /// <param name="model"></param>
    /// <param name="i"></param>
    private void CreateAwardCrad(CurrentCardPoolModel model, int i)
    {
        GameObject tempObject = ResourcesManager.instance.Load("img_Card240") as GameObject;
        tempObject = Common.AddChild(Content_Obj.transform, tempObject);
        tempObject.name = "img_Card" + i;
        EventTrigger trigger = tempObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = tempObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener(delegate { ShowDetail(model, i); });
        trigger.triggers.Add(entry);

        Common.CardDataBind(tempObject, model);
    }


    /// <summary>
    /// 隐藏详情
    /// </summary>
    public void HideCardDetail()
    {
        btn_Upgrade.transform.localScale = Vector3.zero;
        for (int i = 0; i < CardList.Count; i++)
        {
            var Card_img = GameObject.Find($"CardDetails/img_Detail{i}")?.GetComponent<Image>();
            if (Card_img != null)
            {
                Card_img.transform.localScale = Vector3.zero;
            }
            var Card_imgUp = GameObject.Find($"CardDetails/img_Detail_Up{i}")?.GetComponent<Image>();
            if (Card_imgUp != null)
            {
                Card_imgUp.transform.localScale = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// 显示详情
    /// </summary>
    /// <param name="model"></param>
    /// <param name="i"></param>
    public void ShowDetail(CurrentCardPoolModel model, int i)
    {
        HideCardDetail();
        txt_SelectedCardID.text = model.ID.ToString();
        btn_Upgrade.transform.localScale = Vector3.one;
        txt_UpgradePrice.text = (100 - model.Proficiency).ToString();
        if (PlayerRole.Wealth < Convert.ToInt32(txt_UpgradePrice.text))
        {
            txt_UpgradePrice.color = Color.red;
        }
        var Card_Detail = GameObject.Find($"CardDetails/img_Detail{i}")?.GetComponent<Image>();
        if (Card_Detail != null)
        {
            Card_Detail.transform.localScale = Vector3.one;
            var Card_DetailUp = GameObject.Find($"CardDetails/img_Detail_Up{i}")?.GetComponent<Image>();
            if (Card_DetailUp != null)
            {
                Card_DetailUp.transform.localScale = Vector3.one;
            }
        }
        else
        {
            var Card_img = GameObject.Find($"CardDetails");
            GameObject tempImg = ResourcesManager.instance.Load("img_CardDetail") as GameObject;
            tempImg = Common.AddChild(Card_img.transform, tempImg);
            tempImg.name = "img_Detail" + i;
            tempImg.transform.localPosition = new Vector2(0, 80);

            Common.CardDetailDataBind(tempImg, model);

            GameObject tempImgUp = ResourcesManager.instance.Load("img_CardDetail") as GameObject;
            tempImgUp = Common.AddChild(Card_img.transform, tempImgUp);
            tempImgUp.name = "img_Detail_Up" + i;
            tempImgUp.transform.localPosition = new Vector2(0, -80);

            Text tempUp = tempImgUp.transform.Find("Text").GetComponent<Text>();
            var upModel = UpgradeCardList.Find(a => a.ID == model.ID);
            tempUp.text = $"{upModel.CardName}\n{upModel.CardDetail}";
            if (upModel.UpgradeCount == 0)
            {
                tempUp.color = Color.black;
            }
            else if (upModel.UpgradeCount == 1)
            {
                tempUp.color = new Color32(0, 205, 12, 255);
            }
            else
            {
                tempUp.color = Color.blue;
            }
        }
    }
    #endregion

    public override void OnClose()
    {
        //throw new System.NotImplementedException();
    }
}
