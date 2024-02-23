using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttachModeAnim : Animation
{
    public GameObject dim;
    public bool open;

    protected override IEnumerator PlaySpecificAnimation() {
        dim.SetActive(open);
        yield return null;
    }

    protected override void SetAnimatedObjects() {
        animatedObjects.Add(dim);
    }
}