using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Slot : MonoBehaviour, Droppable
{
    public int slotPosition; // Leftmost is 0

    private TablePlayer tablePlayer;
    private bool dropActive = true;
    private NormalCard card;

    public void init(TablePlayer tablePlayer) 
    {
        this.tablePlayer = tablePlayer;
    }

    // ---------- Slot Functions ----------------------------------------------
    // TODO
    public void TurnCards()
    {

    }

    // ---------- Droppable Functions -----------------------------------------
    public bool DropActive
    {
        get {return dropActive;}
        set {dropActive = value;}
    }
    
    // TODO: Alle Droppables �berarbeiten, wenn Karten kombinieren implementiert ist (card -> cards)
    public bool OnDrop(Draggable draggedObject)
    {
        // Check whether the Slot is empty and the dropped Object is a Basic Card
        if (card != null || draggedObject.GetComponent<NormalCard>() == null) // Not the case
        {
            return false;
        }
        else // Yes the case
        {
            card = draggedObject.GetComponent<NormalCard>();
            card.SetWorldTargetPosition(transform.position + new Vector3(0f, 0f, -0.01f));
            card.SetTargetRotation(new Vector3(0f, 0f, 0f));
            StartCoroutine(card.MoveToTarget(0.1f));
            return true;
        }
    }

    public void OnLeave(Draggable draggedObject)
    {
        card = null;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    // ------ Getter und Setter -------------------------------------------------------------------
    public int GetSlotPosition() { return slotPosition; }
    public List<Card> GetNormalAndSuppCards()
    {
        List<Card> cardsInSlot = new List<Card>();
        if(card != null)
        {
            cardsInSlot.Add(card);
            foreach(SupportCard supCard in card.GetAttachedSupportCards())
                cardsInSlot.Add(supCard);
        }
        return cardsInSlot;
    }
    public void SetCard(NormalCard newCard)
    { 
        newCard.SetWorldTargetPosition(this.transform.position);
        //newCard.SetMoveSpeed(Vector3.Distance(newCard.transform.position, this.transform.position) / this.tablePlayer.GetTable().GetCardMoveTime());
        card = newCard; // For Debugging
    }
    public TablePlayer GetTablePlayer() {return tablePlayer;}

    // ------ For Debugging -------------------------------------------------------------------
    public NormalCard GetCard() { return card; }
    public void ClearCard()
    {
        card = null;
    }
}
