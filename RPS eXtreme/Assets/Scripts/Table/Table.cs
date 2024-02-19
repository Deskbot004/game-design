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
    public AnimationHandler opponentAnimHandler;

    [Header("Other Stuff")]
    public Animator resolveTurnAnimator;


    #region Main Functions ----------------------------------------------------------------------------------------
    void Start() {
        TestClass.table = this; // Debugging
        player.Init(this, animHandler);
        enemy.Init(this, opponentAnimHandler);
        ui.Init(this);
        //logic.Init(this);
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
        ui.EnableInteractions(true);
        ui.dim.SetActive(false);
    }

    public void PlayResolveAnimation(int slotNr, string winner, IDictionary<string, int> lifePoints) {
        // TODO: Change it to (Slot, Slot) slotCombination, bool[] {playerWon, enemyWon}, ...
        bool playerWon = winner == "user" || winner == "none";
        bool enemyWon = winner == "enemy" || winner == "none";

        ResolveAnim anim = animHandler.CreateAnim<ResolveAnim>();
        anim.ui = ui;
        anim.resolveTurnAnimator = resolveTurnAnimator;
        anim.startPosition = slotNr == 0? Vector3.zero : new Vector3(7.07f, 0, 0); // TODO: Change to Slot Position
        anim.playerCard = player.GetCardInSlot(slotNr);
        anim.enemyCard = enemy.GetCardInSlot(slotNr);
        anim.playerWon = playerWon;
        anim.enemyWon = enemyWon;
        anim.playerHealth = lifePoints["player"];
        anim.enemyHealth = lifePoints["enemy"];
        animHandler.QueueAnimation(anim);
    }

    public void ClearSlots() {
        player.ClearSlots();
        enemy.ClearSlots();
    }

    public void SetWinner(string name) {
        // TODO: Let Gamelogic queue the animation
    }
    #endregion


    #region Shorthands ----------------------------------------------------------------------------------------------
    public List<(Slot, Slot)> GetSlotsForResolving() { // TODO: Rename
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
        Dictionary<string, int> life = new();
        life["player"] = 12;
        life["enemy"] = 9;
        PlayResolveAnimation(0, "user", life);
    }

    #endregion

}
