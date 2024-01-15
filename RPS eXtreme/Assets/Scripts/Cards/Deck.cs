using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class Deck : MonoBehaviour
{
    public GameObject NormalCard;
    public GameObject SupportCard;
    private Constructor constructor;
    public List<Card> cards = new List<Card>();
    public string deckName;
    private TablePlayer tablePlayer;

    public void Awake()
    {
        this.constructor = GameObject.Find("Constructor").GetComponent<Constructor>();
    }

    public void init(TablePlayer tablePlayer) 
    {
        this.tablePlayer = tablePlayer;
        foreach (Card card in cards) 
        {
            card.init(this);
        }
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
        return this.cards;
    }

    /*
     * Saves the Deck into a text file using Json and a DeckManager.
     */

    public void SaveDeck()
    {
        string filename_location = Path.Combine(Application.persistentDataPath, this.deckName);
        DeckManager save = new DeckManager();
        for (int i = 0; i < this.cards.Count; i++)
        {
            if (this.cards[i].IsBasic()) // card is a NormalCard
            {
                NormalCard card = (NormalCard) this.cards[i];
                save.cardSymbols.Add(card.GetSymbol());
                save.cardTypes.Add(0);
                save.functions.Add("");
                save.slotTypes.Add(card.GetSlotType());
            }
            else
            {
                SupportCard card = (SupportCard)this.cards[i];
                save.cardSymbols.Add(card.GetSymbol());
                save.cardTypes.Add(1);
                List<string> functionnames = card.GetFunctionNames();
                string functionString = functionnames[0];
                foreach(string name in functionnames)
                {
                    if(name != functionnames[0])
                    {
                        functionString += ";" + name;
                    }
                }
                save.functions.Add(functionString);
                save.slotTypes.Add(card.GetSlotType());
            }
        }
        string savedDeck = JsonUtility.ToJson(save);

        Debug.Log("Saved String: " + savedDeck);

        File.WriteAllText(filename_location, savedDeck);

    }

    /*
     * Loads the Deck from a text file using Json and a DeckManager.
     */

    public void LoadDeck(string filename)
    {
        //Read text from file and convert it into a DeckManager
        string filename_location = Path.Combine(Application.persistentDataPath, filename);
        string savedDeck = "";
        if (File.Exists(filename_location))
        {
             savedDeck = File.ReadAllText(filename_location);
        }
        else
        {
            Debug.Log("Something went wrong while saving the deck. The savefile was not found.");
            return;
        }

        Debug.Log("Loaded String: " + savedDeck);

        DeckManager save = JsonUtility.FromJson<DeckManager>(savedDeck);
        if (!(save.cardSymbols.Count == save.cardTypes.Count && save.cardSymbols.Count == save.functions.Count && save.cardSymbols.Count == save.slotTypes.Count))
        {
            Debug.Log("Something went wrong while saving the deck. The lists do not have the same length and can't be decoded.");
            return;
        }

        //Remove old Cards starting with last Element
        int count = this.cards.Count;
        for (int i = count; i > 0; i--)
        {
            Card card = this.cards[i - 1];
            this.cards.RemoveAt(i - 1);
            Destroy(card.gameObject);
        }

        //Add new Cards
        for (int i = 0; i < save.cardSymbols.Count; i++)
        {
            if (save.cardTypes[i] == 0) // saved Card at index i was a NormalCard
            {
                NormalCard card = this.constructor.CreateNormalCard(save.cardSymbols[i], save.slotTypes[i]);
                this.AddCard(card);
            }
            else
            {
                string[] namesArray = save.functions[i].Split(";");
                List<string> names = new List<string>(namesArray);
                SupportCard card = this.constructor.CreateSupportCard(save.slotTypes[i], names);
                this.AddCard(card);
            }
        }
    }


    public string GetDeckName()
    {
        return this.deckName;
    }

    public void SetDeckName(string name)
    {
        this.deckName = name;
    }

    public TablePlayer GetTablePlayer() {return tablePlayer;}

    [ContextMenu("Add Cards")]
    void AddCards()
    {
        this.cards = new List<Card>(GetComponentsInChildren<Card>());
    }
}
