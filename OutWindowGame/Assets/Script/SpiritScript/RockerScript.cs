using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RockerScript : MonoBehaviour
{
    //大圆
    public RectTransform bigRect;
    //小圆
    public RectTransform smallRect;
    //滑轮被激活；默认没有
    private bool isActiveTrue = false;
    //屏幕分辨率比率；这里Canvas是根据宽来缩放的
    private float biLv;
    //用来存储鼠标和大圆的距离；是一个由大圆坐标指向鼠标坐标的方向向量
    private Vector3 dis;
    public Vector2 SmallRectVector
    { 
        get { return smallRect.localPosition; }
    }

    void Start()
    {
        //屏幕分辨率的宽除以实际屏幕宽度
        biLv = 1280f / Screen.width;
    }
    void Update()
    {
        //被激活要做的事情；控制小圆的移动
        if (isActiveTrue)
        {
            //小圆的坐标：等于相对于父物体大圆鼠标的坐标；等于大圆指向鼠标的一个向量；等于鼠标坐标减去大圆坐标；
            smallRect.anchoredPosition = Input.mousePosition * biLv - bigRect.anchoredPosition3D;
            //鼠标到大圆的距离；是一个方向指向鼠标的向量
            dis = smallRect.anchoredPosition;
            //如果鼠标到大圆的距离大于100
            if (dis.magnitude > 100)
            {
                //将这个向量转换成单位向量乘以一百就是小圆的位置了
                dis = smallRect.anchoredPosition.normalized * 100;
                smallRect.anchoredPosition = dis;
            }
        }
    }
    void RockerDownClick()
    {
        isActiveTrue = true;
    }
    void RockerOnPointerUp()
    {
        isActiveTrue = false;
        smallRect.anchoredPosition = Vector2.zero;
    }
}
