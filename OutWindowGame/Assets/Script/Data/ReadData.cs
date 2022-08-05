using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class ReadData
{
    /// <summary>
    /// 返回关卡列表
    /// </summary>
    /// <returns></returns>
    public static List<Level> GetLevels()
    {
        List<Level> levels = new List<Level>();
        string path = Application.dataPath + "/Data/Level.txt";
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        //if (!myDirectoryInfo.Exists)
        //{
        //    Directory.CreateDirectory(path);
        //}
        if (File.Exists(path))
        {
            using (StreamReader tmpReader = File.OpenText(path))
            {
                string result = tmpReader.ReadToEnd();
                JsonData json = JsonMapper.ToObject(result);
                for (int i = 0; i < json.Count; i++)
                {
                    Level level = new Level();
                    level.LevelNum = (int)json[i]["LevelNum"];
                    level.Special = (bool)json[i]["Special"];
                    string[] size = json[i]["Size"].ToString().Split(',');
                    level.Size = new Vector2(Convert.ToInt32(size[0]), Convert.ToInt32(size[1]));
                    level.SceneID = (int)json[i]["SceneID"];
                    level.Props = json[i]["Props"].ToString();
                    levels.Add(level);
                }
            }
        }
        return levels;
    }
    /// <summary>
    /// 返回指定关卡物体列表
    /// </summary>
    /// <param name="LevelID">关卡ID</param>
    /// <returns></returns>
    public static List<LevelDetail> GetLevelDetail(int LevelID)
    {
        List<LevelDetail> detail = new List<LevelDetail>();
        string path = string.Format(Application.dataPath + "/Data/LevelDetail{0}.txt", LevelID);
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        //if (!myDirectoryInfo.Exists)
        //{
        //    Directory.CreateDirectory(path);
        //}
        if (File.Exists(path))
        {
            using (StreamReader tmpReader = File.OpenText(path))
            {
                string result = tmpReader.ReadToEnd();
                JsonData json = JsonMapper.ToObject(result);
                for (int i = 0; i < json.Count; i++)
                {
                    LevelDetail levelDetail = new LevelDetail();
                    levelDetail.LevelID = LevelID;
                    string[] location = json[i]["Location"].ToString().Split(',');
                    levelDetail.Location = new Vector2(float.Parse(location[0]), float.Parse(location[1]));
                    string[] size = json[i]["Size"].ToString().Split(',');
                    levelDetail.Size = new Vector2(float.Parse(size[0]), float.Parse(size[1]));
                    string[] rotation = json[i]["Rotation"].ToString().Split(',');
                    levelDetail.Rotation = new Vector3(float.Parse(rotation[0]), float.Parse(rotation[1]), float.Parse(rotation[2]));
                    levelDetail.IsView = (bool)json[i]["IsView"];
                    levelDetail.IsMotion = (bool)json[i]["IsMotion"];
                    levelDetail.Hierarchy = (int)json[i]["Hierarchy"];
                    levelDetail.IsHide = (bool)json[i]["IsHide"];
                    levelDetail.Gravity = float.Parse(json[i]["Gravity"].ToString());
                    levelDetail.AirDrag = float.Parse(json[i]["AirDrag"].ToString());
                    levelDetail.Mass = float.Parse(json[i]["Mass"].ToString());
                    levelDetail.DetailName = json[i]["DetailName"].ToString();
                    levelDetail.DetailType = (int)json[i]["DetailType"];
                    detail.Add(levelDetail);
                }
            }
        }
        return detail;
    }
    /// <summary>
    /// 返回场景列表
    /// </summary>
    /// <returns></returns>
    public static List<Scene> GetScene()
    {
        List<Scene> scenes = new List<Scene>();
        List<Level> levels = new List<Level>();
        string path = Application.dataPath + "/Data/Scene.txt";
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        //if (!myDirectoryInfo.Exists)
        //{
        //    Directory.CreateDirectory(path);
        //}
        if (File.Exists(path))
        {
            using (StreamReader tmpReader = File.OpenText(path))
            {
                string result = tmpReader.ReadToEnd();
                JsonData json = JsonMapper.ToObject(result);
                for (int i = 0; i < json.Count; i++)
                {
                    Scene scene = new Scene();
                    scene.SceneID = (int)json[i]["SceneID"];
                    scene.ImageName = json[i]["ImageName"].ToString();
                    scene.Gravity = float.Parse(json[i]["Gravity"].ToString());
                    scene.AirDrag = float.Parse(json[i]["AirDrag"].ToString());
                    scene.Mass = float.Parse(json[i]["Mass"].ToString());
                    scene.MusicName = json[i]["MusicName"].ToString();
                    scenes.Add(scene);
                }
            }
        }
        return scenes;
    }
    /// <summary>
    /// 返回游戏进程数据
    /// </summary>
    /// <returns></returns>
    public static Process GetProcess()
    {
        Process process = new Process();
        string path = Application.dataPath + "/Data/Process.txt";
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (File.Exists(path))
        {
            using (StreamReader tmpReader = File.OpenText(path))
            {
                string result = tmpReader.ReadToEnd();
                JsonData json = JsonMapper.ToObject(result);
                process.Level = (int)json["Level"];
                process.Revive = (int)json["Revive"];
                process.Die = (int)json["Die"];
                process.InTime = (int)json["InTime"];
                process.CollLevel = json["CollLevel"].ToString();
            }
        }
        return process;
    }

    /// <summary>
    /// 存储关卡数据
    /// </summary>
    /// <param name="levelDetails">关卡内道具</param>
    /// <param name="LevelNum">关卡数</param>
    public static void SaveLevelDetail(List<LevelDetail> levelDetails,int LevelNum)
    {
        string path = Application.dataPath + @"\Data\";
        string Name = "LevelDetail" + LevelNum;
        JsonData jd = new JsonData();
        for (int i = 0; i < levelDetails.Count; i++)
        {
            jd[i.ToString()] = new JsonData();
            jd[i]["LevelID"] = levelDetails[i].LevelID;
            jd[i]["Location"] = levelDetails[i].Location.x+","+ levelDetails[i].Location.y;
            jd[i]["Size"] = levelDetails[i].Size.x + "," + levelDetails[i].Size.y;
            jd[i]["Rotation"] = levelDetails[i].Rotation.x + "," + levelDetails[i].Rotation.y+","+ levelDetails[i].Rotation.z;
            jd[i]["IsView"] = levelDetails[i].IsView;
            jd[i]["IsMotion"] = levelDetails[i].IsMotion;
            jd[i]["Hierarchy"] = levelDetails[i].Hierarchy;
            jd[i]["IsHide"] = levelDetails[i].IsHide;
            jd[i]["Gravity"] = levelDetails[i].Gravity.ToString();
            jd[i]["AirDrag"] = levelDetails[i].AirDrag.ToString();
            jd[i]["Mass"] = levelDetails[i].Mass.ToString();
            jd[i]["DetailName"] = levelDetails[i].DetailName;
            jd[i]["DetailType"] = levelDetails[i].DetailType;
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
    /// 存储关卡
    /// </summary>
    /// <param name="levels">关卡</param>
    public static void SaveLevel(List<Level> levels)
    {
        string path = Application.dataPath + "/Data/";
        string Name = "Level";
        JsonData jd = new JsonData();
        for (int i = 0; i < levels.Count; i++)
        {
            jd[i.ToString()] = new JsonData();
            jd[i]["LevelNum"] = levels[i].LevelNum;
            jd[i]["Special"] = levels[i].Special;
            jd[i]["Size"] = levels[i].Size.x + "," + levels[i].Size.y;
            jd[i]["SceneID"] = levels[i].SceneID;
            jd[i]["Props"] = levels[i].Props;
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
}

