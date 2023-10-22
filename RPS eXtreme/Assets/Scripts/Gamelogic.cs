using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamelogic : MonoBehaviour
{
    public int startDraw;
    public int turnDraw;
    public int lifepointMax;
    private TablePlayer[] players;
    private IDictionary<string, int> currentLifepoints = new Dictionary<string, int>();
    // [Non serialized]
    public Table table;

    /* Initialises the Gamelogic for the current game
     * 
     * Input: Table from which the game started.
     * Output: none
     */
    public void init(Table table)
    {
        // rework?
        Debug.Log("Game started");
        this.table = table;
        currentLifepoints.Add("user", lifepointMax);
        currentLifepoints.Add("enemy", lifepointMax);
        players = table.GetComponentsInChildren<TablePlayer>();

        foreach (TablePlayer p in players)
        {
            p.DrawCard(startDraw);
        }
        StartTurn();
    }
    
    /* Starts the turn by drawing cards.
     * 
     * Input: none
     * Output: none
     */
    void StartTurn()
    {
        Debug.Log("Turn started");
        foreach (TablePlayer p in players)
        {
            p.DrawCard(turnDraw);
        }
    }

    /* Should get triggered after user is finished for the turn. Evaluates each slot.
     * 
     * Input: none
     * Output: none
     */
    public void ResolveTurn()
    {
        Debug.Log("Turn resolve started");
        Slot[] slotsUser = table.GetSlotsPlayer();
        Slot[] slotsEnemy = table.GetSlotsEnemy();


        foreach (Slot slotUser in slotsUser)
        {
            foreach (Slot slotEnemy in slotsEnemy)
            {
                if(slotUser.GetPosition() == slotEnemy.GetPosition())
                {
                    slotUser.TurnCards();
                    slotEnemy.TurnCards();
                    string winner = EvaluateCards(slotUser.GetCard(), slotEnemy.GetCard());
                    table.ResolveSlot(slotUser.GetPosition(), winner);
                }
            }
        }
        table.ClearSlots();

        Debug.Log("user " + currentLifepoints["user"]);
        Debug.Log("enemy " + currentLifepoints["enemy"]);

        if (currentLifepoints["user"] <= 0)
        {
            GameEnd("enemy");
        } else if (currentLifepoints["enemy"] <= 0)
        {
            GameEnd("user");
        } else
        {
            StartTurn();
        }
    }

    /* Evaluation of the slots and deduction of life points for loss or empty slot.
    * 
    * Input: cardUser the card(s) played by the user, cardEnemy the card(s) played by the enemy
    * Output: string the Winner of the round or none
    */
    private string EvaluateCards(Card cardUser, Card cardEnemy)
    {   
        // if no card was played on either slot
        if (cardUser == null && cardEnemy == null)
        {
            currentLifepoints["user"] -= 1;
            return "none";
        } else if (cardEnemy == null)
        {
            currentLifepoints["enemy"] -= 1;
            return "user";
        } else if (cardUser == null)
        {
            currentLifepoints["enemy"] -= 1;
            return "user";
        }

        if (cardUser.GetValue() > cardEnemy.GetValue())
        {
            currentLifepoints["enemy"] -= 1;
            return "user";
        } else if(cardUser.GetValue() < cardEnemy.GetValue())
        {
            currentLifepoints["user"] -= 1;
            return "enemy";
        }

        return "none";
    }

    /* Function to notify the table if the game ended.
     * 
     * Input: s the winner of the game
     * Output: none
     */
    void GameEnd(string s)
    {
        Debug.Log("Game ended with winner " + s);
        table.SetWinner(s);
    }
}
