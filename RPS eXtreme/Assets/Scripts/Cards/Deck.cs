using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Deck : MonoBehaviour
{ 
    
    public List<Card> cards;

    public void Initialize()
    {
        this.cards = new List<Card>();
    }

    public void AddCard(Card card)
    {
        this.cards.Add(card);
        card.gameObject.transform.parent = this.gameObject.transform;
    }

    public void AddCardDeck(List<Card> deck)
    {
        this.cards = deck;
        foreach(Card card in this.cards)
        {
            card.gameObject.transform.parent = this.gameObject.transform;
        }
    }

    public List<Card> GetCards()
    {
        return cards;
    }


    [ContextMenu("Add Cards")]
    void AddCards()
    {
        cards = new List<Card>(GetComponentsInChildren<Card>());
    }
}
