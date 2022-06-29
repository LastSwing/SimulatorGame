using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleScript : MonoBehaviour
{
    Rigidbody2D rb;
    [Header("力度")]
    public float G = 1;
    [Header("角度")]
    public float A = 1;
    //角色的力
    private Vector3 vector;
    //补偿速度
    private float pay = 4500F;

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
    private bool _Prop = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_start)
        {
            move(vector);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Pillar")
        {
        }
        else if (collision.gameObject.name == "right" || collision.gameObject.name == "Land" || collision.gameObject.name == "Buffer" || collision.gameObject.name == "Top")
        {
            _start = false;
            _Down = false;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Pillar")
        {
            _Down = true;
        }
    }
    /// <summary>
    /// 主页面调用开始方法
    /// </summary>
    /// <param name="g">力度</param>
    void Sprint(float g)
    {
        _start = true;
        G = g;
        vector = Vector3.right * G * pay;
    }
    /// <summary>
    /// 主页面调用道具方法
    /// </summary>
    /// <param name="angle">角度</param>
    void PropWay(float angle)
    {
        _start = true;
        _Prop = true;
        vector = Movement.Angle(pay,angle,0.5F);
    }
    /// <summary>
    /// 重置
    /// </summary>
    void Reset()
    {
        transform.localPosition=new Vector2(-900,384);
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
