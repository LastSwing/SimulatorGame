using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Models
{
    /// <summary>
    /// 游戏内卡池
    /// </summary>
    public class CurrentCardPoolModel : CardPoolModel
    {
        /// <summary>
        /// 熟练度
        /// </summary>
        public int Proficiency { get; set; }

        /// <summary>
        /// 升级次数
        /// </summary>
        public int UpgradeCount { get; set; }
    }
}
