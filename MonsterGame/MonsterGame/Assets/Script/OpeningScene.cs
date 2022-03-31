using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
            Dictionary<string,string> opening = Attributes.Opening("",0);
        foreach (var item in opening)
        {
            Debug.Log(item.Key + "   " + item.Value + "\n");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// º”‘ÿ«œ—®≤„ ˝
    /// </summary>
    /// <param name="vector2"></param>
    /// <param name="Tier"></param>
    private void InsertEnergy(Vector2 vector2,int Tier)
    {
        
        GameObject Energy = (GameObject)Instantiate(Resources.Load("Prefabs/Energy"));
        GameObject EnergyValue = (GameObject)Instantiate(Resources.Load("Prefabs/EnergyValue"));
        Energy.transform.position  = vector2;
        EnergyValue.transform.localScale = new Vector2(Tier, 1);
        EnergyValue.transform.position = new Vector2(vector2.x-(315-Tier * 35), vector2.y);
        Energy.transform.parent = this.transform;
        EnergyValue.transform.parent = Energy.transform;
    }
}
