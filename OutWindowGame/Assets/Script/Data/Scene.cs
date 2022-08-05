using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Scene
{
    /// <summary>
    /// 场景ID
    /// </summary>
    public int SceneID { get; set; }
    /// <summary>
    /// 场景背景图片
    /// </summary>
    public string ImageName { get; set; }
    /// <summary>
    /// 场景重力改变 加算
    /// </summary>
    public float Gravity { get; set; }
    /// <summary>
    /// 场景空气阻力改变
    /// </summary>
    public float AirDrag { get; set; }
    /// <summary>
    /// 场景质量改变
    /// </summary>
    public float Mass { get; set; }
    /// <summary>
    /// 场景背景音乐
    /// </summary>
    public string MusicName { get; set; }
}

