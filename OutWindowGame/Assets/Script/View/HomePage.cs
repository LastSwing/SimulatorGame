using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HomePage : BaseUI
{
    public GameObject UIList;
    public GameObject ScrollView;
    public GameObjectPool GameObjectPool;
    public Button button;
    private int LevelInterval = 575;
    private bool Mdwon = false;//鼠标按下
    private List<Level> levels;
    private float Mx = 0;//上次鼠标x位置
    /// <summary>
    /// 当前关卡
    /// </summary>
    public int LevelNum = 0;
    public override void OnInit()
    {
        //因为获取组件以及绑定事件一般只需要做一次，所以放在OnInit
        InitComponent();
        InitUIevent();
    }
    private void InitComponent()
    {

    }
    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {
        GameObjectPool = new GameObjectPool();
        button.onClick.AddListener(ButtonClick);
        levels = ReadData.GetLevels();
        Process process = ReadData.GetProcess();
        for (int i = 0; i < levels.Count; i++)
        {
            GameObject levelObject = Resources.Load("Prefabs/UI/Level") as GameObject;
            levelObject.name = "Level" +(i+1);
            levelObject.GetComponent<LevelScript>().Text.text = string.Format("第{0}关", levels[i].LevelNum);
            GameObjectPool.GetObject(levelObject, ScrollView.transform);
        }
        if (levels.Count>2)
            ScrollView.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width + (levels.Count-2)*700, 500);
        List<GameObject> list = BaseHelper.GetAllSceneObjects(ScrollView.transform, true,false,"Level");
        for (int i = 0; i < list.Count; i++)
        {
            list[i].name = list[i].name.Replace("(Clone)", string.Empty);
            list[i].transform.localPosition = new Vector2(-575 + i*LevelInterval, 0);
        }
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

    }
    void ButtonClick()
    {
        if (LevelNum == 0) return;
        UIManager.instance.OpenView("MainView");
        UIManager.instance.CloseView("HomePage");
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Mdwon = true;
            Mx = Input.mousePosition.x;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Mdwon = false;
        }
        if (Mdwon)
        {
            ScrollView.transform.localPosition = new Vector2(ScrollView.transform.localPosition.x + Input.mousePosition.x - Mx, ScrollView.transform.localPosition.y);
            Mx = Input.mousePosition.x;
        }
        else if (!Mdwon && ScrollView.transform.localPosition.x > 0)
        {
            ScrollView.transform.localPosition = new Vector2(ScrollView.transform.localPosition.x - ScrollView.transform.localPosition.x * 0.01f, ScrollView.transform.localPosition.y);
        }
        else if (!Mdwon && ScrollView.transform.localPosition.x < 0 && ScrollView.transform.GetComponent<RectTransform>().sizeDelta.x == 1920)
        {
            ScrollView.transform.localPosition = new Vector2(ScrollView.transform.localPosition.x + ScrollView.transform.localPosition.x * -1 * 0.01f, ScrollView.transform.localPosition.y);
        }
        else if (!Mdwon && ScrollView.transform.localPosition.x < 0 && ScrollView.transform.localPosition.x < 420* levels.Count/2*-1)
        {
            ScrollView.transform.localPosition = new Vector2(ScrollView.transform.localPosition.x + ScrollView.transform.localPosition.x * -1 * 0.01f, ScrollView.transform.localPosition.y);
        }
    }
    /// <summary>
    /// 更新按钮选择状态
    /// </summary>
    public void UpdateSelect(string btnname)
    {
        LevelNum = Convert.ToInt32(btnname.Replace("Level",""));
        List<GameObject> gameObjects = BaseHelper.GetAllSceneObjects(ScrollView.transform,true,false,"");
        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (gameObjects[i].name.Contains("Level") && gameObjects[i].name != btnname)
                gameObjects[i].GetComponent<LevelScript>().Conceal();
        }
    }
}
