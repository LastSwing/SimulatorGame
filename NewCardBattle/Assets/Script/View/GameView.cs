using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : BaseUI
{
    Image img_Background;//背景图片
    Text  txt_ReturnView, txt_SettingHasBtn, txt_ReturnView1, txt_CardType;
    Button btn_Setting, btn_RoundOver, btn_leftCards, btn_rightCards;
    Image img_Player, img_Enemy, Pimg_HP, Eimg_HP, Pimg_Armor, Eimg_Armor;
    GameObject obj_CardPools;
    Text txt_P_HP, txt_E_HP, txt_P_Armor, txt_E_Armor, txt_Left_Count, txt_Right_Count;
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

        img_Background = transform.Find("BG").GetComponent<Image>();
        btn_Setting = transform.Find("UI/Setting").GetComponent<Button>();
        btn_RoundOver = transform.Find("UI/btn_RoundOver").GetComponent<Button>();
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
        UIManager.instance.OpenView("SettingView");
        UIManager.instance.CloseView("GameView");
    }

    public void LeftCardsClick()
    {
        txt_ReturnView.text = "GameView";
        txt_CardType.text = "1";
        UIManager.instance.OpenView("CardPoolsView");
        UIManager.instance.CloseView("GameView");
    }
    public void RightCardsClick()
    {
        txt_ReturnView.text = "GameView";
        txt_CardType.text = "2";
        UIManager.instance.OpenView("CardPoolsView");
        UIManager.instance.CloseView("GameView");
    }
    #endregion

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

    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void InitUIData()
    {

    }

    public override void OnClose()
    {
        //throw new System.NotImplementedException();
    }

}
