using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using UnityEngine;

public class DCFunction : Function { }

public class DrawCards : DCFunction 
{
    int amount;

    public override void Init(TableSideName caller, string stringParam){
        this.caller = caller;
        this.amount = Int32.Parse(stringParam);
    }
    
    public override void DoEffect(Gamelogic logic) {
        logic.DrawCards(amount, caller);
    }

    public override void ResetEffect(Gamelogic logic) {
        return;
    }
}