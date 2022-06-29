using UnityEditor;
using UnityEngine;

namespace Assets.Script.Models.Map
{
    /// <summary>
    /// 当前角色在地图上的位置
    /// </summary>
    public class CurrentMapLocation
    {
        /// <summary>
        /// 行
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// 当前位置的图片地址
        /// </summary>
        public string CurrentImgUrl { get; set; }

        /// <summary>
        /// 地图类型 1普通，2商店，3Boss,4冒险
        /// </summary>
        public int MapType { get; set; }

        /// <summary>
        /// 是否击杀了boss 0否，1是
        /// </summary>
        public int HasKillBoss { get; set; }
    }
}