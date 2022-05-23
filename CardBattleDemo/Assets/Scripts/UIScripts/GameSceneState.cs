using Assets.Scripts.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UIScripts
{
    /// <summary>
    /// 当前状态
    /// </summary>
    public class GameSceneState : StateBase<GameScene>
    {
        private static GameSceneState instance;
        /// <summary>
        /// 初始化实例
        /// </summary>
        public static GameSceneState Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameSceneState();

                return instance;
            }
        }

        public override void OnEnter(GameScene entity)
        {
            //Debug.Log("进入状态");
        }

        public override void Execute(GameScene entity)
        {
            //Debug.Log("GameScene状态执行中");

        }

        public override void OnExit(GameScene entity)
        {
            Debug.Log("状态结束");
        }

    }
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
}
