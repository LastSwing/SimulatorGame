using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretClass
{

    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// 分类
    /// </summary>
    public string Classify { get; set; }
    /// <summary>
    /// 数值
    /// </summary>
    public float Numerical { get; set; }
    /// <summary>
    /// 增量
    /// </summary>
    public float Increment { get; set; }
    /// <summary>
    /// 等级
    /// </summary>
    public int Lv { get; set; }
    /// <summary>
    /// 唯一Key
    /// </summary>
    public int Key { get; set; }
    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="name">名字</param>
    /// <param name="value">描述</param>
    /// <param name="classify">分类</param>
    /// <param name="numerical">数值</param>
    /// <param name="increment">增量</param>
    /// <param name="lv">等级</param>
    /// 
    public SecretClass(string name, string value, string classify, float numerical, float increment, int lv, int key)
    {
        Name = name;
        Value = value;
        Classify = classify;
        Numerical = numerical;
        Increment = increment;
        Lv = lv;
        Key = key;
    }

}
