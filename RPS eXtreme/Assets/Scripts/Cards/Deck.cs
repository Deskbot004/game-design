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
    public GameObject Constructor;
    public List<Card> cards = new List<Card>();
    private string deckName;

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
                string functionString = ""; //TODO: Convert functions into a string 
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
            Card card = cards[i - 1];
            cards.RemoveAt(i - 1);
            Destroy(card.gameObject);
        }

        //Add new Cards
        for (int i = 0; i < save.cardSymbols.Count; i++)
        {
            if (save.cardTypes[i] == 0) // saved Card at index i was a NormalCard
            {
                GameObject cardObject = Instantiate(NormalCard, new Vector3(0, 0, 0), Quaternion.identity);
                NormalCard card = cardObject.GetComponent<NormalCard>();
                card.SetSymbol(save.cardSymbols[i]);
                card.SetSlotType(save.slotTypes[i]);
                this.AddCard(card);
            }
            else
            {
                GameObject cardObject = Instantiate(SupportCard, new Vector3(0, 0, 0), Quaternion.identity);
                SupportCard card = cardObject.GetComponent<SupportCard>();
                card.SetSymbol(save.cardSymbols[i]);
                card.SetSlotType(save.slotTypes[i]);
                //TODO: convert string back into functions
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

    [ContextMenu("Add Cards")]
    void AddCards()
    {
        this.cards = new List<Card>(GetComponentsInChildren<Card>());
    }
}
