using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        #endregion
    }
}
