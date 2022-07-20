using Assets.Script.Models;
using Assets.Script.Tools;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCardScene : MonoBehaviour
{
    private Button btn_Player, btn_Save, btn_AddTrigger, btn_Add, btn_Confirm;
    GameObject TriggerCanvas;
    private Dropdown dd_CardType, dd_PlayerOrAI, dd_HasShoppingShow;
    private InputField ipt_CardName, ipt_CardUrl, ipt_Consume, ipt_Effect, ipt_CardDetail, ipt_CardLevel, ipt_CardPrice, ipt_ParentID, ipt_EffectType, ipt_AtkNumber;
    List<TriggerAfterUsing> triList = new List<TriggerAfterUsing>();//触发事件
    int triggerCount = 0;

    void Start()
    {
        #region 数据初始化

        #region 触发事件
        TriggerCanvas = transform.Find("TriggerCanvas").gameObject;
        btn_Add = transform.Find("TriggerCanvas/btn_Add").GetComponent<Button>();
        btn_Confirm = transform.Find("TriggerCanvas/btn_Confirm").GetComponent<Button>();
        btn_Add.onClick.AddListener(AddTriggerEvent);
        btn_Confirm.onClick.AddListener(ConfirmAddTriggerEvent);
        TriggerCanvas.SetActive(false);
        #endregion


        dd_CardType = transform.Find("dd_CardType").GetComponent<Dropdown>();
        dd_PlayerOrAI = transform.Find("dd_PlayerOrAI").GetComponent<Dropdown>();
        dd_HasShoppingShow = transform.Find("dd_HasShoppingShow").GetComponent<Dropdown>();

        ipt_CardName = transform.Find("ipt_CardName").GetComponent<InputField>();
        ipt_ParentID = transform.Find("ipt_ParentID").GetComponent<InputField>();
        ipt_EffectType = transform.Find("ipt_EffectType").GetComponent<InputField>();
        ipt_CardUrl = transform.Find("ipt_CardUrl").GetComponent<InputField>();
        ipt_Consume = transform.Find("ipt_Consume").GetComponent<InputField>();
        ipt_Effect = transform.Find("ipt_Effect").GetComponent<InputField>();
        ipt_AtkNumber = transform.Find("ipt_AtkNumber").GetComponent<InputField>();
        ipt_CardDetail = transform.Find("ipt_CardDetail").GetComponent<InputField>();
        ipt_CardLevel = transform.Find("ipt_CardLevel").GetComponent<InputField>();
        ipt_CardPrice = transform.Find("ipt_CardPrice").GetComponent<InputField>();

        btn_AddTrigger = transform.Find("btn_AddTrigger").GetComponent<Button>();
        btn_AddTrigger.onClick.AddListener(delegate { AddTriggerClick(); });
        btn_Player = transform.Find("PageJump/btn_Roles").GetComponent<Button>();
        btn_Player.onClick.AddListener(delegate { Common.SceneJump("RoleScene"); });
        btn_Save = transform.Find("btn_Save").GetComponent<Button>();
        btn_Save.onClick.AddListener(Save);

        #endregion
        //Common.SaveTablesStruct<TriggerAfterUsing1>("TriggerAfterUsing1");
        var model = new CardPoolModel();
        //model.TriggerAfterUsingList1 = new List<TriggerAfterUsing>();
        //TableStruct tableStruct = new TableStruct();
        //tableStruct.AttrName = "HasShoppingShow1";
        //tableStruct.ClassName = "CardPoolModel";
        //tableStruct.AttrType = model.HasShoppingShow1.GetType().FullName;
        //Common.TableAddCoulum(tableStruct);
        Common.UpdateColumnData<CardPoolModel>(1014, 1028, GlobalAttr.GlobalCardPoolFileName);
    }

    /// <summary>
    /// 点击新增触发事件
    /// </summary>
    public void AddTriggerClick()
    {
        TriggerCanvas.SetActive(true);

    }

    /// <summary>
    /// 新增触发事件
    /// </summary>
    public void AddTriggerEvent()
    {
        GameObject TriggerObj = Resources.Load("Prefabs/TriggerObj") as GameObject;
        TriggerObj = Common.AddChild(TriggerCanvas.transform, TriggerObj);
        TriggerObj.name = "TriggerObj" + triggerCount;
        TriggerObj.transform.position = new Vector3(TriggerObj.transform.position.x, TriggerObj.transform.position.y - (100 * triggerCount));
        triggerCount++;
    }

    /// <summary>
    /// 确认新增触发事件
    /// </summary>
    public void ConfirmAddTriggerEvent()
    {
        int count = TriggerCanvas.transform.childCount;
        if (count > 2)
        {
            for (int i = 0; i < count - 2; i++)
            {
                var ipt_TriggerCondition = transform.Find($"TriggerCanvas/TriggerObj{i}/ipt_TriggerCondition").GetComponent<InputField>();
                var ipt_TriggerState = transform.Find($"TriggerCanvas/TriggerObj{i}/ipt_TriggerState").GetComponent<InputField>();
                var ipt_TriggerValue = transform.Find($"TriggerCanvas/TriggerObj{i}/ipt_TriggerValue").GetComponent<InputField>();
                var ipt_TriggerSort = transform.Find($"TriggerCanvas/TriggerObj{i}/ipt_TriggerSort").GetComponent<InputField>();
                triList.Add(new TriggerAfterUsing
                {
                    TriggerCondition = string.IsNullOrWhiteSpace(ipt_TriggerCondition.text) ? 0 : Convert.ToInt32(ipt_TriggerCondition.text),
                    TriggerState = string.IsNullOrWhiteSpace(ipt_TriggerState.text) ? 0 : Convert.ToInt32(ipt_TriggerState.text),
                    TriggerValue = string.IsNullOrWhiteSpace(ipt_TriggerValue.text) ? 0 : Convert.ToInt32(ipt_TriggerValue.text),
                    Sort = string.IsNullOrWhiteSpace(ipt_TriggerSort.text) ? 0 : Convert.ToInt32(ipt_TriggerSort.text)
                });
            }
        }
        TriggerCanvas.SetActive(false);
    }

    public void Save()
    {
        var list = Common.GetTxtFileToList<CardPoolModel>(GlobalAttr.GlobalCardPoolFileName) ?? new List<CardPoolModel>();
        CardPoolModel model = new CardPoolModel();
        model.ID = GetIncrementID();
        model.ParentID = string.IsNullOrWhiteSpace(ipt_ParentID.text) ? 0 : Convert.ToInt32(ipt_ParentID.text);
        model.CardDetail = ipt_CardDetail.text.Trim();
        model.CardName = ipt_CardName.text.Trim();
        model.PlayerOrAI = dd_PlayerOrAI.value;
        model.CardType = dd_CardType.value;
        model.CardUrl = ipt_CardUrl.text;
        model.Consume = string.IsNullOrWhiteSpace(ipt_Consume.text) ? 0 : Convert.ToInt32(ipt_Consume.text);
        model.Effect = string.IsNullOrWhiteSpace(ipt_Effect.text) ? 0 : Convert.ToInt64(ipt_Effect.text);
        model.CardLevel = string.IsNullOrWhiteSpace(ipt_CardLevel.text) ? 1 : Convert.ToInt32(ipt_CardLevel.text);
        model.CardPrice = string.IsNullOrWhiteSpace(ipt_CardPrice.text) ? 0 : Convert.ToInt64(ipt_CardPrice.text);
        model.EffectType = string.IsNullOrWhiteSpace(ipt_EffectType.text) ? 0 : Convert.ToInt32(ipt_EffectType.text);
        model.HasShoppingShow = dd_HasShoppingShow.value;
        model.AtkNumber = string.IsNullOrWhiteSpace(ipt_AtkNumber.text) ? 0 : Convert.ToInt32(ipt_AtkNumber.text);
        model.TriggerAfterUsingList = triList;
        list.Add(model);
        string json = list.ListToJson();

        Common.SaveTxtFile(json, GlobalAttr.GlobalCardPoolFileName);
        DataReset();
    }

    public void DataReset()
    {
        ipt_CardDetail.text = "";
        ipt_CardName.text = "";
        dd_PlayerOrAI.value = 0;
        dd_CardType.value = 0;
        ipt_CardUrl.text = "";
        ipt_Consume.text = "";
        ipt_Effect.text = "";
        ipt_EffectType.text = "";
        ipt_CardPrice.text = "0";
        ipt_CardLevel.text = "";
        dd_HasShoppingShow.value = 0;
        int count = TriggerCanvas.transform.childCount;
        if (count > 2)
        {
            for (int i = 0; i < triggerCount; i++)
            {
                var obj = TriggerCanvas.transform.Find($"TriggerObj{i}")?.gameObject;
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
            }
        }
        triggerCount = 0;
        triList = new List<TriggerAfterUsing>();
    }


    public int GetIncrementID()
    {
        int result = 1001;
        var list = Common.GetTxtFileToList<CardPoolModel>(GlobalAttr.GlobalCardPoolFileName);
        if (list?.Count > 0)
        {
            result = list.Max(a => a.ID) + 1;
        }
        return result;
    }
}
