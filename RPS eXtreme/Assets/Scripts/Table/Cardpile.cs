using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cardpile : MonoBehaviour
{
    public GameObject openedPile;
    public Collider2D background; // Background of the opened card pile
    public double outerMargin; // Margin between cards and edge of background
    public double cardMargin; // Margin between cards

    private List<Card> cards = new List<Card>(); // Cards currently in the pile
    private Vector3 cardSize; // size of the Collider of a Card

    private Vector3 backgroundSize;
    private int cardsPerRow;
    private List<Card> sortedCards = new List<Card>();
    private bool open = false;

    public void Awake()
    {
        openedPile.SetActive(true);
        backgroundSize = background.bounds.size; // To get the background size, the background needs to be enabled
        openedPile.SetActive(false);
    }

    public void init()
    {
        // Calculate cardsPerRow
        double outerWidth = backgroundSize.x;
        double innerWidth = backgroundSize.x - 2 * outerMargin; // Width without outer margin
        cardsPerRow = 1 + Mathf.FloorToInt((float)((innerWidth - cardSize.x) / (cardSize.x + cardMargin)));
        double actualWidth = cardsPerRow * cardSize.x + (cardsPerRow - 1) * cardMargin;
        outerMargin = (outerWidth - actualWidth) / 2;
    }

    // Randomizes the order of the cards
    public void Shuffle()
    {
        System.Random rng = new System.Random();
        cards = cards.OrderBy(x => rng.Next()).ToList();
    }

    // TODO: Überarbeiten, sobald wir die Assets haben
    // Shows all the cards in the pile on screen
    public void openPile()
    {
        // TODO: Close all other opened Cardpiles
        openedPile.SetActive(true);

        // Create a new temporary sorted list of cards in drawpile
        sortedCards.Clear();
        sortedCards.AddRange(cards);
        // TODO: Sort cards by some criteria

        int currentCard = 0;
        double startX = background.bounds.min.x + outerMargin + cardSize.x / 2; // world
        double startY = background.bounds.max.y - outerMargin - cardSize.y / 2; // local
        foreach (Card card in sortedCards)
        {
            double x = startX + (currentCard % cardsPerRow) * (cardSize.x + cardMargin);
            double y = startY - Mathf.Floor(currentCard / cardsPerRow) * (cardSize.y + cardMargin);
            Vector3 localCardPosition = new Vector3((float) x, (float) y, -5);
            card.transform.eulerAngles = new Vector3(0, 0, 0);
            card.transform.localPosition = localCardPosition;
            card.gameObject.SetActive(true);
            card.GetComponent<Draggable>().enabled = false;
            currentCard++;
        }
        open = true;
    }

    public void closePile()
    {
        openedPile.SetActive(false);
        foreach (Card card in sortedCards)
        {
            card.gameObject.SetActive(false);
            card.GetComponent<Draggable>().enabled = true;
        }
        open = false;
    }

    // TODO: Animation
    // ------ Animation Stuff -------------------------------------------------------------------
    // On Mouse Hover darkens the deck
    void OnMouseEnter()
    {
        //GetComponent<SpriteRenderer>().color = new Color(204, 204, 204, 255);
        GetComponent<SpriteRenderer>().color = Color.gray;
    }

    void OnMouseExit()
    {
        //GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void OnMouseDown()
    {
        GetComponent<SpriteRenderer>().color = Color.black;
    }

    // Displays the cards on the screen
    void OnMouseUpAsButton()
    {
        if (!open) openPile();
        else closePile();
        GetComponent<SpriteRenderer>().color = Color.gray;
    }

    // ------ Getter und Setter -------------------------------------------------------------------
    public List<Card> GetCards() { return cards; }
    public Vector3 GetCardSize() { return cardSize; }

    public void SetCards(List<Card> newCards) //Copies cards, doesn't set pointer of list
    { 
        cards.Clear();
        cards.AddRange(newCards);
    }
    public void SetCardSize(Vector3 newSize) { cardSize = newSize; }
}
