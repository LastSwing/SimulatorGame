using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class UIScript : MonoBehaviour
{
    public Button PropBtn;
    public Button ResetBtn;
    public Transform ParentTransform;//MainView
    public GameObject Role;//角色
    public GameObject Cut;//道具条
    private GameObjectPool GameObjectPool;
    GameObject ProgressBar = null;//力度条
    public string m_PropName = "";

    public GameObject Result;//结果
    public Image image;
    public Button anew;
    public Button next;
    public Button Return;
    public string PropName
    {
        get { return m_PropName; }
        set { m_PropName = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObjectPool = ParentTransform.GetComponent<MainView>().GameObjectPool;
        ResetBtn.onClick.AddListener(SettingBtnClick);
        PropBtn.onClick.AddListener(PropBtnClick);
        anew.onClick.AddListener(anewClick);
        next.onClick.AddListener(nextClick);
        Return.onClick.AddListener(ReturnClick);
    }
    //重玩本关
    private void anewClick()
    {
        
        Result.SetActive(false);
        Cut.GetComponent<CutScript>().Restart();
        ParentTransform.GetComponent<MainView>().Restart();
    }
    //下一关
    private void nextClick()
    {
        anewClick();
    }
    //返回主菜单
    private void ReturnClick()
    {
        anewClick();
        UIManager.instance.OpenView("HomePage");
        UIManager.instance.CloseView("MainView");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F) && m_PropName == "Rocket")
        {
            SettingBtnClick();
        }
    }
    /// <summary>
    /// 游戏结束
    /// </summary>
    /// <param name="Success">胜负</param>
    /// <param name="Stars">星级</param>
    public void GameOver(bool Success,int Stars)
    {
        Result.SetActive(true);
        List<GameObject> gameObjects = BaseHelper.GetAllSceneObjects(Result.transform, false, false,"");
        if (Success)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].name == "VictoryText" || gameObjects[i].name == "anew" || gameObjects[i].name == "next" || gameObjects[i].name == "Image")
                    gameObjects[i].SetActive(true);
                else
                    gameObjects[i].SetActive(false);
            }
            image.sprite = BaseHelper.LoadFromImage(new Vector2(440, 170), Application.dataPath + String.Format(@"\Resources\Image\{0}x.png", Stars));
        }
        else
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].name == "ErrorText" || gameObjects[i].name == "anew" || gameObjects[i].name == "return" || gameObjects[i].name == "Image")
                    gameObjects[i].SetActive(true);
                else
                    gameObjects[i].SetActive(false);
            }
            image.sprite = BaseHelper.LoadFromImage(new Vector2(440, 170), Application.dataPath + @"\Resources\Image\0x.png");
        }
    }
    
    #region 按钮方法
    /// <summary>
    /// 道具
    /// </summary>
    private void PropBtnClick()
    {
        if (Cut.GetComponent<CutScript>().Consume())
        {
            Role.GetComponent<RoleScript>().PropWay(m_PropName);
        }
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
            ProgressBar.SetActive(true);
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
        Role.GetComponent<RoleScript>().Sprint(G);
    }
    /// <summary>
    /// 使用
    /// </summary>
    private void SettingBtnClick()
    {
        if (m_PropName == "Rocket")
        {
            Role.GetComponent<RoleScript>().Moment();
        }
    }
    #endregion
}
