using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Autogeneration
{
    /*    小怪初始值
    攻击力 2
    暴击  0
    暴伤  0
    攻击回复 0
    血量  10
    血量回复 0
    闪避  0
     */
    // 区间值
    static float[] interval = { 0.5F,1.5F};
    static string[] LittleName = { "树精","通灵石怪","蛇妖","蝎子精","狐妖" };
    static string[] BigName = { "天蝼","穷奇","青女","奇相","冥灵","青鴍","梼杌","鬿雀","髡顿","帝俊" };
    /// <summary>
    /// 自动生成小怪数值
    /// </summary>
    /// <param name="number">关卡数</param>
    /// <param name="hard">难度：简易-一般-困难</param>
    /// <returns></returns>
    public static field Littlemonster(int number,int hard)
    {
        field fd = new field();
        fd.Atk = 2 * number* Random.Range(interval[0], interval[1]) * hard;
        fd.HP = 10 * number * Random.Range(interval[0], interval[1]) * hard;
        fd.Name = LittleName[Random.Range(0, 4)];
        return fd;
    }
    /// <summary>
    /// boss生成
    /// </summary>
    /// <param name="number"></param>
    /// <param name="hard"></param>
    /// <returns></returns>
    public static field Bigmonster(int number, int hard)
    {
        field fd = new field();
        fd.Atk = 5 * number * Random.Range(interval[0], interval[1]) * hard;
        fd.HP = 30 * number * Random.Range(interval[0], interval[1]) * hard;
        fd.Name = BigName[(number/30)-1];
        return fd;
    }

}
