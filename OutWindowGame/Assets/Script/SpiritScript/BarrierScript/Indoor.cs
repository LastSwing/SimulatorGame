using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indoor : MonoBehaviour
{
    List<Vector2> vector2s = new List<Vector2>();
    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> gameObjects = BaseHelper.GetAllSceneObjects(transform.Find("Window"),true,false,"");
        foreach (GameObject obj in gameObjects)
        {
            vector2s.Add(obj.transform.localPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 重置窗口位置
    /// </summary>
    public void Restart()
    {
        List<GameObject> gameObjects = BaseHelper.GetAllSceneObjects(transform.Find("Window"), false, false, "");
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].GetComponent<Rigidbody2D>().gravityScale = 0;
            gameObjects[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            gameObjects[i].transform.eulerAngles = Vector3.zero;
            gameObjects[i].transform.localPosition = vector2s[i];
            gameObjects[i].SetActive(true);
        }
        foreach (var item in gameObjects)
        {
            item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            item.GetComponent<Rigidbody2D>().gravityScale = 2;
        }
    }
}
