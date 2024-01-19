using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnim : MonoBehaviour
{
    public NormalCard normal;
    public SupportCard support;

    public void flipCards()
    {
        bool isFacingFront = normal.GetComponent<Animator>().GetBool("isFacingFront");
        normal.GetComponent<Animator>().SetBool("isFacingFront", !isFacingFront);
        support.GetComponent<Animator>().SetBool("isFacingFront", !isFacingFront);
    }
}
