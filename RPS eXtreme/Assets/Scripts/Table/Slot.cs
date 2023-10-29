using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Slot : MonoBehaviour, Droppable
{
    public Hand hand;

#nullable enable
    //private Draggable? dragCard;
    private Card? card;
#nullable disable

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

    // TODO ---------------------------------------------------------------
    public int GetPosition()
    {
        return 0;
    }

    public void TurnCards()
    {

    }

    public Card GetCard()
    {
        return new Card();
    }
}
