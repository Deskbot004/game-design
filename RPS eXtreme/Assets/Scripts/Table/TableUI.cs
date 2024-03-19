using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TableUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button endTurnButton;
    public Button closeCardpileButton;
    public Button attachDoneButton;
    public Button detachButton;
    public Button drawpile;
    public Button discardpile;
    
    [Header("Other")]
    public GameObject dim;
    public WinLoseScreen winLoseScreen;
    public Vector3 attachCardWorldPosition;
    public GameObject cardpileScrollview; // Scrollview object to be activated when opening the cardpile
    public Transform cardpileScrollviewContent; // Content object of the Scrollview

    [UDictionary.Split(30, 70)]
    public HealthbarDict healthbars;
    [Serializable]
    public class HealthbarDict : UDictionary<TableSideName, GameObject> { }

    private Table table;
    private TableSide player;
    private NormalCard attachModeFocusCard;
    private List<Button> allButtons;
    private List<Card> cardsInOpenedPile;

    public void Init(Table table) {
        this.table = table;
        player = table.player;
        allButtons = GetType().GetFields().Where(f => f.FieldType == typeof(Button)).Select(p => (Button) p.GetValue(this)).ToList();
    }

    public void EnableInteractions(bool enabled) {
        foreach (Card card in player.GetCardsInDeck()) {
            card.EnableDrag(enabled);
            card.EnableRightClick(enabled);
        }
        
        foreach (Button button in allButtons) {
            button.interactable = enabled;
        }
    }

    #region Cardpiles --------------------------------------------------------------------------------------------
    // This function can access the Cardpile directly, because the parameter is set in the Unity Editor
    public void OpenCardpile(Cardpile pile) {
        EnableInteractions(false);
        closeCardpileButton.gameObject.SetActive(true);
        closeCardpileButton.interactable = true;
        dim.SetActive(true);

        cardpileScrollview.SetActive(true);
        cardsInOpenedPile = pile.GetSortedCards();
        float cardRows = Mathf.Ceil(cardsInOpenedPile.Count/5f);
        cardpileScrollviewContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, cardRows * 90);
        foreach (Card card in cardsInOpenedPile) {
            card.transform.SetParent(cardpileScrollviewContent);
            card.SetSortingLayer(SortingLayer.UI);
            card.gameObject.SetActive(true);
            card.GetComponent<Animator>().SetBool("faceFront", true);
        }
    }

    public void CloseCardpile() {
        EnableInteractions(true);
        closeCardpileButton.gameObject.SetActive(false);
        dim.SetActive(false);

        cardpileScrollview.SetActive(false);
        foreach (Card card in cardsInOpenedPile) {
            card.SetDeckAsParent();
            card.SetSortingLayer(SortingLayer.CARDS_ON_TABLE);
            card.gameObject.SetActive(false);
        }
    }
    #endregion

    #region AttachMode ------------------------------------------------------------------------------------------------
    public void HandleStartAttaching(NormalCard baseCard) {
        OpenAttachMode();
        if (baseCard == attachModeFocusCard) {
            HandleFinishAttaching();
        } else if (attachModeFocusCard != null) {
            RemoveFocus(withAnimation: false);
            SetFocusOn(baseCard);
        } else {
            SetFocusOn(baseCard);
        }
    }

    public void HandleFinishAttaching() {
        RemoveFocus(withAnimation: true);
        CloseAttachMode();
    }

    void OpenAttachMode() {
        player.EnableSlots(false);
        endTurnButton.gameObject.SetActive(false);
        attachDoneButton.gameObject.SetActive(true);
        detachButton.gameObject.SetActive(true);
        foreach (Card card in player.GetCardsInHand()) {
            card.EnableDrag(false);
        }

        AttachModeAnim anim = AnimationHandler.CreateAnim<AttachModeAnim>();
        anim.Init(dim, true);
        AnimationHandler.QueueAnimation(anim);
    }

    void CloseAttachMode() {
        player.EnableSlots(true);
        endTurnButton.gameObject.SetActive(true);
        attachDoneButton.gameObject.SetActive(false);        
        detachButton.gameObject.SetActive(false);
        foreach (Card card in player.GetCardsInHand()) {
            card.EnableDrag(true);
        }

        AttachModeAnim anim = AnimationHandler.CreateAnim<AttachModeAnim>();
        anim.Init(dim, false);
        AnimationHandler.QueueAnimation(anim);
    }

    void SetFocusOn(NormalCard baseCard) {
        attachModeFocusCard = baseCard;
        baseCard.DropActive = true;
        detachButton.interactable = baseCard.HasAttachedCards();
        
        List<Card> viableSupportCards = player.GetMatchingSupportCards(baseCard);
        foreach(Card card in player.GetCardsInHand()) {
            if (viableSupportCards.Contains(card) || card == baseCard)
                card.SetSortingLayer(SortingLayer.CARDS_IN_FOCUS);
            else
                card.SetSortingLayer(SortingLayer.CARDS_ON_TABLE);
        }

        MoveCardAnim anim = AnimationHandler.CreateAnim<MoveCardAnim>();
        anim.Init(new() {baseCard}, attachCardWorldPosition, Vector3.zero);
        anim.Options(draggableOnArrival: false);

        player.RemoveFromHandWithAnimation(baseCard);
        AnimationHandler.PlayParallelToLastQueuedAnim(anim);
    }

    void RemoveFocus(bool withAnimation) {
        if (withAnimation)
            player.AddToHandWithAnimation(attachModeFocusCard);
        else
            player.AddToHand(attachModeFocusCard);
        attachModeFocusCard.DropActive = false;
        attachModeFocusCard = null;
    }
    
    public void DetachAllCards() {
        List<SupportCard> supCards = attachModeFocusCard.DetachAllCards();
        foreach (SupportCard supCard in supCards) {
            bool dropSuccess = supCard.GetComponent<Draggable>().DropInto(player);
            Debug.Assert(dropSuccess, "Error dropping detached card into hand", supCard);
        }
    }
    #endregion

    #region Health ----------------------------------------------------------------------------------------------------
    public void SetHealth(int health, TableSideName healthbarKey) {
        healthbars[healthbarKey].GetComponentInChildren<TextMeshPro>().text = health.ToString();
    }
    #endregion

    #region Other ----------------------------------------------------------------------------------------------------
    public void ResolveTurn() {
        table.logic.ResolveTurn();
    }
    
    public void ShowWinScreen(TableSideName winner) {
        bool playerWon = winner == TableSideName.PLAYER;
        winLoseScreen.showWinner(playerWon);
    }
    #endregion
}