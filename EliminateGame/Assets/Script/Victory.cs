using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Victory : MonoBehaviour
{
    public Button Victorybtn;
    public Button Remove;
    // Start is called before the first frame update
    void Start()
    {
        Victorybtn.onClick.AddListener(VbtnClick);
        Remove.onClick.AddListener(RbtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void VbtnClick()
    { 
        gameObject.SetActive(false);
        transform.parent.gameObject.SetActive(false);
        transform.parent.parent.Find("Bg").gameObject.SetActive(true);
    }
    void RbtnClick()
    { 
        
    }
}
