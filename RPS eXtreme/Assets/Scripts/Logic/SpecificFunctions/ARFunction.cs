using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using UnityEngine;

public class ARFunction : Function {}

public class AdditionalDamage : ARFunction 
{
    int amount;
    int oldDamage;

    public override void Init(TableSideName caller, string amount) {
        this.caller = caller;
        this.amount = Int32.Parse(amount);
    }

    public override void DoEffect(Gamelogic logic) {
        oldDamage = logic.GetDmgOnLoss();
        logic.SetDmgOnLoss(amount);
    }

    public override void ResetEffect(Gamelogic logic) {
        logic.SetDmgOnLoss(oldDamage);
    }
}

public class Lifesteal : ARFunction 
{
    float amount;

    public override void Init(TableSideName caller, string amount){
        this.caller = caller;
        this.amount = float.Parse(amount);
    }

    public override void DoEffect(Gamelogic logic) {
        int healing = (int) Math.Ceiling(amount * logic.GetDmgOnLoss()); // TODO: Ist das kommutativ? Was ist wenn zuerst Lifesteal und danach Additional Damage aufgerufen wird?
        logic.Damage(-healing, caller);
    }

    public override void ResetEffect(Gamelogic logic) {
        return;
    }
}