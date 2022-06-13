using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerDieScene : MonoBehaviour
{
    Button btn_Home, btn_Map, btn_Return, btn_GameOver;
    GameObject Setting_Obj;
    void Start()
    {
        btn_Home = transform.Find("btn_Home").GetComponent<Button>();
        btn_Home.onClick.AddListener(HomeClick);
        btn_Map = transform.Find("btn_Map").GetComponent<Button>();
        btn_Map.onClick.AddListener(HomeClick);
        Setting_Obj = transform.Find("TopBar/Setting").gameObject;

        #region 绑定点击设置

        btn_Return = GameObject.Find("SettingCanvas/Content/btn_Return").GetComponent<Button>();
        btn_Return.onClick.AddListener(ReturnScene);
        btn_GameOver = GameObject.Find("SettingCanvas/Content/btn_GameOver").GetComponent<Button>();
        btn_GameOver.transform.localScale = Vector3.zero;
        EventTrigger trigger = Setting_Obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = Setting_Obj.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener(delegate { ClickSetting(); });
        trigger.triggers.Add(entry);
        #endregion

        Common.GameOverDataReset();
    }


    public void HomeClick()
    {
        Common.SceneJump("HomeScene");
    }

    public void MapClick()
    {
        Common.SceneJump("MapScene", 99);
    }

    public void ClickSetting()
    {
        //Common.SceneJump("SettingScene", 1, "HomeScene");
        transform.gameObject.SetActive(false);
    }
    public void ReturnScene()
    {
        transform.gameObject.SetActive(true);
    }
}
