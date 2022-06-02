using System;
using System.Collections.Generic;
using System.Text;

namespace ConclusionEditor
{
    /// <summary>
    /// 事件库
    /// </summary>
    public class Livelibrary
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 事件介绍
        /// </summary>
        public string Lifetime { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// 衔接事件
        /// </summary>
        public string JoinName { get; set; }
        /// <summary>
        /// 年份时长
        /// </summary>
        public int YearDuration { get; set; }
        /// <summary>
        /// 衔接年份时长-上个事件发生后多久再发生本事件
        /// </summary>
        public int YearJoin { get; set; }
        /// <summary>
        /// 发生年份-填了衔接年份可不用填此项
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 对话 Dictionary<父ID,Dictionary<己ID, 角色|对话>>
        /// </summary>
        public Dictionary<Guid,Dictionary<Guid, string>> Dialogue { get; set; }
        /// <summary>
        /// 结局年份展示文字
        /// </summary>
        public Dictionary<string,List<Ending>> Ending { get; set; }
        /// <summary>
        /// 对话绑定,选择,BGM,动画,字段,结局
        /// </summary>
        public List<Fileid> Fileid { get; set; }
    }
    /// <summary>
    /// 结局类
    /// </summary>
    public class Ending
    {
        /// <summary>
        /// 绑定结局ID
        /// </summary>
        public Guid PGuid { get; set; }
        /// <summary>
        /// 主ID
        /// </summary>
        public Guid CGuid { get; set; }
        /// <summary>
        /// 星币
        /// </summary>
        public int Stellar { get; set; }
        /// <summary>
        /// 星力
        /// </summary>
        public int Stars { get; set; }
        /// <summary>
        /// 生产力
        /// </summary>
        public int Productivity { get; set; }
        /// <summary>
        /// 发生年份+描述
        /// </summary>
        public  int Vintage { get; set; }
        /// <summary>
        /// 发生描述
        /// </summary>
        public string Result { get; set; }
    }

    /// <summary>
    /// 事件列表
    /// </summary>
    public class Event
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 事件路径
        /// </summary>
        public string EventPath { get; set; }
        /// <summary>
        /// 发生年份
        /// </summary>
        public int Year { get; set; }
    }

    /// <summary>
    /// 绑定类
    /// </summary>
    public class Fileid
    { 
        /// <summary>
        /// 字段ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 父项ID
        /// </summary>
        public Guid ParentId { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public FileidType Fileidtype { get; set; }
        /// <summary>
        /// 开始字节 or 选择中是否可循环0可循环1不可循环
        /// </summary>
        public int InsertByte { get; set; }
        /// <summary>
        /// 结束字节
        /// </summary>
        public int EndByte { get; set; }
        /// <summary>
        /// 字段集
        /// </summary>
        public string[] Fileids { get; set; }
        /// <summary>
        /// 路径名-选择项
        /// </summary>
        public string PathName { get; set; }
    }
    public  enum FileidType
    { 
        选择 = 0,
        字段=1,
        结局=2,
        背景音乐=3,
        动画=4,
        对话 = 5,
        判断对话 = 6
    }
}
