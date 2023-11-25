using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Constructor : MonoBehaviour
{
    public GameObject NormalCard;
    public GameObject SupportCard;
    public GameObject Deck;
    
    public NormalCard CreateNormalCard()
    {
        GameObject cardObject = Instantiate(NormalCard, new Vector3(0, 0, 0), Quaternion.identity);
        return cardObject.GetComponent<NormalCard>();

    }

    public SupportCard CreateSupportCard()
    {
        GameObject cardObject = Instantiate(SupportCard, new Vector3(0, 0, 0), Quaternion.identity);
        return cardObject.GetComponent<SupportCard>();
    }

    public Deck CreateDeck()
    {
        GameObject deckObject = Instantiate(Deck, new Vector3(0, 0, 0), Quaternion.identity);
        return deckObject.GetComponent<Deck>();
    }

    [ContextMenu("Creation")]
    void Creation()
    {
        List<Card> cards = new List<Card>();
        NormalCard card1 = CreateNormalCard();
        SupportCard card2 = CreateSupportCard();
        cards.Add(card1);
        Deck deck = CreateDeck();
        deck.AddCardDeck(cards);
        deck.AddCard(card2);
        List<Card> tryout = deck.GetCards();
        foreach(Card card in tryout)
        {
            if (card.IsBasic())
            {
                Debug.Log("Found Normal Card" + card);
            }
            else
            {
                Debug.Log("Found Support Card" + card);
            }
        }
        card1.SetSymbol("stone");
        Debug.Log(card1.GetSymbol());
         

    }
}
