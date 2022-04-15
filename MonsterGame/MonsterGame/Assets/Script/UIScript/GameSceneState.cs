using Assets.Script.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.UIScript
{
    /// <summary>
    /// 全局状态
    /// </summary>
    public class GameScene_GlobalState : StateBase<GameScene>
    {
        private static GameScene_GlobalState instance;
        public static GameScene_GlobalState Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameScene_GlobalState();

                return instance;
            }
        }

        public override void OnEnter(GameScene entity)
        {

        }

        public override void Execute(GameScene entity)
        {

        }

        public override void OnExit(GameScene entity)
        {

        }

    }

    /// <summary>
    /// 当前状态
    /// </summary>
    public class GameScene_StateIdle : StateBase<GameScene>
    {
        private static GameScene_StateIdle instance;
        public static GameScene_StateIdle Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameScene_StateIdle();

                return instance;
            }
        }

        public override void OnEnter(GameScene entity)
        {
            Debug.Log("进入状态");
        }

        public override void Execute(GameScene entity)
        {
            //Debug.Log("状态执行中");
            //判断是否触发攻击
            #region 单次攻击

            if (entity.txt_AtkState.text == "1")
            {
                entity.SingleATK();
                entity.BtnLoseEfficacy();
            }

            #endregion

            #region 自动攻击

            if (entity.txt_AutoState.text == "1")
            {
                entity.AutoATK();
            }

            #endregion
        }

        public override void OnExit(GameScene entity)
        {
            Debug.Log("状态结束");
            entity.BtnEffective();
        }

    }
}
