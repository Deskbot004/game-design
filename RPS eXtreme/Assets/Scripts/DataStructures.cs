using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DictKeys {
    PLAYER,
    ENEMY
}

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
