using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationHandlerComp : MonoBehaviour
{
    private AnimationQueue mainQueue = new();
    private Dictionary<AnimationQueueName, AnimationQueue> offQueues = new();

    public T CreateAnim<T>() where T:GameAnimation {
        return gameObject.AddComponent<T>();
    }

    #region Queueing -------------------------------------------------------------------------------------------------
    public void QueueAnimation(GameAnimation anim) {
        mainQueue.QueueAnimation(anim);
    }

    public void QueueAnimation(GameAnimation anim, AnimationQueueName queueName) {
        if (!offQueues.ContainsKey(queueName)) {
            offQueues[queueName] = new();
        }
        offQueues[queueName].QueueAnimation(anim);
    }

    public void QueueAnimationExclusively(GameAnimation anim) {
        Queue_PauseInOffQueues();
        Queue_WaitForOffQueues();
        QueueAnimation(anim);
        Queue_ResumeOffQueues();
    }
    #endregion

    #region Parallel -------------------------------------------------------------------------------------------------
    public void PlayParallelToLastQueuedAnim(GameAnimation anim) {
        mainQueue.PlayParallelToLastQueuedAnim(anim);
    }

    public void PlayParallelToLastQueuedAnim(GameAnimation anim, AnimationQueueName queueName) {
        offQueues[queueName].PlayParallelToLastQueuedAnim(anim);
    }
    #endregion

    #region Util -----------------------------------------------------------------------------------------------------
    void Queue_PauseInOffQueues() {
        foreach(AnimationQueueName queueName in Enum.GetValues(typeof(AnimationQueueName))) {
            Queue_Pause(queueName);
        }
    }
    
    void Queue_Pause(AnimationQueueName queueName) {
        Pause anim = CreateAnim<Pause>();
        anim.SetPause(offQueues[queueName], true);
        offQueues[queueName].QueueAnimation(anim);
    }

    void Queue_WaitForOffQueues() {
        WaitForQueues anim = CreateAnim<WaitForQueues>();
        anim.offQueues = offQueues;
        QueueAnimation(anim);
    }

    void Queue_ResumeOffQueues() {
        Resume anim = CreateAnim<Resume>();
        anim.offQueues = offQueues;
        QueueAnimation(anim);
    }
    #endregion
}
