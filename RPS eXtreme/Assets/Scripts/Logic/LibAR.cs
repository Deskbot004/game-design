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

    public void RunTest(List<Action<Gamelogic, string>> actions, Gamelogic logic)
    {
        foreach (Action<Gamelogic, String> action in actions)
        {
            action(logic, "test");
        }
    }


    /* 
     * Multiply damage dealt.
     */
    public void AdditionalDamage(Gamelogic logic, string caller, object value)
    {
        Debug.Log("Additional Damage from" + caller);
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
        
        var amount = Convert.ToInt32(value);
        Debug.Log("Drawing " + amount + " cards for " + caller);
        if (caller == "player")
        {
            logic.UserDraw(amount);
        }
        if (caller == "enemy")
        {
            logic.EnemyDraw(amount);
        }
    }

    /* 
     * Draw additional cards.
     */
    public void Lifesteal(Gamelogic logic, string caller, object value)
    {
        Debug.Log("Lifesteal from " + caller);
        var amount = Convert.ToInt32(value);
        var damage = logic.GetdmgOnLoss() * amount;
        if (caller == "player")
        {
            logic.DamageUser((-1)*amount);
        }
        if (caller == "enemy")
        {
            logic.DamageEnemy((-1) * amount);
        }
    }

    public void Test(Gamelogic logic, string caller)
    {
        logic.stringToFunc["Test"] = ResetTest;
        logic.stringToInput["Test"] = logic.TestVar;
        logic.TestVar = 9000;
    }

    public void ResetTest(Gamelogic logic, object reset)
    {
        var resetT = Convert.ToInt32(reset);
        logic.TestVar = resetT;
    }
}
