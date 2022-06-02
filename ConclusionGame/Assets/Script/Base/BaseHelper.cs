using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BaseHelper
{
    /// <summary>
    /// UI自动生成控件Size    根据中心点
    /// </summary>
    /// <param name="screenwidth">屏幕宽度</param>
    /// <param name="width">物品宽度</param>
    /// <param name="number">物品个数</param>
    /// <returns></returns>
    public static List<Vector2> Meanlocation(int screenwidth, int width,int number)
    {
        List<Vector2> siz = new List<Vector2>();
        int q = number * width;
        if (screenwidth - q <= 0)
        {
            int interval = screenwidth/2 -width/2;
            for (int i = 0; i < number; i++)
            {
                siz.Add(new Vector2(interval-(width*i), 0));
            }
        }
        else if (screenwidth - q > 0)
        {
            int ter = screenwidth - q;//960
            int interval = (ter / (number+1))+ width / 2;
            int interval2 = (screenwidth / 2) - screenwidth;
            for (int i = 0; i < number; i++)
            {
                siz.Add(new Vector2(interval2 + (interval * (i+1))+(width/2*i), 0));
            }
        }
        
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

    /// <summary>
    /// 将字符串分割，每隔number添加回车
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="number">间隔</param>
    /// <returns></returns>
    public static string EnterStr(string str, int number)
    {
        if (str.Length <= number)
        {
            return str;
        }
        else
        {
            string str1 = "";
            for (int i = 0; i < str.Length; i += number)
            {
                if (str.Length - i > number)
                    str1 += str.Substring(i, number) + "\n";
                else
                    str1 += str.Substring(i);
            }
            return str1;
        }
    }

    /// <summary>
    /// 跳转页面
    /// </summary>
    /// <param name="SceneName">场景名称</param>
    /// <param name="HasAgain">是否重新开始1、不重开，0重开</param>
    public static void SceneJump(string SceneName, int Again = 1)
    {
        SceneManager.LoadScene(SceneName);
    }
}
