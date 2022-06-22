using Assets.Scripts.LogicalScripts.Models;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdventureScene : MonoBehaviour
{
    Image img_MainBG, img_HP;
    Text txt_EventDetail, txt_HP, txt_Wealth;
    GameObject obj_EventBtn, obj_CardsReward;
    List<AdventureModel> AllAdt = new List<AdventureModel>();//所有的冒险数据
    List<AdventureModel> CurrentAdt = new List<AdventureModel>();//当前冒险数据
    List<AdventureModel> CurrentLayer = new List<AdventureModel>();//当前事件层级
    List<string> AllAdtName = new List<string>();//所有冒险名称
    CurrentRoleModel PlayerRole;
    List<CurrentCardPoolModel> GlobalCardList = new List<CurrentCardPoolModel>();
    void Start()
    {
        img_MainBG = transform.Find("img_MainBG").GetComponent<Image>();
        txt_EventDetail = transform.Find("txt_EventDetail").GetComponent<Text>();
        obj_EventBtn = transform.Find("obj_EventBtn").gameObject;
        obj_CardsReward = transform.Find("obj_CardsReward").gameObject;
        img_HP = transform.Find("TopBar/HP/img_HP").GetComponent<Image>();
        txt_HP = transform.Find("TopBar/HP/Text").GetComponent<Text>();
        txt_Wealth = transform.Find("TopBar/img_Silver/txt_Silver").GetComponent<Text>();

        #region 数据初始化
        GlobalCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalCardPoolFileName);
        PlayerRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName);
        AllAdt = Common.GetTxtFileToList<AdventureModel>(GlobalAttr.GlobalAdventureFileName, "Adventure");
        if (AllAdt != null)
        {
            var groupAdt = AllAdt.GroupBy(a => a.Name).ToList();
            foreach (var item in groupAdt)
            {
                AllAdtName.Add(item.Key);
            }
            AllAdtName.ListRandom();//让冒险事件随机
            CurrentAdt = groupAdt.Find(a => a.Key == AllAdtName[0]).ToList();
        }

        if (CurrentAdt != null && CurrentAdt?.Count > 0)
        {
            var groupCAdt = CurrentAdt.OrderBy(a => a.EventLayerLevelSort)
                .GroupBy(a => a.EventLayerLevel).ToList();
            CurrentLayer = groupCAdt[0].ToList();//第一层级
        }
        #endregion
        DataBind();

        #region 给画布添加隐藏事件下按钮

        EventTrigger trigger = transform.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = transform.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener(delegate { HideEventUnderBtn(); });
        trigger.triggers.Add(entry);
        #endregion
    }

    void DataBind()
    {
        int count = obj_EventBtn.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            DestroyImmediate(obj_EventBtn.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
        }
        if (CurrentLayer != null)
        {
            Common.ImageBind(CurrentLayer[0].MainBGIUrl, img_MainBG);
            txt_EventDetail.text = CurrentLayer[0].EventDetail;
            for (int i = 0; i < CurrentLayer.Count; i++)
            {
                var model = CurrentLayer[i];
                GameObject tempObj = Resources.Load("Prefab/img_AdventureBtn") as GameObject;
                tempObj = Common.AddChild(obj_EventBtn.transform, tempObj);
                tempObj.name = $"img_AdventureBtn_{i}";
                var tempImg = tempObj.GetComponent<Image>();
                var tempTitle = tempObj.transform.Find("txt_Title").GetComponent<Text>();
                var tempEffect = tempObj.transform.Find("txt_Effect").GetComponent<Text>();
                Common.ImageBind(model.EventEffectImgUrl, tempImg);
                tempTitle.text = model.EventEffectName;
                if (model.EventEffectValue > 0)
                {
                    tempEffect.text = EffectValueToName(model);
                }
                else
                {
                    tempEffect.transform.localScale = Vector3.zero;
                }
                #region 点击事件
                EventTrigger trigger2 = tempObj.GetComponent<EventTrigger>();
                if (trigger2 == null)
                {
                    trigger2 = tempObj.AddComponent<EventTrigger>();
                }
                EventTrigger.Entry entry2 = new EventTrigger.Entry();
                entry2.callback.AddListener(delegate { EventBtnClick(model, tempObj); });
                trigger2.triggers.Add(entry2);
                #endregion
            }
        }
    }

    /// <summary>
    /// 事件按钮点击
    /// </summary>
    /// <param name="model"></param>
    /// <param name="thisObj"></param>
    public void EventBtnClick(AdventureModel model, GameObject thisObj)
    {
        HideEventUnderBtn();
        //先体现效果，再绑定新事件
        if (model.EventEffectType == 1)
        {
            if (model.RewardType == 7)
            {
                var btn = thisObj.transform.Find("btn_Adventure")?.GetComponent<Button>();
                if (btn != null)
                {
                    btn.transform.localScale = Vector3.one;
                }
                else
                {
                    //返回和奖励下都添加一个按钮
                    GameObject tempObj = Resources.Load("Prefab/btn_Adventure") as GameObject;
                    tempObj = Common.AddChild(thisObj.transform, tempObj);
                    tempObj.name = $"btn_Adventure";
                    var thisTxt = tempObj.transform.Find("Text").GetComponent<Text>();
                    thisTxt.text = "战   斗";
                    var thisBtn = tempObj.GetComponent<Button>();
                    thisBtn.onClick.AddListener(delegate { BtnClick(model); });
                    PlayerRole.AdventureIds = model.RelatedEventIDs;
                }
            }
            else
            {
                CurrentLayer = new List<AdventureModel>();
                if (!string.IsNullOrWhiteSpace(model.RelatedEventIDs))
                {
                    var arr = model.RelatedEventIDs.Split(',');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        var entity = CurrentAdt.Find(a => a.ID == arr[i]);
                        if (entity != null)
                        {
                            CurrentLayer.Add(entity);
                        }
                    }
                    CurrentLayer.OrderBy(a => a.EventLayerLevelSort);
                    //修改文件角色/要即时刷新
                    if (PlayerRole != null)
                    {
                        switch (model.RewardType)
                        {
                            case 1:
                                PlayerRole.HP -= model.EventEffectValue;
                                if (PlayerRole.HP <= 0)
                                {
                                    Common.SceneJump("PlayerDieScene");
                                }
                                txt_HP.text = $"{PlayerRole.MaxHP}/{PlayerRole.HP}";
                                Common.HPImageChange(img_HP, PlayerRole.MaxHP, model.EventEffectValue, 0, 150);
                                break;
                            case 2:
                                PlayerRole.HP += model.EventEffectValue;
                                if (PlayerRole.HP > PlayerRole.MaxHP)
                                {
                                    PlayerRole.HP = PlayerRole.MaxHP;
                                }
                                txt_HP.text = $"{PlayerRole.MaxHP}/{PlayerRole.HP}";
                                Common.HPImageChange(img_HP, PlayerRole.MaxHP, model.EventEffectValue, 1, 150);
                                break;
                            case 3:
                                PlayerRole.Wealth -= model.EventEffectValue;
                                if (PlayerRole.Wealth < model.EventEffectValue)
                                {
                                    //不允许进行操作
                                }
                                txt_Wealth.text = PlayerRole.Wealth.ToString();
                                break;
                            case 4:
                                PlayerRole.Wealth += model.EventEffectValue;
                                txt_Wealth.text = PlayerRole.Wealth.ToString();
                                break;
                            case 5:
                                PlayerRole.MaxHP -= model.EventEffectValue;
                                PlayerRole.HP -= model.EventEffectValue;
                                txt_HP.text = $"{PlayerRole.MaxHP}/{PlayerRole.HP}";
                                break;
                            case 6:
                                PlayerRole.MaxHP += model.EventEffectValue;
                                PlayerRole.HP += model.EventEffectValue;
                                txt_HP.text = $"{PlayerRole.MaxHP}/{PlayerRole.HP}";
                                break;
                        }
                        Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
                    }
                    DataBind();
                }
            }
        }
        else
        {
            var btn = thisObj.transform.Find("btn_Adventure")?.GetComponent<Button>();
            if (btn != null)
            {
                btn.transform.localScale = Vector3.one;
            }
            else
            {
                //返回和奖励下都添加一个按钮
                GameObject tempObj = Resources.Load("Prefab/btn_Adventure") as GameObject;
                tempObj = Common.AddChild(thisObj.transform, tempObj);
                tempObj.name = $"btn_Adventure";
                var thisTxt = tempObj.transform.Find("Text").GetComponent<Text>();
                if (model.EventEffectType == 0)
                {
                    thisTxt.text = "返   回";
                }
                else
                {
                    thisTxt.text = "获   取";
                }
                var thisBtn = tempObj.GetComponent<Button>();
                thisBtn.onClick.AddListener(delegate { BtnClick(model); });
            }
        }
    }

    public void BtnClick(AdventureModel model)
    {
        if (model.EventEffectType == 0)
        {
            Common.SceneJump("MapScene");
        }
        else if (model.EventEffectType == 1)
        {
            if (model.RewardType == 7)
            {
                Common.SceneJump("GameScene");//战斗奖励固定
            }
        }
        else
        {
            var RewardList = new List<CurrentCardPoolModel>();
            switch (model.RewardType)
            {
                case 1:
                    PlayerRole.HP -= model.EventEffectValue;
                    if (PlayerRole.HP <= 0)
                    {
                        Common.SceneJump("PlayerDieScene");
                    }
                    Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
                    Common.SceneJump("MapScene");
                    break;
                case 2:
                    PlayerRole.HP += model.EventEffectValue;
                    if (PlayerRole.HP > PlayerRole.MaxHP)
                    {
                        PlayerRole.HP = PlayerRole.MaxHP;
                    }
                    Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
                    Common.SceneJump("MapScene");
                    break;
                case 3:
                    PlayerRole.Wealth -= model.EventEffectValue;
                    if (PlayerRole.Wealth < model.EventEffectValue)
                    {
                        //不允许进行操作
                    }
                    Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
                    Common.SceneJump("MapScene");
                    break;
                case 4:
                    PlayerRole.Wealth += model.EventEffectValue;
                    Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
                    Common.SceneJump("MapScene");
                    break;
                case 5:
                    PlayerRole.MaxHP -= model.EventEffectValue;
                    PlayerRole.HP -= model.EventEffectValue;
                    Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
                    Common.SceneJump("MapScene");
                    break;
                case 6:
                    PlayerRole.MaxHP += model.EventEffectValue;
                    PlayerRole.HP += model.EventEffectValue;
                    Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
                    Common.SceneJump("MapScene");
                    break;
                case 7:
                    break;
                case 8:
                    //点击后展示卡牌奖励
                    obj_EventBtn.SetActive(false);
                    GlobalCardList.ListRandom();
                    for (int i = 0; i < model.EventEffectValue; i++)
                    {
                        RewardList.Add(GlobalCardList[i]);
                    }
                    CardsReward(RewardList);
                    break;
                case 9:
                    obj_EventBtn.SetActive(false);
                    GlobalCardList.ListRandom();
                    for (int i = 0; i < model.EventEffectValue; i++)
                    {
                        RewardList.Add(GlobalCardList[i]);
                    }
                    CardsReward(RewardList);
                    break;
                case 10:
                    obj_EventBtn.SetActive(false);
                    GlobalCardList.ListRandom();
                    for (int i = 0; i < model.EventEffectValue; i++)
                    {
                        RewardList.Add(GlobalCardList[i]);
                    }
                    CardsReward(RewardList);
                    break;
                case 11:
                    obj_EventBtn.SetActive(false);
                    GlobalCardList.ListRandom();
                    for (int i = 0; i < model.EventEffectValue; i++)
                    {
                        RewardList.Add(GlobalCardList[i]);
                    }
                    CardsReward(RewardList);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 展示卡牌奖励
    /// </summary>
    /// <param name="list"></param>
    public void CardsReward(List<CurrentCardPoolModel> list)
    {
        obj_CardsReward.SetActive(true);
        int count = obj_CardsReward.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            DestroyImmediate(obj_CardsReward.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
        }
        for (int i = 0; i < list?.Count; i++)
        {
            var model = list[0];
            GameObject tempObject = Resources.Load("Prefab/img_Card") as GameObject;
            tempObject = Common.AddChild(obj_CardsReward.transform, tempObject);
            tempObject.name = "img_AwardCard" + i;
            //EventTrigger trigger = tempObject.GetComponent<EventTrigger>();
            //if (trigger == null)
            //{
            //    trigger = tempObject.AddComponent<EventTrigger>();
            //}
            //EventTrigger.Entry entry = new EventTrigger.Entry();
            //entry.callback.AddListener(delegate { ShowCardDetail(model, i, list.Count); });
            //trigger.triggers.Add(entry);
            GameObject tempObj = Resources.Load("Prefab/btn_Adventure") as GameObject;
            tempObj = Common.AddChild(tempObject.transform, tempObj);
            tempObj.name = $"btn_Adventure";
            var thisTxt = tempObj.transform.Find("Text").GetComponent<Text>();
            thisTxt.text = "确   定";
            var thisBtn = tempObj.GetComponent<Button>();
            thisBtn.onClick.AddListener(delegate { SaveCard(model); });

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
    }

    private void SaveCard(CurrentCardPoolModel model)
    {
        var currentCards = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentCardPoolsFileName);
        currentCards.Add(model);
        Common.SaveTxtFile(currentCards.ListToJson(), GlobalAttr.CurrentCardPoolsFileName);
        obj_CardsReward.SetActive(false);
        obj_EventBtn.SetActive(true);
        Common.SceneJump("MapScene");
    }

    /// <summary>
    /// 效果值转文字
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    private string EffectValueToName(AdventureModel model)
    {
        string result = "";
        switch (model?.RewardType)
        {
            case 1:
                result = $"-{model.EventEffectValue}生命值";
                break;
            case 2:
                result = $"+{model.EventEffectValue}生命值";
                break;
            case 3:
                result = $"-{model.EventEffectValue}银币";
                break;
            case 4:
                result = $"+{model.EventEffectValue}银币";
                break;
            case 5:
                result = $"-{model.EventEffectValue}最大生命值";
                break;
            case 6:
                result = $"+{model.EventEffectValue}最大生命值";
                break;

        }
        return result;
    }

    /// <summary>
    /// 隐藏事件下的按钮
    /// </summary>
    public void HideEventUnderBtn()
    {
        for (int i = 0; i < CurrentLayer?.Count; i++)
        {
            var btn = transform.Find($"obj_EventBtn/img_AdventureBtn_{i}/btn_Adventure")?.GetComponent<Button>();
            if (btn != null)
            {
                btn.transform.localScale = Vector3.zero;
            }
        }
    }
}
