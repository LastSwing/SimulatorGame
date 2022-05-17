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
        GameObject DefensePanel, ATKPanel, Shuffle, tempObj;
        Animation Anim_Restore, Anim_ATK, Anim_Armor;
        Button btn_RoundOver;
        Image Card_img, Eimg_HP, Player_img_Armor, Pimg_HP, Enemy_img_Armor;
        Text txt_EndCardCount, txt_StartCardCount, Player_txt_Armor, Player_HP, Enemy_txt_Armor;
        public int clickCount, currrentIndex;//当前图片下标
        private int RotationCount, CardCount, ShuffleCount, AiAtkCount;
        private float totalTimer;// 定义每帧累加时间
        private bool hasDrag = false;//是否拖拽
        public bool RecycleCardAnimationState, RecycleCardAnimationEndState,//回收卡牌动画
            CardListAnimationState, CardListAnimationEndState, //卡牌发牌动画
            RoundOverState, CardPoolsDataSave, HasUseCard, CardRecycleSuccess,//回合结束状态、卡池数据保存、是否使用了卡牌、卡牌回收成功
            RotationCardAnimationState, RotationCardAnimationEndState,//卡牌旋转动画
            ShuffleAnimationState, FoldCardAnimationState, ShuffleAnimationSuccessState,   //洗牌状态、叠牌状态、动画完成
            AiAtkState
            = false;

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

            #region 动画控件
            tempObj = Common.AddChild(GameObject.Find("GanmeCanvas").transform, (GameObject)Resources.Load("Prefab/Anim_Restore"));
            tempObj.name = "Anim_Restore";
            Anim_Restore = GameObject.Find("GanmeCanvas/Anim_Restore").GetComponent<Animation>();

            tempObj = Common.AddChild(GameObject.Find("GanmeCanvas").transform, (GameObject)Resources.Load("Prefab/Anim_ATK"));
            tempObj.name = "Anim_ATK";
            Anim_ATK = GameObject.Find("GanmeCanvas/Anim_ATK").GetComponent<Animation>();

            tempObj = Common.AddChild(GameObject.Find("GanmeCanvas").transform, (GameObject)Resources.Load("Prefab/Anim_Armor"));
            tempObj.name = "Anim_Armor";
            Anim_Armor = GameObject.Find("GanmeCanvas/Anim_Armor").GetComponent<Animation>();
            #endregion

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

            Shuffle = GameObject.Find("Shuffle");
            Shuffle.SetActive(false);
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
                ATKList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.ATKBarCardPoolsFileName);
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

                            Common.HPImageChange(Eimg_HP, AiRole.MaxHP, atkModel.Effect, 0);
                            AiRole.HP -= atkModel.Effect;
                            if (AiRole.HP <= 0)
                            {
                                AiRole.HP = 0;
                                AiDieGameOver();
                            }
                            var HpTxt = GameObject.Find("Enemy/Text").GetComponent<Text>();
                            HpTxt.text = $"{AiRole.MaxHP}/{AiRole.HP}";
                            Anim_ATK.transform.localPosition = new Vector3(373, 0);//攻击特效
                            Anim_ATK.Play("ATK");
                            #region 执行动画
                            Card_img.transform.localScale = Vector3.zero;
                            Card_img.transform.localPosition = new Vector3(-298 + 149 * index, 0, 0);
                            UsedCardList.Add(atkModel);
                            CardPoolsDataSave = true;
                            RoundOverUnUseCardIndexList.Remove(index + 1);
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
                }
                //功能卡只能在玩家范围内使用
                else if (cardPosition.x < Scope.x && cardPosition.y > Scope.y)
                {

                    #region 能量恢复5
                    if (stateType == 5)
                    {
                        atkModel.Proficiency++;
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
                            Player_HP.text = $"{PlayerRole.MaxHP}/{PlayerRole.HP}";
                            Common.HPImageChange(Eimg_HP, AiRole.MaxHP, atkModel.Effect, 1);
                            if (PlayerRole.HP > PlayerRole.MaxHP)
                            {
                                PlayerRole.HP = PlayerRole.MaxHP;
                            }
                            Anim_Restore.transform.localPosition = new Vector3(-408, 0);
                            Anim_Restore.Play("Restore");
                            #region 执行动画
                            Card_img.transform.localScale = Vector3.zero;
                            Card_img.transform.localPosition = new Vector3(-298 + 149 * index, 0, 0);//位置初始化
                            UsedCardList.Add(atkModel);
                            CardPoolsDataSave = true;
                            RoundOverUnUseCardIndexList.Remove(index + 1);
                            //执行动画
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
                Image rightImg = GameObject.Find($"CardPool/right_Card/right_Card9").GetComponent<Image>();
                rightImg.transform.localScale = Vector3.one;
                var x = rightImg.transform.position.x;
                rightImg.transform.position = new Vector3(x + 1, rightImg.transform.position.y - 1);
                rightImg.transform.Rotate(Vector3.up, -1);
                if (x >= 1110)
                {
                    txt_EndCardCount.text = UsedCardList.Count.ToString();
                    Common.SaveTxtFile(UsedCardList.ListToJson(), GlobalAttr.UsedCardPoolsFileName);
                    RecycleCardAnimationState = false;
                    rightImg.transform.localScale = Vector3.zero;
                    rightImg.transform.localPosition = new Vector3(-90f, 130f);
                    RecycleCardAnimationEndState = true;
                    CardPoolsDataSave = false;
                }
            }
        }

        /// <summary>
        /// 初始牌堆动画
        /// </summary>
        public void CardListAnimation()
        {
            Image leftImg = GameObject.Find($"CardPool/left_Card/left_Card9").GetComponent<Image>();
            var x = leftImg.transform.position.x;
            leftImg.transform.position = new Vector3(x + 1, leftImg.transform.position.y);
            leftImg.transform.Rotate(Vector3.up, -10);
            if (x >= 268)
            {
                CardListAnimationState = false;
                leftImg.transform.localPosition = new Vector3(0, 9);
                leftImg.transform.rotation = new Quaternion(0, 0, 0, 0);
                CardListAnimationEndState = true;

            }
        }

        /// <summary>
        /// 翻转卡牌动画
        /// </summary>
        public void RotationCardAnimation()
        {
            totalTimer += Time.deltaTime;
            if (totalTimer >= 0.01)
            {
                Image Card = GameObject.Find($"CardPool/Card/img_Card{CardCount + 1}").GetComponent<Image>();
                Card.transform.localScale = Vector3.one;
                Card.transform.Rotate(Vector3.up, -10);
                RotationCount++;
                if (RotationCount == 18)
                {
                    txt_StartCardCount.text = (Convert.ToInt32(txt_StartCardCount.text) - 1).ToString();
                    RotationCount = 0;
                    RotationCardAnimationEndState = true;
                }
                totalTimer = 0;
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
            if (RoundOverUnUseCardIndexList != null && RoundOverUnUseCardIndexList.Count > 0)
            {
                #region 添加动画
                if (RecycleCardAnimationEndState)
                {
                    RoundOverUnUseCardIndexList.RemoveAt(0);
                    RecycleCardAnimationEndState = false;
                }
                else
                {
                    RecycleCardAnimationState = true;
                }
                #endregion
                #region 卡牌隐藏
                if (!CardPoolsDataSave && RecycleCardAnimationState)
                {
                    Card_img = GameObject.Find($"CardPool/Card/img_Card{RoundOverUnUseCardIndexList[0]}").GetComponent<Image>();
                    Card_img.transform.localScale = Vector3.zero;
                    UsedCardList.Add(ATKList?[RoundOverUnUseCardIndexList[0] - 1]);
                    CardPoolsDataSave = true;
                }
                #endregion
            }
            else
            {
                CardRecycleSuccess = true;
                RecycleCardAnimationEndState = false;
                RecycleCardAnimationState = false;
            }
            #endregion
            #region 洗牌动画

            if (CardRecycleSuccess)//回收动画结束
            {
                if (txt_StartCardCount.text == "0" && CardCount == 0)
                {
                    ShuffleAnimationState = true;
                    if (ShuffleAnimationSuccessState)
                    {
                        #region 发牌动画
                        if (CardCount < 5)
                        {
                            if (CardListAnimationEndState)//动画结束
                            {
                                #region 卡牌赋值
                                #endregion
                                if (RotationCardAnimationEndState)
                                {
                                    CardAssignment();
                                    CardCount++;
                                    RotationCardAnimationEndState = false;
                                }
                                else
                                {
                                    RotationCardAnimationState = true;//卡牌旋转动画
                                }
                            }
                            else
                            {
                                CardListAnimationState = true;
                            }
                        }
                        else
                        {
                            #region 状态初始化
                            CardRecycleSuccess = false;
                            CardPoolsDataSave = false;
                            CardListAnimationState = false;
                            CardListAnimationEndState = false;
                            RotationCardAnimationState = false;
                            RoundOverState = false;
                            HasUseCard = false;
                            AiAtkState = true;
                            RoundOverUnUseCardIndexList = new List<int>() { 1, 2, 3, 4, 5 };
                            CardCount = 0;
                            #endregion

                        }
                        #endregion
                    }
                }
                else
                {
                    #region 发牌动画
                    if (CardCount < 5)
                    {
                        if (CardListAnimationEndState)//动画结束
                        {
                            #region 卡牌赋值
                            #endregion
                            if (RotationCardAnimationEndState)
                            {
                                CardAssignment();
                                CardCount++;
                                RotationCardAnimationEndState = false;
                            }
                            else
                            {
                                RotationCardAnimationState = true;//卡牌旋转动画
                            }
                        }
                        else
                        {
                            CardListAnimationState = true;
                        }
                    }
                    else
                    {
                        #region 状态初始化
                        CardRecycleSuccess = false;
                        CardPoolsDataSave = false;
                        CardListAnimationState = false;
                        CardListAnimationEndState = false;
                        RotationCardAnimationState = false;
                        RoundOverState = false;
                        HasUseCard = false;
                        AiAtkState = true;
                        RoundOverUnUseCardIndexList = new List<int>() { 1, 2, 3, 4, 5 };
                        CardCount = 0;
                        #endregion

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
            Image Card = GameObject.Find($"CardPool/Card/img_Card{CardCount + 1}").GetComponent<Image>();
            Card.transform.localScale = Vector3.one;
            Card.transform.localEulerAngles = new Vector3(0, 0, 0);//旋转初始化
            UnusedCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.UnUsedCardPoolsFileName);
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
            Common.SaveTxtFile(ATKList.ListToJson(), GlobalAttr.ATKBarCardPoolsFileName);
            Common.SaveTxtFile(UnusedCardList.ListToJson(), GlobalAttr.UnUsedCardPoolsFileName);
            #endregion
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        public void ShuffleAnimation()
        {
            if (!ShuffleAnimationSuccessState)
            {
                #region 洗牌动画
                if (ShuffleCount < 4)
                {
                    Shuffle.SetActive(true);
                    Image Top_Img = GameObject.Find("Shuffle/Top_CardPools/Top_CardPools1").GetComponent<Image>();
                    var thisPosition = Top_Img.transform.localPosition;
                    Top_Img.transform.localPosition = new Vector3(thisPosition.x - 1.2f, thisPosition.y - 1);
                    if (thisPosition.x < -65)
                    {
                        ShuffleCount++;
                        Top_Img.transform.localPosition = new Vector3(0, 1);
                    }
                }
                else
                {
                    #region 叠牌

                    Image Top_ImgPools = GameObject.Find("Shuffle/Top_CardPools").GetComponent<Image>();
                    Image Down_ImgPools = GameObject.Find("Shuffle/Down_CardPools").GetComponent<Image>();
                    var thisPosition = Down_ImgPools.transform.localPosition;
                    if (!FoldCardAnimationState)
                    {
                        Top_ImgPools.transform.localEulerAngles = new Vector3(20, -30, 10);
                        Down_ImgPools.transform.localEulerAngles = new Vector3(15, -30, 15);
                        Down_ImgPools.transform.localPosition = new Vector3(thisPosition.x + 1f, thisPosition.y + 1.2f);
                    }
                    #endregion
                    if (thisPosition.y > 17)
                    {
                        FoldCardAnimationState = true;
                    }
                    if (FoldCardAnimationState)
                    {
                        Top_ImgPools.transform.localPosition = new Vector3(thisPosition.x - 48, thisPosition.y - 30);
                        Down_ImgPools.transform.localPosition = new Vector3(thisPosition.x - 48, thisPosition.y - 30);
                        if (thisPosition.y < -300)
                        {
                            Top_ImgPools.transform.localPosition = new Vector3(18, 17);
                            Down_ImgPools.transform.localPosition = new Vector3(-23, -33);
                            Top_ImgPools.transform.localEulerAngles = new Vector3(30, -30, 0);
                            Down_ImgPools.transform.localEulerAngles = new Vector3(30, -30, 0);
                            ShuffleAnimationState = false;
                            FoldCardAnimationState = false;
                            ShuffleAnimationSuccessState = true;
                            ShuffleCount = 0;
                            Shuffle.SetActive(false);
                        }
                    }
                }
                #endregion
            }
            else
            {
                UnusedCardList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.UsedCardPoolsFileName).ListRandom();
                Common.SaveTxtFile(UnusedCardList.ListToJson(), GlobalAttr.UnUsedCardPoolsFileName);
                txt_StartCardCount.text = UnusedCardList.Count.ToString();

                UsedCardList = new List<CurrentCardPoolModel>();
                Common.SaveTxtFile(UsedCardList.ListToJson(), GlobalAttr.UsedCardPoolsFileName);
                txt_EndCardCount.text = "0";

                ShuffleAnimationState = false;
                ShuffleAnimationSuccessState = false;
            }
        }
        #endregion

        #region 玩家回合结束AI攻击
        /// <summary>
        /// ai攻击
        /// </summary>
        public void AIAtk()
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
                    AiAtkCount--;
                    var model = AiAtkCardList[AiAtkCount];
                    switch (model?.StateType)
                    {
                        case 0:
                            Anim_ATK.transform.localPosition = new Vector3(-408, 0);
                            Anim_ATK.Play("ATK");
                            PlayerRole.HP -= model.Effect;
                            Common.HPImageChange(Pimg_HP, PlayerRole.MaxHP, model.Effect, 0);
                            Player_HP.text = $"{PlayerRole.MaxHP}/{PlayerRole.HP}";
                            break;
                        case 1:
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
                    AiAtkState = false;
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
                    AiAtkCardList = null;
                }
            }
        }


        /// <summary>
        /// AI攻击牌池绑定
        /// </summary>
        public void AIATKCardPoolsBind()
        {
            //删除原有的攻击栏图片
            GameObject parentObject = GameObject.Find("GanmeCanvas/Enemy/ATKBar");
            for (int i = 0; i < parentObject.transform.childCount; i++)
            {
                DestroyImmediate(parentObject.transform.GetChild(i).gameObject);//如不是删除后马上要使用则用Destroy方法
            }
            if (AiAtkCardList != null && AiAtkCardList?.Count > 0)
            {
                foreach (var item in AiAtkCardList)
                {
                    GameObject tempObj = Resources.Load("Prefab/AI_ATKimg_Prefab") as GameObject;
                    tempObj.name = item.ID;
                    tempObj = Common.AddChild(parentObject.transform, tempObj);
                    var tempImg = parentObject.transform.Find(item.ID + "(Clone)").GetComponent<Image>();
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

        }

        #endregion
    }
}
