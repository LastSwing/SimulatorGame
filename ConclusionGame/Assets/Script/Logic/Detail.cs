using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detail
{
    static string FilePath = Application.dataPath + @"\Data";
    static string FileName = "sortjson.txt";
    /// <summary>
    /// 随机挑选案件
    /// </summary>
    /// <returns></returns>
    //public static Livelibrary RandomInit()
    //{
    //    Dictionary<string, string> keyValues = JsonHelper.DataRead(FileName, FilePath);
    //    int random = UnityEngine.Random.Range(1, keyValues.Count);
    //    Livelibrary livelibrary = new Livelibrary();
    //    int i = 1;
    //    foreach (var item in keyValues)
    //    {
    //        if (random == i)
    //            livelibrary = JsonHelper.JiexiJson(item.Value+".txt", FilePath);
    //        i++;
    //    }
    //    return livelibrary;
    //}

    /// <summary>
    /// 归案
    /// </summary>
    /// <param name="settle">返回的结果</param>
    /// <param name="Answer">正确答案</param>
    //public static void Justice(SettleLawsuit settle)
    //{
    //    Dictionary<string, string> keyValues = JsonHelper.DataRead(FileName, FilePath);
    //    keyValues.Remove(settle.Name);
    //    JsonHelper.DataExport(keyValues,FileName,FilePath);
    //    string Pdescribed = "";
    //    if (settle.Result == settle.RightResult)
    //        Pdescribed = "为官严谨,公正严明。";
    //    else if (settle.Result.Split('|').Length > 0)
    //    {
    //        if (settle.RightResult == "死刑")
    //            Pdescribed = "量刑过少，无法对犯人足够的惩戒。";
    //        else if (settle.RightResult == "无罪释放")
    //            Pdescribed = "量刑过重，无法公正对待每一个犯人。";
    //        else if (settle.RightResult.Split('|').Length > 0)
    //        {
    //            int result = Convert.ToInt32(settle.Result.Split('|')[1]);
    //            int rightResult = Convert.ToInt32(settle.RightResult.Split('|')[1]);
    //            if (result - rightResult >= 5)
    //                Pdescribed = "量刑过重，无法公正对待每一个犯人。";
    //            else if (result - rightResult <= -5)
    //                Pdescribed = "量刑过少，无法对犯人足够的惩戒。";
    //            else
    //                Pdescribed = "为官严谨,公正严明。";
    //        }
    //    }
    //    else if (settle.Result == "死刑")
    //        Pdescribed = "不经严谨论证，便草菅人命，忘记了为官造福百姓的初心。";
    //    else if (settle.Result == "无罪释放")
    //        Pdescribed = "有罪则罚，孰能放任之？";
    //    settle.Pdescribed = Pdescribed;
    //    JsonHelper.SaveResult(settle);
    //}


}
