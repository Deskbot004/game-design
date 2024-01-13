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
        base.SetSprite();

        // Set Text
        string upperCaseSymbol = "";
        if (GetSymbol().Length > 0) 
        { 
            upperCaseSymbol = string.Concat(GetSymbol()[0].ToString().ToUpper(), GetSymbol().Substring(1));
        }
        transform.Find("Title Text").GetComponent<TMP_Text>().text = upperCaseSymbol;

        // Set Window Icon
        //Debug.Log(symbol);
        if (GetCardSprites().supportWindowSprites.ContainsKey(GetSymbol()))
            transform.Find("Symbol").GetComponent<SpriteRenderer>().sprite = GetCardSprites().supportWindowSprites[GetSymbol()];

        // Set slots
        transform.Find("Upper Effect").gameObject.SetActive(GetSlotType() == 0);
        transform.Find("Upper Effect/Text").GetComponent<TMP_Text>().text = description;
        transform.Find("Lower Effect").gameObject.SetActive(GetSlotType() == 1);
        transform.Find("Lower Effect/Text").GetComponent<TMP_Text>().text = description;

        // Set hexagon icons
        if (GetCardSprites().supportIconSprites.ContainsKey(GetSymbol()))
        {
            transform.Find("Upper Effect/Icon").GetComponent<SpriteRenderer>().sprite = GetCardSprites().supportIconSprites[GetSymbol()];
            transform.Find("Lower Effect/Icon").GetComponent<SpriteRenderer>().sprite = GetCardSprites().supportIconSprites[GetSymbol()];
        }
            

    }
}
