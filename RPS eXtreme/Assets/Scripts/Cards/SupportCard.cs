using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Reflection;
using System.Threading;

public class SupportCard : Card
{
    [Header("Support Card Specific")] //This is for easier creation in the Editor and shouldn't be used by scripts
    public FunctionID editor_functionID;
    public List<string> editor_functionParams;

    private FunctionInfo functionInfo;

    /*
    [Header("Support Card Specific")] //TODO: Create easier way to create Support cards in Editor
    [UDictionary.Split(30, 70)] public FunctionDict functionList = new();
    [Serializable] public class FunctionDict : UDictionary<FunctionType, string> { } // Dict Definition
    */
    

    #region Main Functions ----------------------------------------------------------------------------------
    public override void SetSprite() {
        base.SetSprite();

        // Init
        SupportCardSprites cardSprites = transform.GetComponent<SupportCardSprites>();
        Debug.Assert(cardSprites.supportWindowSprites.ContainsKey(functionInfo.functionID), "supportWindowSprites is missing key " + functionInfo.functionID.ToString(), this);
        //Debug.Assert(cardSprites.supportIconSprites.ContainsKey(functionInfo.functionID), "supportIconSprites is missing key " + functionInfo.functionID.ToString(), this);

        // Title
        try {
            this.transform.Find("Card Sprites/Title Text").GetComponent<TMP_Text>().text = functionInfo.title;
        } catch {
            this.transform.Find("Card Sprites/Title Text").GetComponent<TMP_Text>().text = "Support Card";
        }

        // Window Icon
        this.transform.Find("Card Sprites/Symbol").GetComponent<SpriteRenderer>().sprite = cardSprites.supportWindowSprites[functionInfo.functionID];

        // Slots + Description
        string description = functionInfo != null? functionInfo.description : "No Effect";
        this.transform.Find("Card Sprites/Upper Effect").gameObject.SetActive(topSlot);
        this.transform.Find("Card Sprites/Upper Effect/Text").GetComponent<TMP_Text>().text = description;
        this.transform.Find("Card Sprites/Lower Effect").gameObject.SetActive(bottomSlot);
        this.transform.Find("Card Sprites/Lower Effect/Text").GetComponent<TMP_Text>().text = description;

        // Hexagon Icons
        // TODO: Change supportWindowSprites to supportIconSprites
        this.transform.Find("Card Sprites/Upper Effect/Icon").GetComponent<SpriteRenderer>().sprite = cardSprites.supportWindowSprites[functionInfo.functionID];
        this.transform.Find("Card Sprites/Lower Effect/Icon").GetComponent<SpriteRenderer>().sprite = cardSprites.supportWindowSprites[functionInfo.functionID];
    }
    #endregion

    #region Shorthands --------------------------------------------------------------------------------------
    public List<Function> GetFunctionsForResolve() {
        return functionInfo.functions;
    }

    public override (FunctionID, List<string>) GetFunctionsForSaving() {
        return (functionInfo.functionID, functionInfo.functionParams);
    }

    public void SetCardValues(CardSymbol symbol, bool topSlot, bool bottomSlot, (FunctionID, List<string>) function, bool isPlayer) {
        functionInfo = SupportLibrary.supportLibrary[function.Item1].CreateInitialized(isPlayer, function.Item2);
        editor_functionID = function.Item1;
        editor_functionParams = function.Item2;
        SetCardValues(symbol, topSlot, bottomSlot);
    }
    #endregion

    [ContextMenu("Init Functions")]
    void Temp() {
        functionInfo = SupportLibrary.supportLibrary[editor_functionID].CreateInitialized(true, editor_functionParams);
    }
}
