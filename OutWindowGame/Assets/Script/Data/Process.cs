using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 游戏进程数据
/// </summary>
public class Process
{
    /// <summary>
    /// 当前关卡
    /// </summary>
    public int Level { get; set; }
    /// <summary>
    /// 复活次数
    /// </summary>
    public int Revive { get; set; }
    /// <summary>
    /// 死亡次数
    /// </summary>
    public int Die { get; set; }
    /// <summary>
    /// 游戏时长 /S
    /// </summary>
    public int InTime { get; set; }
    /// <summary>
    /// 完成关卡和通关星级（字符串拼接）
    /// </summary>
    public string CollLevel { get; set; }
}

