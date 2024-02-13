using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button endTurnButton;
    public Button closeCardpileButton; // TODO: Create better button in the scene
    public Button attachDoneButton;
    public Button detachButton;
    public Button drawpile;
    public Button discardpile;

    [Header("Other")]
    public GameObject dim; // TODO: Give dim a BoxCollider, so that it stops clicks behind it
    public Vector3 attachCardWorldPosition;
    public GameObject cardpileScrollview; // Scrollview object to be activated when opening the cardpile
    public Transform cardpileScrollviewContent; // Content object of the Scrollview

    private AnimationHandler animHandler;
    private TablePlayer player;
    private NormalCard attachModeFocusCard;
    private List<Button> allButtons;
    private List<Card> cardsInOpenedPile;

    #region Main Functions --------------------------------------------------------------------------------------------
    public void Init(Table table) {
        animHandler = table.animHandler;
        player = table.player;
        allButtons = new() {endTurnButton, closeCardpileButton, attachDoneButton, detachButton, drawpile, discardpile};
    }

    public void OpenPile(Cardpile pile) {
        cardsInOpenedPile = pile.GetSortedCards();
        float cardRows = Mathf.Ceil(cardsInOpenedPile.Count/5f);

        EnableInteractions(false);
        closeCardpileButton.interactable = true;
        closeCardpileButton.gameObject.SetActive(true);
        dim.SetActive(true);
        cardpileScrollview.SetActive(true);
        cardpileScrollviewContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, cardRows * 90);
        foreach (Card card in cardsInOpenedPile) {
            card.transform.SetParent(cardpileScrollviewContent);
            card.SetSortingLayer("UI");
            card.gameObject.SetActive(true);
            card.GetComponent<Animator>().SetBool("faceFront", true);
        }
        // TODO: Show return button
        // TODO: Create animation?
    }

    public void ClosePile() {
        EnableInteractions(true);
        closeCardpileButton.gameObject.SetActive(false);
        dim.SetActive(false);
        cardpileScrollview.SetActive(false);
        foreach (Card card in cardsInOpenedPile) {
            card.transform.SetParent(card.GetDeck().transform);
            card.SetSortingLayer("Cards on Table");
            card.gameObject.SetActive(false);
        }
        // TODO: Hide return button
        // TODO: Create animation?
    }

    public void EnableInteractions(bool enabled) {
        foreach (Card card in player.GetAllCards()) {
            card.EnableDrag(enabled);
            card.EnableRightClick(enabled);
        }
        
        foreach (Button button in allButtons) {
            button.interactable = enabled;
        }
    }
    #endregion

    #region AttachMode ------------------------------------------------------------------------------------------------
    public void BeginAttaching(NormalCard baseCard) {
        OpenAttachMode();
        if (baseCard == attachModeFocusCard) {
            EndAttaching();
        } else if (attachModeFocusCard != null) {
            RemoveFocus(false);
            SetFocusOn(baseCard);
        } else {
            SetFocusOn(baseCard);
        }
    }

    public void EndAttaching() {
        RemoveFocus(true);
        CloseAttachMode();
    }

    public void OpenAttachMode() {
        player.EnableSlots(false);
        endTurnButton.gameObject.SetActive(false);
        attachDoneButton.gameObject.SetActive(true);
        foreach (Card card in player.GetCardsInHand()) {
            card.EnableDrag(false);
        }

        AttachModeAnim anim = animHandler.CreateAnim<AttachModeAnim>();
        anim.dim = dim;
        anim.open = true;
        animHandler.QueueAnimation(anim);
    }

    public void CloseAttachMode() {
        player.EnableSlots(true);
        endTurnButton.gameObject.SetActive(true);
        attachDoneButton.gameObject.SetActive(false);        
        detachButton.gameObject.SetActive(false);
        foreach (Card card in player.GetCardsInHand()) {
            card.EnableDrag(true);
        }

        AttachModeAnim anim = animHandler.CreateAnim<AttachModeAnim>();
        anim.dim = dim;
        anim.open = false;
        animHandler.QueueAnimation(anim);
    }

    public void SetFocusOn(NormalCard baseCard) {
        attachModeFocusCard = baseCard;
        baseCard.DropActive = true;
        detachButton.gameObject.SetActive(baseCard.HasAttachedCards());
        
        List<Card> viableSupportCards = player.GetMatchingSupportCards(baseCard);
        foreach(Card card in player.GetCardsInHand()) {
            if (viableSupportCards.Contains(card) || card == baseCard)
                card.SetSortingLayer("Cards in Focus");
            else
                card.SetSortingLayer("Cards on Table");
        }

        MoveCardAnim anim = animHandler.CreateAnim<MoveCardAnim>();
        anim.cards = new() {baseCard};
        anim.targetWorldPosition = attachCardWorldPosition;
        anim.targetLocalRotation = Vector3.zero;
        anim.draggableOnArrival = false;

        player.RemoveFromHandWithAnimation(baseCard);
        animHandler.PlayParallelToLastQueuedAnim(anim);
        
        /*
        AttachModeAnim anim = animHandler.CreateAnim<AttachModeAnim>();
        anim.animHandler = animHandler;
        anim.focusCard = baseCard;
        anim.focusCardPosition = attachCardWorldPosition;
        anim.cardsInHand = player.GetCardsInHand();
        anim.hand = player.hand;
        anim.viableSupportCards = player.GetMatchingSupportCards(baseCard);
         */
        
    }

    public void RemoveFocus(bool withAnimation) {
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

}
