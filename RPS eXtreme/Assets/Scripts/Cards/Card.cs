using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Card : MonoBehaviour
{
    private string symbol;
    private string[] viableStrings = { "scissors", "stone", "paper", "lizard", "spock", "support" };


    public int GetValue()
    {
        return 0;
    }

    /*
     * Checks, if the card is a SupportCard or a NormalCard. false = Support, true = Normal.
     */

    public virtual bool IsBasic()
    {
        return false;
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
