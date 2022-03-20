using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : SingletonMonoBehaviour<Manager>
{
    /// <summary>
    /// UI获取
    /// </summary>
    private Transform ButtonRoot;
    private Transform ItemRoot;

    /// <summary>
    /// 数据
    /// </summary>
    private Dictionary<int, string> GameDataDic = new Dictionary<int, string>()
    {
        {0,"王星海" },//名字
        {1,"80000" },//钱
        {2,"50" },//体力
        {3,"776" },//健康
        {4,"824" },//心情
        {5,"0" },//钻石
        {6,"-19999" },//回合流水
        {7,"" },//回合日志
        {8,"35" },//岁数
        {9,"10000"},//薪资
        {10,"10000"},//房价
        {11,"10000" },//车价
        {12,"20" }//工作消耗的体力
    };
    private List<int> NowShowID=new List<int>();
    private List<int> NowShowButton = new List<int>();

    protected override void Awake()
    {
        base.Awake();
        ButtonRoot = transform.Find("AllButton/Viewport/Content");
        ItemRoot = transform.Find("AllShow/Viewport/Content");
    }
    // Start is called before the first frame update
    public void Start()
    {
        Manager.instance.DoEventByID(0);
    }
    public void UpdateUI()
    {
        Tool.SetTransChild(ItemRoot, NowShowID.Count);
        for(int i=0;i< NowShowID.Count; i++)
        {
            ItemRoot.GetChild(i).Find("Key").GetComponent<Text>().text= ItemName[NowShowID[i]];
            ItemRoot.GetChild(i).Find("Value").GetComponent<Text>().text = GameDataDic[i];
        }
        Tool.SetTransChild(ButtonRoot, NowShowButton.Count);
        for (int i = 0; i < NowShowButton.Count; i++)
        {
            ButtonRoot.GetChild(i).Find("Text").GetComponent<Text>().text = ButtonName[NowShowButton[i]];
            ButtonRoot.GetChild(i).gameObject.name = NowShowButton[i].ToString();
        }
    }
    public void DoEventByID(int id)
    {
        int ran;
        switch (id)
        {
            case 0:
            case 8:
                NowShowID = new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12 };
                NowShowButton = new List<int>() {1,2,3,4,5,6,7,19 };
                break;
            case 1:
                GameDataDic[2] = (int.Parse(GameDataDic[2]) + 10).ToString();
                break;
            case 2:
                GameDataDic[5] = (int.Parse(GameDataDic[5]) + 10).ToString();
                break;
            case 6:
                NowShowID = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                NowShowButton = new List<int>() { 8, 14,15,16,17,18 };
                break;
            case 3:
            case 13:
                NowShowID = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                NowShowButton = new List<int>() { 8,  9, 10};
                break;
            case 5:
                GameDataDic[2] = "50";
                GameDataDic[6] = "0";
                GameDataDic[7] = "";
                GameDataDic[8] = (int.Parse(GameDataDic[8]) + 1).ToString();
                break;
            case 9:
                NowShowID = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                NowShowButton = new List<int>() {  8,  11,13 };
                break;
            case 10:
                NowShowID = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                NowShowButton = new List<int>() {  8, 12, 13 };
                break;
            case 11:
                GameDataDic[1] = (int.Parse(GameDataDic[1]) - int.Parse(GameDataDic[10])).ToString();
                GameDataDic[6] = (int.Parse(GameDataDic[6]) - int.Parse(GameDataDic[10])).ToString();
                GameDataDic[7] += "买房支出" + int.Parse(GameDataDic[10])+",";
                break;
            case 12:
                GameDataDic[1] = (int.Parse(GameDataDic[1]) - int.Parse(GameDataDic[11])).ToString();
                GameDataDic[6] = (int.Parse(GameDataDic[6]) - int.Parse(GameDataDic[11])).ToString();
                GameDataDic[7] += "买车支出" + int.Parse(GameDataDic[10]) + ",";
                break;
            case 14:
                if (int.Parse(GameDataDic[2]) - int.Parse(GameDataDic[12]) > 0){ 
                GameDataDic[1] = (int.Parse(GameDataDic[1]) + int.Parse(GameDataDic[9])).ToString();
                GameDataDic[2] = (int.Parse(GameDataDic[2]) - int.Parse(GameDataDic[12])).ToString();
                GameDataDic[6] = (int.Parse(GameDataDic[6]) + int.Parse(GameDataDic[9])).ToString();
                GameDataDic[7] += "打工收入" + int.Parse(GameDataDic[9]) + ",";
                }
                else
                {
                    Debug.Log("体力不够");
                }
                break;
            case 15:
                 ran = Random.Range(0, 2);
                if (ran % 2 == 0)
                {
                    GameDataDic[9] = ((int)(int.Parse(GameDataDic[9]) + 0.2f * int.Parse(GameDataDic[9]))).ToString();
                }
                else
                {
                    Debug.Log("加薪失败");
                }
                break;
            case 16:
                ran = Random.Range(0,2);
                if (ran % 2 == 0) { 
                GameDataDic[9] = ((int)(int.Parse(GameDataDic[9]) + 0.2f * int.Parse(GameDataDic[9]))).ToString();
                GameDataDic[12] = ((int)(int.Parse(GameDataDic[12]) - 0.2f * int.Parse(GameDataDic[12]))).ToString();
                }
                else
                {
                    Debug.Log("升职失败");
                }
                break;
            default:
                Debug.Log("功能暂不开放");
                break;
        }
        UpdateUI();
    }

    Dictionary<int, string> ItemName = new Dictionary<int, string>()
    {
        {0,"名字" },
        {1,"钱" },
        {2,"体力" },
        {3,"健康" },
        {4,"精神" },
        {5,"钻石" },
        {6,"回合流水" },
        {7,"失业日记" },
        {8,"岁数" },
        {9,"薪资"},
        {10,"房价"},
        {11,"车价" },
        {12,"工作消耗的体力" }
    };
    Dictionary<int, string> ButtonName = new Dictionary<int, string>()
    {
        {0,"首页" },
        {1,"增加体力" },
        {2,"增加钻石" },
        {3,"财产" },
        {4,"关系" },
        {5,"结束回合" },
        {6,"挣钱" },
        {7,"外出" },
        {8,"返回首页" },
        {9,"房产" },
        {10,"代步工具" },
        {11,"买房" },
        {12,"买车" },
        {13,"上一页" },//回到财产
        {14,"上班" },
        {15,"申请加薪" },
        {16,"申请升职" },
        {17,"跳槽" },
        {18,"辞职" },
        { 19,"头像按钮"}
    };
}
