using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibAR : MonoBehaviour
{
    public void RunAllAR(List<(Action<Gamelogic, string, object>,object)> actions, Gamelogic logic, string caller)
    {
        foreach (var entry in actions)
        {
            Action<Gamelogic, String, object> func = entry.Item1;
            object input = entry.Item2;
            func(logic, caller, input);
        }
    }


    /*
    The caller deals the damage which is given.
    The reset gets thrown into stringToFunc and stringToInput from Gamelogic titled "Dmg".
    input: Gamelogic: the overarching Gamelogic
           caller: the player who used the card / and won
           value: the amount of damage which should be dealt
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
    The caller draws as many cards as specified. Special case the user does not need to win!
    input: Gamelogic: the overarching Gamelogic
           caller: the player who used the card
           value: the amount of cards to be drawn
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
    The caller heals for the given damage. (If expanded needs to be called after multiply dmg)
    input: Gamelogic: the overarching Gamelogic
           caller: the player who used the card
           value: the heal multiplier
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
