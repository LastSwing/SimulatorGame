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
    private Button btn_Player, btn_Monster, btn_Save;
    private Dropdown dd_CardType, dd_EffectType, dd_CardScope, dd_HasAOE;
    private InputField ipt_CardName, ipt_CardUrl, ipt_Consume, ipt_Effect, ipt_CardDetail;
    // Start is called before the first frame update
    void Start()
    {
        #region 数据初始化
        dd_CardType = transform.Find("dd_CardType").GetComponent<Dropdown>();
        dd_EffectType = transform.Find("dd_EffectType").GetComponent<Dropdown>();
        dd_CardScope = transform.Find("dd_CardScope").GetComponent<Dropdown>();
        dd_HasAOE = transform.Find("dd_HasAOE").GetComponent<Dropdown>();

        ipt_CardName = transform.Find("ipt_CardName").GetComponent<InputField>();
        ipt_CardUrl = transform.Find("ipt_CardUrl").GetComponent<InputField>();
        ipt_Consume = transform.Find("ipt_Consume").GetComponent<InputField>();
        ipt_Effect = transform.Find("ipt_Effect").GetComponent<InputField>();
        ipt_CardDetail = transform.Find("ipt_CardDetail").GetComponent<InputField>();

        btn_Player = transform.Find("PageJump/btn_Player").GetComponent<Button>();
        btn_Player.onClick.AddListener(delegate { Common.SceneJump(""); });
        btn_Monster = transform.Find("PageJump/btn_Monster").GetComponent<Button>();
        btn_Monster.onClick.AddListener(delegate { Common.SceneJump(""); });
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
        PlayerCardModel model = new PlayerCardModel();
        model.ID = $"Card{DateTime.Now.ToString("yyyyMMddHHmmssff")}";
        model.CardDetail = ipt_CardDetail.text.Trim();
        model.CardName = ipt_CardName.text.Trim();
        model.CardScope = dd_CardScope.value;
        model.CardType = dd_CardType.value;
        model.CardUrl = ipt_CardUrl.text;
        model.Consume= string.IsNullOrWhiteSpace(ipt_Consume.text) ? 0 : Convert.ToInt64(ipt_Consume.text);
        model.HasAOE = dd_HasAOE.value;
        model.Effect = string.IsNullOrWhiteSpace(ipt_Effect.text) ? 0 : Convert.ToInt64(ipt_Effect.text);
        model.StateType = dd_EffectType.value;

        string json = Common.ObjectToJson(model);

        Common.SaveTxtFile(json, "GlobalCardPool.txt");
        DataReset();
    }

    public void DataReset()
    {
        ipt_CardDetail.text = "";
        ipt_CardName.text = "";
        dd_CardScope.value = 0;
        dd_CardType.value = 0;
        ipt_CardUrl.text = "";
        ipt_Consume.text = "";
        dd_HasAOE.value = 0;
        ipt_Effect.text = "";
        dd_EffectType.value = 0;
    }
}
