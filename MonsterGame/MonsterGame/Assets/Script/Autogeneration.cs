using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Autogeneration
{
    /*    С�ֳ�ʼֵ
    ������ 2
    ����  0
    ����  0
    �����ظ� 0
    Ѫ��  10
    Ѫ���ظ� 0
    ����  0
     */
    // ����ֵ
    static float[] interval = { 0.5F,1.5F};
    static string[] LittleName = { "����","ͨ��ʯ��","����","Ы�Ӿ�","����" };
    static string[] BigName = { "����","����","��Ů","����","ڤ��","���j","���","�oȸ","�ն�","�ۿ�" };
    /// <summary>
    /// �Զ�����С����ֵ
    /// </summary>
    /// <param name="number">�ؿ���</param>
    /// <param name="hard">�Ѷȣ�����-һ��-����</param>
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
    /// boss����
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
