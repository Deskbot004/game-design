using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[Serializable]
public class NormalCard : Card
{
    private Dictionary<string, SupportCard> supportCards = new Dictionary<string, SupportCard>();
    private int[] viableSlotTypes = { 0, 1, 2 , 3 }; // 0: has no slots, 1: has a top slot, 2: has a bottom slot; 3: has both

    public override bool IsBasic()
    {
        return true;
    }

    public override int SetSlotType(int type)
    {
        if(Array.Exists(viableSlotTypes, element => element == type))
        {
            this.slotType = type;
            switch (this.slotType) //Add the keys corresponding to the slottype to the dictionary
            {
                case 3:
                    supportCards["Top"] = null;
                    supportCards["Bottom"] = null;
                    break;
                case 2:
                    supportCards["Bottom"] = null;
                    break;
                case 1:
                    supportCards["Top"] = null;
                    break;
                default: //no keys need to be added to the dictionary
                    break;
                     
            }
            return 0;
        }
        else
        {
            Debug.Log("NormalCard got invalid slottype!");
            return -1;
        }
    }

    //TODO: Check, if there is already a card attached and handle that
    public int AttachSupportCard(SupportCard card)
    {
        int type = card.GetSlotType();
        string slot = "";
        switch (type) //Check the Type of SupportCard, that is to be attached
        {
            case 1:
                slot = "Bottom";
                break;
            case 0:
                slot = "Top";
                break;
            default:
                Debug.Log("SupportCard, to be attached, has invalid type!");
                return -1;
        }
        if (supportCards.ContainsKey(slot))
        {
            supportCards[slot] = card; //TODO: Visual Effects of the attachment
            card.SetAttachmentStatus(card);
            return 0;
        }
        else
        {
            Debug.Log("Attached SupportCard doesn't have a matching type!");
            return 1;
        }
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
