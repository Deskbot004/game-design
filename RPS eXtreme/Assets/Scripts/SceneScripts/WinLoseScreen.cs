using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseScreen : MonoBehaviour
{
    public TextMeshProUGUI winText;

    public void showWinner(bool playerWon)
    {
        if(!playerWon) 
        {
            winText.text = "You Lost...";
        }
        gameObject.SetActive(true);
        GetComponent<Animator>().SetBool("playerWon", playerWon);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("AtTheTable");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
