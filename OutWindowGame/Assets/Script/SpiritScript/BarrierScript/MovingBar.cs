using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBar : MonoBehaviour
{
    /// <summary>
    /// 角度
    /// </summary>
    public float Angle = 180;
    /// <summary>
    /// 周期 0.02s*Cycle
    /// </summary>
    public float Cycle = 150;
    private bool cut = false;
    private float _cycle = 0;
    private Rigidbody2D rb;
    bool leave = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _cycle = Cycle;
    }
    private void FixedUpdate()
    {
        if (transform.parent.name == "MainView(Clone)" && GameObject.Find("Role").GetComponent<RoleScript>().leave)
            leave = true;
        else
            leave = false;
        if (leave)
        {
            if (_cycle == 0)
            {
                cut = true;
                //周期开始时给予角度随机
                Angle = Random.Range(-180f, 180f);
            }
            else if (_cycle == Cycle)
                cut = false;
            Vector3 vector3;
            if (cut)
            {
                vector3 = Movement.Angle(1f, Angle, 1f);
                rb.AddForce(vector3);
                _cycle++;
            }
            else
            {
                vector3 = Movement.Angle(1f, Angle, 1f);
                rb.AddForce(vector3);
                _cycle--;
            }
            if (vector3.x >= 0)
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            else
                transform.localRotation = Quaternion.Euler(180, 0, 180);
        }
    }
}
