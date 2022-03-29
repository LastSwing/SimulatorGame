using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �غ�ս��
/// </summary>
public class field : MonoBehaviour
{
    /// <summary>
    /// ������
    /// </summary>
    public float Atk { get; set; }
    /// <summary>
    /// ����
    /// </summary>
    public float Crit { get; set; }
    /// <summary>
    /// ����
    /// </summary>
    public float CritHarm { get; set; }
    /// <summary>
    /// �����ظ�
    /// </summary>
    public float AtkRegain { get; set; }
    /// <summary>
    /// Ѫ��
    /// </summary>
    public float HP { get; set; }
    /// <summary>
    /// Ѫ���ظ�
    /// </summary>
    public float HPRegen { get; set; }
    /// <summary>
    /// ����
    /// </summary>
    public float Dodge { get; set; }
    /// <summary>
    /// ����
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// ��ɱ����
    /// </summary>
    public float Seckill { get; set; }
    /// <summary>
    /// �غ��ڶ�ι�������
    /// </summary>
    public float Twice { get; set; }
    /// <summary>
    /// ģ��غϹ���
    /// </summary>
    /// <param name="fileid">��ɫ����</param>
    /// <param name="victory">�Ƿ�ʤ��</param>
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
                        str1 += Name + "�������ܣ���" + fileid.Name + "��ɱ��";
                        fileid.HP = 0;
                        round = 2;
                        break;
                    }
                    int atk = System.Convert.ToInt32(System.Math.Round(Atk + Random.Range(0.00F, 1.01F) < Crit ? Atk * CritHarm : 0.00F));
                    fileid.HP -= atk;
                    str1 += Name + "������������" + fileid.Name + "�����" + atk + "���˺���";
                    HP += AtkRegain;
                    if (fileid.HP <= 0)
                    {
                        str1 += fileid.Name + "��������";
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
                    str1 += Name + "��������������" + fileid.Name + "�����ˡ�";
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
                        str1 += fileid.Name + "�������ܣ���" + Name + "��ɱ��";
                        HP = 0;
                        round = 2;
                        break;
                    }
                    int atk = System.Convert.ToInt32(System.Math.Round(fileid.Atk + Random.Range(0.00F, 1.01F) < fileid.Crit ? fileid.Atk * fileid.CritHarm : 0.00F));
                    HP -= atk;
                    str1 += fileid.Name + "������������" + Name + "�����" + atk + "���˺���";
                    fileid.HP += fileid.AtkRegain;
                    if (HP <= 0)
                    {
                        str1 += Name + "��������";
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
                    str1 += fileid.Name + "��������������" + Name + "�����ˡ�";
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
