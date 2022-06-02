using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScript : MonoBehaviour
{
    public Button GalaxyBtn;
    // Start is called before the first frame update
    void Start()
    {
        GalaxyBtn.onClick.AddListener(GalaxyBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GalaxyBtnClick()
    {
        BaseHelper.SceneJump("GalaxyScene",1);
    }
}
