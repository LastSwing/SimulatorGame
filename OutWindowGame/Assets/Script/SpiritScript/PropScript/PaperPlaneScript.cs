using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperPlaneScript : MonoBehaviour
{
    public RoleScript Role;
    Rigidbody2D Rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Role = GameObject.Find("Role").GetComponent<RoleScript>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            Role.DropProp();
        }
    }
}
