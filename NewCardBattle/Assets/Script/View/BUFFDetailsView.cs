using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BUFFDetailsView : BaseUI
{
    Dictionary<string, string> dicBuffEffect = new Dictionary<string, string>();
    Dictionary<string, string> dicBuffUrl = new Dictionary<string, string>();
    Dictionary<string, string> dicBuffName = new Dictionary<string, string>();
    List<BuffData> buffList = new List<BuffData>();
    GameObject Content_Obj;
    Text txt_ClickPlayerOrAI, txt_ReturnView;//0当前玩家卡池;1未使用的卡池;2已使用的卡池
    RectTransform Content_Rect;
    Image img_Background;//背景图片
    Button btn_Return;
    public override void OnInit()
    {
        InitComponent();
        InitUIevent();
    }

    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitComponent()
    {
        img_Background = transform.Find("BG").GetComponent<Image>();
        Content_Obj = transform.Find("UI/BUFFDetails/BUFFPools/Content").gameObject;
        Content_Rect = transform.Find("UI/BUFFDetails/BUFFPools/Content").GetComponent<RectTransform>();
        btn_Return = transform.Find("UI/btn_Return").GetComponent<Button>();

        txt_ReturnView = GameObject.Find("MainCanvas/txt_ReturnView").GetComponent<Text>();
        txt_ClickPlayerOrAI = GameObject.Find("MainCanvas/txt_ClickPlayerOrAI").GetComponent<Text>();
    }

    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {
        btn_Return.onClick.AddListener(ReturnClick);
    }

    public void ReturnClick()
    {
        UIManager.instance.OpenView(txt_ReturnView.text);
        UIManager.instance.CloseView("BUFFDetailsView");
    }
    public override void OnOpen()
    {
        //数据需要每次打开都要刷新，UI状态也是要每次打开都进行刷新，因此放在OnOpen
        InitUIData();
        InitUIState();
        InitSetting();
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void InitUIData()
    {
        dicBuffEffect = new Dictionary<string, string>();
        dicBuffUrl = new Dictionary<string, string>();
        dicBuffName = new Dictionary<string, string>();
        buffList = new List<BuffData>();
        Common.DicDataRead(ref dicBuffName, GlobalAttr.EffectTypeFileName, "Card");
        Common.DicDataRead(ref dicBuffEffect, GlobalAttr.BUFFEffectFileName, "Card");
        Common.DicDataRead(ref dicBuffUrl, GlobalAttr.BUFFUrlFileName, "Card");
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
        #region 清空BUFF栏
        if (Content_Obj != null)
        {
            int childCount = Content_Obj.transform.childCount;
            for (int x = 0; x < childCount; x++)
            {
                DestroyImmediate(Content_Obj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
        }
        #endregion
        if (txt_ClickPlayerOrAI.text == "0")
        {
            if (BattleManager.instance.OwnPlayerData?[0].buffList?.Count > 0)
            {
                buffList = BattleManager.instance.OwnPlayerData[0].buffList;
                CreateBUFFPools();
            }
        }
        else if (txt_ClickPlayerOrAI.text == "1")
        {
            if (BattleManager.instance.EnemyPlayerData?[0].buffList?.Count > 0)
            {
                buffList = BattleManager.instance.EnemyPlayerData[0].buffList;
                CreateBUFFPools();
            }
        }
        else
        {
            foreach (var item in dicBuffEffect.Keys)
            {
                BuffData model = new BuffData();
                model.Name = item;
                model.EffectType = Convert.ToInt32(GetEffectType(item));
                buffList.Add(model);
            }
            CreateBUFFPools();
        }
    }

    private string GetEffectType(string Name)
    {
        string result = "";
        foreach (var item in dicBuffName)
        {
            if (item.Value == Name)
            {
                return item.Key;
            }
        }
        return result;
    }

    public void CreateBUFFPools()
    {
        if (Content_Obj != null)
        {
            int childCount = Content_Obj.transform.childCount;
            for (int x = 0; x < childCount; x++)
            {
                DestroyImmediate(Content_Obj.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
        }
        if (buffList?.Count > 0)
        {
            float height = (float)(110 * System.Math.Ceiling((float)buffList.Count / 2f));
            foreach (var item in buffList)
            {
                CreateBUFF(item);
            }
            Content_Rect.sizeDelta = new Vector2(0, height);
        }
    }

    /// <summary>
    /// 创建卡
    /// </summary>
    /// <param name="model"></param>
    /// <param name="i"></param>
    private void CreateBUFF(BuffData model)
    {
        GameObject tempObject = ResourcesManager.instance.Load("BUFF_Detail") as GameObject;
        tempObject = Common.AddChild(Content_Obj.transform, tempObject);
        tempObject.name = "buffDetails_" + model.EffectType;

        var img = tempObject.transform.Find("Image").GetComponent<Image>();
        Common.ImageBind(dicBuffUrl[model.Name], img);
        var txt = tempObject.transform.Find("Text").GetComponent<Text>();
        txt.text = dicBuffEffect[model.Name];
    }
    public override void OnClose()
    {

    }
}
