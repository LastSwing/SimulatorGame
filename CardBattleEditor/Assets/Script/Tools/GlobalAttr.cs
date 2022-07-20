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
        /// 角色全局卡池文件的名称
        /// </summary>
        public static string GlobalPlayerCardPoolFileName = "GlobalPlayerCardPools";

        /// <summary>
        /// 游戏全局卡池文件的名称
        /// </summary>
        public static string GlobalCardPoolFileName = "GlobalCardPools";

        /// <summary>
        /// 玩家角色池文件名称
        /// </summary>
        public static string GlobalPlayerRolePoolFileName = "GlobalPlayerRolePools";

        /// <summary>
        /// AI角色池文件名称
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

        /// <summary>
        /// 所有表属性
        /// </summary>
        public static string AllTablesAttrFlieName = "AllTablesAttr";
    }
}
