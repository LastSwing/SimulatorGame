using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : SingletonMonoBehaviour<AnimationManager>
{
    /// <summary>
    /// 动作/特效名，持续时间
    /// </summary>
    Dictionary<string, float> animationDic = new Dictionary<string, float>()
    {
        { "RoundChange",1.1f},
        { "CardMove",1.2f},
    };

    /// <summary>
    /// 执行动作/特效
    /// </summary>
    /// <param name="name">动作/特效名</param>
    /// <param name="arg">参数集</param>
    /// <returns>执行时间</returns>
    public float DoAnimation(string name, object[] arg)
    {
        if (animationDic.ContainsKey(name))
        {
            switch (name)
            {
                case "RoundChange":
                    break;
                case "CardMove":
                    break;
            }
            return animationDic[name];
        }
        return 0;
    }
}
