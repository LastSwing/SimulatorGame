using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    public int ID { set; get; }

    public virtual void Enter() { }
    public virtual void Execute() { }
    public virtual void Exit() { }
}
