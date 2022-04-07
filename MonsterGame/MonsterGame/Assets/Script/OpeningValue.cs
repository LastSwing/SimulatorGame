using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningValue : MonoBehaviour
{
    // Start is called before the first frame update
    public Button iadd;
    public Button idel;
    public Text oname;
    public Text ovalue;
    public Image Image;
    public int layer;
    void Start()
    {
        iadd.onClick.AddListener(OnaddChlick);
        idel.onClick.AddListener(OndleChlick);

    }
    public void OnaddChlick()
    { 
        
    }
    public void OndleChlick()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
