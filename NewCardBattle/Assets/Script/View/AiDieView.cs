using Assets.Script.Models;
using Assets.Script.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AiDieView : BaseUI
{
    Image img_Background;//背景图片
    GameObject obj_Award, obj_AwardSilver;
    Button btn_ResetAward, btn_CardPools, btn_Setting;
    Text txt_AwardSilver, txt_Silver, txt_CardPoolsCount, txt_ResetSilver;
    Text txt_ReturnView, txt_CardType, txt_SettingHasBtn, txt_ReturnView1, txt_HasClickSetting;

    CurrentRoleModel PlayerRole;
    List<CurrentCardPoolModel> CurrentCardPools = new List<CurrentCardPoolModel>();
    List<CurrentCardPoolModel> list = new List<CurrentCardPoolModel>();//所有奖励卡
    int AwardCount = 3;
    #region OnInit
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
        obj_Award = transform.Find("UI/Award").gameObject;
        obj_AwardSilver = transform.Find("UI/AwardSilver").gameObject;
        btn_ResetAward = transform.Find("UI/ResetAward").GetComponent<Button>();
        btn_CardPools = transform.Find("UI/CardPools_Obj").GetComponent<Button>();
        btn_Setting = transform.Find("UI/TopBar/Setting").GetComponent<Button>();
        txt_AwardSilver = transform.Find("UI/AwardSilver/Text").GetComponent<Text>();
        txt_CardPoolsCount = transform.Find("UI/CardPools_Obj/Image/Text").GetComponent<Text>();
        txt_ResetSilver = transform.Find("UI/ResetAward/Image/Text").GetComponent<Text>();
        txt_Silver = transform.Find("UI/TopBar/img_Silver/txt_Silver").GetComponent<Text>();

        txt_ReturnView = GameObject.Find("MainCanvas/txt_ReturnView").GetComponent<Text>();
        txt_ReturnView1 = GameObject.Find("MainCanvas/txt_ReturnView1").GetComponent<Text>();
        txt_CardType = GameObject.Find("MainCanvas/txt_CardType").GetComponent<Text>();
        txt_SettingHasBtn = GameObject.Find("MainCanvas/txt_SettingHasBtn").GetComponent<Text>();
        txt_HasClickSetting = GameObject.Find("MainCanvas/txt_HasClickSetting").GetComponent<Text>();
    }

    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {

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
        btn_ResetAward.onClick.AddListener(ResetAward);
        btn_CardPools.onClick.AddListener(CardPoolsClick);
        btn_Setting.onClick.AddListener(SettingClick);
    }

    #region 点击事件


    /// <summary>
    /// 隐藏详情
    /// </summary>
    public void HideCardDetail()
    {
        for (int i = 0; i < AwardCount; i++)
        {
            var Card_img = transform.Find($"UI/Award/img_AwardCard{i}/img_Detail")?.GetComponent<Image>();
            if (Card_img != null)
            {
                Card_img.transform.localScale = Vector3.zero;
            }
            var btn = transform.Find($"UI/Award/img_AwardCard{i}/btn_Confirm")?.GetComponent<Button>();
            if (btn != null)
            {
                btn.transform.localScale = Vector3.zero;
            }
        }
        var goldBtn = transform.Find("UI/Award/img_AwardGold/btn_Adventure")?.GetComponent<Button>();
        if (goldBtn != null)
        {
            goldBtn.transform.localScale = Vector3.zero;
        }
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
            var parantCount = obj_Award.transform.childCount;
            for (int y = 0; y < parantCount; y++)
            {
                DestroyImmediate(obj_Award.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
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

    public void CardPoolsClick()
    {
        txt_ReturnView.text = "AiDieView";
        txt_CardType.text = "0";
        txt_HasClickSetting.text = "1";
        UIManager.instance.OpenView("CardPoolsView");
        UIManager.instance.CloseView("AiDieView");
    }

    public void SettingClick()
    {
        txt_ReturnView.text = "AiDieView";
        txt_ReturnView1.text = "AiDieView";
        txt_SettingHasBtn.text = "1";
        txt_HasClickSetting.text = "1";
        UIManager.instance.OpenView("SettingView");
        UIManager.instance.CloseView("AiDieView");
    }

    #endregion
    #endregion

    #region OnOpen
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
        if (txt_HasClickSetting.text == "0")
        {
            #region 数据初始化
            PlayerRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName);

            list = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName)?.FindAll(a => a.PlayerOrAI == 0 && a.CardLevel > 0).ListRandom();//按关卡展示卡牌 

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

            if (string.IsNullOrEmpty(PlayerRole.AdventureIds))
            {
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
            else //固定的奖励
            {
                btn_ResetAward.transform.localScale = Vector3.zero;
                obj_AwardSilver.SetActive(false);
                var model = Common.GetTxtFileToList<AdventureModel>(GlobalAttr.GlobalAdventureFileName, "Adventure").Find(a => a.ID == PlayerRole.AdventureIds);
                if (model.RewardType == 4)//金币奖励
                {
                    GameObject tempObject = ResourcesManager.instance.Load("img_AdventureBtn") as GameObject;
                    tempObject = Common.AddChild(obj_Award.transform, tempObject);
                    tempObject.name = "img_AwardGold";
                    var temp_img = tempObject.GetComponent<Image>();
                    var temp_title = tempObject.transform.Find("txt_Title").GetComponent<Text>();
                    var temp_Effect = tempObject.transform.Find("txt_Effect").GetComponent<Text>();
                    Common.ImageBind("Images/GoldReward1", temp_img);
                    temp_title.transform.localScale = Vector3.zero;
                    temp_Effect.text = $"+{model.EventEffectValue} 银币";
                    EventTrigger trigger = tempObject.GetComponent<EventTrigger>();
                    if (trigger == null)
                    {
                        trigger = tempObject.AddComponent<EventTrigger>();
                    }
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.callback.AddListener(delegate { ShowClick(model, tempObject); });
                    trigger.triggers.Add(entry);
                }
                else //卡牌奖励
                {
                    var rewardList = new List<CurrentCardPoolModel>();
                    if (model.RewardType == 8)
                    {
                        //rewardList = list.FindAll(a => 1 == 1);//加上查询条件
                        rewardList = list;
                        rewardList.ListRandom();
                        for (int i = 0; i < model.EventEffectValue; i++)
                        {
                            rewardList.Add(rewardList[i]);
                        }
                    }
                    for (int i = 0; i < rewardList.Count; i++)
                    {
                        CreateAwardCrad(rewardList[i], i);
                    }
                }
                PlayerRole.AdventureIds = "";
                Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
            }
            #endregion 
        }
        else
        {
            txt_HasClickSetting.text = "0";
        }
    }
    #endregion

    #region 奖励初始化

    /// <summary>
    /// 显示点击按钮
    /// </summary>
    /// <param name="model"></param>
    /// <param name="thisObj"></param>
    private void ShowClick(AdventureModel model, GameObject thisObj)
    {
        var btn = thisObj.transform.Find("btn_Adventure")?.GetComponent<Button>();
        if (btn == null)
        {
            GameObject tempObject = ResourcesManager.instance.Load("btn_Adventure") as GameObject;
            tempObject = Common.AddChild(thisObj.transform, tempObject);
            tempObject.name = "btn_Adventure";
            var temp = tempObject.GetComponent<Button>();
            var tempName = temp.transform.Find("Text").GetComponent<Text>();
            tempName.text = "确  定";
            temp.onClick.AddListener(delegate { GoldRewardConfirm(model); });
        }
        else
        {
            btn.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 金币奖励确定
    /// </summary>
    /// <param name="model"></param>
    private void GoldRewardConfirm(AdventureModel model)
    {
        PlayerRole.Wealth += model.EventEffectValue;
        Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);

        UIManager.instance.OpenView("MapView");
        UIManager.instance.CloseView("AiDieView");
    }


    /// <summary>
    /// 创建奖励卡
    /// </summary>
    /// <param name="model"></param>
    /// <param name="i"></param>
    private void CreateAwardCrad(CurrentCardPoolModel model, int i)
    {
        GameObject tempObject = ResourcesManager.instance.Load("img_Card240") as GameObject;
        tempObject = Common.AddChild(obj_Award.transform, tempObject);
        tempObject.name = "img_AwardCard" + i;
        EventTrigger trigger = tempObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = tempObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener(delegate { ShowDetail(model, i); });
        trigger.triggers.Add(entry);

        Common.CardDataBind(tempObject, model);
    }

    /// <summary>
    /// 显示详情
    /// </summary>
    /// <param name="model"></param>
    /// <param name="i"></param>
    public void ShowDetail(CurrentCardPoolModel model, int i)
    {
        HideCardDetail();
        var Card_Detail = transform.Find($"UI/Award/img_AwardCard{i}/img_Detail")?.GetComponent<Image>();
        if (Card_Detail != null)
        {
            Card_Detail.transform.localScale = Vector3.one;
            var btn = transform.Find($"UI/Award/img_AwardCard{i}/btn_Confirm")?.GetComponent<Button>();
            btn.transform.localScale = Vector3.one;
        }
        else
        {
            var Card_img = transform.Find($"UI/Award/img_AwardCard{i}").GetComponent<Image>();
            GameObject tempImg = ResourcesManager.instance.Load("img_CardDetail") as GameObject;
            tempImg = Common.AddChild(Card_img.transform, tempImg);
            tempImg.name = "img_Detail";
            tempImg.transform.localPosition = new Vector2(0, 205);

            Common.CardDetailDataBind(tempImg, model);

            GameObject btn_Confirm = ResourcesManager.instance.Load("btn_Confirm") as GameObject;
            btn_Confirm = Common.AddChild(Card_img.transform, btn_Confirm);
            btn_Confirm.name = "btn_Confirm";
            btn_Confirm.transform.localPosition = new Vector2(0, -150);
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
        UIManager.instance.OpenView("MapView");
        UIManager.instance.CloseView("AiDieView");
    }
    #endregion

    public override void OnClose()
    {
        //throw new System.NotImplementedException();
    }
}
