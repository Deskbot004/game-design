using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibAR : MonoBehaviour
{
    void RunAllAR(List<Func<Gamelogic, String, Card, int>> actions, Gamelogic logic, String caller, Card played)
    {
        foreach (Func<Gamelogic, String, Card, int> func in actions)
        {
            func(logic, caller, played);
        }
    }

    public void RunTest(List<Action> actions)
    {
        foreach (Action action in actions)
        {
            action();
        }
    }


    /* Function to notify the table if the game ended.
     * 
     * Input: 
     * Output: none
     */
    public int AdditionalDamage(Gamelogic logic, String caller, Card played)
    {
        return 1;
    }

    public int DrawCards(Gamelogic logic, String caller, Card played)
    {
        return 1;
    }

    public void Test()
    {
        Debug.Log("Jep this works!");
    }
}
