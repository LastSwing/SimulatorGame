using Assets.Scripts.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UIScripts
{
    public class CardManageState : StateBase<CardManage>
    {
        private static CardManageState instance;
        /// <summary>
        /// 初始化实例
        /// </summary>
        public static CardManageState Instance
        {
            get
            {
                if (instance == null)
                    instance = new CardManageState();

                return instance;
            }
        }

        public override void OnEnter(CardManage entity)
        {
            Debug.Log("进入状态");
        }

        public override void Execute(CardManage entity)
        {
            //Debug.Log("状态执行中");

            #region 动画

            if (entity.RecycleCardAnimationState)
            {
                entity.RecycleCardAnimation();
            }
            if (entity.CardListAnimationState)
            {
                entity.CardListAnimation();
            }
            if (entity.RoundOverState)
            {
                entity.RoundOverAnimation();
            }
            if (entity.RotationCardAnimationState)
            {
                entity.RotationCardAnimation();
            }
            if (entity.ShuffleAnimationState)
            {
                entity.ShuffleAnimation();
            }
            #endregion
        }

        public override void OnExit(CardManage entity)
        {
            Debug.Log("状态结束");
        }

    }
    /// <summary>
    /// 全局状态
    /// </summary>
    public class CardManage_GlobalState : StateBase<CardManage>
    {
        private static CardManage_GlobalState instance;
        public static CardManage_GlobalState Instance
        {
            get
            {
                if (instance == null)
                    instance = new CardManage_GlobalState();

                return instance;
            }
        }

        public override void OnEnter(CardManage entity)
        {

        }

        public override void Execute(CardManage entity)
        {

        }

        public override void OnExit(CardManage entity)
        {

        }

    }
}
