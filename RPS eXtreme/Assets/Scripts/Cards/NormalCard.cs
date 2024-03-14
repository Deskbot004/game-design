using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.Assertions;

enum SlotPosition {
    TOP,
    BOTTOM
}

public class NormalCard : Card, Droppable
{
    private Dictionary<SlotPosition, SupportCard> supportCards = new();
    
    // Droppable
    private bool dropActive = false;
    private int priority = (int) DroppablePriorities.CARD;

    #region Main Functions ----------------------------------------------------------------------------------
    public override void Init(Deck deck) {
        base.Init(deck);
        if (topSlot) {
            supportCards.Add(SlotPosition.TOP, null);
        }
        if (bottomSlot) {
            supportCards.Add(SlotPosition.BOTTOM, null);
        }
    }

    public override void OnRightClick() {
        base.OnRightClick();
        GetPlayerSide().HandleStartAttaching(this);
    }

    public override void SetSprite() {
        base.SetSprite();
        NormalCardSprites cardSprites = GetComponent<NormalCardSprites>();
        Debug.Assert(cardSprites.symbolSprites.ContainsKey(symbol), "symbolSprites is missing key " + symbol.ToString(), this);

        // Set Title
        string upperCaseSymbol = string.Concat(symbol.ToString()[0], symbol.ToString().ToLower().Substring(1)); // (:
        this.transform.Find("Card Sprites/Title Text").GetComponent<TMP_Text>().text = upperCaseSymbol;

        // Set Window Icon
        this.transform.Find("Card Sprites/Symbol").GetComponent<SpriteRenderer>().sprite = cardSprites.symbolSprites[GetSymbol()];

        // Set slots
        transform.Find("Card Sprites/Upper Effect").gameObject.SetActive(topSlot);
        transform.Find("Card Sprites/Lower Effect").gameObject.SetActive(bottomSlot);
    }
    #endregion

    #region Attaching ---------------------------------------------------------------------------------------
    public bool AttachSupportCard(SupportCard card) {
        if(!IsCardCompatible(card)) return false;
        
        bool attachSuccess = true;
        if(card.topSlot) {
            attachSuccess = AttachToSlot(card, SlotPosition.TOP) && attachSuccess;
        }
        if(card.bottomSlot) {
            attachSuccess = AttachToSlot(card, SlotPosition.BOTTOM) && attachSuccess;
            // TODO: Detach top slot if not successful
        }
        return attachSuccess;
    }

    bool AttachToSlot(SupportCard card, SlotPosition slotPosition) {
        if (supportCards[slotPosition] != null) return false;
        supportCards[slotPosition] = card;
        return true;
    }

    public List<SupportCard> DetachAllCards() {
        List<SupportCard> removedCards = new();
        for(int i=0; i<supportCards.Count; i++) {
            SupportCard card = supportCards.ElementAt(i).Value;
            if(card != null) {
                DetachSupportCard(card);
                removedCards.Add(card);
            }
        }
        return removedCards;
    }
    
    public void DetachSupportCard(SupportCard card) {
        Debug.Assert(supportCards.Values.Contains(card), "SupportCard is not attached to this card", this);
        List<SlotPosition> slotPositions = supportCards.Where(x => x.Value == card).Select(x => x.Key).ToList();
        foreach (SlotPosition slotPos in slotPositions)
            DetachSlot(slotPos);
            // TODO: Attach again, if something goes wrong
        return;
    }

    void DetachSlot(SlotPosition slotPosition) {
        if (supportCards[slotPosition] == null) return;

        SupportCard detachedCard = supportCards[slotPosition];
        detachedCard.transform.SetParent(detachedCard.GetDeckTransform());
        detachedCard.SetSortingLayer("Cards in Focus"); //TODO Check: Might be wrong layer
        detachedCard.GetComponent<Draggable>().enabled = true;
        
        supportCards[slotPosition] = null;
        return;
    }
    #endregion

    #region Shorthands --------------------------------------------------------------------------------------
    public bool IsCardCompatible(SupportCard card) {
        return (this.topSlot && card.topSlot) || (this.bottomSlot && card.bottomSlot);
    }
    
    public List<SupportCard> GetAttachedSupportCards() {
        return new(supportCards.Values.Where(c => c != null)); // TODO: Test, I'm not sure :x
    }
    
    public bool HasAttachedCards() {
        return supportCards.Values.Where(c => c != null).ToList().Count > 0;
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
        // TODO: Animation when replacing an already attached support card
        bool attachSuccess = AttachSupportCard(draggedObject.GetComponent<SupportCard>());
        if (attachSuccess) {
            draggedObject.transform.SetParent(transform);
            SetSortingLayer("Attached Cards"); // TODO: Layering doesn't work :/
            GetPlayerSide().ShowDetachButton();

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

    public void OnLeave(Draggable draggedObject) {}
    #endregion

}
