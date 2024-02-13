using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// TODO: Test
public class Table : MonoBehaviour
{
    public Gamelogic logic;
    public TablePlayer player;
    public TablePlayer enemy;
    public TableUI ui;
    public AnimationHandler animHandler;

    #region Main Functions ----------------------------------------------------------------------------------------
    void Start() {
        TestClass.table = this; // Debugging
        player.Init(this);
        enemy.Init(this);
        ui.Init(this);
        //logic.init(this); // TODO: Rename to Init (capital letter)
    }

    public void DrawCards(int amount, bool forPlayer) {
        TablePlayer cardDrawer = forPlayer? player : enemy;
        cardDrawer.DrawCards(amount);
    }

    public void ResolveSlot(int slotNr, string winner) {
        // TODO: Actually write it
        // TODO: Change it to int slotNr, bool playerWon, bool enemyWon
    }

    public void ClearSlots() {
        player.ClearSlots();
        enemy.ClearSlots();
    }

    public void SetWinner(string name) {
        // TODO: Let Gamelogic queue the animation
    }
    #endregion

    #region Getter & Setter ----------------------------------------------------------------------------------------
    /*
    // TODO: Hide Internal Structure
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
     */
    #endregion

    #region Debugging ----------------------------------------------------------------------------------------
    public void Test() {
        Debug.Log("Test Function");
        //List<Card> cards = player.drawpile.PopAllCards();
        //player.discardpile.AddCards(cards);
        //player.ShuffleDiscardIntoDraw();
    }

    #endregion

}
