using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TESTED
// Draws a single Card from the drawpile
public class FlipCardAnim : Animation
{
    public List<Card> flippedCards;
    public bool startFront = false;
    public float offsetTime = 0;

    protected override IEnumerator PlaySpecificAnimation() {
        foreach (Card card in flippedCards) {
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
        float lastAnimationLength = flippedCards.Last().GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSecondsRealtime(lastAnimationLength);
    }

    protected override void SetAnimatedObjects() {
        foreach (Card card in flippedCards) {
            animatedObjects.Add(card.gameObject);
        }
    }
}