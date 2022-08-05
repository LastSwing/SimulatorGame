using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class BaseHelper
{
    /// <summary>
    /// 将控件添加到父控件
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="prefab"></param>
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
    /// 获取图片并输出成Sprite
    /// </summary>
    /// <param name="vector">图片宽高</param>
    /// <param name="pathName">图片路径</param>
    /// <returns></returns>
    public static Sprite LoadFromImage(Vector2 vector,string pathName)
    {
        Texture2D m_Tex;
        m_Tex = new Texture2D((int)vector.x, (int)vector.y);
        FileStream fileStream = new FileStream(pathName, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        byte[] binary = new byte[fileStream.Length];
        fileStream.Read(binary, 0, (int)fileStream.Length);
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;
        m_Tex.LoadImage(binary);
        return Sprite.Create(m_Tex, new Rect(0, 0, m_Tex.width, m_Tex.height), new Vector2(10, 10));
    }
    /// <summary>
    /// 遍历去子物体下的所有物体
    /// </summary>
    /// <param name="transform">物体</param>
    /// <param name="Active">true 只取SetActive为true的物体; false 全部取包括SetActive为false的物体</param>
    /// <param name="IsChild">是否取子物体下的子物体</param>
    /// <param name="Keyword">关键词模糊查询，空则取全部物体</param>
    /// <returns></returns>
    public static List<GameObject> GetAllSceneObjects(Transform transform,bool Active,bool IsChild,string Keyword)
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
                if(Keyword == "" || transform.GetChild(c).gameObject.name.Contains(Keyword))
                    games.Add(transform.GetChild(c).gameObject);
            }
        }
        return games;
    }

}
