using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperPlaneScript : MonoBehaviour
{
    public RoleScript Role;
    public UIScript UIScript;
    Rigidbody2D Rigidbody;
    private float speed = 10;
    private float angle = 60;
    private bool IsAngle = false;
    private Vector2 Vector1;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Role = GameObject.Find("Role").GetComponent<RoleScript>();
        UIScript = GameObject.Find("UI").GetComponent<UIScript>();
        Vector1 = (Vector2)transform.localPosition + new Vector2(1000, 500);
    }
    public Vector2 Vector = new Vector2(-6f, -133f);
    private void FixedUpdate()
    {
        if (Rigidbody != null)
        {
            Rigidbody.AddForce(new Vector2(speed,0),ForceMode2D.Force);
        }
        if (Role != null)
        {
            Role.Vector2 = new Vector3(transform.localPosition.x - Vector.x, transform.localPosition.y - Vector.y);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            Role.DropProp();
            UIScript.RrecycleProp();
        }
    }
}
