using Assets.Script.Models;
using Assets.Script.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AllSkillView : BaseUI
{

    Image img_Background;//背景图片
    List<CurrentCardPoolModel> CardList = new List<CurrentCardPoolModel>();//展示卡池
    List<CurrentCardPoolModel> GlobalCardPools = new List<CurrentCardPoolModel>();//游戏全局卡池
    List<CurrentCardPoolModel> GlobalPlayerCardPools = new List<CurrentCardPoolModel>();//角色全局卡池
    GameObject Content_Obj;
    RectTransform Content_Rect;
    Text txt_AllSkillCount, txt_ReturnView;
    Button btn_AllSkill, btn_Atk, btn_function, btn_Debuff, btn_Return;
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
        Content_Obj = transform.Find("UI/CardPoolsArea/CardPools/Content").gameObject;
        Content_Rect = transform.Find("UI/CardPoolsArea/CardPools/Content").GetComponent<RectTransform>();
        txt_AllSkillCount = transform.Find("UI/txt_AllSkillCount").GetComponent<Text>();
        txt_ReturnView = GameObject.Find("MainCanvas/txt_ReturnView").GetComponent<Text>();

        btn_AllSkill = transform.Find("UI/left_Btn_Obj/btn_AllSkill").GetComponent<Button>();
        btn_Atk = transform.Find("UI/left_Btn_Obj/btn_Atk").GetComponent<Button>();
        btn_function = transform.Find("UI/left_Btn_Obj/btn_function").GetComponent<Button>();
        btn_Debuff = transform.Find("UI/left_Btn_Obj/btn_Debuff").GetComponent<Button>();
        btn_Return = transform.Find("UI/btn_Return").GetComponent<Button>();

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
        btn_AllSkill.onClick.AddListener(AllSkillClick);
        btn_Atk.onClick.AddListener(AtkClick);
        btn_function.onClick.AddListener(FunctionClick);
        btn_Debuff.onClick.AddListener(DebuffClick);
        btn_Return.onClick.AddListener(ReturnClick);
    }

    #region 按钮点击方法
    /// <summary>
    /// 0全部，1攻击、2功能、3黑卡
    /// </summary>
    /// <param name="btnType"></param>
    private void BtnColorUpdate(int btnType)
    {
        HideCardDetail();
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
        var palyerAtkList = GlobalPlayerCardPools.FindAll(a => a.CardType == 0 );
        var globalAtkList = GlobalCardPools.FindAll(a => a.CardType == 0);
        txt_AllSkillCount.text = $"{palyerAtkList?.Count}/{globalAtkList?.Count}";
        CreateCardPools(palyerAtkList, globalAtkList);
    }
    public void FunctionClick()
    {
        BtnColorUpdate(2);
        var palyerFunctionList = GlobalPlayerCardPools.FindAll(a => a.CardType == 1 );
        var globalFunctionList = GlobalCardPools.FindAll(a => a.CardType == 1 );
        txt_AllSkillCount.text = $"{palyerFunctionList?.Count}/{globalFunctionList?.Count}";
        CreateCardPools(palyerFunctionList, globalFunctionList);
    }
    public void DebuffClick()
    {
        BtnColorUpdate(3);
        var palyerDebuffList = GlobalPlayerCardPools.FindAll(a => a.CardType == 2);
        var globalDebuffList = GlobalCardPools.FindAll(a => a.CardType == 2);
        txt_AllSkillCount.text = $"{(palyerDebuffList == null ? 0 : palyerDebuffList.Count)}/{(globalDebuffList == null ? 0 : globalDebuffList.Count)}";
        CreateCardPools(palyerDebuffList, globalDebuffList);
    }

    public void ReturnClick()
    {
        UIManager.instance.OpenView(txt_ReturnView.text);
        UIManager.instance.CloseView("AllSkillView");
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
        txt_AllSkillCount.text = $"{GlobalPlayerCardPools?.Count}/{GlobalCardPools?.Count}";
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void InitUIData()
    {
        #region 数据源
        GlobalCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalCardPoolFileName);
        GlobalPlayerCardPools = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName);
        #endregion
        CardList = GlobalCardPools;
        CreateCardPools(GlobalPlayerCardPools, GlobalCardPools);
        BtnColorUpdate(0);
    } 
    #endregion

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
            float height = (float)(290 * System.Math.Ceiling((float)AllCardList.Count / 4f));
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
        GameObject tempObject = ResourcesManager.instance.Load("img_QuestionCard") as GameObject;
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
        GameObject tempObject = ResourcesManager.instance.Load("img_Card240") as GameObject;
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

        Common.CardDataBind(tempObject, model);
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
        var Card_Detail = transform.Find($"UI/CardDetails/img_Detail{model.ID}")?.GetComponent<Image>();
        if (Card_Detail != null)
        {
            var Card_img = Content_Obj.transform.Find($"img_Card{i}").gameObject;
            if (type == 0)
            {
                Card_Detail.transform.position = new Vector2(Card_img.transform.position.x, Card_img.transform.position.y - 50);
            }
            else
            {
                Card_Detail.transform.position = new Vector2(Card_img.transform.position.x, Card_img.transform.position.y - 50);
            }
            Card_Detail.transform.localScale = Vector3.one;
        }
        else
        {
            if (type == 0)
            {
                var Card_Deatils = transform.Find("UI/CardDetails").gameObject;
                var Card_img = Content_Obj.transform.Find($"img_Card{i}").gameObject;
                //RectTransform Irect = Card_img.GetComponent<RectTransform>();
                //var imgHeight = Irect.sizeDelta.y;
                GameObject tempImg = ResourcesManager.instance.Load("img_CardDetail") as GameObject;
                tempImg = Common.AddChild(Card_Deatils.transform, tempImg);
                tempImg.name = "img_Detail" + model.ID;
                tempImg.transform.position = new Vector2(Card_img.transform.position.x, Card_img.transform.position.y - 50);

                GameObject temp = tempImg.transform.Find("Text").gameObject;
                temp.GetComponent<Text>().text = $"{model.CardName}\n{model.CardDetail}";
            }
            else
            {
                //img_QuestionDetail
                var Card_Deatils = transform.Find("UI/CardDetails").gameObject;
                var Card_img = Content_Obj.transform.Find($"img_Card{i}").gameObject;
                GameObject tempImg = ResourcesManager.instance.Load("img_QuestionDetail") as GameObject;
                tempImg = Common.AddChild(Card_Deatils.transform, tempImg);
                tempImg.name = "img_Detail" + model.ID;
                tempImg.transform.position = new Vector2(Card_img.transform.position.x, Card_img.transform.position.y - 50);

                GameObject temp = tempImg.transform.Find("Text").gameObject;
                string typeName = "";
                if (model.CardType == 1)
                {
                    typeName = "功能卡";
                }
                else if (model.CardType == 0)
                {
                    typeName = "攻击卡";
                }
                else
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
        foreach (var item in CardList)
        {
            var Card_img = GameObject.Find($"UI/CardDetails/img_Detail{item.ID}")?.GetComponent<Image>();
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
