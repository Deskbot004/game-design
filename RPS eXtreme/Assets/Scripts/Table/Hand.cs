using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public float borderDistance = 0.001f;
    public float margin = 0;

    private Vector3 circleCenter = new Vector3(0f, 0f, -1f);
    private float circleRadius;

    // For Debugging
    public Deck deck;
    [System.NonSerialized]
    public List<Card> cards = new List<Card>();

    public void ArrangeHand()
    {
        SetCircle();

        // Some general variables
        float circumference = 2 * Mathf.PI * circleRadius;
        int n = cards.Count;
        float extent = GetComponent<SpriteRenderer>().bounds.extents.x;

        // Get Maximal Hand Width
        Vector3 maxLeftBorder = new Vector3(-extent, -borderDistance, 0);
        Vector3 maxRightBorder = new Vector3(extent, -borderDistance, 0);
        maxLeftBorder -= circleCenter;
        maxRightBorder -= circleCenter;
        float maxHandDegree = getDegree(maxLeftBorder, maxRightBorder);
        float maxHandWidth = circumference * maxHandDegree /360;

        // Get Needed Hand Width with given Cards and Margin
        float cardWidth = 2 * cards[0].GetComponent<BoxCollider2D>().size.x;
        float handWidth = (cardWidth + margin) * (n - 1);
        handWidth = Mathf.Min(handWidth, maxHandWidth);

        // Calculate the Borders and the Degree of the Arranged Cards
        float leftBorderY = circleRadius * Mathf.Cos(handWidth * Mathf.PI / circumference);
        float leftBorderX = Mathf.Sqrt(Mathf.Pow(circleRadius, 2) - Mathf.Pow(leftBorderY, 2));
        Vector3 leftBorder = new Vector3(-leftBorderX, leftBorderY, 0);
        Vector3 rightBorder = new Vector3(leftBorderX, leftBorderY, 0);
        float handDegree = getDegree(leftBorder, rightBorder);

        // Setup foreach loop
        Vector3 currentPosition = circleCenter + leftBorder;
        Vector3 rotatedBorder = leftBorder;
        float currentDegree = handDegree / 2;
        float degreeChange = handDegree / (n - 1);
        foreach (Card card in cards)
        {
            card.transform.localPosition = currentPosition;
            card.transform.eulerAngles = new Vector3(0, 0, currentDegree);

            rotatedBorder = Quaternion.Euler(0, 0, -degreeChange) * rotatedBorder;
            currentPosition =  circleCenter + rotatedBorder;
            currentDegree -= degreeChange;
        }
    }

    // Finds Center and Radius of the Circle in which the hand cards are arranged
    // Based on second answer from https://math.stackexchange.com/questions/213658/get-the-equation-of-a-circle-when-given-3-points
    void SetCircle()
    {
        float a = GetComponent<SpriteRenderer>().bounds.extents.x; // Half of Width of the Hand
        float b = borderDistance;

        float m_11 = 2*a*b;
        float m_13 = 2 * a * (Mathf.Pow(a, 2) + Mathf.Pow(b, 2));

        circleCenter.y = -m_13 / (2 * m_11);
        circleRadius = -circleCenter.y;
    }

    // Returns degree between vector v1 and v2. Ignores z Axis
    private float getDegree(Vector3 v1, Vector3 v2)
    {
        Vector2 a = new Vector2(v1.x, v1.y);
        Vector2 b = new Vector2(v2.x, v2.y);
        float dot = Vector2.Dot(a, b);
        return Mathf.Acos(dot / (a.magnitude * b.magnitude)) * 180 / Mathf.PI;
    }

    //--------------------- For Debugging-------------------------------------------------------------------------
    public void LogHand()
    {
        Debug.Log("Hand: " + string.Join(",", cards));
    }

    [ContextMenu("Arrange Hand")]
    public void Test()
    {
        cards = deck.cards;
        ArrangeHand();
    }
}