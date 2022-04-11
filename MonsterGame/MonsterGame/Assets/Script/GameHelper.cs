using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class GameHelper
{
    #region json加密存取
    //密钥
    public static byte[] _KEY = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
    //向量
    public static byte[] _IV = new byte[] { 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 };

    //回合难度

    public static int hard = 1;

    /// <summary>
    /// DES加密操作
    /// </summary>
    /// <param name="normalTxt"></param>
    /// <returns></returns>
    public static string DesEncrypt(string normalTxt)
    {
        //byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
        //byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);

        DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
        int i = cryptoProvider.KeySize;
        MemoryStream ms = new MemoryStream();
        CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(_KEY, _IV), CryptoStreamMode.Write);

        StreamWriter sw = new StreamWriter(cst);
        sw.Write(normalTxt);
        sw.Flush();
        cst.FlushFinalBlock();
        sw.Flush();

        string strRet = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        return strRet;
    }

    /// <summary>
    /// DES解密操作
    /// </summary>
    /// <param name="securityTxt">加密字符串</param>
    /// <returns></returns>
    public static string DesDecrypt(string securityTxt)//解密  
    {
        //byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
        //byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);
        byte[] byEnc;
        try
        {
            securityTxt.Replace("_%_", "/");
            securityTxt.Replace("-%-", "#");
            byEnc = Convert.FromBase64String(securityTxt);
        }
        catch
        {
            return null;
        }
        DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
        MemoryStream ms = new MemoryStream(byEnc);
        CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(_KEY, _IV), CryptoStreamMode.Read);
        StreamReader sr = new StreamReader(cst);
        return sr.ReadToEnd();
    }

    #endregion


    /// <summary>
    /// 数据组装 成JSON
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="isLast"></param>
    /// <returns></returns>
    public static string WriteItem(string key, string value, bool isLast = false)
    {
        string s = "\"" + key + "\":" + "\"" + value + "\"";
        if (!isLast)
            s += ",";
        return s;
    }

    /// <summary>
    /// 存档
    /// </summary>
    /// <param name="dic">属性，值</param>
    /// <param name="Name">文件名</param>
    /// <param name="FolderName">文件夹名称</param>
    public static void DataExport(Dictionary<string, string> dic, string Name)
    {
        var id = $"{System.DateTime.Now.ToString("yyyyMMddHHmmssff")}";
        string json = "{" + WriteItem("ID", id);
        int j = 0;
        foreach (var item in dic)
        {
            j++;
            if (dic.Count == j)
                json += WriteItem(item.Key, item.Value, true);
            else
                json += WriteItem(item.Key, item.Value);
        }
        json += "}";
        Debug.Log(json);
        json = DesEncrypt(json);
        var path = Application.dataPath + "/Data/";
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists($"{path}/{Name}.txt"))
            File.Delete($"{path}/{Name}.txt");
        File.WriteAllText($"{path}/{Name}.txt", json);
        //Debug.Log($"导出成功：{path}/{Name}.txt");
    }

    /// <summary>
    /// 读档
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    public static Dictionary<string, string> DataRead(string Name)
    {
        var path = Application.dataPath + "/Data/";
        Dictionary<string, string> dict = new Dictionary<string, string>();
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists(path + @"\" + Name))
        {
            //string json = DesDecrypt(File.ReadAllText(path + @"\" + Name));
            StreamReader json = File.OpenText(path + @"\" + Name);
            //Debug.Log("读档" + json);
            string input = json.ReadToEnd();
            dict = JsonMapper.ToObject<Dictionary<string, string>>(input);
        }
        return dict;
    }
    
}
