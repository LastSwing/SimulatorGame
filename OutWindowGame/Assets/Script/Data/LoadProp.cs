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
        PropDict.Add("PaperPlane", new MainProp(new Vector2(-6f, -75), false,false,false,false, true,6,1,0, "_zhifeiji.png"));//纸飞机
        PropDict.Add("Property", new MainProp(new Vector2(8f, 79f), true, false,false, false, true,3,1,0.05f, "_parachute.png"));//降落伞
        PropDict.Add("Rocket", new MainProp(new Vector2(-36f, 4f), true, false,false, false, true,10,1,50, "_uijinqi.png"));//推进器
        PropDict.Add("UAV", new MainProp(new Vector2(1f, 68f), true, false,false, false, true,0,1,500, "_luoxuanjiang.png"));//无人机
        PropDict.Add("Reversal", new MainProp(new Vector2(0, 0), true, false, false,  true, false,-1,-1,-1, "fanzhuan.png"));//重力反转
        PropDict.Add("BigRole", new MainProp(new Vector2(0, 0), false, false, false, false, false,-1f,-1f,-1f, "largen.png"));//变大
        //PropDict.Add("HookRope", new MainProp(new Vector2(45, 30), true, false, false, false, false, 1, -1f, -1f, "gousuo.png"));//钩索
    }
}
/// <summary>
/// 道具主体
/// </summary>
public class MainProp
{
    /// <summary>
    /// 道具(道具处于角色的位置,是否使用摇杆,是否与其他道具力叠加,是否突破障碍物,是否为buff,是否跟着角色移动,重力,质量,空气阻力)
    /// </summary>
    public MainProp(Vector2 vector,bool isrocker,bool Isoverlay,bool crash,bool isBuff,bool ismove,float gravity,float mass,float angularDrag,string image)
    {
        Loadlocation = vector;
        IsRocker = isrocker;
        IsOverlay = Isoverlay;
        Crash = crash;
        IsBuff = isBuff;
        IsMove = ismove;
        Gravity = gravity;
        Mass = mass;
        AngularDrag = angularDrag;
        Image = image;
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
    /// <summary>
    /// 是否为buff
    /// </summary>
    public bool IsBuff { get; set; }
    /// <summary>
    /// 是否跟着角色移动
    /// </summary>
    public bool IsMove { get; set; }
    /// <summary>
    /// 改变重力
    /// </summary>
    public float Gravity { get; set; }
    /// <summary>
    /// 改变质量
    /// </summary>
    public float Mass { get; set; }
    /// <summary>
    /// 改变空气阻力
    /// </summary>
    public float AngularDrag { get; set; }
    /// <summary>
    /// 道具显示图片
    /// </summary>
    public string Image { get; set; }
}