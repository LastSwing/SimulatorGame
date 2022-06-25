using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainView : BaseUI
{
    Image img_Background;//背景图片
    Text txt_SkillsCount;//图鉴收集
    Text txt_GoldCount;//金币数量
    Button btn_Setting;//设置按钮
    Button btn_Start;//开始按钮

    int skillNow, skillMax;
    int goldNum;
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
        txt_SkillsCount = transform.Find("UI/TopBar/Skills/txt_SkillsCount").GetComponent<Text>();
        txt_GoldCount = transform.Find("UI/TopBar/Gold/txt_GoldCount").GetComponent<Text>();
        btn_Setting = transform.Find("UI/TopBar/Setting").GetComponent<Button>();
        btn_Start = transform.Find("UI/GameStart_Btn").GetComponent<Button>();
    }

    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {
        btn_Setting.onClick.AddListener(SettingBtnClick);
        btn_Start.onClick.AddListener(StartBtnClick);
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void StartBtnClick()
    {
        UIManager.instance.OpenView("GameView");
        UIManager.instance.CloseView("MainView");
    }

    /// <summary>
    /// 打开设置页
    /// </summary>
    private void SettingBtnClick()
    {
        //todo
    }


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
        SoundManager.instance.PlayOnlyOneSound("BGM_1",(int)TrackType.BGM,true);
    }

    /// <summary>
    /// 更新UI状态
    /// </summary>
    private void InitUIState()
    {
        txt_SkillsCount.text = skillNow.ToString() + "/" + skillMax.ToString();
        txt_GoldCount.text = goldNum.ToString();
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void InitUIData()
    {
        skillNow = 100;
        skillMax = 800;
        goldNum = 1200;
        //测试用例，实际需接入获取到的玩家数据
        //todo
    }

    public override void OnClose()
    {
        //todo
    }

}
