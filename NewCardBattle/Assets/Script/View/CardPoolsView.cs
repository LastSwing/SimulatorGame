using Assets.Script.Models;
using Assets.Script.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        SetSingleID(ref CardList);
    }
    /// <summary>
    /// 设置唯一ID
    /// </summary>
    /// <param name="cardList"></param>
    private void SetSingleID(ref List<CurrentCardPoolModel> cardList)
    {
        foreach (var item in cardList)
        {
            if (item.SingleID > 0)
            {

            }
            else
            {
                var id = cardList.Max(a => a.SingleID);
                if (id < 1)
                {
                    id = 1000000;
                }
                else
                {
                    id += 1;
                }
                item.SingleID = id;
            }
        }
    }
    #endregion

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
    /// 隐藏详情
    /// </summary>
    public void HideCardDetail()
    {
        for (int i = 0; i < CardList.Count; i++)
        {
            var Card_img = GameObject.Find($"CardDetails/img_Detail{CardList[i].SingleID}")?.GetComponent<Image>();
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
        var Card_Detail = GameObject.Find($"CardDetails/img_Detail{model.SingleID}")?.GetComponent<Image>();
        if (Card_Detail != null)
        {
            Card_Detail.transform.localScale = Vector3.one;
        }
        else
        {
            var Card_img = GameObject.Find($"CardDetails");
            GameObject tempImg = ResourcesManager.instance.Load("img_CardDetail") as GameObject;
            tempImg = Common.AddChild(Card_img.transform, tempImg);
            tempImg.name = "img_Detail" + model.SingleID;
            tempImg.transform.localPosition = new Vector2(0, 0);

            Common.CardDetailDataBind(tempImg, model);
        }
    }
    #endregion
    public override void OnClose()
    {

    }

}
