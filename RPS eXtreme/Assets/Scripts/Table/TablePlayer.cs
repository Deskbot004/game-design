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
    public GameObject detachButton;

    protected Table table;
    private bool dropActive = true;
    private NormalCard attachModeCardInFocus;

    // ---------- Main Functions ----------------------------------------------------------------------------------
    public virtual void init(Table table)
    {
        this.table = table;
        playerDeck.init(this);
        playerDeck.transform.localPosition = drawpile.transform.localPosition;
        drawpile.SetCards(playerDeck.GetCards());
        drawpile.Shuffle();
        foreach (Card card in playerDeck.GetCards())
        {
            card.gameObject.SetActive(false);
        }
        drawpile.init(this);
        discardpile.init(this);
        hand.init(this);
        foreach (Slot slot in slots)
        {
            slot.init(this);
        }
    }

    public void DrawCardsButton(int amount)
    {
        DrawCards(amount);
    }

    // Draws an amount of cards from the Drawpile into the Hand
    public List<Card> DrawCards(int amount, bool playAnimation = true)
    {
        // Not enough cards in the drawpile
        if (drawpile.GetCards().Count < amount)
        {
            int amountMissing = amount - drawpile.GetCards().Count;
            List<Card> allDrawnCards = new();
            List<Card> discardToDraw = new();
            discardToDraw.AddRange(discardpile.GetCards());
            allDrawnCards.AddRange(DrawCards(drawpile.GetCards().Count, false));
            DiscardToDrawpile();
            allDrawnCards.AddRange(DrawCards(amountMissing, false));
            hand.ArrangeHand(false);
            StartCoroutine(MoveCardsToDrawpileThenDraw(discardToDraw, allDrawnCards, 0.1f));
            return allDrawnCards;
        }

        // Draw abstract Cards
        List<Card> drawnCards = drawpile.GetCards().Take(amount).ToList();
        drawpile.SetCards(drawpile.GetCards().Except(drawnCards).ToList()); 
        hand.AddCards(drawnCards);

        // Add those Cards as GameObjects
        hand.ArrangeHand(false);
        if(playAnimation) StartCoroutine(DealCards(drawnCards, 0.3f));
        return drawnCards;
    }

// TODO: Avoid pressing multiple times right click on card in focus

    // Shuffles all cards from the Discardpile into the Drawpile
    public void DiscardToDrawpile()
    {
        drawpile.GetCards().AddRange(discardpile.GetCards());
        discardpile.GetCards().Clear();
        drawpile.Shuffle();
    }

    public virtual IEnumerator playCards() 
    {
        yield return new WaitForSeconds(0.3f); 
    }

    // ---------- Attaching Cards ----------------------------------------------------------------------------------
    // ----- Main Functions -----

    public void StartAttach(NormalCard baseCard)
    {
        OpenAttachMode();
        if(attachModeCardInFocus != null) 
            RemoveAttachFocusFrom(attachModeCardInFocus);
        SetAttachFocusOn(baseCard);
        endTurnButton.SetActive(false);
        attachDoneButton.SetActive(true);
        detachButton.SetActive(baseCard.HasAttachedCards());
    }

    public void DetachAllCards() 
    {
        List<SupportCard> supCards = attachModeCardInFocus.DetachAllCards();
        foreach (SupportCard supCard in supCards)
        {
            OnDrop(supCard.GetComponent<Draggable>());
            supCard.GetComponent<Draggable>().SetCurrentDroppable(this);
        }
    }

    public void FinishAttach()
    {
        detachButton.SetActive(false);
        attachDoneButton.SetActive(false);
        endTurnButton.SetActive(true);
        RemoveAttachFocusFrom(attachModeCardInFocus);
        CloseAttachMode();
    }

    // ----- Helper Functions -----

    public void OpenAttachMode()
    {
        foreach (Slot slot in slots) 
            slot.DropActive = false;
        table.dim.gameObject.SetActive(true);
        foreach (Card card in hand.GetCards())
        {
            if(card is NormalCard)
                card.GetComponent<Draggable>().enabled = false;
            else
                card.GetComponent<SortingGroup>().sortingLayerName = "Cards in Focus";
        }
    }

    public void CloseAttachMode()
    {
        foreach (Slot slot in slots) 
            slot.DropActive = true;
        table.dim.gameObject.SetActive(false);
        foreach (Card card in hand.GetCards())
        {
            if(card is NormalCard)
                card.GetComponent<Draggable>().enabled = true;
            else
                card.GetComponent<SortingGroup>().sortingLayerName = "Cards on Table";
        }
        //hand.ArrangeHand();
    }

    public void SetAttachFocusOn(NormalCard focusCard)
    {
        // Remove card from Hand
        hand.RemoveCard(focusCard);

        // Bring card to the middle of screen
        focusCard.transform.SetParent(transform);
        focusCard.SetWorldTargetPosition(transform.TransformPoint(new Vector3 (0, 0, 0)));
        focusCard.GetComponent<Card>().SetTargetRotation(new Vector3 (0, 0, 0));
        StartCoroutine(focusCard.MoveToTarget(0.1f));
        //focusCard.transform.localPosition = new Vector3 (0f, 0f, 0f);
        //focusCard.transform.eulerAngles = new Vector3(0, 0, 0);
        focusCard.transform.SetParent(playerDeck.transform);

        // Bring card in front of dim layer
        focusCard.GetComponent<SortingGroup>().sortingLayerName = "Cards in Focus";

        // Make the card a Droppable
        focusCard.DropActive = true;

        // Everything succeeded
        attachModeCardInFocus = focusCard;
        hand.ArrangeHand();
    }

    public void RemoveAttachFocusFrom(NormalCard focusCard)
    {
        // Add card back to Hand
        hand.AddCard(focusCard);

        // Put card back on proper layer
        focusCard.GetComponent<SortingGroup>().sortingLayerName = "Cards on Table";

        // Disable Droppable
        focusCard.DropActive = false;

        // Everything succeeded
        attachModeCardInFocus = null;
        hand.ArrangeHand(true, 0.3f);
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
        //draggedObject.transform.localPosition = hand.transform.localPosition;
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

    // ------ Animation -------------------------------------------------------------------
    protected virtual IEnumerator DealCards(List<Card> cards, float timeOffset)
    {
        hand.ArrangeHand(false);
        foreach (Card card in hand.GetCards())
        {  
            card.GetComponent<Draggable>().enabled = false;
            card.gameObject.SetActive(true);
            if(cards.Contains(card))
            {
                card.transform.position = drawpile.transform.position;
                card.GetComponent<Animator>().SetBool("faceFront", false);
                //card.GetComponent<Animator>().SetBool("flip", isPlayer);
                card.GetComponent<Animator>().SetBool("flip", true); // TODO: Change back to hide enemy cards
            }
            StartCoroutine(card.MoveToTarget(0.5f));
            float actualOffset = cards.Contains(card)? timeOffset : 0f;
            yield return new WaitForSeconds(actualOffset);
        }
        foreach (Card card in hand.GetCards()) card.GetComponent<Draggable>().enabled = true;
    }

    public IEnumerator DiscardCard(Card card)
    {
        card.GetComponent<Draggable>().enabled = false;
        card.SetWorldTargetPosition(discardpile.transform.TransformPoint(new Vector3(0,0,0)));
        card.GetComponent<Animator>().SetBool("faceFront", false);
        card.GetComponent<Animator>().SetBool("flip", true);
        yield return card.MoveToTarget(0.5f);
        card.gameObject.SetActive(false);
    }

    IEnumerator MoveCardsToDrawpileThenDraw(List<Card> shuffledCard, List<Card> drawnCards, float timeOffset)
    {
        foreach(Card card in shuffledCard)
        {   
            card.transform.position = discardpile.transform.position;
            card.transform.eulerAngles = Vector3.zero;
            card.gameObject.SetActive(true);
            card.GetComponent<Animator>().SetBool("faceFront", false);
            card.SetWorldTargetPosition(drawpile.transform.TransformPoint(Vector3.zero));
            card.SetTargetRotation(Vector3.zero);
            StartCoroutine(card.MoveToTarget(0.5f, false));
            yield return new WaitForSeconds(timeOffset);
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DealCards(drawnCards, 0.3f));
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
