using UnityEditor;
using UnityEngine;

namespace Assets.Script.Models.Map
{
    /// <summary>
    /// 地图路线
    /// </summary>
    public class MapPath
    {
        /// <summary>
        /// 当前行
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// 上一行数量
        /// </summary>
        public int PreviousRow { get; set; }

        /// <summary>
        /// 当前行数量
        /// </summary>
        public int CurrentRow { get; set; }
        /// <summary>
        /// 随机数
        /// </summary>
        public int RandomNum { get; set; }
    }
}