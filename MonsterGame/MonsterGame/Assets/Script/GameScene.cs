using Assets.Script;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
    private GameObject btn_Return, btn_Atk;
    private Text txt_HP, txt_HPReply, txt_Dodge, txt_ATK, txt_Crit, txt_CritHarm, txt_CheckPoint, txt_Monster;//获取页面控件
    private Input ipt_Atk;
    private Image img_Monster;
    private Dictionary<int, object> MonsterDic;
    // Start is called before the first frame update
    void Start()
    {
        #region 控件绑定
        btn_Return = GameObject.Find("btn_Return");
        btn_Return.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("MainScene"); });
        btn_Atk = GameObject.Find("btn_Atk");
        btn_Atk.GetComponent<Button>().onClick.AddListener(ClickAtkBtn);

        txt_HP = GameObject.Find("txt_HP").GetComponent<Text>();
        txt_HPReply = GameObject.Find("txt_HPReply").GetComponent<Text>();
        txt_Dodge = GameObject.Find("txt_Dodge").GetComponent<Text>();
        txt_ATK = GameObject.Find("txt_ATK").GetComponent<Text>();
        txt_Crit = GameObject.Find("txt_Crit").GetComponent<Text>();
        txt_CritHarm = GameObject.Find("txt_CritHarm").GetComponent<Text>();
        txt_CheckPoint = GameObject.Find("txt_CheckPoint").GetComponent<Text>();//关卡

        //ipt_Atk = GameObject.Find("GameCanvas/GameObject/ipt_Atk").GetComponent<Input>();

        #endregion
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        //游戏开始生成小怪

    }

    void Init()
    {
        #region 基础数据绑定

        Dictionary<string, string> dic = GameHelper.DataRead("Role/Role.txt");
        txt_HP.text = dic["HP"];
        txt_HPReply.text = dic["HPRegen"];
        txt_Dodge.text = dic["Dodge"] + "%";
        txt_ATK.text = dic["Atk"];
        txt_Crit.text = dic["Crit"] + "%";
        txt_CritHarm.text = dic["CritHarm"] + "%";
        txt_CheckPoint.text = "厚土劫·初（1/300）";

        #endregion

        int level = GetLeveData();
        MonsterDic = Level.CreateLevel(level);
        if (int.TryParse(System.Convert.ToString(level / 30.00F), out int result))//boss
        {
            field monster = Common.ConvertObject<field>(MonsterDic[99]);
        }
        else   //小怪或灵泉
        {
            for (int i = 0; i < MonsterDic.Count; i++)
            {
                #region 关卡生成
                field monster = Common.ConvertObject<field>(MonsterDic[i]);
                if (monster.HP == 0)//灵泉或秘境
                {
                    Debug.Log("秘境或灵泉");
                }
                else
                {
                    txt_Monster = GameObject.Find($"img_Monster{i}/txt_Monster{i}").GetComponent<Text>();
                    img_Monster = GameObject.Find("img_Monster" + i).GetComponent<Image>();
                    txt_Monster.text = $"{monster.Name}（{monster.HP}/{monster.HP}）";//名称和血量
                    string MonsterPath = "";
                    switch (monster.Name)
                    {
                        case "树精":
                            MonsterPath = "Images/ShuJin";
                            break;
                        case "通灵石怪":
                            MonsterPath = "Images/ShiGuai";
                            break;
                        case "蛇妖":
                            MonsterPath = "Images/SheYao";
                            break;
                        case "蝎子精":
                            MonsterPath = "Images/XieZi";
                            break;
                        case "狐妖":
                            MonsterPath = "Images/HuYao";
                            break;
                        default:
                            MonsterPath = "Images/ShiGuai";
                            break;
                    }
                    Sprite img = Resources.Load(MonsterPath, typeof(Sprite)) as Sprite;
                    img_Monster.sprite = img;//图片
                }
                #endregion
            }
        }
        //当前关卡数据存储到文本
        LevelDataSave(MonsterDic, level);
    }

    /// <summary>
    /// 当前关卡数据保存到文本
    /// </summary>
    /// <param name="dic">野怪数据</param>
    /// <param name="level">当前关卡</param>
    private void LevelDataSave(Dictionary<int, object> monster, int level)
    {
        monster.Add(999, level);//关卡
        string json = JsonConvert.SerializeObject(monster);
        json = GameHelper.DesEncrypt(json);
        var path = Application.dataPath + "/Data/LevelData";
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists($"{path}/Level.txt"))
            File.Delete($"{path}/Level.txt");
        File.WriteAllText($"{path}/Level.txt", json);
    }

    private int GetLeveData()
    {
        var path = Application.dataPath + "/Data/";
        Dictionary<int, object> dict = new Dictionary<int, object>();
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists(path + @"\" + "LevelData/Level.txt"))
        {
            string json = GameHelper.DesDecrypt(File.ReadAllText(path + @"\" + "LevelData/Level.txt"));
            dict = JsonConvert.DeserializeObject<Dictionary<int, object>>(json);
        }
        return System.Convert.ToInt32(dict[999]);
    }

    /// <summary>
    /// 攻击按钮
    /// </summary>
    private void ClickAtkBtn()
    {

    }
}
