using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManager : SingletonMonoBehaviour<AnimationManager>
{
    /// <summary>
    /// 动作/特效名，持续时间
    /// 动画1帧等于16.67毫秒
    /// </summary>
    Dictionary<string, float> animationDic = new Dictionary<string, float>()
    {
        { "Anim_Shuffle",1.09f},//洗牌65帧
        { "Anim_ShowTitle",1.09f},//显示标题75
        { "Anim_DealCard",0.37f},//发牌22
        { "Anim_RecycleCard",0.67f},//收牌40
        { "Anim_ATK",0.67f},//攻击40
        { "Anim_HPDeduction",0.67f},//血量扣减40
        { "Anim_HPRestore",0.67f},//血量恢复40
        { "Anim_Armor",0.67f},//添加护甲40
        { "Anim_EnergyRestore",0.67f},//能量恢复40
        { "Anim_PlayerDie",1.52f},//角色死亡90
    };

    /// <summary>
    /// 执行动作/特效
    /// </summary>
    /// <param name="name">动作/特效名</param>
    /// <param name="arg">参数集</param>
    /// <returns>执行时间</returns>
    public float DoAnimation(string name, object[] arg)
    {
        GameView gameView = UIManager.instance.GetView("GameView") as GameView;
        if (animationDic.ContainsKey(name))
        {
            switch (name)
            {
                case "Anim_Shuffle":
                    gameView.Anim_Shuffle.Play("Anim_Shuffle");
                    break;
                case "Anim_ShowTitle":
                    if (arg != null)
                    {
                        var title = gameView.Anim_ShowTitle.transform.Find("Title/Text").GetComponent<Text>();
                        title.text = arg[0].ToString();
                        gameView.Anim_ShowTitle.Play("Anim_ShowTitle");
                    }
                    else
                    {
                        gameView.Anim_ShowTitle.Play("Anim_ShowTitle");
                    }
                    break;
                case "Anim_DealCard":
                    gameView.Anim_DealCard.Play("Anim_DealCard");
                    break;
                case "Anim_RecycleCard":
                    gameView.Anim_RecycleCard.Play("Anim_RecycleCard");
                    break;
                case "Anim_ATK":
                    if (arg[0].ToString() == "0")//攻击AI
                    {
                        gameView.Anim_ATK.transform.position = gameView.img_Enemy.transform.position;
                    }
                    else//攻击角色
                    {
                        gameView.Anim_ATK.transform.position = gameView.img_Player.transform.position;
                    }
                    gameView.Anim_ATK.Play("Anim_ATK");
                    break;
                case "Anim_HPDeduction":
                    if (arg[0].ToString() == "0")//AI扣减
                    {
                        gameView.Anim_HPDeduction.transform.position = gameView.img_Enemy.transform.position;
                    }
                    else//角色扣减
                    {
                        gameView.Anim_HPDeduction.transform.position = gameView.img_Player.transform.position;
                    }
                    gameView.Anim_HPDeduction.Play("Anim_HPDeduction");
                    break;
                case "Anim_HPRestore":
                    if (arg[0].ToString() == "0")//AI恢复
                    {
                        gameView.Anim_HPRestore.transform.position = gameView.img_Enemy.transform.position;
                    }
                    else//角色恢复
                    {
                        gameView.Anim_HPRestore.transform.position = gameView.img_Player.transform.position;
                    }
                    gameView.Anim_HPRestore.Play("Anim_HPRestore");
                    break;
                case "Anim_Armor":
                    if (arg[0].ToString() == "0")//AI增甲
                    {
                        gameView.Anim_Armor.transform.position = gameView.img_Enemy.transform.position;
                    }
                    else//角色增甲
                    {
                        gameView.Anim_Armor.transform.position = gameView.img_Player.transform.position;
                    }
                    gameView.Anim_Armor.Play("Anim_Armor");
                    break;
                case "Anim_EnergyRestore":
                    if (arg[0].ToString() == "0")//AI能量恢复
                    {
                        gameView.Anim_EnergyRestore.transform.position = gameView.img_Enemy.transform.position;
                    }
                    else//角色能量恢复
                    {
                        gameView.Anim_EnergyRestore.transform.position = gameView.img_Player.transform.position;
                    }
                    gameView.Anim_EnergyRestore.Play("Anim_EnergyRestore");
                    break;
                case "Anim_PlayerDie":
                    gameView.Anim_PlayerDie.Play("Anim_PlayerDie");
                    break;
            }
            return animationDic[name];
        }
        return 0;
    }
}
