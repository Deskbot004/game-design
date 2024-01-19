using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[Serializable]
public class SupportCard : Card
{
    public string description;
    private bool isAttached;
    public List<string> functionNames; // The entries should match one of the patterns nameofFunction:functionValue or nameOfFunction.
    private List<object> functionValues = new List<object>();
    private List<Action<Gamelogic, object>> functions = new List<Action<Gamelogic, object>>(); //Hat aktuell immer nur eine Funktion
    private int[] viableSlotTypes = { 0, 1 }; // 0: fits in top slot, 1: fits in bottom slot
    private LibAR libAR;
    private LibBR libBR;

    // ---------- Main Functions ------------------------------------------------------------------------------

    public void Awake()
    {
        this.libAR = this.GetComponent<LibAR>();
        this.libBR = this.GetComponent<LibBR>();
    }

    // ---------- Getter & Setter ------------------------------------------------------------------------------

    public List<Action<Gamelogic, object>> GetFunctions(){return functions;}

    /*
     * Check each entry of functionNames for a matching pattern and assign entries to functionValues and functions according to the pattern.
     */

    private void SetFunctions()
    {
        foreach(string function in this.functionNames)
        {
            string[] values = function.Split(':');
            int length = values.Length;
            if(this.gamelogic.stringToFunc.ContainsKey(values[0]))
            {
                this.functions.Add(this.gamelogic.stringToFunc[values[0]]);
            }
            switch (length)
            {
                case 2:
                    this.functionValues.Add(values[1]);
                    break;
                case 1:
                    this.functionValues.Add(null);
                    break;
                default:
                    Debug.Log("FunctionName " + function + " did not have one of the required patterns nameOfFunction:functionValue or nameOfFunction");
                    break;
            }
        }
    }

    /*
     * Checks, if the type is a viable slotType and sets the type, if it is.
     */

    public override int SetSlotType(int type)
    {
        if (Array.Exists(viableSlotTypes, element => element == type))
        {
            this.slotType = type;
            return 0;
        }
        else
        {
            Debug.Log("SupportCard got invalid slottype!");
            return -1;
        }
    }

    public List<string> GetFunctionNames(){return this.functionNames;}

    public int SetFunctionNames(List<string> names)
    {
        this.functionNames = names;
        return 0;
    }

    public bool GetAttachmentStatus(){return this.isAttached;}

    public int SetAttachmentStatus(bool status)
    {
        this.isAttached = status;
        return 0;
    }


    public override void SetSprite()
    {
        base.SetSprite();

        // Set Text
        string upperCaseSymbol = "";
        if (this.GetSymbol().Length > 0) 
        { 
            upperCaseSymbol = string.Concat(GetSymbol()[0].ToString().ToUpper(), GetSymbol().Substring(1));
        }
        this.transform.Find("Card Sprites/Title Text").GetComponent<TMP_Text>().text = upperCaseSymbol;

        // Set Window Icon
        //Debug.Log(symbol);
        if (this.GetCardSprites().supportWindowSprites.ContainsKey(GetSymbol()))
            this.transform.Find("Card Sprites/Symbol").GetComponent<SpriteRenderer>().sprite = GetCardSprites().supportWindowSprites[GetSymbol()];

        // Set slots
        this.transform.Find("Card Sprites/Upper Effect").gameObject.SetActive(GetSlotType() == 0);
        this.transform.Find("Card Sprites/Upper Effect/Text").GetComponent<TMP_Text>().text = description;
        this.transform.Find("Card Sprites/Lower Effect").gameObject.SetActive(GetSlotType() == 1);
        this.transform.Find("Card Sprites/Lower Effect/Text").GetComponent<TMP_Text>().text = description;

        // Set hexagon icons
        if (this.GetCardSprites().supportIconSprites.ContainsKey(GetSymbol()))
        {
            this.transform.Find("Card Sprites/Upper Effect/Icon").GetComponent<SpriteRenderer>().sprite = GetCardSprites().supportIconSprites[GetSymbol()];
            this.transform.Find("Card Sprites/Lower Effect/Icon").GetComponent<SpriteRenderer>().sprite = GetCardSprites().supportIconSprites[GetSymbol()];
        }
    }
}
