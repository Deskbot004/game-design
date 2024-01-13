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
    public List<Action> functions = new List<Action>();
    private int[] viableSlotTypes = { 0, 1 }; // 0: fits in top slot, 1: fits in bottom slot

    public List<Action> GetFunctions() {
        return functions;
    }

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

    public bool GetAttachmentStatus()
    {
        return this.isAttached;
    }

    public int SetAttachmentStatus(bool status)
    {
        this.isAttached = status;
        return 0;
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
