using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    private GameObject btn_GameStart, btn_Blood, btn_Acupoint;
    // Start is called before the first frame update
    void Start()
    {
        btn_GameStart = GameObject.Find("btn_GameStart");
        btn_GameStart.GetComponent<Button>().onClick.AddListener(delegate { SceneJump("GameScene"); });
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
