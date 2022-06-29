using UnityEditor;
using UnityEngine;

namespace Assets.Script.Models.Map
{
    /// <summary>
    /// 地图战斗点
    /// </summary>
    public class MapCombatPoint
    {
        /// <summary>
        /// 所在行
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// 所在列
        /// </summary>
        public int Column { get; set; }
        /// <summary>
        /// 战斗点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 战斗点Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 1普通战斗、2商店、3Boss、4冒险
        /// </summary>
        public int Type { get; set; }
    }
}