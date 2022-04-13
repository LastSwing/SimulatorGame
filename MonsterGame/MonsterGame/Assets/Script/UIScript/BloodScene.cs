using Assets.Script;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;

public class BloodScene : MonoBehaviour
{
    public RectTransform _Connect;
    private GameObject btn_Return;
    public GameObject txtPanel;
    public Text txt_Detail,txt_nv,txt_kf,txt_fx,txt_gg,txt_pg,txt_cy;
    private Dictionary<string, string> keyValues;
    // Start is called before the first frame update
    void Start()
    {
        btn_Return = GameObject.Find("btn_Return");
        btn_Return.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("MainScene"); });
        txtPanel.transform.position = new Vector3(542, -3020, 0);
        keyValues = Attributes.Blood("");
        foreach (var item in keyValues)
        {
            if (item.Key == "盘古血脉")
                txt_pg.text = string.Format("盘古({0}/9)",item.Value);
            if (item.Key == "女娲血脉")
                txt_nv.text = string.Format("女娲({0}/9)", item.Value);
            if (item.Key == "伏羲血脉")
                txt_fx.text = string.Format("伏羲({0}/9)", item.Value);
            if (item.Key == "共工血脉")
                txt_gg.text = string.Format("共工({0}/9)", item.Value);
            if (item.Key == "蚩尤血脉")
                txt_cy.text = string.Format("蚩尤({0}/9)", item.Value);
            if (item.Key == "夸父血脉")
                txt_kf.text = string.Format("夸父({0}/9)", item.Value);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HideText()
    {
        txtPanel.transform.position = new Vector3(542, -3020, 0);
    }
    public void ShowText(string Name)
    {
        List<SecretClass> secrets = SecretArea.LoadBlood;
        foreach (var item in secrets)
        {
            if (item.Classify.Equals(Name))
            {
                int value = Convert.ToInt32(keyValues[item.Name]);
                string result = "";
                string result1 = "";
                if (value == 0)
                {
                    if (item.Numerical.Length == 2)
                    {
                        result = string.Format(item.Value, "0", "0");
                        result1 = string.Format(item.Value, item.Numerical[0], item.Numerical[1]);
                    }
                    else if (item.Numerical.Length == 1)
                    {
                        result = string.Format(item.Value, "0");
                        result1 = string.Format(item.Value, item.Numerical[0]);
                    }
                }
                else
                {
                    if (item.Numerical.Length == 2)
                    {
                        result = string.Format(item.Value, item.Numerical[0]+(item.Increment[0]*(value-1)), item.Numerical[1] + (item.Increment[1] * (value - 1)));
                        result1 = string.Format(item.Value, item.Numerical[0] + (item.Increment[0] * (value)), item.Numerical[1] + (item.Increment[1] * (value)));
                    }
                    else if (item.Numerical.Length == 1)
                    {
                        result = string.Format(item.Value, item.Numerical[0] + (item.Increment[0] * (value - 1)));
                        result1 = string.Format(item.Value, item.Numerical[0] + (item.Increment[0] * (value)));
                    }
                }
                    txt_Detail.text = item.Name+"："+ value + "/9。\n 效果："+ result + "。\n 下层效果："+ result1;
            }
        }
        txtPanel.transform.position = _Connect.position;
        txt_Detail.transform.position = _Connect.position;
    }
}
