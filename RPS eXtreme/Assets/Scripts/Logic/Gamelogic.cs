using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gamelogic : MonoBehaviour
{
    public int startDraw = 5;
    public int turnDraw = 1;
    public int lifepointMax = 10;
    public int dmgOnLoss = 1;


    private TablePlayer[] players;
    private IDictionary<string, int> currentLifepoints = new Dictionary<string, int>();
    // [Non serialized]
    public LibAR libAR;
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
    
    // Temporary Globals



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

        // Load Libs
        libAR = GetComponent<LibAR>();

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
        int attack = -1;

        // if no card was played on either slot
        if (!cardsUser.Any() && !cardsEnemy.Any())
        {
            return "none";
        } else if (!cardsEnemy.Any())
        {
            // TODO: NOOOOO cant skip every BR function FUCK
            attack = 1;
            goto AR;
        } else if (!cardsUser.Any())
        {
            attack = 0;
            goto AR;
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
            // TODO: Read effects here -> Translate to Functions
        }

        attack = winMatrix[symbolToEntryUser,symbolToEntryEnemy];
        
    AR:
        /* TODO: How to implement the call of the functions
            A Card should know its function plus its intensity -> How? Dictionary?
            This gets translated into a function -> How? IDK help
            The Resulting list gets executed -> Problem Timing, Variable accessibility
        */
        if (attack == -1)
        {
            Debug.Log("Draw");
            return "none";
        } else if (attack == 1)
        {
            Debug.Log("UserWon");
            DamageEnemy(dmgOnLoss);
            return "user";
        } else if (attack == 0)
        {
            Debug.Log("EnemyWon");
            DamageUser(dmgOnLoss);
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
        // Setup with "S"
        if (Input.GetKeyDown(KeyCode.S))
        {
            symbolToEntry.Add("scissors", 0);
            symbolToEntry.Add("stone", 1);
            symbolToEntry.Add("paper", 2);
            symbolToEntry.Add("lizard", 3);
            symbolToEntry.Add("spock", 4);

            // Load Libs
            libAR = GetComponent<LibAR>();

            Debug.Log("Test Evaluate");
            currentLifepoints.Add("user", lifepointMax);
            currentLifepoints.Add("enemy", lifepointMax);
        }

        // Play Turn with "Space"
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(EvaluateCards(testUser, testEnemy));
            Debug.Log("enemy: " + currentLifepoints["enemy"]);
            Debug.Log("user: " + currentLifepoints["user"]);
        }

        // Proof of Concept calling function list with "F"
        if (Input.GetKeyDown(KeyCode.F))
        {
            List<Action> actions = new List<Action>();
            actions.Add(libAR.Test);
            libAR.RunTest(actions);
        }
    }

    void DamageUser(int dmg)
    {
        currentLifepoints["user"] -= dmg;
    }

    void DamageEnemy(int dmg)
    {
        currentLifepoints["enemy"] -= dmg;
    }

    // Start of various getter stuff -------------------------------------------------------------------------------------------------
    void SetStartDraw(int startDraw)
    {
        this.startDraw = startDraw;
    }

    void SetTurnDraw(int turnDraw)
    {
        this.turnDraw = turnDraw;
    }
    void SetlifepointMax(int lifepointMax)
    {
        this.lifepointMax = lifepointMax;
    }

    void SetdmgOnLoss(int dmgOnLoss)
    {
        this.dmgOnLoss = dmgOnLoss;
    }

    void SetwinMatrix(int[,] winMatrix)
    {
        this.winMatrix = winMatrix;
    }

    // Start of various setter stuff -------------------------------------------------------------------------------------------------
    int GetStartDraw()
    {
        return this.startDraw;
    }

    int GetTurnDraw()
    {
        return this.turnDraw;
    }
    int GetlifepointMax()
    {
        return this.lifepointMax;
    }

    int GetdmgOnLoss()
    {
        return this.dmgOnLoss;
    }

    int[,] GetwinMatrix()
    {
        return this.winMatrix;
    }
}
