using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class TestAnim : MonoBehaviour
{
    public AnimationHandlerComp animHandler;
    public List<Card> cards;

    public void TestAnimation() {
        
    }
}

public class Countdown : GameAnimation
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
