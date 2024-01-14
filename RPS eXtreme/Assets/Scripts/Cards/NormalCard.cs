using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.Rendering;

[Serializable]
public class NormalCard : Card, Droppable
{
    private Dictionary<string, SupportCard> supportCards = new Dictionary<string, SupportCard>();
    private int[] viableSlotTypes = { 0, 1, 2 , 3 }; // 0: has no slots, 1: has a top slot, 2: has a bottom slot; 3: has both
    private bool dropActive = false;

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
        // TODO: Quick Error fix. For some reason, the supportCards dict isn't initated properly -Julia
        SetSlotType(slotType);

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

    public bool hasSlot(string slot)
    {
        switch(slot) 
        {
            case "top":
                return GetSlotType() == 1 || GetSlotType() == 3;
            case "bottom":
                return GetSlotType() == 2 || GetSlotType() == 3;
            default:
                return false;
        }
    }

    public override void SetSprite()
    {
        base.SetSprite();

        // Set Text
        string upperCaseSymbol = "";
        if (GetSymbol().Length > 0)
            upperCaseSymbol = string.Concat(GetSymbol()[0].ToString().ToUpper(), GetSymbol().Substring(1));
        transform.Find("Title Text").GetComponent<TMP_Text>().text = upperCaseSymbol;

        // Set Window Icon
        if (GetCardSprites().symbolSprites.ContainsKey(GetSymbol()))
            transform.Find("Symbol").GetComponent<SpriteRenderer>().sprite = GetCardSprites().symbolSprites[GetSymbol()];

        // Set slots
        transform.Find("Upper Effect").gameObject.SetActive(hasSlot("top"));
        transform.Find("Lower Effect").gameObject.SetActive(hasSlot("bottom"));
    }

    public override void OnRightClickInHand()
    {
        base.OnRightClickInHand();
        deck.GetTablePlayer().startAttach(this);
    }


    public bool DropActive
    {
        get {return dropActive;}
        set 
        {
            dropActive = value;
            if(value)
            {
                gameObject.layer = LayerMask.NameToLayer("Droppable");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    public bool OnDrop(Draggable draggedObject)
    {
        if (AttachSupportCard(draggedObject.GetComponent<SupportCard>()) == 0)
        {
            draggedObject.transform.SetParent(transform);
            draggedObject.transform.localPosition = new Vector3 (0, 0, 0.5f);
            draggedObject.transform.eulerAngles = new Vector3 (0, 0, 0);
            draggedObject.GetComponent<SortingGroup>().sortingLayerName = "Cards Background";
            return true;
        }
        return false;
    }

    public void OnLeave(Draggable draggedObject)
    {
        //card.Detach()
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
