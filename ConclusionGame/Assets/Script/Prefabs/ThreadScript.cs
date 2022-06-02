using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThreadScript : MonoBehaviour
{
    public Text Text0;
    public Text Text1;
    public Text Text2;
    public Text Text3;
    public Button Thread0;
    public Button Thread1;
    public Button Thread2;
    public Button Thread3;
    public Button Skip;
    // Start is called before the first frame update
    void Start()
    {
        Thread0.onClick.AddListener(delegate { button_onClick(Thread0.name); });
        Thread1.onClick.AddListener(delegate { button_onClick(Thread1.name); });
        Thread2.onClick.AddListener(delegate { button_onClick(Thread2.name); });
        Thread3.onClick.AddListener(delegate { button_onClick(Thread3.name); });
        Skip.onClick.AddListener(delegate { button_onClick(Guid.Empty.ToString()); });
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    void button_onClick(string name)
    {
        SendMessageUpwards("ThreadClick", new Guid(name));
    }
}
