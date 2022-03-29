using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
    private GameObject btn_Return;
    // Start is called before the first frame update
    void Start()
    {
        btn_Return = GameObject.Find("btn_Return");
        btn_Return.GetComponent<Button>().onClick.AddListener(delegate { SceneJump("MainScene"); });
    }

    // Update is called once per frame
    void Update()
    {

    }
    void SceneJump(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}
