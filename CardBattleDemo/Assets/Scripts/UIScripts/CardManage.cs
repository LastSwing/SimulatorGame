using Assets.Scripts.FSM;
using Assets.Scripts.LogicalScripts.Models;
using Assets.Scripts.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIScripts
{
    /// <summary>
    /// 卡牌操作
    /// </summary>
    public class CardManage : MonoBehaviour
    {
        StateMachine<CardManage> myStateMachine; //状态机
        GameObject DefensePanel, ATKPanel;
        private Image Card_img;
        private Text txt_EndCardCount;
        public int clickCount = 0;
        public int currrentIndex = 0;//当前图片下标
        private bool hasDrag, HasInitList = false;//是否拖拽
        public bool RecycleCardAnimationState = false;
        /// <summary>
        /// 攻击栏牌池
        /// </summary>
        public List<CurrentCardPoolModel> ATKList;
        /// <summary>
        /// 已使用的卡池
        /// </summary>
        private List<CurrentCardPoolModel> UsedCardList = new List<CurrentCardPoolModel>();

        void Start()
        {

            txt_EndCardCount = GameObject.Find("CardPool/right_Card/txt_EndCardCount").GetComponent<Text>();

            DefensePanel = GameObject.Find("DefensePanel");
            ATKPanel = GameObject.Find("ATKPanel");
            DefensePanel.SetActive(false);
            ATKPanel.SetActive(false);
            #region 状态机初始化

            myStateMachine = new StateMachine<CardManage>(this);
            myStateMachine.SetCurrentState(CardManageState.Instance);
            myStateMachine.SetGlobalState(CardManage_GlobalState.Instance);
            #endregion
        }

        void Update()
        {
            myStateMachine.FSMUpdate();
            if (HasInitList && ATKList == null)
            {
                ATKList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.ATKBarCardPoolsFileName);
                HasInitList = false;
            }
        }

        /// <summary>
        /// 显示卡牌详情
        /// </summary>
        /// <param name="index"></param>
        public void ShowCardSkillDetail(int index)
        {
            if (!hasDrag)
            {
                if (clickCount == 0 || index != currrentIndex)
                {
                    HideCardSkill();
                    Card_img = GameObject.Find($"Card/img_Card{index + 1}/img_Detail")?.GetComponent<Image>();
                    if (Card_img != null)
                    {
                        Card_img.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        var AtkBarList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.ATKBarCardPoolsFileName);
                        Card_img = GameObject.Find($"Card/img_Card{(index + 1)}").GetComponent<Image>();
                        GameObject tempImg = Resources.Load("Prefab/img_Detail") as GameObject;
                        tempImg = Common.AddChild(Card_img.transform, tempImg);
                        tempImg.name = "img_Detail";
                        tempImg.transform.localPosition = new Vector2(0, 160);


                        GameObject temp = tempImg.transform.Find("Text").gameObject;
                        temp.GetComponent<Text>().text = $"{AtkBarList[index].CardName}\n{AtkBarList[index].CardDetail}";
                    }
                    currrentIndex = index;
                    clickCount++;
                }
                else
                {
                    clickCount = 0;
                    HideCardSkill();
                }
            }

        }

        /// <summary>
        /// 隐藏卡牌详情
        /// </summary>
        public void HideCardSkill()
        {
            for (int i = 1; i < 6; i++)
            {
                Card_img = GameObject.Find($"Card/img_Card{i}/img_Detail")?.GetComponent<Image>();
                if (Card_img != null)
                {
                    Card_img.transform.localScale = Vector3.zero;
                }
            }
        }
        /// <summary>
        /// 拖拽卡牌
        /// </summary>
        public void Drag(int index)
        {
            hasDrag = true;
            HasInitList = true;
            Card_img = GameObject.Find($"Card/img_Card{index + 1}")?.GetComponent<Image>();
            var atkModel = ATKList?[index];
            if (atkModel != null)
            {
                var stateType = atkModel.StateType;
                if (stateType == 0 || stateType == 10 || stateType == 11)//攻击
                {
                    //显示防御遮罩
                    DefensePanel.SetActive(true);
                }
                else
                {
                    //显示攻击遮罩
                    ATKPanel.SetActive(true);
                }
            }
            Card_img.transform.position = Input.mousePosition;
        }
        /// <summary>
        /// 松开拖拽
        /// </summary>
        public void DragUp(int index)
        {
            DefensePanel.SetActive(false);
            ATKPanel.SetActive(false);
            Card_img = GameObject.Find($"Card/img_Card{index + 1}")?.GetComponent<Image>();
            var cardPosition = Card_img.transform.position;
            var Scope = new Vector3(640, 270);
            var InitVector = new Vector2(-298 + index * 149, 0);
            var atkModel = ATKList?[index];
            if (atkModel != null)
            {
                var stateType = atkModel.StateType;
                if (stateType == 0 || stateType == 10 || stateType == 11)//攻击
                {
                    //攻击卡只能拖拽到敌人范围
                    if (cardPosition.x > Scope.x && cardPosition.y > Scope.y)
                    {
                        Debug.Log("进入攻击范围");
                        Card_img.transform.localScale = Vector3.zero;
                        UsedCardList.Add(atkModel);
                        ATKList.Remove(atkModel);
                        //执行动画
                        RecycleCardAnimationState = true;
                    }
                    else
                    {
                        Card_img.transform.localPosition = InitVector;
                    }
                    //Debug.Log(cardPosition);
                }
                else
                {
                    //功能卡只能在玩家范围内使用
                    if (cardPosition.x < Scope.x && cardPosition.y > Scope.y)
                    {
                        Debug.Log("进入防御范围");
                        Card_img.transform.localScale = Vector3.zero;
                        UsedCardList.Add(atkModel);
                        ATKList.Remove(atkModel);
                        //执行动画
                        RecycleCardAnimationState = true;
                    }
                    else
                    {
                        Card_img.transform.localPosition = InitVector;
                    }
                }
            }
            else
            {
                Card_img.transform.localPosition = InitVector;
            }
        }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        public void DragExit()
        {
            if (hasDrag)
            {
                HideCardSkill();
                hasDrag = false;
                clickCount = 0;
            }
        }

        /// <summary>
        /// 卡牌回收动画
        /// </summary>
        public void RecycleCardAnimation()
        {
            Image rightImg = GameObject.Find($"CardPool/right_Card/right_Card9").GetComponent<Image>();
            rightImg.transform.localScale = Vector3.one;
            var x = rightImg.transform.position.x;
            Debug.Log(rightImg.transform.position);
            rightImg.transform.position = new Vector3(x + 1, rightImg.transform.position.y - 1);
            rightImg.transform.Rotate(Vector3.up, -1);
            if (x >= 1110)
            {
                txt_EndCardCount.text = UsedCardList.Count.ToString();
                Common.SaveTxtFile(UsedCardList.ListToJson(), GlobalAttr.UsedCardPoolsFileName);
                RecycleCardAnimationState = false;
                rightImg.transform.localScale = Vector3.zero;
                rightImg.transform.localPosition = new Vector3(-90f, 130f);
            }
        }
    }
}
