using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.Rendering;
using System.Linq;

[Serializable]
public class NormalCard : Card, Droppable
{
    private Dictionary<string, SupportCard> supportCards = new Dictionary<string, SupportCard>();
    
    // 0: has no slots, 1: has a top slot, 2: has a bottom slot; 3: has both
    private int[] viableSlotTypes = { 0, 1, 2 , 3 }; 
    private bool dropActive = false;
    private int priority = (int) DroppablePriorities.CARD;

    // ---------- Main Functions ------------------------------------------------------------------------------

    public void Awake()
    {
        if(this.GetSlotType() != -1)
        {
            this.SetSlotType(this.GetSlotType());
        }

    }

    public override bool IsBasic()
    {
        return true;
    }

    /*
    * Checks, if the type of the SupportCard matches any of the slots of the NormalCard.
    * If that is not the case, the function returns 1.
    * Otherwise it assigns the SupportCard to its matching slot.
    * Any SupportCards already in that slot will be removed.
    */
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

        if (this.supportCards.ContainsKey(slot))
        {
            if (this.supportCards[slot] != null) //detach previously attached card
            {
                //TODO Rework
                /*if (this.DetachSupportCard(this.supportCards[slot], slot) == -1)
                {
                    Debug.Log("Called DetachSupportCard with unknown SlotName in AttachSupportCard");
                }*/
                return 1;
            }
            this.supportCards[slot] = card;
            card.SetAttachmentStatus(true);
            return 0;
        }
        else
        {
            return 1;
        }
    }

    /*
    * Checks, if any of the attached cards in the slot match the given SupportCard.
    * If the slot containing the card is already known, its key can be given in the parameter slotName and this check will be skipped.
    * The SupportCard is then removed from the slot.
    */
    public int DetachSupportCard(SupportCard card, string slotName = "Unknown")
    {
        if (slotName == "Unknown")
        {
            foreach (string slot in this.supportCards.Keys)
            {
                if (this.supportCards[slot] == card)
                {
                    slotName = slot;
                }
            }
        }

        if (!this.supportCards.ContainsKey(slotName))
        {
            Debug.Log("DetachCard called with wrong SlotName");
            return -1;
        }

        this.supportCards[slotName] = null;
        card.SetAttachmentStatus(false);
        return 0;

    }

    public List<SupportCard> DetachAllCards()
    {
        SupportCard[] cardSlots = GetSupportCards().Values.ToArray();
        List<SupportCard> supCards = new();
        foreach(SupportCard supCard in cardSlots)
        {
            if(supCard != null && DetachSupportCard(supCard) == 0)
            {
                supCard.transform.SetParent(supCard.GetDeck().transform);
                supCard.GetComponent<SortingGroup>().sortingLayerName = "Cards in Focus";
                supCards.Add(supCard);
                supCard.GetComponent<Draggable>().enabled = true;
            }
        }
        return supCards;
    }

    public bool hasSlot(string slot)
    {
        switch (slot)
        {
            case "top":
                return this.GetSlotType() == 1 || this.GetSlotType() == 3;
            case "bottom":
                return this.GetSlotType() == 2 || this.GetSlotType() == 3;
            default:
                return false;
        }
    }

    public override void OnRightClickInHand()
    {
        base.OnRightClickInHand();
        if(!deck.GetTablePlayer().isPlayer) return;
        if(this.status == 2)
        {
            foreach (Slot slot in deck.GetTablePlayer().GetSlots())
            {
                if(slot.GetCard() == this)
                {
                    slot.OnLeave(this.GetComponent<Draggable>());
                    SetStatus(1);
                    GetComponent<Draggable>().SetCurrentDroppable(deck.GetTablePlayer());
                }
            }
        }
        this.deck.GetTablePlayer().StartAttach(this);
    }

    public bool HasAttachedCards()
    {
        foreach(SupportCard card in supportCards.Values)
        {
            if(card != null) return true;
        }
        return false;
    }

    // ---------- Getter & Setter ------------------------------------------------------------------------------

    /*
    * Sets the SlotType of the card according to the given integer and adds the corresponding slot-keys into the slot-dictionary
    */

    public override int SetSlotType(int type)
    {
        supportCards.Clear();
        if(Array.Exists(viableSlotTypes, element => element == type))
        {
            this.slotType = type;
            switch (this.slotType) //Add the keys corresponding to the slottype to the dictionary
            {
                case 3:
                    this.supportCards["Top"] = null;
                    this.supportCards["Bottom"] = null;
                    break;
                case 2:
                    this.supportCards["Bottom"] = null;
                    break;
                case 1:
                    this.supportCards["Top"] = null;
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

    public override void SetSprite()
    {
        base.SetSprite();

        // Set Text
        string upperCaseSymbol = "";
        if (GetSymbol().Length > 0)
            upperCaseSymbol = string.Concat(GetSymbol()[0].ToString().ToUpper(), GetSymbol().Substring(1));
        this.transform.Find("Card Sprites/Title Text").GetComponent<TMP_Text>().text = upperCaseSymbol;

        // Set Window Icon
        if (GetCardSprites().symbolSprites.ContainsKey(GetSymbol()))
            this.transform.Find("Card Sprites/Symbol").GetComponent<SpriteRenderer>().sprite = GetCardSprites().symbolSprites[GetSymbol()];

        // Set slots
        transform.Find("Card Sprites/Upper Effect").gameObject.SetActive(hasSlot("top"));
        transform.Find("Card Sprites/Lower Effect").gameObject.SetActive(hasSlot("bottom"));
    }

    public Dictionary<string, SupportCard> GetSupportCards() {return supportCards;}
    public List<SupportCard> GetAttachedSupportCards()
    {
        List<SupportCard> attachedSupport = new List<SupportCard>();
        foreach(SupportCard supCard in supportCards.Values)
        {
            if (supCard != null) attachedSupport.Add(supCard);
        }
        return attachedSupport;
    }

    // ---------- Droppable Functions ------------------------------------------------------------------------------

    public bool DropActive
    {
        get {return dropActive;}
        set 
        {
            dropActive = value;
            if(value)
            {
                this.gameObject.layer = LayerMask.NameToLayer("Droppable");
            }
            else
            {
                this.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    public int Priority {
        get {return priority;}
        set {priority = value;}
    }

    public bool OnDrop(Draggable draggedObject)
    {
        // TODO: Animation when replacing an already attached support card
        if (AttachSupportCard(draggedObject.GetComponent<SupportCard>()) == 0)
        {
            draggedObject.transform.SetParent(transform);
            draggedObject.GetComponent<SortingGroup>().sortingLayerName = "Cards Background";
            draggedObject.GetComponent<Card>().SetWorldTargetPosition(transform.TransformPoint(new Vector3 (0, 0, 0.5f)));
            draggedObject.GetComponent<Card>().SetTargetRotation(new Vector3 (0, 0, 0));
            StartCoroutine(draggedObject.GetComponent<Card>().MoveToTarget(0.1f));
            draggedObject.enabled = false;
            if(deck.GetTablePlayer().isPlayer) deck.GetTablePlayer().detachButton.SetActive(true);
            return true;
        }
        return false;
    }

    public void OnLeave(Draggable draggedObject)
    {

    }

    public Transform GetTransform() { return this.transform; }

    // --- unused ---------------------------------------------------------------------------
    // Could be later used for replacing? But seems buggy...
    public int OccupiedSlots() {
        // 0: nothing occupied, 1: top slot occupied, 2: bottom slot occupied; 3: both slots occupied
        var occupied = 0;
        foreach(SupportCard card in supportCards.Values)
        {
            if(!card) continue;
            if(card.GetSlotType() == 1 && occupied == 0) occupied = 1;
            if(card.GetSlotType() == 1 && occupied == 2) occupied = 3;
            if(card.GetSlotType() == 2 && occupied == 0) occupied = 2;
            if(card.GetSlotType() == 2 && occupied == 1) occupied = 3;
        }
        return occupied;
    }

}
