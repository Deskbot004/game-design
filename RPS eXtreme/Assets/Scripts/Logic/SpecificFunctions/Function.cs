using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using UnityEngine;

// Everytime you add a Function, add an enum here and also in SupportLibrary -> FunctionID
//TODO
public enum FunctionType {
    DRAW,
    ADDDMG,
    LIFESTEAL,
    WINDRAW,
    WINAGAINST,
}

public class Function
{
    public DictKeys caller;
    public string description;

    public virtual Function Copy() {
        return null;
    }

    public virtual void Init(DictKeys caller, string stringParam) {
        Debug.Log("Function didn't override Init! " + this);
    }

    public virtual void DoEffect(Gamelogic logic) {
        Debug.Log("Function didn't override DoEffect! " + this);
    }

    public virtual void ResetEffect(Gamelogic logic) {
        Debug.Log("Function didn't override ResetEffect! " + this);
    }
}