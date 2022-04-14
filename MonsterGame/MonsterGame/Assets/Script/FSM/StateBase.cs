using UnityEngine;

namespace Assets.Script.FSM
{
    /// <summary>
    /// 有限状态机基类，且每个不同的状态都只能有一个实例
    /// </summary>
    public abstract class StateBase<T>
    {
        public T Target;
        /// <summary>
        /// 进入该状态之后执行的动作
        /// </summary>
        /// <param name="entity">该状态作用的实体</param>
        public virtual void OnEnter(T entity) { }

        /// <summary>
        /// 在该状态当中时一直执行的动作
        /// </summary>
        /// <param name="entity">该状态作用的实体</param>
        public virtual void Execute(T entity) { }

        /// <summary>
        /// 退出该状态时执行的动作
        /// </summary>
        /// <param name="entity">该状态作用的实体</param>
        public virtual void OnExit(T entity) { }

        /// <summary>
        /// 检查状态
        /// </summary>
        /// <param name="entity">该状态作用的实体</param>
        public virtual void OnCheck(T entity) { }
    }
}
