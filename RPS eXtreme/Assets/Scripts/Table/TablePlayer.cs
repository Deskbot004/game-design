using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class TablePlayer : MonoBehaviour, DefaultDroppable
{
    public Deck playerDeck;
    public Cardpile drawpile;
    public Cardpile discardpile;
    public Hand hand;
    public List<Slot> slots;
    public bool isPlayer;

    [Header("UI Connections")]
    public GameObject endTurnButton;
    public GameObject attachDoneButton;

    protected Table table;
    private bool dropActive = true;
    private NormalCard attachCard;

    // ---------- Main Functions ----------------------------------------------------------------------------------
    public virtual void init(Table table)
    {
        this.table = table;
        drawpile.SetCards(playerDeck.GetCards());
        drawpile.Shuffle();
        foreach (Card card in playerDeck.GetCards())
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

    // Shuffles all cards from the Discardpile into the Drawpile
    public void DiscardToDrawpile()
    {
        drawpile.GetCards().AddRange(discardpile.GetCards());
        discardpile.GetCards().Clear();
        drawpile.Shuffle();
        // TODO: Play animation
    }

    // ---------- Attaching Cards ----------------------------------------------------------------------------------

    public void startAttach(NormalCard baseCard)
    {
        if(attachCard != null)
        {
            hand.AddCard(attachCard);
            attachCard.GetComponent<SortingGroup>().sortingLayerName = "Cards on Table";
        }

        // Disable all other droppables
        foreach (Slot slot in slots) 
        {
            slot.DropActive = false;
        }

        table.dim.gameObject.SetActive(true);
        List<Card> cardsInHand = hand.GetCards();
        foreach (Card card in cardsInHand)
        {
            if(card == baseCard)
            {
                baseCard.transform.SetParent(transform);
                baseCard.transform.localPosition = new Vector3 (0f, 0f, 0f);
                baseCard.transform.SetParent(playerDeck.transform);
                baseCard.transform.eulerAngles = new Vector3(0, 0, 0);
                baseCard.GetComponent<SortingGroup>().sortingLayerName = "Cards in Focus";
                baseCard.GetComponent<Draggable>().enabled = false;
                baseCard.DropActive = true;
            }
            else if(card is NormalCard)
            {
                card.GetComponent<Draggable>().enabled = false;
            }
            else
            {
                card.GetComponent<SortingGroup>().sortingLayerName = "Cards in Focus";
            }
        }
        hand.RemoveCard(baseCard);
        hand.ArrangeHand();
        attachCard = baseCard;
        endTurnButton.SetActive(false);
        attachDoneButton.SetActive(true);
    }

    public void finishAttach()
    {
        foreach (Slot slot in slots) 
        {
            slot.DropActive = true;
        }
        table.dim.gameObject.SetActive(false);
        hand.AddCard(attachCard);
        attachCard.DropActive = false;
        foreach(Card card in hand.GetCards())
        {
            card.GetComponent<SortingGroup>().sortingLayerName = "Cards on Table";
            card.GetComponent<Draggable>().enabled = true;
        }
        hand.ArrangeHand();
        attachCard = null;
        endTurnButton.SetActive(true);
        attachDoneButton.SetActive(false);
    }

    //TODO: Allow empty slots on resolve

    public virtual IEnumerator playCards() { yield return new WaitForSeconds(this.table.GetCardMoveTime()); }

    // ---------- Droppable -------------------------------------------------------------------------------------
    public bool DropActive
    {
        get {return dropActive;}
        set {dropActive = value;}
    }
    
    public bool OnDrop(Draggable draggedObject)
    {
        hand.GetCards().Add(draggedObject.GetComponent<Card>());
        draggedObject.transform.localPosition = hand.transform.localPosition;
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
    public Table GetTable() { return table; }
    public Hand GetHand() { return hand; }
    public List<Cardpile> GetAllCardpiles() {
        List<Cardpile> allPiles  = new List<Cardpile> {drawpile, discardpile};
        return allPiles;
    }

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
