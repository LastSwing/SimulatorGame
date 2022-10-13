using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgScript : BaseUI
{
    public List<Vector2> vector2s;
    public Vector2 Any;//网格宽高个数
    public Text DeText;
    public Text LevelText;
    public int LevelNum = 1;
    public Transform Bgtransform;
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
        Instantiate(BoardGame, Bgtransform);
        vector2s = BaseHelper.Addlocation(BoardGame.GetComponent<RectTransform>().sizeDelta, new Vector2(200, 200),out Any);
        DeText.text = "本次做出 " + level.Score + " 根羊肉串";
        LevelText.text = "第" + levelNum + "关";
        List<Seed> _seed = new List<Seed>();//将出口ImgNum单独拿出来写逻辑
        for (int i = 0; i < level.Seeds.Count; i++)
        {
            if (level.Seeds[i].IsRole)
            {
                GameObject RoleGame = Resources.Load("Prefabs/Role") as GameObject;
                Instantiate(RoleGame, Bgtransform);
                RoleGame = Bgtransform.Find("Role(Clone)").gameObject;
                RoleGame.GetComponent<Role>().StandardNum = level.Score;
                RoleGame.GetComponent<Role>().num.text = level.Seeds[i].Num.ToString();
                RoleGame.GetComponent<Role>().VictoryObj = transform.Find("Victory").gameObject;
                RoleGame.GetComponent<Role>().LoseObj = transform.Find("Lose").gameObject;
                RoleGame.GetComponent<Role>().Grid = Any;
                RoleGame.GetComponent<Role>().Location = level.Seeds[i].NumLocation;
                RoleGame.name = "Role"+ level.Seeds[i].NumLocation;
                RoleGame.transform.localPosition = vector2s[i];
            }
            else
            {
                GameObject ImgNum = Resources.Load("Prefabs/ImgNum") as GameObject;
                Instantiate(ImgNum, Bgtransform);
                ImgNum = Bgtransform.Find(ImgNum.name + "(Clone)").gameObject;
                ImgNum.name = "ImgNum" + level.Seeds[i].NumLocation;
                if(level.Seeds[i].NumType == 0)
                    ImgNum.GetComponent<ImgNum>().num.text = level.Seeds[i].Num.ToString();//"+"+
                else if (level.Seeds[i].NumType == 1)
                    ImgNum.GetComponent<ImgNum>().num.text = level.Seeds[i].Num.ToString();//"-" + 
                else if (level.Seeds[i].NumType == 2)
                    ImgNum.GetComponent<ImgNum>().num.text =  level.Seeds[i].Num.ToString();//"×" +
                else if (level.Seeds[i].NumType == 3)
                    ImgNum.GetComponent<ImgNum>().num.text =  level.Seeds[i].Num.ToString();//"÷" +
                else
                    ImgNum.GetComponent<ImgNum>().num.text = "";
                ImgNum.GetComponent<ImgNum>().Num = level.Seeds[i].Num;
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
            List<GameObject> roles = BaseHelper.GetAllSceneObjects(Bgtransform, true, false, "Role");
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
            Instantiate(Exit, Bgtransform);
            Exit = Bgtransform.Find("Exit(Clone)").gameObject;
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

    public override void OnInit()
    {

    }

    public override void OnOpen()
    {
        LoadLevel(DataRead.GetLevel());
    }

    public override void OnClose()
    {

    }
}
