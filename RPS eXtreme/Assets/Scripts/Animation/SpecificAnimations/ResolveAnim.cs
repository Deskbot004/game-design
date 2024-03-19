using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

// Plays the Resolve Animation when pressing the End Turn Button
public class ResolveAnim : GameAnimation
{
    // Required
    private TableUI ui;
    private Animator resolveTurnAnimator;
    private SlotResult[] slotResults;
    
    // Options
    private bool lastResolve = false;

    // Internal
    private NormalCard[] normalCards;
    private List<SupportCard>[] supportCards;


    #region Main Functions -------------------------------------------------------------------------------------------
    public void Init(Table table, SlotResult playerResult, SlotResult enemyResult) {
        ui = table.ui;
        resolveTurnAnimator = table.resolveTurnAnimator;

        slotResults = new[] {playerResult, enemyResult};
        normalCards = new[] {playerResult.slot.GetCard(), enemyResult.slot.GetCard()};
        supportCards = new[] {normalCards[0] != null? normalCards[0].GetAttachedSupportCards() : new(), normalCards[1] != null? normalCards[1].GetAttachedSupportCards() : new()};

        initialized = true;
    }

    public void Options(bool closeAfterAnim = false) {
        this.lastResolve = closeAfterAnim;
    }

    protected override void SetAnimatedObjects() {
        for(int i=0; i<2; i++) {
            if(normalCards[i] != null) {
                animatedObjects.Add(normalCards[i].gameObject);
                animatedObjects.AddRange(supportCards[i].Select(c => c.gameObject));
            }
        }
    }

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
            newParent.position = slotResults[i].slot.transform.position;
            normalCards[i].transform.SetParent(newParent);
            normalCards[i].SetSortingLayer(SortingLayer.CARDS_IN_FOCUS);
            foreach(SupportCard suppCard in supportCards[i]) {
                suppCard.transform.SetParent(newParent);
                suppCard.SetSortingLayer(SortingLayer.ATTACHED_CARDS_IN_FOCUS);
            }
        }
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator PlayClashAnimation() {
        resolveTurnAnimator.SetBool("playerWon", slotResults[0].slotWon);
        resolveTurnAnimator.SetBool("enemyWon", slotResults[1].slotWon);
        resolveTurnAnimator.SetBool("playAnim", true);
        float animationLength = Mathf.Max(resolveTurnAnimator.GetCurrentAnimatorStateInfo(0).length, resolveTurnAnimator.GetCurrentAnimatorStateInfo(1).length);
        yield return new WaitForSecondsRealtime(animationLength + 0.5f);
    }

    IEnumerator PlayHealthAnimation() {
        TableSideName[] healthbarKeys = {TableSideName.PLAYER, TableSideName.ENEMY};
        Coroutine[] playingAnims = new Coroutine[2];

        for(int i=0; i<2; i++) {
            GameObject healthbar = ui.healthbars[healthbarKeys[i]];
            playingAnims[i] = StartCoroutine(PlayHealthAnimationFor(healthbar, slotResults[i]));
        }

        for(int i=0; i<2; i++) {
            yield return playingAnims[i];
        }
    }

    IEnumerator PlayHealthAnimationFor(GameObject healthbar, SlotResult result) {
        float animationLength = 0;
        healthbar.GetComponent<SortingGroup>().sortingLayerName = EnumUtils.SortingLayerName(SortingLayer.CARDS_IN_FOCUS);
        //healthbar.GetComponent<SortingGroup>().sortingLayerName = "Cards in Focus";
        healthbar.GetComponentInChildren<TextMeshPro>().text = result.healthAfterResolution.ToString(); // TODO Later: Integrate this part into the Unity-animation with SetInt("newhealth", 10);

        if(result.TookDamage()) {
            healthbar.GetComponent<Animator>().SetBool("isDamaged", true);
            animationLength = healthbar.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        } else if(result.WasHealed()) {
            // TODO: Healing Animation
        }

        yield return new WaitForSecondsRealtime(animationLength + 1);
        healthbar.GetComponent<SortingGroup>().sortingLayerName = EnumUtils.SortingLayerName(SortingLayer.DEFAULT);
        //healthbar.GetComponent<SortingGroup>().sortingLayerName = "Default";
        healthbar.GetComponent<Animator>().SetBool("isDamaged", false);
    }

    IEnumerator ResetEverything() {
        for(int i=0; i<2; i++) {
            if(normalCards[i] == null) {
                continue;
            }

            normalCards[i].SetSortingLayer(SortingLayer.CARDS_ON_TABLE);
            normalCards[i].SetDeckAsParent();
            foreach(SupportCard suppCard in supportCards[i]) {
                suppCard.SetSortingLayer(SortingLayer.ATTACHED_CARDS);
                suppCard.SetDeckAsParent();
                
            }
        }
        resolveTurnAnimator.SetBool("playAnim", false);
        yield return new WaitForSeconds(0.5f);
        
        if(lastResolve) {
            ui.EnableInteractions(true);
            ui.dim.SetActive(false);
        }
    }
    #endregion
}
