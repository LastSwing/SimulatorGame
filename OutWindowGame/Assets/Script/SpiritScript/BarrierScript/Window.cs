using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//窗口绑定脚本
public class Window : MonoBehaviour
{
    public int IsCollision = -1;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //碰撞触发后，3秒物体消失
        if (IsCollision >= 0 && IsCollision<150)
            IsCollision++;
        if (IsCollision == 150)
        {
            IsCollision = -1;
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Role")
        {
            IsCollision = 0;
            List<GameObject> gameObjects = BaseHelper.GetAllSceneObjects(transform.parent,true,false,"");
            foreach (var item in gameObjects)
            {
                item.GetComponent<Window>().IsCollision = 0;
            }
        }
    }
}
