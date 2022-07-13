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
    #region 道具使用变量
    private float angle = 0;//初始角度
    private int IsAngle = 0;//角度节点 0向上1向下2调整
    private bool track = true;//true执行道具轨迹false结束执行
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
                                transform.Translate(new Vector3(1, 1));
                                angle += 0.6f;
                                rb.AddForce(Vector3.right * G);
                            }
                            else if (IsAngle == 2)//失力调整方向向下
                            {
                                transform.Translate(new Vector3(1, 0));
                                angle -= 5f;
                            }
                            else if (IsAngle == 1)//向下
                            {
                                transform.Translate(new Vector3(1, 0));
                                angle += 0.3f;
                                if (angle >= 0)
                                {
                                    track = false;
                                    angle = 0;
                                }
                            }
                            transform.rotation = Quaternion.Euler(0, 0, angle);
                        }
                        else
                            transform.Translate(new Vector3(1, 0));
                        break;
                    case "Property":
                        rb.AddForce(new Vector2(vector.x > 0 ? 2 : -2, 0));
                        break;
                    case "UAV":
                        rb.AddForce(new Vector2(vector.x / 5, vector.y / 5));
                        break;
                }
                PropObject[name].transform.localPosition = (Vector2)transform.localPosition + loadProp.PropDict[name].Loadlocation;
                PropObject[name].transform.localRotation = transform.localRotation;
            }
        }
    }

    #region 道具方法
    /// <summary>
    /// 开始调用道具
    /// </summary>
    public void PropWay(string PropName)
    {
        _start = false;
        if (loadProp.PropDict[PropName].IsOverlay)
        {
            propName.Clear();
            propName.Add(PropName);
        }
        else
            propName.Add(PropName);
        GameObject propObject = GameObjectPool.GetObject(Resources.Load(string.Format("Prefabs/Prop/{0}", PropName)) as GameObject, transform.parent);
        propObject.name = PropName;
        propObject.transform.localPosition = (Vector2)transform.localPosition + loadProp.PropDict[PropName].Loadlocation;
        PropObject.Add(PropName, propObject);
        if (loadProp.PropDict[PropName].IsRocker)
            IsRockel(true);
        Rigidbody2D rigidbody = propObject.GetComponent<Rigidbody2D>();
        if (!loadProp.PropDict[PropName].IsOverlay)
        {
            //同步道具和角色的质量，角阻力，重力
            rb.mass = rigidbody.mass;
            rb.angularDrag = rigidbody.angularDrag;
            rb.gravityScale = rigidbody.gravityScale;
        }
        _Prop = true;
    }
    /// <summary>
    /// 结束道具使用
    /// </summary>
    public void DropProp()
    {
        _start = false;
        if (_Prop == true)
        {
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
                        rb.AddForce(new Vector2(vector.x * 0.02f, vector.y * 0.03f), ForceMode2D.Impulse);
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
        }
        else if (collision.gameObject.name == "Teleport")
        {
            _start = false;
            transform.localPosition = new Vector2(-900, 488);
            rb.velocity = new Vector2(0, 0);
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
