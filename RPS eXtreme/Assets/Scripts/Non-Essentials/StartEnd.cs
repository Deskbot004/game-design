using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEnd : MonoBehaviour
{
    // Only works in Editor
    private void OnApplicationQuit()
    {
        Debug.Log("The program would now close itself");
    }

    // Gets called if user leaves through button
    // Sadly the windows X or task manager is not detectable
    public void exitGame()
    {
        //Screen.SetResolution(1920, 1080, false);
        Application.Quit();
    }

    // Does not work before splash screen :(
    // But before first scene ig...
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void Begin() {
        //Screen.SetResolution(1920, 1080, false);
    }
}
