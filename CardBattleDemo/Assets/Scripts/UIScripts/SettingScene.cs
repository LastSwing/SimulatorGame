using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingScene : MonoBehaviour
{
    Button btn_GameOver, btn_Return;
    void Start()
    {
        btn_GameOver = transform.Find("Content/btn_GameOver").GetComponent<Button>();
        btn_GameOver.onClick.AddListener(GameOver);
        if (Common.HasAgain == 1)
        {
            btn_GameOver.transform.localScale = Vector3.zero;
        }
        btn_Return = transform.Find("Content/btn_Return").GetComponent<Button>();
        btn_Return.onClick.AddListener(ReturnScene);

    }

    public void GameOver()
    {
        Common.SceneJump("HomeScene");
    }

    public void ReturnScene()
    {
        Debug.Log(Common.ReturnName);
        Common.SceneJump(Common.ReturnName != null ? Common.ReturnName : "HomeScene");
    }
}
