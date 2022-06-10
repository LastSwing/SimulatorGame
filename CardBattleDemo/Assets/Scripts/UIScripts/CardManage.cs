using Assets.Scripts.FSM;
using Assets.Scripts.LogicalScripts.Models;
using Assets.Scripts.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
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
        GameObject DefensePanel, ATKPanel, Shuffle_Obj, Restore_Obj, Atk_Obj, Armor_Obj, Energy_Obj, RecycleCard_Obj, AiRoundTitle_Obj, Anim_GameOver_obj;
        Animation Anim_Restore, Anim_ATK, Anim_Armor, Anim_EnergyRestore, Anim_DealCards, Anim_RecycleCard, Anim_Shuffle, Anim_AiRoundTitle, Anim_GameOver;
        Button btn_RoundOver;
        Image Card_img, Eimg_HP, Player_img_Armor, Pimg_HP, Enemy_img_Armor;
        Text txt_EndCardCount, txt_StartCardCount, Player_txt_Armor, Player_HP, Enemy_txt_Armor;
        public int clickCount, currrentIndex;//当前图片下标
        private int DealCardCount, CardCount, ShuffleCount, AiAtkCount;
        private float totalTimer;// 定义每帧累加时间
        private bool hasDrag, CardAssignmentState, AiTitleAnimState;//是否拖拽
        public bool RecycleCardAnimationState, RecycleCardAnimationEndState,//回收卡牌动画
            CardListAnimationState, CardListAnimationEndState, //卡牌发牌动画
            RoundOverState, CardPoolsDataSave, HasUseCard, CardRecycleSuccess,//回合结束状态、卡池数据保存、是否使用了卡牌、卡牌回收成功
            RotationCardAnimationState, RotationCardAnimationEndState,//卡牌旋转动画
            ShuffleAnimationState, FoldCardAnimationState, ShuffleAnimationSuccessState,   //洗牌状态、叠牌状态、动画完成
            AiAtkState, GameOverStartState, AiAnimStartState//Ai攻击状态,游戏结束动画开始,Ai动画开始
            = false;

        private bool DealCardsAnimState = true;
        private int WhenShuffleCardCount = 0;//当洗牌时，所洗牌的卡数量

        private CurrentRoleModel PlayerRole;    //玩家角色
        private CurrentRoleModel AiRole;        //Ai角色
        /// <summary>
        /// 未使用的卡池
        /// </summary>
        private List<CurrentCardPoolModel> UnusedCardList = new List<CurrentCardPoolModel>();
        /// <summary>
        /// 攻击栏牌池
        /// </summary>
        public List<CurrentCardPoolModel> ATKList;
        /// <summary>
        /// 已使用的卡池
        /// </summary>
        private List<CurrentCardPoolModel> UsedCardList = new List<CurrentCardPoolModel>();
        /// <summary>
        /// 回合结束时未使用的卡牌下标集合
        /// </summary>
        private List<int> RoundOverUnUseCardIndexList = new List<int>() { 1, 2, 3, 4, 5 };

        /// <summary>
        /// 当前Ai总牌池
        /// </summary>
        private List<CurrentCardPoolModel> AiCardList;
        /// <summary>
        /// ai攻击牌池
        /// </summary>
        private List<CurrentCardPoolModel> AiAtkCardList;

        void Start()
        {
            #region 控件初始化


            Pimg_HP = GameObject.Find("Player/Pimg_HP").GetComponent<Image>();
            Eimg_HP = GameObject.Find("Enemy/Eimg_HP").GetComponent<Image>();
            Player_img_Armor = GameObject.Find("Player/img_Armor").GetComponent<Image>();
            Enemy_img_Armor = GameObject.Find("Enemy/img_Armor").GetComponent<Image>();

            btn_RoundOver = GameObject.Find("btn_RoundOver").GetComponent<Button>();
            btn_RoundOver.onClick.AddListener(ClickRoundOver);


            Enemy_txt_Armor = GameObject.Find("Enemy/img_Armor/Text").GetComponent<Text>();
            Player_txt_Armor = GameObject.Find("Player/img_Armor/Text").GetComponent<Text>();
            txt_EndCardCount = GameObject.Find("CardPool/right_Card/txt_EndCardCount").GetComponent<Text>();
            txt_StartCardCount = GameObject.Find("CardPool/left_Card/txt_StartCardCount").GetComponent<Text>();
            Player_HP = GameObject.Find("Player/Text").GetComponent<Text>();

            DefensePanel = GameObject.Find("DefensePanel");
            ATKPanel = GameObject.Find("ATKPanel");
            DefensePanel.SetActive(false);
            ATKPanel.SetActive(false);

            #endregion

            #region 动画控件
            var gameCanvas = GameObject.Find("GameCanvas");
            Restore_Obj = Common.AddChild(gameCanvas.transform, (GameObject)Resources.Load("Prefab/Anim_Restore"));
            Restore_Obj.name = "Anim_Restore";
            Anim_Restore = GameObject.Find("GameCanvas/Anim_Restore").GetComponent<Animation>();
            Restore_Obj.SetActive(false);

            Atk_Obj = Common.AddChild(gameCanvas.transform, (GameObject)Resources.Load("Prefab/Anim_ATK"));
            Atk_Obj.name = "Anim_ATK";
            Anim_ATK = GameObject.Find("GameCanvas/Anim_ATK").GetComponent<Animation>();
            Atk_Obj.SetActive(false);

            Armor_Obj = Common.AddChild(gameCanvas.transform, (GameObject)Resources.Load("Prefab/Anim_Armor"));
            Armor_Obj.name = "Anim_Armor";
            Anim_Armor = GameObject.Find("GameCanvas/Anim_Armor").GetComponent<Animation>();
            Armor_Obj.SetActive(false);

            Energy_Obj = Common.AddChild(gameCanvas.transform, (GameObject)Resources.Load("Prefab/Anim_EnergyRestore"));
            Energy_Obj.name = "Anim_EnergyRestore";
            Anim_EnergyRestore = GameObject.Find("GameCanvas/Anim_EnergyRestore").GetComponent<Animation>();
            Energy_Obj.SetActive(false);

            //发牌
            Anim_DealCards = GameObject.Find("GameCanvas/CardPool").GetComponent<Animation>();

            RecycleCard_Obj = Common.AddChild(gameCanvas.transform, (GameObject)Resources.Load("Prefab/Anim_RecycleCard"));
            RecycleCard_Obj.name = "Anim_RecycleCard";
            Anim_RecycleCard = GameObject.Find("GameCanvas/Anim_RecycleCard").GetComponent<Animation>();
            RecycleCard_Obj.transform.SetSiblingIndex(3);
            RecycleCard_Obj.SetActive(false);

            Shuffle_Obj = Common.AddChild(gameCanvas.transform, (GameObject)Resources.Load("Prefab/Anim_Shuffle"));
            Shuffle_Obj.name = "Shuffle";
            Anim_Shuffle = GameObject.Find("GameCanvas/Shuffle").GetComponent<Animation>();
            Shuffle_Obj.SetActive(false);

            AiRoundTitle_Obj = Common.AddChild(gameCanvas.transform, (GameObject)Resources.Load("Prefab/Anim_AiRoundTitle"));
            AiRoundTitle_Obj.name = "AiRoundTitle";
            Anim_AiRoundTitle = GameObject.Find("GameCanvas/AiRoundTitle").GetComponent<Animation>();
            AiRoundTitle_Obj.SetActive(false);

            Anim_GameOver_obj = Common.AddChild(gameCanvas.transform, (GameObject)Resources.Load("Prefab/Anim_GameOver"));
            Anim_GameOver_obj.name = "GameOver";
            Anim_GameOver = GameObject.Find("GameCanvas/GameOver").GetComponent<Animation>();
            Anim_GameOver_obj.SetActive(false);
            #endregion

            #region 状态机初始化

            myStateMachine = new StateMachine<CardManage>(this);
            myStateMachine.SetCurrentState(CardManageState.Instance);
            myStateMachine.SetGlobalState(CardManage_GlobalState.Instance);
            #endregion

        }

        void Update()
        {
            myStateMachine.FSMUpdate();
            #region 攻击栏牌池初始化
            if (ATKList == null)
            {
                ATKList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentATKBarCardPoolsFileName);
            }
            #endregion
            #region 角色数据初始化
            if (PlayerRole == null)
            {
                PlayerRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName);
            }
            if (AiRole == null)
            {
                AiRole = Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentAIRoleFileName);
            }
            #endregion
            #region 发牌动画减少牌池总数
            if (DealCardCount != 0)
            {
                if (Anim_DealCards["DealCards"].normalizedTime != 0)
                {
                    totalTimer += Time.deltaTime;
                    if (totalTimer >= 0.37f)
                    {
                        if (DealCardCount < 6 && DealCardsAnimState)
                        {
                            int StartCardCount = Convert.ToInt32(txt_StartCardCount.text);
                            txt_StartCardCount.text = (StartCardCount - 1).ToString();
                            totalTimer = 0;
                            DealCardCount++;
                        }
                        else
                        {
                            totalTimer = 0;
                        }
                    }
                }
            }
            #endregion

            if (GameOverStartState)
            {
                if (!Anim_GameOver.isPlaying)
                {
                    Common.SceneJump("PlayerDieScene");
                    GameOverStartState = false;
                }
            }
        }

        #region 卡牌操作
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
                        Card_img = GameObject.Find($"Card/img_Card{(index + 1)}").GetComponent<Image>();
                        GameObject tempImg = Resources.Load("Prefab/img_Detail") as GameObject;
                        tempImg = Common.AddChild(Card_img.transform, tempImg);
                        tempImg.name = "img_Detail";
                        tempImg.transform.localPosition = new Vector2(0, 160);


                        GameObject temp = tempImg.transform.Find("Text").gameObject;
                        temp.GetComponent<Text>().text = $"{ATKList[index].CardName}\n{ATKList[index].CardDetail}";
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
                //攻击卡只能拖拽到敌人范围
                if (cardPosition.x > Scope.x && cardPosition.y > Scope.y)
                {
                    //Restore_anim.transform.localPosition = new Vector3(373, 0);//血量恢复
                    //Restore_anim.Play("Restore");
                    #region 普通攻击0
                    if (stateType == 0)
                    {
                        if (PlayerRole.Energy >= atkModel.Consume)
                        {
                            atkModel.Proficiency++;
                            if (atkModel.Consume > 0)
                            {
                                Common.EnergyImgChange(PlayerRole.Energy, atkModel.Consume, 0, PlayerRole.MaxEnergy);
                                PlayerRole.Energy -= atkModel.Consume;
                            }
                            float DeductionHp = atkModel.Effect;
                            if (AiRole.Armor > 0)
                            {
                                if (AiRole.Armor >= atkModel.Effect)
                                {
                                    DeductionHp = 0;
                                    AiRole.Armor -= Convert.ToInt32(atkModel.Effect);
                                    Enemy_txt_Armor.text = PlayerRole.Armor.ToString();
                                }
                                else
                                {
                                    DeductionHp -= AiRole.Armor;
                                    AiRole.Armor = 0;
                                }
                                if (AiRole.Armor == 0)
                                {
                                    Enemy_img_Armor.transform.localScale = Vector3.zero;
                                }
                            }
                            Common.HPImageChange(Eimg_HP, AiRole.MaxHP, DeductionHp, 0);
                            AiRole.HP -= DeductionHp;
                            if (AiRole.HP <= 0)
                            {
                                AiRole.HP = 0;
                                AiDieGameOver();
                            }
                            var HpTxt = GameObject.Find("Enemy/Text").GetComponent<Text>();
                            HpTxt.text = $"{AiRole.MaxHP}/{AiRole.HP}";
                            Atk_Obj.SetActive(true);
                            Anim_ATK.transform.localPosition = new Vector3(373, 0);//攻击特效
                            Anim_ATK.Play("ATK");
                            #region 执行动画
                            Card_img.transform.localScale = Vector3.zero;
                            Card_img.transform.localPosition = new Vector3(-298 + 149 * index, 0, 0);
                            UsedCardList.Add(atkModel);
                            CardPoolsDataSave = true;
                            RoundOverUnUseCardIndexList.Remove(index + 1);
                            RecycleCard_Obj.SetActive(true);
                            Anim_RecycleCard.Play("RecycleCard");
                            RecycleCardAnimationState = true;
                            HasUseCard = true;

                            #endregion
                        }
                        else
                        {
                            Card_img.transform.localPosition = InitVector;
                        }
                    }
                    #endregion
                    #region 使用后移除
                    if (atkModel.TriggerState == 6)
                    {
                        UsedCardList.Remove(atkModel);
                    }
                    #endregion
                    else
                    {
                        Card_img.transform.localPosition = InitVector;
                    }
                }
                //功能卡只能在玩家范围内使用
                else if (cardPosition.x < Scope.x && cardPosition.y > Scope.y)
                {

                    #region 能量恢复5
                    if (stateType == 5)
                    {
                        atkModel.Proficiency++;
                        Energy_Obj.SetActive(true);
                        Anim_EnergyRestore.Play("EnergyRestore");
                        Common.EnergyImgChange(PlayerRole.Energy, Convert.ToInt32(atkModel.Effect), 1, PlayerRole.MaxEnergy);
                        PlayerRole.Energy += Convert.ToInt32(atkModel.Effect);
                        if (PlayerRole.Energy > PlayerRole.MaxEnergy)
                        {
                            PlayerRole.Energy = PlayerRole.MaxEnergy;
                        }

                        #region 执行动画
                        Card_img.transform.localScale = Vector3.zero;
                        Card_img.transform.localPosition = new Vector3(-298 + 149 * index, 0, 0);//位置初始化
                        UsedCardList.Add(atkModel);
                        CardPoolsDataSave = true;
                        RoundOverUnUseCardIndexList.Remove(index + 1);
                        //执行动画
                        RecycleCard_Obj.SetActive(true);
                        Anim_RecycleCard.Play("RecycleCard");
                        RecycleCardAnimationState = true;
                        HasUseCard = true;

                        #endregion
                    }
                    #endregion
                    #region 护盾使用1
                    else if (stateType == 1)
                    {
                        if (PlayerRole.Energy >= atkModel.Consume)
                        {
                            atkModel.Proficiency++;
                            if (atkModel.Consume > 0)
                            {
                                Common.EnergyImgChange(PlayerRole.Energy, atkModel.Consume, 0, PlayerRole.MaxEnergy);
                                PlayerRole.Energy -= atkModel.Consume;
                            }
                            Armor_Obj.SetActive(true);
                            Anim_Armor.transform.localPosition = new Vector3(-408, 0);//攻击特效
                            Anim_Armor.Play("Armor");
                            Player_img_Armor.transform.localScale = Vector3.one;
                            PlayerRole.Armor += Convert.ToInt32(atkModel.Effect);
                            if (PlayerRole.Armor > PlayerRole.MaxHP)
                            {
                                PlayerRole.Armor = Convert.ToInt32(PlayerRole.MaxHP);
                            }
                            Player_txt_Armor.text = PlayerRole.Armor.ToString();

                            #region 执行动画
                            Card_img.transform.localScale = Vector3.zero;
                            Card_img.transform.localPosition = new Vector3(-298 + 149 * index, 0, 0);//位置初始化
                            UsedCardList.Add(atkModel);
                            CardPoolsDataSave = true;
                            RoundOverUnUseCardIndexList.Remove(index + 1);
                            //执行动画
                            RecycleCard_Obj.SetActive(true);
                            Anim_RecycleCard.Play("RecycleCard");
                            RecycleCardAnimationState = true;
                            HasUseCard = true;

                            #endregion
                        }
                        else
                        {
                            Card_img.transform.localPosition = InitVector;

                        }
                    }
                    #endregion
                    #region 血量恢复3
                    else if (stateType == 3)
                    {
                        if (PlayerRole.Energy >= atkModel.Consume)
                        {
                            atkModel.Proficiency++;
                            if (atkModel.Consume > 0)
                            {
                                Common.EnergyImgChange(PlayerRole.Energy, atkModel.Consume, 0, PlayerRole.MaxEnergy);
                                PlayerRole.Energy -= atkModel.Consume;
                            }
                            PlayerRole.HP += atkModel.Effect;
                            if (PlayerRole.HP > PlayerRole.MaxHP)
                            {
                                PlayerRole.HP = PlayerRole.MaxHP;
                            }
                            Player_HP.text = $"{PlayerRole.MaxHP}/{PlayerRole.HP}";
                            if (PlayerRole.HP != PlayerRole.MaxHP)
                            {
                                Common.HPImageChange(Pimg_HP, AiRole.MaxHP, atkModel.Effect, 1);
                            }
                            Restore_Obj.SetActive(true);
                            Anim_Restore.transform.localPosition = new Vector3(-408, 0);
                            Anim_Restore.Play("Restore");
                            #region 执行动画
                            Card_img.transform.localScale = Vector3.zero;
                            Card_img.transform.localPosition = new Vector3(-298 + 149 * index, 0, 0);//位置初始化
                            UsedCardList.Add(atkModel);
                            CardPoolsDataSave = true;
                            RoundOverUnUseCardIndexList.Remove(index + 1);
                            //执行动画
                            RecycleCard_Obj.SetActive(true);
                            Anim_RecycleCard.Play("RecycleCard");

                            RecycleCardAnimationState = true;
                            HasUseCard = true;

                            #endregion
                        }
                        else
                        {
                            Card_img.transform.localPosition = InitVector;

                        }
                    }
                    #endregion
                    else
                    {
                        Card_img.transform.localPosition = InitVector;
                    }
                }
                else
                {
                    Card_img.transform.localPosition = InitVector;
                }
            }
            else
            {
                Card_img.transform.localPosition = InitVector;
            }
            Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
            Common.SaveTxtFile(AiRole.ObjectToJson(), GlobalAttr.CurrentAIRoleFileName);
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
        #endregion

        #region 卡牌动画
        /// <summary>
        /// 卡牌回收动画
        /// </summary>
        public void RecycleCardAnimation()
        {
            if (CardPoolsDataSave)
            {
                if (!Anim_RecycleCard.isPlaying)//执行完成
                {
                    txt_EndCardCount.text = UsedCardList.Count.ToString();
                    Common.SaveTxtFile(UsedCardList.ListToJson(), GlobalAttr.CurrentUsedCardPoolsFileName);
                    RecycleCardAnimationState = false;
                    RecycleCardAnimationEndState = true;
                    CardPoolsDataSave = false;
                }
            }
        }

        /// <summary>
        /// 点击回合结束按钮
        /// </summary>
        private void ClickRoundOver()
        {
            RoundOverState = true;
        }

        /// <summary>
        /// 回合结束
        /// </summary>
        public void RoundOverAnimation()
        {
            #region 卡牌回收动画
            if (HasUseCard)//如果使用过卡牌初始化状态
            {
                HasUseCard = false;
                RecycleCardAnimationEndState = false;
            }
            else
            {
                RecycleCard_Obj.SetActive(true);
            }
            if (RoundOverUnUseCardIndexList != null && RoundOverUnUseCardIndexList.Count > 0 && !Anim_RecycleCard.isPlaying)
            {
                if (RecycleCardAnimationEndState)
                {
                    RoundOverUnUseCardIndexList.RemoveAt(0);
                    RecycleCardAnimationEndState = false;
                }
                else
                {
                    RecycleCardAnimationState = true;
                    Anim_RecycleCard.Play("RecycleCard");
                }
                if (!CardPoolsDataSave && RecycleCardAnimationState)
                {
                    Card_img = GameObject.Find($"CardPool/Card/img_Card{RoundOverUnUseCardIndexList[0]}").GetComponent<Image>();
                    Card_img.transform.localScale = Vector3.zero;
                    UsedCardList.Add(ATKList?[RoundOverUnUseCardIndexList[0] - 1]);
                    CardPoolsDataSave = true;
                }
            }
            else
            {
                if (!Anim_RecycleCard.isPlaying)
                {
                    CardRecycleSuccess = true;
                }
            }
            #endregion
            #region 发牌动画

            if (CardRecycleSuccess)//回收动画结束
            {
                if (txt_StartCardCount.text == "0")
                {
                    #region 洗牌动画
                    if (ShuffleCount == 0)//动画只开启一次
                    {
                        Shuffle_Obj.SetActive(true);
                        Anim_Shuffle.Play("Shuffle");
                        Anim_DealCards["DealCards"].speed = 0;//动画暂停
                        DealCardsAnimState = false;
                    }
                    ShuffleCount++;
                    if (!Anim_Shuffle.isPlaying)
                    {
                        Shuffle_Obj.SetActive(false);
                        if (UnusedCardList == null || UnusedCardList?.Count == 0)
                        {
                            UnusedCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUsedCardPoolsFileName).ListRandom();
                            WhenShuffleCardCount = UnusedCardList.Count;
                        }
                        txt_StartCardCount.text = WhenShuffleCardCount.ToString();

                        UsedCardList = new List<CurrentCardPoolModel>();
                        Common.SaveTxtFile(UsedCardList.ListToJson(), GlobalAttr.CurrentUsedCardPoolsFileName);
                        txt_EndCardCount.text = "0";
                        ShuffleCount = 0;
                        Anim_DealCards["DealCards"].speed = 1;//动画继续
                        DealCardsAnimState = true;
                    }
                    #endregion
                }
                else
                {
                    #region 卡牌先赋值再执行动画
                    if (CardCount < 5)
                    {
                        CardAssignment();
                        CardCount++;
                    }
                    else
                    {
                        CardAssignmentState = true;
                    }
                    #endregion
                    #region 发牌动画
                    if (CardAssignmentState)
                    {
                        if (DealCardCount == 0)
                        {
                            Anim_DealCards.Play("DealCards");
                            DealCardCount++;
                        }
                        else
                        {
                            if (!Anim_DealCards.isPlaying)
                            {
                                #region 状态初始化
                                CardPoolsDataSave = false;
                                CardRecycleSuccess = false;
                                RoundOverState = false;
                                HasUseCard = false;
                                AiAtkState = true;
                                CardAssignmentState = false;
                                RoundOverUnUseCardIndexList = new List<int>() { 1, 2, 3, 4, 5 };
                                CardCount = 0;
                                DealCardCount = 0;
                                #endregion
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion
        }

        /// <summary>
        /// 卡牌赋值
        /// </summary>
        public void CardAssignment()
        {
            //Image Card = GameObject.Find($"CardPool/Card/img_Card{CardCount + 1}").GetComponent<Image>();
            //Card.transform.localScale = Vector3.one;
            //Card.transform.localEulerAngles = new Vector3(0, 0, 0);//旋转初始化
            UnusedCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUnUsedCardPoolsFileName);
            if (UnusedCardList == null || UnusedCardList?.Count == 0)
            {
                UnusedCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentUsedCardPoolsFileName).ListRandom();
                WhenShuffleCardCount = UnusedCardList.Count;
            }
            Image Card_ATK_img = GameObject.Find($"Card/img_Card{(CardCount + 1)}/img_ATK").GetComponent<Image>();
            var model = UnusedCardList[0];
            var cardType = model.StateType;
            #region 攻击力图标
            if (cardType == 6 || cardType == 7 || cardType == 8 || cardType == 9)//是否隐藏
            {
                Card_ATK_img.transform.localScale = Vector3.zero;
            }
            else
            {
                var Card_ATK_icon = GameObject.Find($"img_Card{(CardCount + 1)}/img_ATK/Image").GetComponent<Image>();
                if (cardType == 1)
                {
                    Common.ImageBind("Images/Defense", Card_ATK_icon);
                }
                else if (cardType == 2 || cardType == 3)
                {
                    Common.ImageBind("Images/HP_Icon", Card_ATK_icon);
                }
                else if (cardType == 5)
                {
                    Common.ImageBind("Images/CardIcon/ShuiJin", Card_ATK_icon);
                }
                else
                {
                    Common.ImageBind("Images/Atk_Icon", Card_ATK_icon);
                }
                var Card_ATKNumber = GameObject.Find($"img_Card{(CardCount + 1)}/img_ATK/Text").GetComponent<Text>();
                Card_ATKNumber.text = model.Effect.ToString();
            }
            #endregion
            #region 能量图标
            var Card_energy_img = GameObject.Find($"Card/img_Card{(CardCount + 1)}/Image").GetComponent<Image>();
            if (model.Consume == 0)
            {
                Card_energy_img.transform.localScale = Vector3.zero;
            }
            else
            {
                Card_energy_img.transform.localScale = Vector3.one;
            }
            #endregion
            var Card_Skill_img = GameObject.Find($"Card/img_Card{(CardCount + 1)}/img_Skill").GetComponent<Image>();
            Common.ImageBind(model.CardUrl, Card_Skill_img);
            var Card_Energy = GameObject.Find($"Card/img_Card{(CardCount + 1)}/Image/Text").GetComponent<Text>();
            Card_Energy.text = model.Consume.ToString();
            var Card_Title = GameObject.Find($"Card/img_Card{(CardCount + 1)}/img_Title/Text").GetComponent<Text>();
            Card_Title.text = model.CardName.TextSpacing();

            #region 卡牌池操作
            ATKList.RemoveAt(0);
            ATKList.Add(model);
            UnusedCardList.RemoveAt(0);
            Common.SaveTxtFile(ATKList.ListToJson(), GlobalAttr.CurrentATKBarCardPoolsFileName);
            Common.SaveTxtFile(UnusedCardList.ListToJson(), GlobalAttr.CurrentUnUsedCardPoolsFileName);
            #endregion
        }

        #endregion

        #region 玩家回合结束AI攻击
        /// <summary>
        /// ai攻击
        /// </summary>
        public void AIAtk()
        {
            if (!AiTitleAnimState)
            {
                AiRoundTitle_Obj.SetActive(true);
                Anim_AiRoundTitle.Play("AiRoundTitle");
                AiTitleAnimState = true;
            }
            else if (!Anim_AiRoundTitle.isPlaying)
            {
                if (AiCardList == null || AiAtkCardList == null)
                {
                    AiCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentAiCardPoolsFileName);
                    AiAtkCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CurrentAIATKCardPoolsFileName).OrderByDescending(a => a.AiAtkSort).ToList();
                    AiAtkCount = AiAtkCardList.Count;
                }
                else
                {
                    if (AiAtkCount > 0)
                    {
                        if (!AiAnimStartState)
                        {
                            AiAtkCount--;
                            var model = AiAtkCardList[AiAtkCount];
                            switch (model?.StateType)
                            {
                                case 0:
                                    Atk_Obj.SetActive(true);
                                    Anim_ATK.transform.localPosition = new Vector3(-408, 0);
                                    Anim_ATK.Play("ATK");
                                    AiAnimStartState = true;
                                    float DeductionHp = model.Effect;
                                    if (PlayerRole.Armor > 0)
                                    {
                                        if (PlayerRole.Armor >= model.Effect)
                                        {
                                            DeductionHp = 0;
                                            PlayerRole.Armor -= Convert.ToInt32(model.Effect);
                                            Player_txt_Armor.text = PlayerRole.Armor.ToString();
                                        }
                                        else
                                        {
                                            DeductionHp -= PlayerRole.Armor;
                                            PlayerRole.Armor = 0;
                                        }
                                        if (PlayerRole.Armor == 0)
                                        {
                                            Player_img_Armor.transform.localScale = Vector3.zero;
                                        }
                                    }
                                    PlayerRole.HP -= DeductionHp;
                                    if (PlayerRole.HP <= 0)
                                    {
                                        PlayerRole.HP = 0;
                                        GameOver();
                                    }
                                    Common.HPImageChange(Pimg_HP, PlayerRole.MaxHP, DeductionHp, 0);
                                    Player_HP.text = $"{PlayerRole.MaxHP}/{PlayerRole.HP}";
                                    break;
                                case 1:
                                    Armor_Obj.SetActive(true);
                                    AiAnimStartState = true;
                                    Anim_Armor.transform.localPosition = new Vector3(373, 0);//攻击特效
                                    Anim_Armor.Play("Armor");
                                    Enemy_img_Armor.transform.localScale = Vector3.one;
                                    AiRole.Armor += Convert.ToInt32(model.Effect);
                                    if (AiRole.Armor > AiRole.MaxHP)
                                    {
                                        AiRole.Armor = Convert.ToInt32(AiRole.MaxHP);
                                    }
                                    Enemy_txt_Armor.text = AiRole.Armor.ToString();
                                    break;
                                default:
                                    break;
                            }

                            Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
                            Common.SaveTxtFile(AiRole.ObjectToJson(), GlobalAttr.CurrentAIRoleFileName);
                        }
                        else
                        {
                            var model = AiAtkCardList[AiAtkCount];
                            switch (model?.StateType)
                            {
                                case 0:
                                    if (!Anim_ATK.isPlaying)
                                    {
                                        AiAnimStartState = false;
                                    }
                                    break;
                                case 1:
                                    if (!Anim_Armor.isPlaying)
                                    {
                                        AiAnimStartState = false;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        AiAtkState = false;
                        AiTitleAnimState = false;
                        AiAnimStartState = false;
                        AiAtkCardList = new List<CurrentCardPoolModel>();
                        //攻击栏最大五张牌
                        var AtkCardNum = AiRole.AILevel + 1;
                        AiCardList.ListRandom();
                        if (AtkCardNum > 5) AtkCardNum = 5;
                        for (int i = 0; i < AtkCardNum; i++)
                        {
                            AiAtkCardList.Add(AiCardList[i]);
                        }
                        Common.SaveTxtFile(AiAtkCardList.ListToJson(), GlobalAttr.CurrentAIATKCardPoolsFileName);
                        AIATKCardPoolsBind();
                        AiAtkCount = AiAtkCardList.Count;
                    }
                }
            }
        }


        /// <summary>
        /// AI攻击牌池绑定
        /// </summary>
        public void AIATKCardPoolsBind()
        {
            //删除原有的攻击栏图片
            GameObject parentObject = GameObject.Find("GameCanvas/Enemy/ATKBar");
            var parantCount = parentObject.transform.childCount;
            for (int i = 0; i < parantCount; i++)
            {
                DestroyImmediate(parentObject.transform.GetChild(0).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
            if (AiAtkCardList != null && AiAtkCardList?.Count > 0)
            {
                for (int i = 0; i < AiAtkCardList.Count; i++)
                {
                    var item = AiAtkCardList[i];
                    GameObject tempObj = Resources.Load("Prefab/AI_ATKimg_Prefab") as GameObject;
                    tempObj.name = item.ID + "_" + i;
                    tempObj = Common.AddChild(parentObject.transform, tempObj);
                    var tempImg = parentObject.transform.Find($"{item.ID}_{i}(Clone)").GetComponent<Image>();
                    Common.ImageBind(item.CardUrl, tempImg);
                }
            }
        }
        #endregion

        #region 游戏结束

        /// <summary>
        /// AI死亡
        /// </summary>
        public void AiDieGameOver()
        {
            Common.SceneJump("AiDieScene");
        }

        /// <summary>
        /// 玩家死亡
        /// </summary>
        public void GameOver()
        {
            Anim_GameOver_obj.SetActive(true);
            Anim_GameOver.Play("GameOver");
            GameOverStartState = true;
        }
        #endregion
    }
}
