using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public int[] allCardNumbers;
    public int nrCards;
    public int[] undrawnCards; // TODO: Might not be needed, as TablePlayer handles that (?)
    public int[] discardPile; // TODO: See above

    public void schuffleDeck()
    {
        
    }

    public void schuffleDiscardIntoDeck()
    {

    }

    // For Debugging
    public List<Card> cards;

    [ContextMenu("Add Cards")]
    void addCards()
    {
        cards = new List<Card>(GetComponentsInChildren<Card>());
    }
}
