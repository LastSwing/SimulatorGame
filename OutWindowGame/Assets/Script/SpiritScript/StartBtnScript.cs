using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartBtnScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        SendMessageUpwards("StartBtnDownClick");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SendMessageUpwards("StartBtnUpClick");
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
