using Assets.Script.Models;
using Assets.Script.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManager : SingletonMonoBehaviour<AnimationManager>
{
    /// <summary>
    /// 动作/特效名，持续时间
    /// 动画1帧等于16.67毫秒
    /// 每个动画多留5毫秒（不然连续使用的时候可能第二个动画放不出来）
    /// </summary>
    Dictionary<string, float> animationDic = new Dictionary<string, float>()
    {
        { "Anim_Shuffle",1.14f},//洗牌65帧
        { "Anim_ShowTitle",1.14f},//显示标题75
        { "Anim_DealCard",0.42f},//发牌22
        { "Anim_RecycleCard",0.55f},//收牌30
        { "Anim_ATK",0.72f},//攻击40
        { "Anim_HPDeduction",0.72f},//血量扣减40
        { "Anim_HPRestore",0.72f},//血量恢复40
        { "Anim_Armor",0.72f},//添加护甲40
        { "Anim_EnergyRestore",0.72f},//能量恢复40
        { "Anim_PlayerDie",1.55f},//角色死亡90
        { "Anim_RemoveCard",0.89f},//移除卡牌50
        { "Anim_DrawACard",1.10f},//抽一张卡60
        { "Anim_ArmorMelting",0.90f},//摧毁防御50
        { "Anim_Elude",0.65f},//躲避攻击35
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
        Text txt_Effect;
        if (animationDic.ContainsKey(name))
        {
            switch (name)
            {
                #region 洗牌
                case "Anim_Shuffle":
                    gameView.Anim_Shuffle.Play("Anim_Shuffle");
                    break;
                #endregion
                #region 显示标题
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
                #endregion
                #region 发牌
                case "Anim_DealCard":
                    gameView.Anim_DealCard.Play("Anim_DealCard");
                    break;
                #endregion
                #region 收牌
                case "Anim_RecycleCard":
                    gameView.Anim_RecycleCard.Play("Anim_RecycleCard");
                    break;
                #endregion
                #region 攻击
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
                #endregion
                #region HP扣减
                case "Anim_HPDeduction":
                    if (arg[0].ToString() == "0")//AI扣减
                    {
                        gameView.Anim_HPDeduction.transform.position = gameView.img_Enemy.transform.position;
                    }
                    else//角色扣减
                    {
                        gameView.Anim_HPDeduction.transform.position = gameView.img_Player.transform.position;
                    }
                    txt_Effect = gameView.Anim_HPDeduction.transform.Find("Text").GetComponent<Text>();
                    txt_Effect.text = arg?[1].ToString();
                    gameView.Anim_HPDeduction.Play("Anim_HPDeduction");
                    break;
                #endregion
                #region HP恢复
                case "Anim_HPRestore":
                    if (arg[0].ToString() == "0")//AI恢复
                    {
                        gameView.Anim_HPRestore.transform.position = gameView.img_Enemy.transform.position;
                    }
                    else//角色恢复
                    {
                        gameView.Anim_HPRestore.transform.position = gameView.img_Player.transform.position;
                    }
                    txt_Effect = gameView.Anim_HPRestore.transform.Find("Text").GetComponent<Text>();
                    txt_Effect.text = arg?[1].ToString();
                    gameView.Anim_HPRestore.Play("Anim_HPRestore");
                    break;
                #endregion
                #region 防御
                case "Anim_Armor":
                    if (arg[0].ToString() == "0")//AI增甲
                    {
                        gameView.Anim_Armor.transform.position = gameView.img_Enemy.transform.position;
                    }
                    else//角色增甲
                    {
                        gameView.Anim_Armor.transform.position = gameView.img_Player.transform.position;
                    }
                    txt_Effect = gameView.Anim_Armor.transform.Find("Text").GetComponent<Text>();
                    txt_Effect.text = arg?[1].ToString();
                    gameView.Anim_Armor.Play("Anim_Armor");
                    break;
                #endregion
                #region 能量恢复
                case "Anim_EnergyRestore":
                    if (arg == null)
                    {
                        gameView.Anim_EnergyRestore.transform.position = gameView.img_Player.transform.position;
                    }
                    else
                    {
                        if (arg[0].ToString() == "1")
                        {
                            gameView.Anim_EnergyRestore.transform.position = gameView.img_Player.transform.position;
                        }
                    }
                    gameView.Anim_EnergyRestore.Play("Anim_EnergyRestore");
                    break;
                #endregion
                #region 角色死亡
                case "Anim_PlayerDie":
                    gameView.Anim_PlayerDie.Play("Anim_PlayerDie");
                    break;
                #endregion
                #region 移除卡
                case "Anim_RemoveCard":
                    gameView.Anim_RemoveCard.Play("Anim_RemoveCard");
                    break;
                #endregion
                #region 抽一张卡
                case "Anim_DrawACard":
                    if (arg != null)
                    {
                        CurrentCardPoolModel model = arg[0] as CurrentCardPoolModel;
                        var tmepObj = gameView.Anim_DrawACard.transform.Find("img_Card240").gameObject;
                        Common.CardDataBind(tmepObj, model);
                    }
                    gameView.Anim_DrawACard.Play("Anim_DrawACard");
                    break;
                #endregion
                #region 摧毁防御
                case "Anim_ArmorMelting":
                    if (arg[0].ToString() == "0")//攻击AI
                    {
                        gameView.Anim_ArmorMelting.transform.position = gameView.img_Enemy.transform.position;
                    }
                    else//攻击角色
                    {
                        gameView.Anim_ArmorMelting.transform.position = gameView.img_Player.transform.position;
                    }
                    gameView.Anim_ArmorMelting.Play("Anim_ArmorMelting");
                    break;
                #endregion
                #region 闪避
                case "Anim_Elude":
                    if (arg[0].ToString() == "0")//攻击AI
                    {
                        gameView.Anim_Elude.transform.position = gameView.img_Enemy.transform.position;
                    }
                    else//攻击角色
                    {
                        gameView.Anim_Elude.transform.position = gameView.img_Player.transform.position;
                    }
                    gameView.Anim_Elude.Play("Anim_Elude");
                    break;
                    #endregion
            }
            return animationDic[name];
        }
        return 0;
    }
}
