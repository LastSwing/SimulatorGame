using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject tempObject = Resources.Load("Prefab/Option") as GameObject;
        OptionScript Thread = (tempObject).GetComponent<OptionScript>();
        BaseHelper.AddChild(this.transform,tempObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
