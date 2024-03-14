using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public enum FunctionID {
    DRAW,
    ADDDMG,
    LIFESTEAL,
    WINDRAW,
    

    // Variations
    WINAGAINST_ROCK,
    WINAGAINST_PAPER,
    WINAGAINST_SCISSORS,
    WINAGAINST_LIZARD,
    WINAGAINST_SPOCK,

    // Combinations (Example for now)
    DMGANDHEAL
}

// FunctionID -> List<FunctionTypes>
// FunctionType -> Function

// FunctionID.DRAW -> {FunctionType.DRAW}
// FunctionID.DMG&HEAL -> {FunctionType.DMG, FunctionType.HEAL}

public static class SupportLibrary
{
    public static Dictionary<FunctionType, Function> functionLibrary = new() {
        [FunctionType.DRAW] = new DrawCards(),
        [FunctionType.ADDDMG] = new AdditionalDamage(),
        [FunctionType.LIFESTEAL] = new Lifesteal(),
        [FunctionType.WINDRAW] = new WinOnDraw(),
        [FunctionType.WINAGAINST] = new AdditionalWin(),
    };
    //Rock Paper Scissors Lizard Spock
    public static Dictionary<FunctionID, CardInfo> supportLibrary = new() {
        [FunctionID.DRAW] = new CardInfo(new(){FunctionType.DRAW},"Draw", "Draw /x/ on win or loss"),
        [FunctionID.ADDDMG] = new CardInfo(new(){FunctionType.ADDDMG}, "Additional damage", "Do /x/ extra damage on win"),
        [FunctionID.LIFESTEAL] = new CardInfo(new(){FunctionType.LIFESTEAL}, "Lifesteal", "Lifesteal /x/% of dmg dealt"),
        [FunctionID.WINDRAW] = new CardInfo(new(){FunctionType.WINDRAW}, "Win a draw", "Win on draw"),
        [FunctionID.WINAGAINST_ROCK] = new CardInfo(new(){FunctionType.WINAGAINST}, "Additional win", "Win against rock"),
        [FunctionID.WINAGAINST_PAPER] = new CardInfo(new(){FunctionType.WINAGAINST}, "Additional win", "Win against paper"),
        [FunctionID.WINAGAINST_SCISSORS] = new CardInfo(new(){FunctionType.WINAGAINST}, "Additional win", "Win against scissors"),
        [FunctionID.WINAGAINST_LIZARD] = new CardInfo(new(){FunctionType.WINAGAINST}, "Additional win", "Win against lizard"),
        [FunctionID.WINAGAINST_SPOCK] = new CardInfo(new(){FunctionType.WINAGAINST}, "Additional win", "Win against spock"),
        //[FunctionID.DMGANDHEAL] = new CardInfo(new(){FunctionID.ADDDMG, FunctionID.LIFESTEAL},"Deal /x/ extra damage and heal for it"),
    };
    

    public static Function CreateFunction(FunctionType functionType, bool isPlayer, string stringParam) {
        Function oldFunction = functionLibrary[functionType];
        Function newFunction = oldFunction.Copy();
        if (isPlayer) {
            newFunction.Init(DictKeys.PLAYER, stringParam);
        } else {
            newFunction.Init(DictKeys.ENEMY, stringParam);
        }
        return newFunction;
    }

}