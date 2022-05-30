using Assets.Scripts.LogicalScripts.Models;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBar : MonoBehaviour
{
    Image Player_HeadPortrait, HP;
    GameObject Energy_obj;
    Text txt_Silver, txt_HP;
    CurrentRoleModel PlayerRole;
    void Start()
    {

        Player_HeadPortrait = transform.Find("Player_HeadPortrait").GetComponent<Image>();
        HP = transform.Find("HP/img_HP").GetComponent<Image>();

        txt_Silver = transform.Find("img_Silver/txt_Silver").GetComponent<Text>();
        txt_HP = transform.Find("HP/Text").GetComponent<Text>();

        Energy_obj = transform.Find("Energy").gameObject;
        Init();
    }

    void Init()
    {
        PlayerRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName);

        Common.ImageBind(PlayerRole.HeadPortraitUrl, Player_HeadPortrait);
        CreateEnergyImage(PlayerRole.MaxEnergy);
        txt_HP.text = $"{PlayerRole.MaxHP}/{PlayerRole.HP}";

        txt_Silver.text = PlayerRole.Wealth.ToString();
        Common.HPImageChange(HP, PlayerRole.MaxHP, PlayerRole.MaxHP - PlayerRole.HP, 0, 150);
    }
    /// <summary>
    /// 创建能量图片
    /// </summary>
    public void CreateEnergyImage(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject tempObject = Resources.Load("Prefab/img_Energy") as GameObject;
            tempObject = Common.AddChild(Energy_obj.transform, tempObject);
            tempObject.name = "img_Energy" + i;
        }
    }
}
