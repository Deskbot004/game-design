using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private List<Card> cards = new List<Card>();

    public List<Card> GetCards() {
        return new(cards);
    }

    public void AddCard(Card card) {
        cards.Add(card);
    }

    public void RemoveCard(Card cardToBeRemoved) {
        cards.Remove(cardToBeRemoved);
    }

    public List<Card> GetCardsBySymbol(CardSymbol symbol) {
        return cards.Where(c => c.GetSymbol() == symbol).ToList();
    }
}
