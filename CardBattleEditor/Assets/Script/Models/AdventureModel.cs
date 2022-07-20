using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Models
{
    /// <summary>
    /// 冒险类
    /// </summary>
    public class AdventureModel 
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 冒险名称。同一个冒险下有多个事件，每个事件以名称关联，以ID区分
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 主背景图路径
        /// </summary>
        public string MainBGIUrl { get; set; }

        /// <summary>
        /// 事件详情
        /// </summary>
        public string EventDetail { get; set; }

        /// <summary>
        /// 事件效果类型
        /// 0返回。不继续冒险，1接下条事件、2奖励
        /// </summary>
        public int EventEffectType { get; set; }

        /// <summary>
        /// 奖励类型、0、无、1减血、2加血、3减银币、4加银币、5减最大生命值、6加最大生命值、7战斗、8黑卡、9随机普通卡、10带有黑特效的卡、11所有卡随机
        /// </summary>
        public int RewardType { get; set; }

        /// <summary>
        /// 效果名称
        /// </summary>
        public string EventEffectName { get; set; }

        /// <summary>
        /// 效果图路径
        /// </summary>
        public string EventEffectImgUrl { get; set; }

        /// <summary>
        /// 效果值
        /// </summary>
        public int EventEffectValue { get; set; }

        /// <summary>
        /// 事件层级,在同一层数的事件，同时展示
        /// </summary>
        public int EventLayerLevel { get; set; }

        /// <summary>
        /// 层级排序，小在右
        /// </summary>
        public int EventLayerLevelSort { get; set; }

        /// <summary>
        /// 相关联的事件ID
        /// </summary>
        public string RelatedEventIDs { get; set; }
    }
}
