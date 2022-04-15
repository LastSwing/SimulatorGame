using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Assets.Script;
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
        Dictionary<string, string> dict = GameHelper.DataRead("Blood.txt");
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
        {
            int value = Convert.ToInt32(dict[BloodName]) + 1;
            #region 更新角色基础属性值
            field RoleFd = Common.ConvertModel<field>(GameHelper.DataRead("Role/Role.txt"));
            List<SecretClass> secret = SecretArea.LoadBlood;
            foreach (var item in secret)
            {
                if (item.Name == BloodName)
                {
                    switch (BloodName)
                    {
                        case "盘古血脉"://加回复
                            if (value == 1)
                                RoleFd.HPRegen += item.Numerical[0];
                            else if (value > 1)
                                RoleFd.HPRegen += item.Increment[0];
                            break;
                        case "女娲血脉"://加生命值
                            if (value == 1)
                                RoleFd.HP += item.Numerical[0];
                            else if (value > 1)
                                RoleFd.HP += item.Increment[0];
                            break;
                        case "伏羲血脉"://加暴击
                            if (value == 1)
                            {
                                RoleFd.Crit += item.Numerical[0];
                                if (RoleFd.Crit > 100)
                                {
                                    RoleFd.CritHarm += RoleFd.Crit - 100;
                                    RoleFd.Crit = 100;
                                }
                                RoleFd.CritHarm += item.Numerical[1];
                            }
                            else if (value > 1)
                            {
                                RoleFd.Crit += item.Increment[0];
                                if (RoleFd.Crit > 100)
                                {
                                    RoleFd.CritHarm += RoleFd.Crit - 100;
                                    RoleFd.Crit = 100;
                                }
                                RoleFd.CritHarm += item.Increment[1];
                            }
                            break;
                        case "共工血脉"://加闪避
                            if (value == 1)
                                RoleFd.Dodge += item.Numerical[0];
                            else if (value > 1)
                                RoleFd.Dodge += item.Increment[0];
                            break;
                        case "蚩尤血脉"://缴械
                            break;
                        case "夸父血脉"://加攻击力
                            if (value == 1)
                                RoleFd.Atk += item.Numerical[0];
                            else if (value > 1)
                                RoleFd.Atk += item.Increment[0];
                            break;
                        default:
                            break;
                    }
                }
            }
            string json = Common.ObjectToJson(RoleFd);
            //json = GameHelper.DesEncrypt(json);//前期不加密
            var path = Application.dataPath + "/Data/Role";
            //文件夹是否存在
            DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
            if (!myDirectoryInfo.Exists)
            {
                Directory.CreateDirectory(path);
            }
            if (File.Exists($"{path}/Role.txt"))
                File.Delete($"{path}/Role.txt");
            File.WriteAllText($"{path}/Role.txt", json); 
            #endregion
            dict[BloodName] = value.ToString();
        }
        GameHelper.DataExport(dict, "Blood");
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
        Dictionary<string, string> dict = GameHelper.DataRead("Opening.txt");
        field RoleFd = Common.ConvertModel<field>(GameHelper.DataRead("Role/Role.txt"));
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
        {
            foreach (var item in dict)
            {
                if (item.Key == BloodName)
                {
                    switch (item.Key)
                    {
                        case "神庭穴":
                            RoleFd.Atk += 5;
                            break;
                        case "紫宫穴":
                            RoleFd.Crit +=  5;
                            if (RoleFd.Crit > 100)
                            {
                                RoleFd.CritHarm += RoleFd.Crit - 100;
                                RoleFd.Crit = 100;
                            }
                            break;
                        case "鸠尾穴":
                            RoleFd.CritHarm +=  5F;
                            break;
                        case "气冲穴":
                            RoleFd.HP += 40;
                            break;
                        case "关元穴":
                            RoleFd.HPRegen +=  5;
                            break;
                        case "中枢穴":
                            RoleFd.Dodge += 5;
                            break;
                        default:
                            break;
                    }
                }
            }
            string json = Common.ObjectToJson(RoleFd);
            //json = GameHelper.DesEncrypt(json);//前期不加密
            var path = Application.dataPath + "/Data/Role";
            //文件夹是否存在
            DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
            if (!myDirectoryInfo.Exists)
            {
                Directory.CreateDirectory(path);
            }
            if (File.Exists($"{path}/Role.txt"))
                File.Delete($"{path}/Role.txt");
            File.WriteAllText($"{path}/Role.txt", json);
            dict[BloodName] = Convert.ToString(Convert.ToInt32(dict[BloodName]) + 1);
        }
        else if (!string.IsNullOrEmpty(BloodName) && o == 1)
        {
            foreach (var item in dict)
            {
                if (item.Key == BloodName)
                {
                    switch (item.Key)
                    {
                        case "神庭穴":
                            RoleFd.Atk -= 5;
                            break;
                        case "紫宫穴":
                            RoleFd.Crit -= 5;
                            break;
                        case "鸠尾穴":
                            RoleFd.CritHarm -= 5F;
                            break;
                        case "气冲穴":
                            RoleFd.HP -= 40;
                            break;
                        case "关元穴":
                            RoleFd.HPRegen -= 5;
                            break;
                        case "中枢穴":
                            RoleFd.Dodge -= 5;
                            break;
                        default:
                            break;
                    }
                }
            }
            string json = Common.ObjectToJson(RoleFd);
            //json = GameHelper.DesEncrypt(json);//前期不加密
            var path = Application.dataPath + "/Data/Role";
            //文件夹是否存在
            DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
            if (!myDirectoryInfo.Exists)
            {
                Directory.CreateDirectory(path);
            }
            if (File.Exists($"{path}/Role.txt"))
                File.Delete($"{path}/Role.txt");
            File.WriteAllText($"{path}/Role.txt", json);
            dict[BloodName] = Convert.ToString(Convert.ToInt32(dict[BloodName]) - 1);
        }

        GameHelper.DataExport(dict, "Opening");
        return dict;
    }

    /// <summary>
    /// 更新窍穴和血脉后的基础属性值
    /// </summary>
    /// <param name="secretClass">当前血脉属性</param>
    /// <returns>更新后的角色属性</returns>
    public static field OpeningLoad(out List<SecretClass> secretClass)
    {
        field RoleFd =  Common.ConvertModel<field>(GameHelper.DataRead("Role/Role.txt"));
        secretClass = new List<SecretClass>();
        #region 更新血脉属性附加到角色属性上
        Dictionary<string, string> keyValues = Blood("");
        List<SecretClass> secret = SecretArea.LoadBlood;
        foreach (var item in secret)
        {
            int value = Convert.ToInt32(keyValues[item.Name]);
            string result = "";
            if (value == 0)
            {
                if (item.Numerical.Length == 2)
                {
                    result = string.Format(item.Value, "0", "0");
                }
                else if (item.Numerical.Length == 1)
                {
                    result = string.Format(item.Value, "0");
                }
            }
            else
            {
                if (item.Numerical.Length == 2)
                {
                    result = string.Format(item.Value, item.Numerical[0] + (item.Increment[0] * (value - 1)), item.Numerical[1] + (item.Increment[1] * (value - 1)));
                }
                else if (item.Numerical.Length == 1)
                {
                    result = string.Format(item.Value, item.Numerical[0] + (item.Increment[0] * (value - 1)));
                }
            }
            switch (item.Name)
            {
                case "盘古血脉"://加回复
                    if (value == 1)
                        RoleFd.HPRegen += item.Numerical[0];
                    else if (value>1)
                        RoleFd.HPRegen += item.Numerical[0]+(item.Increment[0]*(value-1));
                    break;
                case "女娲血脉"://加生命值
                    if (value == 1)
                        RoleFd.HP += item.Numerical[0];
                    else if (value > 1)
                        RoleFd.HP += item.Numerical[0] + (item.Increment[0] * (value - 1));
                    break;
                case "伏羲血脉"://加暴击
                    if (value == 1)
                    {
                        RoleFd.Crit += item.Numerical[0];
                        if (RoleFd.Crit > 100)
                        {
                            RoleFd.CritHarm += RoleFd.Crit - 100;
                            RoleFd.Crit = 100;
                        }
                        RoleFd.CritHarm += item.Numerical[1];
                    }
                    else if (value > 1)
                    {
                        RoleFd.Crit += item.Numerical[0] + (item.Increment[0] * (value - 1));
                        if (RoleFd.Crit > 100)
                        {
                            RoleFd.CritHarm += RoleFd.Crit - 100;
                            RoleFd.Crit = 100;
                        }
                        RoleFd.CritHarm += item.Numerical[1] + (item.Increment[1] * (value - 1));
                    }
                    break;
                case "共工血脉"://加闪避
                    if (value == 1)
                        RoleFd.Dodge += item.Numerical[0];
                    else if (value > 1)
                        RoleFd.Dodge += item.Numerical[0] + (item.Increment[0] * (value - 1));
                    break;
                case "蚩尤血脉"://缴械
                    break;
                case "夸父血脉"://加攻击力
                    if (value == 1)
                        RoleFd.Atk += item.Numerical[0];
                    else if (value > 1)
                        RoleFd.Atk += item.Numerical[0] + (item.Increment[0] * (value - 1));
                    break;
                default:
                    break;
            }
            SecretClass secret1 = new SecretClass(item.Name, result, item.Classify, item.Numerical, item.Increment, value, item.Key);
            secretClass.Add(secret1);
        }
        #endregion
        #region 更新窍穴属性附加到角色属性上
        Dictionary<string, string> op = Opening("",2);
        foreach (var item in op)
        {
            switch (item.Key)
            {
                case "神庭穴":
                    RoleFd.Atk += Convert.ToInt32(item.Value) * 5;
                    break;
                case "紫宫穴":
                    RoleFd.Crit += Convert.ToInt32(item.Value) * 5;
                    if (RoleFd.Crit > 100)
                    {
                        RoleFd.CritHarm += RoleFd.Crit - 100;
                        RoleFd.Crit = 100;
                    }
                    break;
                case "鸠尾穴":
                    RoleFd.CritHarm += Convert.ToInt32(item.Value) * 5F;
                    break;
                case "气冲穴":
                    RoleFd.HP += Convert.ToInt32(item.Value) * 40;
                    break;
                case "关元穴":
                    RoleFd.HPRegen += Convert.ToInt32(item.Value) * 5;
                    break;
                case "中枢穴":
                    RoleFd.Dodge += Convert.ToInt32(item.Value) * 5;
                    break;
                default:
                    break;
            }
        }
        #endregion

        string json = Common.ObjectToJson(RoleFd);
        //json = GameHelper.DesEncrypt(json);//前期不加密
        var path = Application.dataPath + "/Data/Role";
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists($"{path}/Role.txt"))
            File.Delete($"{path}/Role.txt");
        File.WriteAllText($"{path}/Role.txt", json);
        return RoleFd;
    }
}
