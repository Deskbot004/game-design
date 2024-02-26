using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Plays flip animation for given cards
public class FlipCardAnim : Animation
{
    // Required
    private List<Card> cardsToFlip;

    // Options
    private bool startFront = false;
    private float offsetTime = 0; // -1 if card should only start flipping, once previous card is done


    public void Init(List<Card> cardsToFlip) {
        this.cardsToFlip = cardsToFlip;
        initialized = true;
    }

    public void Options(bool startFront = false, float offsetTime = 0) {
        this.startFront = startFront;
        this.offsetTime = offsetTime;
    }

    protected override IEnumerator PlaySpecificAnimation() {
        foreach (Card card in cardsToFlip) {
            card.gameObject.SetActive(true);
            card.GetComponent<Animator>().SetBool("faceFront", startFront);
            card.GetComponent<Animator>().SetBool("flip", true);
            if (offsetTime < 0) {
                float animationLength = card.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSecondsRealtime(animationLength);
            } else {
                yield return new WaitForSecondsRealtime(offsetTime);
            }
        }
        float lastAnimationLength = cardsToFlip.Last().GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSecondsRealtime(lastAnimationLength);
    }

    protected override void SetAnimatedObjects() {
        animatedObjects.AddRange(cardsToFlip.Select(c => c.gameObject));
    }
}