using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[Serializable]
public class Card : MonoBehaviour
{
    public string symbol;
    
    // For Normal:  0: has no slots, 1: has a top slot, 2: has a bottom slot; 3: has both
    // For Support: 0: fits in top slot, 1: fits in bottom slot
    public int slotType = -1;

    private string[] viableStrings = { "scissors", "rock", "paper", "lizard", "spock", "support" };
    
    protected Gamelogic gamelogic;
    protected Deck deck;
    private CardSprites cardSprites;

    public AudioClip playWhenMoved;

    // -1: outside of game, 0: in a pile, 1: in hand 2: in slot
    protected int status = -1;

    // Animation stuff
    private Vector3 targetPosition; // WorldPosition
    private Vector3 targetRotation;

    // ---------- Main Functions ------------------------------------------------------------------------------
    public virtual void init(Deck deck)
    {
        if(!deck.GetTablePlayer().isPlayer)
            transform.eulerAngles = new Vector3(0f, 0f, 180f);
        this.deck = deck;
        this.gamelogic = deck.GetTablePlayer().GetTable().GetGamelogic();
        this.targetPosition = this.transform.position;
        SetSprite();
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (this.status > 0)
            {
                OnRightClickInHand();
            }
        }
    }

    public virtual void OnRightClickInHand() 
    { 
        if(!deck.GetTablePlayer().isPlayer) return;
    }

    /*
    * Checks, if the card is a SupportCard or a NormalCard. false = Support, true = Normal.
    */

    public virtual bool IsBasic()
    {
        return false;
    }
    public IEnumerator MoveToTarget(float moveTime, bool activeOnArrival = true)
    {
        Vector3 startingPos  = transform.position;
        Vector3 finalPos = targetPosition;
        Quaternion startingRotation = transform.localRotation;
        Quaternion finalRotation = Quaternion.Euler(targetRotation);

        float elapsedTime = 0;
        
        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, elapsedTime / moveTime);
            transform.localRotation = Quaternion.Lerp(startingRotation, finalRotation, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(activeOnArrival);
        if(this.symbol != "support") {
            var audios = this.GetComponents<AudioSource>();
            foreach (var audio in audios) {
                if (audio.clip == playWhenMoved) audio.Play();
            }
        }
    }

    public void flipCard()
    {
        transform.Find("Card Sprites/Cardback").gameObject.SetActive(!transform.Find("Card Sprites/Cardback").gameObject.activeSelf);
    }

    // ---------- Getter & Setter ------------------------------------------------------------------------------

    public string GetSymbol(){return this.symbol;}

    public int SetSymbol(string newSymbol)
    {
        if(Array.Exists(viableStrings,element => element == newSymbol))
        {
            this.symbol = newSymbol;
            return 0;
        }
        else
        {
            return -1;
        }
    }

    public int GetSlotType(){return this.slotType;}

    public virtual int SetSlotType(int type)
    {
        this.slotType = type;
        return 0;
    }

    public CardSprites GetCardSprites() { return this.cardSprites; }

    public virtual void SetSprite()
    {
        cardSprites = transform.GetComponent<CardSprites>();
        if (GetCardSprites().colors.ContainsKey(GetSymbol()))
            transform.Find("Card Sprites/Background").GetComponent<SpriteRenderer>().color = GetCardSprites().colors[GetSymbol()]; // Set color
        if (GetCardSprites().colors.ContainsKey(GetSymbol() + " window"))
            transform.Find("Card Sprites/Picture Background").GetComponent<SpriteRenderer>().color = GetCardSprites().colors[GetSymbol() + " window"]; // Set color
    }

    public void SetStatus(int status) {this.status = status;}

    public int GetStatus() {return this.status;}
    public Deck GetDeck() {return deck;}

    public void SetWorldTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }
    public void SetTargetRotation(Vector3 rotation)
    {
        targetRotation = rotation;
    }

    public virtual List<(Action<Gamelogic, string, object>, object)> GetFunctionsAR() { return null; }

    public virtual List<(Action<Gamelogic, string, object>, object)> GetFunctionsBR() { return null; }

    public virtual List<(Action<Gamelogic, string, object>, object)> GetFunctionsDraw() { return null; }



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
}
