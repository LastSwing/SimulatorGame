using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
    public Vector2 Vector = new Vector2(-36f, 4f);
    public RoleScript Role = null;
    public RockerScript Rocker = null;
    private Rigidbody2D rigidbody2D;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        Role = GameObject.Find("Role").GetComponent<RoleScript>();
        GameObject gameObject = GameObject.Find("Rockel");
        Rocker = gameObject.GetComponent<RockerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Role != null)
        {
            Role._Prop = true;
            //Role.Vector2 = new Vector3(transform.localPosition.x - Vector.x, transform.localPosition.y - Vector.y);
        }
    }
    public void Moment()
    {
        if (Rocker != null && Rocker.SmallRectVector != Vector2.zero)
        {
            Vector2 vector = Rocker.SmallRectVector;
            if(vector.x>=0)
                rigidbody2D.AddForce(new Vector2(vector.x*0.2f,vector.y*0.03f), ForceMode2D.Impulse);
        }
    }
}
