using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EditView : BaseUI
{
    public GameObject UIList;
    public GameObjectPool GameObjectPool;
    public GameObject Role;
    public Button Save;
    public Button PropBtn;
    public bool IsMove = true;
    public bool IsDownUp = true;
    public string m_PropName = "";
    public int SceneID = 1;

    private Vector2 screen;//屏幕分辨率
    private int LevelNum = 0;//关卡数
    private string Props = "";//使用道具
    private LoadProp LoadProp = new LoadProp();
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (IsMove)
        {
            if (Role.transform.localPosition.x > (transform.localPosition.x * -1))//角色处于右半部
            {
                float move = transform.localPosition.x - (Role.transform.localPosition.x * -1);
                transform.localPosition = new Vector2(transform.localPosition.x - move, transform.localPosition.y);
                UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x + move, UIList.transform.localPosition.y);
            }
        }
        if (IsDownUp)
        {
            float a = (transform.localPosition.y * -1 + screen.y * 0.25f);
            float b = transform.localPosition.y * -1;
            if (Role.transform.localPosition.y > a)//角色处于上半部四分之一
            {
                float move = (Role.transform.localPosition.y - a);
                transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - move);
                UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x, UIList.transform.localPosition.y + move);
            }
            else if (Role.transform.localPosition.y < b)//角色处于下半部
            {
                float move = (Role.transform.localPosition.y - b);
                transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - move);
                UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x, UIList.transform.localPosition.y + move);
            }
        }
        if (transform.localPosition.x * -1 >= (GetComponent<RectTransform>().rect.width / 2 - screen.x / 2))//已经到右尽头
        {
            IsMove = false;
            //transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width / 2 - screen.x / 2) * -1, transform.localPosition.y);
            //UIList.transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width * -1 / 2) + screen.x - (UIList.GetComponent<RectTransform>().rect.width / 2) * -1, UIList.transform.localPosition.y);
        }
        else if (transform.localPosition.x >= (GetComponent<RectTransform>().rect.width / 2 - screen.x / 2))//已经到左尽头
        {
            transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width / 2 - screen.x / 2), transform.localPosition.y);
            UIList.transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width * -1 / 2) + screen.x - (UIList.GetComponent<RectTransform>().rect.width / 2), UIList.transform.localPosition.y);
        }
        if (transform.localPosition.y <= (GetComponent<RectTransform>().rect.width / 2 - screen.x / 2) * -1)//已经到上尽头
        {
            transform.localPosition = new Vector2(transform.localPosition.x, (GetComponent<RectTransform>().rect.height / 2 - screen.y / 2) * -1);
            UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x, (GetComponent<RectTransform>().rect.height / 2 - UIList.GetComponent<RectTransform>().rect.height / 2));
        }
        else if (transform.localPosition.y >= (GetComponent<RectTransform>().rect.height / 2 - screen.y / 2))//已经到下尽头
        {
            transform.localPosition = new Vector2(transform.localPosition.x, GetComponent<RectTransform>().rect.height / 2 - screen.y / 2);
            UIList.transform.localPosition = new Vector2(UIList.transform.localPosition.x, (GetComponent<RectTransform>().rect.height / 2 - UIList.GetComponent<RectTransform>().rect.height / 2) * -1);
        }

    }
    /// <summary>
    /// 保存关卡数据
    /// </summary>
    void SaveClick()
    {
        List<Level> list = ReadData.GetLevels();
        Debug.Log("第"+(list.Count + 1)+"关保存中。。。");
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
            if (item.name != "UIA")
            {
                LevelDetail levelDetail = new LevelDetail();
                levelDetail.LevelID = LevelNum;
                levelDetail.IsView = false;
                levelDetail.IsMotion = false;
                levelDetail.Location = item.transform.localPosition;
                //levelDetail.Size = item.GetComponent<RectTransform>().sizeDelta;
                levelDetail.Size = item.GetComponent<Renderer>().bounds.size;
                levelDetail.Rotation = item.transform.localRotation.eulerAngles;
                levelDetail.IsHide = item.activeSelf;
                levelDetail.Gravity = -1;
                levelDetail.AirDrag = -1;
                levelDetail.Mass = -1;
                levelDetail.DetailName = item.name;
                levelDetail.DetailType = 0;
                levelDetails.Add(levelDetail);
            }
        }
        ReadData.SaveLevelDetail(levelDetails, LevelNum);
        Debug.Log("新增关卡成功");
    }

    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitComponent()
    {
        Save.onClick.AddListener(SaveClick);
        PropBtn.onClick.AddListener(PropBtnClick);
        //因为获取组件以及绑定事件一般只需要做一次，所以放在OnInit
        screen = new Vector2(Screen.width, Screen.height);
        //背景位置根据分辨率改变
        transform.localPosition = new Vector2(GetComponent<RectTransform>().rect.width / 2 - screen.x / 2, GetComponent<RectTransform>().rect.height / 2 - screen.y / 2);
        //按钮位置根据分辨率改变
        UIList.transform.localPosition = new Vector2((GetComponent<RectTransform>().rect.width * -1 / 2) + screen.x - (UIList.GetComponent<RectTransform>().rect.width / 2), UIList.transform.localPosition.y);
        #region 场景加载
        GameObject gameObject = UIList.transform.Find("Scenes").gameObject;
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
            GameObjectPool.GetObject(sce, gameObject.transform);
            gameObject.transform.Find("scenes" + scenes[i].SceneID.ToString()+"(Clone)").localPosition = new Vector2(0, 125 - (i * 50));
        }
        #endregion
    }
    /// <summary>
    /// 添加道具
    /// </summary>
    void PropBtnClick()
    {
        Transform props = UIList.transform.Find("Props");
        List<GameObject> gameObjects = BaseHelper.GetAllSceneObjects(props,true,false,"");
        if (!m_PropName.Equals(string.Empty))
        {
            GameObject go = new GameObject();
            go.name = m_PropName;
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            go.AddComponent<Image>().sprite = BaseHelper.LoadFromImage(new Vector2(100, 100), Application.dataPath + "/Resources/Image/PropImage/" + LoadProp.PropDict[m_PropName].Image);
            GameObjectPool.GetObject(go, props);
            props.Find(m_PropName + "(Clone)").GetComponent<RectTransform>().localPosition = new Vector2(-450 + (gameObjects.Count >= 10 ? (gameObjects.Count - 10) * 100 : gameObjects.Count * 100), gameObjects.Count >= 10 ? -50 : 50);
            props.Find(m_PropName + "(Clone)").name = m_PropName;
            if (gameObjects.Count == 0)
                Props += m_PropName;
            else
                Props += ","+m_PropName;
        }
    }
    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitUIevent()
    {
        GameObjectPool = new GameObjectPool();
    }

    public override void OnOpen()
    {
        //数据需要每次打开都要刷新，UI状态也是要每次打开都进行刷新，因此放在OnOpen
        InitUIData();
        InitUIState();
    }

    /// <summary>
    /// 更新UI状态
    /// </summary>
    private void InitUIState()
    {

    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void InitUIData()
    {

        //测试用例，实际需接入获取到的玩家数据
        //todo
    }
    public override void OnClose()
    {
        //todo
    }

    public override void OnInit()
    {
        InitUIevent();
        InitComponent();
    }
}
