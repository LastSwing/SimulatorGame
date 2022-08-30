using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DateTimeScript : MonoBehaviour
{
    float Times =0;
    public string InitialTime = "";
    public TimeSpan initialTime;
    private List<Sprite> sprites = new List<Sprite>();
    // Start is called before the first frame update
    void Start()
    {
        initialTime = TimeSpan.Parse(InitialTime);
       var objects = Resources.LoadAll(@"Image\UI\numbers");
        foreach (var item in objects)
        {
            if(item is Sprite)
                sprites.Add((Sprite)item);
        }
        string second = initialTime.Seconds.ToString();
        if (second.Length > 1)
        {
            transform.Find("second").GetComponent<Image>().sprite = sprites[Convert.ToInt32(second.Substring(0, 1))] as Sprite;
            transform.Find("second1").GetComponent<Image>().sprite = sprites[Convert.ToInt32(second.Substring(1, 1))] as Sprite;
        }
        else if (second.Length == 1)
        {
            transform.Find("second").GetComponent<Image>().sprite = sprites[0] as Sprite;
            transform.Find("second1").GetComponent<Image>().sprite = sprites[Convert.ToInt32(second.Substring(0, 1))] as Sprite;
        }
        string minutes = initialTime.Minutes.ToString();
        if (minutes.Length > 1)
        {
            transform.Find("min").GetComponent<Image>().sprite = sprites[Convert.ToInt32(minutes.Substring(0, 1))] as Sprite;
            transform.Find("min1").GetComponent<Image>().sprite = sprites[Convert.ToInt32(minutes.Substring(1, 1))] as Sprite;
        }
        else if (minutes.Length == 1)
        {
            transform.Find("min").GetComponent<Image>().sprite = sprites[0] as Sprite;
            transform.Find("min1").GetComponent<Image>().sprite = sprites[Convert.ToInt32(minutes.Substring(0, 1))] as Sprite;
        }
        string hours = initialTime.Hours.ToString();
        if (hours.Length > 1)
        {
            transform.Find("hour").GetComponent<Image>().sprite = sprites[Convert.ToInt32(hours.Substring(0, 1))] as Sprite;
            transform.Find("hour1").GetComponent<Image>().sprite = sprites[Convert.ToInt32(hours.Substring(1, 1))] as Sprite;
        }
        else if (hours.Length == 1)
        {
            transform.Find("hour").GetComponent<Image>().sprite = sprites[0] as Sprite;
            transform.Find("hour1").GetComponent<Image>().sprite = sprites[Convert.ToInt32(hours.Substring(0, 1))] as Sprite;
        }
    }
    void FixedUpdate()
    {
        Times += Time.deltaTime;
        if (Times >= 1)
        {
            initialTime = initialTime.Add(TimeSpan.FromSeconds(1));
            Times = 0;
            ChangeTime();
        }
    }

    void ChangeTime()
    {
        string second = initialTime.Seconds.ToString();
        if (second.Length > 1)
        {
            transform.Find("second").GetComponent<Image>().sprite = sprites[Convert.ToInt32(second.Substring(0, 1))] as Sprite;
            transform.Find("second1").GetComponent<Image>().sprite = sprites[Convert.ToInt32(second.Substring(1, 1))] as Sprite;
        }
        else if (second.Length == 1)
        {
            transform.Find("second").GetComponent<Image>().sprite = sprites[0] as Sprite;
            transform.Find("second1").GetComponent<Image>().sprite = sprites[Convert.ToInt32(second.Substring(0, 1))] as Sprite;
        }
        if (initialTime.Seconds == 0)
        {
            string minutes = initialTime.Minutes.ToString();
            if (minutes.Length > 1)
            {
                transform.Find("min").GetComponent<Image>().sprite = sprites[Convert.ToInt32(minutes.Substring(0, 1))] as Sprite;
                transform.Find("min1").GetComponent<Image>().sprite = sprites[Convert.ToInt32(minutes.Substring(1, 1))] as Sprite;
            }
            else if (minutes.Length == 1)
            {
                transform.Find("min").GetComponent<Image>().sprite = sprites[0] as Sprite;
                transform.Find("min1").GetComponent<Image>().sprite = sprites[Convert.ToInt32(minutes.Substring(0, 1))] as Sprite;
            }
            if (initialTime.Minutes == 0)
            { 
                string hours = initialTime.Hours.ToString();
                if (hours.Length > 1)
                {
                    transform.Find("hour").GetComponent<Image>().sprite = sprites[Convert.ToInt32(hours.Substring(0, 1))] as Sprite;
                    transform.Find("hour1").GetComponent<Image>().sprite = sprites[Convert.ToInt32(hours.Substring(1, 1))] as Sprite;
                }
                else if (hours.Length == 1)
                {
                    transform.Find("hour").GetComponent<Image>().sprite = sprites[0] as Sprite;
                    transform.Find("hour1").GetComponent<Image>().sprite = sprites[Convert.ToInt32(hours.Substring(0, 1))] as Sprite;
                }
            }
        }
    }
}
