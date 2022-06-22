using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : SingletonMonoBehaviour<GameObjectPool>
{ 
    /// <summary>
    /// 对象池字典
    /// </summary>
    Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>() { };

    /// <summary>
    /// 从池子得到物体的方法，传递两个参数
    /// </summary>
    /// <param name="go">你需要得到的物体</param>
    /// <param name="parent">你需要放置的节点</param>
    /// <returns></returns>
    public GameObject GetObject(GameObject go,Transform parent)
    {
        string key = go.name;
        GameObject rongqi; //你用来取物体的容器 
        
        //下面分三种情况来分析  
        if (pool.ContainsKey(key) && pool[key].Count > 0)//如果池存在，池里有东西  
        {
            //直接拿走池里面的第一个  
            rongqi = pool[key][0];
            pool[key].RemoveAt(0);//把第一个位置释放  
        }
        else if (pool.ContainsKey(key) && pool[key].Count <= 0)//池存在，池里没东西  
        {
            //生成一个
            rongqi = Instantiate(go, parent) as GameObject;
        }
        else  //池子不存在  
        {
            //创建池子并生成一个  
            rongqi = Instantiate(go, parent) as GameObject;
            pool.Add(key, new List<GameObject>() { });
        }
        
        return rongqi;
    }

    //放回池子中的方法  
    public void IntoPool(GameObject go)
    {
        string key = go.name;
        pool[key].Add(go);
        go.SetActive(false);
    }

}
