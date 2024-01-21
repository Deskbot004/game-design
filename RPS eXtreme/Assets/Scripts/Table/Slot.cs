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
    public void TurnCards()
    {
        card.GetComponent<Animator>().SetBool("flip", true);
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
            SetCard(draggedObject.GetComponent<NormalCard>());
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
        newCard.SetWorldTargetPosition(transform.position + new Vector3(0f, 0f, -0.01f));
        newCard.SetTargetRotation(new Vector3(0f, 0f, 0f));
        newCard.SetStatus(2);
        card = newCard;

        StartCoroutine(newCard.MoveToTarget(0.1f));
    }
    public TablePlayer GetTablePlayer() {return tablePlayer;}

    // ------ For Debugging -------------------------------------------------------------------
    public NormalCard GetCard() { return card; }
    public void ClearCard()
    {
        card = null;
    }
}
