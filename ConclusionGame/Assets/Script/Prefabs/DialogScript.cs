using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogScript : MonoBehaviour
{
    public Image image;
    public Text Text;
    public Button Button;
    // Start is called before the first frame update
    void Start()
    {
        Button.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnClick()
    {
        SendMessageUpwards("DialogClick", new Guid(Button.name));
    }
}
