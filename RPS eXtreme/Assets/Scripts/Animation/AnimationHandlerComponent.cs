using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationHandlerComp : MonoBehaviour
{
    private AnimationQueue mainQueue = new();
    private Dictionary<AnimationQueueName, AnimationQueue> offQueues = new();

    public T CreateAnim<T>() where T:Animation {
        return gameObject.AddComponent<T>();
    }

    #region Queueing -------------------------------------------------------------------------------------------------
    public void QueueAnimation(Animation anim) {
        mainQueue.QueueAnimation(anim);
    }

    public void QueueAnimation(Animation anim, AnimationQueueName queueName) {
        if (!offQueues.ContainsKey(queueName)) {
            offQueues[queueName] = new();
        }
        offQueues[queueName].QueueAnimation(anim);
    }

    public void QueueAfterOffQueues(Animation anim) {
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

    public void PlayParallelToLastQueuedAnim(Animation anim, AnimationQueueName queueName) {
        offQueues[queueName].PlayParallelToLastQueuedAnim(anim);
    }
    #endregion

    #region Other -----------------------------------------------------------------------------------------------------
    // TODO Naming: Better names for all those functions
    void QueuePauseForOffQueues() {
        foreach(AnimationQueueName queueName in Enum.GetValues(typeof(AnimationQueueName))) {
            QueuePause(queueName);
        }
    }
    
    void QueuePause(AnimationQueueName queueName) {
        Pause anim = CreateAnim<Pause>();
        anim.SetPause(offQueues[queueName], true);
        offQueues[queueName].QueueAnimation(anim);
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

