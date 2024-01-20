using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Table : MonoBehaviour
{
    public Gamelogic logic;
    public TablePlayer player;
    public TablePlayer enemy;

    [Header("Visual Stuff")]
    public SpriteRenderer dim;
    public Health healthUI;

    [Header("Debugging")]
    public bool quickResolve;
    public float waitTimer = -1;

    private float cardMoveTime = 0.5f;

    void Start()
    {
        if (waitTimer < 0) waitTimer = quickResolve? 0.3f : 1f;
        player.init(this);
        enemy.init(this);
        logic.init(this);
    }

    public float TurnEnemySlotCards()
    {
        float animationLength = 1f;
        foreach (Slot slot in enemy.GetSlots())
        {
            if (slot.GetCard() != null)
            {
                slot.GetCard().GetComponent<Animator>().SetBool("flip", true);
                animationLength = slot.GetCard().GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
            }
        }
        return animationLength;
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
                    StartCoroutine(p.DiscardCard(card));
                    foreach(SupportCard supCard in supCards)
                    {
                        p.discardpile.GetCards().Add(supCard);
                        StartCoroutine(p.DiscardCard(supCard));
                    }
                }

            }
        }
    }

    //TODO: Fix when no cards in slot
    // TODO: Set all waitforseconds higher
    public IEnumerator ResolveSlot(int slotNr, string winner, IDictionary<string, int> lifePoints)
    {
        Dictionary<string, Slot> slots = GetSlotByNr(slotNr);
        foreach (Slot slot in slots.Values)
        {
            slot.GetCard().GetComponent<SortingGroup>().sortingLayerName = "Cards in Focus";
        }
        yield return new WaitForSeconds(waitTimer);
        if(winner == "user")
        {
            slots["enemy"].GetCard().GetComponent<SortingGroup>().sortingLayerName = "Cards on Table";
            healthUI.Damage(lifePoints["enemy"], "enemy");
        }
        else if (winner == "enemy")
        {
            slots["user"].GetCard().GetComponent<SortingGroup>().sortingLayerName = "Cards on Table";
            healthUI.Damage(lifePoints["user"], "user");
        }
        yield return new WaitForSeconds(waitTimer);
        foreach (Slot slot in slots.Values)
        {
            slot.GetCard().GetComponent<SortingGroup>().sortingLayerName = "Cards on Table";
        }
    }

    public void SetWinner(string name)
    {
        // TODO Add Fade
        DataBetweenScreens.playerWon = name == "user";
        SceneManager.LoadScene("WinLoseScreen");
    }

    public List<Slot> GetSlotsPlayer() { return player.GetSlots(); }
    public List<Slot> GetSlotsEnemy() { return enemy.GetSlots(); }
    public Dictionary<string, Slot> GetSlotByNr(int slotNr)
    {
        Dictionary<string, Slot> slots = new();
        slots["user"] = player.GetSlots().Where(x => x.slotPosition == slotNr).ToList()[0];
        slots["enemy"] = enemy.GetSlots().Where(x => x.slotPosition == slotNr).ToList()[0];
        return slots;
    }
    public Gamelogic GetGamelogic() { return this.logic; }
    public float GetCardMoveTime() { return this.cardMoveTime; }
}
