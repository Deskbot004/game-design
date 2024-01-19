using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibBR : MonoBehaviour
{
    public void RunAllBR(List<(Action<Gamelogic, string, object>, object)> actions, Gamelogic logic, String caller)
    {
        Debug.Log("running BR for " + caller);
        foreach (var entry in actions)
        {
            Debug.Log("Calling a function for " + caller);
            Action<Gamelogic, String, object> func = entry.Item1;
            object input = entry.Item2;
            func(logic, caller, input);
        }
    }

    public void AdditionalWin(Gamelogic logic, string caller, object value)
    {
        logic.stringToFunc["Win"] = ResetMatrix;
        var mat = logic.GetwinMatrix();
        logic.stringToInput["Win"] = mat.Clone();
        var win = Convert.ToString(value);
        int winner = -1;
        if(caller == "user")
        {
            winner = 0;
        }
        else
        {
            winner = 1;
        }
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
         * symbolToEntry.Add("rock", 1);
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
            case "rock":
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
            mat[i, entry] = winner;
        }
        logic.SetwinMatrix(mat);
    }


    public void WinDraw(Gamelogic logic, string caller, object value)
    {
        logic.stringToFunc["Win"] = ResetMatrix;
        var mat = logic.GetwinMatrix();
        logic.stringToInput["Win"] = mat;
        int winner = -1;
        if (caller == "user")
        {
            winner = 0;
        }
        else
        {
            winner = 1;
        }
        for (int i = 0; i < mat.GetLength(0); i++)
        {
            mat[i, i] = winner;
        }
        logic.SetwinMatrix(mat);
    }

    public void ResetMatrix(Gamelogic logic, object value)
    {
        int[,] resetT = (int[,])value;

        logic.SetwinMatrix(resetT);
    }
}
