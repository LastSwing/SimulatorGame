﻿using Assets.Scripts.LogicalScripts.Models;
using Assets.Scripts.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HomeScene : MonoBehaviour
{
    Text txt_SkillsCount, txt_GoldCount;
    Button GameStart_Btn;
    GameObject Skills_Obj;
    Image Player;
    List<CurrentCardPoolModel> GlobalCardPools = new List<CurrentCardPoolModel>();//游戏全局卡池
    List<CurrentCardPoolModel> GlobalPlayerCardPools = new List<CurrentCardPoolModel>();//角色全局卡池
    List<CurrentRoleModel> GlobalRolePools = new List<CurrentRoleModel>();
    CurrentRoleModel CurrentRole = new CurrentRoleModel();
    GlobalPlayerModel GlobalRole;
    void Start()
    {
        #region 控件初始化
        txt_SkillsCount = transform.Find("TopBar/Skills/txt_SkillsCount").GetComponent<Text>();
        txt_GoldCount = transform.Find("TopBar/Gold/txt_GoldCount").GetComponent<Text>();
        GameStart_Btn = transform.Find("GameStart_Btn").GetComponent<Button>();
        Skills_Obj = transform.Find("TopBar/Skills").gameObject;
        Player = transform.Find("Player/Image").GetComponent<Image>();
        #endregion

        GameStart_Btn.onClick.AddListener(delegate { ClickGameStartBtn(); });

        #region 绑定点击图鉴
        EventTrigger trigger1 = Skills_Obj.GetComponent<EventTrigger>();
        if (trigger1 == null)
        {
            trigger1 = Skills_Obj.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.callback.AddListener(delegate { ClickSkills(); });
        trigger1.triggers.Add(entry1);
        #endregion

        Init();
    }

    //数据初始化
    void Init()
    {
        #region 数据初始化

        GlobalRolePools = Common.GetTxtFileToList<CurrentRoleModel>(GlobalAttr.GlobalPlayerRolePoolFileName);
        GlobalCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalCardPoolFileName);
        GlobalPlayerCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName);

        GlobalRole = Common.GetTxtFileToModel<GlobalPlayerModel>(GlobalAttr.GlobalRoleFileName);
        if (GlobalRole == null)
        {
            GlobalRole = new GlobalPlayerModel();
            GlobalRole.ID = DateTime.Now.ToString("yyyyMMddHHmmss");
            GlobalRole.Wealth = 100;
            GlobalRole.RoleList = "2022042716410125|玩家1;2022042811152451|玩家2;";
            GlobalRole.SkillList = "2022042516365099|木盾;2022042516475185|冲拳;2022042516505207|飞腿;2022042516535189|金光手;2022042516543430|琉光掌;2022042516590925|土菇花;2022042516594601|土梨果;";
            GlobalRole.CurrentRoleID = "2022042716410125";
            Common.SaveTxtFile(GlobalRole.ObjectToJson(), GlobalAttr.GlobalRoleFileName);

        }
        CurrentRole = GlobalRolePools.Find(a => a.RoleID == GlobalRole.CurrentRoleID);
        #endregion
        txt_SkillsCount.text = $"{GlobalPlayerCardPools.Count}/{GlobalCardPools?.Count}";
        txt_GoldCount.text = GlobalRole.Wealth.ToString();
        Common.ImageBind(CurrentRole.RoleImgUrl, Player);
    }

    public void ClickGameStartBtn()
    {
        Debug.Log("点击游戏开始按钮");
    }

    public void ClickSkills()
    {
        Debug.Log("点击图鉴");
    }
}
