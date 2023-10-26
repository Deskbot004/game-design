using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardNumber;
    public int Class; // 0 Rock 1 Paper 2 Scissors 3 Support
    public Card[] SupportSlots;
    public int nrSlots;

    public void init(int cardNumber)
    {
        //look up the cardNumber and initialize accordingly

    }

    public int getValue()
    {
        return 0;
    }

}
