using Assets.Scripts.LogicalScripts.Models;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerDieScene : MonoBehaviour
{
    Button btn_Home, btn_Map, btn_Return, btn_GameOver;
    GameObject Setting_Obj, SettingCanvas, Award_Obj, AllSkillCanvas;
    Text txt_AwardGold;
    GlobalPlayerModel globalPlayerModel;
    float totalTime;
    int AccumulateCount, InitValue, MaxValue = 0;
    void Start()
    {
        btn_Home = transform.Find("btn_Home").GetComponent<Button>();
        btn_Home.onClick.AddListener(HomeClick);
        btn_Map = transform.Find("btn_Map").GetComponent<Button>();
        btn_Map.onClick.AddListener(HomeClick);
        Setting_Obj = transform.Find("TopBar/Setting").gameObject;
        SettingCanvas = GameObject.Find("SettingCanvas");
        txt_AwardGold = transform.Find("Award/Gold_Obj/img_Gold/txt_Value").GetComponent<Text>();
        Award_Obj = transform.Find("Award").gameObject;
        AllSkillCanvas = GameObject.Find("AllSkillCanvas");
        AllSkillCanvas.SetActive(false);
        #region 绑定点击设置

        btn_Return = GameObject.Find("SettingCanvas/Content/btn_Return").GetComponent<Button>();
        btn_Return.onClick.AddListener(ReturnScene);
        btn_GameOver = GameObject.Find("SettingCanvas/Content/btn_GameOver").GetComponent<Button>();
        btn_GameOver.transform.localScale = Vector3.zero;
        SettingCanvas.SetActive(false);
        EventTrigger trigger = Setting_Obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = Setting_Obj.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener(delegate { ClickSetting(); });
        trigger.triggers.Add(entry);
        #endregion

        #region 奖励
        globalPlayerModel = Common.GetTxtFileToModel<GlobalPlayerModel>(GlobalAttr.GlobalRoleFileName);
        var mapLo = Common.GetTxtFileToModel<CurrentMapLocation>(GlobalAttr.CurrentMapLocationFileName, "Map");
        if (mapLo != null)
        {
            int goldValue = mapLo.Row * Random.Range(6, 13);
            if (mapLo.HasKillBoss == 1)
            {
                goldValue += 30;
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
                GameObject img_Accumulate = Resources.Load("Prefab/img_Accumulate") as GameObject;
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

        Common.GameOverDataReset();
    }

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

    public void HomeClick()
    {
        Common.SceneJump("HomeScene");
    }

    public void MapClick()
    {
        Common.SceneJump("MapScene", 99);
    }

    public void ClickSetting()
    {
        transform.gameObject.SetActive(false);
        SettingCanvas.SetActive(true);
    }
    public void ReturnScene()
    {
        transform.gameObject.SetActive(true);
        SettingCanvas.SetActive(false);
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
            GameObject img_Card = Resources.Load("Prefab/img_Card") as GameObject;
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
        var Card_Detail = GameObject.Find($"Award/img_Accumulate{i}/img_AwardCard{i}/img_Detail")?.GetComponent<Image>();
        if (Card_Detail != null)
        {
            Card_Detail.transform.localScale = Vector3.one;
        }
        else
        {
            var Card_img = GameObject.Find($"Award/img_Accumulate{i}/img_AwardCard{i}").GetComponent<Image>();
            GameObject tempImg = Resources.Load("Prefab/img_Detail") as GameObject;
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
            var Card_img = GameObject.Find($"Award/img_Accumulate{i}/img_AwardCard{i}/img_Detail")?.GetComponent<Image>();
            if (Card_img != null)
            {
                Card_img.transform.localScale = Vector3.zero;
            }
        }
    }
}
