using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 关卡详细数据
/// </summary>
public class LevelDetail
{
    /// <summary>
    /// 所属关卡
    /// </summary>
    public int LevelID { get; set; }
    /// <summary>
    /// 位置
    /// </summary>
    public Vector2 Location { get; set; }
    /// <summary>
    /// 宽高
    /// </summary>
    public Vector2 Size { get; set; }
    /// <summary>
    /// 旋转
    /// </summary>
    public Vector3 Rotation { get; set; }
    /// <summary>
    /// 是否出现在视野内
    /// </summary>
    public bool IsView { get; set; }
    /// <summary>
    /// 自身运动是否失效
    /// </summary>
    public bool IsMotion { get; set; }
    /// <summary>
    /// 层级
    /// </summary>
    public int Hierarchy { get; set; }
    /// <summary>
    /// 是否隐藏
    /// </summary>
    public bool IsHide { get; set; }
    /// <summary>
    /// 碰撞后重力改变
    /// </summary>
    public float Gravity { get; set; }
    /// <summary>
    /// 碰撞后空气阻力改变
    /// </summary>
    public float AirDrag { get; set; }
    /// <summary>
    /// 碰撞后质量改变
    /// </summary>
    public float Mass { get; set; }
    /// <summary>
    /// 物体名称
    /// </summary>
    public string DetailName { get; set; }
    /// <summary>
    /// 物体类型 0障碍物1地形
    /// </summary>
    public int DetailType { get; set; }
}
