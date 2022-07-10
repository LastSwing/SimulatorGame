using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleScene : MonoBehaviour
{
    private Button btn_CardPools, btn_Save;
    private Dropdown dd_CardSelect, dd_RoleType;
    private InputField ipt_CardPools, ipt_RoleName, ipt_RoleImgUrl, ipt_HP, ipt_Wealth, ipt_Energy, ipt_AILevel, ipt_HeadUrl;
    private List<CardPoolModel> Cardlist;
    // Start is called before the first frame update
    void Start()
    {
        #region 下拉框初始化
        dd_CardSelect = transform.Find("dd_CardSelect").GetComponent<Dropdown>();
        Cardlist = Common.GetTxtFileToList<CardPoolModel>(GlobalAttr.GlobalPlayerCardPoolFileName) ?? new List<CardPoolModel>();
        CardPoolChange(0);
        dd_CardSelect.onValueChanged.AddListener(delegate { SelectChange(); });
        #endregion

        #region 控件初始化
        dd_RoleType = transform.Find("dd_RoleType").GetComponent<Dropdown>();
        dd_RoleType.onValueChanged.AddListener(delegate { CardPoolChange(dd_RoleType.value); });

        ipt_CardPools = transform.Find("ipt_CardPools").GetComponent<InputField>();
        ipt_RoleName = transform.Find("ipt_RoleName").GetComponent<InputField>();
        ipt_RoleImgUrl = transform.Find("ipt_RoleImgUrl").GetComponent<InputField>();
        ipt_HP = transform.Find("ipt_HP").GetComponent<InputField>();
        ipt_Wealth = transform.Find("ipt_Wealth").GetComponent<InputField>();
        ipt_Energy = transform.Find("ipt_Energy").GetComponent<InputField>();
        ipt_AILevel = transform.Find("ipt_AILevel").GetComponent<InputField>();
        ipt_HeadUrl = transform.Find("ipt_HeadUrl").GetComponent<InputField>();

        btn_CardPools = transform.Find("PageJump/btn_CardPools").GetComponent<Button>();
        btn_CardPools.onClick.AddListener(delegate { Common.SceneJump("PlayerCardScene"); });
        btn_Save = transform.Find("btn_Save").GetComponent<Button>();
        btn_Save.onClick.AddListener(Save);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 卡牌选择
    /// </summary>
    public void SelectChange()
    {
        var select = dd_CardSelect.captionText.text;
        ipt_CardPools.text += $"{select}\t";
        dd_CardSelect.value = 0;
    }

    public void Save()
    {
        List<CurrentRoleModel> list = null;
        if (dd_RoleType.value == 0)
        {
            list = Common.GetTxtFileToList<CurrentRoleModel>(GlobalAttr.GlobalPlayerRolePoolFileName) ?? new List<CurrentRoleModel>();
        }
        else if (dd_RoleType.value == 1)
        {
            list = Common.GetTxtFileToList<CurrentRoleModel>(GlobalAttr.GlobalAIRolePoolFileName) ?? new List<CurrentRoleModel>();
        }
        CurrentRoleModel model = new CurrentRoleModel();
        model.RoleID = DateTime.Now.ToString("yyyyMMddHHmmssff");
        model.Name = ipt_RoleName.text.Trim();
        model.RoleType = dd_RoleType.value;
        model.RoleImgUrl = ipt_RoleImgUrl.text;
        model.HP = string.IsNullOrWhiteSpace(ipt_HP.text) ? 0 : Convert.ToInt64(ipt_HP.text);
        model.MaxHP = string.IsNullOrWhiteSpace(ipt_HP.text) ? 0 : Convert.ToInt64(ipt_HP.text);
        model.Wealth = string.IsNullOrWhiteSpace(ipt_Wealth.text) ? 0 : Convert.ToInt64(ipt_Wealth.text);
        model.Energy = string.IsNullOrWhiteSpace(ipt_Energy.text) ? 0 : Convert.ToInt32(ipt_Energy.text);
        model.MaxEnergy = string.IsNullOrWhiteSpace(ipt_Energy.text) ? 0 : Convert.ToInt32(ipt_Energy.text);
        model.CardListStr = ipt_CardPools.text.Trim().Replace("\t", "");
        model.AILevel = string.IsNullOrWhiteSpace(ipt_AILevel.text) ? 0 : Convert.ToInt32(ipt_AILevel.text);
        model.HeadPortraitUrl = ipt_HeadUrl.text.Trim().ToString();

        list.Add(model);

        string json = list.ListToJson();
        var aa = json.JsonToList<CurrentRoleModel>();
        if (dd_RoleType.value == 0)
        {
            Common.SaveTxtFile(json, GlobalAttr.GlobalPlayerRolePoolFileName);
        }
        else if (dd_RoleType.value == 1)
        {
            Common.SaveTxtFile(json, GlobalAttr.GlobalAIRolePoolFileName);
        }
        ResetData();
    }

    public void ResetData()
    {
        dd_RoleType.value = 0;
        ipt_CardPools.text = "";
        ipt_RoleName.text = "";
        ipt_RoleImgUrl.text = "";
        ipt_RoleImgUrl.text = "";
        ipt_HP.text = "";
        ipt_Wealth.text = "";
        ipt_Energy.text = "";
        ipt_AILevel.text = "";
    }

    /// <summary>
    /// 牌池变化
    /// </summary>
    public void CardPoolChange(int index)
    {
        var newList = new List<CardPoolModel>();
        dd_CardSelect.ClearOptions();
        if (index == 0)
        {
            newList = Cardlist.FindAll(a => a.CardLevel == 0);//玩家初始牌池
        }
        else if (index == 1)
        {
            newList = Cardlist.FindAll(a => a.PlayerOrAI == 1);//AI初始牌池
        }
        dd_CardSelect.options.Add(new Dropdown.OptionData { text = "" });
        foreach (var item in newList)
        {
            Dropdown.OptionData optionData = new Dropdown.OptionData();
            optionData.text = $"{item.ID}|{item.CardName};";
            dd_CardSelect.options.Add(optionData);

        }
    }

}
