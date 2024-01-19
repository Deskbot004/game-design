using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject menu;
    public GameObject options;
    public GameObject credits;
    public GameObject selection;

    public string selectedPlayerDeckName;
    public string selectedOpponentDeckName;

    public void Awake()
    {
        this.selectedOpponentDeckName = "rpsOpponentStandard";
        this.selectedPlayerDeckName = "rpsPlayerStandard";
    }


    /*
     * Finds the selected deck and saves them for later use.
     * Then it loads the TableScene.
     */

    public void LoadTable()
    {
        GameObject opponentDeckObject = GameObject.Find(this.selectedOpponentDeckName);
        Debug.Log(opponentDeckObject);
        Deck opponentDeck = opponentDeckObject.GetComponent<Deck>();
        GameObject playerDeckObject = GameObject.Find(this.selectedPlayerDeckName);
        Deck playerDeck = playerDeckObject.GetComponent<Deck>();
        if(playerDeck != null && opponentDeck != null)
        {
            opponentDeck.SaveDeck();
            playerDeck.SaveDeck();
            SceneManager.LoadSceneAsync(1);
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

        this.selectedOpponentDeckName = valueToDeckname[value];

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

        this.selectedPlayerDeckName = valueToDeckname[value];

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
