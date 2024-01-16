using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Card : MonoBehaviour
{
    //private string symbol;
    public string symbol; // Set to public for Debugging
    private string[] viableStrings = { "scissors", "rock", "paper", "lizard", "spock", "support" };
    //protected int slotType = -1;
    public int slotType = -1; // Set to public for Debugging
    private int status = -1; //-1: outside of game, 0: in a pile, 1: in hand/slot // TODO sollte in verschiedenen funktionen angepass werden
    private CardSprites cardSprites;
    protected Deck deck;

    public void init(Deck deck)
    {
        this.deck = deck;
        SetSprite();
    }

    void OnMouseOver () {
        if(Input.GetMouseButtonDown(1))
        {
            if(status == 1) 
            {
                OnRightClickInHand();
            }
        }
    }

    public virtual void OnRightClickInHand() {}


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
        if (GetCardSprites().colors.ContainsKey(GetSymbol()))
            transform.Find("Background").GetComponent<SpriteRenderer>().color = GetCardSprites().colors[GetSymbol()]; // Set color

        if(deck != null && !deck.GetTablePlayer().isPlayer)
        {
            transform.Find("Cardback").gameObject.SetActive(true);
            transform.Find("Upper Effect").gameObject.SetActive(false);
            transform.Find("Lower Effect").gameObject.SetActive(false);
            return;
        }
    }

    public void flipCard()
    {
        transform.Find("Cardback").gameObject.SetActive(!transform.Find("Cardback").gameObject.activeSelf);
    }

    public void SetStatus(int status) {this.status = status;}
    public int GetStatus() {return status;}
    public Deck GetDeck() {return deck;}

    //[ContextMenu("Init Card")]
    // Workaround to avoid Console Spam on change, see #13: https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/
#if UNITY_EDITOR
    void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }
    public void _OnValidate()
    {
        if (this == null) return;
        else if (gameObject.scene.name == null) return;
        SetSprite();
        //if (SetSymbol(symbol) < 0) { return; }
        //else { SetSprite(); }
    }
#endif
}
