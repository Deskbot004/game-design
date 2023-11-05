using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cardpile : MonoBehaviour
{
    [System.NonSerialized]
    public List<Card> cards = new List<Card>();

    // Randomizes the order of the cards
    public void Shuffle()
    {
        System.Random rng = new System.Random();
        cards = cards.OrderBy(x => rng.Next()).ToList();
    }

    // TODO: Animation
    // ------ Animation Stuff -------------------------------------------------------------------
    // On Mouse Hover darkens the deck
    void OnMouseEnter()
    {
        //GetComponent<SpriteRenderer>().color = new Color(204, 204, 204, 255);
        GetComponent<SpriteRenderer>().color = Color.gray;
    }

    void OnMouseExit()
    {
        //GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void OnMouseDown()
    {
        GetComponent<SpriteRenderer>().color = Color.black;
    }

    // Displays the cards on the screen (Currently only logs them on console)
    void OnMouseUpAsButton()
    {
        Debug.Log("Drawpile: " + string.Join(",", cards));
        GetComponent<SpriteRenderer>().color = Color.gray;
    }
}
