using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class TablePlayer : MonoBehaviour, Droppable
{
    [Header("Main Connections")]
    public Deck deck;
    public Cardpile drawpile;
    public Cardpile discardpile;
    public Hand hand;
    public List<Slot> slots;
    public bool isPlayer;
    
    protected Table table;

    // Droppable
    private bool dropActive = true;
    private readonly int priority = (int) DroppablePriorities.TABLE;

    #region Main Functions -------------------------------------------------------------------------------------------
    public virtual void Init(Table table) {
        this.table = table;

        dropActive = isPlayer;
        
        deck.init(this, table.animHandler); // TODO Deck: Cleanup the deck functions
        deck.transform.localPosition = drawpile.transform.localPosition; // TODO Deck: Move to Deck Init?
        drawpile.AddCards(deck.GetCards());
        drawpile.Shuffle();
        foreach (Slot slot in slots) {
            slot.Init(this, table.animHandler);
        }
    }

    // TODO Later: When dropping a card (for example into a slot) while this animation plays, it looks like lag, because the ondrop animation is queued after this one
    public virtual void DrawCards(int amount) {
        List<Card> cardsToFlip = new();
        for(int i=0; i<amount; i++) {
            try {
                Card drawnCard = drawpile.PopCard();
                AddToHandWithAnimation(drawnCard);
                cardsToFlip.Add(drawnCard);
            } catch {
                ShuffleDiscardIntoDraw();
                i--;
            }
        }

        if(isPlayer) {
            FlipCardAnim anim = AnimationHandler.CreateAnim<FlipCardAnim>();
            anim.Init(cardsToFlip);
            anim.Options(offsetTime: 0.2f);
            AnimationHandler.QueueAnimation(anim);
        }
    }

    public void ShuffleDiscardIntoDraw() {
        List<Card> shuffledCards = discardpile.PopAllCards();
        drawpile.AddCards(shuffledCards);
        drawpile.Shuffle();

        MoveCardAnim anim = AnimationHandler.CreateAnim<MoveCardAnim>();
        anim.Init(shuffledCards, drawpile.transform);
        anim.Options(moveTime: 0.5f, offsetTime: 0.1f, disableOnArrival: false);
        foreach (Card card in shuffledCards) { // TODO Kein Bock: This might not be needed, as all discarded cards were brought there by Clear Slot function. But better safe than sorry?
            card.transform.SetPositionAndRotation(discardpile.transform.position, discardpile.transform.rotation);
        }
        AnimationHandler.QueueAnimation(anim); // TODO Later: This animation sucks! (But it works, so I'll leave it for now)
    }

    public void ClearSlots() {
        List<Card> clearedCards = new();
        foreach (Slot slot in slots) {
            if(!slot.IsEmpty()) {
                NormalCard baseCard = slot.PopCard();
                List<SupportCard> attachedCards = baseCard.DetachAllCards();
                clearedCards.Add(baseCard);
                clearedCards.AddRange(attachedCards);
            }            
        }

        MoveCardAnim anim = AnimationHandler.CreateAnim<MoveCardAnim>();
        anim.Init(clearedCards, discardpile.transform);
        anim.Options(moveTime: 0.5f, disableOnArrival: true);
        if(isPlayer) {
            AnimationHandler.QueueAnimation(anim);
        } else {
            AnimationHandler.QueueAnimation(anim, AnimationQueueName.OPPONENT);
        } 
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
        PlayArrangeHandAnimation();
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

    // TODO Cards: Implement, once Cards have been reworked
    public List<Card> GetMatchingSupportCards(NormalCard baseCard) {
        return new();
    }

    public Slot GetSlotByNr(int slotPosition) {
        return slots.Where(s => s.slotPosition == slotPosition).FirstOrDefault();
    }

    public void PlayArrangeHandAnimation() {
        ArrangeHandAnim anim = AnimationHandler.CreateAnim<ArrangeHandAnim>();
        anim.Init(hand, GetCardsInHand());
        if(isPlayer) {
            AnimationHandler.QueueAnimation(anim);
        } else {
            AnimationHandler.QueueAnimation(anim, AnimationQueueName.OPPONENT);
        }
    }

    public void RemoveFromHand(Card card) {
        hand.RemoveCard(card);
    }

    public void RemoveFromHandWithAnimation(Card card) {
        RemoveFromHand(card);
        PlayArrangeHandAnimation();
    }
    #endregion

    // TODO Normalcard: Remove once Normalcard is refactored
    public Table GetTable() {
        return table;
    }
}