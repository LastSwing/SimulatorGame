using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UAVScript : MonoBehaviour
{
    public RoleScript Role;
    public UIScript UIScript;
    Rigidbody2D Rigidbody;
    public RockerScript Rocker = null;
    public Vector2 Vector = new Vector2(1f, 68f);
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Role = GameObject.Find("Role").GetComponent<RoleScript>();
        UIScript = GameObject.Find("UI").GetComponent<UIScript>();
        GameObject gameObject = GameObject.Find("Rockel");
        Rocker = gameObject.GetComponent<RockerScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Role != null)
        {
            if (Rocker != null && Rocker.SmallRectVector != Vector2.zero)
            {
                Vector2 vector = Rocker.SmallRectVector;
                Rigidbody.AddForce(new Vector2(vector.x / 5, vector.y / 5));
            }
            else
            {
                Rigidbody.velocity = Vector2.zero;
            }
            if (Role != null)
            {
                Role._Prop = true;
                //Role.Vector2 = new Vector3(transform.localPosition.x - Vector.x, transform.localPosition.y - Vector.y);
            }
        }
    }
}
