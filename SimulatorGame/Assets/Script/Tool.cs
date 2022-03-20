using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tool
{
    public static void SetTransChild(Transform root,int num)
    {
        int childNum = root.childCount;
        if (childNum == 0 || num == 0)
            return;
        GameObject item = root.GetChild(0).gameObject;
        if (childNum < num)
        {
            for(int i = 0; i < num - childNum; i++)
            {
                GameObject.Instantiate(item,root);
            }
        }
        for (int i = 0; i < root.childCount; i++)
        {
            root.GetChild(i).gameObject.SetActive(i < num);
        }
    }
}
