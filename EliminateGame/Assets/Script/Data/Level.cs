using System.Collections.Generic;

public class Level
{
    /// <summary>
    /// 关卡数
    /// </summary>
    public int LevelNum { get; set; }
    /// <summary>
    /// 关卡需要分数
    /// </summary>
    public int Score { get; set; }
    /// <summary>
    /// 关卡类型 0: 1*3，1:  2*2，2:  2*3，3:  3*3，4:  4*4
    /// </summary>
    public int LevelType { get; set; }
    /// <summary>
    /// 子集
    /// </summary>
    public List<Seed> Seeds { get; set; }
}
