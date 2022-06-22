using Assets.Scripts.LogicalScripts.Models;
using Assets.Scripts.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCardScene : MonoBehaviour
{
    private Button btn_Player, btn_Save;
    private Dropdown dd_CardType, dd_EffectType, dd_PlayerOrAI, dd_HasAOE, dd_HasShoppingShow, dd_HasDeBuff, dd_TriggerState;
    private InputField ipt_CardName, ipt_CardUrl, ipt_Consume, ipt_Effect, ipt_CardDetail, ipt_CardLevel, ipt_CardPrice, ipt_TriggerValue, ipt_AiAtkSort;
    // Start is called before the first frame update
    void Start()
    {
        #region 数据初始化
        dd_CardType = transform.Find("dd_CardType").GetComponent<Dropdown>();
        dd_EffectType = transform.Find("dd_EffectType").GetComponent<Dropdown>();
        dd_PlayerOrAI = transform.Find("dd_PlayerOrAI").GetComponent<Dropdown>();
        dd_HasAOE = transform.Find("dd_HasAOE").GetComponent<Dropdown>();
        dd_HasShoppingShow = transform.Find("dd_HasShoppingShow").GetComponent<Dropdown>();
        dd_HasDeBuff = transform.Find("dd_HasDeBuff").GetComponent<Dropdown>();
        dd_TriggerState = transform.Find("dd_TriggerState").GetComponent<Dropdown>();

        ipt_CardName = transform.Find("ipt_CardName").GetComponent<InputField>();
        ipt_CardUrl = transform.Find("ipt_CardUrl").GetComponent<InputField>();
        ipt_Consume = transform.Find("ipt_Consume").GetComponent<InputField>();
        ipt_Effect = transform.Find("ipt_Effect").GetComponent<InputField>();
        ipt_CardDetail = transform.Find("ipt_CardDetail").GetComponent<InputField>();
        ipt_CardLevel = transform.Find("ipt_CardLevel").GetComponent<InputField>();
        ipt_CardPrice = transform.Find("ipt_CardPrice").GetComponent<InputField>();
        ipt_TriggerValue = transform.Find("ipt_TriggerValue").GetComponent<InputField>();
        ipt_AiAtkSort = transform.Find("ipt_AiAtkSort").GetComponent<InputField>();

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
        var list = Common.GetTxtFileToList<CardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName) ?? new List<CardPoolModel>();
        CardPoolModel model = new CardPoolModel();
        model.ID = $"{DateTime.Now.ToString("yyyyMMddHHmmssff")}";
        model.CardDetail = ipt_CardDetail.text.Trim();
        model.CardName = ipt_CardName.text.Trim();
        model.PlayerOrAI = dd_PlayerOrAI.value;
        model.CardType = dd_CardType.value;
        model.CardUrl = ipt_CardUrl.text;
        model.Consume = string.IsNullOrWhiteSpace(ipt_Consume.text) ? 0 : Convert.ToInt32(ipt_Consume.text);
        model.HasAOE = dd_HasAOE.value;
        model.Effect = string.IsNullOrWhiteSpace(ipt_Effect.text) ? 0 : Convert.ToInt64(ipt_Effect.text);
        model.CardLevel = string.IsNullOrWhiteSpace(ipt_CardLevel.text) ? 1 : Convert.ToInt32(ipt_CardLevel.text);
        model.CardPrice = string.IsNullOrWhiteSpace(ipt_CardPrice.text) ? 0 : Convert.ToInt64(ipt_CardPrice.text);
        model.StateType = dd_EffectType.value;
        model.HasShoppingShow = dd_HasShoppingShow.value;
        model.HasDeBuff = dd_HasDeBuff.value;
        model.TriggerState = dd_TriggerState.value;
        model.TriggerValue = string.IsNullOrWhiteSpace(ipt_TriggerValue.text) ? 1 : Convert.ToInt32(ipt_TriggerValue.text);

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
        dd_HasAOE.value = 0;
        ipt_Effect.text = "";
        dd_EffectType.value = 0;
        ipt_CardPrice.text = "0";
        ipt_CardLevel.text = "1";
        dd_HasDeBuff.value = 0;
        dd_HasShoppingShow.value = 0;
        dd_TriggerState.value = 0;
        ipt_TriggerValue.text = "";
    }
}
