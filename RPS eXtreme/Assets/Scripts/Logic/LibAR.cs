using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibAR : MonoBehaviour
{
    public void RunAllAR(List<(Action<Gamelogic, String, object>,object)> actions, Gamelogic logic, String caller)
    {
        foreach (var entry in actions)
        {
            Action<Gamelogic, String, object> func = entry.Item1;
            object input = entry.Item2;
            func(logic, caller, input);
        }
    }

    public void RunTest(List<Action<Gamelogic, String>> actions, Gamelogic logic)
    {
        foreach (Action<Gamelogic, String> action in actions)
        {
            action(logic, "test");
        }
    }


    /* 
     * Multiply damage dealt.
     */
    public void AdditionalDamage(Gamelogic logic, String caller, object value)
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
    public void DrawCards(Gamelogic logic, String caller, object value)
    {
        var amount = Convert.ToInt32(value);
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
    public void Lifesteal(Gamelogic logic, String caller, object value)
    {
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

    public void Test(Gamelogic logic, String caller)
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
