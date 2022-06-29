using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class BaseHelper
{
    /// <summary>
    /// 将控件添加到父控件
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public static GameObject AddChild(Transform parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.SetParent(parent, false);
            go.layer = parent.gameObject.layer;
        }
        return go;
    }
}
