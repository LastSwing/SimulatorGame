using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;

public class BloodScene : MonoBehaviour
{
    private GameObject btn_Return,txtPanel;
    private Text txt_Detail;
    // Start is called before the first frame update
    void Start()
    {
        btn_Return = GameObject.Find("btn_Return");
        btn_Return.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("MainScene"); });
        txt_Detail = GameObject.Find("txt_Detail").GetComponent<Text>();
        txtPanel = GameObject.Find("TxtPanel");
        txtPanel.transform.position = new Vector3(542, -3020, 0);
        //txt_Detail.transform.position = new Vector3(542, -3020, 0);
        //txt_Detail.gameObject.activeInHierarchy = true;
        //Debug.Log(txt_Detail.gameObject.activeInHierarchy);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HideText()
    {
        txtPanel.transform.position = new Vector3(542, -3020, 0);
        //txt_Detail.transform.position = new Vector3(542,-3020, 0);
    }
    public void ShowText(string Name)
    {
        switch (Name)
        {
            case "NvWa":
                txt_Detail.text = "女娲：1/9。\n 效果：增加20点永久生命值，每秒回复3点生命值。\n 下层效果：增加50点永久生命值，每秒回复5点生命值。";
                break;
            case "PanGu":
                txt_Detail.text = "盘古";
                break;
            case "KuaFu":
                txt_Detail.text = "夸父";
                break;
            case "FuXi":
                txt_Detail.text = "伏羲";
                break;
            case "GongGong":
                txt_Detail.text = "共工";
                break;
            case "ChiYou":
                txt_Detail.text = "蚩尤";
                break;
            default:
                txt_Detail.text = "敬请期待";
                break;
        }
        txtPanel.transform.position = new Vector3(300, 600, 0);
        //txt_Detail.transform.position = new Vector3(300, 600, 0);
    }
}
