using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BaseHelper
{
    /// <summary>
    /// UI自动生成控件    根据y值
    /// </summary>
    /// <param name="y">初始y值</param>
    /// <param name="number">物品个数</param>
    /// <returns></returns>
    public static List<Vector2> Meanlocation(int y, int number)
    {
        List<Vector2> siz = new List<Vector2>();
        
        return siz;
    }
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
