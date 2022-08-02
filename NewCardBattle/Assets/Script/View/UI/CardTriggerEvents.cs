using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardTriggerEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private GameObject CardObj;
    private Vector3 mouseInitPos;
    public void OnPointerDown(PointerEventData eventData)
    {
        mouseInitPos = Input.mousePosition;
        //Debug.Log("Card位置：" + cardPos + "；鼠标位置：" + mousePos);
        //判断当前鼠标在那张卡上。卡Pos在中心位置，宽170，高200
        for (int i = 0; i < transform.childCount; i++)
        {
            Vector3 cardPos = transform.GetChild(i).gameObject.transform.position;
            if (i == transform.childCount - 1)
            {
                if (cardPos.x + 85 > mouseInitPos.x && cardPos.x - 85 < mouseInitPos.x &&
                cardPos.y + 100 > mouseInitPos.y && cardPos.y - 100 < mouseInitPos.y)
                {
                    CardObj = transform.GetChild(i).gameObject;
                }
            }
            else
            {
                if (cardPos.x + 85 > mouseInitPos.x && cardPos.x - 15 < mouseInitPos.x &&
                cardPos.y + 100 > mouseInitPos.y && cardPos.y - 100 < mouseInitPos.y)
                {
                    CardObj = transform.GetChild(i).gameObject;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (CardObj != null)
        {
            GameView view = UIManager.instance.GetView("GameView") as GameView;
            var cMousePos = Input.mousePosition;
            Debug.Log(CardObj.name);
            if (mouseInitPos.x + 10 > cMousePos.x && mouseInitPos.x - 10 < cMousePos.x &&
               mouseInitPos.y + 10 > cMousePos.y && mouseInitPos.y - 10 < cMousePos.y)//如果移动范围不大现实卡牌详情
            {
                view.CardClick(CardObj);
            }
            else
            {
                view.HideCardDetail();
                view.HideMagnifyCard();
                CardObj.transform.DOMove(cMousePos, 0.5f);
            }
        }
    }
}
