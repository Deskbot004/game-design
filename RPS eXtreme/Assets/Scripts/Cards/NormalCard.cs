using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class NormalCard : Card
{
    private List<Card> supportCards;

    public override bool IsBasic()
    {
        return true;
    }

    public override void SetSprite()
    {
        // Set Text
        string upperCaseSymbol = "";
        if (symbol.Length > 0)
            upperCaseSymbol = string.Concat(symbol[0].ToString().ToUpper(), symbol.Substring(1));
        transform.Find("Title Text").GetComponent<TMP_Text>().text = upperCaseSymbol;

        // Set Window Icon
        if (GetCardSprites().symbolSprites.ContainsKey(symbol))
            transform.Find("Symbol").GetComponent<SpriteRenderer>().sprite = GetCardSprites().symbolSprites[symbol];

        // Set slots
        transform.Find("Upper Effect").gameObject.SetActive(topSlot);
        transform.Find("Lower Effect").gameObject.SetActive(bottomSlot);
        
    }
}
