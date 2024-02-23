using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class ResolveAnim : Animation
{
    public TableUI ui;
    public bool closeAfterAnim = false;

    public Animator resolveTurnAnimator;
    public Vector3 startPositionPlayer;
    public Vector3 startPositionEnemy;

    public NormalCard playerCard;
    public List<SupportCard> playerSuppCards;
    public NormalCard enemyCard;
    public List<SupportCard> enemySuppCards;
    public bool playerWon;
    public bool enemyWon;
    public int playerHealth;
    public int enemyHealth;

    // TODO: Handle case that both slots are empty -> PlayClashAnimation & rest shouldn't wait for coroutines in that case
    protected override IEnumerator PlaySpecificAnimation() {
        yield return SetupAnimation();
        yield return PlayClashAnimation();

        Coroutine healthPlayerAnim = StartCoroutine(PlayHealthAnimation(ui.healthbars["player"], !playerWon, playerHealth));
        Coroutine healthEnemyAnim = StartCoroutine(PlayHealthAnimation(ui.healthbars["enemy"], !enemyWon, enemyHealth));
        yield return healthPlayerAnim;
        yield return healthEnemyAnim;

        yield return ResetEverything();
    }

    protected override void SetAnimatedObjects() {
        if (playerCard != null)
            animatedObjects.Add(playerCard.gameObject);
        if (enemyCard != null)
            animatedObjects.Add(enemyCard.gameObject);
    }

    IEnumerator SetupAnimation() {
        ui.EnableInteractions(false);
        ui.dim.SetActive(true);
        if (playerCard != null) {
            resolveTurnAnimator.transform.Find("PlayerCard/Rotation").position = startPositionPlayer;
            playerCard.transform.SetParent(resolveTurnAnimator.transform.Find("PlayerCard/Rotation"));
            playerCard.SetSortingLayer("Cards in Focus");
            foreach (Card suppCard in playerSuppCards) {
                suppCard.SetSortingLayer("Attached Cards in Focus"); // TODO: As this layer is above Attached Cards, the mask of the Normal Card doesn't work on it
                suppCard.transform.SetParent(resolveTurnAnimator.transform.Find("PlayerCard/Rotation"));
            }
        }
        if (enemyCard != null) {
            resolveTurnAnimator.transform.Find("EnemyCard/Rotation").position = startPositionEnemy;
            enemyCard.SetSortingLayer("Cards in Focus");
            enemyCard.transform.SetParent(resolveTurnAnimator.transform.Find("EnemyCard/Rotation"));
            foreach (Card suppCard in enemySuppCards) {
                suppCard.SetSortingLayer("Attached Cards in Focus");
                suppCard.transform.SetParent(resolveTurnAnimator.transform.Find("EnemyCard/Rotation"));
            }
        }
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator PlayClashAnimation() {
        resolveTurnAnimator.SetBool("playerWon", playerWon);
        resolveTurnAnimator.SetBool("enemyWon", enemyWon);
        resolveTurnAnimator.SetBool("playAnim", true);
        float animationLength = Mathf.Max(resolveTurnAnimator.GetCurrentAnimatorStateInfo(0).length, resolveTurnAnimator.GetCurrentAnimatorStateInfo(1).length);
        yield return new WaitForSecondsRealtime(animationLength + 0.5f);
    }

    IEnumerator PlayHealthAnimation(GameObject healthbar, bool tookDamage, int newHealth) {
        float animationLength = 0;
        if (tookDamage) {
            healthbar.GetComponent<SortingGroup>().sortingLayerName = "Cards in Focus";
            healthbar.GetComponent<Animator>().SetBool("isDamaged", true);
            animationLength = healthbar.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + 1;
        }
        healthbar.GetComponentInChildren<TextMeshPro>().text = newHealth.ToString();
        yield return new WaitForSecondsRealtime(animationLength);
        healthbar.GetComponent<SortingGroup>().sortingLayerName = "Default";
        healthbar.GetComponent<Animator>().SetBool("isDamaged", false);
    }

    IEnumerator ResetEverything() {
        if (playerCard != null) {
            playerCard.SetSortingLayer("Cards on Table");
            playerCard.transform.SetParent(playerCard.GetDeck().transform);
            foreach (Card suppCard in playerSuppCards) {
                suppCard.SetSortingLayer("Attached Cards");
                suppCard.transform.SetParent(suppCard.GetDeck().transform);
            }
            
        }
        if (enemyCard != null) {
            enemyCard.SetSortingLayer("Cards on Table");
            enemyCard.transform.SetParent(enemyCard.GetDeck().transform);
            foreach (Card suppCard in enemySuppCards) {
                suppCard.SetSortingLayer("Attached Cards");
                suppCard.transform.SetParent(suppCard.GetDeck().transform);
            }
        }
        resolveTurnAnimator.SetBool("playAnim", false);
        yield return new WaitForSeconds(0.5f);
        if(closeAfterAnim) {
            ui.EnableInteractions(true);
            ui.dim.SetActive(false);
        }
    }
}