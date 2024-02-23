using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class TablePlayer : MonoBehaviour, Droppable
{
    public Deck deck;
    public Cardpile drawpile;
    public Cardpile discardpile;
    public Hand hand;
    public List<Slot> slots;
    public bool isPlayer;
    
    protected AnimationHandler animHandler;
    protected Table table;

    // Droppable
    private bool dropActive = true;
    private readonly int priority = (int) DroppablePriorities.TABLE;

    #region Main Functions -------------------------------------------------------------------------------------------
    public virtual void Init(Table table, AnimationHandler animHandler) {
        // TODO: Cleanup the deck functions
        deck.init(this, animHandler);
        deck.transform.localPosition = drawpile.transform.localPosition;

        drawpile.AddCards(deck.GetCards());
        drawpile.Shuffle();
        foreach (Slot slot in slots) {
            slot.Init(this, animHandler);
        }
        this.animHandler = animHandler;
        this.table = table;
        dropActive = isPlayer;
    }

    // TODO: When dropping a card (for example into a slot) while this animation plays, it looks like lag, because the ondrop animation is queued after this one
    public virtual void DrawCards(int amount) {
        FlipCardAnim anim = animHandler.CreateAnim<FlipCardAnim>();
        anim.flippedCards = new();
        anim.offsetTime = 0.2f;

        for(int i=0; i<amount; i++) {
            try {
                Card drawnCard = drawpile.PopCard();
                AddToHandWithAnimation(drawnCard);

                anim.flippedCards.Add(drawnCard);
            } catch {
                ShuffleDiscardIntoDraw();
                i--;
            }
        }

        if(isPlayer)
            animHandler.QueueAnimation(anim);
        else
            anim.DestroyAnim();
    }

    public void ShuffleDiscardIntoDraw() {
        MoveCardAnim anim = animHandler.CreateAnim<MoveCardAnim>();
        anim.cards = discardpile.PopAllCards();
        drawpile.AddCards(anim.cards);
        drawpile.Shuffle();

        foreach (Card card in anim.cards) {
            card.transform.SetPositionAndRotation(discardpile.transform.position, discardpile.transform.rotation);
        }
        anim.destinationObject = drawpile.transform;
        anim.moveTime = 0.5f;
        anim.offsetTime = 0.1f;
        anim.disableOnArrival = true;
        animHandler.QueueAnimation(anim); // TODO: This animation sucks! (But it works, so I'll leave it for now)
    }

    public void ClearSlots() {
        MoveCardAnim anim = animHandler.CreateAnim<MoveCardAnim>();
        List<Card> clearedCards = new();
        foreach (Slot slot in slots) {
            if(!slot.isEmpty()) {
                NormalCard baseCard = slot.PopCard();
                List<SupportCard> attachedCards = baseCard.DetachAllCards();
                clearedCards.Add(baseCard);
                clearedCards.AddRange(attachedCards);
            }            
        }

        anim.cards = clearedCards;
        anim.destinationObject = discardpile.transform;
        anim.moveTime = 0.5f;
        anim.disableOnArrival = true;
        if(isPlayer)
            animHandler.QueueAnimation(anim);
        else
            animHandler.QueueAnimation(anim, (int) AnimationOffQueues.OPPONENT);
    }
    #endregion

    #region Droppable -------------------------------------------------------------------------------------------
    public bool DropActive {
        get {return dropActive;}
        set {dropActive = value;}
    }

    public int Priority {
        get {return priority;}
    }

    public Transform Transform {
        get {return transform;}
    }

    public bool OnDrop(Draggable draggedObject) {
        AddToHandWithAnimation(draggedObject.GetComponent<Card>());
        return true;
    }

    public void OnLeave(Draggable draggedObject) {
        RemoveFromHandWithAnimation(draggedObject.GetComponent<Card>());
    }
    #endregion

    #region Shorthands -------------------------------------------------------------------------------------------
    public void AddToHand(Card card) {
        hand.AddCard(card);
    }
    
    public void AddToHandWithAnimation(Card card) {
        AddToHand(card);

        ArrangeHandAnim anim = animHandler.CreateAnim<ArrangeHandAnim>();
        anim.animHandler = animHandler;
        anim.hand = hand;
        anim.cardsInHand = GetCardsInHand();
        if(isPlayer)
            animHandler.QueueAnimation(anim);
        else
            animHandler.QueueAnimation(anim, (int) AnimationOffQueues.OPPONENT);
    }
    
    public void EnableSlots(bool enabled) {
        foreach (Slot slot in slots) 
            slot.DropActive = enabled;
    }

    public List<Card> GetAllCards() {
        return new(deck.cards);
    }
    
    public List<Card> GetCardsInHand() {
        return hand.GetCards();
    }

    public Card GetCardInSlot(int slotNr) {
        return GetSlotByNr(slotNr).GetCard();
    }

    // TODO once Cards have been reworked
    public List<Card> GetMatchingSupportCards(NormalCard baseCard) {
        return new();
    }

    public Slot GetSlotByNr(int slotPosition) {
        return slots.Where(s => s.slotPosition == slotPosition).FirstOrDefault();
    }

    public void RemoveFromHand(Card card) {
        hand.RemoveCard(card);
    }

    public void RemoveFromHandWithAnimation(Card card) {
        RemoveFromHand(card);

        ArrangeHandAnim anim = animHandler.CreateAnim<ArrangeHandAnim>();
        anim.animHandler = animHandler;
        anim.hand = hand;
        anim.cardsInHand = GetCardsInHand();
        if(isPlayer)
            animHandler.QueueAnimation(anim);
        else
            animHandler.QueueAnimation(anim, (int) AnimationOffQueues.OPPONENT);
    }
    #endregion

    // TODO: Remove once Normalcard is refactored
    public Table GetTable() {
        return table;
    }
}