using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸս������
/// </summary>
public class field
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
    /// ����ֵ
    /// </summary>
    public int EXP { get; set; }
}

/// <summary>
/// ��Ȫ
/// </summary>
public class spring
{
    /// <summary>
    /// �̶��ظ�50%����ֵ
    /// </summary>
    public Dictionary<string,float> Spring1 
    {
        get 
        {
            Dictionary<string, float> keyValuePairs = new Dictionary<string, float>();
            keyValuePairs.Add("�̶��ظ�50%����ֵ", 0.5F);
            return keyValuePairs; 
        }
    }
    /// <summary>
    /// ����ظ�20-100%����ֵ
    /// </summary>
    public Dictionary<string, float> Spring2 
    {
        get
        {
            Dictionary<string, float> keyValuePairs = new Dictionary<string, float>();
            keyValuePairs.Add("����ظ�20-100%����ֵ", Random.Range(0.2F,1.01F));
            return keyValuePairs;
        }
    }
    /// <summary>
    /// ���ظ�����ֵ������40�������ֵ
    /// </summary>
    public Dictionary<string, float> Spring3
    {
        get
        {
            Dictionary<string, float> keyValuePairs = new Dictionary<string, float>();
            keyValuePairs.Add("����ظ�20-100%����ֵ", 40);
            return keyValuePairs;
        }
    }

}

/// <summary>
/// �ؾ�
/// </summary>
public class Secret
{ 

}
