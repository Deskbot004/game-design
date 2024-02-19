using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class ResolveAnim : Animation
{
    public TableUI ui;
    public Animator resolveTurnAnimator;
    public Vector3 startPosition; // TODO: Change the Animation, so that this equals the position of the slot containing the card

    public Card playerCard;
    public Card enemyCard;
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
        if (playerCard != null) {
            resolveTurnAnimator.transform.Find("PlayerCard/Rotation").localPosition = startPosition;
            playerCard.SetSortingLayer("Cards in Focus");
            playerCard.transform.SetParent(resolveTurnAnimator.transform.Find("PlayerCard/Rotation"));
        }
        if (enemyCard != null) {
            resolveTurnAnimator.transform.Find("EnemyCard/Rotation").localPosition = startPosition;
            enemyCard.SetSortingLayer("Cards in Focus");
            enemyCard.transform.SetParent(resolveTurnAnimator.transform.Find("EnemyCard/Rotation"));
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
        }
        if (enemyCard != null) {
            enemyCard.SetSortingLayer("Cards on Table");
            enemyCard.transform.SetParent(enemyCard.GetDeck().transform);
        }
        resolveTurnAnimator.SetBool("playAnim", false);
        ui.EnableInteractions(true);
        yield return new WaitForSeconds(0.5f);
    }
}