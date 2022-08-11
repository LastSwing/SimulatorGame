using Assets.Script.Models;
using Assets.Script.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 卡牌触发事件
/// </summary>
public class CardItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    GameObject thisObj;
    public CurrentCardPoolModel BasisData;//卡牌数据
    GameView gameView;
    Vector3 InitPos;//初始位置
    int UnUseCardScopeNum = 50;
    private void Start()
    {
        gameView = UIManager.instance.GetView("GameView") as GameView;
    }

    /// <summary>
    /// 鼠标进入时触发
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.Control)
        {
            gameView.HideCardDetail();
            gameView.HideMagnifyCard();
            thisObj = transform.parent.Find(name).gameObject;
            thisObj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            Common.AddChild(gameView.MagnifyObj.transform, thisObj);
            gameView.MagnifyObj.transform.GetChild(0).transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        }
    }

    /// <summary>
    /// 鼠标离开时触发
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.Control)
        {
            gameView.HideCardDetail();
            gameView.HideMagnifyCard();
            thisObj = transform.parent.Find(name).gameObject;
            thisObj.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.Control)
        {
            gameView.HideCardDetail();
            gameView.HideMagnifyCard();
            thisObj = transform.parent.Find(name).gameObject;
            InitPos = thisObj.transform.position;
        }
    }
    /// <summary>
    /// 拖拽中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (BattleManager.instance.BattleStateMachine.CurrentState.ID == BattleStateID.Control)
        {
            gameView.HideCardDetail();
            gameView.HideMagnifyCard();
            thisObj = transform.parent.Find(name).gameObject;
            thisObj.transform.position = Input.mousePosition;
        }
    }
    /// <summary>
    /// 拖拽结束
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        thisObj = transform.parent.Find(name).gameObject;
        //小范围拖动不使用卡牌
        var currentPos = thisObj.transform.position;
        if (InitPos.x + UnUseCardScopeNum > currentPos.x && InitPos.x - UnUseCardScopeNum < currentPos.x &&
            InitPos.y + UnUseCardScopeNum > currentPos.y && InitPos.y - UnUseCardScopeNum < currentPos.x)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameView.thisParent);
        }
        else
        {
            gameView.HideCardDetail();
            gameView.HideMagnifyCard();
            CardUseEffectManager.instance.CurrentCardModel = BasisData;
            CardUseEffectManager.instance.CardToRoleID = CardInRolePosition(eventData.position);
            CardUseEffectManager.instance.CurrentCard = thisObj;
            BattleManager.instance.BattleStateMachine.ChangeState(BattleStateID.EffectSettlement);
        }
    }

    /// <summary>
    /// 卡牌所在哪个角色的范围里
    /// </summary>
    /// <param name="CardPos">卡牌所在位置</param>
    /// <returns>角色ID</returns>
    public int CardInRolePosition(Vector3 CardPos)
    {
        int result = 0;
        //var playerPos = BattleManager.instance.OwnPlayerData
        foreach (var item in BattleManager.instance.OwnPlayerData)
        {
            if (CardPos.x < item.playerPos.x + 200 && CardPos.x > item.playerPos.x - 200 && CardPos.y < item.playerPos.y + 180 && CardPos.y > item.playerPos.y - 180)
            {
                result = item.playerID;
            }
        }
        foreach (var item in BattleManager.instance.EnemyPlayerData)
        {
            if (CardPos.x < item.playerPos.x + 200 && CardPos.x > item.playerPos.x - 200 && CardPos.y < item.playerPos.y + 180 && CardPos.y > item.playerPos.y - 180)
            {
                result = item.playerID;
            }
        }
        return result;
    }

}
