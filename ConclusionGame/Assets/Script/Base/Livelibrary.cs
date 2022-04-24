using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 案件库
/// </summary>
public class Livelibrary
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 难度 0简单1一般2困难
    /// </summary>
    public int Dard { get; set; }
    /// <summary>
    /// 案发经过
    /// </summary>
    public string Lifetime { get; set; }
    /// <summary>
    /// 线索
    /// </summary>
    public Dictionary<int, string> Lifetimes { get; set; }
    /// <summary>
    /// 使用大刑伺候犯人对每种线索的描述
    /// </summary>
    public Dictionary<int, string> Lifetimetrue { get; set; }
    /// <summary>
    /// 犯人证词
    /// </summary>
    public Dictionary<int, string> Stestimony { get; set; }
    /// <summary>
    /// 判官证词
    /// </summary>
    public Dictionary<int, string> Jtestimony { get; set; }
    /// <summary>
    /// 犯人对每种结果的描述
    /// </summary>
    public Dictionary<int,string> Sdescribed { get; set; }
    /// <summary>
    /// 判官对每种结果的描述
    /// </summary>
    public Dictionary<int, string> Jdescribed { get; set; }
    /// <summary>
    /// 系统对每种结果的描述
    /// </summary>
    public Dictionary<int, string> Pdescribed { get; set; }
    /// <summary>
    /// 正确答案 ；分割0入狱1流放2当庭释放；服刑多少年
    /// </summary>
    public string Correct { get; set; }

    public Livelibrary(Dictionary<string,string> keyValues)
    {
        foreach (var item in keyValues)
        {
            string[] str = item.Value.Split('；');
            switch (item.Key)
            {
                case "Lifetime":
                    Lifetime = item.Value;
                    break;
                case "Lifetimes":
                    for (int i = 0; i < str.Length; i++)
                        Lifetimes.Add(i,str[i]);
                    break;
                case "Jtestimony":
                    for (int i = 0; i < str.Length; i++)
                        Jtestimony.Add(i, str[i]);
                    break;
                case "Name":
                    Name = item.Value;
                    break;
                case "Dard":
                    Dard = item.Value == "简单" ? 0 : item.Value == "一般" ? 1 : 2;
                    break;
                case "Lifetimetrue":
                    for (int i = 0; i < str.Length; i++)
                        Lifetimetrue.Add(i, str[i]);
                    break;
                case "Stestimony":
                    for (int i = 0; i < str.Length; i++)
                        Stestimony.Add(i, str[i]);
                    break;
                case "Sdescribed":
                    for (int i = 0; i < str.Length; i++)
                        Sdescribed.Add(i, str[i]);
                    break;
                case "Jdescribed":
                    for (int i = 0; i < str.Length; i++)
                        Jdescribed.Add(i, str[i]);
                    break;
                case "Correct":
                    Correct = item.Value;
                    break;
            }
        }
        Pdescribed.Add(0, "量刑过少，无法对犯人足够的惩戒。");
        Pdescribed.Add(1, "量刑过重，无法公正对待每一个犯人。");
        Pdescribed.Add(2, "不经严谨论证，便草菅人命，忘记了为官造福百姓的初心。");
        Pdescribed.Add(3, "有罪则罚，孰能放任之？");
    }
}
