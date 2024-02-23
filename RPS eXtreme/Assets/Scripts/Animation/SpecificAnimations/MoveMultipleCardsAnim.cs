using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TESTED
// Moves cards to different Locations
public class MoveMultipleCardsAnim : MoveCardAnim
{
    public List<Vector3> targetWorldPositions = new();
    public List<Vector3> targetWorldRotations = new();
    
    protected override IEnumerator PlaySpecificAnimation() {
        Debug.Assert(AreThereEnoughTargets(), "Error: cards count doesn't equal target count", this);

        List<Coroutine> cardsInMotion = new();
        for (int i=0; i<cards.Count; i++) {
            cardsInMotion.Add(StartCoroutine(MoveCard(cards[i], targetWorldPositions[i], Quaternion.Euler(targetWorldRotations[i]))));
            yield return new WaitForSecondsRealtime(offsetTime);
        }
        
        foreach (Coroutine coroutine in cardsInMotion) {
            yield return coroutine;
        }
    }

    public bool AreThereEnoughTargets() {
        return cards.Count == targetWorldPositions.Count || cards.Count == targetWorldRotations.Count;
    }
}