using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves all cards to the same destination Object or to the same targetWorldPosition/Rotation if there isn't an Object
public class MoveCardAnim : GameAnimation
{
    // Required
    protected List<Card> cards = new();
    private Transform destinationObject;
    private Vector3 targetWorldPosition; // Only used, when destinationObjet == null
    private Vector3 targetWorldRotation; // Only used, when destinationObjet == null

    // Options
    protected float moveTime = 0.1f; // Time it takes the card to arrive
    protected float offsetTime = 0; // Time between two cards staring to move
    protected bool disableOnArrival = false;
    protected bool draggableOnArrival = true;


    public void Init(List<Card> cards, Vector3 targetWorldPosition, Vector3 targetWorldRotation) {
        this.cards = cards;
        this.targetWorldPosition = targetWorldPosition;
        this.targetWorldRotation = targetWorldRotation;
        initialized = true;
    }

    public void Init(List<Card> cards, Transform destinationObject) {
        this.cards = cards;
        this.destinationObject = destinationObject;
        initialized = true;
    }

    public void Options(float moveTime = 0.1f, float offsetTime = 0, bool disableOnArrival = false, bool draggableOnArrival = true) {
        this.moveTime = moveTime;
        this.offsetTime = offsetTime;
        this.disableOnArrival = disableOnArrival;
        this.draggableOnArrival = draggableOnArrival;
    }

    protected override IEnumerator PlaySpecificAnimation() {
        if (destinationObject != null) {
            targetWorldPosition = destinationObject.position;
            targetWorldRotation = destinationObject.eulerAngles;
        }

        List<Coroutine> cardsInMotion = new();
        foreach (Card card in cards) {
            cardsInMotion.Add(StartCoroutine(MoveCard(card, targetWorldPosition, Quaternion.Euler(targetWorldRotation))));
            yield return new WaitForSecondsRealtime(offsetTime);
        }
        
        foreach (Coroutine coroutine in cardsInMotion) {
            yield return coroutine;
        }
    }

    protected override void SetAnimatedObjects() {
        foreach (Card card in cards) {
            animatedObjects.Add(card.gameObject);
        }
    }

    protected IEnumerator MoveCard(Card card, Vector3 endingPos, Quaternion endingRotation) {
        Vector3 startingPos  = card.transform.position;
        Quaternion startingRotation = card.transform.rotation;
        float elapsedTime = 0;

        card.gameObject.SetActive(true);
        card.EnableDrag(false);
        while (elapsedTime < moveTime) {
            card.transform.position = Vector3.Lerp(startingPos, endingPos, elapsedTime / moveTime);
            card.transform.rotation = Quaternion.Lerp(startingRotation, endingRotation, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        card.transform.position = endingPos;
        card.transform.rotation = endingRotation;
        card.gameObject.SetActive(!disableOnArrival);
        card.EnableDrag(draggableOnArrival);

        /* TODO Audio: (Copied from Card, so this = card, not CardAnimator)
        if(this.symbol != "support") {
            var audios = this.GetComponents<AudioSource>();
            foreach (var audio in audios) {
                if (audio.clip == playWhenMoved) audio.Play();
            }
        }
        */ 
    }
}