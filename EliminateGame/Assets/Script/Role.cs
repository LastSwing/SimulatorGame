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
                GetOut();
            }
            else if (Grid.y == 2 && Grid.x == 2)
            {
                if (_location == 1 || _location == 3)
                    IsLeft = false;
                if (_location == 1 || _location == 2)
                    IsDown = false;
                if (_location == 2 || _location == 4)
                    IsRight = false;
                if (_location == 3 || _location == 4)
                    IsUp = false;
                GetOut();
            }
            else if (Grid.x == 1 && Grid.y == 3)
            {
                IsRight = false;
                IsLeft = false;
                if (_location == 3)
                    IsUp = false;
                if (_location == 1)
                    IsDown = false;
                GetOut();
            }
            else if (Grid.x == 2 && Grid.y == 3)
            {
                if (_location == 1 || _location == 3 || _location == 5)
                    IsLeft = false;
                if (_location == 1 || _location == 2)
                    IsDown = false;
                if (_location == 2 || _location == 4 || _location == 6)
                    IsRight = false;
                if (_location == 5 || _location == 6)
                    IsUp = false;
                GetOut();
            }
            else if (Grid.x == 3 && Grid.y == 3)
            {
                if (_location == 1 || _location == 4 || _location == 7)
                    IsLeft = false;
                if (_location == 1 || _location == 2 || _location == 3)
                    IsDown = false;
                if (_location == 3 || _location == 6 || _location == 9)
                    IsRight = false;
                if (_location == 7 || _location == 8 || _location == 9)
                    IsUp = false;
                GetOut();
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
    public Dictionary<int,int> ExitLocation;//出口所在格子-位置 0上1下2左3右
    public int StandardNum;//达标数字
    private GameObject gameObject1;//与其互换的imgnum
    private bool IsPass = false;//触发通关动作
    private int PassNum = 80;//通关行走帧数
    private bool IsLeft = true;
    private bool IsRight = true;
    private bool IsUp = true;
    private bool IsDown = true;
    private bool Goon = false;

    public void OnDrag(PointerEventData eventData)
    {
        if (Goon) return;
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
                foreach (var item in ExitLocation)
                {
                    if (item.Key == _location && item.Value == 3)
                    {
                        IsDrop = true;
                        IsPass = true;
                        return;
                    }
                }
                IsLeft = IsUp = IsDown = false;
                transform.localPosition = new Vector3(transform.localPosition.x - (SVector.x - eventData.position.x), transform.localPosition.y);
                //找到向右的num
                List<GameObject> list = BaseHelper.GetAllSceneObjects(transform.parent, true, false, (_location + 1).ToString());
                if (list.Count != 0)
                {
                    gameObject1 = list[0];
                    gameObject1.transform.localPosition = new Vector3(gameObject1.transform.localPosition.x + (SVector.x - eventData.position.x), gameObject1.transform.localPosition.y);
                }
                else
                    gameObject1 = null;
                SVector = eventData.position;
                //横向移动大于二百停止
                if (SVector.x - 200 >= Vector.x)
                {
                    Debug.Log("完成右");
                    IsDrop = true;
                    transform.localPosition = new Vector2(RoleVector.x + 200, RoleVector.y);
                    if (list.Count > 0)
                    {
                        gameObject1.transform.localPosition = RoleVector;
                        if (list[0].name.Contains("ImgNum"))
                        {
                            gameObject1.transform.name = "ImgNum" + _location;
                            gameObject1.GetComponent<ImgNum>().Location = _location;
                        }
                        else
                        {
                            gameObject1.transform.name = "Role" + _location;
                            gameObject1.GetComponent<Role>().Location = _location;
                        }
                    }
                    Location = _location + 1;
                    gameObject.name = "Role" + (_location);
                    if (list.Count > 0 && list[0].name.Contains("ImgNum"))
                        ChangeColor();
                }
            }
            //向左
            else if (eventData.position.x + 3 < Vector.x  && IsLeft)
            {
                //出去
                foreach (var item in ExitLocation)
                {
                    if (item.Key == _location && item.Value == 2)
                    {
                        IsDrop = true;
                        IsPass = true;
                        return;
                    }
                }
               IsRight = IsUp = IsDown = false;
                transform.localPosition = new Vector3(transform.localPosition.x - (SVector.x - eventData.position.x), transform.localPosition.y);
                //找到向左的num
                List<GameObject> list = BaseHelper.GetAllSceneObjects(transform.parent, true, false, (_location - 1).ToString());
                if (list.Count != 0)
                {
                    gameObject1 = list[0];
                    gameObject1.transform.localPosition = new Vector3(gameObject1.transform.localPosition.x + (SVector.x - eventData.position.x), gameObject1.transform.localPosition.y);
                }
                else
                    gameObject1 = null;
                SVector = eventData.position;
                //横向移动大于二百停止
                if (SVector.x + 200 <= Vector.x)
                {
                    Debug.Log("完成左");
                    IsDrop = true;
                    transform.localPosition = new Vector2(RoleVector.x - 200, RoleVector.y);
                    if (list.Count > 0)
                    {
                        gameObject1.transform.localPosition = RoleVector;
                        if (list[0].name.Contains("ImgNum"))
                        {
                            gameObject1.transform.name = "ImgNum" + _location;
                            gameObject1.GetComponent<ImgNum>().Location = _location;
                        }
                        else
                        {
                            gameObject1.transform.name = "Role" + _location;
                            gameObject1.GetComponent<Role>().Location = _location;
                        }
                    }
                    Location = _location - 1;
                    gameObject.name = "Role" + (_location);
                    if (list.Count > 0 && list[0].name.Contains("ImgNum"))
                        ChangeColor();
                }
            }
            //向上
            else if (eventData.position.y - 3 > Vector.y  && IsUp)
            {
                //出去
                foreach (var item in ExitLocation)
                {
                    if (item.Key == _location && item.Value == 0)
                    {
                        IsDrop = true;
                        IsPass = true;
                        return;
                    }
                }
                IsDown =  IsRight = IsLeft = false;
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + (eventData.position.y - SVector.y));
                //找到向上的num
                List<GameObject> list = BaseHelper.GetAllSceneObjects(transform.parent, true, false, (_location + Grid.x).ToString());
                if (list.Count != 0)
                {
                    gameObject1 = list[0];
                    gameObject1.transform.localPosition = new Vector3(gameObject1.transform.localPosition.x, gameObject1.transform.localPosition.y - (eventData.position.y - SVector.y));
                }
                else
                    gameObject1 = null;
                SVector = eventData.position;
                if (SVector.y - 200 >= Vector.y)
                {
                    Debug.Log("完成上");
                    IsDrop = true;
                    transform.localPosition = new Vector2(RoleVector.x, RoleVector.y + 200);
                    if (list.Count > 0)
                    {
                        gameObject1.transform.localPosition = RoleVector;
                        if (list[0].name.Contains("ImgNum"))
                        {
                            gameObject1.transform.name = "ImgNum" + _location;
                            gameObject1.GetComponent<ImgNum>().Location = _location;
                        }
                        else
                        {
                            gameObject1.transform.name = "Role" + _location;
                            gameObject1.GetComponent<Role>().Location = _location;
                        }
                    }
                    Location = _location + (int)Grid.x;
                    gameObject.name = "Role" + (_location);
                    if (list.Count > 0 && list[0].name.Contains("ImgNum"))
                        ChangeColor();
                }
            }
            //向下
            else if (eventData.position.y + 3 < Vector.y  && IsDown)
            {
                //出去
                foreach (var item in ExitLocation)
                {
                    if (item.Key == _location && item.Value == 1)
                    {
                        IsDrop = true;
                        IsPass = true;
                        return;
                    }
                }
                IsUp =  IsRight = IsLeft = false;
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + (eventData.position.y - SVector.y));
                //找到向上的num
                List<GameObject> list = BaseHelper.GetAllSceneObjects(transform.parent, true, false, (_location - Grid.x).ToString());
                if (list.Count != 0)
                {
                    gameObject1 = list[0];
                    gameObject1.transform.localPosition = new Vector3(gameObject1.transform.localPosition.x, gameObject1.transform.localPosition.y - (eventData.position.y - SVector.y));
                }
                else
                    gameObject1 = null;
                SVector = eventData.position;
                if (SVector.y + 200 <= Vector.y)
                {
                    Debug.Log("完成下");
                    IsDrop = true;
                    transform.localPosition = new Vector2(RoleVector.x, RoleVector.y - 200);
                    if (list.Count > 0)
                    {
                        gameObject1.transform.localPosition = RoleVector;
                        if (list[0].name.Contains("ImgNum"))
                        {
                            gameObject1.transform.name = "ImgNum" + _location;
                            gameObject1.GetComponent<ImgNum>().Location = _location;
                        }
                        else
                        {
                            gameObject1.transform.name = "Role" + _location;
                            gameObject1.GetComponent<Role>().Location = _location;
                        }
                    }
                    Location = _location - (int)Grid.x;
                    gameObject.name = "Role" + (_location);
                    if (list.Count > 0 && list[0].name.Contains("ImgNum"))
                        ChangeColor();
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Goon) return;
        IsDrag = false;
        if (!IsDrop && !IsPass)//未完成移动
        {
            if (transform.localPosition.x + 100 <= RoleVector.x || transform.localPosition.y - 100 >= RoleVector.y || transform.localPosition.y + 100 <= RoleVector.y || transform.localPosition.x - 100 >= RoleVector.x)//移动过半
            {
                Goon = true;
            }
            else
            {
                transform.localPosition = RoleVector;
                if (gameObject1 != null)
                {
                    if (gameObject1.name.Contains("ImgNum"))
                    {
                        int lo = Convert.ToInt32(gameObject1.GetComponent<ImgNum>().name.Replace("ImgNum", ""));
                        gameObject1.transform.localPosition = transform.parent.GetComponent<BgScript>().vector2s[lo - 1];
                    }
                    else if(gameObject1.name.Contains("Role"))
                    {
                        int lo = Convert.ToInt32(gameObject1.GetComponent<Role>().Location);
                        gameObject1.transform.localPosition = transform.parent.GetComponent<BgScript>().vector2s[lo - 1];
                    }
                }
                Location = Location;
                IsDrop = false;
            }
        }
        else
            IsDrop = false;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsPass && PassNum >= 0)
        {
            PassNum--;
            foreach (var item in ExitLocation)
            {
                if (item.Key == _location)
                {
                    if (item.Value == 0)
                        transform.Translate(Vector3.up * 0.5f * Time.deltaTime);
                    else if (item.Value == 1)
                        transform.Translate(Vector3.down * 0.5f * Time.deltaTime);
                    else if (item.Value == 2)
                        transform.Translate(Vector3.left * 0.5f * Time.deltaTime);
                    else if (item.Value == 3)
                        transform.Translate(Vector3.right * 0.5f * Time.deltaTime);
                }
            }
        }
        else if(IsPass && PassNum < 0)
        {
            gameObject.SetActive(false);
            //通关
            if (Convert.ToInt32(num.text) == StandardNum)
            {
                List<GameObject> gameObjects = BaseHelper.GetAllSceneObjects(transform.parent,true,false,"Role");
                if(gameObjects.Count == 0)
                    VictoryObj.SetActive(true);
            }
            //通关失败
            else
            {
                LoseObj.SetActive(true);
            }
        }
        if (Goon)
        {
            if (IsDown)
            {
                transform.Translate(Vector2.down * 1 * Time.deltaTime);
                List<GameObject> list = BaseHelper.GetAllSceneObjects(transform.parent, true, false, (_location - Grid.x).ToString());
                if (list.Count != 0)
                {
                    gameObject1 = list[0];
                    gameObject1.transform.Translate(Vector2.up * 1 * Time.deltaTime);
                }
                if (transform.localPosition.y <= RoleVector.y - 200)
                {
                    transform.localPosition = new Vector2(transform.localPosition.x, RoleVector.y - 200);
                    if (list.Count > 0)
                    {
                        gameObject1.transform.localPosition = RoleVector;
                        if (list[0].name.Contains("ImgNum"))
                        {
                            gameObject1.transform.name = "ImgNum" + _location;
                            gameObject1.GetComponent<ImgNum>().Location = _location;
                        }
                        else
                        {
                            gameObject1.transform.name = "Role" + _location;
                            gameObject1.GetComponent<Role>().Location = _location;
                        }
                    }
                    Location = _location - (int)Grid.x;
                    gameObject.name = "Role" + (_location);
                    if (list.Count > 0 && list[0].name.Contains("ImgNum"))
                        ChangeColor();
                    IsDrop = false;
                    Goon = false;
                }
            }
            else if (IsUp)
            {
                transform.Translate(Vector2.up * 1 * Time.deltaTime);
                List<GameObject> list = BaseHelper.GetAllSceneObjects(transform.parent, true, false, (_location + Grid.x).ToString());
                if (list.Count != 0)
                {
                    gameObject1 = list[0];
                    gameObject1.transform.Translate(Vector2.down * 1 * Time.deltaTime);
                }
                if (transform.localPosition.y >= RoleVector.y + 200)
                {
                    transform.localPosition = new Vector2(transform.localPosition.x, RoleVector.y + 200);
                    if (list.Count > 0)
                    {
                        gameObject1.transform.localPosition = RoleVector;
                        if (list[0].name.Contains("ImgNum"))
                        {
                            gameObject1.transform.name = "ImgNum" + _location;
                            gameObject1.GetComponent<ImgNum>().Location = _location;
                        }
                        else
                        {
                            gameObject1.transform.name = "Role" + _location;
                            gameObject1.GetComponent<Role>().Location = _location;
                        }
                    }
                    Location = _location + (int)Grid.x;
                    gameObject.name = "Role" + (_location);
                    if (list.Count > 0 && list[0].name.Contains("ImgNum"))
                        ChangeColor();
                    IsDrop = false;
                    Goon = false;
                }
            }
            else if (IsRight)
            {
                transform.Translate(Vector2.right * 1 * Time.deltaTime);
                List<GameObject> list = BaseHelper.GetAllSceneObjects(transform.parent, true, false, (_location + 1).ToString());
                if (list.Count != 0)
                {
                    gameObject1 = list[0];
                    gameObject1.transform.Translate(Vector2.left * 1 * Time.deltaTime);
                }
                if (transform.localPosition.x >= RoleVector.x + 200)
                {
                    transform.localPosition = new Vector2(RoleVector.x + 200, RoleVector.y);
                    if (list.Count > 0)
                    {
                        gameObject1.transform.localPosition = RoleVector;
                        if (list[0].name.Contains("ImgNum"))
                        {
                            gameObject1.transform.name = "ImgNum" + _location;
                            gameObject1.GetComponent<ImgNum>().Location = _location;
                        }
                        else
                        {
                            gameObject1.transform.name = "Role" + _location;
                            gameObject1.GetComponent<Role>().Location = _location;
                        }
                    }
                    Location = _location + 1;
                    gameObject.name = "Role" + (_location);
                    if (list.Count > 0 && list[0].name.Contains("ImgNum"))
                        ChangeColor();
                    IsDrop = false;
                    Goon = false;
                }
            }
            else if (IsLeft)
            {
                transform.Translate(Vector2.left * 1 * Time.deltaTime);
                List<GameObject> list = BaseHelper.GetAllSceneObjects(transform.parent, true, false, (_location - 1).ToString());
                if (list.Count != 0)
                {
                    gameObject1 = list[0];
                    gameObject1.transform.Translate(Vector2.right * 1 * Time.deltaTime);
                }
                if (transform.localPosition.x <= RoleVector.x - 200)
                {
                    transform.localPosition = new Vector2(RoleVector.x - 200, RoleVector.y);
                    if (list.Count > 0)
                    {
                        gameObject1.transform.localPosition = RoleVector;
                        if (list[0].name.Contains("ImgNum"))
                        {
                            gameObject1.transform.name = "ImgNum" + _location;
                            gameObject1.GetComponent<ImgNum>().Location = _location;
                        }
                        else
                        {
                            gameObject1.transform.name = "Role" + _location;
                            gameObject1.GetComponent<Role>().Location = _location;
                        }
                    }
                    Location = _location - 1;
                    gameObject.name = "Role" + (_location);
                    if (list.Count > 0 && list[0].name.Contains("ImgNum"))
                        ChangeColor();
                    IsDrop = false;
                    Goon = false;
                }
            }
        }
    }

    /// <summary>
    /// 出去路线赋值
    /// </summary>
    void GetOut()
    {
        if (ExitLocation != null)
        {
            foreach (var item in ExitLocation)
            {
                if (!ExitLocation.Equals(string.Empty) && _location == item.Key)
                {
                    switch (item.Value)
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
    /// <summary>
    /// 变色并计算值
    /// </summary>
    void ChangeColor()
    {
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
