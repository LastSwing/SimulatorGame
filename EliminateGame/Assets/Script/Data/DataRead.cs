using LitJson;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataRead
{
    /// <summary>
    /// 返回关卡数据
    /// </summary>
    /// <param name="LevelNum"></param>
    /// <returns></returns>
    public static Level GetSeed(int LevelNum)
    {
        TextAsset itemText = Resources.Load<TextAsset>("LevelData/Level" + LevelNum);
        Level level = new Level();
        string result = itemText.text;
        JsonData json = JsonMapper.ToObject(result);
        level.LevelNum = (int)json["LevelNum"];
        level.LevelType = (int)json["LevelType"];
        level.Score = (int)json["Score"];
        List<Seed> seeds = new List<Seed>();
        for (int i = 0; i < json["Seeds"].Count; i++)
        {
            JsonData SeedJson = json["Seeds"][i.ToString()];
            Seed seed = new Seed();
            seed.Num = (int)SeedJson["Num"];
            seed.NumType = (int)SeedJson["NumType"];
            seed.NumLocation = (int)SeedJson["NumLocation"];
            seed.ExitLocation = (int)SeedJson["ExitLocation"];
            seed.IsExit = (bool)SeedJson["IsExit"];
            seed.IsRole = (bool)SeedJson["IsRole"];
            seeds.Add(seed);
        }
        level.Seeds = seeds;
        return level;
    }
    /// <summary>
    /// 返回当前关卡数
    /// </summary>
    /// <returns></returns>
    public static int GetLevel()
    {
        TextAsset itemText = Resources.Load<TextAsset>("GameData/Level");
        string result = itemText.text;
        JsonData json = JsonMapper.ToObject(result);
        return (int)json["Level"];
    }

    /// <summary>
    /// 更新关卡数
    /// </summary>
    /// <param name="num"></param>
    public static void SetLevel(int num)
    {
        string path = Application.dataPath + "/Resources/GameData/Level.txt";
        if (File.Exists(path))
            File.Delete(path);
        string result = @"{""Level"":" + num + "}";
        File.WriteAllText(path, result);
    }

    /// <summary>
    /// 根据运行平台不同返回路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReturnPath(string path)
    {
        string to_path;
        if (Application.platform == RuntimePlatform.Android)//安卓先将文件从apk中取出，存到持久化数据存储目录中，再通过C#IO读取
        {
            string url = Application.streamingAssetsPath + path;
            to_path = Application.persistentDataPath + "/" + path;
            WWW www = new WWW(url);
            while (!www.isDone)
            {

            }//等待异步读取
            if (www.error == null)
            {
                File.WriteAllBytes(to_path, www.bytes);
            }
        }
        else
            to_path = Application.streamingAssetsPath + "/" + path;
        return to_path;
    }
}
