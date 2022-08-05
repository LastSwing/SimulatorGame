using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleScript : MonoBehaviour
{
    //力度
    public float G = 1;
    //刚体
    Rigidbody2D rb;
    //角色的力
    private Vector3 vector;
    //角色的位置
    private Vector2 location = Vector2.zero;
    //补偿速度
    private float pay = 2500F;
    //UI界面
    public GameObject UI;
    //摇杆
    public GameObject Rockel;
    //对象池
    private GameObjectPool GameObjectPool;
    //使用中道具对象
    private Dictionary<string,GameObject> PropObject = new Dictionary<string, GameObject>();
    //当前道具
    private List<string> propName = new List<string>();
    //道具集类
    private LoadProp loadProp = new LoadProp();
    // 游戏开始
    private bool _start = false;
    // 使用道具
    public bool _Prop = false;
    //模拟高度空气墙
    public GameObject Resist;
    #region 道具使用变量
    private float angle = 0;//初始角度
    private int IsAngle = 0;//角度节点 0向上1向下2调整
    private bool track = true;//true执行道具轨迹false结束执行
    DateTime date = DateTime.Now;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObjectPool = transform.parent.GetComponent<MainView>().GameObjectPool;
    }

    private void FixedUpdate()
    {
        if (_start)
        {
            move(vector);
        }
        if (_Prop)
        {
            Vector2 vector = Rockel.GetComponent<RockerScript>().SmallRectVector;
            foreach (var name in propName)
            {
                switch (name)
                {
                    case "PaperPlane":
                        if (track)
                        {
                            if (angle >= 35)
                                IsAngle = 2;
                            else if (angle <= -40)
                                IsAngle = 1;
                            if (IsAngle == 0)//缓慢向上飞
                            {
                                angle += 0.6f;
                                rb.AddForce(Vector3.right * 50);
                                rb.AddForce(Vector2.up * 200);
                            }
                            else if (IsAngle == 2)//失力调整方向向下
                            {
                                rb.AddForce(Vector2.right * 50);
                                angle -= 10f;
                            }
                            else if (IsAngle == 1)//向下
                            {
                                rb.AddForce(Vector2.right * 50);
                                angle += 0.3f;
                                if (angle >= 0)
                                {
                                    track = false;
                                    angle = 0;
                                }
                            }
                            transform.rotation = Quaternion.Euler(0, 0, angle);
                            PropObject[name].transform.localRotation = transform.localRotation;
                        }
                        else
                            rb.AddForce(Vector2.right * 50);
                        break;
                    case "Property":
                        rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y * 0.9f);
                        rb.AddForce(new Vector2(vector.x > 0 ? 20 : -20, 0));
                        break;
                    case "UAV":
                        rb.AddForce(new Vector2(vector.x / 3, vector.y / 3));
                        if (vector.x == 0)
                            PropObject[name].transform.Find("propeller").GetComponent<Propeller>().FlyAngle(0);
                        else if (vector.x > 0)
                            PropObject[name].transform.Find("propeller").GetComponent<Propeller>().FlyAngle(2);
                        else if(vector.x<0)
                            PropObject[name].transform.Find("propeller").GetComponent<Propeller>().FlyAngle(1);
                        break;
                    case "Reversal":
                        float y = (transform.parent.localPosition.y * -1) + (Screen.height / 2) - 47;
                        if (transform.localPosition.y < y)
                            transform.Translate(Vector2.down);
                        else
                        {
                            transform.localPosition = new Vector2(transform.localPosition.x, y);
                            rb.velocity = new Vector2(rb.velocity.x, 0);
                        }
                        if (vector != new Vector2(0, 0))
                            rb.AddForce(new Vector2(vector.x * 2, 0));
                        break;
                    case "BigRole":
                        if ((DateTime.Now - date).TotalMilliseconds > 5000)
                            DropProp();
                        else
                            PropObject[name].GetComponent<Rigidbody2D>().AddForce(Vector2.right * 50000);
                        break;
                    case "HookRope":
                        rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y * 0.9f);
                        if (vector != new Vector2(0, 0))
                        {
                            float angle = Mathf.Asin(vector.y / (vector.x * vector.x + vector.y * vector.y)) * 100 * Mathf.PI * Mathf.Rad2Deg;
                            if (vector.y >= 0 && vector.x > 0)
                                angle = 90 - (angle / 2);
                            else if (vector.y <= 0 && vector.x > 0)
                                angle = 90 + (angle * -1 / 2);
                            else if (vector.y <= 0 && vector.x < 0)
                                angle = 180 + (angle * -1 / 2);
                            else if (vector.y >= 0 && vector.x < 0)
                                angle = 270 + (angle / 2);
                            Debug.Log(PropObject[name].transform.localRotation.z);
                            if (vector.y < 0)//处于上半部
                            {

                            }
                            else
                            { 
                                
                            }
                            if (angle >360- PropObject[name].transform.localRotation.z)
                            {
                                PropObject[name].transform.RotateAround(transform.position+ new Vector3(1,0,0), transform.forward,-1);
                            }
                            else if (angle < 360 - PropObject[name].transform.eulerAngles.z)
                            {
                                PropObject[name].transform.RotateAround(transform.position + new Vector3(1, 0, 0), transform.forward, 1);
                            }
                        }
                        break;
                }
                //if (loadProp.PropDict[name].IsMove)
                //{
                //    PropObject[name].transform.localPosition = (Vector2)transform.localPosition + loadProp.PropDict[name].Loadlocation;
                //    PropObject[name].transform.localRotation = transform.localRotation;
                //}
            }
        }
    }

    #region 道具方法
    /// <summary>
    /// 开始调用道具
    /// </summary>
    public void PropWay(string PropName)
    {
        if (_Prop == false)
        {
            _start = false;
            _Prop = true;
            CreateProp(PropName);
        }
        else
        {
            DropProp();
            _Prop = true;
            CreateProp(PropName);
        }
    }
    /// <summary>
    /// 创建道具
    /// </summary>
    /// <param name="PropName"></param>
    private void CreateProp(string PropName)
    {
        MainProp mainProp = loadProp.PropDict[PropName];
        if (!mainProp.IsBuff)//不是buff道具
        {
            if (!loadProp.PropDict[PropName].IsOverlay)
            {
                propName.Clear();
                PropObject.Clear();
                propName.Add(PropName);
            }
            else
                propName.Add(PropName);
            GameObject propObject = GameObjectPool.GetObject(Resources.Load(string.Format("Prefabs/Prop/{0}", PropName)) as GameObject, transform.parent);
            propObject.SetActive(true);
            propObject.name = PropName;
            propObject.transform.localPosition = (Vector2)transform.localPosition + mainProp.Loadlocation;
            PropObject.Add(PropName, propObject);
            if (mainProp.Mass != -1)
                rb.mass = mainProp.Mass;
            if (mainProp.AngularDrag != -1)
                rb.angularDrag = mainProp.AngularDrag;
            if (mainProp.Gravity != -1)
                rb.gravityScale = mainProp.Gravity;
            if (mainProp.IsRocker)
                IsRockel(true);
            if (mainProp.IsMove)//跟着角色移动赋予固定关节Body
                propObject.GetComponent<FixedJoint2D>().connectedBody = rb;
            if (PropName == "BigRole")//巨人
            {
                rb.gravityScale = 0;
                transform.parent.GetComponent<MainView>().Role = propObject;
                transform.localPosition = new Vector2(-9999, 0);
                date = DateTime.Now;
            }
            else if (PropName == "HookRope")//钩索
            {
                propObject.GetComponent<HingeJoint2D>().connectedBody = rb;
                Resist.transform.localPosition = new Vector2(0, (transform.parent.localPosition.y * -1) + (Screen.height / 2));
            }
        }
        else//是buff道具
        {
            if (mainProp.IsRocker)
                IsRockel(true);
            if (!loadProp.PropDict[PropName].IsOverlay)
            {
                propName.Clear();
                PropObject.Clear();
                propName.Add(PropName);
            }
            else
                propName.Add(PropName);
            if (PropName == "Reversal")
            {
                transform.parent.GetComponent<MainView>().IsDownUp = false;
                rb.gravityScale = 0;
                transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
        }
    }
    /// <summary>
    /// 结束道具使用
    /// </summary>
    public void DropProp()
    {
        _start = false;
        if (_Prop == true)
        {
            if (propName.Count == 1)
            {
                if (propName[0] == "BigRole")
                {
                    transform.parent.GetComponent<MainView>().Role = transform.gameObject;
                    transform.localPosition = PropObject[propName[0]].transform.localPosition;
                    rb.AddForce(Vector2.right * 1000, ForceMode2D.Impulse);
                }
                else if (propName[0] == "Reversal")
                {
                    transform.parent.GetComponent<MainView>().IsDownUp = true;
                }
                //if (!loadProp.PropDict[propName[0]].IsOverlay)//只有一个道具且道具不和其他叠加时，删除
                //{
                //    propName.Clear();
                //    PropObject.Clear();
                //}
            }
            foreach (var item in PropObject)
            {
                GameObjectPool.IntoPool(item.Value);
                if (loadProp.PropDict[item.Key].IsRocker)
                    IsRockel(false);
            }
            _Prop = false;
            transform.localRotation = Quaternion.Euler(0,0,0);
            rb.mass = 10;
            rb.angularDrag = 0;
            rb.gravityScale = 10;
        }
    }
    /// <summary>
    /// 摇杆操作
    /// </summary>
    /// <param name="Putout">放出收回</param>
    private void IsRockel(bool Putout)
    {
        if (Putout)
            Rockel.transform.localPosition = new Vector3(-1200, -150, 0);
        else
            Rockel.transform.localPosition = new Vector3(-5000, -250, 0);
    }

    /// <summary>
    /// 使用道具
    /// </summary>
    public void Moment()
    {
        if (_Prop && Rockel.GetComponent<RockerScript>().SmallRectVector != Vector2.zero)
        {
            for (int i = 0; i < propName.Count; i++)
            {
                if (propName[i] == "Rocket")
                {
                    Vector2 vector = Rockel.GetComponent<RockerScript>().SmallRectVector;
                    if (vector.x >= 0)
                        rb.AddForce(new Vector2(vector.x * 0.05f, vector.y * 0.03f), ForceMode2D.Impulse);
                }
            }
        }
    }
    #endregion

    #region 碰撞事件
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Pillar")
        {

        }
        else if (collision.gameObject.name == "Ground")
        {
            DropProp();
            UI.GetComponent<UIScript>().GameOver(false, 0);
        }
        else if (collision.gameObject.name == "Teleport")
        {
            _start = false;
            transform.localPosition = new Vector2(-900, 488);
            rb.velocity = new Vector2(0, 0);
        }
        else if (collision.gameObject.name == "Sea")
        {
            DropProp();
            UI.GetComponent<UIScript>().GameOver(true, 1);
        }
        else if (collision.gameObject.name == "Stone")
        {
            DropProp();
            UI.GetComponent<UIScript>().GameOver(true, 2);
        }
        else if (collision.gameObject.name == "Island")
        {
            DropProp();
            UI.GetComponent<UIScript>().GameOver(true, 3);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Pillar")
        {
        }
    }
    #endregion

    #region 一开始跳跃方法
    /// <summary>
    /// UI调用开始方法
    /// </summary>
    /// <param name="g">力度</param>
    public void Sprint(float g)
    {
        _start = true;
        G = g;
        vector = new Vector3(G * pay, -1f);
    }

    /// <summary>
    /// 一开始移动
    /// </summary>
    public void move(Vector3 vector3)
    {
        if (!_Prop)
            rb.AddForce(vector3);
    } 
    #endregion
    
}
