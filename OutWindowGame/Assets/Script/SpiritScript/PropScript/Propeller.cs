using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
    /// <summary>
    /// 飞行角度 0平飞1左飞2右飞
    /// </summary>
    public void FlyAngle(int FlyAngle)
    {
        if (FlyAngle == 0)
        {
            transform.eulerAngles = new Vector3(270, transform.eulerAngles.y, transform.eulerAngles.z);
            transform.RotateAround(transform.position, new Vector3(0, 5, 0), 40);//平飞
        }
        else if (FlyAngle == 1)
        {
            transform.eulerAngles =new Vector3(270,transform.eulerAngles.y, transform.eulerAngles.z);
            transform.RotateAround(transform.position, new Vector3(-3, 5, 0), 40);//左飞
        }
        else if (FlyAngle == 2)
        {
            transform.eulerAngles = new Vector3(270, transform.eulerAngles.y, transform.eulerAngles.z);
            transform.RotateAround(transform.position, new Vector3(1, 4, 0), 40);//右飞
        }
    }
}
