using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public List<Vector2> vector2s;
    private Vector2 vector;
    // Start is called before the first frame update
    void Start()
    {
        vector2s = Addlocation(transform.GetComponent<RectTransform>().sizeDelta, new Vector2(200, 200));
        //transform.Find("Role").GetComponent<Role>().Grid = vector;
        //for (int i = 0; i < vector2s.Count; i++)
        //{

        //    GameObject gameObject1 = Resources.Load("Prefabs/ImgNum") as GameObject;
        //    string name = "ImgNum"+(i+1);
        //    gameObject1.name = name;
        //    GameObject.Instantiate(gameObject1, transform);
        //    transform.Find(name + "(Clone)").transform.localPosition = vector2s[i];
        //    transform.Find(name + "(Clone)").name = name;
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 生成网格位置
    /// </summary>
    /// <param name="WidthHeight">大网格宽高</param>
    /// <param name="SmallWidthHeight">小网格宽高</param>
    List<Vector2> Addlocation(Vector2 WidthHeight, Vector2 SmallWidthHeight)
    {
        List<Vector2> location = new List<Vector2>();
        int shu = (int)Math.Round(WidthHeight.x / SmallWidthHeight.x);//横向数
        int heng = (int)Math.Round(WidthHeight.y / SmallWidthHeight.y);//纵向数
        vector = new Vector2(heng,shu);
        for (int i = shu; i > 0; i--)
        {
            float y = (SmallWidthHeight.y * heng / 2) + (SmallWidthHeight.y / 2) - (i * SmallWidthHeight.y);
            for (int j = heng; j > 0; j--)
            {
                float x = (SmallWidthHeight.x * shu / 2) + (SmallWidthHeight.x / 2) - (j * SmallWidthHeight.x);
                location.Add(new Vector2(x, y));
            }
        }
        return location;
    }
}
