using Assets.Script;
using Assets.Script.FSM;
using Assets.Script.UIScript;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameScene : MonoBehaviour
{
    StateMachine<GameScene> myStateMachine;
    private GameObject btn_Return, btn_Atk, btn_ReturnAtk, DetailPanel, btn_Detail, btn_AutomaticAtk, AgainPanel, btn_Again, btn_AgainReturn, HippocrenePanel, btn_Restore_50, btn_Add_20, btn_Restore_20, btn_ReturnGame;
    private Text txt_HP, txt_HPReply, txt_Dodge, txt_ATK, txt_Crit, txt_CritHarm, txt_CheckPoint, txt_Monster, txt_LevelMonster, txt_AutoAtk;//获取页面控件
    public Text txt_AutoState, txt_AtkState;
    private InputField ipt_Atk, ipt_Detail;
    private Image img_Monster, img_Monster1;
    private RectTransform img_rect, txt_rect, img_rect_temp, txt_rect_temp;
    private Dictionary<string, object> MonsterDic;
    private field RoleFd, monster;
    private Scrollbar TalkWinBar;
    private bool coroutine = false;//是否释放攻击按钮
    private bool hasHippocrene = false;//释放灵泉面板
    private bool hasPlayText = false;//文字播放完成
    public bool HippocreneOptions, HippocreneOver = false; //灵泉选项

    // 定义每帧累加时间
    private float totalTimer;
    // 定义变量存储时间
    private int second, no = 0;
    public string temp, AtkText, AtkDetail = "";
    int round = 0;//0是角色回合，1是野怪回合，2是回合结束


    void Start()
    {
        #region 控件绑定
        btn_Return = transform.Find("btn_Return").gameObject;
        btn_Return.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("MainScene"); });
        btn_Atk = transform.Find("btn_Atk").gameObject;
        btn_Atk.GetComponent<Button>().onClick.AddListener(ClickAtkBtn);
        btn_ReturnAtk = transform.Find("DetailPanel/btn_ReturnAtk").gameObject;
        btn_ReturnAtk.GetComponent<Button>().onClick.AddListener(ReturnAtk);
        btn_Detail = transform.Find("btn_Detail").gameObject;
        btn_Detail.GetComponent<Button>().onClick.AddListener(ShowDetail);
        btn_AutomaticAtk = transform.Find("btn_AutomaticAtk").gameObject;
        btn_AutomaticAtk.GetComponent<Button>().onClick.AddListener(AutomaticAtk);
        btn_Again = transform.Find("AgainPanel/btn_Again").gameObject;
        btn_Again.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("GameScene", 0); });
        btn_AgainReturn = transform.Find("AgainPanel/btn_Return").gameObject;
        btn_AgainReturn.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("MainScene"); });
        btn_Restore_50 = transform.Find("HippocrenePanel/btn_Restore_50").gameObject;
        btn_Restore_50.GetComponent<Button>().onClick.AddListener(Restore50);
        btn_Add_20 = transform.Find("HippocrenePanel/btn_Add_20").gameObject;
        btn_Add_20.GetComponent<Button>().onClick.AddListener(HpAdd);
        btn_Restore_20 = transform.Find("HippocrenePanel/btn_Restore_20").gameObject;
        btn_Restore_20.GetComponent<Button>().onClick.AddListener(Restore20);
        btn_ReturnGame = transform.Find("AgainPanel/btn_ReturnGame").gameObject;
        btn_ReturnGame.GetComponent<Button>().onClick.AddListener(ShowGame);


        txt_HP = transform.Find("t_HP/txt_HP").GetComponent<Text>();
        txt_HPReply = transform.Find("t_HPReply/txt_HPReply").GetComponent<Text>();
        txt_Dodge = transform.Find("t_Dodge/txt_Dodge").GetComponent<Text>();
        txt_ATK = transform.Find("t_ATK/txt_ATK").GetComponent<Text>();
        txt_Crit = transform.Find("t_Crit/txt_Crit").GetComponent<Text>();
        txt_CritHarm = transform.Find("t_CritHarm/txt_CritHarm").GetComponent<Text>();
        txt_CheckPoint = transform.Find("txt_CheckPoint").GetComponent<Text>();//关卡
        txt_LevelMonster = transform.Find("txt_LevelMonster").GetComponent<Text>();//当前关卡的第几个怪
        txt_AutoAtk = transform.Find("btn_AutomaticAtk/txt_AutoAtk").GetComponent<Text>();//自动攻击按钮文本
        txt_AutoState = transform.Find("txt_AutoState").GetComponent<Text>();//自动攻击状态
        txt_AtkState = transform.Find("txt_AtkState").GetComponent<Text>();//攻击状态


        TalkWinBar = transform.Find("Battle/Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>();//滑动框
        ipt_Atk = transform.Find("Battle/Scroll View/Viewport/sv_Content/ipt_Atk").GetComponent<InputField>();//战斗文本
        ipt_Atk.onValueChanged.AddListener(UpdateDetail);
        ipt_Detail = transform.Find("DetailPanel/sv_Detail/v_Detail/svc_Detail/ipt_Detail").GetComponent<InputField>();//详细信息


        DetailPanel = transform.Find("DetailPanel").gameObject;
        AgainPanel = transform.Find("AgainPanel").gameObject;
        HippocrenePanel = transform.Find("HippocrenePanel").gameObject;

        #endregion

        #region 状态机初始化
        myStateMachine = new StateMachine<GameScene>(this);
        myStateMachine.SetCurrentState(GameScene_StateIdle.Instance);
        myStateMachine.SetGlobalState(GameScene_GlobalState.Instance);
        #endregion

        //数据初始化
        #region 是否已存档
        var role = Common.ConvertModel<field>(GameHelper.DataRead("Role/Role.txt"));
        if (Common.HasAgain == 0)
        {
            Init(role, 1);
            SaveCurrentRole(role);
        }
        else
        {
            var dic = GetLeveData();
            var monster = new Dictionary<string, object>();
            for (int i = 0; i < 3; i++)
            {
                monster.Add(i.ToString(), dic[i.ToString()].ToString() == "" ? null : Common.JsonToModel<field>(dic[i.ToString()].ToString()));
            }
            Init(Common.JsonToModel<field>(dic?["888"].ToString()), System.Convert.ToInt32(dic?["999"]), monster);
        }
        #endregion
    }
    // Update is called once per frame
    void Update()
    {
        myStateMachine.FSMUpdate();
        #region 是否释放攻击按钮
        if (coroutine)
        {
            Debug.Log(coroutine);
            myStateMachine.ChangeState(GameScene_StateIdle.Instance);
            coroutine = false;
        }
        #endregion

        #region 展示灵泉秘境画板
        if (hasHippocrene)
        {
            ShowHippocrene();
            hasHippocrene = false;
        }
        #endregion
        #region 灵泉选项完成后刷新数据

        if (HippocreneOver)
        {
            //灵泉选项完成后刷新数据
            int levelMonster = System.Convert.ToInt32(txt_LevelMonster.text);
            if (levelMonster == 2)
            {
                var currentLevelDic = GetLeveData();//当前关卡数据
                int level = System.Convert.ToInt32(currentLevelDic["999"]);
                Init(RoleFd, level + 1);
            }
            else if (txt_AtkState.text == "0")
            {
                //数值刷新
                DataRefresh(RoleFd, levelMonster);

            }
            HippocreneOver = false;
        }
        #endregion
    }

    /// <summary>
    /// 数据初始化
    /// </summary>
    /// <param name="role"></param>
    /// <param name="level"></param>
    /// <param name="monsterD"></param>
    public void Init(field role, int level, Dictionary<string, object> monsterD = null)
    {
        #region 基础数据绑定

        RoleFd = role == null ? Common.ConvertModel<field>(GameHelper.DataRead("Role/Role.txt")) : role;
        float maxHp = GetCurrentRole().HP;
        txt_HP.text = $"({RoleFd.HP}/{maxHp})";
        txt_HPReply.text = RoleFd.HPRegen.ToString();
        txt_Dodge.text = RoleFd.Dodge + "%";
        txt_ATK.text = RoleFd.Atk.ToString();
        txt_Crit.text = RoleFd.Crit + "%";
        txt_CritHarm.text = RoleFd.CritHarm + "%";
        txt_CheckPoint.text = $"厚土劫·初 \n（{level}/300）";
        ipt_Atk.text += $"{(level == 1 ? "" : "\n")}厚土劫·初（{level}/300）";
        txt_LevelMonster.text = "0";
        AtkDetail = "";
        AtkText = "";
        hasPlayText = false;
        no = 0;
        #endregion

        #region 图片初始化

        //图1
        GameObject.Find("img_Monster0").transform.localScale = Vector3.one;
        GameObject.Find("img_Monster1").transform.localScale = Vector3.one;
        //图2
        GameObject.Find($"img_Monster1/txt_Monster1").GetComponent<Text>().fontSize = 23;
        img_rect_temp = GameObject.Find("img_Monster1").GetComponent<RectTransform>();
        img_rect_temp.sizeDelta = new Vector2(225f, 230f);
        img_rect_temp.anchoredPosition = new Vector2(362f, 187f);
        txt_rect_temp = GameObject.Find("img_Monster1/txt_Monster1").GetComponent<RectTransform>();
        txt_rect_temp.sizeDelta = new Vector2(255f, 56f);
        txt_rect_temp.anchoredPosition = new Vector2(0f, 143f);
        //图3
        GameObject.Find($"img_Monster2/txt_Monster2").GetComponent<Text>().fontSize = 23;
        img_rect_temp = GameObject.Find("img_Monster2").GetComponent<RectTransform>();
        img_rect_temp.sizeDelta = new Vector2(225f, 230f);
        img_rect_temp.anchoredPosition = new Vector2(-362f, 187f);
        txt_rect_temp = GameObject.Find("img_Monster2/txt_Monster2").GetComponent<RectTransform>();
        txt_rect_temp.sizeDelta = new Vector2(255f, 56f);
        txt_rect_temp.anchoredPosition = new Vector2(0f, 143f);

        #endregion

        MonsterDic = monsterD == null ? Level.CreateLevel(level) : monsterD;
        if (int.TryParse(System.Convert.ToString(level / 30.00F), out int result))//boss
        {
            field monster = Common.ConvertObject<field>(MonsterDic["99"]);
        }
        else   //小怪或灵泉
        {
            #region 图片绑定
            for (int i = 0; i < MonsterDic.Count; i++)
            {
                #region 关卡生成
                field monster = Common.ConvertObject<field>(MonsterDic[i.ToString()]);
                if (monster == null || monster?.HP == 0)//灵泉或秘境
                {
                    //Debug.Log("秘境或灵泉");
                    txt_Monster = GameObject.Find($"img_Monster{i}/txt_Monster{i}").GetComponent<Text>();
                    txt_Monster.text = "恢复灵泉";
                    img_Monster = GameObject.Find("img_Monster" + i).GetComponent<Image>();
                    Sprite img = Resources.Load("Images/QuanShui", typeof(Sprite)) as Sprite;
                    img_Monster.sprite = img;//图片
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
            #endregion
        }
        //当前关卡数据存储到文本
        MonsterDic.Add("888", RoleFd);
        LevelDataSave(MonsterDic, level);
    }


    #region 按钮
    /// <summary>
    /// 攻击按钮
    /// </summary>
    private void ClickAtkBtn()
    {
        txt_AtkState.text = "1";
    }

    /// <summary>
    /// 返回战斗页面
    /// </summary>
    private void ReturnAtk()
    {
        //DetailPanel.transform.position = new Vector3(0, -2178, 0);
        DetailPanel.SetActive(false);
    }
    /// <summary>
    /// 展示战斗详情
    /// </summary>
    private void ShowDetail()
    {
        //DetailPanel.transform.position = new Vector3(300, 600, 0);
        DetailPanel.SetActive(true);
        ipt_Detail.text = AtkDetail;
    }

    /// <summary>
    /// 自动攻击
    /// </summary>
    private void AutomaticAtk()
    {
        //在 update中修改展示

        var autoTxt = txt_AutoAtk.text;
        if (autoTxt == "自动")
        {
            txt_AutoState.text = "1";
            txt_AutoAtk.text = "暂停";
        }
        else
        {
            txt_AutoState.text = "0";
            txt_AutoAtk.text = "自动";
        }
    }

    /// <summary>
    /// 恢复50%的血量
    /// </summary>
    private void Restore50()
    {
        float maxHp = GetCurrentRole().HP;
        var RestoreHp = Convert.ToInt32(maxHp * 0.5);
        var currentHp = RoleFd.HP;
        RoleFd.HP += RestoreHp;
        if (RoleFd.HP > maxHp)
        {
            RoleFd.HP = maxHp;
        }
        HippocrenePanel.SetActive(false);
        UpdateRoleHP(RoleFd.HP);
        AtkDetail += $"【遇到了灵泉恢复了{RoleFd.HP - currentHp}点血量！】\n\n";
        AtkText += $"遇到了灵泉恢复了{RoleFd.HP - currentHp}点血量！";
        BtnLoseEfficacy();
        second = AtkText.Length;
        HippocreneOptions = true;
        PlayText(AtkText);
    }

    /// <summary>
    /// 恢复20%-100%的血量
    /// </summary>
    private void Restore20()
    {
        float maxHp = GetCurrentRole().HP;
        var RestoreHp = Convert.ToInt32(maxHp * Random.Range(0.2F, 1.01F));
        var currentHp = RoleFd.HP;
        RoleFd.HP += RestoreHp;
        if (RoleFd.HP > maxHp)
        {
            RoleFd.HP = maxHp;
        }
        HippocrenePanel.SetActive(false);
        UpdateRoleHP(RoleFd.HP);
        AtkDetail += $"【遇到了灵泉恢复了{RoleFd.HP - currentHp}点血量！】\n\n";
        AtkText += $"遇到了灵泉恢复了{RoleFd.HP - currentHp}点血量！";
        BtnLoseEfficacy();
        second = AtkText.Length;
        HippocreneOptions = true;
        PlayText(AtkText);
    }

    /// <summary>
    /// 加最大生命值
    /// </summary>
    private void HpAdd()
    {
        var role = GetCurrentRole();
        role.HP += 20;
        SaveCurrentRole(role);
        HippocrenePanel.SetActive(false);
        AtkDetail += $"【遇到了灵泉增加了20点最大生命值！】\n\n";
        AtkText += $"遇到了灵泉增加了20点最大生命值！";
        BtnLoseEfficacy();
        second = AtkText.Length;
        HippocreneOptions = true;
        PlayText(AtkText);
    }

    #endregion


    #region 方法
    /// <summary>
    /// 自动攻击
    /// </summary>
    public void AutoATK()
    {
        totalTimer += Time.deltaTime;
        int levelMonster = System.Convert.ToInt32(txt_LevelMonster.text);
        var currentLevelDic = GetLeveData();//当前关卡数据
        float maxHp = GetCurrentRole().HP;
        int level = System.Convert.ToInt32(currentLevelDic["999"]);
        var InitHp = RoleFd.HP;
        //战斗详情添加到ipt_Detail


        #region 战斗

        //攻击不能用循环要用状态机，1秒进行一轮攻击
        if (totalTimer >= 1 && round != 2)
        {
            if (int.TryParse(System.Convert.ToString(level / 30.00F), out int result))//boss
            {
                monster = Common.ConvertObject<field>(MonsterDic["99"]);
                //var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
                ATK(levelMonster, maxHp, level, InitHp);
            }
            else   //小怪或灵泉
            {
                monster = Common.ConvertObject<field>(MonsterDic[levelMonster.ToString()]);
                if (monster == null || monster?.HP == 0)//灵泉或秘境
                {
                    //AtkDetail += "\n遇到了灵泉或者秘境！";
                    //AtkText += "遇到了灵泉或者秘境！";
                    hasHippocrene = true;
                    txt_AutoState.text = "0";
                    txt_AutoAtk.text = "自动";
                }
                else
                {
                    //var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
                    //1s一次攻击
                    ATK(levelMonster, maxHp, level, InitHp);
                }
            }
            totalTimer = 0;
            //野怪、角色数值刷新
            MonsterDic[levelMonster.ToString()] = monster;
            second = AtkText.Length;
        }

        #endregion

        if (round == 2)//战斗完成
        {
            InitHp = RoleFd.HP;
            PlayText(AtkText);
            //战斗完后
            if (RoleFd.HP <= 0)
            {
                RoleFd.HP = 0;
                //Init(RoleFd, level);
                txt_AutoState.text = "0";
                txt_AutoAtk.text = "自动";
                //战斗结束。弹出窗口
                GameOver();
            }
            else if (hasPlayText && levelMonster == 2)
            {
                Init(RoleFd, level + 1);
            }
            else if (hasPlayText)
            {
                //数值刷新
                DataRefresh(RoleFd, levelMonster);
            }
        }

    }
    /// <summary>
    /// 攻击
    /// </summary>
    /// <param name="levelMonster">当前关卡的野怪序号</param>
    /// <param name="maxHp">当前角色最大血量</param>
    /// <param name="level">当前关卡</param>
    /// <param name="InitHp">角色初始血量</param>
    private void ATK(int levelMonster, float maxHp, int level, float InitHp)
    {
        bool victory = false;
        #region 角色攻击
        if (round == 0)
        {
            #region 血量恢复
            if (RoleFd.HPRegen > 0)
            {
                if (RoleFd.HP < maxHp)
                {
                    var RestoreHp = maxHp - RoleFd.HP;
                    if (RestoreHp < RoleFd.HPRegen)
                    {
                        RoleFd.HP = maxHp;
                        AtkDetail += $" 【{RoleFd.Name} 触发天赋技能，血量恢复{ RestoreHp}点血量。】\n\n ";
                        UpdateRoleHP(RoleFd.HP);
                    }
                    else
                    {
                        RoleFd.HP += RoleFd.HPRegen;
                        if (RoleFd.HPRegen > 0)
                        {
                            AtkDetail += $" 【{RoleFd.Name} 触发天赋技能，血量恢复{ RoleFd.HPRegen}点血量。】\n\n ";
                            UpdateRoleHP(RoleFd.HP);
                        }
                    }
                }
            }
            #endregion
            if (monster.Dodge == 0.00F || Random.Range(0.00F, 1.01F) > (monster.Dodge * 0.01))
            {
                if (Random.Range(0.00F, 1.01F) <= RoleFd.Seckill)
                {
                    AtkDetail += $"【{RoleFd.Name}发动技能，将{ monster.Name }秒杀。】\n\n";
                    monster.HP = 0;
                    UpdateCurrentMonsterHP(levelMonster, monster.HP, monster.Name);
                    victory = true;
                    round = 2;
                }
                int atk = System.Convert.ToInt32(RoleFd.Atk);
                if (Random.Range(0.00F, 1.01F) <= (RoleFd.Crit * 0.01))
                {
                    atk = System.Convert.ToInt32(RoleFd.Atk + (RoleFd.CritHarm * RoleFd.Atk / 100));
                    monster.HP -= atk;
                    AtkDetail += $"【{RoleFd.Name }发动攻击，（暴击）对{ monster.Name }造成了{ atk }点伤害。{(RoleFd.HP < maxHp && RoleFd.AtkRegain > 0 ? "" : "】\n")}\n";
                }
                else
                {
                    monster.HP -= atk;
                    AtkDetail += $"【{RoleFd.Name }发动攻击，对{ monster.Name }造成了{ atk }点伤害。{(RoleFd.HP < maxHp && RoleFd.AtkRegain > 0 ? "" : "】\n")}\n";
                }
                UpdateCurrentMonsterHP(levelMonster, monster.HP, monster.Name);
                if (RoleFd.HP < maxHp)//如果已经满血则不回血
                {
                    var currentHp = RoleFd.HP += RoleFd.AtkRegain;
                    if (currentHp > maxHp)
                        RoleFd.HP = maxHp;
                    else
                        RoleFd.HP = currentHp;
                    if (RoleFd.AtkRegain > 0)
                    {
                        AtkDetail += $" {RoleFd.Name} 触发天赋技能，攻击恢复{RoleFd.AtkRegain}点血量。{(monster.HP <= 0 ? "" : "】\n")}\n ";
                        UpdateRoleHP(RoleFd.HP);
                    }
                }
                if (monster.HP <= 0)
                {
                    AtkDetail += $"{monster.Name}死亡。】\n\n";
                    round = 2;
                    victory = true;
                }
                else
                {
                    round = 1;
                }
            }
            else
            {
                AtkDetail += $"【{RoleFd.Name}发动攻击，但被{monster.Name}闪避了。】\n\n";
                round = 1;
            }
            if (Random.Range(0.00F, 1.01F) <= RoleFd.Twice)
            {
                AtkDetail += $"【{RoleFd.Name}触发回合内多次攻击技能，再次发动攻击。】\n";
                round = 0;
            }
        }
        #endregion
        #region 野怪攻击
        else
        {
            var monsterMaxHp = Common.JsonToModel<field>(GetLeveData()[levelMonster.ToString()].ToString()).HP;
            #region 血量恢复
            if (monster.HPRegen > 0)
            {
                if (monster.HP < monsterMaxHp)
                {
                    var RestoreHp = monsterMaxHp - monster.HP;
                    if (RestoreHp < monster.HPRegen)
                    {
                        monster.HP = monsterMaxHp;
                        AtkDetail += $" 【{monster.Name} 触发天赋技能，血量恢复{ RestoreHp}点血量。】\n\n ";
                        UpdateCurrentMonsterHP(levelMonster, monster.HP, monster.Name);
                    }
                    else
                    {
                        monster.HP += monster.HPRegen;
                        if (monster.HPRegen > 0)
                        {
                            AtkDetail += $" 【{monster.Name} 触发天赋技能，血量恢复{ monster.HPRegen}点血量。】\n\n ";
                            UpdateCurrentMonsterHP(levelMonster, monster.HP, monster.Name);
                        }
                    }
                }
            }
            #endregion
            if (RoleFd.Dodge == 0.00F || Random.Range(0.00F, 1.01F) > (RoleFd.Dodge * 0.01))
            {

                if (Random.Range(0.00F, 1.01F) <= monster.Seckill)
                {
                    AtkDetail += $"【{monster.Name}发动技能，将{ RoleFd.Name}秒杀，游戏失败。】\n\n";
                    RoleFd.HP = 0;
                    UpdateRoleHP(0);
                    round = 2;
                }
                int atk = System.Convert.ToInt32(monster.Atk);
                if (Random.Range(0.00F, 1.01F) <= (monster.Crit * 0.01))
                {
                    atk = System.Convert.ToInt32(monster.Atk + (monster.CritHarm * monster.Atk / 100));
                    RoleFd.HP -= atk;
                    AtkDetail += $"【{monster.Name}发动攻击，（暴击）对{RoleFd.Name }造成了{ atk }点伤害。{(monster.HP < monsterMaxHp && monster.AtkRegain > 0 ? "" : "】\n")}\n";
                }
                else
                {
                    RoleFd.HP -= atk;
                    AtkDetail += $"【{monster.Name}发动攻击，对{RoleFd.Name }造成了{ atk }点伤害。{(monster.HP < monsterMaxHp && monster.AtkRegain > 0 ? "" : "】\n")}\n";
                }
                UpdateRoleHP(RoleFd.HP);
                if (monster.HP < monsterMaxHp)//如果已经满血则不回血
                {
                    var currentHp = monster.HP += monster.AtkRegain;
                    if (currentHp > monsterMaxHp)
                        monster.HP = monsterMaxHp;
                    else
                        monster.HP = currentHp;
                    if (monster.AtkRegain > 0)
                    {
                        AtkDetail += $"{monster.Name} 触发天赋技能，攻击恢复{monster.AtkRegain}点血量。】\n\n";
                        UpdateCurrentMonsterHP(levelMonster, monster.HP, monster.Name);
                    }
                }
                if (RoleFd.HP <= 0)
                {
                    AtkDetail += $"【{RoleFd.Name}被击杀，游戏失败】\n\n";
                    round = 2;
                }
                else
                {
                    round = 0;
                }
            }
            else
            {
                AtkDetail += $"【{monster.Name}发动攻击，但被{RoleFd.Name}闪避了。】\n\n";
                round = 0;
            }
            if (Random.Range(0.00F, 1.01F) <= monster.Twice)
            {
                AtkDetail += $"【{monster.Name}触发回合内多次攻击技能，再次发动攻击。】\n";
                round = 1;
            }
        }
        #endregion

        if (monster.HP > 0 && RoleFd.HP <= 0)
            AtkText = $"被{monster.Name}击杀了，游戏失败！";
        else if (monster.HP <= 0)
            AtkText += $"击败了{monster.Name}{(InitHp != RoleFd.HP ? $"，{(InitHp < RoleFd.HP ? "恢复" : "失去")}{System.Math.Abs(InitHp - RoleFd.HP)}点血量。" : "。")}";

        #region 经验
        if (victory)
        {
            float[] interval = { 0.5F, 1.5F };
            int Exp = System.Convert.ToInt32(3 * level * Random.Range(interval[0], interval[1]) * GameHelper.hard);
            //角色添加经验值
            RoleAddEXP(Exp);
            AtkText += $" 获得{Exp}点经验值。\n";
            AtkDetail += $"【击败{monster.Name},获得{Exp}点经验值。】\n\n";
        }
        #endregion
    }

    /// <summary>
    /// 单次攻击
    /// </summary>
    public void SingleATK()
    {
        #region 攻击
        totalTimer += Time.deltaTime;
        int levelMonster = System.Convert.ToInt32(txt_LevelMonster.text);
        var currentLevelDic = GetLeveData();//当前关卡数据
        float maxHp = GetCurrentRole().HP;
        int level = System.Convert.ToInt32(currentLevelDic["999"]);
        var InitHp = RoleFd.HP;
        //战斗详情添加到AtkDetail
        #region 战斗

        //攻击不能用循环要用状态机，1秒进行一轮攻击
        if (totalTimer >= 1 && round != 2)
        {
            if (int.TryParse(System.Convert.ToString(level / 30.00F), out int result))//boss
            {
                monster = Common.ConvertObject<field>(MonsterDic["99"]);
                //var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
                ATK(levelMonster, maxHp, level, InitHp);
            }
            else   //小怪或灵泉
            {
                monster = Common.ConvertObject<field>(MonsterDic[levelMonster.ToString()]);
                if (monster == null || monster?.HP == 0)//灵泉或秘境
                {
                    //AtkDetail += "\n遇到了灵泉或者秘境！";
                    //AtkText += "遇到了灵泉或者秘境！";
                    hasHippocrene = true;
                }
                else
                {
                    //var Atk = Level.Combat(RoleFd, monster, maxHp, out victory);
                    //1s一次攻击
                    ATK(levelMonster, maxHp, level, InitHp);
                }
            }
            totalTimer = 0;
            //野怪、角色数值刷新
            MonsterDic[levelMonster.ToString()] = monster;
            second = AtkText.Length;
        }

        #endregion

        if (round == 2)//战斗完成
        {
            InitHp = RoleFd.HP;
            PlayText(AtkText);
            //战斗完后
            if (RoleFd.HP <= 0)
            {
                RoleFd.HP = 0;
                //Init(RoleFd, level);
                //战斗结束。弹出窗口
                GameOver();
            }
            else if (txt_AtkState.text == "0" && levelMonster == 2)
            {
                Init(RoleFd, level + 1);
            }
            else if (txt_AtkState.text == "0")
            {
                //数值刷新
                DataRefresh(RoleFd, levelMonster);

            }
        }
        #endregion
    }

    /// <summary>
    /// 修改当前野怪血量
    /// </summary>
    /// <param name="levelMonster">野怪序号</param>
    /// <param name="currentHP">当前血量</param>
    /// <param name="MonsterName">野怪名称</param>
    private void UpdateCurrentMonsterHP(int levelMonster, float currentHP, string MonsterName)
    {
        txt_Monster = GameObject.Find($"img_Monster{levelMonster}/txt_Monster{levelMonster}").GetComponent<Text>();
        txt_Monster.text = $"{MonsterName}（{(currentHP < 0 ? 0 : currentHP)}/{Common.JsonToModel<field>(GetLeveData()[levelMonster.ToString()].ToString()).HP}）";
    }

    /// <summary>
    /// 修改角色血量
    /// </summary>
    private void UpdateRoleHP(float hp)
    {
        float maxHp = GetCurrentRole().HP;
        txt_HP.text = $"({hp}/{maxHp})";
    }

    /// <summary>
    /// 攻击按钮无效
    /// </summary>
    public void BtnLoseEfficacy()
    {
        btn_Atk.GetComponent<Button>().enabled = true;
        btn_Atk.GetComponent<Button>().interactable = false;
        btn_AutomaticAtk.GetComponent<Button>().enabled = true;
        btn_AutomaticAtk.GetComponent<Button>().interactable = false;
    }
    /// <summary>
    /// 攻击按钮有效
    /// </summary>
    public void BtnEffective()
    {
        btn_Atk.GetComponent<Button>().enabled = true;
        btn_Atk.GetComponent<Button>().interactable = true;
        btn_AutomaticAtk.GetComponent<Button>().enabled = true;
        btn_AutomaticAtk.GetComponent<Button>().interactable = true;
    }

    /// <summary>
    /// 实现文字渐显
    /// </summary>
    /// <param name="str">要渐显的字符串</param>
    /// <param name="hasHipp">是否是灵泉</param>
    public void PlayText(string str, bool hasHipp = false)
    {
        Debug.Log("进入方法PlayText");

        if (!string.IsNullOrWhiteSpace(str))
        {
            totalTimer += Time.deltaTime;
            if (second > 0)
            {
                if (totalTimer >= 0.1)
                {
                    second--;
                    temp += str[no].ToString();
                    ipt_Atk.text = temp;
                    // 累加时间归0，重新累加
                    totalTimer = 0;
                    no++;
                }
            }
            if (second == 0)
            {
                temp = "";
                coroutine = true;
                txt_AtkState.text = "0";
                hasPlayText = true;
                round = 0;
                HippocreneOptions = false;
                if (hasHipp)
                {
                    HippocreneOver = true;
                }
            }
        }
    }

    /// <summary>
    /// 显示灵泉面板
    /// </summary>
    private void ShowHippocrene()
    {
        HippocrenePanel.SetActive(true);
    }

    /// <summary>
    /// 数据刷新
    /// </summary>
    /// <param name="role">角色Model</param>
    /// <param name="imgNo">图片编码</param>
    private void DataRefresh(field role, int imgNo)
    {
        #region 野怪图片更新

        #region 图1销毁
        GameObject.Find("img_Monster" + imgNo).transform.localScale = Vector3.zero;
        //GameObject.Find($"img_Monster{imgNo}/txt_Monster{imgNo}").SetActive(false);
        #endregion

        #region 图2中间展示
        GameObject.Find($"img_Monster{imgNo + 1}/txt_Monster{imgNo + 1}").GetComponent<Text>().fontSize = 38;

        img_rect = GameObject.Find("img_Monster" + (imgNo + 1)).GetComponent<RectTransform>();
        img_rect.sizeDelta = new Vector2(391f, 403f);
        img_rect.anchoredPosition = new Vector2(0f, 201f);
        txt_rect = GameObject.Find($"img_Monster{imgNo + 1}/txt_Monster{imgNo + 1}").GetComponent<RectTransform>();
        txt_rect.sizeDelta = new Vector2(391f, 70f);
        txt_rect.anchoredPosition = new Vector2(0f, 239f);

        #endregion

        #endregion

        #region 数值刷新
        float maxHp = GetCurrentRole().HP;
        txt_HP.text = $"({role.HP}/{maxHp})";
        txt_HPReply.text = role.HPRegen.ToString();
        txt_Dodge.text = role.Dodge + "%";
        txt_ATK.text = role.Atk.ToString();
        txt_Crit.text = role.Crit + "%";
        txt_CritHarm.text = role.CritHarm + "%";
        txt_LevelMonster.text = (imgNo + 1).ToString();
        no = 0;
        AtkText = "";
        hasPlayText = false;
        #endregion

    }

    /// <summary>
    /// 设置滚动条
    /// </summary>
    /// <param name="arg0"></param>
    private void UpdateDetail(string arg0)
    {
        TalkWinBar.value = 0;
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    private void GameOver()
    {
        AgainPanel.SetActive(true);
        //AgainPanel.transform.position = new Vector3(300, 600, 0);

    }
    /// <summary>
    /// 显示游戏
    /// </summary>
    private void ShowGame()
    {
        AgainPanel.SetActive(false);
        //AgainPanel.transform.position = new Vector3(300, 600, 0);

    }

    /// <summary>
    /// 当前关卡数据保存到文本
    /// </summary>
    /// <param name="dic">野怪数据</param>
    /// <param name="level">当前关卡</param>
    private void LevelDataSave(Dictionary<string, object> monster, int level)
    {
        if (!monster.ContainsKey("999"))
        {
            monster.Add("999", level);//关卡
        }
        string json = Common.DicToJson(monster);
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
    public Dictionary<string, object> GetLeveData()
    {
        var path = Application.dataPath + "/Data/";
        Dictionary<string, object> dict = new Dictionary<string, object>();
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists(path + @"\" + "LevelData/Level.txt"))
        {
            StreamReader json = File.OpenText(path + @"\" + "LevelData/Level.txt");
            //Debug.Log("读档" + json);
            string input = json.ReadToEnd();
            JsonReader reader = new JsonReader(input);
            string temp = string.Empty;
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    switch (reader.Token)
                    {
                        case JsonToken.PropertyName:
                            dict.Add(reader.Value.ToString(), string.Empty);
                            temp = reader.Value.ToString();
                            break;
                        default:
                            dict[temp] = reader.Value;
                            break;
                    }
                }
            }
            json.Close();
        }
        return dict;
    }

    /// <summary>
    /// 角色添加经验值
    /// </summary>
    private void RoleAddEXP(int exp)
    {
        var role = Common.ConvertModel<field>(GameHelper.DataRead("Role/Role.txt"));
        role.EXP += exp;
        string json = Common.ObjectToJson(role);
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

    /// <summary>
    /// 保存当前角色信息
    /// </summary>
    private void SaveCurrentRole(field role)
    {
        string json = Common.ObjectToJson(role);
        //json = GameHelper.DesEncrypt(json);//前期不加密
        var path = Application.dataPath + "/Data/Role";
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists($"{path}/CurrentRole.txt"))
            File.Delete($"{path}/CurrentRole.txt");
        File.WriteAllText($"{path}/CurrentRole.txt", json);
    }

    private field GetCurrentRole()
    {
        var path = Application.dataPath + "/Data/";
        field role = new field();
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists(path + @"\" + "Role/CurrentRole.txt"))
        {
            StreamReader json = File.OpenText(path + @"\" + "Role/CurrentRole.txt");
            //Debug.Log("读档" + json);
            string input = json.ReadToEnd();
            role = Common.JsonToModel<field>(input);
            json.Close();
        }
        return role;
    }

    #endregion
}
