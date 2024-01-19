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

    /*
    The caller wins against a card type specified by value.
    The reset gets thrown into stringToFunc and stringToInput from Gamelogic titled "Win".

    input: Gamelogic: the overarching Gamelogic
           caller: the player who used the card
           value: the card type against which is won
    */
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

    /*
    The caller wins all draws.
    The reset gets thrown into stringToFunc and stringToInput from Gamelogic titled "WinD".
    input: Gamelogic: the overarching Gamelogic
           caller: the player who used the card
           value: irrelevant
    */
    public void WinDraw(Gamelogic logic, string caller, object value)
    {
        logic.stringToFunc["WinD"] = ResetMatrix;
        var mat = logic.GetwinMatrix();
        logic.stringToInput["WinD"] = mat;
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

    /*
    Resets the WinMatrix.

    input: Gamelogic: the overarching Gamelogic
           value: the old winMatrix
    */
    public void ResetMatrix(Gamelogic logic, object value)
    {
        int[,] resetT = (int[,])value;

        logic.SetwinMatrix(resetT);
    }
}


// unused / untested ------------------------------------------------------------------------------------------------------
    /*
    The caller loses against a card type specified by value.
    The reset gets thrown into stringToFunc and stringToInput from Gamelogic titled "Loss".

    input: Gamelogic: the overarching Gamelogic
           caller: the player who used the card
           value: the card type against which is lost
    */
    public void AdditionalLoss(Gamelogic logic, string caller, object value)
    {
        logic.stringToFunc["Loss"] = ResetMatrix;
        var mat = logic.GetwinMatrix();
        logic.stringToInput["Loss"] = mat.Clone();
        var win = Convert.ToString(value);
        int winner = -1;
        if(caller == "user")
        {
            winner = 1;
        }
        else
        {
            winner = 0;
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
            mat[entry, i] = winner;
        }
        logic.SetwinMatrix(mat);
    }

    /*
    The caller loses all draws.
    The reset gets thrown into stringToFunc and stringToInput from Gamelogic titled "LossD".
    input: Gamelogic: the overarching Gamelogic
           caller: the player who used the card
           value: irrelevant
    */
    public void LossDraw(Gamelogic logic, string caller, object value)
    {
        logic.stringToFunc["LossD"] = ResetMatrix;
        var mat = logic.GetwinMatrix();
        logic.stringToInput["LossD"] = mat;
        int winner = -1;
        if (caller == "user")
        {
            winner = 1;
        }
        else
        {
            winner = 0;
        }
        for (int i = 0; i < mat.GetLength(0); i++)
        {
            mat[i, i] = winner;
        }
        logic.SetwinMatrix(mat);
    }