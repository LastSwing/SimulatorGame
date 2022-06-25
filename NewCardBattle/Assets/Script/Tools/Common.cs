using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.Tools
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Common
    {
        #region Json处理
        /// <summary>
        /// dic转JSON
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string DicToJson(Dictionary<string, object> dic)
        {
            string result = "{";
            char[] mychar = { ',' };
            foreach (var item in dic.Keys)
            {
                if (dic[item] == null)
                {
                    result += $"\"{item}\":\"\",";
                    continue;
                }
                Type type = dic[item]?.GetType();
                if (type.IsClass || type.IsGenericType)
                {
                    result += $"\"{item}\":\"{{";
                    foreach (var info in type.GetProperties())
                    {
                        if (dic[item] != null)
                        {
                            if (info.PropertyType.IsPrimitive)//字符类型
                            {
                                result += $"\\\"{info.Name}\\\":{info.GetValue(dic[item])},";
                            }
                            else if (info.PropertyType == typeof(string))    //基础数据类型，非自定义的class或者struct
                            {
                                result += $"\\\"{info.Name}\\\":\\\"{info.GetValue(dic[item])}\\\",";
                            }
                        }
                        else
                        {
                            if (info.PropertyType.IsPrimitive || info.PropertyType == typeof(string))   //对象为空直接赋双引号
                            {
                                result += $"\\\"{info.Name}\\\":\\\"\\\",";
                            }
                        }
                    }
                    result = result.TrimEnd(mychar);
                    result += "}\",";
                }
                else
                {
                    if (dic[item] != null)
                    {
                        if (type.IsPrimitive)//字符类型
                        {
                            result += $"\"{item}\":{dic[item]},";
                        }
                        else if (type == typeof(string))    //基础数据类型，非自定义的class或者struct
                        {
                            result += $"\"{item}\":\"{dic[item]}\",";
                        }
                    }
                    else
                    {
                        if (type.IsPrimitive || type == typeof(string))   //对象为空直接赋双引号
                        {
                            result += $"\"{item}\":\"\",";
                        }
                    }
                }
            }

            result = result.TrimEnd(mychar);
            result += "}";
            return result;
        }
        /// <summary>
        /// dic转JSON
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string DicToJson(Dictionary<string, string> dic)
        {
            string result = "{";
            char[] mychar = { ',' };
            foreach (var item in dic.Keys)
            {
                if (dic[item] == null)
                {
                    result += $"\"{item}\":\"\",";
                    continue;
                }
                Type type = dic[item]?.GetType();
                if (dic[item] != null)
                {
                    if (type.IsPrimitive)//字符类型
                    {
                        result += $"\"{item}\":{dic[item]},";
                    }
                    else if (type == typeof(string))    //基础数据类型，非自定义的class或者struct
                    {
                        result += $"\"{item}\":\"{dic[item]}\",";
                    }
                }
                else
                {
                    if (type.IsPrimitive || type == typeof(string))   //对象为空直接赋双引号
                    {
                        result += $"\"{item}\":\"\",";
                    }
                }
            }

            result = result.TrimEnd(mychar);
            result += "}";
            return result;
        }


        /// <summary>
        /// Dic转Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static T DicToModel<T>(Dictionary<string, string> dict)
        {
            //
            T obj = default(T);
            obj = Activator.CreateInstance<T>();

            //根据Key值设定 Columns
            foreach (KeyValuePair<string, string> item in dict)
            {
                if (item.Key == "ID")
                {
                    continue;
                }
                PropertyInfo prop = obj.GetType().GetProperty(item.Key);
                if (!string.IsNullOrEmpty(item.Value))
                {
                    object value = item.Value;
                    //Nullable 获取Model类字段的真实类型
                    Type itemType = Nullable.GetUnderlyingType(prop.PropertyType) == null ? prop.PropertyType : Nullable.GetUnderlyingType(prop.PropertyType);
                    //根据Model类字段的真实类型进行转换
                    prop.SetValue(obj, Convert.ChangeType(value, itemType), null);
                }


            }
            return obj;
        }

        /// <summary>
        /// 保存字符串到文本
        /// </summary>
        /// <param name="json">字符串</param>
        /// <param name="path">文件名称</param>
        /// <param name="Folder">Data/文件夹</param>
        public static void SaveTxtFile(string json, string pathName, string Folder = "")
        {

            //json = GameHelper.DesEncrypt(json);//前期不加密
            var path = Application.dataPath + "/Data/" + Folder + "/";
            //文件夹是否存在
            DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
            if (!myDirectoryInfo.Exists)
            {
                Directory.CreateDirectory(path);
            }
            if (File.Exists($"{path}/{pathName}.txt"))
                File.Delete($"{path}/{pathName}.txt");
            File.WriteAllText($"{path}/{pathName}.txt", json);
        }

        /// <summary>
        /// 读档
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static void DicDataRead(ref Dictionary<string, string> dict,string Name)
        {
            var path = Application.dataPath + "/Data/Resources/";
            //Dictionary<string, string> dict = new Dictionary<string, string>();
            //文件夹是否存在
            DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
            if (!myDirectoryInfo.Exists)
            {
                Directory.CreateDirectory(path);
            }
            if (File.Exists(path + Name + ".txt"))
            {
                StreamReader json = File.OpenText(path + Name + ".txt");
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
            //return dict;
        }

        #endregion
    }
}
