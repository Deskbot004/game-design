using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseScreen : MonoBehaviour
{
    public TextMeshProUGUI winText;

    void Start()
    {
        if (!DataBetweenScreens.playerWon) winText.text = "You Lost!";
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("TestingScene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
