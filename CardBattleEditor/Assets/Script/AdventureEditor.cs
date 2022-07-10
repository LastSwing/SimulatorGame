using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureEditor : MonoBehaviour
{
    InputField inp_ID, inp_Name, inp_MainBGIUrl, inp_MainEvent, inp_EventEffectName, inp_EventEffectImgUrl, inp_EventEffectValue, inp_EventLayerLevel, inp_EventLayerLevelSort, inp_RelatedEventIDs;
    Dropdown dd_EventEffectType, dd_ReWardType;
    Button btn_Save;
    List<AdventureModel> list = new List<AdventureModel>();
    void Start()
    {
        #region 控件初始化

        inp_ID = transform.Find("inp_ID").GetComponent<InputField>();
        inp_Name = transform.Find("inp_Name").GetComponent<InputField>();
        inp_MainBGIUrl = transform.Find("inp_MainBGIUrl").GetComponent<InputField>();
        inp_MainEvent = transform.Find("inp_MainEvent").GetComponent<InputField>();
        inp_EventEffectName = transform.Find("inp_EventEffectName").GetComponent<InputField>();
        inp_EventEffectImgUrl = transform.Find("inp_EventEffectImgUrl").GetComponent<InputField>();
        inp_EventEffectValue = transform.Find("inp_EventEffectValue").GetComponent<InputField>();
        inp_EventLayerLevel = transform.Find("inp_EventLayerLevel").GetComponent<InputField>();
        inp_EventLayerLevelSort = transform.Find("inp_EventLayerLevelSort").GetComponent<InputField>();
        inp_RelatedEventIDs = transform.Find("inp_RelatedEventIDs").GetComponent<InputField>();

        dd_EventEffectType = transform.Find("dd_EventEffectType").GetComponent<Dropdown>();
        dd_ReWardType = transform.Find("dd_ReWardType").GetComponent<Dropdown>();

        btn_Save = transform.Find("btn_Save").GetComponent<Button>();
        btn_Save.onClick.AddListener(SaveData);

        #endregion

        #region 数据初始化
        list = Common.GetTxtFileToList<AdventureModel>(GlobalAttr.GlobalAdventureFileName, "Adventure") ?? new List<AdventureModel>();
        inp_ID.text = $"Adventure_{(list == null ? 0 : list?.Count)}";
        #endregion
    }

    public void SaveData()
    {
        AdventureModel model = new AdventureModel();
        model.ID = inp_ID.text.Trim();
        model.Name = inp_Name.text.Trim();
        model.MainBGIUrl = inp_MainBGIUrl.text.Trim();
        model.EventDetail = inp_MainEvent.text.Trim();
        model.EventEffectName = inp_EventEffectName.text.Trim();
        model.EventEffectImgUrl = inp_EventEffectImgUrl.text.Trim();
        model.EventEffectValue = string.IsNullOrEmpty(inp_EventEffectValue.text.Trim()) ? 0 : Convert.ToInt32(inp_EventEffectValue.text.Trim());
        model.EventLayerLevel = string.IsNullOrEmpty(inp_EventLayerLevel.text.Trim()) ? 0 : Convert.ToInt32(inp_EventLayerLevel.text.Trim());
        model.EventLayerLevelSort = string.IsNullOrEmpty(inp_EventLayerLevelSort.text.Trim()) ? 0 : Convert.ToInt32(inp_EventLayerLevelSort.text.Trim());
        model.RelatedEventIDs = inp_RelatedEventIDs.text;
        model.EventEffectType = dd_EventEffectType.value;
        model.RewardType = dd_ReWardType.value;
        list.Add(model);
        Common.SaveTxtFile(list.ListToJson(), GlobalAttr.GlobalAdventureFileName, "Adventure");
        DataReset();
    }

    public void DataReset()
    {
        inp_ID.text = $"Adventure_{(list == null ? 0 : list?.Count)}";
        inp_EventEffectName.text = "";
        inp_EventEffectImgUrl.text = "";
        inp_EventEffectValue.text = "";
        inp_EventLayerLevel.text = "";
        inp_EventLayerLevelSort.text = "";
        inp_RelatedEventIDs.text = "";

        dd_EventEffectType.value = 0;
        dd_ReWardType.value = 0;
    }

}
