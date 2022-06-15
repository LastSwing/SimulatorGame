using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingScene : MonoBehaviour
{
    Button btn_GameOver, btn_CardList, btn_AllSkillReturn;
    GameObject AllSkillCanvas;
    void Start()
    {
        btn_GameOver = transform.Find("Content/btn_GameOver").GetComponent<Button>();
        btn_GameOver.onClick.AddListener(GameOver);
        btn_CardList = transform.Find("Content/btn_CardList").GetComponent<Button>();
        btn_CardList.onClick.AddListener(SkillClick);

        AllSkillCanvas = GameObject.Find("AllSkillCanvas");
    }

    public void GameOver()
    {        
        Common.SceneJump("PlayerDieScene");
    }

    public void SkillClick()
    {
        AllSkillCanvas.SetActive(true);
        btn_AllSkillReturn = GameObject.Find("AllSkillCanvas/btn_Return").GetComponent<Button>();
        btn_AllSkillReturn.onClick.RemoveAllListeners();
        btn_AllSkillReturn.onClick.AddListener(returnClick);
        transform.gameObject.SetActive(false);
    }
    public void returnClick()
    {
        AllSkillCanvas.SetActive(false);
        transform.gameObject.SetActive(true);
    }
}
