using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AnimationQueueName {
    ENEMY
}

public static class AnimationHandler
{
    static public AnimationHandlerComp animHandler;

    public static T CreateAnim<T>() where T:GameAnimation {
        return animHandler.CreateAnim<T>();
    }

    public static void QueueAnimation(GameAnimation anim) {
        animHandler.QueueAnimation(anim);
    }

    public static void QueueAnimation(GameAnimation anim, AnimationQueueName queueName) {
        animHandler.QueueAnimation(anim, queueName);
    }

    public static void QueueAnimationExclusively(GameAnimation anim) {
        animHandler.QueueAnimationExclusively(anim);
    }

    public static void PlayParallelToLastQueuedAnim(GameAnimation anim) {
        animHandler.PlayParallelToLastQueuedAnim(anim);
    }

    public static void PlayParallelToLastQueuedAnim(GameAnimation anim, AnimationQueueName queueName) {
        animHandler.PlayParallelToLastQueuedAnim(anim, queueName);
    }
}
