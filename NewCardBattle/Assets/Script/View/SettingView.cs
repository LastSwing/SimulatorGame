using Assets.Script.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingView : BaseUI
{
    Scrollbar ScbBGM, ScbVoice;
    Image img_Background;//背景图片
    Button btn_GameOver, btn_CardList, btn_Return;
    Text txt_ReturnView, txt_SettingHasBtn, txt_ReturnView1;//返回按钮所跳页面；是否显示设置页面按钮0否，1是
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
        txt_ReturnView = GameObject.Find("MainCanvas/txt_ReturnView").GetComponent<Text>();
        txt_ReturnView1 = GameObject.Find("MainCanvas/txt_ReturnView1").GetComponent<Text>();
        txt_SettingHasBtn = GameObject.Find("MainCanvas/txt_SettingHasBtn").GetComponent<Text>();
        img_Background = transform.Find("BG").GetComponent<Image>();
        btn_GameOver = transform.Find("UI/Content/btn_GameOver").GetComponent<Button>();
        btn_CardList = transform.Find("UI/Content/btn_CardList").GetComponent<Button>();
        btn_Return = transform.Find("UI/Content/btn_Return").GetComponent<Button>();
        ScbBGM = transform.Find("UI/Content/BGM/ScbBGM").GetComponent<Scrollbar>();
        ScbVoice = transform.Find("UI/Content/Voice/ScbVoice").GetComponent<Scrollbar>();
    }

    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {
        btn_GameOver.onClick.AddListener(GameOverClick);
        btn_CardList.onClick.AddListener(CardListClick);
        btn_Return.onClick.AddListener(ReturnClick);
        ScbBGM.onValueChanged.AddListener(delegate { BMGChange(); });
        ScbVoice.onValueChanged.AddListener(delegate { VoiceChange(); });
    }

    #region 按钮点击事件
    public void GameOverClick()
    {
        UIManager.instance.OpenView("PlayerDieView");
        UIManager.instance.CloseView("SettingView");
    }

    public void CardListClick()
    {
        txt_ReturnView.text = "SettingView";
        UIManager.instance.OpenView("AllSkillView");
        UIManager.instance.CloseView("SettingView");
    }

    public void ReturnClick()
    {
        UIManager.instance.OpenView(txt_ReturnView1.text);
        UIManager.instance.CloseView("SettingView");
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
        if (txt_SettingHasBtn.text == "0")
        {
            btn_GameOver.transform.localScale = Vector3.zero;
            ScbBGM.value = SoundManager.instance.CurrentVolume((int)TrackType.BGM);
            ScbVoice.value = SoundManager.instance.CurrentVolume((int)TrackType.Voice);
        }
        else
        {
            btn_GameOver.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void InitUIData()
    {

    }
    #endregion

    public void BMGChange()
    {
        SoundManager.instance.ChangeMusicVolume(ScbBGM.value, (int)TrackType.BGM);
    }
    public void VoiceChange()
    {
        SoundManager.instance.ChangeMusicVolume(ScbVoice.value, (int)TrackType.Voice);
    }

    public override void OnClose()
    {

    }
}
