using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TESTED
public class Slot : MonoBehaviour, Droppable
{
    public int slotPosition;

    private NormalCard cardInSlot;
    private AnimationHandler animHandler;

    // Droppable
    private bool dropActive = true;
    private int priority = (int) DroppablePriorities.SLOT;

    #region Main Functions --------------------------------------------------------------------------------------------
    public void Init(Table table, TablePlayer tablePlayer) {
        dropActive = tablePlayer.isPlayer;
        animHandler = table.animHandler;
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
            anim.destinationObject = transform;
            animHandler.QueueAnimation(anim);
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
    public bool isEmpty() {
        return cardInSlot == null;
    }
    #endregion

    // TODO -----------------------------------------------------------------------------
    // ------ Getter und Setter -------------------------------------------------------------------
    public int GetSlotPosition() { return slotPosition; }
    public List<Card> GetNormalAndSuppCards()
    {
        List<Card> cardsInSlot = new List<Card>();
        if(cardInSlot != null)
        {
            cardsInSlot.Add(cardInSlot);
            foreach(SupportCard supCard in cardInSlot.GetAttachedSupportCards())
                cardsInSlot.Add(supCard);
        }
        return cardsInSlot;
    }

    public void SetCard(NormalCard newCard)
    {
        newCard.SetWorldTargetPosition(transform.position + new Vector3(0f, 0f, -0.01f));
        newCard.SetTargetRotation(new Vector3(0f, 0f, 0f));
        newCard.SetStatus(2);
        cardInSlot = newCard;

        StartCoroutine(newCard.MoveToTarget(0.1f));
    }

    // ------ For Debugging -------------------------------------------------------------------
    public NormalCard GetCard() { return cardInSlot; }
}
