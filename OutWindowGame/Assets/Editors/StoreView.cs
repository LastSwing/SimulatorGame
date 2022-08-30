using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StoreView : MonoBehaviour
{
    public GameObject Connect;
    public string AdoptName = "";
    void Start()
    {
        if (transform.parent.name == "Obstacle")
        {
           var Images = Resources.LoadAll("Image/Barrier");
            if (Images != null)
            {
                foreach (var img in Images)
                {
                    if (img is Sprite)
                    {
                        GameObject gameObject = Resources.Load("Prefabs/UI/PrefabItem") as GameObject;
                        gameObject.name = img.name;
                        gameObject.GetComponent<PrefabItem>().button.image.sprite = img as Sprite;
                        BaseHelper.AddChild(Connect.transform, gameObject);
                    }
                }
            }
        }
        else if(transform.parent.name == "Terrain")
        {
            var Images = Resources.LoadAll("Image/Terrain");
            if (Images != null)
            {
                foreach (var img in Images)
                {
                    if (img is Sprite)
                    {
                        GameObject gameObject = Resources.Load("Prefabs/UI/PrefabItem") as GameObject;
                        gameObject.name = img.name;
                        gameObject.GetComponent<PrefabItem>().button.image.sprite = img as Sprite;
                        BaseHelper.AddChild(Connect.transform, gameObject);
                    }
                }
            }
        }
    }
    void Update()
    {
        
    }
}
