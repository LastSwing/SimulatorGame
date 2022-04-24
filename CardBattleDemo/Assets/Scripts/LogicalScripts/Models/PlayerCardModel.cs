using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.LogicalScripts.Models
{
    /// <summary>
    /// 玩家卡池
    /// </summary>
    public class PlayerCardModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 卡类型
        /// 0物理卡、1武技卡、2功能卡
        /// </summary>
        public int CardType { get; set; }

        /// <summary>
        /// 效果类型
        /// 0攻击、1防御、2血量消耗、3血量恢复、4能量消耗、5能量恢复、6暴击状态、7免疫状态、8愤怒、9虚弱、10满血暴击、11击杀恢复、
        /// </summary>
        public float StateType { get; set; }

        /// <summary>
        /// 卡牌范围
        /// 0任意一个敌人、1第一个敌人
        /// </summary>
        public float CardScope { get; set; }

        /// <summary>
        /// 是否有AOE
        /// 0、没有1、所有敌人、2当前的和后面的
        /// </summary>
        public float HasAOE { get; set; }

        /// <summary>
        /// 卡牌名称
        /// </summary>
        public string CardName { get; set; }

        /// <summary>
        /// 卡牌图片路径
        /// </summary>
        public string CardUrl { get; set; }

        /// <summary>
        /// 卡牌效果详情
        /// </summary>
        public string CardDetail { get; set; }

        /// <summary>
        /// 玩家使用卡牌的消耗
        /// </summary>
        public float Consume { get; set; }

        /// <summary>
        /// 效果值（如血量恢复、愤怒层数、攻击力、护甲）
        /// </summary>
        public float Effect { get; set; }

    }
}
