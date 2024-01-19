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
    public LibBR libBR;
    public Table table;

    [Header("UI Connection")]
    public Health healthUI;

    // Determines the Win of a symbol on attack
    private int[,] winMatrix = { { -1, 1, 0, 1, 0 }, 
                                 { 0, -1, 1, 1, 0 }, 
                                 { 1, 0, -1, 0, 1 }, 
                                 { 0, 0, 1, -1, 1 }, 
                                 { 1, 1, 0, 0, -1 } };
    // TODO WIP translator of Symbol to Entry number 
    public Dictionary<string, int> symbolToEntry = new Dictionary<string, int>();


    // Shitty implementation of reset function
    public Dictionary<string, Action<Gamelogic, object>> stringToFunc = new Dictionary<string, Action<Gamelogic, object>>();
    public Dictionary<string, object> stringToInput = new Dictionary<string, object>();
    //public Dictionary<string, Action<Gamelogic, object>> stringToFunc;
    //public Dictionary<string, object> stringToInput;
    public int TestVar = 2020;

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
        symbolToEntry.Add("scissors", 0);
        symbolToEntry.Add("rock", 1);
        symbolToEntry.Add("paper", 2);
        symbolToEntry.Add("spock", 3);
        symbolToEntry.Add("lizard", 4);

        // Load Libs
        libAR = GetComponent<LibAR>();
        libBR = GetComponent<LibBR>();

        Debug.Log("Game started");
        this.table = table;
        currentLifepoints.Add("user", lifepointMax);
        currentLifepoints.Add("enemy", lifepointMax);
        healthUI.setHealth(lifepointMax, true);
        healthUI.setHealth(lifepointMax, false);
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
            if (!p.isPlayer)
            {
                p.StartCoroutine(p.playCards());
            }
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
                if(slotUser.GetSlotPosition() == slotEnemy.GetSlotPosition())
                {
                    slotUser.TurnCards();
                    slotEnemy.TurnCards();
                    string winner = EvaluateCards(slotUser.GetNormalAndSuppCards(), slotEnemy.GetNormalAndSuppCards());
                    table.ResolveSlot(slotUser.GetSlotPosition(), winner);
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

        int symbolToEntryUser = 0;
        int symbolToEntryEnemy = 0;

        List<(Action<Gamelogic, string, object>, object)> userARfunctions = new List<(Action<Gamelogic, string, object>, object)>();
        List<(Action<Gamelogic, string, object>, object)> userBRfunctions = new List<(Action<Gamelogic, string, object>, object)>();
        List<(Action<Gamelogic, string, object>, object)> userDrawfunctions = new List<(Action<Gamelogic, string, object>, object)>();

        List<(Action<Gamelogic, string, object>, object)> enemyARfunctions = new List<(Action<Gamelogic, string, object>, object)>();
        List<(Action<Gamelogic, string, object>, object)> enemyBRfunctions = new List<(Action<Gamelogic, string, object>, object)>();
        List<(Action<Gamelogic, string, object>, object)> enemyDrawfunctions = new List<(Action<Gamelogic, string, object>, object)>();


        foreach (Card card in cardsUser)
        {
            if (card.IsBasic())
            {
                symbolToEntryUser = symbolToEntry[card.GetSymbol()];
                Debug.Log(card.GetSymbol());
            }
            else
            {
                Debug.Log(card.GetSymbol());
                if (card.GetFunctionsAR().Any())
                {
                    userARfunctions.AddRange(card.GetFunctionsAR());
                }
                if (card.GetFunctionsBR().Any())
                {
                    userBRfunctions.AddRange(card.GetFunctionsBR());
                }
                if (card.GetFunctionsDraw().Any())
                {
                    userDrawfunctions.AddRange(card.GetFunctionsDraw());
                }
            }
        }

        foreach (Card card in cardsEnemy)
        {
            if (card.IsBasic())
            {
                symbolToEntryEnemy = symbolToEntry[card.GetSymbol()];
                Debug.Log(card.GetSymbol());
            }
            else
            {
                Debug.Log(card.GetSymbol());
                if (card.GetFunctionsAR().Any())
                {
                    enemyARfunctions.AddRange(card.GetFunctionsAR());
                }
                if (card.GetFunctionsBR().Any())
                {
                    enemyBRfunctions.AddRange(card.GetFunctionsBR());
                }
                if (card.GetFunctionsDraw().Any())
                {
                    enemyDrawfunctions.AddRange(card.GetFunctionsDraw());
                }
            }
        }
        if (userBRfunctions.Any())
        {
            libBR.RunAllBR(userBRfunctions, this, "user");
        }
        if (enemyBRfunctions.Any())
        {
            libBR.RunAllBR(enemyBRfunctions, this, "enemy");
        }
        

        attack = winMatrix[symbolToEntryUser,symbolToEntryEnemy];
        Debug.Log(symbolToEntryUser);
        Debug.Log(symbolToEntryEnemy);
        Debug.Log(winMatrix[symbolToEntryUser, symbolToEntryEnemy]);


        /* TODO: How to implement the call of the functions
            A Card should know its function plus its intensity -> How? Dictionary?
            This gets translated into a function -> How? IDK help
            The Resulting list gets executed -> Problem Timing, Variable accessibility
            
            Save Dictionary with function -> Input parameter
        */

        if (attack == -1)
        {
            Debug.Log("Draw");
            return "none";
        } else if (attack == 0)
        {
            Debug.Log("UserWon");
            if (userARfunctions.Any())
            {
                libAR.RunAllAR(userARfunctions, this, "user");
            }
            if (userDrawfunctions.Any())
            {
                libAR.RunAllAR(userDrawfunctions, this, "user");
            }
            if (enemyDrawfunctions.Any())
            {
                libAR.RunAllAR(enemyDrawfunctions, this, "enemy");
            }
            DamageEnemy(dmgOnLoss);
            return "user";
        } else if (attack == 1)
        {
            Debug.Log("EnemyWon");
            if (enemyARfunctions.Any())
            {
                libAR.RunAllAR(enemyARfunctions, this, "enemy");
            }
            if (userDrawfunctions.Any())
            {
                libAR.RunAllAR(userDrawfunctions, this, "user");
            }
            if (enemyDrawfunctions.Any())
            {
                libAR.RunAllAR(enemyDrawfunctions, this, "enemy");
            }
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
            symbolToEntry.Add("rock", 1);
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
            libAR = GetComponent<LibAR>();
            List<Action<Gamelogic, String>> actions = new List<Action<Gamelogic, String>>();
            actions.Add(libAR.Test);
            libAR.RunTest(actions, this);
            Debug.Log(TestVar);
        }

        // Proof of Concept Reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (var item in stringToInput) {
                Action<Gamelogic,object> func = stringToFunc[item.Key];
                func(this, item.Value);
                Debug.Log(TestVar);
                stringToFunc.Remove(item.Key);
                stringToInput.Remove(item.Key);
            }
        }
    }

    public void DamageUser(int dmg)
    {
        currentLifepoints["user"] -= dmg;
        healthUI.setHealth(currentLifepoints["user"], true);
        
    }

    public void DamageEnemy(int dmg)
    {
        currentLifepoints["enemy"] -= dmg;
        healthUI.setHealth(currentLifepoints["enemy"], false);
    }

    public void UserDraw(int amount)
    {
        foreach (TablePlayer p in players)
        {
            if (p.isPlayer) {
                p.DrawCards(amount);
            }
        }
    }

    public void EnemyDraw(int amount)
    {
        foreach (TablePlayer p in players)
        {
            if (p.isPlayer)
            {
                p.DrawCards(amount);
            }
        }
    }

    // Start of various getter stuff -------------------------------------------------------------------------------------------------
    public void SetStartDraw(int startDraw)
    {
        this.startDraw = startDraw;
    }

    public void SetTurnDraw(int turnDraw)
    {
        this.turnDraw = turnDraw;
    }
    public void SetlifepointMax(int lifepointMax)
    {
        this.lifepointMax = lifepointMax;
    }

    public void SetdmgOnLoss(int dmgOnLoss)
    {
        this.dmgOnLoss = dmgOnLoss;
    }

    public void SetwinMatrix(int[,] winMatrix)
    {
        this.winMatrix = winMatrix;
    }

    // Start of various setter stuff -------------------------------------------------------------------------------------------------
    public int GetStartDraw()
    {
        return this.startDraw;
    }

    public int GetTurnDraw()
    {
        return this.turnDraw;
    }
    public int GetlifepointMax()
    {
        return this.lifepointMax;
    }

    public int GetdmgOnLoss()
    {
        return this.dmgOnLoss;
    }

    public int[,] GetwinMatrix()
    {
        return this.winMatrix;
    }
}
