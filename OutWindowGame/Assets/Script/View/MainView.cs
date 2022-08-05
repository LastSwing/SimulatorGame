using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainView : BaseUI
{
    public GameObject UIList;
    public GameObjectPool GameObjectPool;
    public GameObject Role;
    public int SceneID = 0;
    private Vector2 screen;//屏幕分辨率
    /// <summary>
    /// 是否左右移动
    /// </summary>
    public bool IsMove = true;
    /// <summary>
    /// 是否上下移动
    /// </summary>
    public bool IsDownUp = true;
    /// <summary>
    /// 当前关卡
    /// </summary>
    public int LevelNum = 0;
    private List<LevelDetail> levelDetails = new List<LevelDetail>();
    public override void OnInit()
    {
        //因为获取组件以及绑定事件一般只需要做一次，所以放在OnInit
        InitComponent();
        InitUIevent();
        screen = new Vector2(Screen.width,Screen.height);
        //背景位置根据分辨率改变
        transform.localPosition=new Vector2(GetComponent<RectTransform>().rect.width/ 2 - screen.x/2, GetComponent<RectTransform>().rect.height / 2 - screen.y / 2);
        //按钮位置根据分辨率改变
        UIList.transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width * -1 / 2) + screen.x - (UIList.GetComponent<RectTransform>().rect.width / 2), UIList.transform.localPosition.y);
        LevelNum = GameObject.Find("HomePage(Clone)").GetComponent<HomePage>().LevelNum;
        if (LevelNum != 0)
        { 
            List<Level> levels = ReadData.GetLevels();
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].LevelNum == LevelNum)
                {
                    transform.GetComponent<RectTransform>().sizeDelta = levels[i].Size;
                    levelDetails = ReadData.GetLevelDetail(LevelNum);
                    for (int j = 0; j < levelDetails.Count; j++)
                    {
                        if (levelDetails[j].DetailName != "Role")
                        {
                            GameObject gameObject = Resources.Load(@"Prefabs\Barrier\" + levelDetails[j].DetailName) as GameObject;
                            gameObject.transform.localPosition = levelDetails[j].Location;
                            gameObject.transform.localEulerAngles = levelDetails[j].Rotation;
                            GameObjectPool.GetObject(gameObject, transform);
                            GameObject game = transform.Find(levelDetails[j].DetailName + "(Clone)").gameObject;
                            game.name = levelDetails[j].DetailName;
                        }
                        else if (levelDetails[j].DetailName == "Role")
                        {
                            Role.transform.localPosition = levelDetails[j].Location;
                        }
                    }
                }
            }
        }
    }
    void FixedUpdate()
    {
        if (IsMove)
        {
            if (Role.transform.localPosition.x > (transform.localPosition.x * -1))//角色处于右半部
            {
                float move = transform.localPosition.x - (Role.transform.localPosition.x * -1);
                transform.localPosition = new Vector2(transform.localPosition.x - move, transform.localPosition.y);
                UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x + move, UIList.transform.localPosition.y);
            }
        }
        if (IsDownUp)
        {
            float a = (transform.localPosition.y * -1 + screen.y * 0.25f);
            float b = transform.localPosition.y * -1;
            if (Role.transform.localPosition.y > a)//角色处于上半部四分之一
            {
                float move = (Role.transform.localPosition.y - a);
                transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - move);
                UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x, UIList.transform.localPosition.y + move);
            }
            else if (Role.transform.localPosition.y < b)//角色处于下半部
            {
                float move = (Role.transform.localPosition.y - b);
                transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - move);
                UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x, UIList.transform.localPosition.y + move);
            }
        }
        if (transform.localPosition.x * -1 >= (GetComponent<RectTransform>().rect.width / 2 - screen.x / 2))//已经到右尽头
        {
            IsMove = false;
            //transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width / 2 - screen.x / 2) * -1, transform.localPosition.y);
            //UIList.transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width * -1 / 2) + screen.x - (UIList.GetComponent<RectTransform>().rect.width / 2) * -1, UIList.transform.localPosition.y);
        }
        else if (transform.localPosition.x >= (GetComponent<RectTransform>().rect.width / 2 - screen.x / 2))//已经到左尽头
        {
            transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width / 2 - screen.x / 2), transform.localPosition.y);
            UIList.transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width * -1 / 2) + screen.x - (UIList.GetComponent<RectTransform>().rect.width / 2), UIList.transform.localPosition.y);
        }
        if (transform.localPosition.y <= (GetComponent<RectTransform>().rect.width / 2 - screen.x / 2) * -1)//已经到上尽头
        {
            transform.localPosition = new Vector2(transform.localPosition.x, (GetComponent<RectTransform>().rect.height / 2 - screen.y / 2) * -1);
            UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x, (GetComponent<RectTransform>().rect.height / 2 - UIList.GetComponent<RectTransform>().rect.height / 2));
        }
        else if (transform.localPosition.y >= (GetComponent<RectTransform>().rect.height / 2 - screen.y / 2))//已经到下尽头
        {
            transform.localPosition = new Vector2(transform.localPosition.x, GetComponent<RectTransform>().rect.height / 2 - screen.y / 2);
            UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x, (GetComponent<RectTransform>().rect.height / 2 - UIList.GetComponent<RectTransform>().rect.height / 2) * -1);
        }
    }
    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitComponent()
    {
        UIList.transform.localPosition = new Vector2(Screen.width / 2 - UIList.transform.localPosition.x / 2, UIList.transform.localPosition.y);
    }
    /// <summary>
    /// 重新开始
    /// </summary>
    public void Restart()
    {
        screen = new Vector2(Screen.width, Screen.height);
        //背景位置根据分辨率改变
        transform.localPosition = new Vector2(GetComponent<RectTransform>().rect.width / 2 - screen.x / 2, GetComponent<RectTransform>().rect.height / 2 - screen.y / 2);
        //按钮位置根据分辨率改变
        UIList.transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width * -1 / 2) + screen.x - (UIList.GetComponent<RectTransform>().rect.width / 2), UIList.transform.localPosition.y);
        UIList.transform.localPosition = new Vector2(Screen.width / 2 - UIList.transform.localPosition.x / 2, UIList.transform.localPosition.y);
        foreach (var item in levelDetails)
        {
            if (item.DetailName == "Role")
            {
                Role.transform.localPosition = item.Location;
            }
        }
    }
    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {
        GameObjectPool = new GameObjectPool();
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
