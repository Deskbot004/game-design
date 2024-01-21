using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Cardpile : MonoBehaviour
{
    public GameObject scrollview; // Scrollview object to be activated when opening the cardpile
    public Transform content; // Content object of the Scrollview

    [Header("Visual Stuff")]
    public Color baseColor;
    public Color hoverColor;
    public Color clickColor;

    private List<Card> sortedCards = new List<Card>(); // Cards shown in the Scrollview
    private bool open = false; // is the scrollview open?

    private TablePlayer tablePlayer;
    private List<Card> cards = new List<Card>(); // Cards currently in the pile


    
    public void init(TablePlayer tablePlayer)
    {
        this.tablePlayer = tablePlayer;
    }

    // Randomizes the order of the cards
    public void Shuffle()
    {
        System.Random rng = new System.Random();
        cards = cards.OrderBy(x => rng.Next()).ToList();
    }

    // Shows all the cards in the pile on screen
    public void openPile()
    {
        this.GetComponent<AudioSource>().Play();
        foreach(Cardpile pile in tablePlayer.GetAllCardpiles())
        {
            if(pile.isOpen()) pile.closePile();
        }
        scrollview.SetActive(true);

        // Create a new temporary sorted list of cards in drawpile
        sortedCards.Clear();
        sortedCards.AddRange(cards);
        // TODO: Sort cards by some criteria

        float cardRows = (float) Math.Ceiling(sortedCards.Count/5.0);
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, cardRows * 90);
        foreach (Card card in sortedCards)
        {
            card.transform.SetParent(content);
            card.GetComponent<SortingGroup>().sortingLayerName = "UI";
            card.gameObject.SetActive(true);
            card.GetComponent<Animator>().SetBool("faceFront", true);
            card.GetComponent<Draggable>().enabled = false;
        }
        foreach (Card card in tablePlayer.GetHand().GetCards())
        {
            card.GetComponent<Draggable>().enabled = false;
        }
        // TODO: Also disable draggable for cards in hand
        open = true;
    }

    public void closePile()
    {
        scrollview.SetActive(false);
        foreach (Card card in sortedCards)
        {
            card.transform.SetParent(card.GetDeck().transform);
            card.GetComponent<SortingGroup>().sortingLayerName = "Cards on Table";
            card.gameObject.SetActive(false);
            card.GetComponent<Draggable>().enabled = true;
        }
        foreach (Card card in tablePlayer.GetHand().GetCards())
        {
            card.GetComponent<Draggable>().enabled = true;
        }
        open = false;
    }

    // TODO: Animation
    // ------ Animation Stuff -------------------------------------------------------------------
    // On Mouse Hover darkens the deck
    void OnMouseEnter()
    {
        if(tablePlayer.isPlayer)
            GetComponent<SpriteRenderer>().color = hoverColor;
    }

    void OnMouseExit()
    {
        //GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
        if(tablePlayer.isPlayer)
            GetComponent<SpriteRenderer>().color = baseColor;
    }

    void OnMouseDown()
    {
        if(tablePlayer.isPlayer)
            GetComponent<SpriteRenderer>().color = clickColor;
    }

    // Displays the cards on the screen
    void OnMouseUpAsButton()
    {
        if(tablePlayer.isPlayer)
        {
            if (!open) openPile();
            else closePile();
            GetComponent<SpriteRenderer>().color = hoverColor;
        }
        
    }

    // ------ Getter und Setter -------------------------------------------------------------------
    public List<Card> GetCards() { return cards; }
    public bool isOpen() {return open;}

    public void SetCards(List<Card> newCards) //Copies cards, doesn't set pointer of list
    { 
        cards.Clear();
        cards.AddRange(newCards);
        foreach (Card card in newCards) 
        {
            card.SetStatus(0);
        }
    }
    public TablePlayer GetTablePlayer() {return tablePlayer;}
}
