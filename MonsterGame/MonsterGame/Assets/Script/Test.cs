using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            /*≤‚ ‘json∂¡»°
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("name","123");
            dic.Add("hp","40");
            dic.Add("atk","5");
            GameHelper.DataExport(dic, "test");
            dic.Clear();
            dic = GameHelper.DataRead("test.txt");
            */
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
