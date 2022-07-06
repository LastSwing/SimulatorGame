using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyScript : MonoBehaviour
{
    public RoleScript Role = null;
    public RockerScript Rocker = null;
    private Rigidbody2D rigidbody2D;
    void Start()
    {
        rigidbody2D =  GetComponent<Rigidbody2D>();
        Role = GameObject.Find("Role").GetComponent<RoleScript>();
        GameObject gameObject = GameObject.Find("Rockel");
        Rocker = gameObject.GetComponent<RockerScript>();
    }
    public Vector2 Vector = new Vector2(8f,79f);
    // Update is called once per frame
    void Update()
    {
        if (Rocker != null && Rocker.SmallRectVector != Vector2.zero)
        {
            Vector2 vector = Rocker.SmallRectVector;
            rigidbody2D.AddForce(new Vector2(vector.x>0?2:-2, 0));
        }
        if (Role != null)
        {
            Role._Prop = true;
            Role.Vector2 = new Vector3(transform.localPosition.x - Vector.x,transform.localPosition.y-Vector.y);
        }
    }
}
