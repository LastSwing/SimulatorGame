﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : SingletonMonoBehaviour<AIManager>
{
    /// <summary>
    /// AI行为
    /// </summary>
    /// <param name="aiLevel">ai强度</param>
    public void AIDo(int aiLevel)
    {
        switch (aiLevel)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
                (UIManager.instance.GetView("GameView") as GameView).AiAtk();
                break;
        }
    }
}
