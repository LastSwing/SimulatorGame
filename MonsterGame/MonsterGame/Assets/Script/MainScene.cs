using Assets.Script;
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
        btn_GameStart.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("GameScene"); });
        btn_Blood = GameObject.Find("btn_Blood");
        btn_Blood.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("BloodScene"); });
        btn_Acupoint = GameObject.Find("btn_Acupoint");
        btn_Acupoint.GetComponent<Button>().onClick.AddListener(delegate { Common.SceneJump("AcupointScene"); });
    }

    // Update is called once per frame
    void Update()
    {

    }

}
