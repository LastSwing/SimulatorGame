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
    private Vector2 screen;//屏幕分辨率
    /// <summary>
    /// 宽度移动速度
    /// </summary>
    private float WidthMSpeed = 5;
    /// <summary>
    /// 高度移动速度
    /// </summary>
    private float HeightMSpeed =5;
    /// <summary>
    /// 是否左右移动
    /// </summary>
    private bool IsMove = false;
    /// <summary>
    /// 移动宽度
    /// </summary>
    private float MobileWidth = 0;
    /// <summary>
    /// 移动高度
    /// </summary>
    private float MobileHeight = 0;
    /// <summary>
    ///移动帧速
    /// </summary>
    private float Speed = 0;
    /// <summary>
    /// 每帧移动宽度
    /// </summary>
    private float FrameWidth = 0;
    /// <summary>
    /// 每帧移动高度
    /// </summary>
    private float FrameHeight = 0;
    /// <summary>
    /// 每帧移动加速宽度
    /// </summary>
    private float FrameWidthSpeed = 5;
    /// <summary>
    /// 每帧移动加速高度
    /// </summary>
    private float FrameHeightSpeed = 5;

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
    }
    /*void FixedUpdate()
    {
        //让屏幕和UI跟着角色向右移动
        if (Role.transform.localPosition.x > (transform.localPosition.x * -1) + MobileWidth)
        {
            if (transform.localPosition.x * -1 >= (GetComponent<RectTransform>().rect.width / 2 - screen.x / 2))//已经到尽头
            {
                MobileWidth = 0;
                transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width / 2 - screen.x / 2) * -1, transform.localPosition.y);
                UIList.transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width / 2) - (UIList.GetComponent<RectTransform>().rect.width / 2), UIList.transform.localPosition.y);
            }
            else//角色在屏幕右边，添加屏幕宽度
            {
                IsMove = true;
                Speed = 20;
                MobileWidth += FrameWidthSpeed * WidthMSpeed;
                FrameWidth = MobileWidth / Speed;
            }
        }
        else
        {
            MobileWidth -= FrameWidth;
            if (Role.transform.localPosition.x < (transform.localPosition.x * -1) - GetComponent<RectTransform>().rect.width / 1.8)
            {
                IsMove = true;
                Speed = 20;
                MobileWidth += FrameWidthSpeed * WidthMSpeed;
                FrameWidth = MobileWidth / Speed;
            }
        }

        //让屏幕和UI跟着角色上下移动
        if (Role.transform.localPosition.y > (transform.localPosition.y * -1) + MobileHeight)
        {

        }

        //移动操作
        if (IsMove)
        {
            if (MobileWidth > 0)
            {
                transform.localPosition = new Vector2(transform.localPosition.x - MobileWidth / Speed, transform.localPosition.y);
                UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x + MobileWidth / Speed, UIList.transform.localPosition.y);
            }
            else if (MobileWidth < 0)
            {
                transform.localPosition = new Vector2(transform.localPosition.x + MobileWidth / Speed, transform.localPosition.y);
                UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x - MobileWidth / Speed, UIList.transform.localPosition.y);
            }
            if (MobileHeight > 0)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - MobileHeight / Speed);
                UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x, UIList.transform.localPosition.y + MobileHeight / Speed);
            }
            Speed--;
            if (Speed <= 0)
            {
                IsMove = false;
            }
        }
    }
    */
    void FixedUpdate()
    {
        if (Role.transform.localPosition.x > (transform.localPosition.x * -1))//角色处于右半部
        {
            float move = transform.localPosition.x - (Role.transform.localPosition.x * -1);
            transform.localPosition = new Vector2(transform.localPosition.x - move, transform.localPosition.y);
            UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x + move, UIList.transform.localPosition.y);
        }
        //else if (Role.transform.localPosition.x < (transform.localPosition.x * -1))
        //{
        //    float move = (Role.transform.localPosition.y - transform.localPosition.x * -1);
        //    transform.localPosition = new Vector2(transform.localPosition.x + move, transform.localPosition.y );
        //    UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x - move, UIList.transform.localPosition.y);
        //}
        float a =(transform.localPosition.y * -1 + screen.y*0.25f);
        float b = transform.localPosition.y * -1;//-screen.y*0.25f
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
        if (transform.localPosition.x * -1 >= (GetComponent<RectTransform>().rect.width / 2 - screen.x / 2))//已经到右尽头
        {
            transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width / 2 - screen.x / 2) * -1, transform.localPosition.y);
            UIList.transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width * -1 / 2) + screen.x - (UIList.GetComponent<RectTransform>().rect.width / 2) * -1, UIList.transform.localPosition.y);
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
    #region 其他脚本调用方法
    /// <summary>
    /// 视角移动速度给予
    /// </summary>
    /// <param name="WidthMSpeed">移动宽度速度 0-10</param>
    ///  <param name="HeightMSpeed">移动高度速度 0-10</param>
    public void SceenSpeed(float widthMSpeed, float heightMSpeed)
    {
        WidthMSpeed = widthMSpeed;
        HeightMSpeed = heightMSpeed;
    }
    #endregion
    public override void OnClose()
    {
        //todo
    }
}
