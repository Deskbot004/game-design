using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.Rendering;

public enum CardSymbol {
    ROCK,
    PAPER,
    SCISSORS,
    LIZARD,
    SPOCK,
    SUPPORT
}

[Serializable]
public class Card : MonoBehaviour
{
    [Header("General")]
    [SerializeField] protected CardSymbol symbol;
    [SerializeField] public bool topSlot;
    [SerializeField] public bool bottomSlot;
    
    protected Deck deck;
    protected bool rightClickEnabled = true;

    #region Main Functions ----------------------------------------------------------------------------------
    public virtual void Init(Deck deck) {
        this.deck = deck;
        SetSprite();
        if(!BelongsToPlayer()) {
            transform.eulerAngles = new Vector3(0f, 0f, 180f);
        }
        GetComponent<Draggable>().Init();
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonDown(1)) {
            OnRightClick();
        }
    }

    public virtual void OnRightClick()  { 
        if(!BelongsToPlayer()) return;
        if(!rightClickEnabled) return;
    }

    public virtual void SetSprite() {
        CardSprites cardSprites = transform.GetComponent<CardSprites>();
        Debug.Assert(cardSprites.backgroundColor.ContainsKey(symbol), "backgroundColor is missing key " + symbol.ToString(), this);
        Debug.Assert(cardSprites.windowColor.ContainsKey(symbol), "windowColor is missing key " + symbol.ToString(), this);

        // Set Color
        transform.Find("Card Sprites/Background").GetComponent<SpriteRenderer>().color = cardSprites.backgroundColor[symbol];
        transform.Find("Card Sprites/Picture Background").GetComponent<SpriteRenderer>().color = cardSprites.windowColor[symbol]; 
    }
    #endregion


    #region Shorthands --------------------------------------------------------------------------------------
    public bool BelongsToPlayer() {
        return deck.GetTableSide().isPlayer;
    }

    public void EnableDrag(bool enabled) {
        GetComponent<Draggable>().enabled = enabled;
    }

    public void EnableRightClick(bool enabled) {
        rightClickEnabled = enabled;
    }

    public Transform GetDeckTransform() {
        return deck.transform;
    }

    public virtual List<Function> GetFunctionsForResolve() {
        return null;
    }

    public virtual (FunctionID, List<string>) GetFunctionsForSave() { // TODO
        return new();
    }

    public TableSide GetPlayerSide() {
        return deck.GetTableSide();
    }

    public virtual bool IsNormal() {
        return this is NormalCard;
    }

    public void SetCardValues(CardSymbol symbol, bool topSlot, bool bottomSlot) {
        this.symbol = symbol;
        this.topSlot = topSlot;
        this.bottomSlot = bottomSlot;
        SetSprite();
    }

    public void SetSortingLayer(string layerName) {
        GetComponent<SortingGroup>().sortingLayerName = layerName;
    }
    #endregion

    
    #region Still here because other things use it, but might should be deleted later -----------------------
    public CardSymbol GetSymbol() { 
        return this.symbol;
    }

    public virtual List<(Action<Gamelogic, string, object>, object)> GetFunctionsAR() { return null; }

    public virtual List<(Action<Gamelogic, string, object>, object)> GetFunctionsBR() { return null; }

    public virtual List<(Action<Gamelogic, string, object>, object)> GetFunctionsDraw() { return null; }

    #endregion

    
    #region Editor Stuff ------------------------------------------------------------------------------------
    //[ContextMenu("Init Card")]
    // Workaround to avoid Console Spam on change, see #13: https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/
    #if UNITY_EDITOR
    void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }
    public void _OnValidate()
    {
        if (this == null) return;
        else if (gameObject.scene.name == null) return;
        SetSprite();
    }
    #endif
    #endregion
}
