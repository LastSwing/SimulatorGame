using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CentreBtnScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        SendMessageUpwards("RockerDownClick");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SendMessageUpwards("RockerOnPointerUp");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
