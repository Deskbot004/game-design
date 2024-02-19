using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    // Determines the Win of a symbol on attack
    // Currently -1 Draw; 0 User win; 1 enemy win
    // Want: User > 0; Draw = 0; Enemy < 0
    private int[,] winMatrix = { { 0, -1, 1, -1, 1 }, 
                                 { 1, 0, -1, -1, 1 }, 
                                 { -1, 1, 0, 1, -1 }, 
                                 { 1, 1, -1, 0, -1 }, 
                                 { -1, -1, 1, 1, 0 } };
    public Dictionary<string, int> symbolToEntry = new Dictionary<string, int>();


    // Shitty implementation of reset function
    public Dictionary<string, Action<Gamelogic, object>> stringToFunc = new Dictionary<string, Action<Gamelogic, object>>();
    public Dictionary<string, object> stringToInput = new Dictionary<string, object>();
    
    // Temporary Globals



    /* Initialises the Gamelogic for the current game
    * 
    * Input: Table from which the game started.
    * Output: none
    */
    public void Init(Table table)
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

        this.table = table;
        currentLifepoints.Add("user", lifepointMax);
        currentLifepoints.Add("enemy", lifepointMax);
        table.ui.SetHealth(lifepointMax, "player");
        table.ui.SetHealth(lifepointMax, "enemy");
        this.players = table.GetComponentsInChildren<TablePlayer>();

        EnemyDraw(startDraw + turnDraw);
        UserDraw(startDraw + turnDraw);
    }
    
    /* Starts the turn by drawing cards.
    * 
    * Input: none
    * Output: none
    */
    void StartTurn()
    {
        foreach (TablePlayer p in players)
        {
            if(p.hand.GetCards().Count == 0) p.DrawCards(startDraw);
            else p.DrawCards(turnDraw);
        }
    }

    /* Should get triggered after user is finished for the turn. Evaluates each slot.
    * 
    * Input: none
    * Output: none
    */
    public void ResolveTurn()
    {
        table.StartResolve();
        foreach ((Slot slotPlayer, Slot slotEnemy) in table.GetSlotsForResolving()) {
            string winner = EvaluateCards(slotPlayer.GetNormalAndSuppCards(), slotEnemy.GetNormalAndSuppCards());
            table.PlayResolveAnimation(slotPlayer.slotPosition, winner, currentLifepoints);
            stringToInput.Reverse();
            foreach (var item in stringToInput) {
                Action<Gamelogic,object> func = stringToFunc[item.Key];
                func(this, item.Value);
            }
            stringToInput.Clear();
            stringToFunc.Clear();
        }
        table.EndResolve();
        table.ClearSlots();

        if (currentLifepoints["user"] <= 0) {
            GameEnd("enemy");
        } else if (currentLifepoints["enemy"] <= 0) {
            GameEnd("user");
        } else {
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
        int attack = 0;
        bool skipEval = false;

        int symbolToEntryUser = 0;
        int symbolToEntryEnemy = 0;

        List<(Action<Gamelogic, string, object>, object)> userARfunctions = new List<(Action<Gamelogic, string, object>, object)>();
        List<(Action<Gamelogic, string, object>, object)> userBRfunctions = new List<(Action<Gamelogic, string, object>, object)>();
        List<(Action<Gamelogic, string, object>, object)> userDrawfunctions = new List<(Action<Gamelogic, string, object>, object)>();

        List<(Action<Gamelogic, string, object>, object)> enemyARfunctions = new List<(Action<Gamelogic, string, object>, object)>();
        List<(Action<Gamelogic, string, object>, object)> enemyBRfunctions = new List<(Action<Gamelogic, string, object>, object)>();
        List<(Action<Gamelogic, string, object>, object)> enemyDrawfunctions = new List<(Action<Gamelogic, string, object>, object)>();


        if(!cardsUser.Any() && !cardsEnemy.Any()) //automatic draw because no card was played
        {
            skipEval = true;
        }
        else if (!cardsUser.Any()) //automatic win for enemy because user didn't play cards
        {
            skipEval = true;
            attack = -99;
        }
        else if (!cardsEnemy.Any()) //automatic win for user because enemy didn't play cards
        {
            skipEval = true;
            attack = 99;
        }

        foreach (Card card in cardsUser)
        {
            if (card.IsBasic())
            {
                symbolToEntryUser = symbolToEntry[card.GetSymbol()];
            }
            else
            {
                
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
            }
            else
            {
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

        if (!skipEval)
        {
            attack = winMatrix[symbolToEntryUser, symbolToEntryEnemy];
        }

        if (attack == 0)
        {
            return "none";
        } else if (attack > 0)
        {
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
        } else if (attack < 0)
        {
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
        table.SetWinner(s);
    }


    public void DamageUser(int dmg)
    {
        currentLifepoints["user"] -= dmg;
    }

    public void DamageEnemy(int dmg)
    {
        currentLifepoints["enemy"] -= dmg;
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
            if (!p.isPlayer)
            {
                p.DrawCards(amount);
            }
        }
    }

    // Start of various setter stuff -------------------------------------------------------------------------------------------------
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

    // Start of various getter stuff -------------------------------------------------------------------------------------------------
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
