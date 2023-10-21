using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [System.NonSerialized]
    public List<Card> cards = new List<Card>();

    public void LogHand()
    {
        Debug.Log("Hand: " + string.Join(",", cards));
    }
}
