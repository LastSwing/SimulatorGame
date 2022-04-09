using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningScene : MonoBehaviour
{
    public RectTransform _content;
    public GameObject itemPrefab;
    public Text Energyindex;
    /// <summary>
    /// 能量值
    /// </summary>
    public int Energy { get; set; }
    private List<OpeningValue> openings;
    // Start is called before the first frame update
    void Start()
    {
        Dictionary<string,string> opening = Attributes.Opening("",0);
        openings = new List<OpeningValue>();
        int index = 0;
        Energy = 1000;
        Energyindex.text = Energy.ToString();
        foreach (var item in opening)
        {
            itemPrefab.transform.position = new Vector3(0f,-400+(index*200));
            OpeningValue opening1 = GameTools.AddChild(_content, itemPrefab).GetComponent<OpeningValue>();
            opening1.oname.text = item.Key;
            opening1.iadd.onClick.AddListener(() => OnaddChlick(index));
            opening1.idel.onClick.AddListener(() => OndelChlick(index));
            int _value = Convert.ToInt32(item.Value);
            opening1.layer = _value;
            opening1.Index = index;
            Change(opening1);
            openings.Add(opening1);
            index++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnaddChlick(int i)
    {
        var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        OpeningValue opening1 = openings[i-1];
        if (Energy >= opening1.energyValue && opening1.layer < 10)
        {
            Energy = Energy - opening1.energyValue;
            Energyindex.text = Energy.ToString();
            opening1.layer += 1;
            Change(opening1);
            List<OpeningValue> open = new List<OpeningValue>();
            open.Add(opening1);
            openings.RemoveRange(i-1, 1);
            openings.InsertRange(i-1, open);
        }
        Debug.Log("add");
    }
    public void OndelChlick(int i)
    {
        OpeningValue opening1 = openings[i-1];
        if (opening1.layer > 0)
        {
            opening1.layer -= 1;
            Energy = Energy + opening1.energyValue;
            Energyindex.text = Energy.ToString();
            Change(opening1);
            List<OpeningValue> open = new List<OpeningValue>();
            open.Add(opening1);
            openings.RemoveRange(i-1, 1);
            openings.InsertRange(i-1, open);
        }
        Debug.Log("del");
    }
    private OpeningValue Change(OpeningValue opening)
    {
        switch (opening.oname.text)
        {
            case "神庭穴":
                opening.ovalue.text = "攻击力 + " + opening.layer * 5;
                break;
            case "紫宫穴":
                opening.ovalue.text = "暴击 + " + opening.layer * 5;
                break;
            case "鸠尾穴":
                opening.ovalue.text = "暴伤 + " + opening.layer * 0.05F;
                break;
            case "气冲穴":
                opening.ovalue.text = "生命值 + " + opening.layer * 40;
                break;
            case "关元穴":
                opening.ovalue.text = "生命恢复 + " + opening.layer * 5;
                break;
            case "中枢穴":
                opening.ovalue.text = "闪避 + " + opening.layer * 5;
                break;
            default:
                break;
        }
        opening.Odetail.text = "所需能量值\r" + opening.energyValue + "\n(" + opening.layer + "/10)";
        return opening;
    }
}
