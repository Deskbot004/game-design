using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Table : MonoBehaviour
{
    public Gamelogic logic;
    public TablePlayer player;
    public TablePlayer enemy;

    void Start()
    {
        player.init();
        enemy.init();
        //logic.init(this);
    }

    // Removes the card from every slot and puts them into the Discard Pile
    public void ClearSlots()
    {
        Debug.Log("Clearing Slots...");
        TablePlayer[] players = { player, enemy };
        foreach (TablePlayer p in players)
        {
            // TODO: �berarbeiten, wenn Karten kombinieren implementiert ist (card -> cards)
            foreach (Slot slot in p.slots)
            {
                Card card = slot.GetCard();
                slot.ClearCard();
                if (true) // TODO: Check that card is not an empty card
                {
                    p.discardpile.GetCards().Add(card);
                }
                card.gameObject.SetActive(false);
                // TODO: Play animation of card being removed
                // Something like card.playAnimation
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

    // ------ Getter und Setter -------------------------------------------------------------------
    public List<Slot> GetSlotsPlayer() { return player.GetSlots(); }
    public List<Slot> GetSlotsEnemy() { return enemy.GetSlots(); }

    // ---------- For Debbuging -----------------------------------------------------------
    public void startLogic()
    {
        logic.init(this);
    }

}
