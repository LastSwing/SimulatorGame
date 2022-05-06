using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Tools
{
    public static class Common
    {
        public static int HasAgain;

        #region JSON格式化

        #region 作废

        /// <summary>
        /// Dic转Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static T DicToModel<T>(Dictionary<string, string> dict)
        {
            //
            T obj = Activator.CreateInstance<T>();
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
        #endregion

        /// <summary>
        /// object转json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToJson(this object obj)
        {
            string result = "{";
            Type t = obj.GetType();
            foreach (var item in t.GetProperties())
            {
                if (obj != null)
                {
                    if (item.PropertyType.IsPrimitive)//字符类型
                    {
                        result += $"\"{item.Name}\":{item.GetValue(obj)},";
                    }
                    else if (item.PropertyType == typeof(string))    //基础数据类型，非自定义的class或者struct
                    {
                        result += $"\"{item.Name}\":\"{item.GetValue(obj)}\",";
                    }
                    else if (item.PropertyType.IsGenericType)   //是否是泛型
                    {
                        var list = (IList)item.GetValue(obj);
                        if (list != null)
                        {
                            result += $"\"{item.Name}\":[";
                            foreach (var info in list)
                            {
                                result += info.ObjectToJson() + ",";
                            }

                            result = result.TrimEnd(',');
                            result += "]";
                        }
                    }
                    else if (item.PropertyType.IsClass)   //是否是对象
                    {
                        result += $"\"{item.Name}\":{item.GetValue(obj).ObjectToJson()},";
                    }
                }
                else
                {
                    if (item.PropertyType.IsPrimitive || item.PropertyType == typeof(string))   //对象为空直接赋双引号
                    {
                        result += $"\"{item.Name}\":\"\",";
                    }
                }
            }
            char[] mychar = { ',' };
            result = result.TrimEnd(mychar);
            result += "}";
            return result;
        }

        /// <summary>
        /// List转字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ListToJson<T>(this List<T> list)
        {
            string result = "[";
            foreach (var item in list)
            {
                result += item.ObjectToJson() + ",";
            }
            result = result.TrimEnd(',');
            result += "]";
            return result;
        }

        /// <summary>
        /// 字符串转Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonToModel<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default(T);
            var data = JsonMapper.ToObject(json);
            var t = Activator.CreateInstance<T>();
            foreach (var info in typeof(T).GetProperties())
            {
                object obj = null;
                //取得object对象中此属性的值
                object val = null;
                if (data.GetJsonType() != JsonType.None)
                {
                    if (((IDictionary)data).Contains(info?.Name))
                    {
                        val = data[info?.Name].ToString();
                    }
                }
                if (val != null)
                {
                    //非泛型
                    if (!info.PropertyType.IsGenericType)
                        obj = Convert.ChangeType(val, info.PropertyType);
                    else//泛型Nullable<>
                    {
                        Type genericTypeDefinition = info.PropertyType.GetGenericTypeDefinition();
                        if (genericTypeDefinition == typeof(Nullable<>))
                        {
                            obj = Convert.ChangeType(val, Nullable.GetUnderlyingType(info.PropertyType));
                        }
                        else
                        {
                            obj = Convert.ChangeType(val, info.PropertyType);
                        }
                    }
                    info.SetValue(t, obj, null);
                }
            }
            return t;
        }

        /// <summary>
        /// 单层字符串转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> JsonToList<T>(this string json)
        {
            List<T> list = new List<T>();
            if (string.IsNullOrWhiteSpace(json)) return default(List<T>);
            var data = JsonMapper.ToObject(json);
            foreach (JsonData item in data)
            {
                var t = Activator.CreateInstance<T>();
                foreach (var info in typeof(T).GetProperties())
                {
                    object obj = null;
                    //取得object对象中此属性的值
                    object val = null;
                    if (item.GetJsonType() != JsonType.None)
                    {
                        if (((IDictionary)item).Contains(info?.Name))//判断这个字段在字符串是否存在
                        {
                            val = item[info?.Name].ToString();
                        }
                    }
                    if (val != null)
                    {
                        //非泛型
                        if (!info.PropertyType.IsGenericType)
                            obj = Convert.ChangeType(val, info.PropertyType);
                        else//泛型Nullable<>
                        {

                        }
                        info.SetValue(t, obj, null);
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        /// 单层object对象转换为实体对象
        /// </summary>
        /// <typeparam name="T">实体对象类名</typeparam>
        /// <param name="asObject">object对象</param>
        /// <returns></returns>
        public static T ObjToModel<T>(this object asObject)
        {
            //创建实体对象实例
            var t = Activator.CreateInstance<T>();
            if (asObject != null)
            {
                Type type = asObject.GetType();
                //遍历实体对象属性
                foreach (var info in typeof(T).GetProperties())
                {
                    object obj = null;
                    //取得object对象中此属性的值
                    var val = type.GetProperty(info.Name)?.GetValue(asObject);
                    if (val != null)
                    {
                        //非泛型
                        if (!info.PropertyType.IsGenericType)
                            obj = Convert.ChangeType(val, info.PropertyType);
                        else//泛型Nullable<>
                        {
                            Type genericTypeDefinition = info.PropertyType.GetGenericTypeDefinition();
                            if (genericTypeDefinition == typeof(Nullable<>))
                            {
                                obj = Convert.ChangeType(val, Nullable.GetUnderlyingType(info.PropertyType));
                            }
                            else
                            {
                                obj = Convert.ChangeType(val, info.PropertyType);
                            }
                        }
                        info.SetValue(t, obj, null);
                    }
                }
            }
            return t;
        }

        #endregion

        #region 文件保存提取

        /// <summary>
        /// 保存字符串到文本
        /// </summary>
        /// <param name="json">字符串</param>
        /// <param name="path">文件名称</param>
        public static void SaveTxtFile(string json, string pathName)
        {

            //json = GameHelper.DesEncrypt(json);//前期不加密
            var path = Application.dataPath + "/Data/";
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
        /// 按文件名称获取文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pathName"></param>
        /// <returns>单个实体</returns>
        public static T GetTxtFileToModel<T>(string pathName)
        {
            var path = Application.dataPath + "/Data/";
            T role = Activator.CreateInstance<T>();
            //文件夹是否存在
            DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
            if (!myDirectoryInfo.Exists)
            {
                Directory.CreateDirectory(path);
            }
            if (File.Exists(path + pathName + ".txt"))
            {
                StreamReader json = File.OpenText(path + pathName + ".txt");
                //Debug.Log("读档" + json);
                string input = json.ReadToEnd();
                role = input.JsonToModel<T>();
                json.Close();
            }
            else
            {
                return default(T);
            }
            return role;
        }


        /// <summary>
        /// 按文件名称获取文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pathName"></param>
        /// <returns>实体泛型</returns>
        public static List<T> GetTxtFileToList<T>(string pathName)
        {
            var path = Application.dataPath + "/Data/";
            List<T> role = Activator.CreateInstance<List<T>>();
            //文件夹是否存在
            DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
            if (!myDirectoryInfo.Exists)
            {
                Directory.CreateDirectory(path);
            }
            if (File.Exists(path + pathName + ".txt"))
            {
                StreamReader json = File.OpenText(path + pathName + ".txt");
                //Debug.Log("读档" + json);
                string input = json.ReadToEnd();
                role = input.JsonToList<T>();
                json.Close();
            }
            else
            {
                return default(List<T>);
            }
            return role;
        }

        #endregion


        #region UI

        /// <summary>
        /// 跳转页面
        /// </summary>
        /// <param name="SceneName">场景名称</param>
        /// <param name="HasAgain">是否重新开始1、不重开，0重开</param>
        public static void SceneJump(string SceneName, int Again = 1)
        {
            HasAgain = Again;
            SceneManager.LoadScene(SceneName);
        }

        /// <summary>
        /// 将控件添加到父控件
        /// </summary>
        /// <param name="parent">父控件</param>
        /// <param name="prefab">预制件</param>
        /// <returns></returns>
        public static GameObject AddChild(Transform parent, GameObject prefab)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;

            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.SetParent(parent, false);
                go.layer = parent.gameObject.layer;
            }
            return go;
        }

        #endregion

        #region Method
        /// <summary>
        /// 随机排列数组元素
        /// </summary>
        /// <param name="myList"></param>
        /// <returns></returns>
        public static List<T> ListRandom<T>(this List<T> myList)
        {

            System.Random ran = new System.Random();
            int index = 0;
            var temp = Activator.CreateInstance<T>();
            for (int i = 0; i < myList.Count; i++)
            {

                index = ran.Next(0, myList.Count - 1);
                if (index != i)
                {
                    temp = myList[i];
                    myList[i] = myList[index];
                    myList[index] = temp;
                }
            }
            return myList;
        }

        /// <summary>
        /// 设置文字间距
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TextSpacing(this string str)
        {
            var result = string.Empty;
            for (int i = 0; i < str.Length; i++)
            {
                result += str[i] + "  ";
            }
            return result;
        }
        #endregion
    }
}
