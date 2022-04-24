using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionScript : MonoBehaviour
{
    public Dictionary<int, string> keys;
    private List<ThreadScript> threads;
    private RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        keys = new Dictionary<int, string>();
        keys.Add(1, "测试1");
        keys.Add(2, "测试2");
        keys.Add(3, "测试3");
        keys.Add(4, "测试4");
        threads = new List<ThreadScript>();
        int i = 0;
        foreach (var item in keys)
        {
            GameObject tempObject = Resources.Load("Prefab/Therad") as GameObject;
            tempObject = BaseHelper.AddChild(this.transform, tempObject);
            ThreadScript Thread = (tempObject).GetComponent<ThreadScript>();
            rect = (tempObject).GetComponent<RectTransform>();
            //List<int> num = BaseHelper.Meanlocation(Convert.ToInt32(rect.offsetMin.y * 2), 35,4);
            //rect.offsetMin = new Vector2(rect.offsetMin.x, num[i]+ rect.offsetMin.y);
            //rect.offsetMax = new Vector2(rect.offsetMax.x, -390+num[i]);
            i++;
            Thread.button.tag = i.ToString();
            Thread.Text.text = item.Value;
            Thread.button.onClick.AddListener(OnChlick);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnChlick()
    {
        var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        for (int i = 0; i < keys.Count; i++)
        {
            ThreadScript thread = threads[i];
            GameObject.Destroy(thread.gameObject);
        }
        this.gameObject.SetActive(false);
    }
}
