using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pause : Animation 
{
    AnimationQueue animQueue;
    bool pause;

    public Pause() {
        initialized = true;
    }

    public void SetPause(AnimationQueue animQueue, bool pause) {
        this.animQueue = animQueue;
        this.pause = pause;
    }

    protected override void SetAnimatedObjects() {
        initialized = true;
        return;
    }

    protected override IEnumerator PlaySpecificAnimation(){
        animQueue.SetPause(pause);
        yield return null;
    }
}

public class WaitForQueues : Animation
{
    public Dictionary<AnimationQueueName, AnimationQueue> offQueues;

    public WaitForQueues() {
        initialized = true;
    }

    protected override void SetAnimatedObjects() {
        return;
    }

    protected override IEnumerator PlaySpecificAnimation(){
        while(offQueues.Where(q => !q.Value.IsPaused()).ToList().Count > 0) {
            yield return null;
        }
    }
}

public class Resume : Animation
{
    public Dictionary<AnimationQueueName, AnimationQueue> offQueues;

    public Resume() {
        initialized = true;
    }

    protected override void SetAnimatedObjects() {
        return;
    }

    protected override IEnumerator PlaySpecificAnimation(){
        foreach (AnimationQueue animQueue in offQueues.Values) {
            animQueue.Resume();
        }
        yield return null;
    }
}