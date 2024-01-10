using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SupportCard : Card
{
    private bool isAttached;
    public List<Action> functions = new List<Action>();
    private int[] viableSlotTypes = { 0, 1 }; // 0: fits in top slot, 1: fits in bottom slot

    public List<Action> GetFunctions() {
        return functions;
    }

    public override int SetSlotType(int type)
    {
        if (Array.Exists(viableSlotTypes, element => element == type))
        {
            this.slotType = type;
            return 0;
        }
        else
        {
            Debug.Log("SupportCard got invalid slottype!");
            return -1;
        }
    }

    public bool GetAttachmentStatus()
    {
        return this.isAttached;
    }

    public int SetAttachmentStatus(bool status)
    {
        this.isAttached = status;
        return 0;
    }

}
