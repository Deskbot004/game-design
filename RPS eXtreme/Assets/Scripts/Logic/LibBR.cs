using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibBR : MonoBehaviour
{
    void RunAllBR(List<Action<Gamelogic, String, object>> actions, Gamelogic logic, String caller, object input)
    {
        foreach (Action<Gamelogic, String, object> func in actions)
        {
            func(logic, caller, input);
        }
    }

    public void AdditionalWin(Gamelogic logic, String caller, object value)
    {

        logic.stringToFunc["Win"] = ResetMatrix;
        var mat = logic.GetwinMatrix();
        logic.stringToInput["Win"] = mat;
        var win = Convert.ToString(value);
        /* 
         * { -1, 1, 0, 1, 0 },
         * { 0, -1, 1, 1, 0 },
         * { 1, 0, -1, 0, 1 },
         * { 0, 0, 1, -1, 1 },
         * { 1, 1, 0, 0, -1 }
         * 
         * [User, Enemy]
         * 
         * symbolToEntry.Add("scissors", 0);
         * symbolToEntry.Add("stone", 1);
         * symbolToEntry.Add("paper", 2);
         * symbolToEntry.Add("spock", 3);
         * symbolToEntry.Add("lizard", 4);
        */
        var entry = -1;
        switch (win)
        {
            case "scissors":
                entry = 0;
                break;
            case "stone":
                entry = 1;
                break;
            case "paper":
                entry = 2;
                break;
            case "spock":
                entry = 3;
                break;
            case "lizard":
                entry = 4;
                break;
            default:
                Debug.Log("Error: No additional win mentioned");
                break;
        }
        for (int i = 0; i < mat.GetLength(0); i++)
        {
            mat[i, 0] = 1;
        }
        logic.SetwinMatrix(mat);
    }

    public void WinDraw(Gamelogic logic, String caller, object value)
    {

        logic.stringToFunc["Win"] = ResetMatrix;
        var mat = logic.GetwinMatrix();
        logic.stringToInput["Win"] = mat;
        for (int i = 0; i < mat.GetLength(0); i++)
        {
            mat[i, i] = 1;
        }
        logic.SetwinMatrix(mat);
    }

    public void ResetMatrix(Gamelogic logic, object value)
    {
        var resetT = Convert.ToInt32(value);
        logic.SetdmgOnLoss(resetT);
    }
}
