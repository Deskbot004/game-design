using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject menu;
    public GameObject options;
    public GameObject credits;

    public void LoadTable()
    {
        SceneManager.LoadSceneAsync("TestingScene");
    }

    public void openOptions()
    {
        this.menu.SetActive(false);
        this.options.SetActive(true);
    }

    public void closeOptions()
    {
        this.options.SetActive(false);
        this.menu.SetActive(true);
    }

    public void openCredits()
    {
        this.menu.SetActive(false);
        this.credits.SetActive(true);
    }

    public void closeCredits()
    {
        this.credits.SetActive(false);
        this.menu.SetActive(true);
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
