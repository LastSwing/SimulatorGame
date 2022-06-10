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

    }

    public void GameOver()
    {        
        Common.SceneJump("PlayerDieScene");
    }

}
