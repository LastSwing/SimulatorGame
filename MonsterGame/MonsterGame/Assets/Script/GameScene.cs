using Assets.Script;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
    private GameObject btn_Return, btn_Atk, btn_ReturnAtk, DetailPanel, btn_Detail, btn_AutomaticAtk, AgainPanel, btn_Again, btn_AgainReturn;
    private Text txt_HP, txt_HPReply, txt_Dodge, txt_ATK, txt_Crit, txt_CritHarm, txt_CheckPoint, txt_Monster, txt_LevelMonster, txt_AutoAtk, txt_AutoState;//获取页面控件
    private InputField ipt_Atk, ipt_Detail;
    private Image img_Monster, img_Monster1;
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
        btn_AutomaticAtk = GameObject.Find("btn_AutomaticAtk");
        btn_AutomaticAtk.GetComponent<Button>().onClick.AddListener(AutomaticAtk);
        btn_Again = GameObject.Find("btn_Again");
        btn_Again.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("GameScene", 0); });
        btn_AgainReturn = GameObject.Find("AgainPanel/btn_Return");
        btn_AgainReturn.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("MainScene"); });

        txt_HP = GameObject.Find("txt_HP").GetComponent<Text>();
        txt_HPReply = GameObject.Find("txt_HPReply").GetComponent<Text>();
        txt_Dodge = GameObject.Find("txt_Dodge").GetComponent<Text>();
        txt_ATK = GameObject.Find("txt_ATK").GetComponent<Text>();
        txt_Crit = GameObject.Find("txt_Crit").GetComponent<Text>();
        txt_CritHarm = GameObject.Find("txt_CritHarm").GetComponent<Text>();
        txt_CheckPoint = GameObject.Find("txt_CheckPoint").GetComponent<Text>();//关卡
        txt_LevelMonster = GameObject.Find("txt_LevelMonster").GetComponent<Text>();//当前关卡的第几个怪
        txt_AutoAtk = GameObject.Find("btn_AutomaticAtk/txt_AutoAtk").GetComponent<Text>();//自动攻击按钮文本
        txt_AutoState = GameObject.Find("txt_AutoState").GetComponent<Text>();//自动攻击状态

        ipt_Atk = GameObject.Find("ipt_Atk").GetComponent<InputField>();//战斗文本
        ipt_Detail = GameObject.Find("ipt_Detail").GetComponent<InputField>();//详细信息

        DetailPanel = GameObject.Find("DetailPanel");
        AgainPanel = GameObject.Find("AgainPanel");

        #endregion
        if (Common.HasAgain == 0)
        {
            Init(null, 1);
        }
        else
        {
            var dic = GetLeveData();
            var monster = new Dictionary<int, object>();
            monster.Add(0, dic[0]);
            monster.Add(1, dic[1]);
            monster.Add(2, dic[2]);
            Init(JsonMapper.ToObject<field>(dic?[888].ToString()), System.Convert.ToInt32(dic?[999]), monster);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //游戏开始生成小怪
        if (txt_AutoState.text == "1")
        {
            var autoTxt = txt_AutoAtk.text;
            int levelMonster = System.Convert.ToInt32(txt_LevelMonster.text);
            float maxHp = Common.ConvertModel<field>(GameHelper.DataRead("Role/Role.txt")).HP;
            ipt_Detail.text = "";
            int level = System.Convert.ToInt32(GetLeveData()[999]);
            var InitHp = RoleFd.HP;
            //战斗详情添加到ipt_Detail
            bool victory = false;
            //判断是否是从第一关开始
            if (levelMonster == 0)
            {
                txt_AutoAtk.text = "暂停";
                if (int.TryParse(System.Convert.ToString(level / 30.00F), out int result))//boss
                {
                    field monster = Common.ConvertObject<field>(MonsterDic[99]);
                    var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
                }
                else   //小怪或灵泉
                {
                    for (int i = 0; i < MonsterDic.Count - 2; i++)
                    {
                        #region 战斗
                        field monster = MonsterDic[i] == null ? null : JsonMapper.ToObject<field>(MonsterDic[i].ToString());
                        if (monster == null || monster?.HP == 0)//灵泉或秘境
                        {
                            ipt_Detail.text += "\n遇到了灵泉或者秘境！";
                            ipt_Atk.text += "\n遇到了灵泉或者秘境！";
                            txt_AutoState.text = "0";
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
                                ipt_Atk.text += $"\n击败了{monster.Name}{(InitHp != newRole.HP ? $"，{(InitHp < newRole.HP ? "恢复" : "失去")}{System.Math.Abs(InitHp - newRole.HP)}点血量。" : "。")}";
                            RoleFd = newRole;
                            InitHp = newRole.HP;

                            #region 经验
                            if (monster.HP <= 0 && RoleFd.HP > 0)
                            {
                                float[] interval = { 0.5F, 1.5F };
                                int Exp = System.Convert.ToInt32(3 * level * Random.Range(interval[0], interval[1]) * GameHelper.hard);
                                //角色添加经验值
                                RoleAddEXP(Exp);
                                ipt_Atk.text += $"获得{Exp}点经验值。";
                                ipt_Detail.text += $"击败{monster.Name},获得{Exp}点经验值。";
                            }
                            #endregion
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
                    GameOver();
                }
                else
                {
                    //数值刷新
                    Init(RoleFd, level + 1);

                }
                txt_AutoAtk.text = "自动";
                txt_AutoState.text = "0";
            }
            else
            {
                while (levelMonster < 3)
                {
                    if (int.TryParse(System.Convert.ToString(level / 30.00F), out int result))//boss
                    {
                        field monster = Common.ConvertObject<field>(MonsterDic[99]);
                        var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
                    }
                    else   //小怪或灵泉
                    {
                        #region 战斗
                        field monster = MonsterDic[levelMonster] == null ? null : JsonMapper.ToObject<field>(MonsterDic[levelMonster].ToString());
                        if (monster == null || monster?.HP == 0)//灵泉或秘境
                        {
                            ipt_Detail.text += "\n遇到了灵泉或者秘境！";
                            ipt_Atk.text += "\n遇到了灵泉或者秘境！";
                            txt_AutoState.text = "0";
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
                                ipt_Atk.text += $"\n击败了{monster.Name}{(InitHp != newRole.HP ? $"，{(InitHp < newRole.HP ? "恢复" : "失去")}{System.Math.Abs(InitHp - newRole.HP)}点血量。" : "。")}";
                            RoleFd = newRole;
                            InitHp = newRole.HP;
                            #region 经验
                            if (monster.HP <= 0 && RoleFd.HP > 0)
                            {
                                float[] interval = { 0.5F, 1.5F };
                                int Exp = System.Convert.ToInt32(3 * level * Random.Range(interval[0], interval[1]) * GameHelper.hard);
                                //角色添加经验值
                                RoleAddEXP(Exp);
                                ipt_Atk.text += $"获得{Exp}点经验值。";
                                ipt_Detail.text += $"击败{monster.Name},获得{Exp}点经验值。";
                            }
                            #endregion
                        }
                        #endregion
                    }
                    //战斗完后
                    if (RoleFd.HP <= 0)
                    {
                        RoleFd.HP = 0;
                        Init(RoleFd, level);
                        //战斗结束。弹出窗口
                        GameOver();
                    }
                    else if (levelMonster == 2)
                    {
                        Init(RoleFd, level + 1);
                    }
                    else
                    {
                        //数值刷新
                        DataRefresh(RoleFd, levelMonster);
                        //数值刷新

                    }
                    levelMonster++;
                }
                txt_AutoState.text = "0";
            }
        }
    }

    void Init(field role, int level, Dictionary<int, object> monsterD = null)
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
        txt_LevelMonster.text = "0";
        #endregion

        #region 初始图片位置

        img_Monster = GameObject.Find("img_Monster0").GetComponent<Image>();
        img_Monster1 = GameObject.Find("img_Monster2").GetComponent<Image>();

        img_Monster1.transform.position = new Vector3(0, 2290, 0);
        img_Monster.transform.position = new Vector3(300, 700, 0);

        #endregion

        //int level = GetLeveData();
        MonsterDic = monsterD == null ? Level.CreateLevel(level) : monsterD;
        if (int.TryParse(System.Convert.ToString(level / 30.00F), out int result))//boss
        {
            field monster = Common.ConvertObject<field>(MonsterDic[99]);
        }
        else   //小怪或灵泉
        {
            for (int i = 0; i < MonsterDic.Count; i++)
            {
                #region 关卡生成
                field monster = MonsterDic[i] == null ? null : JsonMapper.ToObject<field>(MonsterDic[i].ToString());
                if (monster == null || monster?.HP == 0)//灵泉或秘境
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
        MonsterDic.Add(888, RoleFd);
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
        string json = JsonMapper.ToJson(monster);
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
    /// <summary>
    /// 获取当前关卡
    /// </summary>
    /// <returns></returns>
    private Dictionary<int, object> GetLeveData()
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
            StreamReader json = File.OpenText(path + @"\" + "LevelData/Level.txt");
            string input = json.ReadToEnd();
            dict = JsonMapper.ToObject<Dictionary<int, object>>(input);
        }
        return dict;
    }

    /// <summary>
    /// 攻击按钮
    /// </summary>
    private void ClickAtkBtn()
    {
        float maxHp = Common.ConvertModel<field>(GameHelper.DataRead("Role/Role.txt")).HP;
        int levelMonster = System.Convert.ToInt32(txt_LevelMonster.text);
        ipt_Detail.text = "";
        int level = System.Convert.ToInt32(GetLeveData()[999]);
        var InitHp = RoleFd.HP;
        var monsterName = "";
        //战斗详情添加到ipt_Detail
        bool victory = false;
        if (int.TryParse(System.Convert.ToString(level / 30.00F), out int result))//boss
        {
            field monster = Common.ConvertObject<field>(MonsterDic[99]);
            monsterName = monster.Name;
            var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
        }
        else   //小怪或灵泉
        {
            //for (int i = 0; i < MonsterDic.Count - 1; i++)
            //{
            #region 战斗
            field monster = MonsterDic[levelMonster] == null ? null : JsonMapper.ToObject<field>(MonsterDic[levelMonster].ToString());
            if (monster == null || monster?.HP == 0)//灵泉或秘境
            {
                ipt_Detail.text += "\n遇到了灵泉或者秘境！";
                ipt_Atk.text += "\n遇到了灵泉或者秘境！";
            }
            else
            {
                var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
                var newRole = Common.ConvertObject<field>(Atk["Role"]);
                //var Monster = Common.ConvertObject<field>(Atk["Monster"]);
                var AtkDetail = (IList<string>)Atk["AtkDetail"];

                foreach (var item in AtkDetail)
                {
                    ipt_Detail.text += item;
                }
                if (newRole.HP <= 0)
                    ipt_Atk.text = $"被{monster.Name}击杀了你，游戏失败！";
                else
                    ipt_Atk.text += $"\n击败了{monster.Name}{(InitHp != newRole.HP ? $"，{(InitHp < newRole.HP ? "恢复" : "失去")}{System.Math.Abs(InitHp - newRole.HP)}点血量。" : "。")}";
                RoleFd = newRole;
                InitHp = newRole.HP;

                #region 经验
                if (monster.HP <= 0 && RoleFd.HP > 0)
                {
                    float[] interval = { 0.5F, 1.5F };
                    int Exp = System.Convert.ToInt32(3 * level * Random.Range(interval[0], interval[1]) * GameHelper.hard);
                    //角色添加经验值
                    RoleAddEXP(Exp);
                    ipt_Atk.text += $"获得{Exp}点经验值。";
                    ipt_Detail.text += $"击败{monster.Name},获得{Exp}点经验值。";
                }
                #endregion
            }
            #endregion
            //}
        }
        //战斗完后
        if (RoleFd.HP <= 0)
        {
            RoleFd.HP = 0;
            Init(RoleFd, level);
            //战斗结束。弹出窗口
            GameOver();
        }
        else if (levelMonster == 2)
        {
            Init(RoleFd, level + 1);
        }
        else
        {
            //数值刷新
            DataRefresh(RoleFd, levelMonster);

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
    /// <summary>
    /// 数据刷新
    /// </summary>
    private void DataRefresh(field role, int imgNo)
    {
        #region 野怪图片更新
        img_Monster = GameObject.Find("img_Monster" + imgNo).GetComponent<Image>();
        img_Monster1 = GameObject.Find("img_Monster" + (imgNo + 1)).GetComponent<Image>();
        img_Monster.transform.position = new Vector3(0, 2290, 0);
        img_Monster1.transform.position = new Vector3(300, 700, 0);
        #endregion

        #region 数值刷新
        txt_HP.text = role.HP.ToString();
        txt_HPReply.text = role.HPRegen.ToString();
        txt_Dodge.text = role.Dodge + "%";
        txt_ATK.text = role.Atk.ToString();
        txt_Crit.text = role.Crit + "%";
        txt_CritHarm.text = role.CritHarm + "%";
        txt_LevelMonster.text = (imgNo + 1).ToString();
        #endregion
    }
    /// <summary>
    /// 自动攻击
    /// </summary>
    private void AutomaticAtk()
    {
        //在 update中修改展示
        txt_AutoState.text = "1";
        #region 注释
        //var autoTxt = txt_AutoAtk.text;
        //int levelMonster = System.Convert.ToInt32(txt_LevelMonster.text);
        //float maxHp = Common.ConvertModel<field>(GameHelper.DataRead("Role/Role.txt")).HP;
        //ipt_Detail.text = "";
        //int level = GetLeveData();
        //var InitHp = RoleFd.HP;
        ////战斗详情添加到ipt_Detail
        //bool victory = false;
        ////判断是否是从第一关开始
        //if (levelMonster == 0)
        //{
        //    if (autoTxt == "自动")
        //    {
        //        txt_AutoAtk.text = "暂停";
        //        if (int.TryParse(System.Convert.ToString(level / 30.00F), out int result))//boss
        //        {
        //            field monster = Common.ConvertObject<field>(MonsterDic[99]);
        //            var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
        //        }
        //        else   //小怪或灵泉
        //        {
        //            for (int i = 0; i < MonsterDic.Count - 2; i++)
        //            {
        //                #region 战斗
        //                field monster = Common.ConvertObject<field>(MonsterDic[i]);
        //                if (monster?.HP == 0)//灵泉或秘境
        //                {
        //                    ipt_Detail.text += "\n遇到了灵泉或者秘境！";
        //                    ipt_Atk.text += "\n遇到了灵泉或者秘境！";
        //                }
        //                else
        //                {
        //                    var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
        //                    var newRole = Common.ConvertObject<field>(Atk["Role"]);
        //                    var Monster = Common.ConvertObject<field>(Atk["Monster"]);
        //                    var AtkDetail = (IList<string>)Atk["AtkDetail"];

        //                    foreach (var item in AtkDetail)
        //                    {
        //                        ipt_Detail.text += item;
        //                    }
        //                    if (newRole.HP <= 0)
        //                        ipt_Atk.text = $"被{monster.Name}击杀了你，游戏失败！";
        //                    else
        //                        ipt_Atk.text += $"\n击败了{monster.Name}{(InitHp != newRole.HP ? $"，{(InitHp < newRole.HP ? "恢复" : "失去")}{System.Math.Abs(InitHp - newRole.HP)}点血量。" : "。")}";
        //                    RoleFd = newRole;
        //                    InitHp = newRole.HP;
        //                }
        //                #endregion
        //            }
        //        }
        //        //战斗完后
        //        if (RoleFd.HP <= 0)
        //        {
        //            RoleFd.HP = 0;
        //            Init(RoleFd, level);
        //            //战斗结束。弹出窗口
        //            GameOver();
        //        }
        //        else
        //        {
        //            //数值刷新
        //            Init(RoleFd, level + 1);

        //        }
        //        txt_AutoAtk.text = "自动";
        //    }
        //    else
        //    {
        //        txt_AutoAtk.text = "自动";

        //    }
        //}
        //else
        //{
        //    while (levelMonster < 3)
        //    {
        //        if (int.TryParse(System.Convert.ToString(level / 30.00F), out int result))//boss
        //        {
        //            field monster = Common.ConvertObject<field>(MonsterDic[99]);
        //            var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
        //        }
        //        else   //小怪或灵泉
        //        {
        //            #region 战斗
        //            field monster = Common.ConvertObject<field>(MonsterDic[levelMonster]);
        //            if (monster?.HP == 0)//灵泉或秘境
        //            {
        //                ipt_Detail.text += "\n遇到了灵泉或者秘境！";
        //                ipt_Atk.text += "\n遇到了灵泉或者秘境！";
        //            }
        //            else
        //            {
        //                var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
        //                var newRole = Common.ConvertObject<field>(Atk["Role"]);
        //                var Monster = Common.ConvertObject<field>(Atk["Monster"]);
        //                var AtkDetail = (IList<string>)Atk["AtkDetail"];

        //                foreach (var item in AtkDetail)
        //                {
        //                    ipt_Detail.text += item;
        //                }
        //                if (newRole.HP <= 0)
        //                    ipt_Atk.text = $"被{monster.Name}击杀了你，游戏失败！";
        //                else
        //                    ipt_Atk.text += $"\n击败了{monster.Name}{(InitHp != newRole.HP ? $"，{(InitHp < newRole.HP ? "恢复" : "失去")}{System.Math.Abs(InitHp - newRole.HP)}点血量。" : "。")}";
        //                RoleFd = newRole;
        //                InitHp = newRole.HP;
        //            }
        //            #endregion
        //        }
        //        //战斗完后
        //        if (RoleFd.HP <= 0)
        //        {
        //            RoleFd.HP = 0;
        //            Init(RoleFd, level);
        //            //战斗结束。弹出窗口
        //            GameOver();
        //        }
        //        else if (levelMonster == 2)
        //        {
        //            Init(RoleFd, level + 1);
        //        }
        //        else
        //        {
        //            //数值刷新
        //            DataRefresh(RoleFd, levelMonster);
        //            //数值刷新

        //        }
        //        levelMonster++;
        //    }
        //} 
        #endregion

    }


    /// <summary>
    /// 游戏结束
    /// </summary>
    private void GameOver()
    {
        AgainPanel.transform.position = new Vector3(300, 600, 0);
    }

    /// <summary>
    /// 角色添加经验值
    /// </summary>
    private void RoleAddEXP(int exp)
    {
        var role = Common.ConvertModel<field>(GameHelper.DataRead("Role/Role.txt"));
        role.EXP += exp;
        string json = JsonMapper.ToJson(role);
        //json = GameHelper.DesEncrypt(json);//前期不加密
        var path = Application.dataPath + "/Data/Role";
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists($"{path}/Role.txt"))
            File.Delete($"{path}/Role.txt");
        File.WriteAllText($"{path}/Role.txt", json);
    }
}
