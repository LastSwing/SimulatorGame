using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainView : BaseUI
{
    public Button PropBtn;
    public Button ResetBtn;
    public Text AngleText;
    private GameObjectPool GameObjectPool;
    GameObject ProgressBar = null;

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
        ResetBtn.onClick.AddListener(SettingBtnClick);
        PropBtn.onClick.AddListener(PropBtnClick);
    }

    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {
        GameObjectPool = new GameObjectPool();
    }

    /// <summary>
    /// 道具
    /// </summary>
    private void PropBtnClick()
    {
        float angle = float.Parse(AngleText.text);
        BroadcastMessage("PropWay", angle);
    }
    /// <summary>
    /// 开始按钮按下
    /// </summary>
    private void StartBtnDownClick()
    {
        if (ProgressBar == null)
        {
            ProgressBar = Resources.Load("Prefabs/HorizontalBoxGradient") as GameObject;
            GameObjectPool.GetObject(ProgressBar, transform);
            GameObject gameObject1 = transform.Find("HorizontalBoxGradient(Clone)").gameObject;
            gameObject1.name = "HorizontalBoxGradient";
        }
        else
        {
            ProgressBar = transform.Find("HorizontalBoxGradient").gameObject;
            ProgressBar.GetComponent<ProgressBarPro>().Value = 0f;
            GameObjectPool.GetObject(ProgressBar, transform);
            ProgressBar.SetActive(true);
        }
    }
    /// <summary>
    /// 开始按钮抬起
    /// </summary>
    private void StartBtnUpClick()
    {
        GameObject gameObject = transform.Find("HorizontalBoxGradient").gameObject;
        float G = gameObject.GetComponent<ProgressBarPro>().Value;
        GameObjectPool.IntoPool(gameObject);
        BroadcastMessage("Sprint", G);
    }
    /// <summary>
    /// 重置
    /// </summary>
    private void SettingBtnClick()
    {
        BroadcastMessage("Reset");
    }


    public override void OnOpen()
    {
        //数据需要每次打开都要刷新，UI状态也是要每次打开都进行刷新，因此放在OnOpen
        InitUIData();
        InitUIState();
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

        //测试用例，实际需接入获取到的玩家数据
        //todo
    }

    public override void OnClose()
    {
        //todo
    }
}
