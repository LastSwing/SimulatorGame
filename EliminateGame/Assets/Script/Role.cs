using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Role : MonoBehaviour, IDragHandler, IEndDragHandler
{
    //当前位置
    public int Location
    {
        get { return _location; }
        set
        {
            _location = value;
            IsLeft = IsRight = IsUp = IsDown = true;
            if (Grid == null)
                Grid = new Vector2(4, 4);
            if (Grid.y == 4 && Grid.x == 4)
            {
                //1,5,9,13禁左
                //1,2,3,4禁下
                //4,8,12,16禁右
                //13,14,15,16禁上
                if (_location == 1 || _location == 5 || _location == 9 || _location == 13)
                    IsLeft = false;
                if (_location == 1 || _location == 2 || _location == 3 || _location == 4)
                    IsDown = false;
                if (_location == 4 || _location == 8 || _location == 12 || _location == 16)
                    IsRight = false;
                if (_location == 13 || _location == 14 || _location == 15 || _location == 16)
                    IsUp = false;
                if (!ExitLocation.Equals(string.Empty) && _location == ExitLocation)
                {
                    switch (ExitDirection)
                    {
                        case 0:
                            IsUp = true;
                            break;
                        case 1:
                            IsDown = true;
                            break;
                        case 2:
                            IsLeft = true;
                            break;
                        case 3:
                            IsRight = true;
                            break;
                    }
                }
            }
            else if (Grid.y == 2 && Grid.x == 2)
            {
                if (_location == 1 || _location == 3)
                    IsLeft = false;
                if (_location == 1 || _location == 2)
                    IsDown = false;
                if (_location == 2|| _location == 4)
                    IsRight = false;
                if (_location == 3 || _location == 4)
                    IsUp = false;
                if (!ExitLocation.Equals(string.Empty) && _location == ExitLocation)
                {
                    switch (ExitDirection)
                    {
                        case 0:
                            IsUp = true;
                            break;
                        case 1:
                            IsDown = true;
                            break;
                        case 2:
                            IsLeft = true;
                            break;
                        case 3:
                            IsRight = true;
                            break;
                    }
                }
            }
        }
    }
    public Text num;
    public GameObject VictoryObj;
    public GameObject LoseObj;
    public Vector2 Grid;//横竖几格
    private int _location;//角色处在哪个格子
    private bool IsDrag = false;//第一次进入
    private bool IsDrop = false;//强制中止进入
    private Vector2 Vector;//第一次拖拽起点
    private Vector2 SVector;//上次拖拽位置
    private Vector2 RoleVector;//角色的初始位置
    public int ExitLocation;//出口所在格子
    public int ExitDirection;//出口方向 0上1下2左3右
    public int StandardNum;//达标数字
    private GameObject gameObject1;//与其互换的imgnum
    private bool IsPass = false;//触发通关动作
    private int PassNum = 80;//通关行走帧数
    private bool IsLeft = true;
    private bool IsRight = true;
    private bool IsUp = true;
    private bool IsDown = true;

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("进入");
        //第一次进入时，将状态修改并赋值
        if (IsDrag == false)
        {
            IsDrag = true;
            Vector = eventData.position;
            SVector = eventData.position;
            RoleVector = transform.localPosition;
        }
        if (!IsDrop)
        {
            //向右
            if (eventData.position.x - 3 > Vector.x && IsRight)
            {
                //出去
                if (ExitLocation == _location && ExitDirection == 3)
                {
                    IsDrop = true;
                    IsPass = true;
                    return;
                }
                Debug.Log("进入右");
                IsLeft = IsUp = IsDown = false;
                transform.localPosition = new Vector3(transform.localPosition.x - (SVector.x - eventData.position.x), transform.localPosition.y);
                //找到向右的num
                gameObject1 = GameObject.Find("ImgNum" + (_location + 1));
                gameObject1.transform.localPosition = new Vector3(gameObject1.transform.localPosition.x + (SVector.x - eventData.position.x), gameObject1.transform.localPosition.y);
                SVector = eventData.position;
                //横向移动大于二百停止
                if (SVector.x - 200 >= Vector.x)
                {
                    Debug.Log("完成右");
                    IsDrop = true;
                    transform.localPosition = new Vector2(RoleVector.x + 200, RoleVector.y);
                    gameObject1.transform.localPosition = RoleVector;
                    gameObject1.transform.name = "ImgNum" + _location;
                    gameObject1.GetComponent<ImgNum>().Location = _location;
                    Location = _location + 1;
                    //变色并计算值
                    if (gameObject1.GetComponent<ImgNum>().Color == 0)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) + Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 1;
                    }
                    else if (gameObject1.GetComponent<ImgNum>().Color == 1)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) - Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 0;
                    }
                    else if (gameObject1.GetComponent<ImgNum>().Color == 2)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) * Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 3;
                    }
                    else if (gameObject1.GetComponent<ImgNum>().Color == 3)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) / Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 2;
                    }
                }
            }
            //向左
            else if (eventData.position.x + 3 < Vector.x  && IsLeft)
            {
                //出去
                if (ExitLocation == _location && ExitDirection == 2)
                {
                    IsDrop = true;
                    IsPass = true;
                    return;
                }
                Debug.Log("进入左");
               IsRight = IsUp = IsDown = false;
                transform.localPosition = new Vector3(transform.localPosition.x - (SVector.x - eventData.position.x), transform.localPosition.y);
                //找到向左的num
                gameObject1 = GameObject.Find("ImgNum" + (_location - 1));
                gameObject1.transform.localPosition = new Vector3(gameObject1.transform.localPosition.x + (SVector.x - eventData.position.x), gameObject1.transform.localPosition.y);
                SVector = eventData.position;
                //横向移动大于二百停止
                if (SVector.x + 200 <= Vector.x)
                {
                    Debug.Log("完成左");
                    IsDrop = true;
                    transform.localPosition = new Vector2(RoleVector.x - 200, RoleVector.y);
                    gameObject1.transform.localPosition = RoleVector;
                    gameObject1.transform.name = "ImgNum" + _location;
                    gameObject1.GetComponent<ImgNum>().Location = _location;
                    Location = _location - 1;
                    //变色并计算值
                    if (gameObject1.GetComponent<ImgNum>().Color == 0)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) + Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 1;
                    }
                    else if (gameObject1.GetComponent<ImgNum>().Color == 1)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) - Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 0;
                    }
                    else if (gameObject1.GetComponent<ImgNum>().Color == 2)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) * Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 3;
                    }
                    else if (gameObject1.GetComponent<ImgNum>().Color == 3)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) / Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 2;
                    }
                }
            }
            //向上
            else if (eventData.position.y - 3 > Vector.y  && IsUp)
            {
                //出去
                if (ExitLocation == _location && ExitDirection == 0)
                {
                    IsDrop = true;
                    IsPass = true;
                    return;
                }
                Debug.Log("进入上");
               IsDown =  IsRight = IsLeft = false;
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + (eventData.position.y - SVector.y));
                //找到向上的num
                gameObject1 = GameObject.Find("ImgNum" + (_location + Grid.y));
                gameObject1.transform.localPosition = new Vector3(gameObject1.transform.localPosition.x, gameObject1.transform.localPosition.y - (eventData.position.y - SVector.y));
                SVector = eventData.position;
                if (SVector.y - 200 >= Vector.y)
                {
                    Debug.Log("完成上");
                    IsDrop = true;
                    transform.localPosition = new Vector2(RoleVector.x, RoleVector.y + 200);
                    gameObject1.transform.localPosition = RoleVector;
                    gameObject1.transform.name = "ImgNum" + _location;
                    gameObject1.GetComponent<ImgNum>().Location = _location;
                    Location = _location + (int)Grid.y;
                    //变色并计算值
                    if (gameObject1.GetComponent<ImgNum>().Color == 0)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) + Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 1;
                    }
                    else if (gameObject1.GetComponent<ImgNum>().Color == 1)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) - Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 0;
                    }
                    else if (gameObject1.GetComponent<ImgNum>().Color == 2)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) * Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 3;
                    }
                    else if (gameObject1.GetComponent<ImgNum>().Color == 3)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) / Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 2;
                    }
                }
            }
            //向下
            else if (eventData.position.y + 3 < Vector.y  && IsDown)
            {
                //出去
                if (ExitLocation == _location && ExitDirection == 1)
                {
                    IsDrop = true;
                    IsPass = true;
                    return;
                }
                Debug.Log("进入下");
               IsUp =  IsRight = IsLeft = false;
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + (eventData.position.y - SVector.y));
                //找到向上的num
                gameObject1 = GameObject.Find("ImgNum" + (_location - Grid.y));
                gameObject1.transform.localPosition = new Vector3(gameObject1.transform.localPosition.x, gameObject1.transform.localPosition.y - (eventData.position.y - SVector.y));
                SVector = eventData.position;
                if (SVector.y + 200 <= Vector.y)
                {
                    Debug.Log("完成下");
                    IsDrop = true;
                    transform.localPosition = new Vector2(RoleVector.x, RoleVector.y - 200);
                    gameObject1.transform.localPosition = RoleVector;
                    gameObject1.transform.name = "ImgNum" + _location;
                    gameObject1.GetComponent<ImgNum>().Location = _location;
                    Location = _location - (int)Grid.y;
                    //变色并计算值
                    if (gameObject1.GetComponent<ImgNum>().Color == 0)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) + Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 1;
                    }
                    else if (gameObject1.GetComponent<ImgNum>().Color == 1)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) - Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 0;
                    }
                    else if (gameObject1.GetComponent<ImgNum>().Color == 2)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) * Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 3;
                    }
                    else if (gameObject1.GetComponent<ImgNum>().Color == 3)
                    {
                        int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) / Convert.ToInt32(gameObject1.GetComponent<ImgNum>().num.text));
                        if (numtext > 10000)
                            numtext = 10000;
                        else if (numtext <= 0)
                            numtext = 1;
                        num.text = numtext.ToString();
                        gameObject1.GetComponent<ImgNum>().Color = 2;
                    }
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDrag = false;
        if (!IsDrop && !IsPass)//未完成移动
        {
            transform.localPosition = RoleVector;
           int lo = Convert.ToInt32(gameObject1.GetComponent<ImgNum>().name.Replace("ImgNum",""));
            gameObject1.transform.localPosition = transform.parent.GetComponent<Board>().vector2s[lo - 1];
            Location = Location;
            IsDrop = false;
        }
        else
            IsDrop = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent.parent.name == "Bg1")
        {
            Grid = new Vector2(2, 2);
            Location = 3;
        }
        else
        {
            Grid = new Vector2(4, 4);
            Location = 1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsPass && PassNum >= 0)
        {
            PassNum--;
            if (ExitDirection == 0)
                transform.Translate(Vector3.up * 0.6f * Time.deltaTime);
            else if (ExitDirection == 1)
                transform.Translate(Vector3.down * 0.6f * Time.deltaTime);
            else if (ExitDirection == 2)
                transform.Translate(Vector3.left * 0.6f * Time.deltaTime);
            else if (ExitDirection == 3)
                transform.Translate(Vector3.right * 0.6f * Time.deltaTime);
        }
        else if(IsPass && PassNum < 0)
        {
            //通关
            if (Convert.ToInt32(num.text) == StandardNum)
            {
                VictoryObj.SetActive(true);
            }
            //通关失败
            else
            {
                LoseObj.SetActive(true);
            }
        }
    }
}
