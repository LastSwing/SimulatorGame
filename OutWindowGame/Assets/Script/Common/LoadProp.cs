using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// 道具
/// </summary>
public class LoadProp
{
    public Dictionary<string, MainProp> PropDict;
    public LoadProp()
    {
        PropDict = new Dictionary<string, MainProp>();
        PropDict.Add("PaperPlane", new MainProp(new Vector2(-6f, -133f), false,false,false));//纸飞机
        PropDict.Add("Property", new MainProp(new Vector2(8f, 79f), true, false,false));//降落伞
        PropDict.Add("Rocket", new MainProp(new Vector2(-36f, 4f), true, false,false));//推进器
        PropDict.Add("UAV", new MainProp(new Vector2(1f, 68f), true, false,false));//无人机
    }
}
/// <summary>
/// 道具主体
/// </summary>
public class MainProp
{
    public MainProp(Vector2 vector,bool isrocker,bool Isoverlay,bool crash)
    {
        Loadlocation = vector;
        IsRocker = isrocker;
        IsOverlay = Isoverlay;
        Crash = crash;
    }


    /// <summary>
    /// 道具处于角色的位置
    /// </summary>
    public Vector2 Loadlocation { get; set; }
    /// <summary>
    /// 是否使用摇杆
    /// </summary>
    public bool IsRocker { get; set; }
    /// <summary>
    /// 是否与其他道具力叠加
    /// </summary>
    public bool IsOverlay { get; set; }
    /// <summary>
    /// 是否突破障碍物
    /// </summary>
    public bool Crash { get; set; }
}