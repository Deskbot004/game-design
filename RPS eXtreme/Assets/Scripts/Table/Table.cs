using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class Table : MonoBehaviour
{
    public Gamelogic logic;
    public TablePlayer player;
    public TablePlayer enemy;

    [Header("Visual Stuff")]
    public SpriteRenderer dim;

    private float cardMoveTime = 0.5f;

    void Start()
    {
        player.init(this);
        enemy.init(this);
        logic.init(this);
    }

    // Removes the card from every slot and puts them into the Discard Pile
    public void ClearSlots()
    {
        TablePlayer[] players = { player, enemy };
        foreach (TablePlayer p in players)
        {
            foreach (Slot slot in p.slots)
            {
                NormalCard card = slot.GetCard();
                slot.ClearCard();
                if (card != null)
                {
                    // TODO: Play animation of card being removed
                    List<SupportCard> supCards = card.DetachAllCards();
                    p.discardpile.GetCards().Add(card);
                    card.gameObject.SetActive(false);
                    foreach(SupportCard supCard in supCards)
                    {
                        p.discardpile.GetCards().Add(supCard);
                        supCard.gameObject.SetActive(false);
                    }
                }

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
        // TODO End Screen
        DataBetweenScreens.playerWon = name == "user";
        SceneManager.LoadScene("WinLoseScreen");
    }

    public List<Slot> GetSlotsPlayer() { return player.GetSlots(); }
    public List<Slot> GetSlotsEnemy() { return enemy.GetSlots(); }
    public Gamelogic GetGamelogic() { return this.logic; }
    public float GetCardMoveTime() { return this.cardMoveTime; }
}
