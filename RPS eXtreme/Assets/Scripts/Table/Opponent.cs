using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : TablePlayer
{
    public override void init(Table table)
    {
        base.init(table);
        this.isPlayer = false;
    }

    public override IEnumerator playCards()
    {
        Debug.Log("Opponent playing Cards");
        int i = 0;
        List<Card> playedCards = new List<Card>();
        foreach(Card card in this.hand.GetCards())
        {
            Debug.Log("slot " + i + " of " + this.slots.Count);
            if(i >= this.slots.Count)
            {
                break;
            }
            if(playedCards.Contains(card)) //Card has been played already (probably together with a support Card)
            { 
                continue;
            }
            if (!card.IsBasic())
            {
                NormalCard normal = playSupportCard(card,i,playedCards);
                if(normal != null)
                {
                    yield return new WaitForSeconds(1);
                    card.transform.localPosition = new Vector3(0,0,0.5f);
                    StartCoroutine(normal.MoveToTarget(1));
                    i++;
                }
                continue;
            }

            if (playNormalCard(card, i, playedCards) == 0) // Normal Card was played in slot i
            {
                i++;
            }
        }
        foreach(Card card in playedCards)
        {
            this.hand.RemoveCard(card);
        }
        this.hand.ArrangeHand();
        yield return null;
    }

    public int playNormalCard(Card card, int slot, List<Card> playedCards)
    {
        Debug.Log("playing Normal Card with symbol " + card.GetSymbol());
        if (this.slots[slot].GetNormalAndSuppCards().Count == 0) 
        {
            NormalCard norm = (NormalCard)card;
            this.slots[slot].SetCard(norm);
            playedCards.Add(card);
            card.SetWorldTargetPosition(slots[slot].transform.TransformPoint(Vector3.zero));
            card.SetTargetRotation(Vector3.zero);
            StartCoroutine(card.MoveToTarget(1));
            return 0;
        }
        else
        {
            return 1;
        }
    }

    public NormalCard playSupportCard(Card card, int slot, List<Card> playedCards)
    {
        SupportCard support = (SupportCard)card;
        foreach (Card card2 in this.hand.GetCards())
        {
            if (playedCards.Contains(card))
            { //Card has been played already (probably together with a support Card)
                continue;
            }
            if (card2.IsBasic())
            {
                NormalCard normal = (NormalCard)card2;
                if (normal.OnDrop(support.GetComponent<Draggable>()) && this.slots[slot].GetNormalAndSuppCards().Count == 0) //Attach the support Card to the basic card and play the basic Card into the slot
                {
                    Debug.Log("Normal to be attached to Card found with symbol " + normal.GetSymbol());
                    Debug.Log(normal.GetSlotType());
                    Debug.Log(support.GetSlotType());
                    this.slots[slot].SetCard(normal);
                    playedCards.Add(normal);
                    playedCards.Add(support);
                    normal.SetWorldTargetPosition(slots[slot].transform.TransformPoint(Vector3.zero));
                    normal.SetTargetRotation(Vector3.zero);
                    return normal;
                }
            }
        }
        return null;
    }

    protected override IEnumerator DealCards(List<Card> cards, float timeOffset)
    {
        yield return base.DealCards(cards, timeOffset);
        yield return new WaitForSeconds(1);
        StartCoroutine(playCards());
        yield return null;
    }
}
