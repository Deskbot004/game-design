using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public bool fadein = false;
    public bool fadeout = false;
    public float timeToFade;
 
    // Update is called once per frame
    void Update()
    {
        if (fadein) {
            if (canvasGroup.alpha < 1) {
                canvasGroup.alpha += timeToFade * Time.deltaTime;
                if (canvasGroup.alpha >= 1) {
                    fadein = false;
                }
            }
        }

        if (fadeout) {
            if (canvasGroup.alpha >= 0) {
                canvasGroup.alpha -= timeToFade * Time.deltaTime;
                if (canvasGroup.alpha == 0) {
                    fadeout = false;
                }
            }
        }
    }

    public void FadeIn() {
        fadein = true;
    }

    public void FadeOut() {
        fadeout = true;
    }
}
