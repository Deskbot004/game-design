using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibAR : MonoBehaviour
{
    public void RunAllAR(List<(Action<Gamelogic, string, object>,object)> actions, Gamelogic logic, string caller)
    {
        Debug.Log("running AR for " + caller);
        foreach (var entry in actions)
        {
            Debug.Log("Calling a function for " + caller);
            Action<Gamelogic, String, object> func = entry.Item1;
            object input = entry.Item2;
            func(logic, caller, input);
        }
    }


    /* 
     * Multiply damage dealt.
     */
    public void AdditionalDamage(Gamelogic logic, string caller, object value)
    {
        logic.stringToFunc["Dmg"] = ResetDamage;
        logic.stringToInput["Dmg"] = logic.GetdmgOnLoss();
        var damage = Convert.ToInt32(value);
        logic.SetdmgOnLoss(damage);
    }

    public void ResetDamage(Gamelogic logic, object value)
    {
        var resetT = Convert.ToInt32(value);
        logic.SetdmgOnLoss(resetT);
    }

    /* 
     * Draw additional cards.
     */
    public void DrawCards(Gamelogic logic, string caller, object value)
    {
        
        int amount = Convert.ToInt32(value);
        if (String.Equals(caller, "user"))
        {
            logic.UserDraw(amount);
        }
        if (String.Equals(caller, "enemy"))
        {
            logic.EnemyDraw(amount);
        }
    }

    /* 
     * Draw additional cards.
     */
    public void Lifesteal(Gamelogic logic, string caller, object value)
    {
        var amount = Convert.ToInt32(value);
        int damage = logic.GetdmgOnLoss() * amount;
        if (String.Equals(caller, "user"))
        {
            logic.DamageUser((-1)*amount);
        }
        if (String.Equals(caller, "enemy"))
        {
            logic.DamageEnemy((-1) * amount);
        }
    }
}
