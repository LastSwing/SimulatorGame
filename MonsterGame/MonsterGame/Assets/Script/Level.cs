using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 关卡
/// </summary>
public class Level
{
    float spring = 0.05F;//灵泉出现概率
    float Secret = 0.1F;//秘境出现概率
    /// <summary>
    /// 生成关卡
    /// </summary>
    /// <param name="number">关卡数</param>
    /// <returns></returns>
    public Dictionary<string,object> CreateLevel(float number)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        if (int.TryParse(System.Convert.ToString(number / 30.00F), out int result))//出现boss
        {
            dict.Add("boss", Autogeneration.Bigmonster(result, GameHelper.hard));
            return dict;
        }
        else
        {
            for (int i = 1; i < 3; i++)//模拟一关有三个，可以是小怪和灵泉或秘境
            {
                if (Random.Range(0.00F, 1.01F) <= Random.Range(0.00F, spring))
                {
                    dict.Add("spring",null);
                }
                else if (Random.Range(0.00F, 1.01F) <= Random.Range(0.00F, Secret))
                {
                    dict.Add("Secret", null);
                }
                else
                {
                    dict.Add("monster", Autogeneration.Bigmonster(result, GameHelper.hard));
                }
            }
            return dict;
        }
    }
    /// <summary>
    /// 模拟回合攻击
    /// </summary>
    ///<param name = "List<string>" > 回合记录 </ param >
    ///<param name="fileid">角色属性</param>
    /// <param name="fileid1">怪物属性</param>
    /// <param name="victory">是否胜利</param>
    /// <returns></returns>
    public List<string> Combat(field fileid, field fileid1, out bool victory)
    {
        List<string> str = new List<string>();
        float random = Random.Range(0.00F, 1.01F);
        int round = 0;
        victory = false;
        while (round > 1)
        {
            round = 0;
            string str1 = "";
            if (round == 0)
            {
                if (fileid1.Dodge == 0.00F || Random.Range(0.00F, 1.01F) <= fileid1.Dodge)
                {
                    if (Random.Range(0.00F, 1.01F) <= fileid.Seckill)
                    {
                        str1 += fileid.Name + "发动技能，将" + fileid1.Name + "秒杀。";
                        fileid1.HP = 0;
                        round = 2;
                        break;
                    }
                    int atk = System.Convert.ToInt32(System.Math.Round(fileid.Atk + Random.Range(0.00F, 1.01F) < fileid.Crit ? fileid.Atk * fileid.CritHarm : 0.00F));
                    fileid1.HP -= atk;
                    str1 += fileid.Name + "发动攻击，对" + fileid1.Name + "造成了" + atk + "点伤害。";
                    fileid.HP += fileid.AtkRegain;
                    if (fileid1.HP <= 0)
                    {
                        str1 += fileid1.Name + "已死亡。";
                        round = 2;
                        victory = true;
                    }
                    else
                    {
                        fileid1.HP += fileid1.HPRegen;
                    }
                }
                else
                {
                    str1 += fileid.Name + "发动攻击，但被" + fileid1.Name + "闪避了。";
                    round = 1;
                }
                if (Random.Range(0.00F, 1.01F) <= fileid.Twice)
                    round = 0;
            }
            else
            {
                if (fileid.Dodge == 0.00F || Random.Range(0.00F, 1.01F) <= fileid.Dodge)
                {
                    if (Random.Range(0.00F, 1.01F) <= fileid1.Seckill)
                    {
                        str1 += fileid1.Name + "发动技能，将" + fileid.Name + "秒杀。";
                        fileid.HP = 0;
                        round = 2;
                        break;
                    }
                    int atk = System.Convert.ToInt32(System.Math.Round(fileid1.Atk + Random.Range(0.00F, 1.01F) < fileid1.Crit ? fileid1.Atk * fileid1.CritHarm : 0.00F));
                    fileid.HP -= atk;
                    str1 += fileid1.Name + "发动攻击，对" + fileid.Name + "造成了" + atk + "点伤害。";
                    fileid1.HP += fileid1.AtkRegain;
                    if (fileid.HP <= 0)
                    {
                        str1 += fileid.Name + "已死亡。";
                        round = 2;
                        victory = false;
                    }
                    else
                    {
                        fileid.HP += fileid1.HPRegen;
                        round = 0;
                    }
                }
                else
                {
                    str1 += fileid1.Name + "发动攻击，但被" + fileid.Name + "闪避了。";
                    round = 0;
                }
                if (Random.Range(0.00F, 1.01F) <= fileid1.Twice)
                    round = 1;
            }
            str.Add(str1);
        }
        return str;
    }
}
