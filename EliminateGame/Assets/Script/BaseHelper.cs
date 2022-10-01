using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class BaseHelper
{
    /// <summary>
    /// 遍历去子物体下的所有物体
    /// </summary>
    /// <param name="transform">物体</param>
    /// <param name="Active">true 只取SetActive为true的物体; false 全部取包括SetActive为false的物体</param>
    /// <param name="IsChild">是否取子物体下的子物体</param>
    /// <param name="Keyword">关键词模糊查询，空则取全部物体</param>
    /// <returns></returns>
    public static List<GameObject> GetAllSceneObjects(Transform transform, bool Active, bool IsChild, string Keyword)
    {
        List<GameObject> games = new List<GameObject>();
        //利用for循环 获取物体下的全部子物体
        for (int c = 0; c < transform.childCount; c++)
        {
            if (IsChild)
            {
                //如果子物体下还有子物体 就将子物体传入进行回调查找 直到物体没有子物体为止
                if (transform.GetChild(c).childCount > 0)
                    GetAllSceneObjects(transform.GetChild(c).transform, Active, IsChild, Keyword);
            }
            if (transform.GetChild(c).gameObject.activeSelf || !Active)
            {
                if (Keyword == "" || transform.GetChild(c).gameObject.name.Contains(Keyword))
                    games.Add(transform.GetChild(c).gameObject);
            }
        }
        return games;
    }

    /// <summary>
    /// 生成网格位置
    /// </summary>
    /// <param name="WidthHeight">大网格宽高</param>
    /// <param name="SmallWidthHeight">小网格宽高</param>
    public static List<Vector2> Addlocation(Vector2 WidthHeight, Vector2 SmallWidthHeight,out Vector2 Any)
    {
        List<Vector2> location = new List<Vector2>();
        int heng = (int)Math.Round(WidthHeight.x / SmallWidthHeight.x);//横向数
        int shu = (int)Math.Round(WidthHeight.y / SmallWidthHeight.y);//纵向数
        Any = new Vector2(heng, shu);
        for (int i = shu; i > 0; i--)
        {
            float y = (SmallWidthHeight.y * shu / 2) - (i * SmallWidthHeight.y) + SmallWidthHeight.y / 2;
            for (int j = heng; j > 0; j--)
            {
                float x = (SmallWidthHeight.x * heng / 2) + (SmallWidthHeight.x / 2) - (j * SmallWidthHeight.x);
                location.Add(new Vector2(x, y));
            }
        }
        return location;
    }
}

