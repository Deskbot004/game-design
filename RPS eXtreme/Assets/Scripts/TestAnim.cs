using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class TestAnim : MonoBehaviour
{
    public AnimationHandler animHandler;
    public List<Card> cards;

    public void TestAnimation() {
        MoveMultipleCardsAnim anim = animHandler.CreateAnim<MoveMultipleCardsAnim>();
        SetupAnimator(anim);
        animHandler.QueueAnimation(anim);
        /*
        Countdown anim = animHandler.CreateAnim<Countdown>();
        anim.count = 2;
        anim.total = total;
        animHandler.QueueAnimation(anim);
        */
    }

    public void SetupAnimator(MoveMultipleCardsAnim anim) {
        anim.cards = cards;
        anim.targetWorldPositions = new() {Vector3.zero, new Vector3(5,5,0)};
        anim.targetLocalRotations = new() {new Vector3(0,0,45), Vector3.zero};
        anim.moveTime = 0.5f;
        anim.offsetTime = 0.2f;
    }
}

public class Countdown : Animation
{
    public int total;
    public int count;

    protected override IEnumerator PlaySpecificAnimation() {
        Debug.Log("Counting down for the " + total + "th time!");
        while(count > 0) {
            Debug.Log(count);
            count--;
            yield return new WaitForSecondsRealtime(1);
        }
        Debug.Log("Done!");
    }
}
