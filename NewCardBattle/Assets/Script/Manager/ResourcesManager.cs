using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : SingletonMonoBehaviour<ResourcesManager>
{
    /// <summary>
    /// 资源路径字典，初始化的时候将所有动态资源的路径全部获取到这里
    /// </summary>
    /// 最好走配表的方式，这里先直接填充
    Dictionary<string, string> urlDic = new Dictionary<string, string>() {
        { "MainView","View/MainView" },
        {"SettingView","View/SettingView" },
        {"CardPoolsView","View/CardPoolsView" },
        {"AdventureView","View/AdventureView" },
        {"AiDieView","View/AiDieView" },
        {"AllSkillView","View/AllSkillView" },
        {"PlayerDieView","View/PlayerDieView" }
    };

    /// <summary>
    /// 预加载字典,弹窗类的，UI类的资源提前load放入此字典
    /// 一般大资源比如UI页面
    /// </summary>
    Dictionary<string, object> resourcesDic = new Dictionary<string, object>();


    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {

    }

    /// <summary>
    /// 检查资源是否存在
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool CheckResourcesExist(string name)
    {
        return urlDic.ContainsKey(name);
    }

    /// <summary>
    /// 提前预加载
    /// </summary>
    /// <param name="dicName"></param>
    public void SetToResourcesDic(string dicName)
    {
        resourcesDic[dicName] = Resources.Load(urlDic[dicName]);
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="dicName"></param>
    /// <returns></returns>
    public object Load(string dicName)
    {
        if (resourcesDic.ContainsKey(dicName))
        {
            return resourcesDic[dicName];
        }
        else if (urlDic.ContainsKey(dicName))
        {
            return Resources.Load(urlDic[dicName]);
        }
        else
        {
            return null;
        }
    }
}
