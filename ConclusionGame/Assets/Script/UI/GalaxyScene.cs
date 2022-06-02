using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalaxyScene : MonoBehaviour
{
    public Button button0;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;
    public Button button6;
    public Button Error;
    public Text Intro;
    public Text Money;
    public Text Force;
    public Text Output;
    public Transform Rtransform;
    private  List<Vector3> StarVect = new List<Vector3>();
    private List<Star> stars;
    private List<Galaxy> galaxies;
    private string Gbutton;//上个点击的button名称
    // Start is called before the first frame update
    void Start()
    {
        Load();
        button0.onClick.AddListener(delegate { ChangeGalaxyClick(button0); });
        button1.onClick.AddListener(delegate { ChangeGalaxyClick(button1); });
        button2.onClick.AddListener(delegate { ChangeGalaxyClick(button2); });
        button3.onClick.AddListener(delegate { ChangeGalaxyClick(button3); });
        button4.onClick.AddListener(delegate { ChangeGalaxyClick(button4); });
        button5.onClick.AddListener(delegate { ChangeGalaxyClick(button5); });
        button6.onClick.AddListener(delegate { ChangeGalaxyClick(button6); });
        Error.onClick.AddListener(ErrorClick);
        ChangeGalaxyClick(button0);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void Load()
    {
        StarVect.Add(new Vector3(-610,311.5F));
        StarVect.Add(new Vector3(-310, 311.5F));
        StarVect.Add(new Vector3(-10, 311.5F));
        StarVect.Add(new Vector3(290, 311.5F));
        StarVect.Add(new Vector3(590, 311.5F));
        StarVect.Add(new Vector3(-610, 0));
        StarVect.Add(new Vector3(-310, 0));
        StarVect.Add(new Vector3(-10, 0));
        StarVect.Add(new Vector3(290, 0));
        StarVect.Add(new Vector3(590, 0));
        galaxies = JsonHelper.JsonToGalaxy();
        stars = JsonHelper.JsonToStar();
    }
    void ChangeGalaxyClick(Button button)
    {
        button0.image.color = new Color(255, 255, 255, 15);
        button1.image.color = new Color(255, 255, 255, 15);
        button2.image.color = new Color(255, 255, 255, 15);
        button3.image.color = new Color(255, 255, 255, 15);
        button4.image.color = new Color(255, 255, 255, 15);
        button5.image.color = new Color(255, 255, 255, 15);
        button6.image.color = new Color(255, 255, 255, 15);
        button.image.color = new Color(255, 255, 255, 0);
        int count = Convert.ToInt32(button.name.Substring(button.name.Length-1,1));
        Intro.text = galaxies[count].Intro;
        List<Star> starList = new List<Star>();
        foreach (var star1 in stars)
        {
            if(star1.Galaxy == galaxies[count].Name)
                starList.Add(star1);
        }
        //删除上次产生的物体
        int i = Rtransform.childCount;
        while (i > 0)
        {
            i--;
            Destroy(Rtransform.GetChild(i).gameObject);
        }
        //循环生成星球
        for (int j = 0; j < starList.Count; j++)
        {
            GameObject star = Resources.Load("Prefab/Star") as GameObject;
            star.name = "star" + j;
            StarScript starScript = star.GetComponent<StarScript>();
            starScript.button.image.sprite = PersonImage.ImageToName(Application.dataPath + "/Resources/Image/star", starList[j].Image + ".png", 250, 250);
            starScript.text.text = starList[j].Name;
            starScript.button.onClick.AddListener(() => StaronClick(star));
            star.transform.localPosition = StarVect[j];
            BaseHelper.AddChild(Rtransform, star);
            //Rtransform.Find($"star{j}(Clone)").transform.localPosition = StarVect[j];
            GameObject.Find($"Canvas/Panel/body/Subordinate/star{j}(Clone)").transform.localPosition = StarVect[j];
        }
    }
    void ErrorClick()
    {
        BaseHelper.SceneJump("HomeScene",1);
    }
    void StaronClick(GameObject star)
    {
        StarScript starScript = star.GetComponent<StarScript>();
        starScript.jianjiao.transform.localScale = new Vector3(-9, 80);
        starScript.qinlue.transform.localScale = new Vector3(-9, 33);
    }
}
