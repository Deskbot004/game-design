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
    private Gamelogic logic;

    public void Awake()
    {
        this.logic = GameObject.Find("Gamelogic").GetComponent<Gamelogic>();
    }



    private int counter;
    
    /*
     *  Creates a NormalCard but doesn't initialize it
     */

    public NormalCard CreateEmptyNormalCard()
    {
        GameObject cardObject = Instantiate(NormalCard, new Vector3(0, 0, 0), Quaternion.identity);
        cardObject.SetActive(false);
        return cardObject.GetComponent<NormalCard>();

    }

    /*
     *  Creates a NormalCard and initializes it
     */

    public NormalCard CreateNormalCard(string symbol, int type)
    {
        GameObject cardObject = Instantiate(NormalCard, new Vector3(0, 0, 0), Quaternion.identity);
        cardObject.name = "Normal Card " + counter;
        counter++;
        cardObject.SetActive(false);
        NormalCard card = cardObject.GetComponent<NormalCard>();
        card.SetSymbol(symbol);
        card.SetSlotType(type);
        card.SetSprite();
        return card;
    }

    /*
     *  Creates a SupportCard but doesn't initialize it,except for its symbol
     */

    public SupportCard CreateEmptySupportCard()
    {
        GameObject cardObject = Instantiate(SupportCard, new Vector3(0, 0, 0), Quaternion.identity);
        cardObject.SetActive(false);
        SupportCard card = cardObject.GetComponent<SupportCard>();
        card.SetSymbol("support");
        card.SetSprite();
        return card;
    }

    /*
     *  Creates a SupportCard and initializes it
     */

    public SupportCard CreateSupportCard(int type, List<string> names)
    {
        GameObject cardObject = Instantiate(SupportCard, new Vector3(0, 0, 0), Quaternion.identity);
        cardObject.name = "Support Card " + counter;
        counter++;
        cardObject.SetActive(false);
        SupportCard card = cardObject.GetComponent<SupportCard>();
        card.SetSymbol("support");
        card.SetSlotType(type);
        card.SetSprite();
        card.SetFunctionNames(names);
        return card;
    }

    /*
     *  Creates a Deck but doesn't initialize it
     */

    public Deck CreateEmptyDeck()
    {
        GameObject deckObject = Instantiate(Deck, new Vector3(0, 0, 0), Quaternion.identity);
        deckObject.GetComponent<Deck>().SetConstructor(this);
        return deckObject.GetComponent<Deck>();
    }

    /*
     *  Creates a Deck and initializes it
     */

    public Deck CreateDeck(List<Card> cards, string deckname)
    {
        GameObject deckObject = Instantiate(Deck, new Vector3(0, 0, 0), Quaternion.identity);
        Deck deck = deckObject.GetComponent<Deck>();
        deck.SetConstructor(this);
        deck.AddCardDeck(cards);
        deck.SetDeckName(deckname);
        return deck;
    }

    // ---------- For Debugging --------------------------------------------------------------------------------

    [ContextMenu("Create tryout Deck")]
    void Creation()
    {
        counter = 0;
        List<Card> cards = new List<Card>();
        string[] names = { "draw:2", "win against:rock", "extra damage:2", "win on draw" };
        for(int i = 0; i < 4; i++)
        {
            NormalCard card0 = CreateNormalCard("scissors", i);
            cards.Add(card0);
            NormalCard card1 = CreateNormalCard("rock", i);
            cards.Add(card1);
            NormalCard card2 = CreateNormalCard("paper", i);
            cards.Add(card2);
            NormalCard card3 = CreateNormalCard("lizard", i);
            cards.Add(card3);
            NormalCard card4 = CreateNormalCard("spock", i);
            cards.Add(card4);
            List<string> functionnames = new List<string>();
            functionnames.Add(names[i]);
            SupportCard card5 = CreateSupportCard(i % 2, functionnames);
            cards.Add(card5);
        }
        Deck deck = CreateDeck(cards, "playerDeck");

        List<Card> deckCards = deck.GetCards();

        deck.SaveDeck();

        Deck deck2 = CreateEmptyDeck();

        deck2.LoadDeck("opponentDeck");

    }

    public void Test()
    {
        Debug.Log("Hello");
    }
}
