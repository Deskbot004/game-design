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
    [Header("Main Connections")]
    public Gamelogic logic;
    public TableSide player;
    public Enemy enemy;
    public TableUI ui;
    public AnimationHandlerComp animHandler;

    [Header("Other Stuff")]
    public Animator resolveTurnAnimator;


    #region Main Functions ----------------------------------------------------------------------------------------
    void Start() {
        StaticObjectRefs.table = this; // Debugging
        AnimationHandler.animHandler = animHandler;
        player.Init(this);
        enemy.Init(this);
        ui.Init(this);
        logic.Init(this);
    }

    public void DrawCards(int amount, DictKeys forPlayer) {
        TableSide cardDrawer = (forPlayer == DictKeys.PLAYER)? player : enemy;
        cardDrawer.DrawCards(amount);
    }

    public void PlaySlotResolveAnim(SlotResult playerResult, SlotResult enemyResult, bool lastSlot) {
        ResolveAnim anim = AnimationHandler.CreateAnim<ResolveAnim>();
        anim.Init(this, playerResult, enemyResult);
        anim.Options(closeAfterAnim: lastSlot);
        AnimationHandler.QueueAnimationExclusively(anim);
    }

    public void ClearSlots() {
        player.ClearSlots();
        enemy.ClearSlots();
    }

    public void SetWinner(DictKeys winner) {
        ui.ShowWinScreen(winner);
    }

    #endregion

    #region Shorthands ----------------------------------------------------------------------------------------------
    public List<(Slot, Slot)> GetMatchingSlots() {
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
