using Assets.Script.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : SingletonMonoBehaviour<ResourcesManager>
{
    /// <summary>
    /// 资源路径字典，初始化的时候将所有动态资源的路径全部获取到这里
    /// </summary>
    /// 最好走配表的方式，这里先直接填充
    Dictionary<string, string> urlDic = new Dictionary<string, string>()
    {
        {"SoundItem","SoundItem/SoundItem" },
        {"img_QuestionDetail","Prefabs/img_QuestionDetail" },
        {"img_Accumulate","Prefabs/img_Accumulate" },
        {"btn_Adventure","Prefabs/btn_Adventure" },
        {"btn_Confirm","Prefabs/btn_Confirm" },
        {"ShoppingView","View/ShoppingView" },
        {"UpgradeView","View/UpgradeView" },
        {"img_CardShop","Prefabs/img_CardShop" },
        {"btn_ShopConfirm","Prefabs/btn_ShopConfirm" },
        {"img_EnergyBG","Prefabs/img_EnergyBG" },
        {"img_Energy","Prefabs/img_Energy" },
        {"img_Buff","Prefabs/img_Buff" },
        {"BGM_1","AudioClip/BGM_1" }
        //{ "MainView","View/MainView" },
        //{"SettingView","View/SettingView" },
        //{"CardPoolsView","View/CardPoolsView" },
        //{"AdventureView","View/AdventureView" },
        //{"AiDieView","View/AiDieView" },
        //{"AllSkillView","View/AllSkillView" },
        //{"PlayerDieView","View/PlayerDieView" },
        //{"MapView","View/MapView" },
        //{"GameView","View/GameView" },

        //{"AI_ATKimg_Prefab","Prefabs/AI_ATKimg_Prefab" },
        //{"CardPools_Obj","Prefabs/CardPools_Obj" },
        //{"img_AdventureBtn","Prefabs/img_AdventureBtn" },
        //{"img_Card200","Prefabs/img_Card200" },
        //{"img_Card240","Prefabs/img_Card240" },
        //{"img_CardDetail","Prefabs/img_CardDetail" },
        //{"img_QuestionCard","Prefabs/img_QuestionCard" },
        //{"TopBar","Prefabs/TopBar" },

        //{"Map_Atk_img","Prefabs/Map/Map_Atk_img" },
        //{"Map_Head_Box","Prefabs/Map/Map_Head_Box" },
        //{"Map_Row","Prefabs/Map/Map_Row" },
        //{"Map_Title_img","Prefabs/Map/Map_Title_img" },
        //{"Path_img_0","Prefabs/Map/Path_img_0" },
        //{"Path_img_0_y","Prefabs/Map/Path_img_0_y" },
        //{"Path_img_1","Prefabs/Map/Path_img_1" },
        //{"Path_img_1_x","Prefabs/Map/Path_img_1_x" },
        //{"Path_img_1_xy","Prefabs/Map/Path_img_1_xy" },
        //{"Path_img_1_y","Prefabs/Map/Path_img_1_y" },
        //{"Path_img_2","Prefabs/Map/Path_img_2" },
        //{"Path_img_2_y","Prefabs/Map/Path_img_2_y" },
        //{"Path_img_3","Prefabs/Map/Path_img_3" },
        //{"Path_img_3_y","Prefabs/Map/Path_img_3_y" },
        //{"Path_NullObj","Prefabs/Map/Path_NullObj" }
    };

    /// <summary>
    /// 预加载字典,弹窗类的，UI类的资源提前load放入此字典
    /// 一般大资源比如UI页面
    /// </summary>
    Dictionary<string, object> resourcesDic = new Dictionary<string, object>();


    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        #region 资源路径初始化
        //Common.SaveTxtFile(Common.DicToJson(urlDic), GlobalAttr.AllResourcesFileName, "Resources");
        Common.DicDataRead(ref urlDic, GlobalAttr.AllResourcesFileName, "Resources");
        #endregion

        //页面预加载
        foreach (var item in urlDic)
        {
            var aa = item.Value.Substring(0, 5);
            if (item.Value.Substring(0, 5) == "View/")
            {
                resourcesDic.Add(item.Key, null);
                SetToResourcesDic(item.Key);
            }
        }
    }

    /// <summary>
    /// 检查资源是否存在
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool CheckResourcesExist(string name)
    {
        return urlDic.ContainsKey(name);
    }

    /// <summary>
    /// 提前预加载
    /// </summary>
    /// <param name="dicName"></param>
    public void SetToResourcesDic(string dicName)
    {
        resourcesDic[dicName] = Resources.Load(urlDic[dicName]);
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="dicName"></param>
    /// <returns></returns>
    public object Load(string dicName)
    {
        if (resourcesDic.ContainsKey(dicName))
        {
            return resourcesDic[dicName];
        }
        else if (urlDic.ContainsKey(dicName))
        {
            return Resources.Load(urlDic[dicName]);
        }
        else
        {
            return null;
        }
    }
}
