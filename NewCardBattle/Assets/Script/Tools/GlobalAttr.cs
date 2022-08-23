using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Tools
{
    public static class GlobalAttr
    {
        /// <summary>
        /// 所有资源文件路径
        /// </summary>
        public static string AllResourcesFileName = "AllResourcesPath";

        /// <summary>
        /// 全局角色卡池文件的名称
        /// </summary>
        public static string GlobalPlayerCardPoolFileName = "GlobalPlayerCardPools";

        /// <summary>
        /// 游戏全局卡池文件的名称
        /// </summary>
        public static string GlobalCardPoolFileName = "GlobalCardPools";

        /// <summary>
        /// 全局玩家角色池文件名称
        /// </summary>
        public static string GlobalPlayerRolePoolFileName = "GlobalPlayerRolePools";

        /// <summary>
        /// 全局AI角色池文件名称
        /// </summary>
        public static string GlobalAIRolePoolFileName = "GlobalAIRolePools";

        /// <summary>
        /// 全局角色
        /// </summary>
        public static string GlobalRoleFileName = "GlobalRole";

        /// <summary>
        /// 当局游戏内玩家角色文件名称
        /// </summary>
        public static string CurrentPlayerRoleFileName = "CurrentPlayerRole";

        /// <summary>
        /// 当前玩家卡池
        /// </summary>
        public static string CurrentCardPoolsFileName = "CurrentPlayerCardPools";

        /// <summary>
        /// 未使用卡池文件名称
        /// </summary>
        public static string CurrentUnUsedCardPoolsFileName = "CurrentUnUsedCardPools";

        /// <summary>
        /// 已使用卡池文件名称
        /// </summary>
        public static string CurrentUsedCardPoolsFileName = "CurrentUsedCardPools";

        /// <summary>
        /// 攻击栏卡池文件名称
        /// </summary>
        public static string CurrentATKBarCardPoolsFileName = "CurrentATKBarCardPools";

        /// <summary>
        /// 当局游戏内Ai角色文件名称
        /// </summary>
        public static string CurrentAIRoleFileName = "CurrentAiRole";

        /// <summary>
        /// 当前AI牌池
        /// </summary>
        public static string CurrentAiCardPoolsFileName = "CurrentAiCardPools";

        /// <summary>
        /// 当前AI攻击牌池
        /// </summary>
        public static string CurrentAIATKCardPoolsFileName = "CurrentAiATKCardPools";

        #region Map
        /// <summary>
        /// 当前角色在地图上的位置
        /// </summary>
        public static string CurrentMapLocationFileName = "CurrentMapLocation";

        /// <summary>
        /// 当前地图的所有战斗点
        /// </summary>
        public static string CurrentMapCombatPointFileName = "CurrentMapCombatPoint";

        /// <summary>
        /// 当前地图的路线
        /// </summary>
        public static string CurrentMapPathFileName = "CurrentMapPath";
        #endregion

        #region Adventure
        /// <summary>
        /// 冒险文件名称
        /// </summary>
        public static string GlobalAdventureFileName = "GlobalAdventure";
        #endregion

        #region Card
        /// <summary>
        /// buff效果
        /// </summary>
        public static string BUFFEffectFileName = "BUFFEffect";

        /// <summary>
        /// buff图片路径
        /// </summary>
        public static string BUFFUrlFileName = "BUFFUrl";

        /// <summary>
        /// 卡效果类型
        /// </summary>
        public static string EffectTypeFileName = "EffectType";

        /// <summary>
        /// 卡触发条件
        /// </summary>
        public static string TriggerConditionFileName = "TriggerCondition";
        /// <summary>
        /// 卡触发类型
        /// </summary>
        public static string TriggerStateFileName = "TriggerState";

        #endregion

        /// <summary>
        /// 所有表属性
        /// </summary>
        public static string AllTablesAttrFlieName = "AllTablesAttr";
    }
}
