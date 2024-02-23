using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum AnimationOffQueues {
    OPPONENT
}

public class AnimationHandler : MonoBehaviour
{
    private AnimationQueue mainQueue = new();
    private AnimationQueue[] offQueues = new AnimationQueue[Enum.GetNames(typeof(AnimationOffQueues)).Length];

    public T CreateAnim<T>() where T:Animation {
        return gameObject.AddComponent<T>();
    }

    #region Queueing -------------------------------------------------------------------------------------------------
    public void QueueAnimation(Animation anim) {
        mainQueue.QueueAnimation(anim);
    }

    public void QueueAnimation(Animation anim, int animQueueIndex) {
        if (offQueues.ElementAtOrDefault(animQueueIndex) == null) {
            offQueues[animQueueIndex] = new();
        }
        offQueues[animQueueIndex].QueueAnimation(anim);
    }

    public void QueueAfterOffQueues(Animation anim) {
        // TODO: Find better names for all those functions
        QueuePauseForOffQueues();
        QueueWaitForOffQueues();
        QueueAnimation(anim);
        QueueResumeForOffQueues();
    }
    #endregion

    #region Parallel -------------------------------------------------------------------------------------------------
    public void PlayParallelToLastQueuedAnim(Animation anim) {
        mainQueue.PlayParallelToLastQueuedAnim(anim);
    }

    public void PlayParallelToLastQueuedAnim(Animation anim, int animQueueIndex) {
        offQueues[animQueueIndex].PlayParallelToLastQueuedAnim(anim);
    }
    #endregion

    #region Other -----------------------------------------------------------------------------------------------------
    void QueuePauseForOffQueues() {
        for(int i = 0; i<offQueues.Length; i++) {
            QueuePause(i);
        }
    }
    
    void QueuePause(int animQueueIndex) {
        Pause anim = CreateAnim<Pause>();
        anim.SetPause(offQueues[animQueueIndex], true);
        offQueues[animQueueIndex].QueueAnimation(anim);
    }

    void QueueWaitForOffQueues() {
        WaitForQueues anim = CreateAnim<WaitForQueues>();
        anim.offQueues = offQueues;
        QueueAnimation(anim);
    }

    void QueueResumeForOffQueues() {
        Resume anim = CreateAnim<Resume>();
        anim.offQueues = offQueues;
        QueueAnimation(anim);
    }
    #endregion
}



public class Pause : Animation 
{
    AnimationQueue animQueue;
    bool pause;

    public void SetPause(AnimationQueue animQueue, bool pause) {
        this.animQueue = animQueue;
        this.pause = pause;
    }

    protected override void SetAnimatedObjects() {
        return;
    }

    protected override IEnumerator PlaySpecificAnimation(){
        animQueue.SetPause(pause);
        yield return null;
    }
}

public class WaitForQueues : Animation
{
    public AnimationQueue[] offQueues;

    protected override void SetAnimatedObjects() {
        return;
    }

    protected override IEnumerator PlaySpecificAnimation(){
        while(offQueues.Where(q => !q.IsPaused()).ToList().Count > 0) {
            yield return null;
        }
    }
}

public class Resume : Animation
{
    public AnimationQueue[] offQueues;

    protected override void SetAnimatedObjects() {
        return;
    }

    protected override IEnumerator PlaySpecificAnimation(){
        foreach (AnimationQueue animQueue in offQueues) {
            animQueue.Resume();
        }
        yield return null;
    }
}