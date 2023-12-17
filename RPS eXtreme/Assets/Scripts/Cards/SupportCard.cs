using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class SupportCard : Card
{
    public string description;
    public bool isAttached;
    public List<Action> functions;

    public List<Action> GetFunctions() {
        return functions;
    }

    public override void SetSprite()
    {
        // Set Text
        string upperCaseSymbol = "";
        if (symbol.Length > 0) 
        { 
            upperCaseSymbol = string.Concat(symbol[0].ToString().ToUpper(), symbol.Substring(1));
        }
        transform.Find("Title Text").GetComponent<TMP_Text>().text = upperCaseSymbol;

        // Set Window Icon
        //Debug.Log(symbol);
        if (GetCardSprites().supportWindowSprites.ContainsKey(symbol))
            transform.Find("Symbol").GetComponent<SpriteRenderer>().sprite = GetCardSprites().supportWindowSprites[symbol];

        // Set slots
        transform.Find("Upper Effect").gameObject.SetActive(topSlot);
        transform.Find("Upper Effect/Text").GetComponent<TMP_Text>().text = description;
        transform.Find("Lower Effect").gameObject.SetActive(bottomSlot);
        transform.Find("Lower Effect/Text").GetComponent<TMP_Text>().text = description;

        // Set hexagon icons
        if (GetCardSprites().supportIconSprites.ContainsKey(symbol))
        {
            transform.Find("Upper Effect/Icon").GetComponent<SpriteRenderer>().sprite = GetCardSprites().supportIconSprites[symbol];
            transform.Find("Lower Effect/Icon").GetComponent<SpriteRenderer>().sprite = GetCardSprites().supportIconSprites[symbol];
        }
            

    }
}
