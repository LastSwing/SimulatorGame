using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileidScript : MonoBehaviour
{
    public Text NewText;
    public Text EndText;
    public Text Text1;
    public Text Text2;
    public Text Text3;
    public Text Text4;
    public Text Text5;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;
    public Image Image;
    // Start is called before the first frame update
    void Start()
    {
        button1.onClick.AddListener(delegate { onClick(Text1.text); });
        button2.onClick.AddListener(delegate { onClick(Text2.text); });
        button3.onClick.AddListener(delegate { onClick(Text3.text); });
        button4.onClick.AddListener(delegate { onClick(Text4.text); });
        button5.onClick.AddListener(delegate { onClick(Text5.text); });
    }

    // Update is called once per frame
    void Update()
    {
    }

    void onClick(string text)
    {
        SendMessageUpwards("FileidClick",text);
    }
}
