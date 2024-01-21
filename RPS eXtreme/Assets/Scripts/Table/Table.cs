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
    public Animator resolveTurnAnimator;
    public Health healthUI;
    public WinLoseScreen winLoseScreen;

    [Header("Debugging")]
    public bool quickResolve;
    public float waitTimer = -1;

    private float cardMoveTime = 0.5f;

    private FadeInOut fade;

    void Awake(){
        this.fade = this.GetComponent<FadeInOut>();
        fade.FadeOut();
    }

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
            foreach(Card card in slot.GetNormalAndSuppCards())
            {
                card.GetComponent<Animator>().SetBool("flip", true);
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

    // TODO: Make better anim for no cards in slot
    public IEnumerator ResolveSlot(int slotNr, string winner, IDictionary<string, int> lifePoints)
    {
        List<string> winnerToInt = new() {"user", "enemy", "none"};
        int winnerInt = winnerToInt.FindIndex(a => a == winner);

        // Setup position of animation box
        if(slotNr == 0)
        {
            resolveTurnAnimator.transform.Find("PlayerCard/Rotation").localPosition = Vector3.zero;
            resolveTurnAnimator.transform.Find("EnemyCard/Rotation").localPosition = Vector3.zero;
        }
        else
        {
            resolveTurnAnimator.transform.Find("PlayerCard/Rotation").localPosition = new Vector3(7.07f, 0, 0);
            resolveTurnAnimator.transform.Find("EnemyCard/Rotation").localPosition = new Vector3(7.07f, 0, 0);
        }

        // Bring all cards to front
        Dictionary<string, Slot> slots = GetSlotByNr(slotNr);
        foreach (Slot slot in slots.Values)
        {
            Card card = slot.GetCard();
            if(card != null)
            {
                card.GetComponent<SortingGroup>().sortingLayerName = "Cards in Focus";
                string newParent = "";
                if(card.GetDeck().GetTablePlayer().isPlayer) newParent = "PlayerCard/Rotation";
                else newParent = "EnemyCard/Rotation";
                card.transform.SetParent(resolveTurnAnimator.transform.Find(newParent));
            }
        }
        yield return new WaitForSeconds(0.5f);

        // Play animation of cards and wait until it's done
        resolveTurnAnimator.SetInteger("winner", winnerInt);
        resolveTurnAnimator.SetBool("playAnim", true);
        float animationLength = resolveTurnAnimator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSecondsRealtime(animationLength + 0.5f);

        // Play animation for life
        if(winner == "user")
        {
            yield return healthUI.PlayDamageAnimation(lifePoints["enemy"], "enemy");
            healthUI.SetHealth(lifePoints["user"], "user");
        }
        else if (winner == "enemy")
        {
            yield return healthUI.PlayDamageAnimation(lifePoints["user"], "user");
            healthUI.SetHealth(lifePoints["enemy"], "enemy");
        }

        // Return Cards to previous state
        foreach (Slot slot in slots.Values)
        {
            Card card = slot.GetCard();
            if(card != null)
            {
                card.GetComponent<SortingGroup>().sortingLayerName = "Cards on Table";
                card.transform.SetParent(card.GetDeck().transform);
            }
        }
        resolveTurnAnimator.SetBool("playAnim", false);
        yield return new WaitForSeconds(0.5f);
    }

    public void SetWinner(string name)
    {
        winLoseScreen.showWinner(name);
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
