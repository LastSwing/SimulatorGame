using Assets.Script;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
    private GameObject btn_Return, btn_Atk, btn_ReturnAtk, DetailPanel, btn_Detail;
    private Text txt_HP, txt_HPReply, txt_Dodge, txt_ATK, txt_Crit, txt_CritHarm, txt_CheckPoint, txt_Monster;//获取页面控件
    private InputField ipt_Atk, ipt_Detail;
    private Image img_Monster;
    private Dictionary<int, object> MonsterDic;
    private field RoleFd;
    // Start is called before the first frame update
    void Start()
    {
        #region 控件绑定
        btn_Return = GameObject.Find("btn_Return");
        btn_Return.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("MainScene"); });
        btn_Atk = GameObject.Find("btn_Atk");
        btn_Atk.GetComponent<Button>().onClick.AddListener(ClickAtkBtn);
        btn_ReturnAtk = GameObject.Find("btn_ReturnAtk");
        btn_ReturnAtk.GetComponent<Button>().onClick.AddListener(ReturnAtk);
        btn_Detail = GameObject.Find("btn_Detail");
        btn_Detail.GetComponent<Button>().onClick.AddListener(ShowDetail);

        txt_HP = GameObject.Find("txt_HP").GetComponent<Text>();
        txt_HPReply = GameObject.Find("txt_HPReply").GetComponent<Text>();
        txt_Dodge = GameObject.Find("txt_Dodge").GetComponent<Text>();
        txt_ATK = GameObject.Find("txt_ATK").GetComponent<Text>();
        txt_Crit = GameObject.Find("txt_Crit").GetComponent<Text>();
        txt_CritHarm = GameObject.Find("txt_CritHarm").GetComponent<Text>();
        txt_CheckPoint = GameObject.Find("txt_CheckPoint").GetComponent<Text>();//关卡

        ipt_Atk = GameObject.Find("ipt_Atk").GetComponent<InputField>();//战斗文本
        ipt_Detail = GameObject.Find("ipt_Detail").GetComponent<InputField>();//详细信息

        DetailPanel = GameObject.Find("DetailPanel");

        #endregion
        Init(null, 1);
    }

    // Update is called once per frame
    void Update()
    {
        //游戏开始生成小怪
    }

    void Init(field role, int level)
    {
        #region 基础数据绑定

        RoleFd = role == null ? Common.ConvertModel<field>(GameHelper.DataRead("Role/Role.txt")) : role;
        txt_HP.text = RoleFd.HP.ToString();
        txt_HPReply.text = RoleFd.HPRegen.ToString();
        txt_Dodge.text = RoleFd.Dodge + "%";
        txt_ATK.text = RoleFd.Atk.ToString();
        txt_Crit.text = RoleFd.Crit + "%";
        txt_CritHarm.text = RoleFd.CritHarm + "%";
        txt_CheckPoint.text = $"厚土劫·初（{level}/300）";
        ipt_Atk.text += $"{(level == 1 ? "" : "\n")}厚土劫·初（{level}/300）";
        #endregion

        //int level = GetLeveData();
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
                if (monster?.HP == 0)//灵泉或秘境
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
        //json = GameHelper.DesEncrypt(json);//前期不加密
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
            //string json = GameHelper.DesDecrypt(File.ReadAllText(path + @"\" + "LevelData/Level.txt"));
            string json = File.ReadAllText(path + @"\" + "LevelData/Level.txt");
            dict = JsonConvert.DeserializeObject<Dictionary<int, object>>(json);
        }
        return System.Convert.ToInt32(dict[999]);
    }

    /// <summary>
    /// 攻击按钮
    /// </summary>
    private void ClickAtkBtn()
    {
        float maxHp = Common.ConvertModel<field>(GameHelper.DataRead("Role/Role.txt")).HP;
        ipt_Detail.text = "";
        int level = GetLeveData();
        var InitHp = RoleFd.HP;
        //战斗详情添加到ipt_Detail
        bool victory = false;
        if (int.TryParse(System.Convert.ToString(level / 30.00F), out int result))//boss
        {
            field monster = Common.ConvertObject<field>(MonsterDic[99]);
            var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
        }
        else   //小怪或灵泉
        {
            for (int i = 0; i < MonsterDic.Count - 1; i++)
            {
                #region 战斗
                field monster = Common.ConvertObject<field>(MonsterDic[i]);
                if (monster?.HP == 0)//灵泉或秘境
                {
                    ipt_Detail.text += "\n遇到了灵泉或者秘境！";
                    ipt_Atk.text += "\n遇到了灵泉或者秘境！";
                }
                else
                {
                    var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
                    var newRole = Common.ConvertObject<field>(Atk["Role"]);
                    var Monster = Common.ConvertObject<field>(Atk["Monster"]);
                    var AtkDetail = (IList<string>)Atk["AtkDetail"];

                    foreach (var item in AtkDetail)
                    {
                        ipt_Detail.text += item;
                    }
                    if (newRole.HP <= 0)
                        ipt_Atk.text = $"被{monster.Name}击杀了你，游戏失败！";
                    else
                        ipt_Atk.text += $"\n击败了{monster.Name}{(InitHp != newRole.HP ? $"，失去{InitHp - newRole.HP}点血量。" : "。")}";
                    RoleFd = newRole;
                    InitHp = newRole.HP;
                }
                #endregion
            }
        }
        //战斗完后
        if (RoleFd.HP <= 0)
        {
            RoleFd.HP = 0;
            Init(RoleFd, level);
            //战斗结束。弹出窗口
        }
        else
        {
            //页面数据刷新
            Init(RoleFd, level + 1);
        }
    }

    /// <summary>
    /// 返回战斗页面
    /// </summary>
    private void ReturnAtk()
    {
        DetailPanel.transform.position = new Vector3(0, -2178, 0);
    }
    /// <summary>
    /// 展示战斗详情
    /// </summary>
    private void ShowDetail()
    {
        DetailPanel.transform.position = new Vector3(300, 600, 0);
    }
}
