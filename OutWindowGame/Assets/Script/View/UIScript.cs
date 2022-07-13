using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public Button PropBtn;
    public Button ResetBtn;
    public Transform ParentTransform;//MainView
    public GameObject Role;//角色
    private GameObjectPool GameObjectPool;
    GameObject ProgressBar = null;//力度条
    private string m_PropName = "Rocket";
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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F) && m_PropName == "Rocket")
        {
            SettingBtnClick();
        }
    }

    #region 按钮方法
    /// <summary>
    /// 道具
    /// </summary>
    private void PropBtnClick()
    {
        Role.GetComponent<RoleScript>().PropWay(m_PropName);
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
