using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // Determines the Win of a symbol on attack
    private int[,] winMatrix = { { -1, 1, 0, 1, 0 }, 
                                 { 0, -1, 1, 1, 0 }, 
                                 { 1, 0, -1, 0, 1 }, 
                                 { 0, 0, 1, -1, 1 }, 
                                 { 1, 1, 0, 0, -1 } };
    // TODO WIP translator of Symbol to Entry number 
    public Dictionary<string, int> symbolToEntry = new Dictionary<string, int>();


    // Tests
    public List<Card> testUser = new List<Card>();
    public List<Card> testEnemy = new List<Card>();




    /* Initialises the Gamelogic for the current game
     * 
     * Input: Table from which the game started.
     * Output: none
     */
    public void init(Table table)
    {
        // rework?
        symbolToEntry.Add("Scissors", 0);
        symbolToEntry.Add("Stone", 1);
        symbolToEntry.Add("Paper", 2);
        symbolToEntry.Add("Lizard", 3);
        symbolToEntry.Add("Spock", 4);

        Debug.Log("Game started");
        this.table = table;
        currentLifepoints.Add("user", lifepointMax);
        currentLifepoints.Add("enemy", lifepointMax);
        players = table.GetComponentsInChildren<TablePlayer>();

        foreach (TablePlayer p in players)
        {
            p.DrawCards(startDraw);
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
            p.DrawCards(turnDraw);
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
        List<Slot> slotsUser = table.GetSlotsPlayer();
        List<Slot> slotsEnemy = table.GetSlotsEnemy();


        foreach (Slot slotUser in slotsUser)
        {
            foreach (Slot slotEnemy in slotsEnemy)
            {
                if(slotUser.GetPosition() == slotEnemy.GetPosition())
                {
                    slotUser.TurnCards();
                    slotEnemy.TurnCards();
                    string winner = EvaluateCards(slotUser.GetCards(), slotEnemy.GetCards());
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
    private string EvaluateCards(List<Card> cardsUser, List<Card> cardsEnemy)
    {   
        // if no card was played on either slot
        if (!cardsUser.Any() && !cardsEnemy.Any())
        {
            return "none";
        } else if (!cardsEnemy.Any())
        {
            currentLifepoints["enemy"] -= 1;
            return "user";
        } else if (!cardsUser.Any())
        {
            currentLifepoints["user"] -= 1;
            return "ememy";
        }

        int symbolToEntryUser = 0;
        int symbolToEntryEnemy = 0;

        foreach (Card card in cardsUser)
        {
            if (card.isBasic())
            {
                symbolToEntryUser = symbolToEntry[card.GetSymbol()];
            }
        }

        foreach (Card card in cardsEnemy)
        {
            if (card.isBasic())
            {
                symbolToEntryEnemy = symbolToEntry[card.GetSymbol()];
            }
        }

        int attack = winMatrix[symbolToEntryUser,symbolToEntryEnemy];
        if (attack == -1)
        {
            Debug.Log("Draw");
            return "none";
        } else if (attack == 1)
        {
            Debug.Log("UserWon");
            currentLifepoints["enemy"] -= 1;
            return "user";
        } else if (attack == 0)
        {
            Debug.Log("EnemyWon");
            currentLifepoints["user"] -= 1;
            return "enemy";
        } else
        {
            Debug.Log("Error: unknown winstate");
            return "none";
        }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            symbolToEntry.Add("scissors", 0);
            symbolToEntry.Add("stone", 1);
            symbolToEntry.Add("paper", 2);
            symbolToEntry.Add("lizard", 3);
            symbolToEntry.Add("spock", 4);

            Debug.Log("Test Evaluate");
            currentLifepoints.Add("user", lifepointMax);
            currentLifepoints.Add("enemy", lifepointMax);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(EvaluateCards(testUser, testEnemy));
            Debug.Log("enemy: " + currentLifepoints["enemy"]);
            Debug.Log("user: " + currentLifepoints["user"]);
        }
    }
}
