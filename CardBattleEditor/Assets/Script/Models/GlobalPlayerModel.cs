using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Models
{
    /// <summary>
    /// 玩家类
    /// </summary>
    public class GlobalPlayerModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 出战的角色ID
        /// </summary>
        public string CurrentRoleID { get; set; }

        /// <summary>
        /// 所拥有的的角色
        /// </summary>
        public string RoleList { get; set; }

        /// <summary>
        /// 所拥有的技能
        /// </summary>
        public string SkillList { get; set; }

        /// <summary>
        /// 财富值
        /// </summary>
        public float Wealth { get; set; }

        /// <summary>
        /// 累计值，达到累计值可解锁新的卡牌
        /// 每次击杀存储 4*关卡等级
        /// </summary>
        public int AccumulateValue { get; set; }

        /// <summary>
        /// 所需达到的累计值
        /// 初始80
        /// </summary>
        public int MaxAccumulate { get; set; }
    }
}
