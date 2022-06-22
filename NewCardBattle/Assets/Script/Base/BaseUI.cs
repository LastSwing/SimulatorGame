using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    /// <summary>
    /// 初始化，只在最初生成时调用一次
    /// </summary>
    public abstract void OnInit();

    /// <summary>
    /// 页面打开一次调用一次
    /// </summary>
    public abstract void OnOpen();

    /// <summary>
    /// 关掉时调用
    /// </summary>
    public abstract void OnClose();
    
}
