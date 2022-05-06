using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.LogicalScripts.Models
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
        public List<CurrentRoleModel> RoleList { get; set; }

        /// <summary>
        /// 财富值
        /// </summary>
        public float Wealth { get; set; }
    }
}
