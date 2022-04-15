using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecretArea
{
    /// <summary>
    /// 加载秘境效果
    /// </summary>
    /// <returns></returns>
    public static List<SecretClass> LoadSecret
    {
        /*五种秘境
         * 轮回秘境 +攻击回血
         * 仙林秘境 +血+回复
         * 火狱秘境 +攻击力暴击暴伤
         * 藏风秘境 +概率闪避
         * 沧海秘境 +概率攻击
        */
        get
        {
            List<SecretClass> Secret = new List<SecretClass>();
            Secret.Add(new SecretClass("轮回之力 Lv1", "攻击10%概率恢复40血量", "Rinne", new float[] { 40F }, new float[] { 20F }, 1, 1));
            Secret.Add(new SecretClass("轮回狂潮 Lv1", "攻击时回复4点血量", "Rinne", new float[] { 4F }, new float[] { 2F }, 1, 2));
            Secret.Add(new SecretClass("轮回之鉴 Lv1", "攻击时回复自身攻击力10%血量", "Rinne", new float[] { 0.1F }, new float[] { 0.05F }, 1, 3));
            Secret.Add(new SecretClass("仙林之蕴 Lv1", "最大生命值+70", "Forests", new float[] { 70F }, new float[] { 30F }, 1, 4));
            Secret.Add(new SecretClass("仙林之力 Lv1", "恢复 + 7", "Forests", new float[] { 7F }, new float[] { 3F }, 1, 5));
            Secret.Add(new SecretClass("生生不息 Lv1", "每通过一关血量+5", "Forests", new float[] { 5F }, new float[] { 2F }, 1, 6));
            Secret.Add(new SecretClass("火狱之怒 Lv1", "攻击力+5", "Fire", new float[] { 5F }, new float[] { 2F }, 1, 7));
            Secret.Add(new SecretClass("火狱之狂 Lv1", "暴击+5", "Fire", new float[] { 0.05F }, new float[] { 0.02F }, 1, 8));
            Secret.Add(new SecretClass("火狱之伤 Lv1", "暴伤+0.1", "Fire", new float[] { 0.01F }, new float[] { 0.01F }, 1, 9));
            Secret.Add(new SecretClass("藏形匿影 Lv1", "+5%闪避概率", "Wind", new float[] { 0.05F }, new float[] { 0.02F }, 1, 10));
            Secret.Add(new SecretClass("藏风之力 Lv1", "被攻击时1%的概率反弹本次攻击伤害", "Wind", new float[] { 0.01F }, new float[] { 0.01F }, 1, 11));
            Secret.Add(new SecretClass("纹风不动 Lv1", "+10%免伤", "Wind", new float[] { 0.1F }, new float[] { 0.05F }, 1, 12));
            Secret.Add(new SecretClass("沧海一粟 Lv1", "攻击时，有1%概率秒杀对手", "Sea", new float[] { 0.01F }, new float[] { 0.01F }, 1, 13));
            Secret.Add(new SecretClass("如堕烟海 Lv1", "攻击时，有5%的概率使对手眩晕", "Sea", new float[] { 0.05F }, new float[] { 0.02F }, 1, 14));
            Secret.Add(new SecretClass("沧海之力 Lv1", "攻击时，有5%的概率使对手缴械", "Sea", new float[] { 0.05F }, new float[] { 0.02F }, 1, 15));
            return Secret;
        }
    }

    /// <summary>
    /// 获得秘境之力存档
    /// </summary>
    /// <param name="secretClass"></param>
    public void InsertSecret(SecretClass secretClass)
    {
        Dictionary<string,string> secret = GameHelper.DataRead("Secret");
        if (secret.TryGetValue(secretClass.Key.ToString(),out string vcl))//已存在秘境之力则升级
        {
            secret[secretClass.Key.ToString()] =(secretClass.Lv+1).ToString();
        }
        else
            secret.Add(secretClass.Key.ToString(), secretClass.Lv.ToString());
        GameHelper.DataExport(secret,"Secret");
    }
    /// <summary>
    /// 加载单个秘境
    /// </summary>
    /// <param name="Secret"></param>
    public static SecretClass Upgrade(SecretClass Secrets)
    {
        Secrets.Value = Secrets.Value.Replace(Secrets.Numerical.ToString(), (Secrets.Numerical[0] += Secrets.Increment[0]).ToString());
        Secrets.Name = Secrets.Name.Replace(Secrets.Lv.ToString(), (Secrets.Lv).ToString());
        Secrets.Numerical[0] += Secrets.Increment[0]* Secrets.Lv;
        return Secrets;
    }
    /// <summary>
    /// 加载血脉效果
    /// </summary>
    public static List<SecretClass> LoadBlood
    {
        get
        {
            List<SecretClass> Secret = new List<SecretClass>();
            Secret.Add(new SecretClass("女娲血脉", "增加{0}点永久生命值，每回合回复{1}点生命值。", "NvWa", new float[] { 20F ,3F}, new float[] { 30F, 2F }, 1, 16));
            Secret.Add(new SecretClass("盘古血脉", "回复+{0}，遇到灵泉概率+{1}%。", "PanGu", new float[] { 7F, 10F},new float[] {3F,5F }, 1, 17));
            Secret.Add(new SecretClass("共工血脉", "增加{0}闪避概率，免伤+{1}%。", "GongGong", new float[] { 0.1F, 5F }, new float[] { 5F, 5F }, 1, 18));
            Secret.Add(new SecretClass("蚩尤血脉", "攻击时{0}%概率让对手缴械。", "ChiYou", new float[] { 10F }, new float[] {  5F }, 1, 19));
            Secret.Add(new SecretClass("伏羲血脉", "增加{0}暴击率，{1}暴击伤害，多余的暴击率将转化为暴击伤害。", "FuXi", new float[] { 10F, 5F }, new float[] { 10F, 4F }, 1, 20));
            Secret.Add(new SecretClass("夸父血脉", "增加{0}点永久攻击力，攻击回复+{1}%。", "KuaFu", new float[] { 5F, 10F }, new float[] { 2F, 5F }, 1, 21));
            return Secret;
        }
    }
}