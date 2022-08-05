using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScript : MonoBehaviour
{
    public Button button;
    public GameObject biankuan;
    public Text Text;
    public Image xingji;
    public Image image;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(ButtonClick);
    }
    void ButtonClick()
    {
        biankuan.SetActive(true);
        transform.parent.parent.gameObject.GetComponent<HomePage>().UpdateSelect(button.name);
    }
    public void Conceal()
    {
        biankuan.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
