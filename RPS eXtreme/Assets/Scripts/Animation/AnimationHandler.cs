using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private Task currentAnimation;
    private List<Task> parallelAnimations = new();
    private List<List<Animation>> animationQueue = new();

    private List<GameObject> animatedObjects = new();

    public T CreateAnim<T> () where T:Animation {
        return gameObject.AddComponent<T>();
    }

    public void QueueAnimation(Animation anim) {
        // Put the animation into the queue
        // If there is no current animation running play it

        animationQueue.Add(new() {anim});
        if(currentAnimation == null || !currentAnimation.Running) {
            PlayNextAnimation();
        }
    }

    public void PlayParallelToLastQueuedAnim(Animation anim) {
        if (animationQueue.Count > 0) {
            animationQueue.Last().Add(anim);
        } else { // Meaning last queued animation is already playing
            PlayParallelToCurrentAnimation(anim);
        }
    }

    void PlayNextAnimation() {
        // Check whether there is an animation in the list
        // Create a Task for that animation and save it under current Animation
        // Make it Call this Function when it's finished
        // Remove animation from queue

        if (parallelAnimations.Count > 0) {
            return;
        } else if (animationQueue.Count > 0) {
            animatedObjects.Clear();

            List<Animation> animations = animationQueue[0];
            animationQueue.RemoveAt(0);

            Animation nextAnimation = animations[0];
            animations.RemoveAt(0);
            animatedObjects.AddRange(nextAnimation.animatedObjects);
            currentAnimation = new Task(nextAnimation.Play());
            currentAnimation.Finished += delegate(bool manual) {
                PlayNextAnimation();
            };

            foreach (Animation anim in animations) {
                PlayParallelToCurrentAnimation(anim);
            }
        }
    }

    void PlayParallelToCurrentAnimation(Animation anim) {
        Debug.Assert(IsOverlapZero(animatedObjects, anim.animatedObjects), "Parallel Animation trying to animate same Object as this animation", this);
        animatedObjects.AddRange(anim.animatedObjects);
        Task parallelAnim = new Task(anim.Play());
        parallelAnimations.Add(parallelAnim);
        parallelAnim.Finished += delegate(bool manual) {
            parallelAnimations.Remove(parallelAnim);
            PlayNextAnimation();
        };
    }

    bool IsOverlapZero(List<GameObject> firstList, List<GameObject> secondList) {
        return firstList.Intersect(secondList).ToList().Count == 0;
    }
}