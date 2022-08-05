using UnityEngine;
/// <summary>
/// 关卡
/// </summary>
public class Level
{
    /// <summary>
    /// 关卡ID-第几关
    /// </summary>
    public int LevelNum { get; set; }
    /// <summary>
    /// 是否为特殊关卡
    /// </summary>
    public bool Special { get; set; }
    /// <summary>
    /// 关卡宽高
    /// </summary>
    public Vector2 Size { get; set; }
    /// <summary>
    /// 场景ID   
    /// </summary>
    public int SceneID { get; set; }
    /// <summary>
    /// 关卡使用道具
    /// </summary>
    public string Props { get; set; }
}
