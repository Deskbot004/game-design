using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Reflection;
using System.Threading;

public class SupportCard : Card
{
    [UDictionary.Split(30, 70)]
    public FunctionDict functionList;
    [Serializable]
    public class FunctionDict : UDictionary<FunctionID, string> { }
    
    private List<Function> functions = new();
    public FunctionID functionID;
    private CardInfo cardInfo;
    public List<string> stringParam;

    public override List<Function> GetFunctionsForResolve() {
        return functions;
    }

    public void SetCardValues(CardSymbol symbol, bool topSlot, bool bottomSlot, (FunctionID, List<string>) function, bool isPlayer) {
        this.cardInfo = SupportLibrary.supportLibrary[function.Item1];
        this.stringParam = new(function.Item2);
        SetCardValues(symbol, topSlot, bottomSlot);
        SetFunctions(isPlayer);
        SetSprite();
    }

    public void SetFunctions(bool isPlayer) {
        for(int i=0; i<cardInfo.functions.Count; i++) {
            FunctionType functionType = cardInfo.functions[i];
            Function newFunction = SupportLibrary.CreateFunction(functionType, isPlayer, stringParam[i]);
            functions.Add(newFunction);
        }
    }

    public override void SetSprite() {
        base.SetSprite();
        SupportCardSprites cardSprites = transform.GetComponent<SupportCardSprites>();
        Debug.Assert(cardSprites.supportWindowSprites.ContainsKey(functionID), "supportWindowSprites is missing key " + functionID.ToString(), this);
        Debug.Assert(cardSprites.supportIconSprites.ContainsKey(functionID), "supportIconSprites is missing key " + functionID.ToString(), this);

        // Set Title
        try {
            this.transform.Find("Card Sprites/Title Text").GetComponent<TMP_Text>().text = cardInfo.title;
        } catch {
            this.transform.Find("Card Sprites/Title Text").GetComponent<TMP_Text>().text = "Support Card";
        }

        // Set Window Icon
        this.transform.Find("Card Sprites/Symbol").GetComponent<SpriteRenderer>().sprite = cardSprites.supportWindowSprites[functionID];

        // Set slots
        string description = cardInfo != null? cardInfo.CreateDescription(stringParam) : "No Effect";
        this.transform.Find("Card Sprites/Upper Effect").gameObject.SetActive(topSlot);
        this.transform.Find("Card Sprites/Upper Effect/Text").GetComponent<TMP_Text>().text = description;
        this.transform.Find("Card Sprites/Lower Effect").gameObject.SetActive(bottomSlot);
        this.transform.Find("Card Sprites/Lower Effect/Text").GetComponent<TMP_Text>().text = description;

        // Set hexagon icons
        // TODO: Change supportWindowSprites to supportIconSprites
        this.transform.Find("Card Sprites/Upper Effect/Icon").GetComponent<SpriteRenderer>().sprite = cardSprites.supportWindowSprites[functionID];
        this.transform.Find("Card Sprites/Lower Effect/Icon").GetComponent<SpriteRenderer>().sprite = cardSprites.supportWindowSprites[functionID];
    }

    public override (FunctionID, List<string>) GetFunctionsForSave() {
        return (functionID, stringParam);
    }
}
