﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.Script
{
    public class Common
    {
        public static int HasAgain;
        /// <summary>
        /// 跳转页面
        /// </summary>
        /// <param name="SceneName">场景名称</param>
        /// <param name="HasAgain">是否重新开始1、不重开，0重开</param>
        public static void SceneJump(string SceneName,int Again= 1)
        {
            HasAgain = Again;
            SceneManager.LoadScene(SceneName);
        }

        /// <summary>
        /// 将object对象转换为实体对象
        /// </summary>
        /// <typeparam name="T">实体对象类名</typeparam>
        /// <param name="asObject">object对象</param>
        /// <returns></returns>
        public static T ConvertObject<T>(object asObject) where T : new()
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

        /// <summary>
        /// Dic转Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static T ConvertModel<T>(Dictionary<string, string> dict)
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
        /// Obj转List<string>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static List<string> ConvertToList(object asObject)
        {
            IList<string> objList = (IList<string>)asObject;
            return objList.ToList();
        }

    }
}
