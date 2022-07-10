using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCardScene : MonoBehaviour
{
    private Button btn_Player, btn_Save;
    private Dropdown dd_CardType, dd_PlayerOrAI, dd_HasShoppingShow;
    private InputField ipt_CardName, ipt_CardUrl, ipt_Consume, ipt_Effect, ipt_CardDetail, ipt_CardLevel, ipt_CardPrice, ipt_TriggerValue, ipt_ParentID, ipt_EffectType, ipt_AtkNumber, ipt_TriggerState, ipt_TriggerCondition
        ,ipt_TriggerValue2,ipt_TriggerState2, ipt_TriggerCondition2;
    // Start is called before the first frame update
    void Start()
    {
        #region 数据初始化
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
        ipt_TriggerState = transform.Find("ipt_TriggerState").GetComponent<InputField>();
        ipt_TriggerValue = transform.Find("ipt_TriggerValue").GetComponent<InputField>();
        ipt_TriggerCondition = transform.Find("ipt_TriggerCondition").GetComponent<InputField>();
        ipt_TriggerState2 = transform.Find("ipt_TriggerState2").GetComponent<InputField>();
        ipt_TriggerValue2 = transform.Find("ipt_TriggerValue2").GetComponent<InputField>();
        ipt_TriggerCondition2 = transform.Find("ipt_TriggerCondition2").GetComponent<InputField>();

        btn_Player = transform.Find("PageJump/btn_Roles").GetComponent<Button>();
        btn_Player.onClick.AddListener(delegate { Common.SceneJump("RoleScene"); });
        btn_Save = transform.Find("btn_Save").GetComponent<Button>();
        btn_Save.onClick.AddListener(Save);

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
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
        model.TriggerState = string.IsNullOrWhiteSpace(ipt_TriggerState.text) ? 0 : Convert.ToInt32(ipt_TriggerState.text);
        model.TriggerValue = string.IsNullOrWhiteSpace(ipt_TriggerValue.text) ? 0 : Convert.ToInt32(ipt_TriggerValue.text);
        model.AtkNumber = string.IsNullOrWhiteSpace(ipt_AtkNumber.text) ? 0 : Convert.ToInt32(ipt_AtkNumber.text);
        model.TriggerCondition = string.IsNullOrWhiteSpace(ipt_TriggerCondition.text) ? 0 : Convert.ToInt32(ipt_TriggerCondition.text);
        model.TriggerState2 = string.IsNullOrWhiteSpace(ipt_TriggerState2.text) ? 0 : Convert.ToInt32(ipt_TriggerState2.text);
        model.TriggerValue2 = string.IsNullOrWhiteSpace(ipt_TriggerValue2.text) ? 0 : Convert.ToInt32(ipt_TriggerValue2.text);
        model.TriggerCondition2 = string.IsNullOrWhiteSpace(ipt_TriggerCondition2.text) ? 0 : Convert.ToInt32(ipt_TriggerCondition2.text);

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
        ipt_TriggerState.text = "";
        ipt_TriggerValue.text = "";
        ipt_TriggerCondition.text = "";
        ipt_TriggerState2.text = "";
        ipt_TriggerValue2.text = "";
        ipt_TriggerCondition2.text = "";
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
