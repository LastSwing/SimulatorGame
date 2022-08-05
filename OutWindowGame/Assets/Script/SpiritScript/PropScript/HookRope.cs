using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookRope : MonoBehaviour
{
    private SpriteRenderer SpriteRenderer;
    private Rigidbody2D Rigidbody;
    private float a = 12f;
    public bool Lengthen = true;//是否延长
    float Angle;//摆动角度值
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Rigidbody = GetComponent<Rigidbody2D>();
        Angle = ((180-transform.rotation.eulerAngles.z)*2)+ transform.rotation.eulerAngles.z;
    }
    private void FixedUpdate()
    {
        if (Lengthen)
        {
            //SpriteRenderer.size = new Vector2(SpriteRenderer.size.x, SpriteRenderer.size.y + 0.1f);
            //transform.localPosition = new Vector2(transform.localPosition.x + 0.75f, transform.localPosition.y + 1.3f);
        }
        else
        {
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Resist")
        {
            Lengthen = false;
        }
    }
}
