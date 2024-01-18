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
    public int rockAmount;
    public int paperAmount;
    public int scissorsAmount;
    public int spockAmount;
    public int lizardAmount;
    public int supportAmount;
    public string deckName;

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
        card.populateDictionaries();
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
        card.populateDictionaries();
        card.SetSlotType(type);
        card.SetFunctionNames(names);
        card.SetSprite();
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

    [ContextMenu("Create Deck")]
    void Creation()
    {
        counter = 0;
        List<Card> cards = new List<Card>();
        string[] names = { "draw:draw:2", "BR:win against:rock", "AR:extra damage:2", "BR:win on draw", "AR:lifesteal:1" };

        for (int i = 0; i < this.supportAmount; i++)
        {
            List<string> functionnames = new List<string>();
            functionnames.Add(names[i % 5]);
            SupportCard card5 = CreateSupportCard(i % 2, functionnames);
            cards.Add(card5);
        }
        for (int i = 0; i < this.scissorsAmount; i++)
        {
            NormalCard card0 = CreateNormalCard("scissors", i % 4);
            cards.Add(card0);
        }
        for (int i = 0; i < this.rockAmount; i++)
        {
            NormalCard card1 = CreateNormalCard("rock", i % 4);
            cards.Add(card1);
        }
        for (int i = 0; i < this.paperAmount; i++)
        {
            NormalCard card2 = CreateNormalCard("paper", i % 4);
            cards.Add(card2);
        }
        for (int i = 0; i < this.lizardAmount; i++)
        {
            NormalCard card3 = CreateNormalCard("lizard", i % 4);
            cards.Add(card3);
        }
        for (int i = 0; i < this.spockAmount; i++)
        {
            NormalCard card4 = CreateNormalCard("spock", i % 4);
            cards.Add(card4);
        }
        
        
        Deck deck = CreateDeck(cards, this.deckName);

        List<Card> deckCards = deck.GetCards();

    }

    public void Test()
    {
        Debug.Log("Hello");
    }
}
