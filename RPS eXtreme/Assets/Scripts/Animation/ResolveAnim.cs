using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Write
public class ResolveAnim : Animation
{
    public bool userWon;
    public bool enemyWon;

    protected override IEnumerator PlaySpecificAnimation() {
        yield return null;
    }

    /*
    public IEnumerator playResolveAnimation() {
        yield return playCardClash();
        yield return new WaitForSecondsRealtime(0.5f);
        yield return playHealthAnimation();
    }

    public IEnumerator playCardClash() {

    }
     */
}