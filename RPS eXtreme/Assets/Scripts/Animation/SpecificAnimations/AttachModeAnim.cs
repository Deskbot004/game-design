using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Opens and Closes the Attach Mode
public class AttachModeAnim : GameAnimation
{
    // Required
    private GameObject dim; // TODO: dim -> screenDim (everywhere not just here)
    private bool playOpenAnim;


    public void Init(GameObject dim, bool playOpenAnim) {
        this.dim = dim;
        this.playOpenAnim = playOpenAnim;
        initialized = true;
    }

    protected override IEnumerator PlaySpecificAnimation() {
        dim.SetActive(playOpenAnim);
        yield return null;
    }

    protected override void SetAnimatedObjects() {
        animatedObjects.Add(dim);
    }
}