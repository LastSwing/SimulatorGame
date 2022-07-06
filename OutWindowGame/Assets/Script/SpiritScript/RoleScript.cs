using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleScript : MonoBehaviour
{
    public GameObject UI;
    Rigidbody2D rb;
    [Header("力度")]
    public float G = 1;
    [Header("角度")]
    public float A = 1;
    //角色的力
    private Vector3 vector;
    //角色的位置
    private Vector2 location = Vector2.zero;
    //补偿速度
    private float pay = 2500F;
    private UIScript uIScript = null;

    public Vector2 Vector2 { set { location = value; } }

    /// <summary>
    /// 游戏开始
    /// </summary>
    private bool _start = false;
    /// <summary>
    /// 离开掉落平台
    /// </summary>
    private bool _Down = false;
    /// <summary>
    /// 使用道具
    /// </summary>
    public bool _Prop = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(UI != null)
            uIScript = UI.GetComponent<UIScript>();
    }

    private void FixedUpdate()
    {
        if (_start)
        {
            move(vector);
        }
        if (_Prop && location != Vector2.zero)
        {
            transform.localPosition = location;
        }
    }
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
    /// <summary>
    /// 结束道具使用
    /// </summary>
    public void DropProp()
    {
        _start = false;
        _Down = false;
        if (_Prop == true)
        {
            _Prop = false;
            rb.gravityScale = 10;
            uIScript.RrecycleProp();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Pillar")
        {
            //_Down = true;
        }
    }
    /// <summary>
    /// UI调用开始方法
    /// </summary>
    /// <param name="g">力度</param>
    public void Sprint(float g)
    {
        _start = true;
        G = g;
        vector = new Vector3(G * pay,-1f);
    }
    /// <summary>
    /// 主页面调用道具方法
    /// </summary>
    public void PropWay()
    {
        rb.velocity = new Vector2(0, 0);
        rb.gravityScale = 0;
        _start = false;
        _Prop = true;
    }
    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        transform.localPosition=new Vector2(-900,375);
        _Down = false;
        _Prop = false;
    }

    /// <summary>
    /// 移动
    /// </summary>
    public void move(Vector3 vector3)
    {
        if(!_Down || _Prop)
        rb.AddForce(vector3);
    }

    
}
