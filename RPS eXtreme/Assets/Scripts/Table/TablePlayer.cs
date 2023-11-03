using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TablePlayer : MonoBehaviour, DefaultDroppable
{
    public Deck playerDeck;
    public Cardpile drawpile;
    public Cardpile discardpile;
    public Hand hand;
    public List<Slot> slots;

    // For Debugging
    public void resolve()
    {
        foreach (Slot slot in slots)
        {
            discardpile.cards.Add(slot.card);
            slot.card.gameObject.SetActive(false);
            slot.card = null;
        }
    }

    // -----Funktionen---------------------------------------------------------------
    void Start()
    {
        drawpile.cards = playerDeck.cards;
        drawpile.Shuffle();
        foreach (Card card in playerDeck.cards)
        {
            card.gameObject.SetActive(false);
        }
    }

    public void DrawCards(int amount)
    {
        // Nicht genug Karten im Draw Pile
        if (drawpile.cards.Count < amount)
        {
            int amountMissing = amount - drawpile.cards.Count;
            DrawCards(drawpile.cards.Count);
            DiscardToDrawpile();
            DrawCards(amountMissing);
            return;
        }

        // Draw abstract Cards
        List<Card> drawnCards = drawpile.cards.Take(amount).ToList();
        drawpile.cards = drawpile.cards.Except(drawnCards).ToList();
        hand.cards.AddRange(drawnCards);

        // Add those Cards as GameObjects
        hand.ArrangeHand();
        foreach (Card card in drawnCards)
        {
            // TODO: Play Animation
            card.gameObject.SetActive(true);
        }
    }

    // Shuffles all cards in the Discardpile into the Drawpile
    public void DiscardToDrawpile()
    {
        drawpile.cards.AddRange(discardpile.cards);
        discardpile.cards.Clear();
        drawpile.Shuffle();
        // TODO: Play animation
    }

    // Droppable -----------------------------------------------------------------------------------------------
    public bool OnDrop(Draggable draggedObject)
    {
        hand.cards.Add(draggedObject.GetComponent<Card>());
        hand.ArrangeHand();
        return true;
    }

    public void OnLeave(Draggable draggedObject)
    {
        hand.cards.Remove(draggedObject.GetComponent<Card>());
        hand.ArrangeHand();
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
