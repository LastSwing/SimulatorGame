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

    /// <summary>
    /// 事件读档
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Livelibrary JsonToLivelibrary(string Name, string path)
    {
        Livelibrary livelibrary = new Livelibrary();

        Dictionary<string, string> dict = new Dictionary<string, string>();
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists(path + @"\" + Name))
        {
            using (StreamReader tmpReader = File.OpenText(path + @"\" + Name))
            {
                string result = tmpReader.ReadToEnd();
                JsonData json = JsonMapper.ToObject(result);
                livelibrary.Name = json["Name"].ToString();
                livelibrary.Lifetime = json["Lifetime"].ToString();
                livelibrary.Role = json["Role"].ToString();
                livelibrary.JoinName = json["JoinName"].ToString();
                livelibrary.YearDuration = (int)json["YearDuration"];
                livelibrary.YearJoin = (int)json["YearJoin"];
                livelibrary.Year = (int)json["Year"];
                livelibrary.Dialogue = new Dictionary<Guid, Dictionary<Guid, string>>();
                JsonData jsonData = json["Dialogue"];
                for (int i = 0; i < jsonData.Count; i++)
                {
                    Dictionary<Guid, string> keyValues = new Dictionary<Guid, string>();
                    JsonData jsonData1 = jsonData[i.ToString()];
                    for (int j = 0; j < jsonData1.Count; j++)
                    {
                        try
                        {
                            keyValues.Add(new Guid(jsonData1[j.ToString()]["CGuid"].ToString()), jsonData1[j.ToString()]["Dialogue"].ToString());
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    livelibrary.Dialogue.Add(new Guid(jsonData[i.ToString()]["PGuid"].ToString()), keyValues);
                }
                livelibrary.Ending = new Dictionary<string, List<Ending>>();
                JsonData Endingdata = json["Ending"];
                for (int i = 0; i < Endingdata.Count; i++)
                {
                    List<Ending> endings = new List<Ending>();
                    JsonData jsonData1 = Endingdata[i.ToString()];
                    for (int j = 0; j < jsonData1.Count; j++)
                    {
                        try
                        {
                            Ending ending = new Ending();
                            ending.Stars = (int)jsonData1[j.ToString()]["Stars"];
                            ending.PGuid = new Guid(jsonData1[j.ToString()]["PGuid"].ToString());
                            ending.CGuid = new Guid(jsonData1[j.ToString()]["CGuid"].ToString());
                            ending.Stellar = (int)jsonData1[j.ToString()]["Stellar"];
                            ending.Productivity = (int)jsonData1[j.ToString()]["Productivity"];
                            ending.Vintage = (int)jsonData1[j.ToString()]["Vintage"];
                            ending.Result = jsonData1[j.ToString()]["Result"].ToString();
                            endings.Add(ending);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    livelibrary.Ending.Add(jsonData1["Result"].ToString() + "|" + jsonData1["Guid"].ToString(), endings);
                }
                livelibrary.Fileid = new List<Fileid>();
                JsonData Fileiddata = json["Fileid"];
                for (int i = 0; i < Fileiddata.Count; i++)
                {
                    Fileid fileid = new Fileid();
                    fileid.Id = new Guid(Fileiddata[i.ToString()]["Id"].ToString());
                    fileid.ParentId = new Guid(Fileiddata[i.ToString()]["ParentId"].ToString());
                    FileidType fileidType = new FileidType();
                    int type = (int)Fileiddata[i.ToString()]["Fileidtype"];
                    if (type == 0)
                        fileidType = FileidType.选择;
                    else if (type == 1)
                        fileidType = FileidType.字段;
                    else if (type == 2)
                        fileidType = FileidType.结局;
                    else if (type == 3)
                        fileidType = FileidType.背景音乐;
                    else if (type == 4)
                        fileidType = FileidType.动画;
                    else if (type == 6)
                        fileidType = FileidType.判断对话;
                    fileid.Fileidtype = fileidType;
                    fileid.InsertByte = (int)Fileiddata[i.ToString()]["InsertByte"];
                    fileid.EndByte = (int)Fileiddata[i.ToString()]["EndByte"];
                    fileid.Fileids = Fileiddata[i.ToString()]["Fileids"].ToString().Split('|');
                    fileid.PathName = Fileiddata[i.ToString()]["PathName"].ToString();
                    livelibrary.Fileid.Add(fileid);
                }
            }
        }
        return livelibrary;
    }

    /// <summary>
    /// 事件存档
    /// </summary>
    /// <param name="livelibrary"></param>
    /// <param name="path"></param>
    /// <param name="Name"></param>
    public static void LivelibraryTojson(Livelibrary livelibrary, string path, string Name)
    {
        JsonData jd = new JsonData();
        jd["Name"] = livelibrary.Name;
        jd["Lifetime"] = livelibrary.Lifetime;
        jd["Role"] = livelibrary.Role;
        jd["JoinName"] = livelibrary.JoinName;
        jd["YearDuration"] = livelibrary.YearDuration;
        jd["YearJoin"] = livelibrary.YearJoin;
        jd["Year"] = livelibrary.Year;
        jd["Dialogue"] = new JsonData();
        int q = 0; int w = 0; int e = 0; int r = 0;
        if (livelibrary.Dialogue.Count > 0)
        {
            foreach (var Dialogue in livelibrary.Dialogue)
            {
                jd["Dialogue"][q.ToString()] = new JsonData();
                jd["Dialogue"][q.ToString()]["PGuid"] = Dialogue.Key.ToString();
                w = 0;
                foreach (var item in Dialogue.Value)
                {
                    jd["Dialogue"][q.ToString()][w.ToString()] = new JsonData();
                    jd["Dialogue"][q.ToString()][w.ToString()]["CGuid"] = item.Key.ToString();
                    jd["Dialogue"][q.ToString()][w.ToString()]["Dialogue"] = item.Value.ToString();
                    w++;
                }
                q++;
            }
        }
        if (livelibrary.Ending.Count > 0)
        {
            jd["Ending"] = new JsonData();
            foreach (var Ending in livelibrary.Ending)
            {
                jd["Ending"][e.ToString()] = new JsonData();
                string[] a = Ending.Key.Split('|');
                jd["Ending"][e.ToString()]["Result"] = a[0];
                jd["Ending"][e.ToString()]["Guid"] = a[1];
                w = 0;
                foreach (var item in Ending.Value)
                {
                    jd["Ending"][e.ToString()][w.ToString()] = new JsonData();
                    jd["Ending"][e.ToString()][w.ToString()]["PGuid"] = item.PGuid.ToString();
                    jd["Ending"][e.ToString()][w.ToString()]["CGuid"] = item.CGuid.ToString();
                    jd["Ending"][e.ToString()][w.ToString()]["Stellar"] = (int)item.Stellar;
                    jd["Ending"][e.ToString()][w.ToString()]["Stars"] = (int)item.Stars;
                    jd["Ending"][e.ToString()][w.ToString()]["Productivity"] = (int)item.Productivity;
                    jd["Ending"][e.ToString()][w.ToString()]["Vintage"] = (int)item.Vintage;
                    jd["Ending"][e.ToString()][w.ToString()]["Result"] = item.Result.ToString();
                    w++;
                }
                e++;
            }
        }
        if (livelibrary.Fileid.Count > 0)
        {
            jd["Fileid"] = new JsonData();
            foreach (var item in livelibrary.Fileid)
            {
                jd["Fileid"][r.ToString()] = new JsonData();
                jd["Fileid"][r.ToString()]["Id"] = item.Id.ToString();
                jd["Fileid"][r.ToString()]["ParentId"] = item.ParentId.ToString();
                jd["Fileid"][r.ToString()]["Fileidtype"] = (int)item.Fileidtype;
                jd["Fileid"][r.ToString()]["InsertByte"] = (int)item.InsertByte;
                jd["Fileid"][r.ToString()]["EndByte"] = (int)item.EndByte;
                string Fileids = "";
                if (item.Fileids != null)
                {
                    foreach (var items in item.Fileids)
                    {
                        Fileids += items + "|";
                    }
                    if (Fileids.Length > 1)
                        Fileids = Fileids.Remove(Fileids.Length - 1, 1);
                }
                jd["Fileid"][r.ToString()]["Fileids"] = Fileids;
                jd["Fileid"][r.ToString()]["PathName"] = item.PathName.ToString();
                r++;
            }
        }
        string json = jd.ToJson();
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists($"{path}/{Name}.txt"))
            File.Delete($"{path}/{Name}.txt");
        File.WriteAllText($"{path}/{Name}.txt", json);
    }

    /// <summary>
    /// 获取事件列表
    /// </summary>
    /// <returns></returns>
    public static List<Event> JsonToEvent()
    {
        string path = Application.dataPath + "/Data/sortjson.txt";
        List<Event> events = new List<Event>();
        if (File.Exists(path))
        {
            using (StreamReader tmpReader = File.OpenText(path))
            {
                string result = tmpReader.ReadToEnd();
                JsonData json = JsonMapper.ToObject(result);
                for (int i = 0; i < json.Count; i++)
                {
                    Event event1 = new Event();
                    event1.EventName = json[i]["Enevt"].ToString();
                    event1.EventPath = json[i]["PathName"].ToString();
                    event1.Year = (int)json[i]["Year"];
                    events.Add(event1);
                }
            }
        }
        return events;
    }

    public static List<Galaxy> JsonToGalaxy()
    { 
        List<Galaxy> galaxy = new List<Galaxy>();
        string path = Application.dataPath + "/BackData/Galaxy.txt";
        if (File.Exists(path))
        {
            using (StreamReader tmpReader = File.OpenText(path))
            {
                string result = tmpReader.ReadToEnd();
                JsonData json = JsonMapper.ToObject(result);
                for (int i = 0; i < json.Count; i++)
                {
                    Galaxy galaxy1 = new Galaxy();
                    galaxy1.subordinate = json[i]["subordinate"].ToString().Split('|');
                    galaxy1.Name = json[i]["galaxy"].ToString();
                    galaxy1.Intro = json[i]["intro"].ToString();
                    galaxy.Add(galaxy1);
                }
            }
        }
        return galaxy;
    }

    public static List<Star> JsonToStar()
    {
        List<Star> star = new List<Star>();
        string path = Application.dataPath + "/BackData/Star.txt";
        if (File.Exists(path))
        {
            using (StreamReader tmpReader = File.OpenText(path))
            {
                string result = tmpReader.ReadToEnd();
                JsonData json = JsonMapper.ToObject(result);
                for (int i = 0; i < json.Count; i++)
                {
                    Star galaxy1 = new Star();
                    galaxy1.Name = json[i]["Name"].ToString();
                    galaxy1.Output = (int)json[i]["Output"];
                    galaxy1.Money = (int)json[i]["Money"];
                    galaxy1.Force = (int)json[i]["Force"];
                    galaxy1.Image = json[i]["Image"].ToString();
                    galaxy1.Galaxy = json[i]["Galaxy"].ToString();
                    galaxy1.Intro = json[i]["Intro"].ToString();
                    star.Add(galaxy1);
                }
            }
        }
        return star;
    }
}
