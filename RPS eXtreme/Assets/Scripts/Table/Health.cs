using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Rendering;

public class Health : MonoBehaviour
{
    [UDictionary.Split(30, 70)]
    public UDictionary1 healthbars;

    [Serializable]
    public class UDictionary1 : UDictionary<string, GameObject> { }
    
    public void Damage(int health, string loser)
    {
        StartCoroutine(PlayDamageAnimation(health, loser));
        
    }

    public void SetHealth(int health, string player)
    {
        healthbars[player].GetComponentInChildren<TextMeshPro>().text = health.ToString();
    }

    public IEnumerator PlayDamageAnimation(int health, string loser)
    {
        healthbars[loser].GetComponent<SortingGroup>().sortingLayerName = "Cards in Focus";
        healthbars[loser].GetComponent<Animator>().SetBool("isDamaged", true);
        healthbars[loser].GetComponentInChildren<TextMeshPro>().text = health.ToString();
        float animationLength = healthbars[loser].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSecondsRealtime(animationLength + 1f);
        healthbars[loser].GetComponent<SortingGroup>().sortingLayerName = "Default";
        healthbars[loser].GetComponent<Animator>().SetBool("isDamaged", false);
    }
}
