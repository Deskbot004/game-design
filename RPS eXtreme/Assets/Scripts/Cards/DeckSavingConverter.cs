using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// Bridge between Objects and JSON file
public class DeckSavingConverter 
{
    public int cardAmount;
    public List<CardSymbol> cardSymbols = new();
    public List<(bool, bool)> slotPositions = new();
    public List<(FunctionID, List<string>)> functions = new();
    public List<(EnemyPrefs, float)> preferences = new();
    
    public void AddCardToSave(Card card) {
        cardAmount++;
        cardSymbols.Add(card.GetSymbol());
        slotPositions.Add((card.topSlot, card.bottomSlot));
        functions.Add(card.GetFunctionsForSaving());
    }

    public bool AreListsSameLength() {
        return cardSymbols.Count == cardAmount && slotPositions.Count == cardAmount && functions.Count == cardAmount;
    }
}
