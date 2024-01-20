using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[Serializable]
public class SupportCard : Card
{
    public string effectType;
    public string effectValue;
    public string description;

    private bool isAttached;
    public List<string> functionNames; // The entries should match one of the patterns library:nameofFunction:functionValue or library:nameOfFunction.
    private List<(Action<Gamelogic, string, object>, object)> ARFunctions = new List<(Action<Gamelogic, string, object>,object)>();
    private List<(Action<Gamelogic, string, object>, object)> BRFunctions = new List<(Action<Gamelogic, string, object>, object)>();
    private List<(Action<Gamelogic, string, object>, object)> drawFunctions = new List<(Action<Gamelogic, string, object>, object)>();
    private int[] viableSlotTypes = { 0, 1 }; // 0: fits in top slot, 1: fits in bottom slot
    private LibAR libAR;
    private LibBR libBR;
    private Dictionary<string, Action<Gamelogic, string, object>> ARLibrary;
    private Dictionary<string, Action<Gamelogic, string, object>> BRLibrary;
    private Dictionary<string, Action<Gamelogic, string, object>> drawLibrary;

    // ---------- Main Functions ------------------------------------------------------------------------------

    public void Awake()
    {
        this.populateDictionaries();
    }

    // ---------- Getter & Setter ------------------------------------------------------------------------------

    public override List<(Action<Gamelogic, string, object>, object)> GetFunctionsAR(){return ARFunctions;}

    public override List<(Action<Gamelogic, string, object>, object)> GetFunctionsBR() { return BRFunctions; }

    public override List<(Action<Gamelogic, string, object>, object)> GetFunctionsDraw() { return drawFunctions; }


    /*
     * Check each entry of functionNames for a matching pattern and assign entries to functionValues and functions according to the pattern.
     */

    private void SetFunctions()
    {
        foreach(string function in this.functionNames)
        {
            string[] values = function.Split(':');
            int length = values.Length;
            switch (values[0]) //Check in which library the function should be in
            {
                case "AR":
                    if (this.ARLibrary.ContainsKey(values[1])) //Check if the library has a function of that name
                    {
                        switch (length)
                        {
                            case 3:
                                this.ARFunctions.Add((this.ARLibrary[values[1]], values[2]));
                                this.SetEffectType(values[1], values[2]);
                                break;
                            case 2:
                                this.ARFunctions.Add((this.ARLibrary[values[1]], null));
                                this.SetEffectType(values[1], null);
                                break;
                            default:
                                Debug.Log("FunctionName " + function + " did not have one of the required patterns nameOfFunction:functionValue or nameOfFunction");
                                break;
                        }
                    }
                    else
                    {
                        Debug.Log("Function with name " + values[1] + " does not exist in library AR");
                    }
                    break;
                case "BR":
                    if (this.BRLibrary.ContainsKey(values[1]))//Check if the library has a function of that name
                    {
                        switch (length)
                        {
                            case 3:
                                this.BRFunctions.Add((this.BRLibrary[values[1]], values[2]));
                                this.SetEffectType(values[1], values[2]);
                                break;
                            case 2:
                                this.BRFunctions.Add((this.BRLibrary[values[1]], null));
                                this.SetEffectType(values[1], null);
                                break;
                            default:
                                Debug.Log("FunctionName " + function + " did not have one of the required patterns nameOfFunction:functionValue or nameOfFunction");
                                break;
                        }
                    }
                    else
                    {
                        Debug.Log("Function with name " + values[1] + " does not exist in library BR");
                    }
                    break;
                case "draw":
                    if (this.drawLibrary.ContainsKey(values[1]))//Check if the library has a function of that name
                    {
                        switch (length)
                        {
                            case 3:
                                this.drawFunctions.Add((this.drawLibrary[values[1]], values[2]));
                                this.SetEffectType(values[1], values[2]);
                                break;
                            case 2:
                                this.drawFunctions.Add((this.drawLibrary[values[1]], null));
                                this.SetEffectType(values[1], null);
                                break;
                            default:
                                Debug.Log("FunctionName " + function + " did not have one of the required patterns nameOfFunction:functionValue or nameOfFunction");
                                break;
                        }
                    }
                    else
                    {
                        Debug.Log("Function with name " + values[1] + " does not exist in library Draw");
                    }
                    break;
                default:
                    Debug.Log("Library of name " + values[0] + " does not exist");
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
        this.SetFunctions();
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
        SetEffectType(effectType, effectValue);

        // Set Text
        string upperCaseSymbol = "";
        if (effectType.Length > 0) 
        { 
            upperCaseSymbol = string.Concat(effectType[0].ToString().ToUpper(), effectType.Substring(1));
        }
        this.transform.Find("Card Sprites/Title Text").GetComponent<TMP_Text>().text = upperCaseSymbol;

        // Set Window Icon
        if (this.GetCardSprites().supportWindowSprites.ContainsKey(effectType))
            this.transform.Find("Card Sprites/Symbol").GetComponent<SpriteRenderer>().sprite = GetCardSprites().supportWindowSprites[effectType];

        // Set slots
        this.transform.Find("Card Sprites/Upper Effect").gameObject.SetActive(GetSlotType() == 0);
        this.transform.Find("Card Sprites/Upper Effect/Text").GetComponent<TMP_Text>().text = description;
        this.transform.Find("Card Sprites/Lower Effect").gameObject.SetActive(GetSlotType() == 1);
        this.transform.Find("Card Sprites/Lower Effect/Text").GetComponent<TMP_Text>().text = description;

        // Set hexagon icons
        if (this.GetCardSprites().supportIconSprites.ContainsKey(effectType))
        {
            // TODO: Change supportWindowSprites to supportIconSprites
            this.transform.Find("Card Sprites/Upper Effect/Icon").GetComponent<SpriteRenderer>().sprite = GetCardSprites().supportWindowSprites[effectType];
            this.transform.Find("Card Sprites/Lower Effect/Icon").GetComponent<SpriteRenderer>().sprite = GetCardSprites().supportWindowSprites[effectType];
        }
    }

    private void SetEffectType(string functionName, string value)
    {
        effectType = functionName;
        effectValue = value;

        switch (functionName)
        {
            case "draw":
                this.description = "Draw " + value;
                break;

            case "extra damage":
                this.description = "Do  " + value + " extra damage on win";
                break;
            case "lifesteal":
                this.description = "Lifesteal " + value;
                break;
            case "win on draw":
                this.description = "Win on Draw";
                break;
            case "win against":
                this.description = "Win against " + value;
                break;
            default:
                break;
        }
    }

    // ---------- Dictionaries ------------------------------------------------------------------------------

    public void populateDictionaries()
    {
        this.libAR = this.GetComponent<LibAR>();
        this.libBR = this.GetComponent<LibBR>();

        this.drawLibrary = new Dictionary<string, Action<Gamelogic, string, object>>();
        this.ARLibrary = new Dictionary<string, Action<Gamelogic, string, object>>();
        this.BRLibrary = new Dictionary<string, Action<Gamelogic, string, object>>();

        //draw functions
        this.drawLibrary["draw"] = this.libAR.DrawCards;

        //After resolution functions
        this.ARLibrary["extra damage"] = this.libAR.AdditionalDamage;
        this.ARLibrary["lifesteal"] = this.libAR.Lifesteal;

        //Before resolution functions
        this.BRLibrary["win on draw"] = this.libBR.WinDraw;
        this.BRLibrary["win against"] = this.libBR.AdditionalWin;
    }
}
