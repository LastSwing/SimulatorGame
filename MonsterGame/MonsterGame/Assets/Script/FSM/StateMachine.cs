using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.FSM
{
    /// <summary>
    /// 状态机实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StateMachine<T>
    {

        /// <summary>
        ///  指向目前的状态
        /// </summary>
        public StateBase<T> CurrentState { get { return mpCurrentState; } }

        /// <summary>
        /// 指向全局状态
        /// </summary>
        public StateBase<T> GlobalState { get { return mpGlobalState; } }

        /// <summary>
        /// 指向前一个状态
        /// </summary>
        public StateBase<T> PrevioudState { get { return mpPreviousState; } }


        // 指向该状态机目前所指向的实例
        private T mpOwner;

        // 当前实例状态
        private StateBase<T> mpCurrentState;

        // 记录实例的上一个状态
        private StateBase<T> mpPreviousState;

        // 每次FSM（有限状态机）被更新时调用该状态逻辑
        private StateBase<T> mpGlobalState;
        public StateMachine(T owner)
        {
            mpOwner = owner;
            mpCurrentState = null;
            mpPreviousState = null;
            mpGlobalState = null;
        }

        /// <summary>
        /// 进入状态
        /// </summary>
        public void GlobalStateEnter()
        {
            mpGlobalState.OnEnter(mpOwner);
        }

        /// <summary>
        /// 为实体设置全局状态
        /// </summary>
        /// <param name="globalState">实例化之后的某个状态</param>
        public void SetGlobalState(StateBase<T> globalState)
        {
            mpGlobalState = globalState;
            mpGlobalState.Target = mpOwner;
            mpGlobalState.OnEnter(mpOwner);
        }

        /// <summary>
        /// 为实体设置目前的状态
        /// </summary>
        /// <param name="currentState">实例化之后的某个状态</param>
        public void SetCurrentState(StateBase<T> currentState)
        {
            mpCurrentState = currentState;
            mpCurrentState.Target = mpOwner;
            mpCurrentState.OnEnter(mpOwner);
        }

        /// <summary>
        /// 调用该函数来实现当前状态中的行为
        /// </summary>
        public void FSMUpdate()
        {
            // 如果存在全局状态，则需要调用它的Execute方法
            if (mpGlobalState != null)
                mpGlobalState.Execute(mpOwner);
            if (mpCurrentState != null)
                mpCurrentState.Execute(mpOwner);
        }

        /// <summary>
        /// 改变某个实体的状态
        /// </summary>
        /// <param name="pNewState">新的状态</param>
        public void ChangeState(StateBase<T> pNewState)
        {
            if (pNewState == null)
            {
                Debug.LogError("该状态不存在");
            }
            mpCurrentState.OnExit(mpOwner);
            // 保留前一个状态的记录
            mpPreviousState = mpCurrentState;

            mpCurrentState = pNewState;
            mpCurrentState.Target = mpOwner;
            mpCurrentState.OnEnter(mpOwner);
        }

        /// <summary>
        /// 使当前实体返回上一个状态
        /// </summary>
        public void RevertToPreviousState()
        {
            ChangeState(mpPreviousState);
        }

    }
}
