using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditMainScript : MonoBehaviour
{
    public Button Save;
    public Button PropBtn;
    public Button SceneBtn;
    public Button ObstacleBtn;
    public Button TerrainBtn;
    public Button Confirm;
    public Button ConfirmBtn;
    public Button ItemsBtn;
    public Button DelItemsBtn;
    public Text ItemsText;
    public InputField Width;
    public InputField Height;
    public InputField ItemWidth;
    public InputField ItemHeight;
    public InputField SpinX;
    public InputField SpinY;
    public InputField SpinZ;
    public GameObject UIList;
    public GameObject ItemsConnect;
    //摇杆
    public GameObject Rockel;
    public string m_PropName = "";
    public string AdoptName = "";//选中的障碍物或者移动物体
    private Vector2 screen;//屏幕分辨率
    private int LevelNum = 0;//关卡数
    private string Props = "";//使用道具
    private LoadProp LoadProp = new LoadProp();
    public int SceneID = 1;
    private bool Move = false;//true 物体移动，false 窗口移动
    public Camera Camera;
    void Start()
    {
        #region UI事件绑定
        Save.onClick.AddListener(SaveClick);
        PropBtn.onClick.AddListener(PropBtnClick);
        SceneBtn.onClick.AddListener(SceneClick);
        ObstacleBtn.onClick.AddListener(ObstacleClick);
        TerrainBtn.onClick.AddListener(TerrainClick);
        Confirm.onClick.AddListener(OConfirmClick);
        ConfirmBtn.onClick.AddListener(TConfirmClick);
        ItemsBtn.onClick.AddListener(ItemButtonClick);
        DelItemsBtn.onClick.AddListener(DelItemBtnClick);
        Width.onEndEdit.AddListener(delegate { EndEditWidth(); });
        Height.onEndEdit.AddListener(delegate { EndEditHeight(); });
        ItemHeight.onEndEdit.AddListener(delegate { EndItemEditHeiGht(); });
        ItemWidth.onEndEdit.AddListener(delegate { EndItemEditWidth(); });
        SpinX.onEndEdit.AddListener(delegate { EndItemEditX(); });
        SpinY.onEndEdit.AddListener(delegate { EndItemEditY(); });
        SpinZ.onEndEdit.AddListener(delegate { EndItemEditZ(); });
        #endregion
        #region UI赋值
        Width.text = GetComponent<RectTransform>().rect.width.ToString();
        Height.text = GetComponent<RectTransform>().rect.height.ToString();
        screen = new Vector2(Screen.width, Screen.height);
        UILoadPosition();
        //添加初始地形
        //添加集合中的地形
        GameObject game = Resources.Load("Prefabs/UI/PrefabItem") as GameObject;
        foreach (var item in Resources.LoadAll("Image/Terrain"))
        {
            if (item is Sprite && item.name == "Ground")
                game.GetComponent<PrefabItem>().button.image.sprite = item as Sprite;
        }
        game.name = "Ground";
        BaseHelper.AddChild(ItemsConnect.transform, game);
        GameObject game1 = Resources.Load("Prefabs/UI/PrefabItem") as GameObject;
        foreach (var item in Resources.LoadAll("Image/Terrain"))
        {
            if (item is Sprite && item.name == "Pillar")
                game1.GetComponent<PrefabItem>().button.image.sprite = item as Sprite;
        }
        game1.name = "Pillar";
        BaseHelper.AddChild(ItemsConnect.transform, game1);
        GameObject game2 = Resources.Load("Prefabs/UI/PrefabItem") as GameObject;
        foreach (var item in Resources.LoadAll("Image"))
        {
            if (item is Sprite && item.name == "Role")
                game2.GetComponent<PrefabItem>().button.image.sprite = item as Sprite;
        }
        game2.name = "Role";
        BaseHelper.AddChild(ItemsConnect.transform, game2);
        GameObject game3 = Resources.Load("Prefabs/UI/PrefabItem") as GameObject;
        foreach (var item in Resources.LoadAll("Image/UI"))
        {
            if (item is Sprite && item.name == "Room0")
                game3.GetComponent<PrefabItem>().button.image.sprite = item as Sprite;
        }
        game3.name = "Indoor";
        BaseHelper.AddChild(ItemsConnect.transform, game3);
        #endregion
        #region 场景加载
        GameObject gameObject = GameObject.Find("Scenes").gameObject;
        List<Scene> scenes = ReadData.GetScene();
        for (int i = 0; i < scenes.Count; i++)
        {
            GameObject sce = Resources.Load(@"Prefabs\UI\SceneBtn") as GameObject;
            sce.transform.name = "scenes" + scenes[i].SceneID.ToString();
            ScenesScript script = sce.GetComponent<ScenesScript>();
            script.ScenesID = scenes[i].SceneID;
            script.Text.text = "场景" + script.ScenesID;
            if (script.ScenesID != 1)
                script.Hide();
            BaseHelper.AddChild(gameObject.transform, sce);
            gameObject.transform.Find("scenes" + scenes[i].SceneID.ToString() + "(Clone)").localPosition = new Vector2(0, 125 - (i * 50));
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 vector = Rockel.GetComponent<RockerScript>().SmallRectVector;
        if (vector != Vector2.zero)
        {
            if (Move)//物体移动
            {
                Transform tf = transform.Find(AdoptName);
                tf.localPosition = new Vector2(tf.localPosition.x + (vector.x * 0.002f), tf.localPosition.y + (vector.y * 0.002f));
            }
            else
            {
                UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x + (vector.x * 0.01f), UIList.transform.localPosition.y + (vector.y * 0.01f));
                transform.localPosition = new Vector2(transform.localPosition.x - (vector.x * 0.01f), transform.localPosition.y - (vector.y * 0.01f));
            }
        }
        float wheel = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 100;
        Camera.transform.localScale += Vector3.one * wheel;
        UIList.transform.localScale = new Vector3(transform.parent.transform.localPosition.z / 100, transform.parent.transform.localPosition.z / 100, 1f);
    }

    #region UI事件
    /// <summary>
    /// 保存关卡数据
    /// </summary>
    void SaveClick()
    {
        List<Level> list = ReadData.GetLevels();
        Debug.Log("第" + (list.Count + 1) + "关保存中。。。");
        LevelNum = list.Count + 1;
        Level level = new Level();
        level.LevelNum = LevelNum;
        level.SceneID = SceneID;
        level.Size = transform.GetComponent<RectTransform>().sizeDelta;
        level.Props = Props;
        level.Special = false;
        list.Add(level);
        ReadData.SaveLevel(list);
        List<GameObject> gameObjects = BaseHelper.GetAllSceneObjects(transform, true, false, "");
        List<LevelDetail> levelDetails = new List<LevelDetail>();
        foreach (var item in gameObjects)
        {
            //判断是精灵还是UI
            if (item.name != "UIS")
            {
                LevelDetail levelDetail = new LevelDetail();
                levelDetail.LevelID = LevelNum;
                levelDetail.IsView = false;
                levelDetail.IsMotion = false;
                levelDetail.Location = item.transform.localPosition;
                //levelDetail.Size = item.GetComponent<RectTransform>().sizeDelta;
                levelDetail.Size = item.transform.localScale;
                levelDetail.Rotation = item.transform.localRotation.eulerAngles;
                levelDetail.IsHide = item.activeSelf;
                levelDetail.Gravity = -1;
                levelDetail.AirDrag = -1;
                levelDetail.Mass = -1;
                string name = item.name.Replace("(Clone)", "");
                if (item.name.LastIndexOf("-") >0)
                    name = item.name.Substring(0, item.name.LastIndexOf("-"));
                levelDetail.DetailName = name;
                levelDetail.DetailType = 0;
                levelDetails.Add(levelDetail);
            }
        }
        ReadData.SaveLevelDetail(levelDetails, LevelNum);
        Debug.Log("新增关卡成功");
    }

    /// <summary>
    /// 添加道具
    /// </summary>
    void PropBtnClick()
    {
        Transform props = UIList.transform.Find("Props");
        List<GameObject> gameObjects = BaseHelper.GetAllSceneObjects(props, true, false, "");
        if (!m_PropName.Equals(string.Empty))
        {
            GameObject go = new GameObject();
            go.name = m_PropName;
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            go.AddComponent<Image>().sprite = BaseHelper.LoadFromImage(new Vector2(100, 100), Application.dataPath + "/Resources/Image/PropImage/" + LoadProp.PropDict[m_PropName].Image);
            BaseHelper.AddChild(props, go);
            props.Find(m_PropName + "(Clone)").GetComponent<RectTransform>().localPosition = new Vector2(-450 + (gameObjects.Count >= 10 ? (gameObjects.Count - 10) * 100 : gameObjects.Count * 100), gameObjects.Count >= 10 ? -50 : 50);
            props.Find(m_PropName + "(Clone)").name = m_PropName;
            if (gameObjects.Count == 0)
                Props += m_PropName;
            else
                Props += "," + m_PropName;
        }
    }
    /// <summary>
    /// 关卡宽度改变
    /// </summary>
    void EndEditWidth()
    {
        if (float.TryParse(Width.text, out float width))
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(width, GetComponent<RectTransform>().rect.height);
            UILoadPosition();
        }
    }
    /// <summary>
    /// 关卡高度改变
    /// </summary>
    void EndEditHeight()
    {
        if (float.TryParse(Height.text, out float height))
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().rect.width, height);
            UILoadPosition();
        }
    }
    /// <summary>
    /// 添加场景
    /// </summary>
    void SceneClick()
    { 
        
    }
    /// <summary>
    /// 关卡显示障碍物
    /// </summary>
    void ObstacleClick()
    {
        UIList.transform.Find("Obstacle").gameObject.SetActive(true);
    }
    /// <summary>
    /// 关卡显示地形
    /// </summary>
    void TerrainClick()
    {
        UIList.transform.Find("Terrain").gameObject.SetActive(true);
    }
    /// <summary>
    /// 关卡添加障碍物
    /// </summary>
    void OConfirmClick()
    {
        string aname = UIList.transform.Find("Obstacle/StoreView").GetComponent<StoreView>().AdoptName.Replace("(Clone)","");
        if (!string.IsNullOrEmpty(aname))
        {
            //找到障碍物预制体，添加地形
            GameObject go = Resources.Load("Prefabs/Barrier/" + aname) as GameObject;
            go.transform.localPosition = new Vector2((transform.localPosition.x * -1), (transform.localPosition.y * -1));
            //查看是否已添加重复障碍物，有则给编号
            List<GameObject> games = BaseHelper.GetAllSceneObjects(ItemsConnect.transform,true,false, aname);
            if (games.Count > 0)
                go.name = aname+"-"+games.Count.ToString();
            BaseHelper.AddChild(transform, go);
            UIList.transform.Find("Obstacle").gameObject.SetActive(false);
            //添加集合中的障碍物
            GameObject gameObject = Resources.Load("Prefabs/UI/PrefabItem") as GameObject;
            foreach (var item in Resources.LoadAll("Image/Barrier"))
            {
                if (item is Sprite && item.name == aname)
                    gameObject.GetComponent<PrefabItem>().button.image.sprite = item as Sprite;
            }
            gameObject.name = aname;
            if (games.Count > 0)
                gameObject.name = aname + "-" + games.Count.ToString();
            BaseHelper.AddChild(ItemsConnect.transform, gameObject);
        }
    }

    /// <summary>
    /// 关卡添加地形
    /// </summary>
    void TConfirmClick()
    {
        string aname = UIList.transform.Find("Terrain/StoreView").GetComponent<StoreView>().AdoptName.Replace("(Clone)", "");
        if (!string.IsNullOrEmpty(aname))
        {
            GameObject go = Resources.Load("Prefabs/Terrain/" + aname) as GameObject;
            //查看是否已添加重复地形，有则给编号
            List<GameObject> games = BaseHelper.GetAllSceneObjects(ItemsConnect.transform, true, false, aname);
            if (games.Count > 0)
                go.name = aname + "-" + games.Count.ToString();
            go.transform.localPosition = new Vector2((transform.localPosition.x * -1), (transform.localPosition.y * -1));
            BaseHelper.AddChild(transform, go);
            UIList.transform.Find("Terrain").gameObject.SetActive(false);
            //添加集合中的地形
            GameObject gameObject = Resources.Load("Prefabs/UI/PrefabItem") as GameObject;
            foreach (var item in Resources.LoadAll("Image/Terrain"))
            {
                if (item is Sprite && item.name == aname)
                    gameObject.GetComponent<PrefabItem>().button.image.sprite = item as Sprite;
            }
            gameObject.name = aname;
            if (games.Count > 0)
                gameObject.name = aname + "-" + games.Count.ToString();
            BaseHelper.AddChild(ItemsConnect.transform, gameObject);
        }
    }

    /// <summary>
    /// 物体编辑宽度
    /// </summary>
    void EndItemEditWidth()
    {
        Transform tf = transform.Find(AdoptName);
        if (tf != null)
            tf.localScale = new Vector2(float.Parse(ItemWidth.text),tf.localScale.y);
    }
    /// <summary>
    /// 物体编辑高度
    /// </summary>
    void EndItemEditHeiGht()
    {
        Transform tf = transform.Find(AdoptName);
        if(tf != null)
            tf.localScale = new Vector2(tf.localScale.x, float.Parse(ItemHeight.text));
    }

    /// <summary>
    /// 物体移动
    /// </summary>
    void ItemButtonClick()
    {
        if (ItemsText.text == "移动")
        {
            ItemsText.text = "取消移动";
            Move = true;
        }
        else
        {
            ItemsText.text = "移动";
            Move=false;
        }
    }

    /// <summary>
    /// 删除物体
    /// </summary>
    void DelItemBtnClick()
    {
        if (!AdoptName.Equals(string.Empty))
        { 
            transform.Find(AdoptName).gameObject.SetActive(false);
            ItemsConnect.transform.Find(AdoptName).gameObject.SetActive(false);
        }
    }

    void EndItemEditX()
    {
        Transform tf = transform.Find(AdoptName);
        if (tf != null)
            tf.localEulerAngles = new Vector3(float.Parse(SpinX.text), tf.localEulerAngles.y, tf.localEulerAngles.z);
    }
    void EndItemEditY()
    {
        Transform tf = transform.Find(AdoptName);
        if (tf != null)
            tf.localEulerAngles = new Vector3(tf.localEulerAngles.x, float.Parse(SpinY.text), tf.localEulerAngles.z);
    }
    void EndItemEditZ()
    {
        Transform tf = transform.Find(AdoptName);
        if (tf != null)
            tf.localEulerAngles = new Vector3(tf.localEulerAngles.x, tf.localEulerAngles.y, float.Parse(SpinZ.text));
    }
    #endregion

    /// <summary>
    /// UI出现位置
    /// </summary>
    void UILoadPosition()
    {
        //背景位置根据分辨率改变
        transform.localPosition = new Vector2(GetComponent<RectTransform>().rect.width / 2 - screen.x / 2, transform.Find("Indoor(Clone)").localPosition.y * -1);
        //按钮位置根据分辨率改变
        UIList.transform.localPosition = transform.Find("Indoor(Clone)").localPosition;
    }

    /// <summary>
    /// 选中物体改变
    /// </summary>
    public void ItemsChange()
    { 
        Transform tf = transform.Find(AdoptName);
        ItemHeight.text = tf.localScale.y.ToString();
        ItemWidth.text = tf.localScale.x.ToString();
        SpinX.text = tf.localEulerAngles.x.ToString();
        SpinY.text = tf.localEulerAngles.y.ToString();
        SpinZ.text = tf.localEulerAngles.z.ToString();
    }
}
