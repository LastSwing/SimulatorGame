using Assets.Scripts.LogicalScripts.Models;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AiDieScene : MonoBehaviour
{
    Button btn_Return, btn_GameOver, btn_CardPoolsReturn;
    GameObject Award_Obj, ResetAward_Obj, CardPools_Obj, Setting_Obj, SettingCanvas, CardPoolsCanvas, AllSkillCanvas;
    Text txt_Silver, txt_AwardSilver, txt_ResetSilver, txt_CardPoolsCount;
    CurrentRoleModel PlayerRole;
    List<CurrentCardPoolModel> CurrentCardPools = new List<CurrentCardPoolModel>();
    List<CurrentCardPoolModel> list = new List<CurrentCardPoolModel>();//所有奖励卡
    int AwardCount = 3;
    void Start()
    {
        #region 控件初始化

        txt_Silver = transform.Find("TopBar/img_Silver/txt_Silver").GetComponent<Text>();
        txt_AwardSilver = transform.Find("AwardSilver/Text").GetComponent<Text>();
        txt_ResetSilver = transform.Find("ResetAward/Image/Text").GetComponent<Text>();
        txt_CardPoolsCount = transform.Find("CardPools_Obj/Image/Text").GetComponent<Text>();

        Award_Obj = transform.Find("Award").gameObject;
        ResetAward_Obj = transform.Find("ResetAward").gameObject;
        CardPools_Obj = transform.Find("CardPools_Obj").gameObject;
        SettingCanvas = GameObject.Find("SettingCanvas");
        CardPoolsCanvas = GameObject.Find("CardPoolsCanvas");
        AllSkillCanvas = GameObject.Find("AllSkillCanvas");
        AllSkillCanvas.SetActive(false);
        #endregion
        #region 给画布添加隐藏技能详细事件

        EventTrigger trigger = transform.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = transform.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener(delegate { HideCardDetail(); });
        trigger.triggers.Add(entry);
        #endregion
        #region 刷新卡牌按钮事件
        EventTrigger trigger1 = ResetAward_Obj.GetComponent<EventTrigger>();
        if (trigger1 == null)
        {
            trigger1 = ResetAward_Obj.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.callback.AddListener(delegate { ResetAward(); });
        trigger1.triggers.Add(entry1);
        #endregion
        #region 卡池单击事件
        btn_CardPoolsReturn = GameObject.Find("CardPoolsCanvas/Button").GetComponent<Button>();
        btn_CardPoolsReturn.onClick.AddListener(ReturnScene);
        CardPoolsCanvas.SetActive(false);
        EventTrigger trigger2 = CardPools_Obj.GetComponent<EventTrigger>();
        if (trigger2 == null)
        {
            trigger2 = CardPools_Obj.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.callback.AddListener(delegate { ShowCardPools(); });
        trigger2.triggers.Add(entry2);
        #endregion

        #region 绑定点击设置

        Setting_Obj = transform.Find("TopBar/Setting").gameObject;
        btn_Return = GameObject.Find("SettingCanvas/Content/btn_Return").GetComponent<Button>();
        btn_Return.onClick.AddListener(ReturnScene);
        //btn_GameOver = GameObject.Find("SettingCanvas/Content/btn_GameOver").GetComponent<Button>();
        //btn_GameOver.transform.localScale = Vector3.zero;
        SettingCanvas.SetActive(false);
        EventTrigger trigger3 = Setting_Obj.GetComponent<EventTrigger>();
        if (trigger3 == null)
        {
            trigger3 = Setting_Obj.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry3 = new EventTrigger.Entry();
        entry3.callback.AddListener(delegate { ClickSetting(); });
        trigger3.triggers.Add(entry3);
        #endregion
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Init()
    {
        PlayerRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName);

        list = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName)?.FindAll(a => a.CardLevel == 1 && a.PlayerOrAI == 0).ListRandom();

        CurrentCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentCardPoolsFileName);

        #region 玩家卡池
        if (CurrentCardPools?.Count > 0)
        {
            txt_CardPoolsCount.text = CurrentCardPools.Count.ToString();
        }
        else
        {
            txt_CardPoolsCount.text = "0";
        }
        #endregion

        for (int i = 0; i < AwardCount; i++)
        {
            CreateAwardCrad(list[i], i);
        }

        var RAwardSilver = Random.Range(15, 25);

        txt_AwardSilver.text = $"+ {RAwardSilver} 银币";

        PlayerRole.Wealth += RAwardSilver;
        txt_Silver.text = PlayerRole.Wealth.ToString();
        Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
        if (PlayerRole.Wealth < 25)
        {
            txt_ResetSilver.color = Color.red;
        }
    }


    /// <summary>
    /// 创建奖励卡
    /// </summary>
    /// <param name="model"></param>
    /// <param name="i"></param>
    private void CreateAwardCrad(CurrentCardPoolModel model, int i)
    {
        GameObject tempObject = Resources.Load("Prefab/img_Card") as GameObject;
        tempObject = Common.AddChild(Award_Obj.transform, tempObject);
        tempObject.name = "img_AwardCard" + i;
        EventTrigger trigger = tempObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = tempObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener(delegate { ShowDetail(model, i); });
        trigger.triggers.Add(entry);

        #region 卡牌数据绑定
        var cardType = model.StateType;
        #region 攻击力图标
        var Card_ATK_img = tempObject.transform.Find("img_ATK").GetComponent<Image>();
        var Card_ATK_icon = tempObject.transform.Find("img_ATK/Image").GetComponent<Image>();
        var Card_ATKNumber = tempObject.transform.Find("img_ATK/Text").GetComponent<Text>();
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
        var Card_energy_img = tempObject.transform.Find("img_Energy").GetComponent<Image>();
        var Card_Skill_img = tempObject.transform.Find("img_Skill").GetComponent<Image>();
        var Card_Energy = tempObject.transform.Find("img_Energy/Text").GetComponent<Text>();
        var Card_Title = tempObject.transform.Find("img_Title/Text").GetComponent<Text>();
        if (model.Consume == 0)
        {
            Card_energy_img.transform.localScale = Vector3.zero;
        }
        Common.ImageBind(model.CardUrl, Card_Skill_img);
        Card_Energy.text = model.Consume.ToString();
        Card_Title.text = model.CardName.TextSpacing();
        #endregion
    }

    /// <summary>
    /// 显示详情
    /// </summary>
    /// <param name="model"></param>
    /// <param name="i"></param>
    public void ShowDetail(CurrentCardPoolModel model, int i)
    {
        HideCardDetail();
        var Card_Detail = GameObject.Find($"Award/img_AwardCard{i}/img_Detail")?.GetComponent<Image>();
        if (Card_Detail != null)
        {
            Card_Detail.transform.localScale = Vector3.one;
            var btn = GameObject.Find($"Award/img_AwardCard{i}/btn_Confirm")?.GetComponent<Button>();
            btn.transform.localScale = Vector3.one;
        }
        else
        {
            var Card_img = GameObject.Find($"Award/img_AwardCard{i}").GetComponent<Image>();
            GameObject tempImg = Resources.Load("Prefab/img_Detail") as GameObject;
            tempImg = Common.AddChild(Card_img.transform, tempImg);
            tempImg.name = "img_Detail";
            tempImg.transform.localPosition = new Vector2(0, 160);

            GameObject temp = tempImg.transform.Find("Text").gameObject;
            temp.GetComponent<Text>().text = $"{model.CardName}\n{model.CardDetail}";

            GameObject btn_Confirm = Resources.Load("Prefab/btn_Confirm") as GameObject;
            btn_Confirm = Common.AddChild(Card_img.transform, btn_Confirm);
            btn_Confirm.name = "btn_Confirm";
            btn_Confirm.transform.localPosition = new Vector2(0, -110);
            EventTrigger trigger = btn_Confirm.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = btn_Confirm.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.callback.AddListener(delegate { Confirm(model); });
            trigger.triggers.Add(entry);
        }
    }

    /// <summary>
    /// 隐藏详情
    /// </summary>
    public void HideCardDetail()
    {
        for (int i = 0; i < AwardCount; i++)
        {
            var Card_img = GameObject.Find($"Award/img_AwardCard{i}/img_Detail")?.GetComponent<Image>();
            if (Card_img != null)
            {
                Card_img.transform.localScale = Vector3.zero;
            }
            var btn = GameObject.Find($"Award/img_AwardCard{i}/btn_Confirm")?.GetComponent<Button>();
            if (btn != null)
            {
                btn.transform.localScale = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// 点击确认
    /// </summary>
    /// <param name="model"></param>
    public void Confirm(CurrentCardPoolModel model)
    {
        Debug.Log(model.CardName);
        PlayerRole.CardListStr += $"{model.ID}|{model.CardName};";
        CurrentCardPools.Add(model);
        txt_CardPoolsCount.text = CurrentCardPools.Count.ToString();
        Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);

        var cardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentCardPoolsFileName);
        cardList.Add(model);
        Common.SaveTxtFile(cardList.ListToJson(), GlobalAttr.CurrentCardPoolsFileName);

        //关闭当前页
        Common.SceneJump("MapScene", 1);
    }

    /// <summary>
    /// 重置奖励
    /// </summary>
    public void ResetAward()
    {
        if (PlayerRole.Wealth >= 25)
        {
            #region 奖励重置
            //删除原有的奖励
            GameObject parentObject = GameObject.Find("Award");
            var parantCount = parentObject.transform.childCount;
            for (int y = 0; y < parantCount; y++)
            {
                DestroyImmediate(parentObject.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
            for (int i = 0; i < AwardCount; i++)
            {
                list.RemoveAt(0);
            }
            if (list != null && list.Count > AwardCount)
            {
                for (int i = 0; i < AwardCount; i++)
                {
                    CreateAwardCrad(list[i], i);
                }
            }
            else
            {
                list = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName)?.FindAll(a => a.CardLevel == 0).ListRandom();
                for (int i = 0; i < AwardCount; i++)
                {
                    CreateAwardCrad(list[i], i);
                }
            }
            #endregion
            //扣减金币
            PlayerRole.Wealth -= 25;
            txt_Silver.text = PlayerRole.Wealth.ToString();
            Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
            if (PlayerRole.Wealth < 25)
            {
                txt_ResetSilver.color = Color.red;
            }
        }
    }

    public void ShowCardPools()
    {
        transform.gameObject.SetActive(false);
        SettingCanvas.SetActive(false);
        CardPoolsCanvas.SetActive(true);
    }

    public void ClickSetting()
    {
        transform.gameObject.SetActive(false);
        SettingCanvas.SetActive(true);
        CardPoolsCanvas.SetActive(false);
    }
    public void ReturnScene()
    {
        transform.gameObject.SetActive(true);
        SettingCanvas.SetActive(false);
        CardPoolsCanvas.SetActive(false);
    }
}
