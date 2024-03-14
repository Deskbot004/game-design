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
    private IDictionary<DictKeys, int> currentHealth = new Dictionary<DictKeys, int>();
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
        currentHealth.Add(DictKeys.PLAYER, maxHealth);
        currentHealth.Add(DictKeys.ENEMY, maxHealth);
        table.ui.SetHealth(maxHealth, DictKeys.PLAYER);
        table.ui.SetHealth(maxHealth, DictKeys.ENEMY);
        this.players = table.GetComponentsInChildren<TableSide>();

        DrawCards(startDraw + turnDraw, DictKeys.PLAYER);
        DrawCards(startDraw + turnDraw, DictKeys.ENEMY);
    }
    
    void StartTurn() {
        foreach (TableSide p in players) {
            DictKeys cardDrawer = p.isPlayer? DictKeys.PLAYER : DictKeys.ENEMY;
            if(p.GetCardsInHand().Count == 0)
                table.DrawCards(startDraw, cardDrawer);
            else
                table.DrawCards(turnDraw, cardDrawer);
        }
    }

    public void ResolveTurn() {
        foreach ((Slot slotPlayer, Slot slotEnemy) in table.GetMatchingSlots()) {
            int[] prevHealth = {currentHealth[DictKeys.PLAYER], currentHealth[DictKeys.ENEMY]};
            bool[] winner = EvaluateCards(slotPlayer.GetNormalAndSuppCards(), slotEnemy.GetNormalAndSuppCards());
            SlotResult playerResult = new(slotPlayer, winner[0], prevHealth[0], currentHealth[DictKeys.PLAYER]);
            SlotResult enemyResult = new(slotEnemy, winner[1], prevHealth[1], currentHealth[DictKeys.ENEMY]);
            table.PlaySlotResolveAnim(playerResult, enemyResult, slotPlayer.slotPosition == table.player.slots.Count - 1);
            functionHandler.ResetEverything();
        }
        table.ClearSlots();

        if (currentHealth[DictKeys.PLAYER] <= 0) {
            EndGame(DictKeys.ENEMY);
        } else if (currentHealth[DictKeys.ENEMY] <= 0) {
            EndGame(DictKeys.PLAYER);
        } else {
            StartTurn();
        }
    }

    public void ExtractFunctions(List<Card> cards) {
        foreach (Card card in cards) {
            if (!card.IsNormal()) {
                functionHandler.AddFunctions(card.GetFunctionsForResolve());
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

        if (winValue == 0) {
            bool[] result = {true, true};
            return result;
        } else if (winValue > 0) { // player won
            bool[] result = {true, false};
            functionHandler.HandleARFunctions(DictKeys.PLAYER);
            Damage(dmgOnLoss, DictKeys.ENEMY);
            return result;
        } else if (winValue < 0) {
            bool[] result = {false, true};
            functionHandler.HandleARFunctions(DictKeys.ENEMY);
            Damage(dmgOnLoss, DictKeys.PLAYER);
            return result;
        } else {
            bool[] result = {false, false};
            Debug.Log("Error: unknown winstate");
            return result;
        }
    }


    void EndGame(DictKeys winner) {
        table.SetWinner(winner);
    }

    public void Damage(int dmg, DictKeys damagee) {
        currentHealth[damagee] -= dmg;
    }

    public void DrawCards(int amount, DictKeys caller){
        table.DrawCards(amount, caller);
    }

    // Start of various setter stuff -------------------------------------------------------------------------------------------------

    public void SetdmgOnLoss(int dmgOnLoss) {
        this.dmgOnLoss = dmgOnLoss;
    }

    public void SetwinMatrix(int[,] winMatrix) {
        this.winMatrix = winMatrix;
    }

    // Start of various getter stuff -------------------------------------------------------------------------------------------------

    public int GetdmgOnLoss() {
        return this.dmgOnLoss;
    }

    public int[,] GetwinMatrix() {
        return this.winMatrix;
    }
}
