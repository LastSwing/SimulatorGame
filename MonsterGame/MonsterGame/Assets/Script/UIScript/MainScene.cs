using Assets.Script;
using LitJson;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    private GameObject btn_GameStart, btn_Blood, btn_Acupoint, btn_ContinueGame;
    private Text txt_Level, txt_GameStart;
    // Start is called before the first frame update
    void Start()
    {
        #region 初始化控件
        btn_GameStart = transform.Find("btn_GameStart").gameObject;
        btn_GameStart.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("GameScene", 0); });
        btn_Blood = transform.Find("btn_Blood").gameObject;
        btn_Blood.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("BloodScene"); });
        btn_Acupoint = GameObject.Find("btn_Acupoint");
        btn_Acupoint.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("OpeningScene"); });
        btn_ContinueGame = GameObject.Find("btn_ContinueGame");
        btn_ContinueGame.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("GameScene"); });

        //txt_Level = transform.Find("txt_Level").GetComponent<Text>();//关卡 
        txt_GameStart = transform.Find("btn_GameStart/txt_GameStart").GetComponent<Text>();//按钮文本 
        #endregion
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Init()
    {
        //判断是否显示继续游戏
        var dic = GetSaveData();

        var field = Common.JsonToModel<field>(dic["888"].ToString());
        if (field?.HP != 0)
        {
            ShowContinueGame();
            //txt_Level.text = $"厚土界 · 初 \n ({dic["999"]} / 300)";
            txt_GameStart.text = "重新开始";
        }
    }

    /// <summary>
    /// 获取是否有存档数据
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, object> GetSaveData()
    {
        var path = Application.dataPath + "/Data/";
        Dictionary<string, object> dict = new Dictionary<string, object>();
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists(path + @"\" + "LevelData/Level.txt"))
        {
            StreamReader json = File.OpenText(path + @"\" + "LevelData/Level.txt");
            //Debug.Log("读档" + json);
            string input = json.ReadToEnd();
            JsonReader reader = new JsonReader(input);
            string temp = string.Empty;
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    switch (reader.Token)
                    {
                        case JsonToken.PropertyName:
                            dict.Add(reader.Value.ToString(), string.Empty);
                            temp = reader.Value.ToString();
                            break;
                        default:
                            dict[temp] = reader.Value;
                            break;
                    }
                }
            }
            json.Close();
        }
        return dict;
    }

    /// <summary>
    /// 显示继续游戏按钮
    /// </summary>
    private void ShowContinueGame()
    {
        //btn_ContinueGame.transform.position = new Vector3(300, -534, 0);
        btn_ContinueGame.SetActive(true);
    }

}
