﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 游戏入口
/// </summary>
public class GameStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ResourcesManager.instance.Init();
        UIManager.instance.Init();
        //UIManager.instance.OpenView("MainView");
        UIManager.instance.OpenView("SettingView");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
