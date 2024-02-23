using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slot : MonoBehaviour, Droppable
{
    public int slotPosition;

    private NormalCard cardInSlot;
    private AnimationHandler animHandler;
    private bool isPlayer;

    // Droppable
    private bool dropActive = true;
    private int priority = (int) DroppablePriorities.SLOT;

    #region Main Functions --------------------------------------------------------------------------------------------
    public void Init(TablePlayer tablePlayer, AnimationHandler animHandler) {
        isPlayer = tablePlayer.isPlayer;
        dropActive = tablePlayer.isPlayer;
        this.animHandler = animHandler;
    }
    
    public NormalCard PopCard() {
        NormalCard poppedCard = cardInSlot;
        OnLeave(poppedCard.GetComponent<Draggable>());
        return poppedCard;
    }
    #endregion

    #region Droppable --------------------------------------------------------------------------------------------
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
        MoveCardAnim anim = animHandler.CreateAnim<MoveCardAnim>();
        NormalCard droppedCard = draggedObject.GetComponent<NormalCard>();
        if (cardInSlot == null && droppedCard != null) {
            GetComponent<BoxCollider2D>().enabled = false;
            cardInSlot = droppedCard;
            anim.cards = new() {droppedCard};
            anim.targetWorldPosition = transform.position;
            anim.targetWorldRotation = Vector3.zero;
            if(isPlayer)
                animHandler.QueueAnimation(anim);
            else
                animHandler.QueueAnimation(anim, (int) AnimationOffQueues.OPPONENT);
            return true;
        } else {
            return false;
        }
    }

    public void OnLeave(Draggable draggedObject) {
        GetComponent<BoxCollider2D>().enabled = true;
        cardInSlot = null;
    }
    #endregion

    #region Shorthands -------------------------------------------------------------------------------------------
    public NormalCard GetCard() { 
        return cardInSlot; 
    }

    public List<Card> GetNormalAndSuppCards() {
        List<Card> cardsInSlot = new List<Card>();
        if(cardInSlot != null) {
            cardsInSlot.Add(cardInSlot);
            foreach(SupportCard supCard in cardInSlot.GetAttachedSupportCards())
                cardsInSlot.Add(supCard);
        }
        return cardsInSlot;
    }
    
    public bool isEmpty() {
        return cardInSlot == null;
    }
    #endregion
}
