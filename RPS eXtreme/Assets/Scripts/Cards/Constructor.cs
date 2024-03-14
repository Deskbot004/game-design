using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

// A class to generate new cards and decks using the prefabs
public class Constructor : MonoBehaviour
{
    public GameObject NormalCard;
    public GameObject SupportCard;
    public GameObject Deck;

    [Header("Setup CreateDeck")]
    public string deckName;
    [UDictionary.Split(30, 70)]
    public CardAmountDict cardAmounts;
    [Serializable]
    public class CardAmountDict : UDictionary<CardSymbol, int> { }

    private int counter;

    public NormalCard CreateNormalCard(CardSymbol symbol, (bool,bool) slotPositions) {
        GameObject cardObject = Instantiate(NormalCard, new Vector3(0, 0, 0), Quaternion.identity);
        cardObject.name = "Normal Card " + this.counter;
        cardObject.SetActive(false);
        NormalCard card = cardObject.GetComponent<NormalCard>();
        card.SetCardValues(symbol, slotPositions.Item1, slotPositions.Item2);

        this.counter++;
        return card;
    }

    public SupportCard CreateSupportCard((FunctionID, List<string>) function, (bool, bool) slotPositions, bool isPlayer) {
        GameObject cardObject = Instantiate(SupportCard, new Vector3(0, 0, 0), Quaternion.identity);
        cardObject.name = "Support Card " + this.counter;
        cardObject.SetActive(false);
        SupportCard card = cardObject.GetComponent<SupportCard>();
        card.SetCardValues(CardSymbol.SUPPORT, slotPositions.Item1, slotPositions.Item2, function, isPlayer);

        this.counter++;
        return card;
    }

    public Deck CreateDeck(List<Card> cards, string deckname) {
        GameObject deckObject = Instantiate(Deck, new Vector3(0, 0, 0), Quaternion.identity);
        Deck deck = deckObject.GetComponent<Deck>();
        deck.AddCards(cards);
        deck.SetDeckName(deckname);
        deck.gameObject.name = deckname;
        return deck;
    }

    #region Editor ------------------------------------------------------------------------------------------
    [ContextMenu("Create Deck for Player")]
    void CreateDeckPlayer() {
        this.counter = 0;
        List<Card> cards = new List<Card>();

        List<(bool,bool)> slotPositions = new() {(true, false), (false, true), (false, false), (true, true)};
        for (int i = 0; i < cardAmounts[CardSymbol.SUPPORT]; i++) {
            SupportCard card = CreateSupportCard((FunctionID.DRAW, new(){"2"}), slotPositions[i % 2], true);
            cards.Add(card);
        }
        for (int i = 0; i < cardAmounts[CardSymbol.ROCK]; i++) {
            NormalCard card = CreateNormalCard(CardSymbol.ROCK, slotPositions[i % 4]);
            cards.Add(card);
        }
        for (int i = 0; i < cardAmounts[CardSymbol.PAPER]; i++) {
            NormalCard card = CreateNormalCard(CardSymbol.PAPER, slotPositions[i % 4]);
            cards.Add(card);
        }
        for (int i = 0; i < cardAmounts[CardSymbol.SCISSORS]; i++) {
            NormalCard card = CreateNormalCard(CardSymbol.SCISSORS, slotPositions[i % 4]);
            cards.Add(card);
        }
        for (int i = 0; i < cardAmounts[CardSymbol.LIZARD]; i++) {
            NormalCard card = CreateNormalCard(CardSymbol.LIZARD, slotPositions[i % 4]);
            cards.Add(card);
        }
        for (int i = 0; i < cardAmounts[CardSymbol.SPOCK]; i++) {
            NormalCard card = CreateNormalCard(CardSymbol.SPOCK, slotPositions[i % 4]);
            cards.Add(card);
        }
        CreateDeck(cards, "Player Deck: " + this.deckName);
    }

    [ContextMenu("Create Deck for Enemy")]
    void CreateDeckEnemy() {
        this.counter = 0;
        List<Card> cards = new List<Card>();

        List<(bool,bool)> slotPositions = new() {(true, false), (false, true), (false, false), (true, true)};
        for (int i = 0; i < cardAmounts[CardSymbol.SUPPORT]; i++) {
            SupportCard card = CreateSupportCard((FunctionID.DRAW, new(){"2"}), slotPositions[i % 2], false);
            cards.Add(card);
        }
        for (int i = 0; i < cardAmounts[CardSymbol.ROCK]; i++) {
            NormalCard card = CreateNormalCard(CardSymbol.ROCK, slotPositions[i % 4]);
            cards.Add(card);
        }
        for (int i = 0; i < cardAmounts[CardSymbol.PAPER]; i++) {
            NormalCard card = CreateNormalCard(CardSymbol.PAPER, slotPositions[i % 4]);
            cards.Add(card);
        }
        for (int i = 0; i < cardAmounts[CardSymbol.SCISSORS]; i++) {
            NormalCard card = CreateNormalCard(CardSymbol.SCISSORS, slotPositions[i % 4]);
            cards.Add(card);
        }
        for (int i = 0; i < cardAmounts[CardSymbol.LIZARD]; i++) {
            NormalCard card = CreateNormalCard(CardSymbol.LIZARD, slotPositions[i % 4]);
            cards.Add(card);
        }
        for (int i = 0; i < cardAmounts[CardSymbol.SPOCK]; i++) {
            NormalCard card = CreateNormalCard(CardSymbol.SPOCK, slotPositions[i % 4]);
            cards.Add(card);
        }
        CreateDeck(cards, "Enemy Deck: " + this.deckName);
    }
    #endregion
}
