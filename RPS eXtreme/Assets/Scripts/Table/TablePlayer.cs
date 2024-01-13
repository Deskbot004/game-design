using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TablePlayer : MonoBehaviour, DefaultDroppable
{
    public Deck playerDeck;
    public Cardpile drawpile;
    public Cardpile discardpile;
    public Hand hand;
    public List<Slot> slots;
    public bool isPlayer;

    private Table table;
    private bool dropActive = true;

    // ---------- Main Functions ----------------------------------------------------------------------------------
    void Awake()
    {
        Card card = playerDeck.GetCards()[0];
        card.gameObject.SetActive(true);
        drawpile.SetCardSize(card.GetComponent<BoxCollider2D>().bounds.size);
        discardpile.SetCardSize(card.GetComponent<BoxCollider2D>().bounds.size);
    }
    
    public void init(Table table)
    {
        this.table = table;
        drawpile.SetCards(playerDeck.cards);
        drawpile.Shuffle();
        foreach (Card card in playerDeck.cards)
        {
            card.gameObject.SetActive(false);
        }
        playerDeck.init(this);
        drawpile.init(this);
        discardpile.init(this);
        hand.init(this);
        foreach (Slot slot in slots)
        {
            slot.init(this);
        }
    }

    // Draws an amount of cards from the Drawpile into the Hand
    public void DrawCards(int amount)
    {
        // Not enough cards in the drawpile
        if (drawpile.GetCards().Count < amount)
        {
            int amountMissing = amount - drawpile.GetCards().Count;
            DrawCards(drawpile.GetCards().Count);
            DiscardToDrawpile();
            DrawCards(amountMissing);
            return;
        }

        // Draw abstract Cards
        List<Card> drawnCards = drawpile.GetCards().Take(amount).ToList();
        drawpile.SetCards(drawpile.GetCards().Except(drawnCards).ToList()); 
        hand.AddCards(drawnCards);

        // Add those Cards as GameObjects
        hand.ArrangeHand();
        foreach (Card card in drawnCards)
        {
            // TODO: Play Animation
            card.gameObject.SetActive(true);
        }
    }

    public void startAttach(NormalCard baseCard)
    {
        // Disable all other droppables
        foreach (Slot slot in slots) 
        {
            slot.DropActive = false;
        }

        table.dim.gameObject.SetActive(true);
        List<Card> cardsInHand = hand.GetCards();
        foreach (Card card in cardsInHand)
        {
            card.transform.localPosition += new Vector3 (0, 0, -3.5f);
            if(card == baseCard)
            {
                //card.GetComponent<Draggable>().SavePosition();
                baseCard.transform.localPosition = new Vector3 (9f, 2.5f, -3.6f);
                baseCard.transform.eulerAngles = new Vector3(0, 0, 0);
                baseCard.GetComponent<Draggable>().enabled = false;
                baseCard.DropActive = true;
                hand.RemoveCard(baseCard);
                hand.ArrangeHand();
            }
            else if(card is NormalCard)
            {
                card.GetComponent<SpriteRenderer>().color = Color.black;
                card.GetComponent<Draggable>().enabled = false;
            }
            //Later: card.GetComponent<Draggable>().RestorePosition();
        }
    }

    // Shuffles all cards from the Discardpile into the Drawpile
    public void DiscardToDrawpile()
    {
        drawpile.GetCards().AddRange(discardpile.GetCards());
        discardpile.GetCards().Clear();
        drawpile.Shuffle();
        // TODO: Play animation
    }

    // ---------- Droppable -------------------------------------------------------------------------------------
    public bool DropActive
    {
        get {return dropActive;}
        set {dropActive = value;}
    }
    
    public bool OnDrop(Draggable draggedObject)
    {
        hand.GetCards().Add(draggedObject.GetComponent<Card>());
        hand.ArrangeHand();
        return true;
    }

    public void OnLeave(Draggable draggedObject)
    {
        hand.RemoveCard(draggedObject.GetComponent<Card>());
        hand.ArrangeHand();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    // ------ Getter und Setter -------------------------------------------------------------------
    public List<Slot> GetSlots() { return slots; }

    // ---------- For Debugging --------------------------------------------------------------------------------
    public void fakeResolve()
    {
        foreach (Slot slot in slots)
        {
            discardpile.GetCards().Add(slot.GetCard());
            slot.GetCard().gameObject.SetActive(false);
            slot.ClearCard();
        }
    }

    
}
