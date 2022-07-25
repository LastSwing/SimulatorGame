using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Models
{
    /// <summary>
    /// 表结构
    /// 在新增表和新增字段时都要操作此表
    /// </summary>
    public class TableStruct
    {
        public int ID { get; set; }

        /// <summary>
        /// 类名称
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string AttrName { get; set; }
        /// <summary>
        /// 属性类型
        /// 请以此 model.ID.GetType().FullName; (model.ID不能为空，不然获取不正确)方式存储
        /// </summary>
        public string AttrType { get; set; }
    }
}
