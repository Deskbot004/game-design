using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject options;
    public GameObject credits;
    public GameObject selection;

    public string selectedPlayerDeckName;
    public string selectedOpponentDeckName;

    public TextMeshProUGUI playerSelectedText;
    public TextMeshProUGUI enemySelectedText;

    public void Awake()
    {
        this.selectedOpponentDeckName = "rpsOpponentStandard";
        this.selectedPlayerDeckName = "rpsPlayerStandard";
        Deck playerDeck = GameObject.Find(this.selectedPlayerDeckName).GetComponent<Deck>();
        Deck opponentDeck = GameObject.Find(this.selectedOpponentDeckName).GetComponent<Deck>();
        this.playerSelectedText.text = playerDeck.flavor;
        this.enemySelectedText.text = opponentDeck.flavor;
    }


    /*
     * Finds the selected deck and saves them for later use.
     * Then it loads the TableScene.
     */

    public void LoadTable()
    {
        GameObject opponentDeckObject = GameObject.Find(this.selectedOpponentDeckName);
        Deck opponentDeck = opponentDeckObject.GetComponent<Deck>();
        GameObject playerDeckObject = GameObject.Find(this.selectedPlayerDeckName);
        Deck playerDeck = playerDeckObject.GetComponent<Deck>();
        if(playerDeck != null && opponentDeck != null)
        {
            opponentDeck.SaveDeck();
            playerDeck.SaveDeck();
            SceneManager.LoadSceneAsync("Main Game");
        }
        

        
    }

    /*
     * Converts the selected value in the Opponent DropDown Menu into its corresponding DeckName.
     * It then set the DeckName as the currently selected Deck.
     */

    public void HandleRPSOpponentSelection(int value)
    {
        Dictionary<int, string> valueToDeckname = new Dictionary<int, string>();
        valueToDeckname[0] = "rpsOpponentStandard";
        valueToDeckname[1] = "rpsOpponentNoSupport";
        valueToDeckname[2] = "rpsOpponentRockHeavy";
        valueToDeckname[3] = "rpsOpponentScissorHeavy";
        valueToDeckname[4] = "rpsOpponentPaperHeavy";
        valueToDeckname[5] = "rpsOpponentLifesteal";

        this.selectedOpponentDeckName = valueToDeckname[value];
        Deck opponentDeck = GameObject.Find(this.selectedOpponentDeckName).GetComponent<Deck>();
        this.enemySelectedText.text = opponentDeck.flavor;

    }

    /*
     * Converts the selected value in the Player DropDown Menu into its corresponding DeckName.
     * It then set the DeckName as the currently selected Deck.
     */

    public void HandleRPSPlayerSelection(int value)
    {
        Dictionary<int, string> valueToDeckname = new Dictionary<int, string>();
        valueToDeckname[0] = "rpsPlayerStandard";
        valueToDeckname[1] = "rpsPlayerNoSupport";
        valueToDeckname[2] = "rpsPlayerPaperHeavy";
        valueToDeckname[3] = "rpsPlayerRockHeavy";
        valueToDeckname[4] = "rpsPlayerScissorHeavy";
        valueToDeckname[5] = "rpsPlayerLifesteal";

        this.selectedPlayerDeckName = valueToDeckname[value];
        Deck playerDeck = GameObject.Find(this.selectedPlayerDeckName).GetComponent<Deck>();
        this.playerSelectedText.text = playerDeck.flavor;

    }

    /*
     * Opens the Selection Menu.
     */

    public void openSelection()
    {
        this.menu.SetActive(false);
        this.selection.SetActive(true);
    }

    /*
     * Closes the Selection Menu.
     */

    public void closeSelection()
    {
        this.selection.SetActive(false);
        this.menu.SetActive(true);
    }

    /*
     * Opens the Options Menu.
     */

    public void openOptions()
    {
        this.menu.SetActive(false);
        this.options.SetActive(true);
    }

    /*
     * Closes the Options Menu.
     */

    public void closeOptions()
    {
        this.options.SetActive(false);
        this.menu.SetActive(true);
    }

    /*
     * Opens the Credits Menu.
     */

    public void openCredits()
    {
        this.menu.SetActive(false);
        this.credits.SetActive(true);
    }

    /*
     * Closes the Credits Menu.
     */

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
