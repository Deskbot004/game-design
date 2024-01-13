using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Card : MonoBehaviour
{
    //private string symbol;
    public string symbol; //For Debugging
    private string[] viableStrings = { "scissors", "rock", "paper", "lizard", "spock", "support" };
    //protected int slotType = -1;
    public int slotType = -1; // For Debugging
    private CardSprites cardSprites;

    public int GetValue()
    {
        return 0;
    }

    /*
     * Checks, if the card is a SupportCard or a NormalCard. false = Support, true = Normal.
     */

    public virtual bool IsBasic()
    {
        return false;
    }

    public string GetSymbol()
    {
        return this.symbol;
    }

    public int SetSymbol(string newSymbol)
    {
        if(Array.Exists(viableStrings,element => element == newSymbol))
        {
            this.symbol = newSymbol;
            return 0;
        }
        else
        {
            //Debug.Log("The given symbol is not a viable symbol");
            return -1;
        }
    }

    public int GetSlotType()
    {
        return this.slotType;
    }

    public virtual int SetSlotType(int type)
    {
        this.slotType = type;
        return 0;
    }

    public CardSprites GetCardSprites() { return cardSprites; }

    public virtual void SetSprite()
    { 
        cardSprites = transform.GetComponent<CardSprites>();
    }

    //[ContextMenu("Init Card")]
    // Workaround to avoid Console Spam on change, see #13: https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/
#if UNITY_EDITOR
    void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }
    public void _OnValidate()
    {
        if (this == null) return;
        SetSprite();
        //if (SetSymbol(symbol) < 0) { return; }
        //else { SetSprite(); }
    }
#endif
}
