using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningScene : MonoBehaviour
{
    public RectTransform _content;
    public GameObject itemPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Dictionary<string,string> opening = Attributes.Opening("",0);
        int index = 0;
        foreach (var item in opening)
        {
            itemPrefab.transform.position = new Vector3(0f,-400+(index*200),0f);
            OpeningValue opening1 = GameTools.AddChild(_content, itemPrefab).GetComponent<OpeningValue>();
            opening1.oname.text = item.Key;
            int _value = Convert.ToInt32(item.Value);
            opening1.layer = _value;
            switch (item.Key)
            {
                case "ÉñÍ¥Ñ¨":
                    opening1.ovalue.text = "¹¥»÷Á¦ + " + _value * 5;
                    break;
                case "×Ï¹¬Ñ¨":
                    opening1.ovalue.text = "±©»÷ + " + _value * 5;
                    break;
                case "ð¯Î²Ñ¨":
                    opening1.ovalue.text = "±©ÉË + " + _value * 0.05F;
                    break;
                case "Æø³åÑ¨":
                    opening1.ovalue.text = "ÉúÃüÖµ + " + _value * 40;
                    break;
                case "¹ØÔªÑ¨":
                    opening1.ovalue.text = "ÉúÃü»Ö¸´ + " + _value * 5;
                    break;
                case "ÖÐÊàÑ¨":
                    opening1.ovalue.text = "ÉÁ±Ü + " + _value * 5;
                    break;
                default:
                    break;
            }
            index++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// ¼ÓÔØÇÏÑ¨²ãÊý
    /// </summary>
    /// <param name="vector2"></param>
    /// <param name="Tier"></param>
    private void InsertEnergy(Vector2 vector2,int Tier)
    {
        
        GameObject Energy = (GameObject)Instantiate(Resources.Load("Prefabs/Energy"));
        GameObject EnergyValue = (GameObject)Instantiate(Resources.Load("Prefabs/EnergyValue"));
        Energy.transform.position  = vector2;
        EnergyValue.transform.localScale = new Vector2(Tier, 1);
        EnergyValue.transform.position = new Vector2(vector2.x-(315-Tier * 35), vector2.y);
        Energy.transform.parent = this.transform;
        EnergyValue.transform.parent = Energy.transform;
    }
}
