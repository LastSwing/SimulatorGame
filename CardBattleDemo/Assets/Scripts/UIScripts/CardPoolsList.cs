using Assets.Scripts.LogicalScripts.Models;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPoolsList : MonoBehaviour
{
    List<CurrentCardPoolModel> CardList = new List<CurrentCardPoolModel>();
    GameObject Content_Obj;
    Text txt_CardType;
    RectTransform Content_Rect;
    public bool CreateState = true;
    void Start()
    {
        Content_Obj = transform.Find("CardPoolsArea/CardPools/Content").gameObject;
        Content_Rect = transform.Find("CardPoolsArea/CardPools/Content").GetComponent<RectTransform>();

        txt_CardType = transform.Find("txt_CardType")?.GetComponent<Text>();

        CardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentCardPoolsFileName);
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

    }

    /// <summary>
    /// 不可用时执行
    /// </summary>
    private void OnDisable()
    {
        CreateState = true;
    }

    private void Update()
    {
        if (CreateState)
        {
            //每次进来创建一次足矣
            if (txt_CardType != null)
            {
                txt_CardType.transform.localScale = Vector3.zero;
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
        }
    }

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
                float height = (float)(210 * System.Math.Ceiling((float)CardList.Count / 3f));
                for (int i = 0; i < CardList?.Count; i++)
                {
                    CreateAwardCrad(CardList[i], i);
                }
                Content_Rect.sizeDelta = new Vector2(0, height);
            }
            CreateState = false;
        }
    }

    /// <summary>
    /// 创建卡
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
            GameObject tempImg = Resources.Load("Prefab/img_Detail") as GameObject;
            tempImg = Common.AddChild(Card_img.transform, tempImg);
            tempImg.name = "img_Detail" + i;
            tempImg.transform.localPosition = new Vector2(0, 0);

            GameObject temp = tempImg.transform.Find("Text").gameObject;
            temp.GetComponent<Text>().text = $"{model.CardName}\n{model.CardDetail}";
        }
    }
}
