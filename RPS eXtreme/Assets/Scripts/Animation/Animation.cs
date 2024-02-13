using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using UnityEngine;

// TODO: Find better name, this one overshadows Unity's build in Script Animation
public class Animation : MonoBehaviour
{
    public List<GameObject> animatedObjects = new();

    public IEnumerator Play() {
        SetAnimatedObjects();
        yield return PlaySpecificAnimation();
        DestroyAnim();
    }

    protected virtual void SetAnimatedObjects() {
        Debug.Log("Error: Animation doesn't set animated Objects", this);
    }

    protected void DestroyAnim() {
        Destroy(this);
    }

    protected virtual IEnumerator PlaySpecificAnimation(){
        yield return null;
    }
}