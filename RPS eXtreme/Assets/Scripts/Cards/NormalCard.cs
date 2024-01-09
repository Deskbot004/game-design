using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NormalCard : Card
{
    private List<Card> supportCards = new List<Card>();

    public override bool IsBasic()
    {
        return true;
    }
}
