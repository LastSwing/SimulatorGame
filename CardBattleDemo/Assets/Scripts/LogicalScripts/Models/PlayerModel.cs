using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.LogicalScripts.Models
{
    /// <summary>
    /// 玩家属性
    /// </summary>
    public class PlayerModel
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
        /// 财富值
        /// </summary>
        public float Wealth { get; set; }

        /// <summary>
        /// 能量值
        /// </summary>
        public float Energy { get; set; }

        /// <summary>
        /// 战力掌握 %
        /// </summary>
        public float Grasp { get; set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public float CurrentState { get; set; }


    }
}
