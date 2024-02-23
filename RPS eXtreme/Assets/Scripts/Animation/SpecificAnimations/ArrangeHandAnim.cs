using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

// TESTED
// Arranges cards on a semi-circle in the hand
public class ArrangeHandAnim : Animation 
{
    public AnimationHandler animHandler;
    public Hand hand;
    public List<Card> cardsInHand;

    private float marginBetweenCards = -0.2f;
    private float cardWidth;
    private Vector3 handExtents;
    private Transform handTransform;
    private Vector3 circleCenter = Vector3.zero;
    private float circleRadius;

    protected override IEnumerator PlaySpecificAnimation() {
        if (cardsInHand.Count > 0) {
            SetupCircle();
            MoveMultipleCardsAnim moveAnim = SetupAnim();
            yield return moveAnim.Play();
        }
        yield return null;
    }

    protected override void SetAnimatedObjects() {
        foreach (Card card in cardsInHand) {
            animatedObjects.Add(card.gameObject);
        }
    }

    void SetupCircle() {
        Card exampleCard = cardsInHand[0];
        cardWidth = exampleCard.GetComponent<BoxCollider2D>().size.x;
        handExtents = hand.GetComponent<SpriteRenderer>().bounds.extents;
        handTransform = hand.transform;
        CalculateCircle();
    }

    // Finds Center and Radius of the Circle in which the hand cards are arranged. Results are local to hand.
    // Based on second answer from https://math.stackexchange.com/questions/213658/get-the-equation-of-a-circle-when-given-3-points
    void CalculateCircle() {
        float a = handExtents.x; // Half of Width of the Hand
        float b = handExtents.y; // Half Height of the Hand

        float m_11 = 2*a*b;
        float m_13 = 2 * a * (Mathf.Pow(a, 2) + Mathf.Pow(b, 2));

        circleCenter.y = -m_13 / (2 * m_11);
        circleRadius = -circleCenter.y;
    }

    MoveMultipleCardsAnim SetupAnim() {
        MoveMultipleCardsAnim moveAnim = animHandler.CreateAnim<MoveMultipleCardsAnim>();
        ArrangeHand(moveAnim);
        moveAnim.moveTime = 0.3f;
        return moveAnim;
    }

    void ArrangeHand(MoveMultipleCardsAnim moveAnim) {
        (Vector3 targetLocalPosition, Vector3 targetLocalRotation, float degreeChange) = CalculateHandArrangement(cardsInHand.Count, cardsInHand[0].BelongsToPlayer());
        float currentZ = 0;

        foreach (Card handCard in cardsInHand) {
            moveAnim.cards.Add(handCard);;
            moveAnim.targetWorldPositions.Add(handTransform.TransformDirection(targetLocalPosition) + handTransform.position);
            moveAnim.targetWorldRotations.Add(handTransform.TransformDirection(targetLocalRotation));

            currentZ -= 0.1f;
            targetLocalPosition = Quaternion.Euler(0, 0, -degreeChange) * (targetLocalPosition - circleCenter);
            targetLocalPosition += circleCenter;
            targetLocalPosition.z = currentZ;
            targetLocalRotation.z -= degreeChange;
        }
    }

    // Results are local to hand
    (Vector3, Vector3, float) CalculateHandArrangement(int cardAmount, bool isPlayer) {
        float circumference = 2 * Mathf.PI * circleRadius;

        // Get Maximal Hand Width
        Vector3 maxLeftBorder = new Vector3(-handExtents.x, -handExtents.y, 0);
        Vector3 maxRightBorder = new Vector3(handExtents.x, -handExtents.y, 0);
        maxLeftBorder -= circleCenter;
        maxRightBorder -= circleCenter;
        float maxHandDegree = getDegree(maxLeftBorder, maxRightBorder);
        float maxHandWidth = circumference * maxHandDegree/360;

        // Get Needed Hand Width with given Cards and Margin
        float handWidth = (cardWidth + marginBetweenCards) * (cardAmount - 1);
        handWidth = Mathf.Min(handWidth, maxHandWidth);

        // Calculate the Borders and the Degree of the Arranged Cards
        float leftBorderY = circleRadius * Mathf.Cos(handWidth * Mathf.PI / circumference);
        float leftBorderX = Mathf.Sqrt(Mathf.Pow(circleRadius, 2) - Mathf.Pow(leftBorderY, 2));
        Vector3 leftBorder = new Vector3(-leftBorderX, leftBorderY, 0);
        Vector3 rightBorder = new Vector3(leftBorderX, leftBorderY, 0);
        float handDegree = getDegree(leftBorder, rightBorder);
        float startDegree = isPlayer ? handDegree/2 : handDegree/2 + 180;

        Vector3 startPosition = circleCenter + leftBorder;
        Vector3 startRotation = new Vector3(0, 0, startDegree);
        float degreeChange = (cardAmount-1) > 0 ? handDegree / (cardAmount-1) : 0;

        return (startPosition, startRotation, degreeChange);
    }

    float getDegree(Vector3 v1, Vector3 v2)
    {
        Vector2 a = (Vector2) v1;
        Vector2 b = (Vector2) v2;
        float dot = Vector2.Dot(a, b);
        return Mathf.Acos(dot / (a.magnitude * b.magnitude)) * 180 / Mathf.PI;
    }
}