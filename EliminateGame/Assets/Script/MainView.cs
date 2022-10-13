using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainView : BaseUI
{
    public Button StartBtn;
    public Button RankBtn;
    public Button HelpBtn;
    public override void OnClose()
    {
    }

    public override void OnInit()
    {
        StartBtn.onClick.AddListener(StartBtnClick);
        RankBtn.onClick.AddListener(RankBtnClick);
        HelpBtn.onClick.AddListener(HelpBtnClick);
    }

    public override void OnOpen()
    {

    }
    void StartBtnClick()
    {
        UIManager.instance.OpenView("GamePage");
        UIManager.instance.CloseView("MainView");
    }

    void RankBtnClick()
    { 
    
    }

    void HelpBtnClick()
    { 
    
    }
}
