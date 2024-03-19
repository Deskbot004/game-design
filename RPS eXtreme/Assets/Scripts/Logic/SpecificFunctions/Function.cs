using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using UnityEngine;

public class Function
{
    public TableSideName caller;

    public virtual void Init(TableSideName caller, string stringParam) {
        Debug.Log("Function didn't override Init! " + this);
    }

    public virtual void DoEffect(Gamelogic logic) {
        Debug.Log("Function didn't override DoEffect! " + this);
    }

    public virtual void ResetEffect(Gamelogic logic) {
        Debug.Log("Function didn't override ResetEffect! " + this);
    }
}