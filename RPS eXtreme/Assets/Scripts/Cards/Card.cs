using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardNumber;
    public int Class; // 0 Rock 1 Paper 2 Scissors 3 Support
    public Card[] SupportSlots;
    public int nrSlots;
    public string Symbol;

    public void Init(int cardNumber, string Symbol)
    {
        //look up the cardNumber and initialize accordingly
        this.Symbol = Symbol;
    }

    public string GetSymbol()
    {
        return Symbol;
    }

    public bool isBasic()
    {
        return true;
    }

}
