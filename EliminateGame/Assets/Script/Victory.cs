﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Victory : MonoBehaviour
{
    public Button Victorybtn;
    public Button Remove;
    // Start is called before the first frame update
    void Start()
    {
        Victorybtn.onClick.AddListener(VbtnClick);
        Remove.onClick.AddListener(RbtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void VbtnClick()
    { 
        gameObject.SetActive(false);
        EmptySubclass(transform.parent.Find("Bg").gameObject);
        transform.parent.Find("Bg").GetComponent<BgScript>().LoadLevel(transform.parent.Find("Bg").GetComponent<BgScript>().LevelNum + 1);
    }
    void RbtnClick()
    { 
        
    }
    //tempObj：父物体
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
