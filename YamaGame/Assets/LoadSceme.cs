using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceme : MonoBehaviour
{
    public Text Lifetime, Lifetimes, Jtestimony,Name, Dard, Lifetimetrue, Stestimony, Sdescribed, Jdescribed, Correct;
    public Button Button;
    // Start is called before the first frame update
    void Start()
    {
        Button.onClick.AddListener(OnSaveClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnSaveClick()
    {
        Dictionary<string, string> keyValues = JsonHelper.DataRead("sortjson.txt");
        string _case = "case_" + (keyValues.Count + 1).ToString()+"";
        if (keyValues.ContainsKey(Name.text)) 
        {
            Debug.Log("已有同名档案，保存失败");
            return;
        }
        keyValues.Add(Name.text, _case);
        JsonHelper.DataExport(keyValues, "sortjson");
        Dictionary<string, string> keys = new Dictionary<string, string>();
        keys.Add("Lifetime", Lifetime.text);
        keys.Add("Lifetimes", Lifetimes.text);
        keys.Add("Jtestimony", Jtestimony.text);
        keys.Add("Name", Name.text);
        keys.Add("Dard", Dard.text);
        keys.Add("Lifetimetrue", Lifetimetrue.text);
        keys.Add("Stestimony", Stestimony.text);
        keys.Add("Sdescribed", Sdescribed.text);
        keys.Add("Jdescribed", Jdescribed.text);
        keys.Add("Correct", Correct.text);
        JsonHelper.DataExport(keys, _case);
        Debug.Log("保存成功");
        Lifetime.text = Lifetimes.text = Jtestimony.text = Name.text = Dard.text = Lifetimetrue.text = Stestimony.text = Jdescribed.text = Correct.text = "";
    }
}
