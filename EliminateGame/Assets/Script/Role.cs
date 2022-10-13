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
    private bool _IsProtect = false;
    private bool _IsDevour = false;
    /// <summary>
    /// 是否保护
    /// </summary>
    private bool IsProtect
    {
        get { return _IsProtect; }
        set
        {
            transform.Find("Protect").gameObject.SetActive(value);
            _IsProtect = value;
        }
    }
    /// <summary>
    /// 是否吞噬
    /// </summary>
    private bool IsDevour
    {
        get { return _IsDevour; }
        set
        {
            transform.Find("Devour").gameObject.SetActive(value);
            _IsDevour = value;
        }
    }

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
            if (eventData.position.x - 5 > Vector.x && IsRight)
            {
                Move(3, eventData.position);
            }
            //向左
            else if (eventData.position.x + 5 < Vector.x  && IsLeft)
            {
                Move(2, eventData.position);
            }
            //向上
            else if (eventData.position.y - 5 > Vector.y  && IsUp)
            {
                Move(0, eventData.position);
            }
            //向下
            else if (eventData.position.y + 5 < Vector.y  && IsDown)
            {
                Move(1, eventData.position);
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
                        gameObject1.transform.localPosition = transform.parent.parent.GetComponent<BgScript>().vector2s[lo - 1];
                    }
                    else if(gameObject1.name.Contains("Role"))
                    {
                        int lo = Convert.ToInt32(gameObject1.GetComponent<Role>().Location);
                        gameObject1.transform.localPosition = transform.parent.parent.GetComponent<BgScript>().vector2s[lo - 1];
                    }
                }
                Location = Location;
                IsDrop = false;
            }
        }
        else
            IsDrop = false;
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
                if (gameObjects.Count == 0)
                {
                    VictoryObj.SetActive(true);
                    transform.parent.parent.GetComponent<BgScript>().LevelNum = transform.parent.parent.GetComponent<BgScript>().LevelNum + 1;
                    DataRead.SetLevel(transform.parent.parent.GetComponent<BgScript>().LevelNum);
                }
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
                    EndMove(1, list, _location - (int)Grid.x);
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
                    EndMove(0, list, _location + (int)Grid.x);
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
                    EndMove(3, list, _location + 1);
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
                    EndMove(2, list, _location - 1);
                    IsDrop = false;
                    Goon = false;
                }
            }
        }

        if (IsDevour)
            transform.Find("Devour").Rotate(Vector3.back);
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
        if (IsProtect && gameObject1.GetComponent<ImgNum>().Color < 4) return;
        if (gameObject1.GetComponent<ImgNum>().Color == 0)
        {
            int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) + gameObject1.GetComponent<ImgNum>().Num);
            if (numtext > 10000)
                numtext = 10000;
            else if (numtext <= 0)
                numtext = 1;
            num.text = numtext.ToString();
            if (!_IsProtect)
            {
                gameObject1.GetComponent<ImgNum>().Color = 1;
                gameObject1.GetComponent<ImgNum>().num.text = gameObject1.GetComponent<ImgNum>().Num.ToString();
            }
            else
                gameObject1.SetActive(false);
        }
        else if (gameObject1.GetComponent<ImgNum>().Color == 1)
        {
            int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) - gameObject1.GetComponent<ImgNum>().Num);
            if (numtext > 10000)
                numtext = 10000;
            else if (numtext <= 0)
                numtext = 1;
            num.text = numtext.ToString();
            if (!_IsProtect)
            {
                gameObject1.GetComponent<ImgNum>().Color = 0;
                gameObject1.GetComponent<ImgNum>().num.text = gameObject1.GetComponent<ImgNum>().Num.ToString();
            }
            else
                gameObject1.SetActive(false);
        }
        else if (gameObject1.GetComponent<ImgNum>().Color == 2)
        {
            int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) * gameObject1.GetComponent<ImgNum>().Num);
            if (numtext > 10000)
                numtext = 10000;
            else if (numtext <= 0)
                numtext = 1;
            num.text = numtext.ToString();
            if (!_IsProtect)
            {
                gameObject1.GetComponent<ImgNum>().Color = 3;
                gameObject1.GetComponent<ImgNum>().num.text = gameObject1.GetComponent<ImgNum>().Num.ToString();
            }
            else
                gameObject1.SetActive(false);
        }
        else if (gameObject1.GetComponent<ImgNum>().Color == 3)
        {
            int numtext = (int)Math.Ceiling(Convert.ToDouble(num.text) / gameObject1.GetComponent<ImgNum>().Num);
            if (numtext > 10000)
                numtext = 10000;
            else if (numtext <= 0)
                numtext = 1;
            num.text = numtext.ToString();
            if (!_IsProtect)
            {
                gameObject1.GetComponent<ImgNum>().Color = 2;
                gameObject1.GetComponent<ImgNum>().num.text = gameObject1.GetComponent<ImgNum>().Num.ToString();
            }
            else
                gameObject1.SetActive(false);
        }
        else if (gameObject1.GetComponent<ImgNum>().Color == 4)
        {
            IsProtect = true;
            if (IsDevour)
                IsDevour = false;
            gameObject1.SetActive(false);
        }
        else if(gameObject1.GetComponent<ImgNum>().Color == 5)
        {
            IsDevour = true;
            if (IsProtect)
                IsProtect = false;
            gameObject1.SetActive(false);
        }
        if (IsDevour) gameObject1.SetActive(false);
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="Wz">方向 0Up1Down2Left3Rignt</param>
    /// <param name="vector">相对拖拽位置</param>
    void Move(int Wz,Vector2 vector)
    {
        //可否出去
        foreach (var item in ExitLocation)
        {
            if (item.Key == _location && item.Value == Wz)
            {
                IsDrop = true;
                IsPass = true;
                return;
            }
        }
        IsDown = IsRight = IsLeft = IsUp = false;
        int itemLocition = 0;
        switch (Wz)
        {
            case 0:
                IsUp = true;
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + (vector.y - SVector.y));
                itemLocition = _location + (int)Grid.x;
                break;
            case 1: 
                IsDown = true;
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + (vector.y - SVector.y));
                itemLocition = _location - (int)Grid.x;
                break;
            case 2: 
                IsLeft = true;
                transform.localPosition = new Vector3(transform.localPosition.x - (SVector.x - vector.x), transform.localPosition.y);
                itemLocition = _location -1;
                break;
            case 3: 
                IsRight = true;
                transform.localPosition = new Vector3(transform.localPosition.x - (SVector.x - vector.x), transform.localPosition.y);
                itemLocition = _location + 1;
                break;
        }

        //找到对应切换的num
        List<GameObject> list = BaseHelper.GetAllSceneObjects(transform.parent, true, false, (itemLocition).ToString());
        bool numMove = true;//判断num是否需要移动
        if (list.Count != 0)
        {
            gameObject1 = list[0];
            if (gameObject1.name.Contains("ImgNum"))
            {
                if (gameObject1.GetComponent<ImgNum>().Color == 6)//为障碍物，不允许通过
                {
                    IsDrag = false;
                    transform.localPosition = RoleVector;
                    return;
                }
                if (IsDevour || gameObject1.GetComponent<ImgNum>().Color > 3)
                    numMove = false;

            }
            if(numMove)
                gameObject1.transform.localPosition = new Vector2(Wz > 1 ? gameObject1.transform.localPosition.x + (SVector.x - vector.x) : gameObject1.transform.localPosition.x,
            Wz < 2 ? gameObject1.transform.localPosition.y - (vector.y - SVector.y) : gameObject1.transform.localPosition.y);
        }
        else
            gameObject1 = null;
        SVector = vector;

        if ((SVector.y - 200 >= Vector.y && Wz == 0) || (SVector.y + 200 <= Vector.y && Wz == 1) || (SVector.x + 200 <= Vector.x && Wz == 2) || (SVector.x - 200 >= Vector.x && Wz == 3))
        {
            EndMove(Wz,list,itemLocition);
        }
    }
    /// <summary>
    /// 完成移动
    /// </summary>
    /// <param name="Wz">方向 0Up1Down2Left3Rignt</param>
    /// <param name="list">移动方向game</param>
    /// <param name="itemLocition">完成后的位置值</param>
    void EndMove(int Wz,List<GameObject> list,int itemLocition)
    {
        IsDrop = true;
        transform.localPosition = new Vector2(Wz > 1 ? Wz == 2 ? RoleVector.x - 200 : RoleVector.x + 200 : RoleVector.x,
            Wz < 2 ? Wz == 1 ? RoleVector.y - 200 : RoleVector.y + 200 : RoleVector.y);
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
        Location = itemLocition;
        gameObject.name = "Role" + (itemLocition);
        if (list.Count > 0 && list[0].name.Contains("ImgNum"))
            ChangeColor();
    }
}
