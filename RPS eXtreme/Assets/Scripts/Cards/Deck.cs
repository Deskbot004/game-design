using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Assertions;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

[Serializable]
public class Deck : MonoBehaviour
{
    public string deckName;
    [TextArea(3,20)] public string flavor;
    public List<Card> cards = new List<Card>();

    private Constructor constructor; // TODO: Change constructor to Component on Deck
    private TableSide tableSide;

    [Header("[rock,paper,scissors,lizard,spock,resourcing,right,random,support]")] // TODO: Nein :(
    public List<(EnemyPrefs, float)> preferences;

    public void Awake() { //TODO Constructor: Remove?
        if(GameObject.Find("Constructor") != null) {
            this.constructor = GameObject.Find("Constructor").GetComponent<Constructor>();
        }
    }

    #region Main Functions ----------------------------------------------------------------------------------
    public void InitAndReloadCards(TableSide tableSide) {
        this.tableSide = tableSide;
        transform.position = tableSide.drawpile.transform.position;
        ResetCardPositions();
        LoadDeck(this.deckName);
        foreach (Card card in cards) {
            card.gameObject.SetActive(false);
            card.Init(this);
        }
        if(!this.tableSide.isPlayer) {
            Enemy enemy = (Enemy) this.tableSide;
            enemy.SetPreferences(this.preferences); // TODO: Change
        }
    }
    #endregion

    #region Saving & Loading --------------------------------------------------------------------------------
    [ContextMenu("Save Deck")]
    public void SaveDeck() {
        Regex invalidSymbols = new("[/\\:*?\"<>|]");
        deckName = invalidSymbols.Replace(deckName, "");
        string filepath = Path.Combine(Application.persistentDataPath, deckName + ".json");
        DeckManager deckToSave = new();
        foreach (Card card in cards) {
            deckToSave.AddCardToSave(card);
        }
        deckToSave.preferences = this.preferences;
        string deckJson = JsonConvert.SerializeObject(deckToSave, Formatting.Indented);
        //string deckJson = JsonUtility.ToJson(deckToSave);
        File.WriteAllText(filepath, deckJson);
        Debug.Log("Saved successfully to " + filepath);
    }

    [ContextMenu("Load Deck")]
    public void LoadDeckFromEditor() {
        this.constructor = GameObject.Find("Constructor").GetComponent<Constructor>();
        Regex invalidSymbols = new("[/\\:*?\"<>|]");
        deckName = invalidSymbols.Replace(deckName, "");
        LoadDeck(deckName, true);
        Debug.Log("Loaded Deck: " + deckName);
    }

    public void LoadDeck(string filename, bool fromEditor = false) {
        string filepath = Path.Combine(Application.persistentDataPath, filename + ".json");
        DeckManager loadedDeck = LoadDeckManager(filepath);

        this.preferences = loadedDeck.preferences;
        this.deckName = filename;
        if(fromEditor)
            DeleteCardObjectsfromEditor();
        else
            DeleteCardObjects();
        CreateCards(loadedDeck);
    }

    public DeckManager LoadDeckManager(string filepath) {
        Debug.Assert(File.Exists(filepath), "Error loading deck: file (" + filepath + ") doesn't exist", this);
        //DeckManager loadedDeck = JsonUtility.FromJson<DeckManager>(File.ReadAllText(filepath));
        DeckManager loadedDeck = JsonConvert.DeserializeObject<DeckManager>(File.ReadAllText(filepath));
        Debug.Assert(loadedDeck.AreListsSameLength(), "Error loading deck: lists in DeckManager have wrong sizes", this);
        return loadedDeck;
    }

    public void DeleteCardObjects() {
        for (int i = this.cards.Count; i > 0; i--) {
            Card card = this.cards[i - 1];
            this.cards.RemoveAt(i - 1);
            Destroy(card.gameObject);
        }
    }
    
    public void DeleteCardObjectsfromEditor() {
        for (int i = this.cards.Count; i > 0; i--) {
            Card card = this.cards[i - 1];
            this.cards.RemoveAt(i - 1);
            DestroyImmediate(card.gameObject);
        }
    }

    public void CreateCards(DeckManager loadedDeck) {
        for (int i = 0; i < loadedDeck.cardAmount; i++) {
            if (loadedDeck.cardSymbols[i] == CardSymbol.SUPPORT) {
                SupportCard card = this.constructor.CreateSupportCard(loadedDeck.functions[i], loadedDeck.slotPositions[i], tableSide.isPlayer);
                this.AddCard(card);
            } else {
                NormalCard card = this.constructor.CreateNormalCard(loadedDeck.cardSymbols[i], loadedDeck.slotPositions[i]);
                this.AddCard(card);
            }  
        }
    }
    #endregion

    #region Shorthands --------------------------------------------------------------------------------------
    public void AddCard(Card card) {
        this.cards.Add(card);
        card.gameObject.transform.SetParent(gameObject.transform);
    }

    public void AddCards(List<Card> cards) {
        foreach (Card card in cards) {
            AddCard(card);
        }
    }

    public List<Card> GetCards() {
        return this.cards;
    }

    public TableSide GetTableSide() {
        return tableSide;
    }

    public void SetDeckName(string name) {
        this.deckName = name;
    } // TODO Constructor: Needed?

    
    #endregion
    
    #region Debugging ---------------------------------------------------------------------------------------
    [ContextMenu("Add Cards")]
    void AddCards() {
        this.cards = new List<Card>(GetComponentsInChildren<Card>());
    }

    [ContextMenu("Reset Card Positions")]
    void ResetCardPositions() {
        foreach (Card card in cards) {
            card.transform.localPosition = new Vector3(0f,0f,0f);
            card.transform.eulerAngles = new Vector3(0f,0f,0f);
        }
    }
    #endregion
}
