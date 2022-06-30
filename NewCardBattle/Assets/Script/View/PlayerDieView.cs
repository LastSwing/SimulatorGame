using Assets.Script.Models;
using Assets.Script.Models.Map;
using Assets.Script.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerDieView : BaseUI
{
    Button btn_Continue, btn_Setting, btn_Again;
    Image img_Background;//背景图片
    GameObject Award_Obj;
    Text txt_AwardGold, txt_ReturnView, txt_ReturnView1;
    GlobalPlayerModel globalPlayerModel;
    GameObject UI_Obj;
    float totalTime;
    int AccumulateCount, InitValue, MaxValue = 0;
    public override void OnInit()
    {
        //因为获取组件以及绑定事件一般只需要做一次，所以放在OnInit
        InitComponent();
        InitUIevent();
    }

    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitComponent()
    {
        img_Background = transform.Find("BG").GetComponent<Image>();
        btn_Continue = transform.Find("UI/btn_Continue").GetComponent<Button>();
        btn_Again = transform.Find("UI/btn_Again").GetComponent<Button>();
        btn_Setting = transform.Find("UI/TopBar/Setting").GetComponent<Button>();
        txt_AwardGold = transform.Find("UI/Award/GoldBox/txt_Value").GetComponent<Text>();
        Award_Obj = transform.Find("UI/Award").gameObject;
        txt_ReturnView = GameObject.Find("MainCanvas/txt_ReturnView").GetComponent<Text>();
        txt_ReturnView1 = GameObject.Find("MainCanvas/txt_ReturnView1").GetComponent<Text>();
        UI_Obj = transform.Find("UI").gameObject;
    }

    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {
        #region 给画布添加隐藏技能详细事件

        EventTrigger trigger3 = transform.GetComponent<EventTrigger>();
        if (trigger3 == null)
        {
            trigger3 = transform.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry3 = new EventTrigger.Entry();
        entry3.callback.AddListener(delegate { HideCardDetail(); });
        trigger3.triggers.Add(entry3);
        #endregion
        btn_Continue.onClick.AddListener(ContinueClick);
        btn_Again.onClick.AddListener(AginClick);
        btn_Setting.onClick.AddListener(SettingClick);
    }

    #region 按钮事件
    public void ContinueClick()
    {
        Common.GameOverDataReset();
        UIManager.instance.OpenView("MapView");
        UIManager.instance.CloseView("PlayerDieView");
    }
    public void AginClick()
    {
        Common.GameOverDataReset();
        UIManager.instance.OpenView("MainView");
        UIManager.instance.CloseView("PlayerDieView");
    }
    public void SettingClick()
    {
        txt_ReturnView.text = "PlayerDieView";
        txt_ReturnView1.text = "PlayerDieView";
        UIManager.instance.OpenView("SettingView");
        UIManager.instance.CloseView("PlayerDieView");
    }
    #endregion

    public override void OnOpen()
    {
        //数据需要每次打开都要刷新，UI状态也是要每次打开都进行刷新，因此放在OnOpen
        InitUIData();
        InitUIState();
        InitSetting();
    }

    /// <summary>
    /// 初始化其余设置
    /// </summary>
    private void InitSetting()
    {
        //SoundManager.instance.PlayOnlyOneSound("BGM_1", (int)TrackType.BGM, true);
    }

    /// <summary>
    /// 更新UI状态
    /// </summary>
    private void InitUIState()
    {

    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void InitUIData()
    {
        #region 奖励
        globalPlayerModel = Common.GetTxtFileToModel<GlobalPlayerModel>(GlobalAttr.GlobalRoleFileName);
        var mapLo = Common.GetTxtFileToModel<CurrentMapLocation>(GlobalAttr.CurrentMapLocationFileName, "Map");
        if (mapLo != null)
        {
            int goldValue = mapLo.Row * Random.Range(6, 13);
            if (mapLo.HasKillBoss == 1)
            {
                goldValue = 130;
            }
            txt_AwardGold.text = $"+ {goldValue}";
            globalPlayerModel.Wealth += goldValue;
            Common.SaveTxtFile(globalPlayerModel.ObjectToJson(), GlobalAttr.GlobalRoleFileName);
        }
        else
        {
            txt_AwardGold.text = $"+ 0";
        }
        //卡牌奖励
        if (globalPlayerModel.AccumulateValue > globalPlayerModel.MaxAccumulate)
        {
            InitValue = globalPlayerModel.AccumulateValue;
            MaxValue = globalPlayerModel.MaxAccumulate;
            Calculate();
            for (int x = 0; x < AccumulateCount; x++)
            {
                GameObject img_Accumulate = ResourcesManager.instance.Load("img_Accumulate") as GameObject;
                img_Accumulate = Common.AddChild(Award_Obj.transform, img_Accumulate);
                img_Accumulate.name = "img_Accumulate" + x;
                #region 点击事件
                EventTrigger trigger2 = img_Accumulate.GetComponent<EventTrigger>();
                if (trigger2 == null)
                {
                    trigger2 = img_Accumulate.AddComponent<EventTrigger>();
                }
                EventTrigger.Entry entry2 = new EventTrigger.Entry();
                entry2.callback.AddListener(delegate { ShowSkill(img_Accumulate); });
                trigger2.triggers.Add(entry2);
            }
            globalPlayerModel.AccumulateValue = InitValue;
            globalPlayerModel.MaxAccumulate = MaxValue;
            Common.SaveTxtFile(globalPlayerModel.ObjectToJson(), GlobalAttr.GlobalRoleFileName);
            #endregion
        }
        #endregion

        #region 加载TopBar预制件

        //加载TopBar预制件
        var tempBar = transform.Find("UI/TopBar")?.gameObject;
        if (tempBar == null)
        {
            GameObject topBar = ResourcesManager.instance.Load("TopBar") as GameObject;
            topBar = Common.AddChild(UI_Obj.transform, topBar);
            topBar.name = "TopBar";
        }
        #endregion
    }

    #region 卡片奖励
    //计算奖励几张卡牌
    public void Calculate()
    {
        if (InitValue >= MaxValue)
        {
            InitValue -= MaxValue;
            MaxValue += 20;
            AccumulateCount++;
            Calculate();
        }
    }

    public void ShowSkill(GameObject obj)
    {
        DestroyImmediate(obj.transform.GetChild(0).gameObject);//删除子类
        int i = System.Convert.ToInt32(obj.name.Substring(14));
        var GlobalCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalCardPoolFileName);
        var GlobalPlayerCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName);
        //找出玩家没拥有的卡池
        var surplusList = GlobalCardPools.Where(a => !GlobalPlayerCardPools.Exists(t => a.ID.Equals(t.ID))).ToList().ListRandom();
        if (surplusList?.Count > 0)
        {
            var model = surplusList[0];
            GameObject img_Card = ResourcesManager.instance.Load("img_Card240") as GameObject;
            img_Card = Common.AddChild(obj.transform, img_Card);
            img_Card.name = "img_AwardCard" + i;
            img_Card.transform.localPosition = new Vector3(0, 0);
            #region 点击事件
            EventTrigger trigger2 = img_Card.GetComponent<EventTrigger>();
            if (trigger2 == null)
            {
                trigger2 = img_Card.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.callback.AddListener(delegate { SkillClick(model, i); });
            trigger2.triggers.Add(entry2);
            #endregion

            #region 卡牌数据绑定
            var cardType = model.StateType;
            #region 攻击力图标
            var Card_ATK_img = img_Card.transform.Find("img_ATK").GetComponent<Image>();
            var Card_ATK_icon = img_Card.transform.Find("img_ATK/Image").GetComponent<Image>();
            var Card_ATKNumber = img_Card.transform.Find("img_ATK/Text").GetComponent<Text>();
            if (cardType == 6 || cardType == 7 || cardType == 8 || cardType == 9)//是否隐藏
            {
                Card_ATK_img.transform.localScale = Vector3.zero;
            }
            else
            {
                if (cardType == 1)
                {
                    Common.ImageBind("Images/Defense", Card_ATK_icon);
                }
                else if (cardType == 2 || cardType == 3)
                {
                    Common.ImageBind("Images/HP_Icon", Card_ATK_icon);
                }
                else if (cardType == 5)
                {
                    Common.ImageBind("Images/CardIcon/ShuiJin", Card_ATK_icon);
                }
                else
                {
                    Common.ImageBind("Images/Atk_Icon", Card_ATK_icon);
                }
                Card_ATKNumber.text = model.Effect.ToString();
            }
            #endregion
            var Card_energy_img = img_Card.transform.Find("img_Energy").GetComponent<Image>();
            var Card_Skill_img = img_Card.transform.Find("img_Skill").GetComponent<Image>();
            var Card_Energy = img_Card.transform.Find("img_Energy/Text").GetComponent<Text>();
            var Card_Title = img_Card.transform.Find("img_Title/Text").GetComponent<Text>();
            if (model.Consume == 0)
            {
                Card_energy_img.transform.localScale = Vector3.zero;
            }
            Common.ImageBind(model.CardUrl, Card_Skill_img);
            Card_Energy.text = model.Consume.ToString();
            Card_Title.text = model.CardName.TextSpacing();
            #endregion

            GlobalPlayerCardPools.Add(model);
            Common.SaveTxtFile(GlobalPlayerCardPools.ListToJson(), GlobalAttr.GlobalPlayerCardPoolFileName);
        }
    }

    public void SkillClick(CurrentCardPoolModel model, int i)
    {
        HideCardDetail();
        var Card_Detail = GameObject.Find($"UI/Award/img_Accumulate{i}/img_AwardCard{i}/img_Detail")?.GetComponent<Image>();
        if (Card_Detail != null)
        {
            Card_Detail.transform.localScale = Vector3.one;
        }
        else
        {
            var Card_img = GameObject.Find($"UI/Award/img_Accumulate{i}/img_AwardCard{i}").GetComponent<Image>();
            GameObject tempImg = ResourcesManager.instance.Load("img_CardDetail") as GameObject;
            tempImg = Common.AddChild(Card_img.transform, tempImg);
            tempImg.name = "img_Detail";
            tempImg.transform.localPosition = new Vector2(0, 160);

            GameObject temp = tempImg.transform.Find("Text").gameObject;
            temp.GetComponent<Text>().text = $"{model.CardName}\n{model.CardDetail}";

        }
    }

    /// <summary>
    /// 隐藏详情
    /// </summary>
    public void HideCardDetail()
    {
        for (int i = 0; i < AccumulateCount; i++)
        {
            var Card_img = GameObject.Find($"UI/Award/img_Accumulate{i}/img_AwardCard{i}/img_Detail")?.GetComponent<Image>();
            if (Card_img != null)
            {
                Card_img.transform.localScale = Vector3.zero;
            }
        }
    }
    #endregion

    public override void OnClose()
    {
        //throw new System.NotImplementedException();
    }
}
