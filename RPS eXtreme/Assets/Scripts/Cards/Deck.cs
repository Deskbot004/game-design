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

    [TextArea(3,20)]
    public string flavor;
    private TablePlayer tablePlayer;
    [Header("[resourcing,right,rock,paper,scissors,random,support]")]
    public List<float> preferences;

    // ---------- Main Functions ------------------------------------------------------------------------------

    public void Awake()
    {
        if(GameObject.Find("Constructor") != null)
        {
            this.constructor = GameObject.Find("Constructor").GetComponent<Constructor>();
        }
    }

    public void init(TablePlayer tablePlayer)
    {
        this.tablePlayer = tablePlayer;
        this.LoadDeck(this.deckName);
        foreach (Card card in cards)
        {
            card.gameObject.SetActive(false);
            card.init(this);
        }
        if(!this.tablePlayer.isPlayer){
            Opponent enemy = (Opponent) this.tablePlayer;
            enemy.SetPreferences(this.preferences);
        }
    }

    public void AddCard(Card card)
    {
        this.cards.Add(card);
        card.gameObject.transform.SetParent(gameObject.transform);
    }

    public void AddCardDeck(List<Card> deck)
    {
        this.cards = deck;
        foreach (Card card in this.cards)
        {
            card.gameObject.transform.SetParent(gameObject.transform);
        }
    }

    /*
    * Saves the Deck into a text file using Json and a DeckManager.
    */
    [ContextMenu("Save Deck")]
    public void SaveDeck()
    {
        string filename_location = Path.Combine(Application.persistentDataPath, this.deckName);
        DeckManager save = new DeckManager();
        for (int i = 0; i < this.cards.Count; i++)
        {
            if (this.cards[i].IsBasic()) // card is a NormalCard
            {
                NormalCard card = (NormalCard)this.cards[i];
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
                foreach (string name in functionnames)
                {
                    if (name != functionnames[0])
                    {
                        functionString += ";" + name;
                    }
                }
                save.functions.Add(functionString);
                save.slotTypes.Add(card.GetSlotType());
            }
        }
        save.preferences = this.preferences;
        string savedDeck = JsonUtility.ToJson(save);

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
        this.preferences = save.preferences;
        this.deckName = filename;
    }

    /*
    * Loads the Deck saved under its deckname from a text file using Json and a DeckManager.
    */
    [ContextMenu("Load Deck")]
    public void LoadDeckFromEditor()
    {
        //Read text from file and convert it into a DeckManager
        this.constructor = GameObject.Find("Constructor").GetComponent<Constructor>();
        string filename = this.deckName;
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
            DestroyImmediate(card.gameObject);
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
        this.deckName = filename;
    }

    // ---------- Getter & Setter ------------------------------------------------------------------------------

    public Constructor GetConstructor(){return this.constructor;}

    public void SetConstructor(Constructor constructor){this.constructor = constructor;}

    public List<Card> GetCards(){return this.cards;}

    public string GetDeckName(){return this.deckName;}

    public void SetDeckName(string name){this.deckName = name;}

    public TablePlayer GetTablePlayer() {return tablePlayer;}

    // ---------- For Debugging --------------------------------------------------------------------------------

    [ContextMenu("Add Cards")]
    void AddCards()
    {
        this.cards = new List<Card>(GetComponentsInChildren<Card>());
    }

    [ContextMenu("Reset Card Positions")]
    void resetCardPositions()
    {
        foreach (Card card in cards)
        {
            card.transform.localPosition = new Vector3(0f,0f,0f);
            card.transform.eulerAngles = new Vector3(0f,0f,0f);
        }
    }
}
