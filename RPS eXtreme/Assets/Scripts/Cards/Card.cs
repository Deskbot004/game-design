using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Card : MonoBehaviour
{
    public string symbol;
    public bool topSlot;
    public bool bottomSlot;

    private string[] viableStrings = { "scissors", "rock", "paper", "lizard", "spock" };
    private CardSprites cardSprites;

    public int GetValue()
    {
        return 0;
    }

    public virtual bool IsBasic()
    {
        return true;
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

    public CardSprites GetCardSprites() { return cardSprites; }

    public virtual void SetSprite() { }

    //[ContextMenu("Init Card")]
    // Workaround to avoid Console Spam on change, see #13: https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/
#if UNITY_EDITOR
    void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }
    public void _OnValidate()
    {
        if (this == null) return;
        cardSprites = transform.GetComponent<CardSprites>();
        SetSprite();
        //if (SetSymbol(symbol) < 0) { return; }
        //else { SetSprite(); }
    }
#endif
}
