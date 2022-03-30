using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏战斗属性
/// </summary>
public class field
{
    /// <summary>
    /// 攻击力
    /// </summary>
    public float Atk { get; set; }
    /// <summary>
    /// 暴击
    /// </summary>
    public float Crit { get; set; }
    /// <summary>
    /// 暴伤
    /// </summary>
    public float CritHarm { get; set; }
    /// <summary>
    /// 攻击回复
    /// </summary>
    public float AtkRegain { get; set; }
    /// <summary>
    /// 血量
    /// </summary>
    public float HP { get; set; }
    /// <summary>
    /// 血量回复
    /// </summary>
    public float HPRegen { get; set; }
    /// <summary>
    /// 闪避
    /// </summary>
    public float Dodge { get; set; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 秒杀概率
    /// </summary>
    public float Seckill { get; set; }
    /// <summary>
    /// 回合内多次攻击概率
    /// </summary>
    public float Twice { get; set; }
    /// <summary>
    /// 经验值
    /// </summary>
    public int EXP { get; set; }
}

/// <summary>
/// 灵泉
/// </summary>
public class spring
{
    /// <summary>
    /// 固定回复50%生命值
    /// </summary>
    public Dictionary<string,float> Spring1 
    {
        get 
        {
            Dictionary<string, float> keyValuePairs = new Dictionary<string, float>();
            keyValuePairs.Add("固定回复50%生命值", 0.5F);
            return keyValuePairs; 
        }
    }
    /// <summary>
    /// 随机回复20-100%生命值
    /// </summary>
    public Dictionary<string, float> Spring2 
    {
        get
        {
            Dictionary<string, float> keyValuePairs = new Dictionary<string, float>();
            keyValuePairs.Add("随机回复20-100%生命值", Random.Range(0.2F,1.01F));
            return keyValuePairs;
        }
    }
    /// <summary>
    /// 不回复生命值，增加40最大生命值
    /// </summary>
    public Dictionary<string, float> Spring3
    {
        get
        {
            Dictionary<string, float> keyValuePairs = new Dictionary<string, float>();
            keyValuePairs.Add("随机回复20-100%生命值", 40);
            return keyValuePairs;
        }
    }

}

/// <summary>
/// 秘境
/// </summary>
public class Secret
{ 

}
