using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Models
{
    public class CardPoolModel
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// 卡类型
        /// 0攻击卡、1功能卡、2黑卡
        /// </summary>
        public int CardType { get; set; }

        /// <summary>
        /// 效果类型
        /// 存Key在表里找到对应的效果
        /// </summary>
        public int EffectType { get; set; }

        /// <summary>
        /// 攻击次数
        /// </summary>
        public int AtkNumber { get; set; }

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
        /// 该卡所属Ai或玩家
        /// 0玩家、1AI
        /// </summary>
        public int PlayerOrAI { get; set; }

        /// <summary>
        /// 稀有度、用于判断什么时候出现在奖励牌池中
        /// 每过一关，获得稀有卡牌概率提升
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
        /// 触发条件
        /// </summary>
        public int TriggerCondition { get; set; }

        /// <summary>
        /// 卡牌使用后触发状态
        /// 存Key在表里找对应的值
        /// </summary>
        public int TriggerState { get; set; }

        /// <summary>
        /// 触发值
        /// </summary>
        public int TriggerValue { get; set; }

        /// <summary>
        /// 触发条件
        /// 存Key在表里找对应的值
        /// </summary>
        public int TriggerCondition2 { get; set; }

        /// <summary>
        /// 卡牌使用后触发状态
        /// 存Key在表里找对应的值
        /// </summary>
        public int TriggerState2 { get; set; }

        /// <summary>
        /// 触发值
        /// </summary>
        public int TriggerValue2 { get; set; }

    }
}
