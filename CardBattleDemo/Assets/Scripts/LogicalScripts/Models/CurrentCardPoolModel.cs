using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.LogicalScripts.Models
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
    }
}
