using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
/// <summary>
/// 主角基础属性值
/// </summary>
public class Attributes
{
    /// <summary>
    /// 始神血脉加载更新
    /// </summary>
    /// <param name="BloodName">需要提升的始神血脉名称</param>
    /// <returns></returns>
    public static Dictionary<string, string> Blood(string BloodName)
    {
        Dictionary<string, string> dict = GameHelper.DataRead("Blood");
        if (dict.Count == 0)
        {
            dict.Add("女娲血脉", "0");
            dict.Add("盘古血脉", "0");
            dict.Add("夸父血脉", "0");
            dict.Add("伏羲血脉", "0");
            dict.Add("共工血脉", "0");
            dict.Add("蚩尤血脉", "0");
        }
        if (!string.IsNullOrEmpty(BloodName))
            dict[BloodName] = Convert.ToString(Convert.ToInt32(dict[BloodName]) + 1);
        GameHelper.DataExport(dict, "Blood.txt");
        return dict;
    }
    /// <summary>
    /// 窍穴加载更新
    /// </summary>
    /// <param name="BloodName">需要提升的始神血脉名称</param>
    /// <param name="o">0增加 1减少</param>
    /// <returns></returns>
    public static Dictionary<string, string> Opening(string BloodName, int o)
    {
        Dictionary<string, string> dict = GameHelper.DataRead("Opening");
        if (dict.Count == 0)
        {
            dict.Add("神庭穴", "0");
            dict.Add("紫宫穴", "0");
            dict.Add("鸠尾穴", "0");
            dict.Add("气冲穴", "0");
            dict.Add("关元穴", "0");
            dict.Add("中枢穴", "0");
        }
        if (!string.IsNullOrEmpty(BloodName) && o == 0)
            dict[BloodName] = Convert.ToString(Convert.ToInt32(dict[BloodName]) + 1);
        else if (!string.IsNullOrEmpty(BloodName) && o == 1)
            dict[BloodName] = Convert.ToString(Convert.ToInt32(dict[BloodName]) - 1);
        GameHelper.DataExport(dict, "Opening.txt");
        return dict;
    }
}
