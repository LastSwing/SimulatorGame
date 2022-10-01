
public class Seed
{
    /// <summary>
    /// 数字
    /// </summary>
    public int Num { get; set; }
    /// <summary>
    /// 0+ 1- 2* 3/
    /// </summary>
    public int NumType { get; set; }
    /// <summary>
    /// 位置
    /// </summary>
    public int NumLocation { get; set; }
    /// <summary>
    /// 是否为出口
    /// </summary>
    public bool IsExit { get; set; }
    /// <summary>
    /// 出口位置 0上1下2左3右
    /// </summary>
    public int ExitLocation { get; set; }

    /// <summary>
    /// 是否为角色
    /// </summary>
    public bool IsRole { get; set; }

}

