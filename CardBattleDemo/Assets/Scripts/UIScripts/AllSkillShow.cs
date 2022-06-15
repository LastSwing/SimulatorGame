using Assets.Scripts.LogicalScripts.Models;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AllSkillShow : MonoBehaviour
{
    List<CurrentCardPoolModel> CardList = new List<CurrentCardPoolModel>();//展示卡池
    List<CurrentCardPoolModel> GlobalCardPools = new List<CurrentCardPoolModel>();//游戏全局卡池
    List<CurrentCardPoolModel> GlobalPlayerCardPools = new List<CurrentCardPoolModel>();//角色全局卡池
    GameObject Content_Obj;
    RectTransform Content_Rect;
    Text txt_AllSkillCount;
    Button btn_AllSkill, btn_Atk, btn_function, btn_Debuff, btn_Return;
    void Start()
    {
        Content_Obj = transform.Find("CardPoolsArea/CardPools/Content").gameObject;
        Content_Rect = transform.Find("CardPoolsArea/CardPools/Content").GetComponent<RectTransform>();
        txt_AllSkillCount = transform.Find("txt_AllSkillCount").GetComponent<Text>();

        btn_AllSkill = transform.Find("left_Btn_Obj/btn_AllSkill").GetComponent<Button>();
        btn_AllSkill.onClick.AddListener(AllSkillClick);
        btn_Atk = transform.Find("left_Btn_Obj/btn_Atk").GetComponent<Button>();
        btn_Atk.onClick.AddListener(AtkClick);
        btn_function = transform.Find("left_Btn_Obj/btn_function").GetComponent<Button>();
        btn_function.onClick.AddListener(FunctionClick);
        btn_Debuff = transform.Find("left_Btn_Obj/btn_Debuff").GetComponent<Button>();
        btn_Debuff.onClick.AddListener(DebuffClick);

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

        #region 数据源
        GlobalCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalCardPoolFileName);
        GlobalPlayerCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName);
        #endregion
        CardList = GlobalCardPools;
        txt_AllSkillCount.text = $"{GlobalPlayerCardPools?.Count}/{GlobalCardPools?.Count}";
        CreateCardPools(GlobalPlayerCardPools, GlobalCardPools);
        BtnColorUpdate(0);
    }

    #region 卡池创建
    /// <summary>
    /// 创建卡池
    /// </summary>
    /// <param name="HaveCardList">所拥有的的卡池</param>
    /// <param name="AllCardList">所有的卡池</param>
    public void CreateCardPools(List<CurrentCardPoolModel> HaveCardList, List<CurrentCardPoolModel> AllCardList)
    {
        if (Content_Obj != null)
        {
            int childCount = Content_Obj.transform.childCount;
            for (int x = 0; x < childCount; x++)
            {
                DestroyImmediate(Content_Obj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
        }
        if (AllCardList != null && AllCardList?.Count > 0)
        {
            float height = (float)(210 * System.Math.Ceiling((float)AllCardList.Count / 5f));
            Content_Rect.sizeDelta = new Vector2(0, height);
            for (int i = 0; i < AllCardList?.Count; i++)
            {
                var model = HaveCardList.Find(a => a.ID == AllCardList[i].ID);
                if (model != null)
                {
                    CreateAwardCrad(AllCardList[i], i);
                }
                else
                {
                    CreateQuestionCard(AllCardList[i], i);
                }
            }
        }
    }

    /// <summary>
    /// 创建未拥有的卡
    /// </summary>
    /// <param name="model"></param>
    /// <param name="i"></param>
    private void CreateQuestionCard(CurrentCardPoolModel model, int i)
    {
        GameObject tempObject = Resources.Load("Prefab/img_QuestionCard") as GameObject;
        tempObject = Common.AddChild(Content_Obj.transform, tempObject);
        tempObject.name = "img_Card" + i;
        EventTrigger trigger = tempObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = tempObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener(delegate { ShowDetail(model, i, 1); });
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// 创建已拥有的卡
    /// </summary>
    /// <param name="model"></param>
    /// <param name="i"></param>
    private void CreateAwardCrad(CurrentCardPoolModel model, int i)
    {
        GameObject tempObject = Resources.Load("Prefab/img_Card") as GameObject;
        tempObject = Common.AddChild(Content_Obj.transform, tempObject);
        tempObject.name = "img_Card" + i;
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
    /// <param name="type">1未拥有的卡</param>
    public void ShowDetail(CurrentCardPoolModel model, int i, int type = 0)
    {
        HideCardDetail();
        var Card_Detail = transform.Find($"CardDetails/img_Detail{i}")?.GetComponent<Image>();
        if (Card_Detail != null)
        {
            var Card_img = Content_Obj.transform.Find($"img_Card{i}").gameObject;
            if (type == 0)
            {
                Card_Detail.transform.position = new Vector2(Card_img.transform.position.x, Card_img.transform.position.y + 160);
            }
            else
            {
                Card_Detail.transform.position = new Vector2(Card_img.transform.position.x, Card_img.transform.position.y + 130);
            }
            Card_Detail.transform.localScale = Vector3.one;
        }
        else
        {
            if (type == 0)
            {
                var Card_Deatils = transform.Find("CardDetails").gameObject;
                var Card_img = Content_Obj.transform.Find($"img_Card{i}").gameObject;
                GameObject tempImg = Resources.Load("Prefab/img_Detail") as GameObject;
                tempImg = Common.AddChild(Card_Deatils.transform, tempImg);
                tempImg.name = "img_Detail" + i;
                tempImg.transform.position = new Vector2(Card_img.transform.position.x, Card_img.transform.position.y + 160);

                GameObject temp = tempImg.transform.Find("Text").gameObject;
                temp.GetComponent<Text>().text = $"{model.CardName}\n{model.CardDetail}";
            }
            else
            {
                //img_QuestionDetail
                var Card_Deatils = transform.Find("CardDetails").gameObject;
                var Card_img = Content_Obj.transform.Find($"img_Card{i}").gameObject;
                GameObject tempImg = Resources.Load("Prefab/img_QuestionDetail") as GameObject;
                tempImg = Common.AddChild(Card_Deatils.transform, tempImg);
                tempImg.name = "img_Detail" + i;
                tempImg.transform.position = new Vector2(Card_img.transform.position.x, Card_img.transform.position.y + 130);

                GameObject temp = tempImg.transform.Find("Text").gameObject;
                string typeName = "";
                if (model.CardType == 2)
                {
                    typeName = "功能卡";
                }
                else
                {
                    typeName = "攻击卡";
                }
                if (model.HasDeBuff == 1)
                {
                    typeName = "黑卡";
                }
                temp.GetComponent<Text>().text = $"{typeName.TextSpacing()}";
            }
        }
    }

    /// <summary>
    /// 隐藏详情
    /// </summary>
    public void HideCardDetail()
    {
        for (int i = 0; i < CardList.Count; i++)
        {
            var Card_img = GameObject.Find($"CardDetails/img_Detail{i}")?.GetComponent<Image>();
            if (Card_img != null)
            {
                Card_img.transform.localScale = Vector3.zero;
            }
        }
    }
    #endregion

    #region 按钮点击方法
    /// <summary>
    /// 0全部，1攻击、2功能、3黑卡
    /// </summary>
    /// <param name="btnType"></param>
    private void BtnColorUpdate(int btnType)
    {
        #region 基础颜色
        Color green = new Color32(21, 56, 60, 255);
        Color white = new Color32(238, 239, 199, 255);
        ColorBlock ClickCb = new ColorBlock();
        ClickCb.normalColor = white;
        ClickCb.highlightedColor = white;
        ClickCb.pressedColor = white;
        ClickCb.disabledColor = white;
        ClickCb.selectedColor = white;
        ClickCb.colorMultiplier = 1;
        ClickCb.fadeDuration = 0.1f;
        ColorBlock NotClickCb = btn_AllSkill.colors;
        NotClickCb.normalColor = green;
        NotClickCb.highlightedColor = green;
        NotClickCb.pressedColor = green;
        NotClickCb.disabledColor = green;
        NotClickCb.selectedColor = green;
        ClickCb.colorMultiplier = 1;
        ClickCb.fadeDuration = 0.1f;

        Text txt_AllSkill = btn_AllSkill.transform.Find("Text").GetComponent<Text>();
        Text txt_Atk = btn_Atk.transform.Find("Text").GetComponent<Text>();
        Text txt_function = btn_function.transform.Find("Text").GetComponent<Text>();
        Text txt_Debuff = btn_Debuff.transform.Find("Text").GetComponent<Text>(); 
        #endregion
        switch (btnType)
        {
            case 1:
                btn_AllSkill.colors = NotClickCb;
                btn_Atk.colors = ClickCb;
                btn_function.colors = NotClickCb;
                btn_Debuff.colors = NotClickCb;

                txt_AllSkill.color = white;
                txt_Atk.color = green;
                txt_function.color = white;
                txt_Debuff.color = white;
                break;
            case 2:
                btn_AllSkill.colors = NotClickCb;
                btn_Atk.colors = NotClickCb;
                btn_function.colors = ClickCb;
                btn_Debuff.colors = NotClickCb;

                txt_AllSkill.color = white;
                txt_Atk.color = white;
                txt_function.color = green;
                txt_Debuff.color = white;
                break;
            case 3:
                btn_AllSkill.colors = NotClickCb;
                btn_Atk.colors = NotClickCb;
                btn_function.colors = NotClickCb;
                btn_Debuff.colors = ClickCb;

                txt_AllSkill.color = white;
                txt_Atk.color = white;
                txt_function.color = white;
                txt_Debuff.color = green;
                break;
            default:
                btn_AllSkill.colors = ClickCb;
                btn_Atk.colors = NotClickCb;
                btn_function.colors = NotClickCb;
                btn_Debuff.colors = NotClickCb;

                txt_AllSkill.color = green;
                txt_Atk.color = white;
                txt_function.color = white;
                txt_Debuff.color = white;
                break;
        }
    }
    public void AllSkillClick()
    {
        BtnColorUpdate(0);
        txt_AllSkillCount.text = $"{GlobalPlayerCardPools?.Count}/{GlobalCardPools?.Count}";
        CreateCardPools(GlobalPlayerCardPools, GlobalCardPools);
    }
    public void AtkClick()
    {
        BtnColorUpdate(1);
        var palyerAtkList = GlobalPlayerCardPools.FindAll(a => a.CardType != 2 && a.HasDeBuff == 0);
        var globalAtkList = GlobalCardPools.FindAll(a => a.CardType != 2 && a.HasDeBuff == 0);
        txt_AllSkillCount.text = $"{palyerAtkList?.Count}/{globalAtkList?.Count}";
        CreateCardPools(palyerAtkList, globalAtkList);
    }
    public void FunctionClick()
    {
        BtnColorUpdate(2);
        var palyerFunctionList = GlobalPlayerCardPools.FindAll(a => a.CardType == 2 && a.HasDeBuff == 0);
        var globalFunctionList = GlobalCardPools.FindAll(a => a.CardType == 2 && a.HasDeBuff == 0);
        txt_AllSkillCount.text = $"{palyerFunctionList?.Count}/{globalFunctionList?.Count}";
        CreateCardPools(palyerFunctionList, globalFunctionList);
    }
    public void DebuffClick()
    {
        BtnColorUpdate(3);
        var palyerDebuffList = GlobalPlayerCardPools.FindAll(a => a.HasDeBuff == 1);
        var globalDebuffList = GlobalCardPools.FindAll(a => a.HasDeBuff == 1);
        txt_AllSkillCount.text = $"{(palyerDebuffList == null ? 0 : palyerDebuffList.Count)}/{(globalDebuffList == null ? 0 : globalDebuffList.Count)}";
        CreateCardPools(palyerDebuffList, globalDebuffList);
    }
    #endregion
}
