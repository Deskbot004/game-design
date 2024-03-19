using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gamelogic : MonoBehaviour
{
    public int startDraw = 5;
    public int turnDraw = 1;
    public int maxHealth = 10;
    public int dmgOnLoss = 1;

    private TableSide[] players;
    private IDictionary<TableSideName, int> currentHealth = new Dictionary<TableSideName, int>();
    // [Non serialized]
    private Table table;
    private FunctionHandler functionHandler;

    // Determines the Win of a symbol on attack
    // User > 0; Draw = 0; Enemy < 0
    private List<CardSymbol> winMatrixIndex = new() {CardSymbol.SCISSORS, CardSymbol.ROCK, CardSymbol.PAPER, CardSymbol.SPOCK, CardSymbol.LIZARD};
    private int[,] winMatrix = { { 0, -1, 1, -1, 1 }, 
                                 { 1, 0, -1, -1, 1 }, 
                                 { -1, 1, 0, 1, -1 }, 
                                 { 1, 1, -1, 0, -1 }, 
                                 { -1, -1, 1, 1, 0 } };
    // Temporary Globals

    public void Init(Table table) {
        functionHandler = new(this);
        this.table = table;
        currentHealth.Add(TableSideName.PLAYER, maxHealth);
        currentHealth.Add(TableSideName.ENEMY, maxHealth);
        table.ui.SetHealth(maxHealth, TableSideName.PLAYER);
        table.ui.SetHealth(maxHealth, TableSideName.ENEMY);
        this.players = table.GetComponentsInChildren<TableSide>();

        DrawCards(startDraw + turnDraw, TableSideName.PLAYER);
        DrawCards(startDraw + turnDraw, TableSideName.ENEMY);
    }
    
    void StartTurn() {
        foreach (TableSide p in players) {
            TableSideName cardDrawer = p.isPlayer? TableSideName.PLAYER : TableSideName.ENEMY;
            if(p.GetCardsInHand().Count == 0)
                table.DrawCards(startDraw, cardDrawer);
            else
                table.DrawCards(turnDraw, cardDrawer);
        }
    }

    public void ResolveTurn() {
        foreach ((Slot slotPlayer, Slot slotEnemy) in table.GetMatchingSlots()) {
            int[] prevHealth = {currentHealth[TableSideName.PLAYER], currentHealth[TableSideName.ENEMY]};
            bool[] winner = EvaluateCards(slotPlayer.GetNormalAndSuppCards(), slotEnemy.GetNormalAndSuppCards());
            SlotResult playerResult = new(slotPlayer, winner[0], prevHealth[0], currentHealth[TableSideName.PLAYER]);
            SlotResult enemyResult = new(slotEnemy, winner[1], prevHealth[1], currentHealth[TableSideName.ENEMY]);
            table.PlaySlotResolveAnim(playerResult, enemyResult, slotPlayer.slotPosition == table.player.slots.Count - 1);
            functionHandler.ResetEverything();
        }
        table.ClearSlots();

        if (currentHealth[TableSideName.PLAYER] <= 0) {
            EndGame(TableSideName.ENEMY);
        } else if (currentHealth[TableSideName.ENEMY] <= 0) {
            EndGame(TableSideName.PLAYER);
        } else {
            StartTurn();
        }
    }

    public void ExtractFunctions(List<Card> cards) {
        foreach (Card card in cards) {
            if (!card.IsNormal()) {
                functionHandler.AddFunctions(((SupportCard) card).GetFunctionsForResolve());
            }
        }
    }

    public CardSymbol ExtractNormalCard(List<Card> cards) {
        List<Card> trimmedList = cards.Where(c => c is NormalCard).ToList();
        Debug.Assert(trimmedList.Count == 1, "Error in extracting Normal Card: Provided List has multiple or no Normal Cards", this);
        return trimmedList[0].GetSymbol();
    }

    public int SymbolToInt(CardSymbol symbol) {
        return winMatrixIndex.IndexOf(symbol);
    }

    private bool[] EvaluateCards(List<Card> cardsPlayer, List<Card> cardsEnemy) {
        int winValue = 0;
        bool skipEval = false;

        if(!cardsPlayer.Any() && !cardsEnemy.Any()) { //automatic draw because no card was played
            skipEval = true;
        } else if (!cardsPlayer.Any()) { //automatic win for enemy because user didn't play cards
            skipEval = true;
            winValue = -99;
        } else if (!cardsEnemy.Any()) { //automatic win for user because enemy didn't play cards
            skipEval = true;
            winValue = 99;
        }

        ExtractFunctions(cardsPlayer);
        ExtractFunctions(cardsEnemy);
        functionHandler.HandleBRFunctions();

        if (!skipEval) {
            CardSymbol symbolPlayer = ExtractNormalCard(cardsPlayer);
            CardSymbol symbolEnemy = ExtractNormalCard(cardsEnemy);
            winValue = winMatrix[SymbolToInt(symbolPlayer),SymbolToInt(symbolEnemy)];
        }

        functionHandler.HandleDCFunctions();
        if (winValue == 0) {
            bool[] result = {true, true};
            return result;
        } else if (winValue > 0) { // player won
            bool[] result = {true, false};
            functionHandler.HandleARFunctions(TableSideName.PLAYER);
            Damage(dmgOnLoss, TableSideName.ENEMY);
            return result;
        } else if (winValue < 0) {
            bool[] result = {false, true};
            functionHandler.HandleARFunctions(TableSideName.ENEMY);
            Damage(dmgOnLoss, TableSideName.PLAYER);
            return result;
        } else {
            bool[] result = {false, false};
            Debug.Log("Error: unknown winstate");
            return result;
        }
    }


    void EndGame(TableSideName winner) {
        table.SetWinner(winner);
    }

    public void Damage(int dmg, TableSideName damagee) {
        currentHealth[damagee] -= dmg;
    }

    public void DrawCards(int amount, TableSideName caller){
        table.DrawCards(amount, caller);
    }

    // Start of various setter stuff -------------------------------------------------------------------------------------------------

    public void SetDmgOnLoss(int dmgOnLoss) {
        this.dmgOnLoss = dmgOnLoss;
    }

    public void SetwinMatrix(int[,] winMatrix) {
        this.winMatrix = winMatrix;
    }

    // Start of various getter stuff -------------------------------------------------------------------------------------------------

    public int GetDmgOnLoss() {
        return this.dmgOnLoss;
    }

    public int[,] GetwinMatrix() {
        return this.winMatrix;
    }
}
