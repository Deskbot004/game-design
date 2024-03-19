using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#region Enums -----------------------------------------------------------------------------------------------
public static class EnumUtils
{
    public static string SortingLayerName(SortingLayer layer) {
        List<string> indexToString = new() {
            "Default", "Attached Cards", "Cards on Table", "Dim", "Attached Cards in Focus", "Attached Cards in Focus", 
            "UI", "Win Screen"
        };
        return indexToString[(int) layer];
    }
}

public enum CardSlotPosition {
    TOP,
    BOTTOM
}

public enum CardSymbol {
    ROCK,
    PAPER,
    SCISSORS,
    LIZARD,
    SPOCK,
    SUPPORT
}

public enum SortingLayer {
    DEFAULT,
    ATTACHED_CARDS,
    CARDS_ON_TABLE,
    DIM,
    ATTACHED_CARDS_IN_FOCUS,
    CARDS_IN_FOCUS,
    UI,
    WIN_SCREEN
}

public enum TableSideName {
    PLAYER,
    ENEMY
}
#endregion


#region Wrapper Classes -------------------------------------------------------------------------------------
public class SlotResult
{
    public Slot slot;
    public bool slotWon;
    public int healthBeforeResolution;
    public int healthAfterResolution;

    public SlotResult(Slot slot, bool slotWon, int healthBeforeResolution, int healthAfterResolution) {
        this.slot = slot;
        this.slotWon = slotWon;
        this.healthBeforeResolution = healthBeforeResolution;
        this.healthAfterResolution = healthAfterResolution;
    }

    public bool TookDamage() {
        return healthBeforeResolution > healthAfterResolution;
    }

    public bool WasHealed() {
        return healthBeforeResolution < healthAfterResolution;
    }
}
#endregion
