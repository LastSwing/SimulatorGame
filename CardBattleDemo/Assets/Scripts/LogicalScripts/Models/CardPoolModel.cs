using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.LogicalScripts.Models
{
    /// <summary>
    /// 卡池
    /// </summary>
    public class CardPoolModel
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
        /// 0攻击、1防御、2血量消耗、3血量恢复、4能量消耗、5能量恢复、6暴击状态、7免疫状态、8愤怒、9虚弱、10二连击、11破甲攻击
        /// </summary>
        public int StateType { get; set; }

        /// <summary>
        /// 是否有AOE
        /// 0、没有1、所有敌人、2当前的和后面的
        /// </summary>
        public int HasAOE { get; set; }

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
        public int Consume { get; set; }

        /// <summary>
        /// 效果值（如血量恢复、愤怒层数、攻击力、护甲）
        /// </summary>
        public float Effect { get; set; }

        /// <summary>
        /// 玩家或AI的牌池
        /// 0玩家、1AI
        /// </summary>
        public int PlayerOrAI { get; set; }

        /// <summary>
        /// 卡牌等级、用于判断什么时候出现在牌池中
        /// </summary>
        public int CardLevel { get; set; }

        /// <summary>
        /// 卡牌价格
        /// </summary>
        public float CardPrice { get; set; }

        /// <summary>
        /// 是否商店展示
        /// 0否、1是
        /// </summary>
        public int HasShoppingShow { get; set; }

        /// <summary>
        /// 是否是BUFF卡（黑卡）
        /// 0否、1是
        /// </summary>
        public int HasDeBuff { get; set; }

        /// <summary>
        /// 触发状态
        /// 0无、1敌人满血、2击杀恢复、3敌人是人类、4敌人有护甲、5敌人血量到达斩杀线、6使用后移除
        /// </summary>
        public int TriggerState { get; set; }

        /// <summary>
        /// 触发值
        /// </summary>
        public int TriggerValue { get; set; }

        /// <summary>
        /// Ai攻击顺序（在Ai有多次攻击时按排序攻击）
        /// </summary>
        public int AiAtkSort { get; set; }

    }
}
