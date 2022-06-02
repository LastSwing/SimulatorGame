using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDieScene : MonoBehaviour
{
    Button btn_Home, btn_Map;
    void Start()
    {
        btn_Home = transform.Find("btn_Home").GetComponent<Button>();
        btn_Home.onClick.AddListener(HomeClick);
        btn_Map = transform.Find("btn_Map").GetComponent<Button>();
        btn_Map.onClick.AddListener(HomeClick);
    }


    public void HomeClick()
    {
        Common.SceneJump("HomeScene");
    }

    public void MapClick()
    {
        Common.SceneJump("MapScene", 99);
    }
}
