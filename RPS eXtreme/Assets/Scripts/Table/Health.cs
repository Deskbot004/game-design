using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Health : MonoBehaviour
{
    [UDictionary.Split(30, 70)]
    public UDictionary1 healthbars;

    [Serializable]
    public class UDictionary1 : UDictionary<string, GameObject> { }
    
    public void Damage(int health, string loser)
    {
        healthbars[loser].GetComponent<Animator>().SetBool("isDamaged", false);
        healthbars[loser].GetComponent<Animator>().SetBool("isDamaged", true);
        healthbars[loser].GetComponentInChildren<TextMeshProUGUI>().text = health.ToString();
    }

    public void SetHealth(int health, string player)
    {
        healthbars[player].GetComponentInChildren<TextMeshProUGUI>().text = health.ToString();
    }
}
