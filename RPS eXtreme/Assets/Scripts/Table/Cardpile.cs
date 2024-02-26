using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Cardpile : MonoBehaviour
{
    public List<Card> cardsInside = new();

    #region Main Functions -----------------------------------------------------------------------------------------
    public void Shuffle() {
        System.Random rng = new System.Random();
        cardsInside = cardsInside.OrderBy(x => rng.Next()).ToList();
    }

    public List<Card> GetSortedCards() {
        return cardsInside.OrderBy(c => c.symbol).ToList();
    }
    #endregion

    #region Shorthands -------------------------------------------------------------------------------------------
    public void AddCards(List<Card> cards) {
        cardsInside.AddRange(cards);
    }
    
    public List<Card> PopAllCards() {
        List<Card> poppedCards = new(cardsInside);
        cardsInside.Clear();
        return poppedCards;
    }
    
    public Card PopCard() {
        Card poppedCard = cardsInside[0];
        cardsInside.RemoveAt(0);
        return poppedCard;
    }
    #endregion
}
