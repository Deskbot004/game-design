using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NormalCard : Card
{
    private List<Card> supportCards;

    public override bool IsBasic()
    {
        return true;
    }
}
