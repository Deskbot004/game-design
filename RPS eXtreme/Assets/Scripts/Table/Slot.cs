using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Slot : MonoBehaviour, Droppable
{
    public int slotPosition; // Leftmost is 0

            #nullable enable
    public Card? card;
#nullable disable
    private List<Card> cards = new List<Card>();

    [Header("For Debugging")]
    public Card exampleCard; // Default card if slot is empty

    // ---------- Slot Functions ----------------------------------------------
    // Returns the current Card in the Slot
    public Card GetCard()
    {
        // TODO: The object calling GetCard should check whether the returned card isn't null
        // Since card is public, do we even need this function?
        //Debug.Assert(card != null, "Slot doesn't have a card", this);
        //return card;

        return card != null ? card : exampleCard;
    }

    // ---------- Droppable Functions -----------------------------------------
    public bool OnDrop(Draggable draggedObject)
    {
        // Check whether the Slot is empty and the dropped Object is a Card
        if (card != null || draggedObject.GetComponent<Card>() == null) // Not the case
        {
            return false;
        }
        else // Yes the case
        {
            card = draggedObject.GetComponent<Card>();
            cards.Add(card);
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

    // TODO ---------------------------------------------------------------
    public void TurnCards()
    {

    }

    public int GetSlotPosition()
    {
        return slotPosition;
    }

    public List<Card> GetCards()
    {
        return cards;
    }
}
