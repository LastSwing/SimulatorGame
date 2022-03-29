using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 回合战斗
/// </summary>
public class field : MonoBehaviour
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
    /// 模拟回合攻击
    /// </summary>
    /// <param name="fileid">角色属性</param>
    /// <param name="victory">是否胜利</param>
    /// <returns></returns>
    public List<string> Combat(field fileid, out bool victory)
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
                if (fileid.Dodge == 0.00F || Random.Range(0.00F, 1.01F) <= fileid.Dodge)
                {
                    if (Random.Range(0.00F, 1.01F) <= Seckill)
                    {
                        str1 += Name + "发动技能，将" + fileid.Name + "秒杀。";
                        fileid.HP = 0;
                        round = 2;
                        break;
                    }
                    int atk = System.Convert.ToInt32(System.Math.Round(Atk + Random.Range(0.00F, 1.01F) < Crit ? Atk * CritHarm : 0.00F));
                    fileid.HP -= atk;
                    str1 += Name + "发动攻击，对" + fileid.Name + "造成了" + atk + "点伤害。";
                    HP += AtkRegain;
                    if (fileid.HP <= 0)
                    {
                        str1 += fileid.Name + "已死亡。";
                        round = 2;
                        victory = true;
                    }
                    else
                    {
                        fileid.HP += fileid.HPRegen;
                    }
                }
                else
                {
                    str1 += Name + "发动攻击，但被" + fileid.Name + "闪避了。";
                    round = 1;
                }
                if (Random.Range(0.00F, 1.01F) <= Twice)
                    round = 0;
            }
            else
            {
                if (Dodge == 0.00F || Random.Range(0.00F, 1.01F) <= Dodge)
                {
                    if (Random.Range(0.00F, 1.01F) <= fileid.Seckill)
                    {
                        str1 += fileid.Name + "发动技能，将" + Name + "秒杀。";
                        HP = 0;
                        round = 2;
                        break;
                    }
                    int atk = System.Convert.ToInt32(System.Math.Round(fileid.Atk + Random.Range(0.00F, 1.01F) < fileid.Crit ? fileid.Atk * fileid.CritHarm : 0.00F));
                    HP -= atk;
                    str1 += fileid.Name + "发动攻击，对" + Name + "造成了" + atk + "点伤害。";
                    fileid.HP += fileid.AtkRegain;
                    if (HP <= 0)
                    {
                        str1 += Name + "已死亡。";
                        round = 2;
                        victory = false;
                    }
                    else
                    {
                        HP += fileid.HPRegen;
                        round = 0;
                    }
                }
                else
                {
                    str1 += fileid.Name + "发动攻击，但被" + Name + "闪避了。";
                    round = 0;
                }
                if (Random.Range(0.00F, 1.01F) <= fileid.Twice)
                    round = 1;
            }
            str.Add(str1);
        }
        return str;
    }
}
