using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgScript : MonoBehaviour
{
    public List<Vector2> vector2s;
    public Vector2 Any;//网格宽高个数
    public Text DeText;
    public Text LevelText;
    public int LevelNum = 1;
    // Start is called before the first frame update
    void Start()
    {
        LoadLevel(1);
    }

    void Update()
    {

    }
    public void LoadLevel(int levelNum)
    {
        Level level = DataRead.GetSeed(levelNum);
        LevelNum = levelNum;
        string Board;
        switch (level.LevelType)
        {
            case 0: Board = "Boarda"; break;
            case 1: Board = "Boardb"; break;
            case 2: Board = "Boardc"; break;
            case 3: Board = "Boardd"; break;
            case 4: Board = "Boarde"; break;
            default: Board = string.Empty; break;
        }
        GameObject BoardGame = Resources.Load("Prefabs/" + Board) as GameObject;
        Instantiate(BoardGame, transform);
        vector2s = BaseHelper.Addlocation(BoardGame.GetComponent<RectTransform>().sizeDelta, new Vector2(200, 200),out Any);
        DeText.text = "本次做出 " + level.Score + " 根羊肉串";
        LevelText.text = "第" + levelNum + "关";
        List<Seed> _seed = new List<Seed>();//将出口ImgNum单独拿出来写逻辑
        for (int i = 0; i < level.Seeds.Count; i++)
        {
            if (level.Seeds[i].IsRole)
            {
                GameObject RoleGame = Resources.Load("Prefabs/Role") as GameObject;
                Instantiate(RoleGame, transform);
                RoleGame = transform.Find("Role(Clone)").gameObject;
                RoleGame.GetComponent<Role>().StandardNum = level.Score;
                RoleGame.GetComponent<Role>().num.text = level.Seeds[i].Num.ToString();
                RoleGame.GetComponent<Role>().VictoryObj = transform.parent.Find("Victory").gameObject;
                RoleGame.GetComponent<Role>().LoseObj = transform.parent.Find("Lose").gameObject;
                RoleGame.GetComponent<Role>().Grid = Any;
                RoleGame.GetComponent<Role>().Location = level.Seeds[i].NumLocation;
                RoleGame.name = "Role"+ level.Seeds[i].NumLocation;
                RoleGame.transform.localPosition = vector2s[i];
            }
            else
            {
                GameObject ImgNum = Resources.Load("Prefabs/ImgNum") as GameObject;
                Instantiate(ImgNum, transform);
                ImgNum = transform.Find(ImgNum.name + "(Clone)").gameObject;
                ImgNum.name = "ImgNum" + level.Seeds[i].NumLocation;
                ImgNum.GetComponent<ImgNum>().num.text = level.Seeds[i].Num.ToString();
                ImgNum.GetComponent<ImgNum>().Color = level.Seeds[i].NumType;
                ImgNum.transform.localPosition = vector2s[i];
            }
            if (level.Seeds[i].IsExit == true)
            {
                _seed.Add(level.Seeds[i]);
            }
        }
        foreach (var items in _seed)
        {
            List<GameObject> roles = BaseHelper.GetAllSceneObjects(transform, true, false, "Role");
            foreach (var item in roles)
            {
                if (item.GetComponent<Role>().ExitLocation == null)
                {
                    item.GetComponent<Role>().ExitLocation = new Dictionary<int, int>();
                }
                item.GetComponent<Role>().ExitLocation.Add(items.NumLocation, items.ExitLocation);
                item.GetComponent<Role>().Location = item.GetComponent<Role>().Location;
            }
            Vector2 v = vector2s[items.NumLocation - 1];
            GameObject Exit = Resources.Load("Prefabs/Exit") as GameObject;
            Instantiate(Exit, transform);
            Exit = transform.Find("Exit(Clone)").gameObject;
            Exit.name = "Exit";
            if (items.ExitLocation == 0)
            {
                Exit.transform.localPosition = new Vector2(v.x, v.y + 200);
                Exit.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            else if (items.ExitLocation == 1)
            {
                Exit.transform.localPosition = new Vector2(v.x, v.y - 200);
                Exit.transform.localRotation = Quaternion.Euler(0, 0, 270);
            }
            else if (items.ExitLocation == 2)
            {
                Exit.transform.localPosition = new Vector2(v.x - 200, v.y);
                Exit.transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
            else if (items.ExitLocation == 3)
            {
                Exit.transform.localPosition = new Vector2(v.x + 200, v.y);
                Exit.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
    //parent：父物体的Transform;prefab：要添加的控件
    public GameObject AddChild(Transform parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.SetParent(parent, false);
            go.layer = parent.gameObject.layer;
        }
        return go;
    }

}
