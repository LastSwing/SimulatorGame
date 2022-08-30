using PlayfulSystems.ProgressBar;
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
    public GameObject Hp = null;//血条
    public Text HpText = null;
    private GameObjectPool GameObjectPool;
    GameObject ProgressBar = null;//力度条
    public string m_PropName = "";

    public GameObject Result;//结果
    public Image image;
    public Button anew;
    public Button next;
    public Button Return;

    public GameObject Bleed;
    private int BleedFps = 15;
    private bool IsBleed = false;//掉血动画
    public string PropName 
    {
        get { return m_PropName; }
        set { m_PropName = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        Hp.GetComponent<ProgressBarPro>().Update = false;
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
        Role.GetComponent<RoleScript>().Restart();
        Damage(100);
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
        //if (Input.GetKey(KeyCode.F) && m_PropName == "Rocket")
        //{
        //    SettingBtnClick();
        //}
    }

    private void FixedUpdate()
    {
        if (BleedFps == 0)
        {
            IsBleed = false;
            BleedFps = 15;
            Bleed.SetActive(false);
        }
        //掉血动画开始
        if (IsBleed)
        {
            BleedFps--;
            Bleed.SetActive(true);
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
            image.sprite = BaseHelper.LoadFromImage(new Vector2(440, 170), Application.dataPath + String.Format(@"\Resources\Image\UI\{0}x.png", Stars));
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
            image.sprite = BaseHelper.LoadFromImage(new Vector2(440, 170), Application.dataPath + @"\Resources\Image\UI\0x.png");
        }
    }

    /// <summary>
    /// 血量赋值
    /// </summary>
    /// <param name="hp"></param>
    public void Damage(int hp)
    {
        if(hp == 100)//解决重置血量时不变色的bug
            Hp.GetComponent<ProgressBarPro>().Value = 0.01f;
        float value = hp *0.01f;
        Hp.GetComponent<ProgressBarPro>().Value = value;
        HpText.text = "血量："+ hp.ToString();
        if (hp == 0)
            GameOver(false, 0);
    }
    public void OutBleed()
    {
        IsBleed = true;
    }
    #region 按钮方法
    /// <summary>
    /// 道具
    /// </summary>
    private void PropBtnClick()
    {
        if (Role.GetComponent<RoleScript>().leave)
        {
            if (Cut.GetComponent<CutScript>().Consume())
            {
                Role.GetComponent<RoleScript>().PropWay(m_PropName);
            }
        }
    }
    /// <summary>
    /// 开始按钮按下
    /// </summary>
    private void StartBtnDownClick()
    {
        if (Role.GetComponent<RoleScript>()._start) return;
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
        ParentTransform.GetComponent<MainView>().IsDownUp = true;
        ParentTransform.GetComponent<MainView>().IsMove = true;
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
