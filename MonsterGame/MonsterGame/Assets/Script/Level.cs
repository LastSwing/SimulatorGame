using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 关卡
/// </summary>
public class Level
{
    static float spring = 0.05F;//灵泉出现概率
    static float Secret = 0.1F;//秘境出现概率
    /// <summary>
    /// 生成关卡
    /// </summary>
    /// <param name="number">关卡数</param>
    /// <returns></returns>
    public static Dictionary<int, object> CreateLevel(float number)
    {
        Dictionary<int, object> dict = new Dictionary<int, object>();
        if (int.TryParse(System.Convert.ToString(number / 30.00F), out int result))//出现boss
        {
            dict.Add(99, Autogeneration.Bigmonster(result, GameHelper.hard));
            return dict;
        }
        else
        {
            for (int i = 0; i < 3; i++)//模拟一关有三个，可以是小怪和灵泉或秘境
            {
                if (Random.Range(0.00F, 1.01F) <= Random.Range(0.00F, spring))
                {
                    dict.Add(i, null);//灵泉
                }
                else if (Random.Range(0.00F, 1.01F) <= Random.Range(0.00F, Secret))
                {
                    dict.Add(i, null);//秘境
                }
                else
                {
                    dict.Add(i, Autogeneration.Littlemonster(System.Convert.ToInt32(number), GameHelper.hard));
                }
            }
            return dict;
        }
    }
    /// <summary>
    /// 模拟回合攻击
    /// </summary>
    ///<param name="fileid">角色属性</param>
    /// <param name="fileid1">怪物属性</param>
    /// <param name="victory">是否胜利</param>
    /// <param name="maxHp">最大血量</param>
    /// <returns>回合记录</returns>
    public static Dictionary<string, object> Combat(field fileid, field fileid1, float maxHp, out bool victory)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        List<string> str = new List<string>();
        float random = Random.Range(0.00F, 1.01F);
        int round = 0;//0是角色回合，1是野怪回合，2是一方死亡
        victory = false;
        while (round < 2)
        {
            string str1 = "";
            #region 角色攻击
            if (round == 0)
            {
                if (fileid1.Dodge == 0.00F || Random.Range(0.00F, 1.01F) > fileid1.Dodge)
                {
                    if (Random.Range(0.00F, 1.01F) <= fileid.Seckill)
                    {
                        str1 += fileid.Name + "\n发动技能，将" + fileid1.Name + "秒杀。";
                        fileid1.HP = 0;
                        victory = true;
                        round = 2;
                        break;
                    }
                    int atk = System.Convert.ToInt32(System.Math.Round(fileid.Atk + (Random.Range(0.00F, 1.01F) <= fileid.Crit ? (fileid.CritHarm * fileid.Atk / 100) : 0.00F)));
                    fileid1.HP -= atk;
                    str1 += fileid.Name + "发动攻击，对" + fileid1.Name + "\n造成了" + atk + "点伤害。";
                    if (fileid.HP < maxHp)//如果已经满血则不回血
                    {
                        var currentHp = fileid.HP += fileid.AtkRegain;
                        if (currentHp > maxHp)
                            fileid.HP = maxHp;
                        else
                            fileid.HP = currentHp;
                        str1 += $"\n触发天赋技能，攻击恢复{fileid.AtkRegain}点血量。";
                    }
                    if (fileid1.HP <= 0)
                    {
                        str1 += $"\n{fileid1.Name}死亡。";
                        round = 2;
                        victory = true;
                    }
                    else
                    {
                        fileid1.HP += fileid1.HPRegen;
                        round = 1;
                    }
                }
                else
                {
                    str1 += $"\n{fileid.Name}发动攻击，但被{fileid1.Name}闪避了。";
                    round = 1;
                }
                if (Random.Range(0.00F, 1.01F) <= fileid.Twice)
                {
                    str1 += $"\n{fileid.Name}触发回合内多次攻击技能，再次发动攻击。";
                    round = 0;
                }
            }
            #endregion
            #region 野怪攻击
            else
            {
                if (fileid.Dodge == 0.00F || Random.Range(0.00F, 1.01F) > fileid.Dodge)
                {
                    if (Random.Range(0.00F, 1.01F) <= fileid1.Seckill)
                    {
                        str1 += $"\n{fileid1.Name}发动技能，将{ fileid.Name}秒杀，游戏失败。";
                        fileid.HP = 0;
                        round = 2;
                        break;
                    }
                    int atk = System.Convert.ToInt32(System.Math.Round(fileid1.Atk + (Random.Range(0.00F, 1.01F) < fileid1.Crit ? (fileid1.CritHarm * fileid1.Atk / 100) : 0.00F)));
                    fileid.HP -= atk;
                    str1 += $"\n{fileid1.Name}发动攻击，对{fileid.Name}造成了{atk}点伤害。";
                    fileid1.HP += fileid1.AtkRegain;
                    if (fileid.HP <= 0)
                    {
                        str1 += $"\n{fileid.Name}被击杀，游戏失败";
                        round = 2;
                    }
                    else
                    {
                        fileid.HP += fileid1.HPRegen;
                        round = 0;
                    }
                }
                else
                {
                    str1 +=$"\n{fileid1.Name}发动攻击，但被{fileid.Name}闪避了。";
                    round = 0;
                }
                if (Random.Range(0.00F, 1.01F) <= fileid1.Twice)
                {
                    str1 += $"\n{fileid1.Name}触发回合内多次攻击技能，再次发动攻击。";
                    round = 1;
                }
            }
            #endregion
            str.Add(str1);
        }
        dic.Add("AtkDetail", str);
        dic.Add("Role", fileid);
        dic.Add("Monster", fileid1);
        return dic;
    }
}
