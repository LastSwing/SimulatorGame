using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonHelper
{
    /// <summary>
    /// 数据组装 成JSON
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="isLast"></param>
    /// <returns></returns>
    public static string WriteItem(string key, string value, bool isLast = false)
    {
        string s = "\"" + key + "\":" + "\"" + value + "\"";
        if (!isLast)
            s += ",";
        return s;
    }
    /// <summary>
    /// 存档
    /// </summary>
    /// <param name="dic">属性，值</param>
    /// <param name="Name">文件名</param>
    /// <param name="FolderName">文件夹名称</param>
    public static void DataExport(Dictionary<string, string> dic, string Name)
    {
        var id = $"{System.DateTime.Now.ToString("yyyyMMddHHmmssff")}";
        string json = "{";
        int j = 0;
        foreach (var item in dic)
        {
            j++;
            if (dic.Count == j)
                json += WriteItem(item.Key, item.Value, true);
            else
                json += WriteItem(item.Key, item.Value);
        }
        json += "}";
        Debug.Log(json);
        //json = DesEncrypt(json);
        var path = Application.dataPath + "/Data/";
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists($"{path}/{Name}.txt"))
            File.Delete($"{path}/{Name}.txt");
        File.WriteAllText($"{path}/{Name}.txt", json);
        //Debug.Log($"导出成功：{path}/{Name}.txt");
    }

    /// <summary>
    /// 读档
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    public static Dictionary<string, string> DataRead(string Name)
    {
        var path = Application.dataPath + "/Data/";
        Dictionary<string, string> dict = new Dictionary<string, string>();
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists(path + @"\" + Name))
        {
            StreamReader json = File.OpenText(path + @"\" + Name);
            //Debug.Log("读档" + json);
            string input = json.ReadToEnd();
            JsonReader reader = new JsonReader(input);
            string temp = string.Empty;
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    switch (reader.Token)
                    {
                        case JsonToken.PropertyName:
                            dict.Add(reader.Value.ToString(), string.Empty);
                            temp = reader.Value.ToString();
                            break;
                        default:
                            dict[temp] = reader.Value.ToString();
                            break;
                    }
                    Console.WriteLine(reader.Token + "\t" + reader.Value);
                }
            }
            json.Close();
        }
        return dict;
    }
}
