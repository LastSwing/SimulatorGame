using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 星球属性
/// </summary>
public class Star
{
    /// <summary>
    /// 星球名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 经济
    /// </summary>
    public int Money { get; set; }
    /// <summary>
    /// 武力
    /// </summary>
    public int Force { get; set; }
    /// <summary>
    /// 产值
    /// </summary>
    public int Output { get; set; }
    /// <summary>
    /// 简介
    /// </summary>
    public string Intro { get; set; }
    /// <summary>
    /// 配图
    /// </summary>
    public string Image { get; set; }
    /// <summary>
    /// 所属星系
    /// </summary>
    public string Galaxy { get; set; }
}
/// <summary>
/// 星系
/// </summary>
public class Galaxy
{
    /// <summary>
    /// 星系名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 所辖星球
    /// </summary>
    public string[] subordinate { get; set; }
    /// <summary>
    /// 简介
    /// </summary>
    public string Intro { get; set; }
    
}
