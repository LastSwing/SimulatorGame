using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapScene : MonoBehaviour
{
    LineRenderer line;
    List<Vector3> listPath = new List<Vector3>();

    Image img_map;

    int MapRow = 1;
    int OneRowY = -1640;

    void Start()
    {
        img_map = transform.Find("Map/img_map").GetComponent<Image>();


        line = transform.Find("Map/line_Obj").GetComponent<LineRenderer>();
        var list = iTweenPath.GetPath("Test");
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = new Vector3(575 - (i * 50), 210 + (i * 36), 0);
        }
        listPath = iTween.GetCrvePaths(list);

        //line.SetVertexCount(listPath.Count);
        //line.SetWidth(0.1f, 0.1f);
        line.positionCount = listPath.Count;
        line.endWidth = 5;
        line.startWidth = 5;
        for (int i = 0; i < listPath.Count; i++)
        {
            line.SetPosition(i, listPath[i]);
        }
        //line.receiveShadows = false;
        //MovePath("Test");
    }


    void Update()
    {

        //第一行玩家
        if (MapRow < 13)
        {
            CreateMap(0);
        }
        //boss
        if (MapRow == 13)
        {
            CreateMap(99);
        }
        //boss
        if (MapRow == 12)
        {
            CreateMap(1);
        }
    }

    void MovePath(string path)
    {
        Hashtable ht = new Hashtable();
        ht.Add("time", 10);
        ht.Add("movetopath", false);
        ht.Add("easetype", iTween.EaseType.linear);
        ht.Add("path", iTweenPath.GetPath(path));
        iTween.MoveTo(line.gameObject, ht);
    }

    /// <summary>
    /// 生成地图行
    /// </summary>
    /// <param name="type">0普通地图，99boss,1商店</param>
    public void CreateMap(int type)
    {
        //地图长度13*220
        //宽度最大4，最小2
        //商店2-4,商店在3-5行遇到
        //奇遇2-4
        //一行一行进行渲染

        #region 普通地图
        if (type == 0)
        {
            //4-6必出商店
            GameObject tempObject = Resources.Load("Prefab/Map_Row") as GameObject;
            tempObject = Common.AddChild(img_map.transform, tempObject);
            tempObject.name = "Map_Row" + MapRow;
            tempObject.transform.localPosition = new Vector2(0, OneRowY + MapRow * 240);
            //当前行战斗数量
            int rowCount = Random.Range(2, 5);
            for (int i = 0; i < rowCount; i++)
            {
                int atkType = Random.Range(1, 11);
                if (MapRow == 1)
                {
                    atkType = 1;
                }
                else if (MapRow > 4 && MapRow < 6)
                {
                    atkType = Random.Range(6, 16);
                }
                else if (MapRow > 4 && MapRow < 11)
                {
                    atkType = Random.Range(1, 12);
                }
                //普通地图
                if (atkType < 8)
                {
                    //图片生成
                    int imgIndex = Random.Range(0, 4);
                    GameObject Atkimg = Resources.Load("Prefab/Map_Atk_img") as GameObject;
                    Atkimg = Common.AddChild(tempObject.transform, Atkimg);
                    Atkimg.name = "Atk_img" + i;
                    var tempImg = tempObject.transform.Find($"Atk_img{i}").GetComponent<Image>();
                    Common.ImageBind("Images/Map_Atk" + imgIndex, tempImg);
                }
                //秘境
                else if (atkType < 11)
                {
                    //图片生成
                    int imgIndex = Random.Range(0, 2);
                    GameObject Atkimg = Resources.Load("Prefab/Map_Atk_img") as GameObject;
                    Atkimg = Common.AddChild(tempObject.transform, Atkimg);
                    Atkimg.name = "Adventure_img" + i;
                    var tempImg = tempObject.transform.Find($"Adventure_img{i}").GetComponent<Image>();
                    Common.ImageBind("Images/Map_Adventrue" + imgIndex, tempImg);
                }
                //商城
                else
                {
                    //图片生成
                    GameObject Atkimg = Resources.Load("Prefab/Map_Atk_img") as GameObject;
                    Atkimg = Common.AddChild(tempObject.transform, Atkimg);
                    Atkimg.name = "Atk_imgShop" + i;
                    var tempImg = tempObject.transform.Find($"Atk_imgShop{i}").GetComponent<Image>();
                    Common.ImageBind("Images/Map_Shop", tempImg);
                }
            }
            MapRow++;
        }
        #endregion
        #region Boss
        else if (type == 99)
        {
            //图片生成
            GameObject Atkimg = Resources.Load("Prefab/Map_Atk_img") as GameObject;
            Atkimg = Common.AddChild(img_map.transform, Atkimg);
            Atkimg.name = "Atk_imgBoss";
            Atkimg.transform.localPosition = new Vector2(0, OneRowY + MapRow * 240 + 70);
            var tempImg = img_map.transform.Find($"Atk_imgBoss").GetComponent<Image>();
            var rect = img_map.transform.Find($"Atk_imgBoss").GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(500, 250);
            Common.ImageBind("Images/Map_AtkBoss", tempImg);
            MapRow++;
        }
        #endregion
        #region 商店
        else if (type == 1)
        {
            GameObject tempObject = Resources.Load("Prefab/Map_Row") as GameObject;
            tempObject = Common.AddChild(img_map.transform, tempObject);
            tempObject.name = "Map_RowShop";
            tempObject.transform.localPosition = new Vector2(0, OneRowY + MapRow * 240);
            //当前行战斗数量
            int rowCount = Random.Range(2, 5);
            for (int i = 0; i < rowCount; i++)
            {
                //图片生成
                GameObject Atkimg = Resources.Load("Prefab/Map_Atk_img") as GameObject;
                Atkimg = Common.AddChild(tempObject.transform, Atkimg);
                Atkimg.name = "Atk_imgShop" + i;
                var tempImg = tempObject.transform.Find($"Atk_imgShop{i}").GetComponent<Image>();
                Common.ImageBind("Images/Map_Shop", tempImg);
            }
            MapRow++;
        }
        #endregion
    }

}
