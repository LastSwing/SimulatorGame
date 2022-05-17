using Assets.Scripts.FSM;
using Assets.Scripts.LogicalScripts.Models;
using Assets.Scripts.Tools;
using Assets.Scripts.UIScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
    StateMachine<GameScene> myStateMachine; //状态机
    private GameObject GameStart;
    private CurrentRoleModel PlayerRole;    //玩家角色
    private CurrentRoleModel AiRole;        //Ai角色
    /// <summary>
    /// 未使用的卡池
    /// </summary>
    private List<CurrentCardPoolModel> UnusedCardList = new List<CurrentCardPoolModel>();
    /// <summary>
    /// 当前攻击栏卡池
    /// </summary>
    private List<CurrentCardPoolModel> ATKBarCardList = new List<CurrentCardPoolModel>();
    /// <summary>
    /// 已使用的卡池
    /// </summary>
    private List<CurrentCardPoolModel> UsedCardList = new List<CurrentCardPoolModel>();
    /// <summary>
    /// AI卡池
    /// </summary>
    private List<CurrentCardPoolModel> AiCardList = new List<CurrentCardPoolModel>();
    /// <summary>
    /// AI攻击牌池
    /// </summary>
    private List<CurrentCardPoolModel> AiATKCardList = new List<CurrentCardPoolModel>();
    private Image Player, Enemy, Card_ATK_img, Card_ATK_icon, Card_Skill_img, Card_energy_img, Player_img_Armor, Enemy_img_Armor;
    private Text Player_HP, Ai_HP, txt_StartCardCount, txt_EndCardCount, Card_Energy, Card_ATKNumber, Card_Title, Player_txt_Armor, Enemy_txt_Armor;
    public bool GameStartAnimationState = true;//初始动画
    public bool GameStartAnimationEndState, GameStartAnimationMoveState, CardListAnimationState, RotationCardAnimationState = false;
    // 定义每帧累加时间
    private float totalTimer;
    private int GameStartCount, RotationCount, CardCount;
    private Vector3 GameStartPosition;

    void Start()
    {

        #region 控件初始化

        Player = transform.Find("Player/Player").GetComponent<Image>();
        Enemy = transform.Find("Enemy").GetComponent<Image>();
        Player_img_Armor = transform.Find("Player/img_Armor").GetComponent<Image>();
        Enemy_img_Armor = transform.Find("Enemy/img_Armor").GetComponent<Image>();

        Enemy_txt_Armor = transform.Find("Enemy/img_Armor/Text").GetComponent<Text>();
        Player_txt_Armor = transform.Find("Player/img_Armor/Text").GetComponent<Text>();
        Player_HP = transform.Find("Player/Text").GetComponent<Text>();
        Ai_HP = transform.Find("Enemy/Text").GetComponent<Text>();
        txt_StartCardCount = transform.Find("CardPool/left_Card/txt_StartCardCount").GetComponent<Text>();
        txt_EndCardCount = transform.Find("CardPool/right_Card/txt_EndCardCount").GetComponent<Text>();

        GameStart = transform.Find("GameStart").gameObject;
        #endregion
        #region 状态机初始化

        myStateMachine = new StateMachine<GameScene>(this);
        myStateMachine.SetCurrentState(GameSceneState.Instance);
        myStateMachine.SetGlobalState(GameScene_GlobalState.Instance);
        #endregion
        Init();
        GameStartAnimaImgHide();
    }

    void Init()
    {
        #region 数据源初始化
        Common.DelereTxtFile(GlobalAttr.UsedCardPoolsFileName);
        PlayerRole = Common.GetTxtFileToList<CurrentRoleModel>(GlobalAttr.PlayerRolePoolFileName).Find(a => a.RoleID == "2022042716410125");//Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentPlayerRoleFileName) ?? 
        AiRole = Common.GetTxtFileToList<CurrentRoleModel>(GlobalAttr.AIRolePoolFileName).Find(a => a.RoleID == "2022042809503249");//Common.GetTxtFileToModel<CurrentRoleModel>(GlobalAttr.CurrentAIRoleFileName) ?? 
        Common.SaveTxtFile(PlayerRole.ObjectToJson(), GlobalAttr.CurrentPlayerRoleFileName);
        Common.SaveTxtFile(AiRole.ObjectToJson(), GlobalAttr.CurrentAIRoleFileName);
        var cardPoolList = Common.GetTxtFileToList<CurrentCardPoolModel>(GlobalAttr.CardPoolFileName) ?? new List<CurrentCardPoolModel>();//卡池
        #endregion
        txt_EndCardCount.text = UsedCardList == null ? "0" : UsedCardList.Count.ToString();
        Player_HP.text = $"{PlayerRole.MaxHP}/{PlayerRole.HP}";
        Ai_HP.text = $"{AiRole.MaxHP}/{AiRole.HP}";
        Common.ImageBind(PlayerRole.RoleImgUrl, Player);
        Common.ImageBind(AiRole.RoleImgUrl, Enemy);
        if (PlayerRole.Armor > 0)
        {
            Player_img_Armor.transform.localScale = Vector3.one;
            Player_txt_Armor.text = PlayerRole.Armor.ToString();
        }
        else
        {
            Player_img_Armor.transform.localScale = Vector3.zero;
        }
        if (AiRole.Armor > 0)
        {
            Enemy_img_Armor.transform.localScale = Vector3.one;
            Enemy_txt_Armor.text = PlayerRole.Armor.ToString();
        }
        else
        {
            Enemy_img_Armor.transform.localScale = Vector3.zero;
        }
        CreateEnergyImage(PlayerRole.MaxEnergy);

        if (cardPoolList != null && cardPoolList?.Count > 0)
        {
            #region 玩家卡池

            if (!string.IsNullOrEmpty(PlayerRole.CardListStr))
            {
                var arr = PlayerRole.CardListStr.Split(';');
                for (int i = 0; i < arr.Length; i++)
                {
                    CurrentCardPoolModel cardModel = new CurrentCardPoolModel();
                    var id = arr[i].Split('|')[0].ToString().Trim();
                    if (!string.IsNullOrEmpty(id))
                    {
                        cardModel = cardPoolList?.Find(a => a.ID == id);
                        UnusedCardList.Add(cardModel);
                    }
                }
                UnusedCardList.ListRandom();
                txt_StartCardCount.text = UnusedCardList.Count.ToString();
            }
            else
            {
                txt_StartCardCount.text = "0";
            }
            #endregion

            #region AI牌池
            if (!string.IsNullOrEmpty(AiRole.CardListStr))
            {
                var arr = AiRole.CardListStr.Split(';');
                for (int i = 0; i < arr.Length; i++)
                {
                    CurrentCardPoolModel cardModel = new CurrentCardPoolModel();
                    var id = arr[i].Split('|')[0].ToString().Trim();
                    if (!string.IsNullOrEmpty(id))
                    {
                        cardModel = cardPoolList.Find(a => a.ID == id);
                        AiCardList.Add(cardModel);
                    }
                }
                AiCardList.ListRandom();
                Common.SaveTxtFile(AiCardList.ListToJson(), GlobalAttr.CurrentAiCardPoolsFileName);

                #region AI攻击栏
                //攻击栏最大五张牌
                var AtkCardNum = AiRole.AILevel + 1;
                if (AtkCardNum > 5) AtkCardNum = 5;
                for (int i = 0; i < AtkCardNum; i++)
                {
                    AiATKCardList.Add(AiCardList[i]);
                }
                Common.SaveTxtFile(AiATKCardList.ListToJson(), GlobalAttr.CurrentAIATKCardPoolsFileName);
                #endregion
            }
            #endregion

            #region 攻击栏卡池
            for (int i = 0; i < 5; i++)
            {
                ATKBarCardList.Add(UnusedCardList[i]);
                UnusedCardList.Remove(UnusedCardList[i]);
            }
            CardAssignment();
            Common.SaveTxtFile(ATKBarCardList.ListToJson(), GlobalAttr.ATKBarCardPoolsFileName);
            Common.SaveTxtFile(UnusedCardList.ListToJson(), GlobalAttr.UnUsedCardPoolsFileName);
            #endregion 
        }
        AIATKCardPoolsBind();
    }

    // Update is called once per frame
    void Update()
    {
        //数据初始化后开始发牌
        myStateMachine.FSMUpdate();
    }

    /// <summary>
    /// 创建能量图片
    /// </summary>
    public void CreateEnergyImage(int count)
    {
        GameObject parentObject = GameObject.Find("GanmeCanvas/CardPool");
        //Debug.Log(parentObject.transform.localPosition);
        for (int i = 0; i < count; i++)
        {
            GameObject tempBgObject = Resources.Load("Prefab/img_EnergyBg") as GameObject;
            tempBgObject = Common.AddChild(parentObject.transform, tempBgObject);
            tempBgObject.name = "img_EnergyBg" + i;
            tempBgObject.transform.localPosition = new Vector2(-553 + i * 33, 103);

            GameObject tempObject = Resources.Load("Prefab/img_Energy") as GameObject;
            tempObject = Common.AddChild(parentObject.transform, tempObject);
            tempObject.name = "img_Energy" + i;
            tempObject.transform.localPosition = new Vector2(-553 + i * 33, 103);
        }
    }



    /// <summary>
    /// 卡牌赋值
    /// </summary>
    public void CardAssignment()
    {
        for (int i = 0; i < 5; i++)
        {
            Card_ATK_img = GameObject.Find($"Card/img_Card{(i + 1)}/img_ATK").GetComponent<Image>();
            var model = ATKBarCardList[i];
            var cardType = ATKBarCardList[i].StateType;
            #region 攻击力图标
            if (cardType == 6 || cardType == 7 || cardType == 8 || cardType == 9)//是否隐藏
            {
                Card_ATK_img.transform.localScale = Vector3.zero;
            }
            else
            {
                Card_ATK_icon = GameObject.Find($"img_Card{(i + 1)}/img_ATK/Image").GetComponent<Image>();
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
                Card_ATKNumber = GameObject.Find($"img_Card{(i + 1)}/img_ATK/Text").GetComponent<Text>();
                Card_ATKNumber.text = model.Effect.ToString();
            }
            #endregion
            if (model.Consume == 0)
            {
                Card_energy_img = GameObject.Find($"Card/img_Card{(i + 1)}/Image").GetComponent<Image>();
                Card_energy_img.transform.localScale = Vector3.zero;
            }
            Card_Skill_img = GameObject.Find($"Card/img_Card{(i + 1)}/img_Skill").GetComponent<Image>();
            Common.ImageBind(model.CardUrl, Card_Skill_img);
            Card_Energy = GameObject.Find($"Card/img_Card{(i + 1)}/Image/Text").GetComponent<Text>();
            Card_Energy.text = model.Consume.ToString();
            Card_Title = GameObject.Find($"Card/img_Card{(i + 1)}/img_Title/Text").GetComponent<Text>();
            Card_Title.text = model.CardName.TextSpacing();

        }

    }

    /// <summary>
    /// AI攻击牌池绑定
    /// </summary>
    public void AIATKCardPoolsBind()
    {
        if (AiATKCardList != null && AiATKCardList?.Count > 0)
        {
            GameObject parentObject = GameObject.Find("GanmeCanvas/Enemy/ATKBar");
            foreach (var item in AiATKCardList)
            {
                GameObject tempObj = Resources.Load("Prefab/AI_ATKimg_Prefab") as GameObject;
                tempObj.name = item.ID;
                tempObj = Common.AddChild(parentObject.transform, tempObj);
                var tempImg = parentObject.transform.Find(item.ID + "(Clone)").GetComponent<Image>();
                Common.ImageBind(item.CardUrl, tempImg);
            }
        }
    }

    #region Animation
    /// <summary>
    /// 开始动画的图片隐藏
    /// </summary>
    private void GameStartAnimaImgHide()
    {
        Image imgStartText = GameObject.Find($"GameStart/imgStartText").GetComponent<Image>();
        Image rightImg = GameObject.Find($"CardPool/right_Card/right_Card9").GetComponent<Image>();
        rightImg.transform.localScale = Vector3.zero;
        imgStartText.transform.localScale = Vector3.one;
        for (int i = 1; i < 6; i++)
        {
            Image img = GameObject.Find($"GameStart/Image{i}").GetComponent<Image>();
            img.transform.localScale = Vector3.zero;
            Image Card = GameObject.Find($"CardPool/Card/img_Card{i}").GetComponent<Image>();
            Card.transform.localScale = Vector3.zero;
        }
    }

    /// <summary>
    /// 游戏开始动画
    /// </summary>
    public void GameStartAnimation()
    {
        totalTimer += Time.deltaTime;
        if (totalTimer >= 0.06)
        {
            if (GameStartCount < 5)
            {
                Image img = GameObject.Find($"GameStart/Image{GameStartCount + 1}").GetComponent<Image>();
                img.transform.localScale = Vector3.one;
                GameStartCount++;
            }
            if (GameStartCount == 5)
            {
                GameStartAnimationEndState = true;
                GameStartAnimationState = false;
            }
            totalTimer = 0;
        }
    }

    /// <summary>
    /// 游戏开始动画结束
    /// </summary>
    public void GameStartAnimationEnd()
    {
        totalTimer += Time.deltaTime;
        if (totalTimer >= 0.06)
        {
            if (GameStartCount > 0)
            {
                Image img = GameObject.Find($"GameStart/Image{GameStartCount}").GetComponent<Image>();
                img.transform.rotation = new Quaternion(0, 0, 0, 0);
                img.transform.localPosition = new Vector3(0, GameStartCount * 2, 0);
                GameStartCount--;
            }
            if (GameStartCount == 0)
            {
                Image img = GameObject.Find($"GameStart/imgStartText").GetComponent<Image>();
                img.transform.localScale = Vector3.zero;
                GameStartAnimationEndState = false;
                GameStartAnimationMoveState = true;
            }
            totalTimer = 0;
        }
    }
    /// <summary>
    /// 移动开始动画
    /// </summary>
    public void GameStartAnimationMove()
    {
        GameStartPosition = GameStart.transform.localPosition;
        GameStart.transform.localPosition = new Vector3(GameStartPosition.x - 6, GameStartPosition.y - 3);
        if (GameStartPosition.y <= -130)
        {
            GameStartAnimationMoveState = false;
            GameStart.SetActive(false);
            CardListAnimationState = true;
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
            RotationCardAnimationState = true;
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
                CardListAnimationState = true;
                CardCount++;
                txt_StartCardCount.text = (Convert.ToInt32(txt_StartCardCount.text) - 1).ToString();
                RotationCount = 0;
            }
            if (CardCount > 4)
            {
                CardListAnimationState = false;
                RotationCardAnimationState = false;
                CardCount = 0;
            }
            totalTimer = 0;
        }
    }
    #endregion


}
