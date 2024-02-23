using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    public Gamelogic logic;
    public TablePlayer player;
    public TablePlayer enemy;
    public TableUI ui;
    public AnimationHandler animHandler;

    [Header("Other Stuff")]
    public Animator resolveTurnAnimator;


    #region Main Functions ----------------------------------------------------------------------------------------
    void Start() {
        StaticObjectRefs.table = this; // Debugging
        player.Init(this, animHandler);
        enemy.Init(this, animHandler);
        ui.Init(this);
        logic.Init(this);
    }

    public void DrawCards(int amount, bool forPlayer) {
        TablePlayer cardDrawer = forPlayer? player : enemy;
        cardDrawer.DrawCards(amount);
    }

    public void StartResolve() {
        ui.EnableInteractions(false);
        ui.dim.SetActive(true);
    }

    public void EndResolve() {
        //ui.EnableInteractions(true);
        //ui.dim.SetActive(false);
    }

    public void PlayResolveAnimation(int slotNr, string winner, IDictionary<string, int> lifePoints) {
        // TODO: Change it to (Slot, Slot) slotCombination, bool[] {playerWon, enemyWon}, ...
        bool playerWon = winner == "user" || winner == "none";
        bool enemyWon = winner == "enemy" || winner == "none";

        ResolveAnim anim = animHandler.CreateAnim<ResolveAnim>();
        anim.ui = ui;
        anim.closeAfterAnim = slotNr == player.slots.Count - 1;
        anim.resolveTurnAnimator = resolveTurnAnimator;
        anim.startPositionPlayer = player.GetSlotByNr(slotNr).transform.position;
        anim.startPositionEnemy = enemy.GetSlotByNr(slotNr).transform.position;
        anim.playerCard = (NormalCard) player.GetCardInSlot(slotNr);
        if(anim.playerCard != null)
            anim.playerSuppCards = anim.playerCard.GetAttachedSupportCards();
        anim.enemyCard = (NormalCard) enemy.GetCardInSlot(slotNr);
        if(anim.enemyCard != null)
            anim.enemySuppCards = anim.enemyCard.GetAttachedSupportCards();
        anim.playerWon = playerWon;
        anim.enemyWon = enemyWon;
        anim.playerHealth = lifePoints["user"]; // TODO: Change to player
        anim.enemyHealth = lifePoints["enemy"];
        animHandler.QueueAfterOffQueues(anim);
    }

    public void ClearSlots() {
        player.ClearSlots();
        enemy.ClearSlots();
    }

    public void SetWinner(string winner) {
        // TODO: Change it to a bool
        bool playerWon = winner == "user";
        ui.ShowWinScreen(playerWon);
    }
    #endregion


    #region Shorthands ----------------------------------------------------------------------------------------------
    public List<(Slot, Slot)> GetSlotsForResolving() { // TODO: Rename?
        List<(Slot, Slot)> slotCombinations = new();
        foreach (Slot slot in player.slots) {
            slotCombinations.Add((slot, enemy.GetSlotByNr(slot.slotPosition)));
        }
        return slotCombinations;
    }
    #endregion

    #region Debugging ----------------------------------------------------------------------------------------
    // Function that is called when pressing the Test Button in Unity
    public void Test() {
        Debug.Log("Test Function");
    }

    #endregion

}
