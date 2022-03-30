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
    public Dictionary<string, string> Blood(string BloodName)
    {
        Dictionary<string, string> dict = GameHelper.DataRead("Blood.txt");
        if (dict.Count == 0)
        {
            dict.Add("女娲血脉","0");
            dict.Add("盘古血脉", "0");
            dict.Add("夸父血脉", "0");
            dict.Add("伏羲血脉", "0");
            dict.Add("共工血脉", "0");
            dict.Add("蚩尤血脉", "0");
        }
        if(!string.IsNullOrEmpty(BloodName))
            dict[BloodName] = Convert.ToString(Convert.ToInt32(dict[BloodName]) + 1);
        GameHelper.DataExport(dict, "Blood.txt");
        return dict;
    }
}
