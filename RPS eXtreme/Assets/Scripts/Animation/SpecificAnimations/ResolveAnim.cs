using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class ResolveAnim : Animation
{
    // Main stuff
    TableUI ui;
    Animator resolveTurnAnimator;

    // Player Result
    SlotResult[] results;
    NormalCard[] normalCards;
    List<SupportCard>[] supportCards;

    // Options
    bool closeAfterAnim = false;


    #region Main Functions -------------------------------------------------------------------------------------------
    public void Init(Table table, SlotResult playerResult, SlotResult enemyResult) {
        ui = table.ui;
        resolveTurnAnimator = table.resolveTurnAnimator;

        results = new[] {playerResult, enemyResult};
        normalCards = new[] {playerResult.slot.GetCard(), enemyResult.slot.GetCard()};
        supportCards = new[] {normalCards[0].GetAttachedSupportCards(), normalCards[1].GetAttachedSupportCards()};

        initialized = true;
    }

    public void Options(bool closeAfterAnim = false) {
        this.closeAfterAnim = closeAfterAnim;
    }

    protected override void SetAnimatedObjects() {
        for(int i=0; i<2; i++) {
            if(normalCards[i] != null) {
                animatedObjects.Add(normalCards[i].gameObject);
                animatedObjects.AddRange(supportCards[i].Select(c => c.gameObject));
            }
        }
    }

    // TODO Later: Handle case that both slots are empty -> PlayClashAnimation & rest shouldn't wait for coroutines in that case
    protected override IEnumerator PlaySpecificAnimation() {
        yield return SetupAnimation();
        yield return PlayClashAnimation();
        yield return PlayHealthAnimation();
        yield return ResetEverything();
    }
    #endregion

    #region Helper Functions -------------------------------------------------------------------------------------------
    IEnumerator SetupAnimation() {
        ui.EnableInteractions(false);
        ui.dim.SetActive(true);

        string[] playerName = {"Player", "Enemy"};
        for(int i=0; i<2; i++) {
            if(normalCards[i] == null) {
                continue;
            }

            Transform newParent = resolveTurnAnimator.transform.Find(playerName[i] + "Card/Rotation");
            newParent.position = results[i].slot.transform.position;
            normalCards[i].transform.SetParent(newParent);
            normalCards[i].SetSortingLayer("Cards in Focus");
            foreach(SupportCard suppCard in supportCards[i]) {
                suppCard.transform.SetParent(newParent);
                suppCard.SetSortingLayer("Attached Cards in Focus"); // TODO Later: As this layer is above Attached Cards, the mask of the Normal Card doesn't work on it
            }
        }
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator PlayClashAnimation() {
        resolveTurnAnimator.SetBool("playerWon", results[0].slotWon);
        resolveTurnAnimator.SetBool("enemyWon", results[1].slotWon);
        resolveTurnAnimator.SetBool("playAnim", true);
        float animationLength = Mathf.Max(resolveTurnAnimator.GetCurrentAnimatorStateInfo(0).length, resolveTurnAnimator.GetCurrentAnimatorStateInfo(1).length);
        yield return new WaitForSecondsRealtime(animationLength + 0.5f);
    }

    IEnumerator PlayHealthAnimation() {
        DictKeys[] healthbarKeys = {DictKeys.PLAYER, DictKeys.ENEMY};
        Coroutine[] playingAnims = new Coroutine[2];

        for(int i=0; i<2; i++) {
            GameObject healthbar = ui.healthbars[healthbarKeys[i]];
            playingAnims[i] = StartCoroutine(PlayHealthAnimationFor(healthbar, results[i]));
        }

        for(int i=0; i<2; i++) {
            yield return playingAnims[i];
        }
    }

    IEnumerator PlayHealthAnimationFor(GameObject healthbar, SlotResult result) {
        float animationLength = 0;
        healthbar.GetComponent<SortingGroup>().sortingLayerName = "Cards in Focus";
        healthbar.GetComponentInChildren<TextMeshPro>().text = result.healthAfterResolution.ToString(); // TODO Later: Integrate this part into the Unity-animation with SetInt("newhealth", 10);

        if(result.TookDamage()) {
            healthbar.GetComponent<Animator>().SetBool("isDamaged", true);
            animationLength = healthbar.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        } else if(result.WasHealed()) {
            // TODO Later: Create Animation
        }

        yield return new WaitForSecondsRealtime(animationLength + 1);
        healthbar.GetComponent<SortingGroup>().sortingLayerName = "Default";
        healthbar.GetComponent<Animator>().SetBool("isDamaged", false);
    }

    IEnumerator ResetEverything() {
        for(int i=0; i<2; i++) {
            if(normalCards[i] == null) {
                continue;
            }

            normalCards[i].SetSortingLayer("Cards on Table");
            normalCards[i].transform.SetParent(normalCards[i].GetDeck().transform); // TODO: Trainwreck Card -> Deck transform
            foreach(SupportCard suppCard in supportCards[i]) {
                suppCard.SetSortingLayer("Attached Cards");
                suppCard.transform.SetParent(suppCard.GetDeck().transform); // TODO: Trainwreck Card -> Deck transform
                
            }
        }
        resolveTurnAnimator.SetBool("playAnim", false);
        yield return new WaitForSeconds(0.5f);
        
        if(closeAfterAnim) {
            ui.EnableInteractions(true);
            ui.dim.SetActive(false);
        }
    }
    #endregion
}