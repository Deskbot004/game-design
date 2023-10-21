using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TablePlayer : MonoBehaviour
{
    public Deck playerDeck;
    public Cardpile drawpile;
    public Cardpile discardpile;
    public Hand hand;
    public Slot[] slots;

    // -----Funktionen---------------------------------------------------------------
    void Start()
    {
        drawpile.cards = playerDeck.cards;
        drawpile.Shuffle();
    }

    public void DrawCards(int amount)
    {
        // Nicht genug Karten im Draw Pile
        if (drawpile.cards.Count < amount)
        {
            int amountMissing = amount - drawpile.cards.Count;
            DrawCards(drawpile.cards.Count);
            //DiscardToDrawpile();
            DrawCards(amountMissing);
            return;
        }

        // Draw abstract Cards
        List<Card> drawnCards = drawpile.cards.Take(amount).ToList();
        drawpile.cards = drawpile.cards.Except(drawnCards).ToList();
        hand.cards.AddRange(drawnCards);

        // Add those Cards as GameObjects
        foreach(Card card in drawnCards)
        {
            // TODO
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
}
