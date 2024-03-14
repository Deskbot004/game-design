using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using UnityEngine;

public class ARFunction : Function
{

}

public class AdditionalDamage : ARFunction 
{
    int amount;
    int oldDamage;

    public override Function Copy() {
        return new AdditionalDamage();
    }

    public override void Init(DictKeys caller, string amount) {
        this.caller = caller;
        this.amount = Int32.Parse(amount);
    }

    public override void DoEffect(Gamelogic logic) {
        int oldDamage = logic.GetdmgOnLoss();
        logic.SetdmgOnLoss(amount);
    }

    public override void ResetEffect(Gamelogic logic) {
        logic.SetdmgOnLoss(oldDamage);
    }
}

public class Lifesteal : ARFunction 
{
    float amount;

    public override void Init(DictKeys caller, string amount){
        this.caller = caller;
        this.amount = float.Parse(amount);
    }

    public override Function Copy() {
        return new Lifesteal();
    }

    public override void DoEffect(Gamelogic logic) {
        int damage = (int) Math.Ceiling(amount * logic.GetdmgOnLoss())*(-1);
        logic.Damage(damage, caller);
    }

    public override void ResetEffect(Gamelogic logic) {
        return;
    }
}