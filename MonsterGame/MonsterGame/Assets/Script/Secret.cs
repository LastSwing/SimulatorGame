using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecretArea
{
    /// <summary>
    /// 加载秘境
    /// </summary>
    /// <returns></returns>
    public List<SecretClass> LoadSecret()
    {
        /*五种秘境
         * 轮回秘境 +攻击回血
         * 仙林秘境 +血+回复
         * 火狱秘境 +攻击力暴击暴伤
         * 藏风秘境 +概率闪避
         * 沧海秘境 +概率攻击
        */
        List<SecretClass> Secret = new List<SecretClass>();
        Secret.Add(new SecretClass("轮回之力 Lv1", "攻击10%概率恢复40血量", "Rinne", 40F, 20F, 1, 1));
        Secret.Add(new SecretClass("轮回狂潮 Lv1", "攻击时回复4点血量", "Rinne", 4F, 2F, 1, 2));
        Secret.Add(new SecretClass("轮回之鉴 Lv1", "攻击时回复自身攻击力10%血量", "Rinne", 0.1F, 0.05F, 1, 3));
        Secret.Add(new SecretClass("仙林之蕴 Lv1", "最大生命值+70", "Forests", 70F, 30F, 1, 4));
        Secret.Add(new SecretClass("仙林之力 Lv1", "恢复 + 7", "Forests", 7F, 3F, 1, 5));
        Secret.Add(new SecretClass("生生不息 Lv1", "每通过一关血量+5", "Forests", 5F, 2F, 1, 6));
        Secret.Add(new SecretClass("火狱之怒 Lv1", "攻击力+5", "Fire", 5F, 2F, 1, 7));
        Secret.Add(new SecretClass("火狱之狂 Lv1", "暴击+5", "Fire", 0.05F, 0.02F, 1, 8));
        Secret.Add(new SecretClass("火狱之伤 Lv1", "暴伤+0.1", "Fire", 0.01F, 0.01F, 1, 9));
        Secret.Add(new SecretClass("藏形匿影 Lv1", "+5%闪避概率", "Wind", 0.05F, 0.02F, 1, 10));
        Secret.Add(new SecretClass("藏风之力 Lv1", "被攻击时1%的概率反弹本次攻击伤害", "Wind", 0.01F, 0.01F, 1, 11));
        Secret.Add(new SecretClass("纹风不动 Lv1", "+10%免伤", "Wind", 0.1F, 0.05F, 1, 12));
        Secret.Add(new SecretClass("沧海一粟 Lv1", "攻击时，有1%概率秒杀对手", "Sea", 0.01F, 0.01F, 1, 13));
        Secret.Add(new SecretClass("如堕烟海 Lv1", "攻击时，有5%的概率使对手眩晕", "Sea", 0.05F, 0.02F, 1, 14));
        Secret.Add(new SecretClass("沧海之力 Lv1", "攻击时，有5%的概率使对手缴械", "Sea", 0.05F, 0.02F, 1, 15));
        return Secret;
    }
    /// <summary>
    /// 升级秘境
    /// </summary>
    /// <param name="Secret"></param>
    public void Upgrade(SecretClass Secrets)
    {
        Secrets.Value = Secrets.Value.Replace(Secrets.Numerical.ToString(), (Secrets.Numerical += Secrets.Increment).ToString());
        Secrets.Name = Secrets.Name.Replace(Secrets.Lv.ToString(), (Secrets.Lv += 1).ToString());
        Secrets.Lv += 1;
        Secrets.Numerical += Secrets.Increment;
    }
}
public class SecretClass
{
    /// <summary>
    /// 名字
    /// </summary>
    public  string Name { get; set; }
    /// <summary>
    /// 描述
    /// </summary>
    public  string Value { get; set; }
    /// <summary>
    /// 分类
    /// </summary>
    public  string Classify { get; set; }
    /// <summary>
    /// 数值
    /// </summary>
    public  float Numerical { get; set; }
    /// <summary>
    /// 增量
    /// </summary>
    public  float Increment { get; set; }
    /// <summary>
    /// 等级
    /// </summary>
    public int Lv { get; set; }
    /// <summary>
    /// 唯一Key
    /// </summary>
    public int Key { get; set; }
    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="name">名字</param>
    /// <param name="value">描述</param>
    /// <param name="classify">分类</param>
    /// <param name="numerical">数值</param>
    /// <param name="increment">增量</param>
    /// <param name="lv">等级</param>
    /// 
    public SecretClass(string name, string value, string classify, float numerical, float increment, int lv,int key)
    {
        Name = name;
        Value = value;
        Classify = classify;
        Numerical = numerical;
        Increment = increment;
        Lv = lv;
        Key = key;
    }
}