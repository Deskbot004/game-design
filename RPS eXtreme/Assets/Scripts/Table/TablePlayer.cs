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

    protected Table table;
    private bool dropActive = true;

    // ---------- Main Functions ----------------------------------------------------------------------------------
    void Awake()
    {
        Card card = playerDeck.GetCards()[0];
        card.gameObject.SetActive(true);
        drawpile.SetCardSize(card.GetComponent<BoxCollider2D>().bounds.size);
        discardpile.SetCardSize(card.GetComponent<BoxCollider2D>().bounds.size);
    }
    
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
            //Debug.Log(card.gameObject + ": " + card.transform.localPosition);
            if(card == baseCard)
            {
                //card.GetComponent<Draggable>().SavePosition();
                baseCard.transform.localPosition = new Vector3 (9f, 2.5f, -3.6f);
                baseCard.transform.eulerAngles = new Vector3(0, 0, 0);
                baseCard.GetComponent<Draggable>().enabled = false;
                baseCard.DropActive = true;
            }
            else if(card is NormalCard)
            {
                card.GetComponent<Draggable>().enabled = false;
            }
            else
            {
                Vector3 oldPosition = card.transform.localPosition;
                oldPosition.z = -3.5f;
                card.transform.localPosition = oldPosition;
            }
            //Later: card.GetComponent<Draggable>().RestorePosition();
        }
        hand.RemoveCard(baseCard);
        hand.ArrangeHand();
    }

    // Shuffles all cards from the Discardpile into the Drawpile
    public void DiscardToDrawpile()
    {
        drawpile.GetCards().AddRange(discardpile.GetCards());
        discardpile.GetCards().Clear();
        drawpile.Shuffle();
        // TODO: Play animation
    }

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
    public Table GetTable() {return table;}

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
