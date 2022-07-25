using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainView : BaseUI
{
    Image img_Background;//背景图片
    Text txt_SkillsCount, txt_ReturnView, txt_SettingHasBtn, txt_ReturnView1;//图鉴收集；返回按钮所跳页面；是否显示设置页面按钮0否，1是
    Text txt_GoldCount;//金币数量
    Button btn_Setting;//设置按钮
    Button btn_Start;//开始按钮
    Button btn_Skills;//图鉴按钮

    int skillNow, skillMax;
    int goldNum;

    List<CurrentCardPoolModel> GlobalCardPools = new List<CurrentCardPoolModel>();//游戏全局卡池
    List<CurrentCardPoolModel> GlobalPlayerCardPools = new List<CurrentCardPoolModel>();//角色全局卡池
    List<CurrentRoleModel> GlobalRolePools = new List<CurrentRoleModel>();
    CurrentRoleModel CurrentRole = new CurrentRoleModel();
    GlobalPlayerModel GlobalRole;
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
        txt_ReturnView = GameObject.Find("MainCanvas/txt_ReturnView").GetComponent<Text>();
        txt_ReturnView1 = GameObject.Find("MainCanvas/txt_ReturnView1").GetComponent<Text>();
        txt_SettingHasBtn = GameObject.Find("MainCanvas/txt_SettingHasBtn").GetComponent<Text>();
        txt_SkillsCount = transform.Find("UI/TopBar/Skills/txt_SkillsCount").GetComponent<Text>();
        txt_GoldCount = transform.Find("UI/TopBar/Gold/txt_GoldCount").GetComponent<Text>();
        btn_Setting = transform.Find("UI/TopBar/Setting").GetComponent<Button>();
        btn_Start = transform.Find("UI/GameStart_Btn").GetComponent<Button>();
        btn_Skills = transform.Find("UI/TopBar/Skills").GetComponent<Button>();
    }

    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {
        btn_Setting.onClick.AddListener(SettingBtnClick);
        btn_Start.onClick.AddListener(StartBtnClick);
        btn_Skills.onClick.AddListener(SkillBtnClick);
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void StartBtnClick()
    {
        UIManager.instance.OpenView("MapView");
        UIManager.instance.CloseView("MainView");
    }

    /// <summary>
    /// 打开设置页
    /// </summary>
    private void SettingBtnClick()
    {
        txt_ReturnView.text = "MainView";
        txt_ReturnView1.text = "MainView";
        txt_SettingHasBtn.text = "0";
        UIManager.instance.OpenView("SettingView");
        UIManager.instance.CloseView("MainView");
    }

    /// <summary>
    /// 打开图鉴页
    /// </summary>
    private void SkillBtnClick()
    {
        txt_ReturnView.text = "MainView";
        UIManager.instance.OpenView("AllSkillView");
        UIManager.instance.CloseView("MainView");
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
        txt_SkillsCount.text = skillNow.ToString() + "/" + skillMax.ToString();
        txt_GoldCount.text = goldNum.ToString();
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void InitUIData()
    {
        //测试用例，实际需接入获取到的玩家数据
        #region 数据初始化

        GlobalRolePools = Common.GetTxtFileToList<CurrentRoleModel>(GlobalAttr.GlobalPlayerRolePoolFileName);
        GlobalCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalCardPoolFileName);
        GlobalPlayerCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName);

        GlobalRole = Common.GetTxtFileToModel<GlobalPlayerModel>(GlobalAttr.GlobalRoleFileName);
        if (GlobalRole == null)
        {
            GlobalRole = new GlobalPlayerModel();
            GlobalRole.ID = 1001;
            GlobalRole.Wealth = 100;
            GlobalRole.RoleList = "2022042716410125|玩家1;2022042811152451|玩家2;";
            GlobalRole.SkillList = "1001|木盾;1001|木盾;1002|冲拳;1002|冲拳;1003|飞腿;1004|金光手;1005|琉光掌;1006|土菇花;1007|土梨果;1007|土梨果;";
            GlobalRole.CurrentRoleID = 1001;
            Common.SaveTxtFile(GlobalRole.ObjectToJson(), GlobalAttr.GlobalRoleFileName);

        }
        CurrentRole = GlobalRolePools.Find(a => a.RoleID == GlobalRole.CurrentRoleID);
        #endregion
        skillNow = GlobalPlayerCardPools.Count;
        skillMax = GlobalCardPools.Count;
        goldNum = Convert.ToInt32(GlobalRole.Wealth);

    }

    #endregion
    public override void OnClose()
    {
        //todo
    }

}
