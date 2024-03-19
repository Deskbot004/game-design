using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationQueue
{
    private Task currentAnimation;
    private List<Task> parallelAnimations = new();
    private List<GameObject> animatedObjects = new();

    private List<List<GameAnimation>> upcomingAnimations = new();
    private bool paused = false;

    #region Queueing -------------------------------------------------------------------------------------------------
    public void QueueAnimation(GameAnimation anim) {
        upcomingAnimations.Add(new() {anim});
        PlayNextAnimation();
    }

    void PlayNextAnimation() {
        if(paused) return;

        if (currentAnimation != null && (currentAnimation.Running || parallelAnimations.Count > 0)) {
            return;
        } else if (upcomingAnimations.Count > 0) { // Check whether there is an animation in the list
            animatedObjects.Clear();

            List<GameAnimation> animations = upcomingAnimations[0];
            upcomingAnimations.RemoveAt(0);

            GameAnimation nextAnimation = animations[0];
            animations.RemoveAt(0);
            animatedObjects.AddRange(nextAnimation.animatedObjects);
            currentAnimation = new Task(nextAnimation.Play());
            currentAnimation.Finished += delegate(bool manual) {
                PlayNextAnimation();
            };

            foreach (GameAnimation anim in animations) {
                PlayParallelToCurrentAnimation(anim);
            }
        }
    }
    #endregion

    #region Parallel -------------------------------------------------------------------------------------------------
    public void PlayParallelToLastQueuedAnim(GameAnimation anim) {
        if (upcomingAnimations.Count > 0) {
            upcomingAnimations.Last().Add(anim);
        } else { // Case that the last queued animation is already playing
            PlayParallelToCurrentAnimation(anim);
        }
    }

    void PlayParallelToCurrentAnimation(GameAnimation anim) {
        Debug.AssertFormat(!AreIntersecting(animatedObjects, anim.animatedObjects), "Animation {0} is trying to animation same objects as {1} ({2})", anim, currentAnimation, animatedObjects);
        animatedObjects.AddRange(anim.animatedObjects);
        Task parallelAnim = new Task(anim.Play());
        parallelAnimations.Add(parallelAnim);
        parallelAnim.Finished += delegate(bool manual) {
            parallelAnimations.Remove(parallelAnim);
            PlayNextAnimation();
        };
    }

    bool AreIntersecting(List<GameObject> firstList, List<GameObject> secondList) {
        return firstList.Intersect(secondList).ToList().Count > 0;
    }
    #endregion

    #region Other -----------------------------------------------------------------------------------------------------
    public void SetPause(bool enabled) {
        paused = enabled;
    }

    public bool IsPaused() {
        return paused;
    }

    public void Resume() {
        paused = false;
        PlayNextAnimation();
    }
    #endregion
}