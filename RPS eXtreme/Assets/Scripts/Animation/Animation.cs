using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using UnityEngine;

// TODO Naming: Find better name, this one overshadows Unity's build in Script Animation
public class Animation : MonoBehaviour
{
    public bool initialized = false;
    public List<GameObject> animatedObjects = new();

    public IEnumerator Play() {
        Debug.Assert(initialized, "Trying to play uninitialized Animation " + this, this);
        SetAnimatedObjects();
        yield return PlaySpecificAnimation();
        DestroyAnim();
    }

    protected virtual void SetAnimatedObjects() {
        Debug.Log("Error: Animation doesn't set animated Objects", this);
    }

    public void DestroyAnim() {
        Destroy(this);
    }

    protected virtual IEnumerator PlaySpecificAnimation(){
        Debug.Log("Error: Animation doesn't override PlaySpecificAnimation", this);
        yield return null;
    }
}