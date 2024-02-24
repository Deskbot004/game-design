using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AnimationQueueName {
    OPPONENT
}

public static class AnimationHandler
{
    static public AnimationHandlerComp animHandler;

    public static T CreateAnim<T>() where T:Animation {
        return animHandler.CreateAnim<T>();
    }

    public static void QueueAnimation(Animation anim) {
        animHandler.QueueAnimation(anim);
    }

    public static void QueueAnimation(Animation anim, AnimationQueueName queueName) {
        animHandler.QueueAnimation(anim, queueName);
    }

    public static void QueueAfterOffQueues(Animation anim) {
        animHandler.QueueAfterOffQueues(anim);
    }

    public static void PlayParallelToLastQueuedAnim(Animation anim) {
        animHandler.PlayParallelToLastQueuedAnim(anim);
    }

    public static void PlayParallelToLastQueuedAnim(Animation anim, AnimationQueueName queueName) {
        animHandler.PlayParallelToLastQueuedAnim(anim, queueName);
    }
}
