﻿using Assets.Script.Models;
using LitJson;
using Newtonsoft.Json;
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
using UnityEngine.UI;

namespace Assets.Script.Tools
{
    public static class Common
    {
        public static int HasAgain;

        public static string ReturnName;

        #region JSON格式化

        #region 作废

        ///// <summary>
        ///// Dic转Model
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="dict"></param>
        ///// <returns></returns>
        //public static T DicToModel<T>(Dictionary<string, string> dict)
        //{
        //    //
        //    T obj = Activator.CreateInstance<T>();
        //    //根据Key值设定 Columns
        //    foreach (KeyValuePair<string, string> item in dict)
        //    {
        //        if (item.Key == "ID")
        //        {
        //            continue;
        //        }
        //        PropertyInfo prop = obj.GetType().GetProperty(item.Key);
        //        if (!string.IsNullOrEmpty(item.Value))
        //        {
        //            object value = item.Value;
        //            //Nullable 获取Model类字段的真实类型
        //            Type itemType = Nullable.GetUnderlyingType(prop.PropertyType) == null ? prop.PropertyType : Nullable.GetUnderlyingType(prop.PropertyType);
        //            //根据Model类字段的真实类型进行转换
        //            prop.SetValue(obj, Convert.ChangeType(value, itemType), null);
        //        }


        //    }
        //    return obj;
        //}

        ///// <summary>
        ///// dic转JSON
        ///// </summary>
        ///// <param name="dic"></param>
        ///// <returns></returns>
        //public static string DicToJson(Dictionary<string, object> dic)
        //{
        //    string result = "{";
        //    char[] mychar = { ',' };
        //    foreach (var item in dic.Keys)
        //    {
        //        if (dic[item] == null)
        //        {
        //            result += $"\"{item}\":\"\",";
        //            continue;
        //        }
        //        Type type = dic[item]?.GetType();
        //        if (type.IsClass || type.IsGenericType)
        //        {
        //            result += $"\"{item}\":\"{{";
        //            foreach (var info in type.GetProperties())
        //            {
        //                if (dic[item] != null)
        //                {
        //                    if (info.PropertyType.IsPrimitive)//字符类型
        //                    {
        //                        result += $"\\\"{info.Name}\\\":{info.GetValue(dic[item])},";
        //                    }
        //                    else if (info.PropertyType == typeof(string))    //基础数据类型，非自定义的class或者struct
        //                    {
        //                        result += $"\\\"{info.Name}\\\":\\\"{info.GetValue(dic[item])}\\\",";
        //                    }
        //                }
        //                else
        //                {
        //                    if (info.PropertyType.IsPrimitive || info.PropertyType == typeof(string))   //对象为空直接赋双引号
        //                    {
        //                        result += $"\\\"{info.Name}\\\":\\\"\\\",";
        //                    }
        //                }
        //            }
        //            result = result.TrimEnd(mychar);
        //            result += "}\",";
        //        }
        //        else
        //        {
        //            if (dic[item] != null)
        //            {
        //                if (type.IsPrimitive)//字符类型
        //                {
        //                    result += $"\"{item}\":{dic[item]},";
        //                }
        //                else if (type == typeof(string))    //基础数据类型，非自定义的class或者struct
        //                {
        //                    result += $"\"{item}\":\"{dic[item]}\",";
        //                }
        //            }
        //            else
        //            {
        //                if (type.IsPrimitive || type == typeof(string))   //对象为空直接赋双引号
        //                {
        //                    result += $"\"{item}\":\"\",";
        //                }
        //            }
        //        }
        //    }

        //    result = result.TrimEnd(mychar);
        //    result += "}";
        //    return result;
        //}
        #endregion

        ///// <summary>
        ///// object转json
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static string ObjectToJson(this object obj)
        //{
        //    string result = "{";
        //    Type t = obj.GetType();
        //    foreach (var item in t.GetProperties())
        //    {
        //        if (obj != null)
        //        {
        //            if (item.PropertyType.IsPrimitive)//字符类型
        //            {
        //                result += $"\"{item.Name}\":{item.GetValue(obj)},";
        //            }
        //            else if (item.PropertyType == typeof(string))    //基础数据类型，非自定义的class或者struct
        //            {
        //                result += $"\"{item.Name}\":\"{item.GetValue(obj)}\",";
        //            }
        //            else if (item.PropertyType.IsGenericType)   //是否是泛型
        //            {
        //                var list = (IList)item.GetValue(obj);
        //                if (list != null)
        //                {
        //                    result += $"\"{item.Name}\":[";
        //                    foreach (var info in list)
        //                    {
        //                        result += info.ObjectToJson() + ",";
        //                    }

        //                    result = result.TrimEnd(',');
        //                    result += "]";
        //                }
        //            }
        //            else if (item.PropertyType.IsClass)   //是否是对象
        //            {
        //                result += $"\"{item.Name}\":{item.GetValue(obj).ObjectToJson()},";
        //            }
        //        }
        //        else
        //        {
        //            if (item.PropertyType.IsPrimitive || item.PropertyType == typeof(string))   //对象为空直接赋双引号
        //            {
        //                result += $"\"{item.Name}\":\"\",";
        //            }
        //        }
        //    }
        //    char[] mychar = { ',' };
        //    result = result.TrimEnd(mychar);
        //    result += "}";
        //    return result;
        //}


        ///// <summary>
        ///// 字符串转Model
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="json"></param>
        ///// <returns></returns>
        //public static T JsonToModel<T>(this string json)
        //{
        //    if (string.IsNullOrWhiteSpace(json)) return default(T);
        //    var data = JsonMapper.ToObject(json);
        //    var t = Activator.CreateInstance<T>();
        //    foreach (var info in typeof(T).GetProperties())
        //    {
        //        object obj = null;
        //        //取得object对象中此属性的值
        //        object val = null;
        //        if (data.GetJsonType() != JsonType.None)
        //        {
        //            if (((IDictionary)data).Contains(info?.Name))
        //            {
        //                val = data[info?.Name].ToString();
        //            }
        //        }
        //        if (val != null)
        //        {
        //            //非泛型
        //            if (!info.PropertyType.IsGenericType)
        //                obj = Convert.ChangeType(val, info.PropertyType);
        //            else//泛型Nullable<>
        //            {
        //                Type genericTypeDefinition = info.PropertyType.GetGenericTypeDefinition();
        //                if (genericTypeDefinition == typeof(Nullable<>))
        //                {
        //                    obj = Convert.ChangeType(val, Nullable.GetUnderlyingType(info.PropertyType));
        //                }
        //                else
        //                {
        //                    obj = Convert.ChangeType(val, info.PropertyType);
        //                }
        //            }
        //            info.SetValue(t, obj, null);
        //        }
        //    }
        //    return t;
        //}

        ///// <summary>
        ///// 单层字符串转List
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="json"></param>
        ///// <returns></returns>
        //public static List<T> JsonToList<T>(this string json)
        //{
        //    List<T> list = new List<T>();
        //    if (string.IsNullOrWhiteSpace(json)) return default(List<T>);
        //    var data = JsonMapper.ToObject(json);
        //    foreach (JsonData item in data)
        //    {
        //        var t = Activator.CreateInstance<T>();
        //        foreach (var info in typeof(T).GetProperties())
        //        {
        //            object obj = null;
        //            //取得object对象中此属性的值
        //            object val = null;
        //            if (item.GetJsonType() != JsonType.None)
        //            {
        //                if (((IDictionary)item).Contains(info?.Name))//判断这个字段在字符串是否存在
        //                {
        //                    val = item[info?.Name];
        //                }
        //            }
        //            if (val != null)
        //            {
        //                //非泛型
        //                if (!info.PropertyType.IsGenericType)
        //                    obj = Convert.ChangeType(val.ToString(), info.PropertyType);
        //                else//泛型Nullable<>
        //                {
        //                    JsonData jd = (JsonData)val;
        //                    obj = Convert.ChangeType(GenericToList(info.PropertyType, jd), info.PropertyType);
        //                }
        //                info.SetValue(t, obj, null);
        //            }
        //        }
        //        list.Add(t);
        //    }
        //    return list;
        //}

        //private static object GenericToList(Type type, JsonData data)
        //{
        //    ArrayList arrayList = new ArrayList();
        //    foreach (JsonData item in data)
        //    {
        //        var t = Activator.CreateInstance(type);
        //        foreach (var info in type.GetProperties())
        //        {
        //            object obj = null;
        //            //取得object对象中此属性的值
        //            object val = null;
        //            if (item.GetJsonType() != JsonType.None)
        //            {
        //                if (((IDictionary)item).Contains(info?.Name))//判断这个字段在字符串是否存在
        //                {
        //                    val = item[info?.Name];
        //                }
        //            }
        //            if (val != null)
        //            {
        //                //非泛型
        //                if (!info.PropertyType.IsGenericType)
        //                    obj = Convert.ChangeType(val.ToString(), info.PropertyType);
        //                else//泛型Nullable<>
        //                {
        //                    JsonData jd = (JsonData)val;
        //                    obj = Convert.ChangeType(GenericToList(info.PropertyType, jd), info.PropertyType);
        //                }
        //                info.SetValue(t, obj, null);
        //            }
        //        }
        //        arrayList.Add(t);
        //    }
        //    return arrayList;
        //}

        ///// <summary>
        ///// 单层object对象转换为实体对象
        ///// </summary>
        ///// <typeparam name="T">实体对象类名</typeparam>
        ///// <param name="asObject">object对象</param>
        ///// <returns></returns>
        //public static T ObjToModel<T>(this object asObject)
        //{
        //    //创建实体对象实例
        //    var t = Activator.CreateInstance<T>();
        //    if (asObject != null)
        //    {
        //        Type type = asObject.GetType();
        //        //遍历实体对象属性
        //        foreach (var info in typeof(T).GetProperties())
        //        {
        //            object obj = null;
        //            //取得object对象中此属性的值
        //            var val = type.GetProperty(info.Name)?.GetValue(asObject);
        //            if (val != null)
        //            {
        //                //非泛型
        //                if (!info.PropertyType.IsGenericType)
        //                    obj = Convert.ChangeType(val, info.PropertyType);
        //                else//泛型Nullable<>
        //                {
        //                    Type genericTypeDefinition = info.PropertyType.GetGenericTypeDefinition();
        //                    if (genericTypeDefinition == typeof(Nullable<>))
        //                    {
        //                        obj = Convert.ChangeType(val, Nullable.GetUnderlyingType(info.PropertyType));
        //                    }
        //                    else
        //                    {
        //                        obj = Convert.ChangeType(val, info.PropertyType);
        //                    }
        //                }
        //                info.SetValue(t, obj, null);
        //            }
        //        }
        //    }
        //    return t;
        //}

        #endregion

        #region 文本操作

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
        /// 按文件名称获取文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pathName"></param>
        /// <returns>单个实体</returns>
        public static T GetTxtFileToModel<T>(string pathName, string Folder = "")
        {
            var path = Application.dataPath + "/Data/" + Folder + "/";
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
        public static List<T> GetTxtFileToList<T>(string pathName, string Folder = "")
        {
            var path = Application.dataPath + "/Data/" + Folder + "/";
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
                role = JsonToList<T>(input);
                json.Close();
            }
            else
            {
                return default(List<T>);
            }
            return role;
        }

        /// <summary>
        /// 删除文本
        /// </summary>
        /// <param name="path">文件名称</param>
        public static void DeleteTxtFile(string pathName)
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
        }

        #endregion

        #region UI

        /// <summary>
        /// 跳转页面
        /// </summary>
        /// <param name="SceneName">场景名称</param>
        /// <param name="Again"></param>
        /// <param name="Name">起始页面名称</param>
        public static void SceneJump(string SceneName, int Again = 1, string Name = "")
        {
            HasAgain = Again;
            ReturnName = Name;
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

        /// <summary>
        /// 图片绑定
        /// </summary>
        /// <param name="imgUrl">图片路径</param>
        /// <param name="imgControl">图片控件</param>
        public static void ImageBind(string imgUrl, Image imgControl)
        {
            Sprite img = Resources.Load(imgUrl, typeof(Sprite)) as Sprite;
            imgControl.sprite = img;
        }

        /// <summary>
        /// 血量图片变化
        /// </summary>
        /// <param name="HpImg">血量图片控件</param>
        /// <param name="InitHP">最大血量</param>
        /// <param name="ChangeHp">变化血量值</param>
        /// <param name="ChangeType">变化类型：1加血，0减血</param>
        public static void HPImageChange(Image HpImg, float InitHP, float ChangeHp, int ChangeType, int ImgWidth = 310)
        {
            if (ChangeHp > 0)
            {
                RectTransform Irect = HpImg.GetComponent<RectTransform>();
                float imgW = Irect.sizeDelta.x;
                float OneHpWidth = ImgWidth / InitHP;//图片一滴血的宽度
                float ChangeImgW = ChangeHp * OneHpWidth;
                if (ChangeType == 0)
                {
                    float total = imgW - ChangeImgW;
                    if (total < 0)
                    {
                        total = 0;
                    }
                    Irect.sizeDelta = new Vector2(total, Irect.sizeDelta.y);
                    HpImg.transform.localPosition = new Vector3(HpImg.transform.localPosition.x - ChangeImgW / 2, HpImg.transform.localPosition.y);
                }
                else
                {
                    float total = imgW + ChangeImgW;
                    if (total > ImgWidth)//初始宽度310
                    {
                        total = ImgWidth;
                    }
                    Irect.sizeDelta = new Vector2(total, Irect.sizeDelta.y);
                    HpImg.transform.localPosition = new Vector3(HpImg.transform.localPosition.x - ChangeImgW / 2, HpImg.transform.localPosition.y);
                }
            }
        }

        /// <summary>
        ///  能量变化
        /// </summary>
        /// <param name="cEnergy">当前能量</param>
        /// <param name="changeEnergy">变化的能量</param>
        /// <param name="ChangeType">变化类型。0减能量，1加能量</param>
        /// <param name="MaxEnergy">最大能量</param>
        public static void EnergyImgChange(int cEnergy, int changeEnergy, int ChangeType, int MaxEnergy)
        {
            for (int i = 1; i <= changeEnergy; i++)
            {
                if (ChangeType == 0)
                {
                    if (changeEnergy > cEnergy) return;
                    Image EnergyI = GameObject.Find($"GameCanvas/CardPool/img_Energy{cEnergy - i}").GetComponent<Image>();
                    EnergyI.transform.localScale = Vector3.zero;
                }
                else
                {
                    if (MaxEnergy == cEnergy) return;
                    Image EnergyI = GameObject.Find($"GameCanvas/CardPool/img_Energy{cEnergy + changeEnergy - i}").GetComponent<Image>();
                    EnergyI.transform.localScale = Vector3.one;
                }
            }
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
        /// 随机排列数组元素
        /// </summary>
        /// <param name="myList"></param>
        /// <returns></returns>
        public static List<string> ListRandom(this List<string> myList)
        {
            System.Random ran = new System.Random();
            int index = 0;
            var temp = string.Empty;
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

        /// <summary>
        /// 游戏结束数据重置
        /// </summary>
        /// <returns></returns>
        public static void GameOverDataReset()
        {
            SaveTxtFile(null, GlobalAttr.CurrentMapCombatPointFileName, "Map");
            SaveTxtFile(null, GlobalAttr.CurrentMapLocationFileName, "Map");
            SaveTxtFile(null, GlobalAttr.CurrentMapPathFileName, "Map");
            SaveTxtFile(null, GlobalAttr.CurrentCardPoolsFileName);
            SaveTxtFile(null, GlobalAttr.CurrentPlayerRoleFileName);

        }

        /// <summary>
        /// 获取表结构ID
        /// </summary>
        /// <returns></returns>
        public static int GetTablesID()
        {
            int result = 1001;
            var list = GetTxtFileToList<TableStruct>(GlobalAttr.AllTablesAttrFlieName, "Tables");
            if (list?.Count > 0)
            {
                result = list.Max(a => a.ID) + 1;
            }
            return result;
        }
        #endregion

        #region 新Json解析


        /// <summary>
        /// 单层字符串转List(老的解析)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> JsonToListOnSingleLayer<T>(this string json)
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
                            val = item[info?.Name];
                        }
                    }
                    if (val != null)
                    {
                        //非泛型
                        if (!info.PropertyType.IsGenericType)
                            obj = Convert.ChangeType(val.ToString(), info.PropertyType);
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
        /// ModelToJson
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToJson(this object obj)
        {
            string result = "{";
            Type t = obj.GetType();
            string className = "";
            string typeStr = t.ToString();
            if (typeStr.Contains("Models"))
            {
                className = typeStr.Substring(typeStr.IndexOf("Models") + 7);
                List<TableStruct> tables = GetTableStructData();
                if (tables != null)
                {
                    tables = tables.FindAll(a => a.ClassName == className);
                    foreach (var item in tables)
                    {
                        if (item.AttrType == "System.String")
                        {
                            result += $"\"{item.AttrName}\":\"{t.GetProperty(item.AttrName)?.GetValue(obj)}\",";
                        }
                        else if (item.AttrType == "System.Int32" || item.AttrType == "System.Single")//数字类型
                        {
                            var val = t.GetProperty(item.AttrName)?.GetValue(obj);
                            if (val == null)
                            {
                                val = 0;
                            }
                            result += $"\"{item.AttrName}\":{val},";
                        }
                        else if (item.AttrType.Contains("Generic.List"))//List类型
                        {
                            var list = (IList)t.GetProperty(item.AttrName)?.GetValue(obj);
                            if (list != null)
                            {
                                result += $"\"{item.AttrName}\":[";
                                foreach (var info in list)
                                {
                                    result += info.ObjectToJson() + ",";
                                }

                                result = result.TrimEnd(',');
                                result += "],";
                            }
                            else
                            {
                                result += $"\"{item.AttrName}\":null,";
                            }
                        }
                        else if (item.AttrType.Contains("Models"))//普通Model
                        {
                            var val = t.GetProperty(item.AttrName)?.GetValue(obj)?.ObjectToJson();
                            if (string.IsNullOrEmpty(val))
                            {
                                result += $"\"{item.AttrName}\":null,";
                            }
                            else
                            {
                                result += $"\"{item.AttrName}\":{val},";
                            }

                        }
                    }
                }
            }
            else if (typeStr.Contains("JsonData"))
            {
                if (obj != null)
                {
                    return JsonMapper.ToJson(obj);
                }
                else
                {
                    return "";
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
            if (list == null) return "";
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
            string className = "";
            string typeStr = t.ToString();
            if (typeStr.Contains("Models"))
            {
                className = typeStr.Substring(typeStr.IndexOf("Models") + 7);
                List<TableStruct> tables = GetTableStructData();
                if (tables != null)
                {
                    tables = tables.FindAll(a => a.ClassName == className);
                    foreach (var info in tables)
                    {
                        object obj = null;
                        //取得object对象中此属性的值
                        object val = null;
                        if (data.GetJsonType() != JsonType.None)
                        {
                            if (((IDictionary)data).Contains(info?.AttrName))
                            {
                                val = data[info?.AttrName];
                            }
                        }
                        if (val != null)
                        {
                            if (info.AttrType == "System.String")
                            {
                                obj = val.ToString();
                            }
                            else if (info.AttrType == "System.Int32" || info.AttrType == "System.Single")//数字类型
                            {
                                obj = Convert.ToInt32(val.ToString());
                            }
                            else if (info.AttrType.Contains("Generic.List"))//List类型
                            {
                                Type modelType = Type.GetType(info.AttrType);
                                string tName = info.AttrType.Substring(info.AttrType.IndexOf('[') + 1);
                                tName = tName.Remove(tName.IndexOf(']'));
                                obj = ToType(val, modelType, tName);
                            }
                            else if (info.AttrType.Contains("Models"))//普通Model
                            {
                                Type modelType = Type.GetType(info.AttrType);
                                obj = ToType(val.ObjectToJson(), modelType);
                            }
                            t.GetType().GetProperty(info.AttrName).SetValue(t, obj, null);
                        }
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// 字符串转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> JsonToList<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default(List<T>);
            List<T> list = new List<T>();
            var jData = JsonMapper.ToObject(json);
            foreach (JsonData data in jData)
            {
                var t = Activator.CreateInstance<T>();
                string className = "";
                string typeStr = t.ToString();
                if (typeStr.Contains("Models"))
                {
                    className = typeStr.Substring(typeStr.IndexOf("Models") + 7);
                    List<TableStruct> tables = GetTableStructData();
                    if (tables != null)
                    {
                        tables = tables.FindAll(a => a.ClassName == className);
                        foreach (var info in tables)
                        {
                            object obj = null;
                            //取得object对象中此属性的值
                            object val = null;
                            if (data.GetJsonType() != JsonType.None)
                            {
                                if (((IDictionary)data).Contains(info?.AttrName))
                                {
                                    val = data[info?.AttrName];
                                }
                            }
                            if (val != null)
                            {
                                if (info.AttrType == "System.String")
                                {
                                    obj = val.ToString();
                                }
                                else if (info.AttrType == "System.Int32" || info.AttrType == "System.Single")//数字类型
                                {
                                    obj = Convert.ToInt32(val.ToString());
                                }
                                else if (info.AttrType.Contains("Generic.List"))//List类型
                                {
                                    Type modelType = Type.GetType(info.AttrType);
                                    string tName = info.AttrType.Substring(info.AttrType.IndexOf('[') + 1);
                                    tName = tName.Remove(tName.IndexOf(']'));
                                    obj = ToType(val, modelType, tName);
                                }
                                else if (info.AttrType.Contains("Models"))//普通Model
                                {
                                    Type modelType = Type.GetType(info.AttrType);
                                    obj = ToType(val.ObjectToJson(), modelType);
                                }
                                t.GetType().GetProperty(info.AttrName).SetValue(t, obj, null);
                            }
                        }
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        /// 返回泛型类型
        /// </summary>
        /// <param name="inObj"></param>
        /// <param name="returnType"></param>
        /// <param name="TName">ModelName</param>
        /// <returns></returns>
        public static dynamic ToType(object inObj, Type returnType, string TName)
        {
            if (string.IsNullOrWhiteSpace(inObj.ToString())) return null;
            var list = returnType.Assembly.CreateInstance(returnType.FullName) as IList;
            JsonData jd = (JsonData)inObj;
            Type modelT = Type.GetType(TName);
            var t = Activator.CreateInstance(modelT);
            string className = "";
            string typeStr = t.ToString();
            if (typeStr.Contains("Models"))
            {
                className = typeStr.Substring(typeStr.IndexOf("Models") + 7);
                List<TableStruct> tables = GetTableStructData();
                if (tables != null)
                {
                    tables = tables.FindAll(a => a.ClassName == className);
                    foreach (JsonData data in jd)
                    {
                        foreach (var info in tables)
                        {
                            object obj = null;
                            //取得object对象中此属性的值
                            object val = null;
                            if (data.GetJsonType() != JsonType.None)
                            {
                                if (((IDictionary)data).Contains(info?.AttrName))
                                {
                                    val = data[info?.AttrName];
                                }
                            }
                            if (val != null)
                            {
                                if (info.AttrType == "System.String")
                                {
                                    obj = val.ToString();
                                }
                                else if (info.AttrType == "System.Int32" || info.AttrType == "System.Single")//数字类型
                                {
                                    obj = Convert.ToInt32(val.ToString());
                                }
                                else if (info.AttrType.Contains("Generic.List"))//List类型
                                {
                                    Type modelType = Type.GetType(info.AttrType);
                                    string tName = info.AttrType.Substring(info.AttrType.IndexOf('[') + 1);
                                    tName = tName.Remove(tName.IndexOf(']'));
                                    obj = ToType(val, modelType, tName);
                                }
                                else if (info.AttrType.Contains("Models"))//普通Model
                                {
                                    Type modelType = Type.GetType(info.AttrType);
                                    obj = ToType(val.ObjectToJson(), modelType);
                                }
                                t.GetType().GetProperty(info.AttrName).SetValue(t, obj, null);
                            }
                        }
                        list.Add(t);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 返回指定类型
        /// </summary>
        /// <param name="json"></param>
        /// <param name="returnType">指定类型</param>
        /// <returns></returns>
        public static dynamic ToType(string json, Type returnType)
        {
            if (string.IsNullOrWhiteSpace(json)) return Activator.CreateInstance(returnType);
            var t = Activator.CreateInstance(returnType);
            var data = JsonMapper.ToObject(json);
            string className = "";
            string typeStr = t.ToString();
            if (typeStr.Contains("Models"))
            {
                className = typeStr.Substring(typeStr.IndexOf("Models") + 7);
                List<TableStruct> tables = GetTableStructData();
                if (tables != null)
                {
                    tables = tables.FindAll(a => a.ClassName == className);
                    foreach (var info in tables)
                    {
                        object obj = null;
                        //取得object对象中此属性的值
                        object val = null;
                        if (data.GetJsonType() != JsonType.None)
                        {
                            if (((IDictionary)data).Contains(info?.AttrName))
                            {
                                val = data[info?.AttrName];
                            }
                        }
                        if (val != null)
                        {
                            if (info.AttrType == "System.String")
                            {
                                obj = val.ToString();
                            }
                            else if (info.AttrType == "System.Int32" || info.AttrType == "System.Single")//数字类型
                            {
                                obj = Convert.ToInt32(val.ToString());
                            }
                            else if (info.AttrType.Contains("Generic.List"))//List类型
                            {
                                Type modelType = Type.GetType(info.AttrType);
                                string tName = info.AttrType.Substring(info.AttrType.IndexOf('[') + 1);
                                tName = tName.Remove(tName.IndexOf(']'));
                                obj = ToType(val, modelType, tName);
                            }
                            else if (info.AttrType.Contains("Models"))//普通Model
                            {
                                Type modelType = Type.GetType(info.AttrType);
                                obj = ToType(val.ObjectToJson(), modelType);
                            }
                            t.GetType().GetProperty(info.AttrName).SetValue(t, obj, null);
                        }
                    }
                }
            }
            return t;
        }

        #endregion

        #region 表结构操作

        /// <summary>
        /// 获取表结构数据
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns>实体泛型</returns>
        public static List<TableStruct> GetTableStructData()
        {
            var path = Application.dataPath + "/Data/Tables/";
            List<TableStruct> role = new List<TableStruct>();
            //文件夹是否存在
            DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
            if (!myDirectoryInfo.Exists)
            {
                Directory.CreateDirectory(path);
            }
            if (File.Exists(path + GlobalAttr.AllTablesAttrFlieName + ".txt"))
            {
                StreamReader json = File.OpenText(path + GlobalAttr.AllTablesAttrFlieName + ".txt");
                //Debug.Log("读档" + json);
                string input = json.ReadToEnd();
                role = JsonToListOnSingleLayer<TableStruct>(input);
                json.Close();
            }
            else
            {
                return null;
            }
            return role;
        }

        /// <summary>
        /// 保存表结构到文本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="className">表名称</param>
        public static void SaveTablesStruct<T>(string className)
        {
            var list = GetTableStructData();
            if (list == null)
            {
                list = new List<TableStruct>();
            }
            int id = GetTablesID();
            foreach (var item in typeof(T).GetProperties())
            {
                TableStruct table = new TableStruct();
                table.AttrName = item.Name;
                table.AttrType = item.PropertyType.ToString();
                table.ID = id;
                table.ClassName = className;
                id++;
                list.Add(table);
            }
            SaveTxtFile(list.ListToJson(), GlobalAttr.AllTablesAttrFlieName, "Tables");
        }

        /// <summary>
        /// 表新增字段
        /// </summary>
        /// <param name="table"></param>
        public static void TableAddCoulum(TableStruct table)
        {
            var list = GetTableStructData();
            if (list == null)
            {
                list = new List<TableStruct>();
            }
            table.ID = GetTablesID();
            list.Add(table);
            SaveTxtFile(list.ListToJson(), GlobalAttr.AllTablesAttrFlieName, "Tables");
        }

        /// <summary>
        /// 删除表字段
        /// </summary>
        public static void DeleteTableColumn(TableStruct table)
        {
            var list = GetTableStructData();
            if (list != null)
            {
                list.Remove(table);
                SaveTxtFile(list.ListToJson(), GlobalAttr.AllTablesAttrFlieName, "Tables");
            }
        }

        /// <summary>
        /// 把原来字段的数据绑定到新字段里
        /// </summary>
        /// <param name="OriginalColumnID">原字段TableStructID</param>
        /// <param name="NowColumnID">现字段TableStructID</param>
        /// <param name="FileName">要更新的文件名称</param>
        /// <param name="Folder">文件夹名称</param>
        public static void UpdateColumnData<T>(int OriginalColumnID, int NowColumnID, string FileName, string Folder = "")
        {
            var list = GetTableStructData();
            var originModel = list.Find(a => a.ID == OriginalColumnID);
            var nowModel = list.Find(a => a.ID == NowColumnID);
            var data = GetTxtFileToList<T>(FileName, Folder);
            if (originModel.AttrType == nowModel.AttrType)//如果两个类型相同直接赋值
            {
                foreach (var info in data)
                {
                    object temp = null;
                    foreach (var item in info.GetType().GetProperties())
                    {
                        if (originModel.AttrName == item.Name)
                        {
                            temp = item.GetValue(info, null);
                        }
                        if (nowModel.AttrName == item.Name)
                        {
                            info.GetType().GetProperty(item.Name).SetValue(info, temp, null);
                        }
                    }
                }
            }
            else
            {
                foreach (var info in data)
                {
                    object temp = null;
                    foreach (var item in info.GetType().GetProperties())
                    {
                        if (originModel.AttrName == item.Name)
                        {
                            temp = item.GetValue(info, null);
                        }
                        if (nowModel.AttrName == item.Name)
                        {
                            #region ListAndList
                            if (originModel.AttrType.Contains("Generic.List") && nowModel.AttrType.Contains("Generic.List"))//如果两个都是List类型，如果字段名和类型都相同，则赋值
                            {
                                if (temp != null)
                                {
                                    string nowTName = nowModel.AttrType.Substring(nowModel.AttrType.IndexOf('[') + 1);
                                    nowTName = nowTName.Remove(nowTName.IndexOf(']'));
                                    string originTName = originModel.AttrType.Substring(originModel.AttrType.IndexOf('[') + 1);
                                    originTName = originTName.Remove(originTName.IndexOf(']'));
                                    var tt = originTName.Split('.');
                                    originTName = tt[tt.Length - 1];
                                    info.GetType().GetProperty(item.Name).SetValue(info, ListAndList(JsonMapper.ToJson(temp), item.PropertyType, nowTName, originTName), null);
                                }
                            }
                            #endregion
                            #region ModelAndModel
                            else if (originModel.AttrType.Contains("Models") && nowModel.AttrType.Contains("Models"))//如果两个都是实体类，如果字段名和类型都相同，则赋值
                            {
                                if (temp != null)
                                {
                                    string originTName = originModel.AttrType.Substring(originModel.AttrType.IndexOf('[') + 1);
                                    originTName = originTName.Remove(originTName.IndexOf(']'));
                                    var tt = originTName.Split('.');
                                    originTName = tt[tt.Length - 1];
                                    info.GetType().GetProperty(item.Name).SetValue(info, ModelAndModel(JsonMapper.ToJson(temp), item.PropertyType, originTName), null);
                                }
                            }
                            #endregion
                            #region ListAndModel
                            else if (originModel.AttrType.Contains("Generic.List") && nowModel.AttrType.Contains("Models"))
                            {
                                if (temp != null)
                                {
                                    string originTName = originModel.AttrType.Substring(originModel.AttrType.IndexOf('[') + 1);
                                    originTName = originTName.Remove(originTName.IndexOf(']'));
                                    var tt = originTName.Split('.');
                                    originTName = tt[tt.Length - 1];
                                    info.GetType().GetProperty(item.Name).SetValue(info, ListAndModel(JsonMapper.ToJson(temp), item.PropertyType, originTName), null);
                                }
                            }
                            #endregion
                            #region ModelAndList
                            else if (originModel.AttrType.Contains("Models") && nowModel.AttrType.Contains("Generic.List"))
                            {
                                if (temp != null)
                                {
                                    string nowTName = nowModel.AttrType.Substring(nowModel.AttrType.IndexOf('[') + 1);
                                    nowTName = nowTName.Remove(nowTName.IndexOf(']'));
                                    string originTName = originModel.AttrType.Substring(originModel.AttrType.IndexOf('[') + 1);
                                    originTName = originTName.Remove(originTName.IndexOf(']'));
                                    var tt = originTName.Split('.');
                                    originTName = tt[tt.Length - 1];
                                    info.GetType().GetProperty(item.Name).SetValue(info, ModelAndList(JsonMapper.ToJson(temp), item.PropertyType, nowTName, originTName), null);
                                }
                            }
                            #endregion
                            #region 继承IConvertible的
                            else if ((originModel.AttrType.Contains("String") || originModel.AttrType.Contains("Int") || originModel.AttrType.Contains("Single"))
                                                    && typeof(IConvertible).IsAssignableFrom(item.PropertyType)) //常见类型。如string、int、bool等 继承IConvertible的
                            {
                                info.GetType().GetProperty(item.Name).SetValue(info, Convert.ChangeType(temp, item.PropertyType), null);
                            }
                            #endregion
                        }
                    }
                }
            }
            SaveTxtFile(data.ListToJson(), FileName, Folder);
        }

        /// <summary>
        /// 对比两个泛型。将旧泛型数据填到新的泛型里
        /// </summary>
        /// <param name="originJson">原来泛型的Json字符串</param>
        /// <param name="nowType">当前泛型类型</param>
        /// <param name="nowTName">现在的泛型实体名称</param>
        /// <param name="originTName">原来的泛型实体名称</param>
        /// <returns></returns>
        private static object ListAndList(string originJson, Type nowType, string nowTName, string originTName)
        {
            var tables = GetTableStructData().FindAll(a => a.ClassName == originTName);
            var list = nowType.Assembly.CreateInstance(nowType.FullName) as IList;
            Type modelT = Type.GetType(nowTName);
            var t = Activator.CreateInstance(modelT);
            var originData = JsonMapper.ToObject(originJson);

            foreach (JsonData data in originData)
            {
                foreach (var table in tables)
                {
                    foreach (var info in modelT.GetProperties())
                    {
                        if (table.AttrName == info.Name && table.AttrType == info.PropertyType.ToString())
                        {
                            object obj = null;
                            //取得object对象中此属性的值
                            object val = null;
                            if (data.GetJsonType() != JsonType.None)
                            {
                                if (((IDictionary)data).Contains(info?.Name))
                                {
                                    val = data[info?.Name];
                                }
                            }
                            if (val != null)
                            {
                                if (table.AttrType == "System.String")
                                {
                                    obj = val.ToString();
                                }
                                else if (table.AttrType == "System.Int32" || table.AttrType == "System.Single")//数字类型
                                {
                                    obj = Convert.ToInt32(val.ToString());
                                }
                                else if (table.AttrType.Contains("Generic.List"))//List类型
                                {
                                    Type modelType = Type.GetType(table.AttrType);
                                    string tName = table.AttrType.Substring(table.AttrType.IndexOf('[') + 1);
                                    tName = tName.Remove(tName.IndexOf(']'));
                                    obj = ToType(val, modelType, tName);
                                }
                                else if (table.AttrType.Contains("Models"))//普通Model
                                {
                                    Type modelType = Type.GetType(table.AttrType);
                                    obj = ToType(val.ObjectToJson(), modelType);
                                }
                                t.GetType().GetProperty(info.Name).SetValue(t, obj, null);
                            }
                        }
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        /// 对比两个实体。将旧实体数据填到新的实体里
        /// </summary>
        /// <param name="originJson">原来对象的Json字符串</param>
        /// <param name="nowType">当前对象类型</param>
        /// <param name="originTName">原来对象的实体名称</param>
        /// <returns></returns>
        private static object ModelAndModel(string originJson, Type nowType, string originTName)
        {
            var tables = GetTableStructData().FindAll(a => a.ClassName == originTName);
            var t = Activator.CreateInstance(nowType);
            var data = JsonMapper.ToObject(originJson);

            foreach (var table in tables)
            {
                foreach (var info in nowType.GetProperties())
                {
                    if (table.AttrName == info.Name && table.AttrType == info.PropertyType.ToString())
                    {
                        object obj = null;
                        //取得object对象中此属性的值
                        object val = null;
                        if (data.GetJsonType() != JsonType.None)
                        {
                            if (((IDictionary)data).Contains(info?.Name))
                            {
                                val = data[info?.Name];
                            }
                        }
                        if (val != null)
                        {
                            if (table.AttrType == "System.String")
                            {
                                obj = val.ToString();
                            }
                            else if (table.AttrType == "System.Int32" || table.AttrType == "System.Single")//数字类型
                            {
                                obj = Convert.ToInt32(val.ToString());
                            }
                            else if (table.AttrType.Contains("Generic.List"))//List类型
                            {
                                Type modelType = Type.GetType(table.AttrType);
                                string tName = table.AttrType.Substring(table.AttrType.IndexOf('[') + 1);
                                tName = tName.Remove(tName.IndexOf(']'));
                                obj = ToType(val, modelType, tName);
                            }
                            else if (table.AttrType.Contains("Models"))//普通Model
                            {
                                Type modelType = Type.GetType(table.AttrType);
                                obj = ToType(val.ObjectToJson(), modelType);
                            }
                            t.GetType().GetProperty(info.Name).SetValue(t, obj, null);
                        }
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// 对比List填充到Model中
        /// </summary>
        /// <param name="originJson">原来对象的Json字符串</param>
        /// <param name="nowType">当前对象类型</param>
        /// <param name="originTName">原来对象的实体名称</param>
        /// <returns></returns>
        private static object ListAndModel(string originJson, Type nowType, string originTName)
        {
            var tables = GetTableStructData().FindAll(a => a.ClassName == originTName);
            var t = Activator.CreateInstance(nowType);
            var data = JsonMapper.ToObject(originJson);
            foreach (var table in tables)
            {
                foreach (var info in nowType.GetProperties())
                {
                    if (table.AttrName == info.Name && table.AttrType == info.PropertyType.ToString())
                    {
                        object obj = null;
                        //取得object对象中此属性的值
                        object val = null;
                        if (data[0].GetJsonType() != JsonType.None)
                        {
                            if (((IDictionary)data[0]).Contains(info?.Name))
                            {
                                val = data[0][info?.Name];
                            }
                        }
                        if (val != null)
                        {
                            if (table.AttrType == "System.String")
                            {
                                obj = val.ToString();
                            }
                            else if (table.AttrType == "System.Int32" || table.AttrType == "System.Single")//数字类型
                            {
                                obj = Convert.ToInt32(val.ToString());
                            }
                            else if (table.AttrType.Contains("Generic.List"))//List类型
                            {
                                Type modelType = Type.GetType(table.AttrType);
                                string tName = table.AttrType.Substring(table.AttrType.IndexOf('[') + 1);
                                tName = tName.Remove(tName.IndexOf(']'));
                                obj = ToType(val, modelType, tName);
                            }
                            else if (table.AttrType.Contains("Models"))//普通Model
                            {
                                Type modelType = Type.GetType(table.AttrType);
                                obj = ToType(val.ObjectToJson(), modelType);
                            }
                            t.GetType().GetProperty(info.Name).SetValue(t, obj, null);
                        }
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// 对比实体和泛型。将旧泛型数据填到新的泛型里
        /// </summary>
        /// <param name="originJson">原来实体的Json字符串</param>
        /// <param name="nowType">当前泛型类型</param>
        /// <param name="nowTName">现在的泛型实体名称</param>
        /// <param name="originTName">原来的泛型实体名称</param>
        /// <returns></returns>
        private static object ModelAndList(string originJson, Type nowType, string nowTName, string originTName)
        {
            var tables = GetTableStructData().FindAll(a => a.ClassName == originTName);
            var list = nowType.Assembly.CreateInstance(nowType.FullName) as IList;
            Type modelT = Type.GetType(nowTName);
            var t = Activator.CreateInstance(modelT);
            var data = JsonMapper.ToObject(originJson);

            foreach (var table in tables)
            {
                foreach (var info in modelT.GetProperties())
                {
                    if (table.AttrName == info.Name && table.AttrType == info.PropertyType.ToString())
                    {
                        object obj = null;
                        //取得object对象中此属性的值
                        object val = null;
                        if (data.GetJsonType() != JsonType.None)
                        {
                            if (((IDictionary)data).Contains(info?.Name))
                            {
                                val = data[info?.Name];
                            }
                        }
                        if (val != null)
                        {
                            if (table.AttrType == "System.String")
                            {
                                obj = val.ToString();
                            }
                            else if (table.AttrType == "System.Int32" || table.AttrType == "System.Single")//数字类型
                            {
                                obj = Convert.ToInt32(val.ToString());
                            }
                            else if (table.AttrType.Contains("Generic.List"))//List类型
                            {
                                Type modelType = Type.GetType(table.AttrType);
                                string tName = table.AttrType.Substring(table.AttrType.IndexOf('[') + 1);
                                tName = tName.Remove(tName.IndexOf(']'));
                                obj = ToType(val, modelType, tName);
                            }
                            else if (table.AttrType.Contains("Models"))//普通Model
                            {
                                Type modelType = Type.GetType(table.AttrType);
                                obj = ToType(val.ObjectToJson(), modelType);
                            }
                            t.GetType().GetProperty(info.Name).SetValue(t, obj, null);
                        }
                    }
                }
            }
            list.Add(t);
            return list;
        }
        #endregion
    }
}
