using Assets.Script.Models;
using Assets.Script.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPoolsView : BaseUI
{
    List<CurrentCardPoolModel> CardList = new List<CurrentCardPoolModel>();
    GameObject Content_Obj;
    Text txt_CardType, txt_ReturnView;//0当前玩家卡池;1未使用的卡池;2已使用的卡池
    RectTransform Content_Rect;
    Image img_Background;//背景图片
    Button btn_Return;
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
        btn_Return = transform.Find("UI/btn_Return").GetComponent<Button>();

        txt_ReturnView = GameObject.Find("MainCanvas/txt_ReturnView").GetComponent<Text>();
        txt_CardType = GameObject.Find("MainCanvas/txt_CardType").GetComponent<Text>();
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
        btn_Return.onClick.AddListener(ReturnClick);
    }

    public void ReturnClick()
    {
        UIManager.instance.OpenView(txt_ReturnView.text);
        UIManager.instance.CloseView("CardPoolsView");
    }

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

    #region 卡池创建

    public void CreateCardPools()
    {
        if (Content_Obj != null)
        {
            int childCount = Content_Obj.transform.childCount;
            for (int x = 0; x < childCount; x++)
            {
                DestroyImmediate(Content_Obj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
            if (CardList != null && CardList?.Count > 0)
            {
                float height = (float)(290 * System.Math.Ceiling((float)CardList.Count / 3f));
                for (int i = 0; i < CardList?.Count; i++)
                {
                    CreateAwardCrad(CardList[i], i);
                }
                Content_Rect.sizeDelta = new Vector2(0, height);
            }
        }
    }

    /// <summary>
    /// 创建卡
    /// </summary>
    /// <param name="model"></param>
    /// <param name="i"></param>
    private void CreateAwardCrad(CurrentCardPoolModel model, int i)
    {
        GameObject tempObject = Resources.Load("Prefabs/img_Card240") as GameObject;
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

    /// <summary>
    /// 显示详情
    /// </summary>
    /// <param name="model"></param>
    /// <param name="i"></param>
    public void ShowDetail(CurrentCardPoolModel model, int i)
    {
        HideCardDetail();
        var Card_Detail = GameObject.Find($"CardDetails/img_Detail{i}")?.GetComponent<Image>();
        if (Card_Detail != null)
        {
            Card_Detail.transform.localScale = Vector3.one;
        }
        else
        {
            var Card_img = GameObject.Find($"CardDetails");
            GameObject tempImg = Resources.Load("Prefabs/img_CardDetail") as GameObject;
            tempImg = Common.AddChild(Card_img.transform, tempImg);
            tempImg.name = "img_Detail" + i;
            tempImg.transform.localPosition = new Vector2(0, 0);

            GameObject temp = tempImg.transform.Find("Text").gameObject;
            temp.GetComponent<Text>().text = $"{model.CardName}\n{model.CardDetail}";
        }
    }
    #endregion

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
        if (txt_CardType.text == "1")
        {
            CardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUnUsedCardPoolsFileName);
            CreateCardPools();
        }
        else if (txt_CardType.text == "2")
        {
            CardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUsedCardPoolsFileName);
            CreateCardPools();
        }
        else
        {
            CardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentCardPoolsFileName);
            CreateCardPools();
        }
    }

    public override void OnClose()
    {

    }
}
