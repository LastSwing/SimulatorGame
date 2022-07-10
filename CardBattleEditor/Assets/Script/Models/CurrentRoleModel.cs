using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Models
{
    /// <summary>
    /// 游戏内的角色
    /// </summary>
    public class CurrentRoleModel
    {
        /// <summary>
        /// 角色主键
        /// </summary>
        public string RoleID { get; set; }

        /// <summary>
        /// 角色属性
        /// 0玩家、1AI
        /// </summary>
        public int RoleType { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色图片路径
        /// </summary>
        public string RoleImgUrl { get; set; }

        /// <summary>
        /// 当前血量
        /// </summary>
        public float HP { get; set; }

        /// <summary>
        /// 最大血量
        /// </summary>
        public float MaxHP { get; set; }

        /// <summary>
        /// 财富值
        /// </summary>
        public float Wealth { get; set; }

        /// <summary>
        /// 当前能量值
        /// </summary>
        public int Energy { get; set; }

        /// <summary>
        /// 最大能量值
        /// </summary>
        public int MaxEnergy { get; set; }

        /// <summary>
        /// 护甲值
        /// </summary>
        public int Armor { get; set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public float CurrentState { get; set; }

        /// <summary>
        /// 所拥有的卡牌
        /// </summary>
        public string CardListStr { get; set; }

        /// <summary>
        /// 头像Url
        /// </summary>
        public string HeadPortraitUrl { get; set; }

        /// <summary>
        /// AI按等级出现
        /// </summary>
        public int AILevel { get; set; }

        /// <summary>
        /// 额外奖励
        /// </summary>
        public int ExtraReward { get; set; }

        /// <summary>
        /// 冒险ID
        /// </summary>
        public string AdventureIds { get; set; }
    }
}
