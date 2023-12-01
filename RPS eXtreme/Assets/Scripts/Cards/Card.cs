using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Card : MonoBehaviour
{
    public string symbol;
    private string[] viableStrings = { "scissors", "stone", "paper", "lizard", "spock" };


    public int GetValue()
    {
        return 0;
    }

    public virtual bool IsBasic()
    {
        return true;
    }

    public string GetSymbol()
    {
        return this.symbol;
    }

    public int SetSymbol(string newSymbol)
    {
        if(Array.Exists(viableStrings,element => element == newSymbol))
        {
            this.symbol = newSymbol;
            return 0;
        }
        else
        {
            Debug.Log("The given symbol is not a viable symbol");
            return -1;
        }
    }
}
