﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lose : MonoBehaviour
{
    public Button Losebtn;
    public Button Remove;
    // Start is called before the first frame update
    void Start()
    {
        Losebtn.onClick.AddListener(LbtnClick);
        Remove.onClick.AddListener(RbtnClick);
    }
    void LbtnClick()
    {
        gameObject.SetActive(false);
        EmptySubclass(transform.parent.Find("Bg").gameObject);
        transform.parent.GetComponent<BgScript>().LoadLevel(transform.parent.GetComponent<BgScript>().LevelNum);
    }
    void RbtnClick()
    {
        gameObject.SetActive(false);
        EmptySubclass(transform.parent.Find("Bg").gameObject);
        UIManager.instance.CloseView("GamePage");
        UIManager.instance.OpenView("MainView");
    }

    void EmptySubclass(GameObject tempObj)
    {
        var count = tempObj.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            DestroyImmediate(tempObj.transform.GetChild(0).gameObject);//即时删除
            //Destroy(tempObj.transform.GetChild(0).gameObject);//延时删除
        }
    }
}
