using Assets.Script;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningScene : MonoBehaviour
{
    public RectTransform _content;
    public GameObject itemPrefab, btn_Return;
    public Text Energyindex;
    /// <summary>
    /// ����ֵ
    /// </summary>
    public int Energy { get; set; }
    private List<OpeningValue> openings;
    // Start is called before the first frame update
    void Start()
    {
        Dictionary<string,string> opening = Attributes.Opening("",0);
        openings = new List<OpeningValue>();
        int index = 0;
        Energy = 100000;
        Energyindex.text = Energy.ToString();
        foreach (var item in opening)
        {
            btn_Return.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("MainScene"); });
            itemPrefab.transform.position = new Vector2(0f,650-(index*200));
            OpeningValue opening1 = GameTools.AddChild(_content, itemPrefab).GetComponent<OpeningValue>(); //
            opening1.oname.text = item.Key;
            opening1.iadd.onClick.AddListener(OnaddChlick);
            opening1.iadd.tag = index.ToString();
            opening1.idel.onClick.AddListener(OndelChlick);
            opening1.idel.tag = index.ToString();
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

    public void OnaddChlick()
    {
        var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        OpeningValue opening1 = openings[Convert.ToInt32(button.transform.tag)];
        if (Energy >= opening1.energyValue && opening1.layer < 10)
        {
            Energy = Energy - opening1.energyValue;
            Energyindex.text = Energy.ToString();
            opening1.layer += 1;
            Change(opening1);
            List<OpeningValue> open = new List<OpeningValue>();
            open.Add(opening1);
            openings.RemoveRange(Convert.ToInt32(button.transform.tag), 1);
            openings.InsertRange(Convert.ToInt32(button.transform.tag), open);
        }
    }
    public void OndelChlick()
    {
        var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        OpeningValue opening1 = openings[Convert.ToInt32(button.transform.tag)];
        if (opening1.layer > 0)
        {
            opening1.layer -= 1;
            Energy = Energy + opening1.energyValue;
            Energyindex.text = Energy.ToString();
            Change(opening1);
            List<OpeningValue> open = new List<OpeningValue>();
            open.Add(opening1);
            openings.RemoveRange(Convert.ToInt32(button.transform.tag), 1);
            openings.InsertRange(Convert.ToInt32(button.transform.tag), open);
        }
        Debug.Log("del");
    }
    private OpeningValue Change(OpeningValue opening)
    {
        switch (opening.oname.text)
        {
            case "��ͥѨ":
                opening.ovalue.text = "������ + " + opening.layer * 5;
                break;
            case "�Ϲ�Ѩ":
                opening.ovalue.text = "���� + " + opening.layer * 5;
                break;
            case "�βѨ":
                opening.ovalue.text = "���� + " + opening.layer * 0.05F;
                break;
            case "����Ѩ":
                opening.ovalue.text = "����ֵ + " + opening.layer * 40;
                break;
            case "��ԪѨ":
                opening.ovalue.text = "�����ָ� + " + opening.layer * 5;
                break;
            case "����Ѩ":
                opening.ovalue.text = "���� + " + opening.layer * 5;
                break;
            default:
                break;
        }
        opening.Odetail.text = "��Ѩ���ȣ�" + opening.layer*10 + "%\n\r" + (opening.energyValue == 1000?"MAX": opening.energyValue.ToString()) + "����ֵ";
        return opening;
    }
}
