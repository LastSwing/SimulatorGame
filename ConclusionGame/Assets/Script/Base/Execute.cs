using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 执行类
/// </summary>
public class Execute
{
    /// <summary>
    /// 执行ID
    /// </summary>
    public Guid Guid { get; set; }
    /// <summary>
    /// 执行父ID
    /// </summary>
    public Guid CGuid { get; set; }
    /// <summary>
    /// 执行步骤
    /// </summary>
    public FileidType Fileidtype { get; set; }
    /// <summary>
    /// 执行状态
    /// </summary>
    public bool Update { get; set; }
    /// <summary>
    /// 执行过ID
    /// </summary>
    public List<Guid> FormerlyID { get; set; }

    /// <summary>
    /// 是否重复执行
    /// </summary>
    public bool Repeat { get; set; }
}
