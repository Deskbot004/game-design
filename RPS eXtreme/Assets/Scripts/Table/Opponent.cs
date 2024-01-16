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
            if(i >= this.slots.Count)
            {
                break;
            }
            if (!card.IsBasic())
            {
                continue;
            }
            if(this.slots[i].GetCards().Count == 0)
            {
                this.slots[i].SetCards(card);
                playedCards.Add(card);
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
}
