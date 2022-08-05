using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CutScript : MonoBehaviour
{
    public Button leftBtn;
    public Button rightBtn;
    private LoadProp loadProp;
    private List<LoadPropList> propName = new List<LoadPropList>();
    public string rightname, leftname, bigname = "";
    #region 道具移动动画使用变量
    GameObject LeftG;//落点在左边的物体
    GameObject RightG;//落点在右边的物体
    GameObject BigG;//落点在中间的物体
    GameObject DisG;//消失的物体
    bool State = false;//是否开始动画
    int cycle = 25;//循环周期
    /// <summary>
    /// 0 全部移动 1 移动两个道具（道具使用）  2 只移动成为主道具的一个道具 3 移动两个道具（点击左右）
    /// </summary>
    int move = 0;
    /// <summary>
    ///左移true右移false
    /// </summary>
    bool LAndR = false;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        leftBtn.onClick.AddListener(leftBtnClick);
        rightBtn.onClick.AddListener(rightBtnClick);
        loadProp = new LoadProp();
        int i = 0;
        //生成道具到道具条
        foreach (var item in loadProp.PropDict)
        {
            propName.Add(new LoadPropList(i, item.Key, item.Value));
            GameObject go = new GameObject();
            go.transform.localPosition = new Vector2(-9999, 9999);
            go.name = item.Key;
            go.AddComponent<Image>().sprite = BaseHelper.LoadFromImage(new Vector2(50, 50), Application.dataPath + @"\Resources\Image\PropImage\" + item.Value.Image);
            go.GetComponent<Image>().preserveAspect = true;
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            go.transform.parent = transform;
            go.transform.localScale = Vector3.one;
            i++;
        }
        //显示道具
        if (loadProp.PropDict.Count == 0)
        {
            return;
        }
        if (loadProp.PropDict.Count > 0)
        {
            GameObject game = transform.Find(propName[0].Name).gameObject;
            game.transform.localPosition = new Vector2(-100, 0);
            leftname = propName[0].Name;
        }
        if (loadProp.PropDict.Count > 1)
        {
            GameObject game = transform.Find(propName[1].Name).gameObject;
            game.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);
            game.transform.localPosition = new Vector2(0, 0);
            bigname = propName[1].Name;
        }
        if (loadProp.PropDict.Count > 2)
        {
            GameObject game = transform.Find(propName[2].Name).gameObject;
            game.transform.localPosition = new Vector2(100, 0);
            rightname = propName[2].Name;
        }
        if (transform.parent.name == "UI")
            transform.parent.GetComponent<UIScript>().m_PropName = bigname;
        else
            transform.parent.parent.GetComponent<EditView>().m_PropName = bigname;
    }
    public void Restart()
    {
        propName.Clear();
        int i = 0;
        foreach (var item in loadProp.PropDict)
        {
            propName.Add(new LoadPropList(i, item.Key, item.Value));
            transform.Find(item.Key).localPosition = new Vector2(-9999, 9999);
            transform.Find(item.Key).GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            i++;
        }
        //显示道具
        if (loadProp.PropDict.Count == 0)
        {
            return;
        }
        if (loadProp.PropDict.Count > 0)
        {
            GameObject game = transform.Find(propName[0].Name).gameObject;
            game.transform.localPosition = new Vector2(-100, 0);
            leftname = propName[0].Name;
        }
        if (loadProp.PropDict.Count > 1)
        {
            GameObject game = transform.Find(propName[1].Name).gameObject;
            game.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);
            game.transform.localPosition = new Vector2(0, 0);
            bigname = propName[1].Name;
        }
        if (loadProp.PropDict.Count > 2)
        {
            GameObject game = transform.Find(propName[2].Name).gameObject;
            game.transform.localPosition = new Vector2(100, 0);
            rightname = propName[2].Name;
        }
        if (transform.parent.name == "UI")
            transform.parent.GetComponent<UIScript>().m_PropName = bigname;
        else
            transform.parent.parent.GetComponent<EditView>().m_PropName = bigname;
    }
    private void FixedUpdate()
    {
        if (State)
        {
            if (LAndR)//左移
            {
                BigG.transform.localPosition = new Vector2(BigG.transform.localPosition.x - 4, BigG.transform.localPosition.y);
                BigG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(BigG.transform.GetComponent<RectTransform>().sizeDelta.x + 4, BigG.transform.GetComponent<RectTransform>().sizeDelta.y + 4);
                if (move == 0)
                {
                    LeftG.transform.localPosition = new Vector2(LeftG.transform.localPosition.x - 4, LeftG.transform.localPosition.y);
                    LeftG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(LeftG.transform.GetComponent<RectTransform>().sizeDelta.x - 4, LeftG.transform.GetComponent<RectTransform>().sizeDelta.y - 4);
                    if (DisG != null)
                    {
                        DisG.transform.localPosition = new Vector2(DisG.transform.localPosition.x - 4, DisG.transform.localPosition.y);
                        DisG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(DisG.transform.GetComponent<RectTransform>().sizeDelta.x - 2, DisG.transform.GetComponent<RectTransform>().sizeDelta.y - 2);
                    }
                }
                if (move <= 1)
                {
                    RightG.transform.localPosition = new Vector2(RightG.transform.localPosition.x - 4, RightG.transform.localPosition.y);
                    RightG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(RightG.transform.GetComponent<RectTransform>().sizeDelta.x + 2, RightG.transform.GetComponent<RectTransform>().sizeDelta.y + 2);
                }
                if (move == 3)
                {
                    LeftG.transform.localPosition = new Vector2(LeftG.transform.localPosition.x - 4, LeftG.transform.localPosition.y);
                    LeftG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(LeftG.transform.GetComponent<RectTransform>().sizeDelta.x - 4, LeftG.transform.GetComponent<RectTransform>().sizeDelta.y - 4);
                }
            }
            else//右移
            {
                BigG.transform.localPosition = new Vector2(BigG.transform.localPosition.x + 4, BigG.transform.localPosition.y);
                BigG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(BigG.transform.GetComponent<RectTransform>().sizeDelta.x + 4, BigG.transform.GetComponent<RectTransform>().sizeDelta.y + 4);
                if (move <= 1)
                {
                    LeftG.transform.localPosition = new Vector2(LeftG.transform.localPosition.x + 4, LeftG.transform.localPosition.y);
                    LeftG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(LeftG.transform.GetComponent<RectTransform>().sizeDelta.x + 2, LeftG.transform.GetComponent<RectTransform>().sizeDelta.y + 2);
                }
                if (move == 0)
                {
                    if (DisG != null)
                    {
                        DisG.transform.localPosition = new Vector2(DisG.transform.localPosition.x + 4, DisG.transform.localPosition.y);
                        DisG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(DisG.transform.GetComponent<RectTransform>().sizeDelta.x - 2, DisG.transform.GetComponent<RectTransform>().sizeDelta.y - 2);
                    }
                    RightG.transform.localPosition = new Vector2(RightG.transform.localPosition.x + 4, RightG.transform.localPosition.y);
                    RightG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(RightG.transform.GetComponent<RectTransform>().sizeDelta.x - 4, RightG.transform.GetComponent<RectTransform>().sizeDelta.y - 4);
                }
                if (move == 3)
                {
                    RightG.transform.localPosition = new Vector2(RightG.transform.localPosition.x + 4, RightG.transform.localPosition.y);
                    RightG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(RightG.transform.GetComponent<RectTransform>().sizeDelta.x - 4, RightG.transform.GetComponent<RectTransform>().sizeDelta.y - 4);
                }
            }
            cycle--;
            if (cycle == 0)//退出循环
            {
                cycle = 25;
                move = 0;
                State = false;
            }
        }
    }
    /// <summary>
    /// 左
    /// </summary>
    void leftBtnClick()
    {
        if (propName.Count == 1 || propName.Count == 0 || rightname == "" || cycle != 25) return;
        if (propName.Count > 3)
        {
            int number = 0;
            List<int> vs = new List<int>();
            //右图向左变大
            BigG = transform.Find(rightname).gameObject;
            //中图向左变小
            LeftG = transform.Find(bigname).gameObject;
            //左图向左消失  
            DisG = transform.Find(leftname).gameObject;
            //找到下个图向左变大并在右边
            foreach (var item in propName)
            {
                if (item.Name == rightname)
                {
                    number = item.Number;
                    foreach (var items in propName)
                    {
                        if (items.Number > item.Number)
                            vs.Add(items.Number);
                    }
                }
            }
            if (vs.Count == 0)//没有排序比右图更高的道具，就取排序最低的道具
                RightG = transform.Find(propName[0].Name).gameObject;
            else//找出与上个道具排序最相近的道具
            {
                int closestTo = vs.Aggregate((x, y) => Math.Abs(x - number) < Math.Abs(y - number) ? x : y);
                foreach (var item in propName)
                {
                    if (item.Number == closestTo)
                        RightG = transform.Find(item.Name).gameObject;
                }
            }
            RightG.transform.localPosition = new Vector2(200, 0);
            RightG.transform.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            bigname = BigG.name;
            leftname = LeftG.name;
            rightname = RightG.name;
            LAndR = true;
            State = true;
        }
        else if (propName.Count == 3)
        {
            //右图向左变大
            BigG = transform.Find(rightname).gameObject;
            //中图向左变小
            LeftG = transform.Find(bigname).gameObject;
            //左图生成再右边  
            RightG = transform.Find(leftname).gameObject;
            RightG.transform.localPosition = new Vector2(200, 0);
            RightG.transform.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            DisG = null;
            bigname = BigG.name;
            leftname = LeftG.name;
            rightname = RightG.name;
            LAndR = true;
            State = true;
        }
        else if (propName.Count == 2)
        {
            move = 3;
            if (rightname != "")
            {
                //右图向左变大
                BigG = transform.Find(rightname).gameObject;
                //中图向左变小
                LeftG = transform.Find(bigname).gameObject;
                bigname = BigG.name;
                leftname = LeftG.name;
                rightname = "";
                LAndR = true;
                State = true;
            }
        }
        if (transform.parent.name == "UI")
            transform.parent.GetComponent<UIScript>().m_PropName = bigname;
        else
            transform.parent.parent.GetComponent<EditView>().m_PropName = bigname;
    }
    /// <summary>
    /// 右
    /// </summary>
    void rightBtnClick()
    {
        if (propName.Count == 1 || propName.Count == 0 || leftname == "" || cycle != 25) return;
        if (propName.Count > 3)
        {
            int number = 0;
            List<int> vs = new List<int>();
            //右图向右消失
            DisG = transform.Find(rightname).gameObject;
            //中图向右变小
            RightG = transform.Find(bigname).gameObject;
            //左图向右变大  
            BigG = transform.Find(leftname).gameObject;
            //找出左图并往右变大
            foreach (var item in propName)
            {
                if (item.Name == leftname)
                {
                    number = item.Number;
                    foreach (var items in propName)
                    {
                        if (items.Number < item.Number)
                            vs.Add(items.Number);
                    }
                }
            }
            if (vs.Count == 0)//没有排序比左图更低的道具，就取排序最大的道具
                LeftG = transform.Find(propName[propName.Count - 1].Name).gameObject;
            else//找出与上个道具排序最相近的道具
            {
                int closestTo = vs.Aggregate((x, y) => Math.Abs(x - number) < Math.Abs(y - number) ? x : y);
                foreach (var item in propName)
                {
                    if (item.Number == closestTo)
                        LeftG = transform.Find(item.Name).gameObject;
                }
            }
            LeftG.transform.localPosition = new Vector2(-200, 0);
            LeftG.transform.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            LAndR = false;
            State = true;
            bigname = BigG.name;
            leftname = LeftG.name;
            rightname = RightG.name;
        }
        else if (propName.Count == 3)
        {
            //左图向右变大  
            BigG = transform.Find(leftname).gameObject;
            //中图向右变小
            RightG = transform.Find(bigname).gameObject;
            //右图生成再右边  
            LeftG = transform.Find(rightname).gameObject;
            LeftG.transform.localPosition = new Vector2(-200, 0);
            LeftG.transform.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            DisG = null;
            bigname = BigG.name;
            leftname = LeftG.name;
            rightname = RightG.name;
            LAndR = false;
            State = true;
        }
        else if (propName.Count == 2)
        {
            move = 3;
            if (leftname != "")
            {
                //右图向右变大
                BigG = transform.Find(leftname).gameObject;
                //中图向右变小
                RightG = transform.Find(bigname).gameObject;
                bigname = BigG.name;
                rightname = RightG.name;
                leftname = "";
                LAndR = false;
                State = true;
            }
        }
        if (transform.parent.name == "UI")
            transform.parent.GetComponent<UIScript>().m_PropName = bigname;
        else
            transform.parent.parent.GetComponent<EditView>().m_PropName = bigname;
    }
    /// <summary>
    /// 道具已使用 使用规则-从右到左
    /// </summary>
    public bool Consume()
    {
        if (cycle != 25) return false;
        else
        {
            LoadPropList loadPropList = new LoadPropList();
            foreach (var item in propName)
            {
                if (item.Name == bigname)
                    loadPropList = item;
            }
            transform.parent.GetComponent<UIScript>().m_PropName = bigname;
            propName.Remove(loadPropList);
            transform.Find(bigname).transform.localPosition = new Vector2(-9999, 0);
            if (propName.Count > 2)
            {
                int number = 0;
                List<int> vs = new List<int>();
                BigG = transform.Find(rightname).gameObject;
                bigname = rightname;
                //找到下个图向左变大并在右边
                foreach (var item in propName)
                {
                    if (item.Name == rightname)
                    {
                        number = item.Number;
                        foreach (var items in propName)
                        {
                            if (items.Number > item.Number)
                                vs.Add(items.Number);
                        }
                    }
                }
                if (vs.Count == 0)//没有排序比右图更高的道具，就取排序最低的道具
                    RightG = transform.Find(propName[0].Name).gameObject;
                else//找出与上个道具排序最相近的道具
                {
                    int closestTo = vs.Aggregate((x, y) => Math.Abs(x - number) < Math.Abs(y - number) ? x : y);
                    foreach (var item in propName)
                    {
                        if (item.Number == closestTo)
                        {
                            RightG = transform.Find(item.Name).gameObject;
                            rightname = item.Name;
                        }
                    }
                }
                RightG.transform.localPosition = new Vector2(200, 0);
                RightG.transform.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                LAndR = true;
                State = true;
                move = 1;
            }
            else if (propName.Count == 2)
            {
                BigG = transform.Find(rightname).gameObject;
                bigname = rightname;
                move = 2;
                LAndR = true;
                State = true;
                rightname = "";
            }
            else if (propName.Count == 1)
            {
                if (leftname != "")
                {
                    BigG = transform.Find(leftname).gameObject;
                    bigname = leftname;
                    move = 2;
                    LAndR = false;
                    State = true;
                    leftname = "";
                }
                else if (rightname != "")
                {
                    BigG = transform.Find(rightname).gameObject;
                    bigname = rightname;
                    move = 2;
                    LAndR = true;
                    State = true;
                    rightname = "";
                }
            }
            return true;
        }
    }
    private class LoadPropList
    {
        public LoadPropList(int _number, string _name, MainProp _mainProp)
        {
            Number = _number;
            Name = _name;
            mainProp = _mainProp;
        }
        public LoadPropList()
        { 
            
        }
        public int Number { get; set; }
        public string Name { get; set; }
        public MainProp mainProp { get; set; }
    }
}
