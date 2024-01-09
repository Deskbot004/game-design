using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/*
 * A class to generate new cards and decks using the prefabs
 */

public class Constructor : MonoBehaviour
{
    public GameObject NormalCard;
    public GameObject SupportCard;
    public GameObject Deck;
    
    /*
     *  Creates a NormalCard but doesn't initialize it
     */

    public NormalCard CreateEmptyNormalCard()
    {
        GameObject cardObject = Instantiate(NormalCard, new Vector3(0, 0, 0), Quaternion.identity);
        return cardObject.GetComponent<NormalCard>();

    }

    /*
     *  Creates a NormalCard and initializes it
     */

    public NormalCard CreateNormalCard(string symbol)
    {
        GameObject cardObject = Instantiate(NormalCard, new Vector3(0, 0, 0), Quaternion.identity);
        NormalCard card = cardObject.GetComponent<NormalCard>();
        card.SetSymbol(symbol);
        return card;
    }

    /*
     *  Creates a SupportCard but doesn't initialize it,except for its symbol
     */

    public SupportCard CreateEmptySupportCard()
    {
        GameObject cardObject = Instantiate(SupportCard, new Vector3(0, 0, 0), Quaternion.identity);
        SupportCard card = cardObject.GetComponent<SupportCard>();
        card.SetSymbol("support");
        return card;
    }

    /*
     *  Creates a Deck but doesn't initialize it
     */

    public Deck CreateEmptyDeck()
    {
        GameObject deckObject = Instantiate(Deck, new Vector3(0, 0, 0), Quaternion.identity);
        return deckObject.GetComponent<Deck>();
    }

    /*
     *  Creates a Deck and initializes it
     */

    public Deck CreateDeck(List<Card> cards, string deckname)
    {
        GameObject deckObject = Instantiate(Deck, new Vector3(0, 0, 0), Quaternion.identity);
        Deck deck = deckObject.GetComponent<Deck>();
        deck.AddCardDeck(cards);
        deck.SetDeckName(deckname);
        return deck;
    }

    

    [ContextMenu("Create tryout Deck")]
    void Creation()
    {
        List<Card> cards = new List<Card>();
        for(int i = 0; i < 1; i++)
        {
            NormalCard card0 = CreateNormalCard("scissors");
            cards.Add(card0);
            NormalCard card1 = CreateNormalCard("stone");
            cards.Add(card1);
            NormalCard card2 = CreateNormalCard("paper");
            cards.Add(card2);
            NormalCard card3 = CreateNormalCard("lizard");
            cards.Add(card3);
            NormalCard card4 = CreateNormalCard("spock");
            cards.Add(card4);
            SupportCard card5 = CreateEmptySupportCard();
            cards.Add(card5);
        }
        Deck deck = CreateDeck(cards, "tryout");

        List<Card> deckCards = deck.GetCards();

        deck.SaveDeck();

        deck.LoadDeck("tryout");

    }
}
