using Assets.Script;
using LitJson;
using System.Collections.Generic;
using System.IO;
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
        btn_GameStart = GameObject.Find("btn_GameStart");
        btn_GameStart.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("GameScene", 0); });
        btn_Blood = GameObject.Find("btn_Blood");
        btn_Blood.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("BloodScene"); });
        btn_Acupoint = GameObject.Find("btn_Acupoint");
        btn_Acupoint.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("AcupointScene"); });
        btn_ContinueGame = GameObject.Find("btn_ContinueGame");
        btn_ContinueGame.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("GameScene"); });

        txt_Level = GameObject.Find("txt_Level").GetComponent<Text>();//关卡 
        txt_GameStart = GameObject.Find("btn_GameStart/txt_GameStart").GetComponent<Text>();//按钮文本 
        #endregion
        Init();
        btn_Acupoint.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("OpeningScene"); });
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Init()
    {
        //判断是否显示继续游戏
        var dic = GetSaveData();
        var field = JsonMapper.ToObject<field>(dic[888].ToString());
        if (field?.HP != 0)
        {
            ShowContinueGame();
            txt_Level.text = $"厚土界 · 初 \n ({dic[999]} / 300)";
            txt_GameStart.text = "重新开始";
        }
    }

    /// <summary>
    /// 获取是否有存档数据
    /// </summary>
    /// <returns></returns>
    private Dictionary<int, object> GetSaveData()
    {
        var path = Application.dataPath + "/Data/";
        Dictionary<int, object> dict = new Dictionary<int, object>();
        //文件夹是否存在
        DirectoryInfo myDirectoryInfo = new DirectoryInfo(path);
        if (!myDirectoryInfo.Exists)
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists(path + @"\" + "LevelData/Level.txt"))
        {
            StreamReader json = File.OpenText(path + @"\" + "LevelData/Level.txt");
            string input = json.ReadToEnd();
            dict = JsonMapper.ToObject<Dictionary<int, object>>(input);
        }
        return dict;
    }

    /// <summary>
    /// 显示继续游戏按钮
    /// </summary>
    private void ShowContinueGame()
    {
        btn_ContinueGame.transform.position = new Vector3(300, -534, 0);
    }

}
