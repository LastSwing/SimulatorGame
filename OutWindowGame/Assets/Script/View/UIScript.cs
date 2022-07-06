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
        if (Input.GetKey(KeyCode.F))
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
        //float angle = float.Parse(AngleText.text);
        //BroadcastMessage("PropWay", angle);
        if (m_PropName == "Property")//降落伞
        {
            //将摇杆放出
            GameObject gameObject = transform.Find("Rockel").gameObject;
            gameObject.transform.localPosition = new Vector3(-1200, -150, 0);
            //获取角色位置，设置道具到角色的指定位置，然后根据道具的运动轨迹慢慢下落
            GameObject Prop = Resources.Load("Prefabs/Prop/Property") as GameObject;
            PropertyScript propertyScript = Prop.GetComponent<PropertyScript>();
            Prop.transform.position = (Vector2)Role.transform.localPosition + propertyScript.Vector;
            GameObjectPool.GetObject(Prop, ParentTransform);
            GameObject gameObject2 = ParentTransform.Find("Property(Clone)").gameObject;
            gameObject2.name = "Property";
            Role.GetComponent<RoleScript>().PropWay();
        }
        else if (m_PropName == "PaperPlane")//纸飞机
        {
            GameObject Prop = Resources.Load("Prefabs/Prop/PaperPlane") as GameObject;
            PaperPlaneScript PaperPlane = Prop.GetComponent<PaperPlaneScript>();
            Prop.transform.position = (Vector2)Role.transform.localPosition + PaperPlane.Vector;
            GameObjectPool.GetObject(Prop, ParentTransform);
            GameObject gameObject2 = ParentTransform.Find("PaperPlane(Clone)").gameObject;
            gameObject2.name = "PaperPlane";
            Role.GetComponent<RoleScript>().PropWay();
        }
        else if (m_PropName == "Rocket")//推进器
        {
            //将摇杆放出
            GameObject gameObject = transform.Find("Rockel").gameObject;
            gameObject.transform.localPosition = new Vector3(-1200, -150, 0);
            GameObject Prop = Resources.Load("Prefabs/Prop/Rocket") as GameObject;
            RocketScript Rocket = Prop.GetComponent<RocketScript>();
            Prop.transform.position = (Vector2)Role.transform.localPosition + Rocket.Vector;
            GameObjectPool.GetObject(Prop, ParentTransform);
            GameObject gameObject2 = ParentTransform.Find("Rocket(Clone)").gameObject;
            gameObject2.name = "Rocket";
            Role.GetComponent<RoleScript>().PropWay();
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
            GameObject.Find("Rocket").GetComponent<RocketScript>().Moment();
        }
    }
    #endregion

    #region 其他脚本调用方法
    /// <summary>
    /// 回收道具
    /// </summary>
    public void RrecycleProp()
    {
        if (m_PropName == "Property")
        {
            GameObject gameObject = transform.Find("Rockel").gameObject;
            gameObject.transform.localPosition = new Vector3(-5000, -250, 0);
            GameObject gameObject1 = ParentTransform.Find("Property").gameObject;
            GameObjectPool.IntoPool(gameObject1);
        }
        else if (m_PropName == "PaperPlane")
        {
            GameObject gameObject1 = ParentTransform.Find("PaperPlane").gameObject;
            GameObjectPool.IntoPool(gameObject1);
        }
        else if (m_PropName == "Rocket")
        {
            GameObject gameObject = transform.Find("Rockel").gameObject;
            gameObject.transform.localPosition = new Vector3(-5000, -250, 0);
            GameObject gameObject1 = ParentTransform.Find("Rocket").gameObject;
            GameObjectPool.IntoPool(gameObject1);
        }
    }
    #endregion
}
