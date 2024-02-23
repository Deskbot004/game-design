using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class TestAnim : MonoBehaviour
{
    public AnimationHandler animHandler;
    public List<Card> cards;

    public void TestAnimation() {
        MoveCardAnim anim1 = animHandler.CreateAnim<MoveCardAnim>();
        MoveCardAnim anim2 = animHandler.CreateAnim<MoveCardAnim>();
        MoveCardAnim anim3 = animHandler.CreateAnim<MoveCardAnim>();
        MoveCardAnim anim4 = animHandler.CreateAnim<MoveCardAnim>();
        MoveCardAnim anim5 = animHandler.CreateAnim<MoveCardAnim>();
        MoveCardAnim anim6 = animHandler.CreateAnim<MoveCardAnim>();
        MoveCardAnim anim7 = animHandler.CreateAnim<MoveCardAnim>();

        anim1.cards = new() {cards[0]};
        anim1.targetWorldPosition = new Vector3(0,0,0);
        anim1.targetWorldRotation = new Vector3(0,0,45);
        anim1.moveTime = 0.5f;

        anim2.cards = new() {cards[1]};
        anim2.targetWorldPosition = new Vector3(2,2,0);
        anim2.targetWorldRotation = new Vector3(0,0,-45);
        anim2.moveTime = 0.5f;

        anim3.cards = new() {cards[2]};
        anim3.targetWorldPosition = new Vector3(-2,-2,0);
        anim3.targetWorldRotation = new Vector3(0,0,90);
        anim3.moveTime = 0.5f;

        anim4.cards = cards;
        anim4.targetWorldPosition = new Vector3(-5,0,0);
        anim4.targetWorldRotation = new Vector3(0,0,0);
        anim4.moveTime = 0.5f;

        anim5.cards = new() {cards[0]};
        anim5.targetWorldPosition = new Vector3(-2,-2,0);
        anim5.targetWorldRotation = new Vector3(0,0,90);
        anim5.moveTime = 0.5f;

        anim6.cards = new() {cards[1]};
        anim6.targetWorldPosition = new Vector3(2,2,0);
        anim6.targetWorldRotation = new Vector3(0,0,-45);
        anim6.moveTime = 0.5f;

        anim7.cards = new() {cards[2]};
        anim7.targetWorldPosition = new Vector3(0,0,0);
        anim7.targetWorldRotation = new Vector3(0,0,45);
        anim7.moveTime = 0.5f;

        animHandler.QueueAnimation(anim1);
        animHandler.QueueAnimation(anim2, (int) AnimationOffQueues.OPPONENT);
        animHandler.QueueAnimation(anim3, (int) AnimationOffQueues.OPPONENT);
        animHandler.QueueAfterOffQueues(anim4);
        animHandler.QueueAfterOffQueues(anim7);
        animHandler.QueueAnimation(anim5, (int) AnimationOffQueues.OPPONENT);
        animHandler.QueueAnimation(anim6);
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
