using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb;
    int cut = 0;
    float Angel = 0;
    bool leave = false;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = "Bullet";
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
        if (transform.parent.parent.parent.name == "MainView(Clone)" && GameObject.Find("Role").GetComponent<RoleScript>().leave)
            leave = true;
        else
        {
            leave = false;
        }
        if (leave)
        {
            if (cut == 250)
            {
                B_Destroy();
            }
            else
            {
                Angel = transform.eulerAngles.z * -1;
                cut++;
                float eur = Angel * Mathf.Deg2Rad;
                float forceX = (float)(0.8f * Math.Sin(eur));
                rb.AddForce(new Vector2(forceX, 0.8f), ForceMode2D.Impulse);
            }
        }
    }
    /// <summary>
    /// 销毁子弹
    /// </summary>
    public void B_Destroy()
    {
        cut = 0;
        gameObject.SetActive(false);
        transform.parent.GetComponent<Bpipe>().gameObjects.Add(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        B_Destroy();
    }
}
