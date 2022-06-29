using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleScript : MonoBehaviour
{
    public float Value 
    {
        get { return m_Value; }
        set { m_Value = value; }
    }
    private float m_Value = 0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
    }

}
