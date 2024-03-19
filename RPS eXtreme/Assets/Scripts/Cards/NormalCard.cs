using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.Assertions;

public class NormalCard : Card, Droppable
{
    private Dictionary<CardSlotPosition, SupportCard> attachedSupportCards = new();
    
    // Droppable
    private bool dropActive = false;
    private int priority = (int) DroppablePriorities.CARD;

    #region Main Functions ----------------------------------------------------------------------------------
    public override void Init(Deck deck) {
        base.Init(deck);
        if (topSlot) {
            attachedSupportCards.Add(CardSlotPosition.TOP, null);
        }
        if (bottomSlot) {
            attachedSupportCards.Add(CardSlotPosition.BOTTOM, null);
        }
    }

    public override void OnRightClick() {
        base.OnRightClick();
        GetTableSide().HandleStartAttaching(this);
    }

    public override void SetSprite() {
        base.SetSprite();

        // Init
        NormalCardSprites cardSprites = GetComponent<NormalCardSprites>();
        Debug.Assert(cardSprites.symbolSprites.ContainsKey(symbol), "symbolSprites is missing key " + symbol.ToString(), this);

        // Set Title
        string upperCaseSymbol = string.Concat(symbol.ToString()[0], symbol.ToString().ToLower().Substring(1)); // (:
        transform.Find("Card Sprites/Title Text").GetComponent<TMP_Text>().text = upperCaseSymbol;

        // Set Window Icon
        transform.Find("Card Sprites/Symbol").GetComponent<SpriteRenderer>().sprite = cardSprites.symbolSprites[symbol];

        // Set slots
        transform.Find("Card Sprites/Upper Effect").gameObject.SetActive(topSlot);
        transform.Find("Card Sprites/Lower Effect").gameObject.SetActive(bottomSlot);
    }
    #endregion

    #region Attaching ---------------------------------------------------------------------------------------
    public bool AttachSupportCard(SupportCard supportCard) {
        if(!IsCardCompatible(supportCard)) return false;
        
        bool attachSuccess = true;
        if(supportCard.topSlot) {
            attachSuccess = AttachCardToSlot(supportCard, CardSlotPosition.TOP) && attachSuccess;
        }
        if(supportCard.bottomSlot) {
            attachSuccess = AttachCardToSlot(supportCard, CardSlotPosition.BOTTOM) && attachSuccess;
            // TODO: Detach top slot if not successful, but top attach was successful with this card
        }
        return attachSuccess;
    }

    bool AttachCardToSlot(SupportCard card, CardSlotPosition slotPosition) {
        if (attachedSupportCards[slotPosition] != null) return false;
        attachedSupportCards[slotPosition] = card;
        return true;
    }

    public List<SupportCard> DetachAllCards() {
        List<SupportCard> detachedCards = new();
        for(int i=0; i<attachedSupportCards.Count; i++) {
            CardSlotPosition slotPosition = attachedSupportCards.ElementAt(i).Key;
            SupportCard supportCard = DetachFromSlot(slotPosition);
            if(supportCard != null && !detachedCards.Contains(supportCard)) {
                detachedCards.Add(supportCard);
            }
        }
        return detachedCards;
    }

    SupportCard DetachFromSlot(CardSlotPosition slotPosition) {
        if (attachedSupportCards[slotPosition] == null) return null;

        SupportCard detachedCard = attachedSupportCards[slotPosition];
        detachedCard.SetDeckAsParent();
        detachedCard.SetSortingLayer(SortingLayer.CARDS_IN_FOCUS); //TODO Check: Might be wrong layer
        detachedCard.GetComponent<Draggable>().enabled = true;
        attachedSupportCards[slotPosition] = null;
        return detachedCard;
    }
    #endregion

    #region Shorthands --------------------------------------------------------------------------------------
    public bool IsCardCompatible(SupportCard supportCard) {
        // (supCard.top => this.top) && (supCard.bot => this.bot)
        return (!supportCard.topSlot || this.topSlot) && (!supportCard.bottomSlot || this.bottomSlot);
    }
    
    public List<SupportCard> GetAttachedSupportCards() {
        return new(attachedSupportCards.Values.Where(c => c != null));
    }
    
    public bool HasAttachedCards() {
        return attachedSupportCards.Values.Where(c => c != null).ToList().Count > 0;
    }
    #endregion

    #region Droppable ---------------------------------------------------------------------------------------
    public bool DropActive {
        get {return dropActive;}
        set {
            dropActive = value;
            if(value) {
                this.gameObject.layer = LayerMask.NameToLayer("Droppable");
            } else {
                this.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    public int Priority {
        get {return priority;}
        set {priority = value;}
    }

    public Transform Transform {
        get {return transform;}
    }

    public bool OnDrop(Draggable draggedObject) {
        bool attachSuccess = AttachSupportCard(draggedObject.GetComponent<SupportCard>());
        if (attachSuccess) {
            draggedObject.transform.SetParent(transform);
            SetSortingLayer(SortingLayer.ATTACHED_CARDS_IN_FOCUS);
            GetTableSide().ShowDetachButton();

            MoveCardAnim anim = AnimationHandler.CreateAnim<MoveCardAnim>();
            anim.Init(new() {draggedObject.GetComponent<Card>()}, transform);
            anim.Options(draggableOnArrival: false);
            if(BelongsToPlayer())
                AnimationHandler.QueueAnimation(anim);
            else
                AnimationHandler.QueueAnimation(anim, AnimationQueueName.ENEMY);
        }
        return attachSuccess;
    }

    // This Function is called, when picking up a Draggable (by left clicking) inside this Droppable
    // Since you can't drag an Attached SupportCard, this function shouldn't be called
    public void OnLeave(Draggable draggedObject) {}
    #endregion

}
