using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImgNum : MonoBehaviour
{
    public int Location;
    public Text num;
    public Sprite[] sprites;
    private int _color;
    public int Num;
    /// <summary>
    /// 0+,1-,2*,3/
    /// </summary>
    public int Color { 
        get { return _color; }
        set
        {
            _color = value;
            gameObject.GetComponent<Image>().sprite = sprites[_color];
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<Image>().sprite.name == "Blue")
            Color = 0;
        else if (gameObject.GetComponent<Image>().sprite.name == "Green")
            Color = 1;
        else if (gameObject.GetComponent<Image>().sprite.name == "Yellow")
            Color = 2;
        else if (gameObject.GetComponent<Image>().sprite.name == "Red")
            Color = 3;
        else if(gameObject.GetComponent<Image>().sprite.name == "Protect")
            Color = 4;
        else if (gameObject.GetComponent<Image>().sprite.name == "Devour")
            Color = 5;
        else if (gameObject.GetComponent<Image>().sprite.name == "Obstacle")
            Color = 6;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
