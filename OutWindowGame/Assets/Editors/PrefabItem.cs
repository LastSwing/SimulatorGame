using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabItem : MonoBehaviour
{
    public Button button;
    public GameObject Image;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(ButtonClick);
    }

    void ButtonClick()
    {
        ItemShow();
    }
    void ItemShow()
    {
        ItemShow(true);
        List<GameObject> list = BaseHelper.GetAllSceneObjects(transform.parent,true,false, "");
        foreach (GameObject obj in list)
        {
            if (obj.name != this.name)
                obj.GetComponent<PrefabItem>().ItemShow(false);
        }
        if (transform.parent.name == "Content")
            transform.parent.parent.parent.GetComponent<StoreView>().AdoptName = this.name;
        else if (transform.parent.name == "ItemsContent")
        {
            GameObject.Find("EditMain").GetComponent<EditMainScript>().AdoptName = this.name;
            GameObject.Find("EditMain").GetComponent<EditMainScript>().ItemsChange();
        }

    }
    public void ItemShow(bool IsTrue)
    {
        Image.SetActive(IsTrue);
    }
}
