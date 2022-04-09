using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningValue : MonoBehaviour
{
    // Start is called before the first frame update
    public Button iadd;
    public Button idel;
    public Text oname;
    public Text ovalue;
    public Image Image;
    public Text Odetail;
    public int layer;//²ãÊý
    public int Index;//±êÊ¶
    public int energyValue
    {
        get
        {
            int energy = 50;
            if (layer == 0)
                energy = 50;
            else
                energy = 100 * layer;
            return energy;
        }
        set
        {
            energyValue = value;
        }
    }
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
