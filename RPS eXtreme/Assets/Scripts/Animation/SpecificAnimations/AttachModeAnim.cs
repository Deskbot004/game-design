using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Opens and Closes the Attach Mode
public class AttachModeAnim : Animation
{
    private GameObject dim;
    private bool open;

    public void Init(GameObject dim, bool open) {
        this.dim = dim;
        this.open = open;
        initialized = true;
    }

    protected override IEnumerator PlaySpecificAnimation() {
        dim.SetActive(open);
        yield return null;
    }

    protected override void SetAnimatedObjects() {
        animatedObjects.Add(dim);
    }
}