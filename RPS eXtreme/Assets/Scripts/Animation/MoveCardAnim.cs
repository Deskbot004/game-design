using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TESTED
// Moves all cards to the same destination Object, or to the same targetWorldPosition/Rotation if there isn't an Object
public class MoveCardAnim : Animation
{
    public List<Card> cards = new();

    public Transform destinationObject;
    public Vector3 targetWorldPosition;
    public Vector3 targetLocalRotation; // TODO: Change to World Rotation

    public float moveTime = 0.1f; // Time it takes the card to arrive
    public float offsetTime = 0; // Time between two cards staring to move
    public bool disableOnArrival = false;
    public bool draggableOnArrival = true;
    

    protected override IEnumerator PlaySpecificAnimation() {
        if (destinationObject != null) {
            targetWorldPosition = destinationObject.position;
            targetLocalRotation = destinationObject.localEulerAngles;
        }

        List<Coroutine> cardsInMotion = new();
        foreach (Card card in cards) {
            cardsInMotion.Add(StartCoroutine(MoveCard(card, targetWorldPosition, Quaternion.Euler(targetLocalRotation))));
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

    public IEnumerator MoveCard(Card card, Vector3 endingPos, Quaternion endingRotation) {
        Vector3 startingPos  = card.transform.position;
        Quaternion startingRotation = card.transform.localRotation;
        float elapsedTime = 0;

        card.gameObject.SetActive(true);
        card.EnableDrag(false);
        while (elapsedTime < moveTime) {
            card.transform.position = Vector3.Lerp(startingPos, endingPos, elapsedTime / moveTime);
            card.transform.localRotation = Quaternion.Lerp(startingRotation, endingRotation, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        card.transform.position = endingPos;
        card.transform.localRotation = endingRotation;
        card.gameObject.SetActive(!disableOnArrival);
        card.EnableDrag(draggableOnArrival);

        /* TODO (Copied from Card, so this = card, not CardAnimator)
        if(this.symbol != "support") {
            var audios = this.GetComponents<AudioSource>();
            foreach (var audio in audios) {
                if (audio.clip == playWhenMoved) audio.Play();
            }
        }
        */ 
    }
}