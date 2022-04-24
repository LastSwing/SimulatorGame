using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.LogicalScripts.Models
{
    /// <summary>
    /// 怪物属性
    /// </summary>
    public class MonsterModel
    {
        /// <summary>
        /// 血量
        /// </summary>
        public float HP { get; set; }

        /// <summary>
        /// 初始攻击力
        /// </summary>
        public float ATK { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public float Level { get; set; }

        /// <summary>
        /// 野怪类型
        /// 1战士、2坦克、3辅助、4Boss
        /// </summary>
        public int Type { get; set; }
    }
}
