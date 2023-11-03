using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Gamelogic logic;
    public TablePlayer player;
    public TablePlayer enemy;

    void Start()
    {
        //logic.init(this);
    }

    public void ClearSlots()
    {
        TablePlayer[] players = { player, enemy };
        foreach (TablePlayer p in players)
        {
            foreach(Slot slot in p.slots)
            {
                Card card = slot.card;
                slot.card = null;
                if (true) // TODO: Check that card is not an empty card
                {
                    p.discardpile.cards.Add(card);
                }
                // TODO: Play animation of card being removed
            }
        }
    }

    // TODO ------------------------------------------
    public void ResolveSlot(int slotNr, string smth)
    {
        // Play winning animation
    }

    public void SetWinner(string name)
    {
        
    }

    

}
