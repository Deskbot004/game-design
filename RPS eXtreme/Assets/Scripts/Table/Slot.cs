using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Slot : MonoBehaviour, Droppable
{
    public int slotPosition; // Leftmost is 0

    private TablePlayer tablePlayer;
    public List<Card> cards = new List<Card>();
    private bool dropActive = true;

    [Header("For Debugging")]
    public Card exampleCard; // Default card if slot is empty
#nullable enable
    public Card? card; // While combining cards isn't implemented
#nullable disable

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
            card = draggedObject.GetComponent<Card>();
            cards.Add(card);
            NormalCard normalCard = draggedObject.GetComponent<NormalCard>();
            cards.AddRange(normalCard.GetAttachedCards());
            card.transform.position = transform.position;
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
    public List<Card> GetCards()
    {
        // This check is only for Debugging
        // Once we have Empty Cards, cards can't be empty on resolve
        //if (cards.Count == 0) {
        //    List<Card> emptyCard = new List<Card>();
        //    emptyCard.Add(exampleCard);
        //    return emptyCard;
        //}
        return cards;
    }
    public void SetCards(List<Card> newCards) //Copies cards, doesn't set pointer of list
    {
        cards.Clear();
        cards.AddRange(newCards);
        card = newCards[0]; // For Debugging
    }
    public void SetCards(Card newCard)
    { 
        cards.Clear();
        cards.Add(newCard);
        newCard.SetSupposedPosition(this.transform.position);
        newCard.SetMoveSpeed(Vector3.Distance(newCard.transform.position, this.transform.position) / this.tablePlayer.GetTable().GetCardMoveTime());
        card = newCard; // For Debugging
    }
    public TablePlayer GetTablePlayer() {return tablePlayer;}

    // ------ For Debugging -------------------------------------------------------------------
    public Card GetCard() { return card; }
    public void ClearCard()
    {
        card = null;
        cards.Clear();
    }
}
