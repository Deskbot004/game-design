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
        Debug.Log("Opponent is playing cards");
        int i = 0;
        List<Card> playedCards = new List<Card>();
        foreach(Card card in this.hand.GetCards())
        {
            Debug.Log("slot " + i + " of " + this.slots.Count);
            if(i >= this.slots.Count)
            {
                break;
            }
            if(playedCards.Contains(card)){ //Card has been played already (probably together with a support Card)
                continue;
            }
            if (!card.IsBasic())
            {
                if(playSupportCard(card,i,playedCards) == 0)
                {
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
        yield return new WaitForSeconds(this.table.GetCardMoveTime());
        this.hand.ArrangeHand();
    }

    public int playNormalCard(Card card, int slot, List<Card> playedCards)
    {
        Debug.Log("playing Normal Card");
        if (this.slots[slot].GetCards().Count == 0) 
        {
            this.slots[slot].SetCards(card);
            playedCards.Add(card);
            return 0;
        }
        else
        {
            return 1;
        }
    }

    public int playSupportCard(Card card, int slot, List<Card> playedCards)
    {
        Debug.Log("playing Support Card");
        SupportCard support = (SupportCard)card;
        foreach (Card card2 in this.hand.GetCards())
        {
            if (card2.IsBasic())
            {
                NormalCard normal = (NormalCard)card2;
                if (normal.AttachSupportCard(support) == 0 && this.slots[slot].GetCards().Count == 0) //Attach the support Card to the basic card and play the basic Card into the slot
                {
                    Debug.Log("Attached Normal Card found");
                    List<Card> slotCards = new List<Card>();
                    slotCards.Add(normal);
                    slotCards.Add(support);
                    this.slots[slot].SetCards(slotCards);
                    playedCards.Add(normal);
                    return 0;
                }
            }
        }
        return 1;
    }
}
