using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    Dictionary<string, BaseUI> UIDic = new Dictionary<string, BaseUI>();
    Transform UIRoot;

    internal void Init()
    {
        UIRoot = GameObject.Find("MainCanvas").transform;
    }

    /// <summary>
    /// 打开View
    /// </summary>
    /// <param name="name"></param>
    public void OpenView(string name)
    {
        if (!UIDic.ContainsKey(name))
        {
            if (ResourcesManager.instance.CheckResourcesExist(name))
            {
                GameObject view = Instantiate(ResourcesManager.instance.Load(name) as GameObject, UIRoot);
                UIDic.Add(name, view.GetComponent<BaseUI>());
                UIDic[name].gameObject.SetActive(true);
                UIDic[name].OnInit();
            }
            else
            {
                return;//如果资源并不存在，则直接中断
            }
        }
        UIDic[name].gameObject.SetActive(true);
        UIDic[name].OnOpen();
    }

    /// <summary>
    /// 关闭页面
    /// </summary>
    /// <param name="name"></param>
    public void CloseView(string name)
    {
        if (UIDic.ContainsKey(name))
        {
            UIDic[name].OnClose();
            UIDic[name].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 获取View
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public BaseUI GetView(string name)
    {
        if (UIDic.ContainsKey(name))
            return UIDic[name];
        else
            return null;
    }
}
