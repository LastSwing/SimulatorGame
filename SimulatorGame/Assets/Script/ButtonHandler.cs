using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(ButtonClick);
    }
    public void ButtonClick()
    {
        Manager.instance.DoEventByID(int.Parse(gameObject.name));
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
