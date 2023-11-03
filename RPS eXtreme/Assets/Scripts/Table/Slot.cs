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

    [Header("For Debugging")]
    public Card exampleCard;

    // ---------- Slot Functions ----------------------------------------------
    public Card GetCard()
    {
        //Debug.Assert(card != null, "Slot doesn't have a card", this);
        //return card;

        return card != null ? card : exampleCard;
    }

    // ---------- Droppable Functions -----------------------------------------
    public bool OnDrop(Draggable draggedObject)
    {
        if (card != null || draggedObject.GetComponent<Card>() == null)
        {
            return false;
        }
        else
        {
            //dragCard = draggedObject;
            card = draggedObject.GetComponent<Card>();
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
}
