using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenesScript : MonoBehaviour
{
    public Button button;
    public Text Text;
    public GameObject Image;
    public int ScenesID = 0;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(BtnClick);
    }
    void BtnClick()
    {
        Image.SetActive(true);
        transform.parent.parent.parent.GetComponent<EditView>().SceneID = ScenesID;
        List<GameObject> list = BaseHelper.GetAllSceneObjects(transform.parent,false,false,"");
        foreach (GameObject obj in list)
        {
            if (obj.name != transform.name)
                obj.GetComponent<ScenesScript>().Hide();
        }
    }
    public void Hide()
    {
        Image.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
