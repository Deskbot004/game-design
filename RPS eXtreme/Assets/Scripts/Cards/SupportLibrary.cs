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
    /*
    public static Dictionary<FunctionType, Function> functionLibrary = new() {
        [FunctionType.DRAW] = new DrawCards(),
        [FunctionType.ADDDMG] = new AdditionalDamage(),
        [FunctionType.LIFESTEAL] = new Lifesteal(),
        [FunctionType.WINDRAW] = new WinOnDraw(),
        [FunctionType.WINAGAINST] = new AdditionalWin(),
    };
     */

    //Rock Paper Scissors Lizard Spock
    /*
    public static Dictionary<FunctionID, FunctionInfo> supportLibrary = new() {
        [FunctionID.DRAW] = new FunctionInfo(new(){FunctionType.DRAW},"Draw", "Draw /x/ on win or loss"),
        [FunctionID.ADDDMG] = new FunctionInfo(new(){FunctionType.ADDDMG}, "Additional damage", "Do /x/ extra damage on win"),
        [FunctionID.LIFESTEAL] = new FunctionInfo(new(){FunctionType.LIFESTEAL}, "Lifesteal", "Lifesteal /x/% of dmg dealt"),
        [FunctionID.WINDRAW] = new FunctionInfo(new(){FunctionType.WINDRAW}, "Win a draw", "Win on draw"),
        [FunctionID.WINAGAINST_ROCK] = new FunctionInfo(new(){FunctionType.WINAGAINST}, "Additional win", "Win against rock"),
        [FunctionID.WINAGAINST_PAPER] = new FunctionInfo(new(){FunctionType.WINAGAINST}, "Additional win", "Win against paper"),
        [FunctionID.WINAGAINST_SCISSORS] = new FunctionInfo(new(){FunctionType.WINAGAINST}, "Additional win", "Win against scissors"),
        [FunctionID.WINAGAINST_LIZARD] = new FunctionInfo(new(){FunctionType.WINAGAINST}, "Additional win", "Win against lizard"),
        [FunctionID.WINAGAINST_SPOCK] = new FunctionInfo(new(){FunctionType.WINAGAINST}, "Additional win", "Win against spock"),
        //[FunctionID.DMGANDHEAL] = new CardInfo(new(){FunctionID.ADDDMG, FunctionID.LIFESTEAL},"Deal /x/ extra damage and heal for it"),
    };
     */

    public static readonly Dictionary<FunctionID, FunctionInfo> supportLibrary = new() {
        [FunctionID.DRAW] = new FunctionInfo( FunctionID.DRAW,
            new(){new DrawCards()},
            "Draw Cards",
            "Draw /x/ on win or loss"),
        [FunctionID.ADDDMG] = new FunctionInfo( FunctionID.ADDDMG,
            new(){new AdditionalDamage()},
            "Strength",
            "Do /x/ extra damage on win"),
        [FunctionID.LIFESTEAL] = new FunctionInfo( FunctionID.LIFESTEAL,
            new(){new Lifesteal()},
            "Lifesteal",
            "Heal /x/% of damage dealt"),
        [FunctionID.WINDRAW] = new FunctionInfo( FunctionID.WINDRAW,
            new(){new WinOnDraw()},
            "Win on draw",
            "Win on draw"),
        [FunctionID.WINAGAINST_ROCK] = new FunctionInfo( FunctionID.WINAGAINST_ROCK,
            new(){new AdditionalWin(CardSymbol.ROCK)},
            "Additional win",
            "Win against rock"),
        [FunctionID.WINAGAINST_PAPER] = new FunctionInfo( FunctionID.WINAGAINST_PAPER,
            new(){new AdditionalWin(CardSymbol.PAPER)},
            "Additional win",
            "Win against paper"),
        [FunctionID.WINAGAINST_SCISSORS] = new FunctionInfo( FunctionID.WINAGAINST_SCISSORS,
            new(){new AdditionalWin(CardSymbol.SCISSORS)},
            "Additional win",
            "Win against scissors"),
        [FunctionID.WINAGAINST_LIZARD] = new FunctionInfo( FunctionID.WINAGAINST_LIZARD,
            new(){new AdditionalWin(CardSymbol.LIZARD)},
            "Additional win",
            "Win against lizard"),
        [FunctionID.WINAGAINST_SPOCK] = new FunctionInfo( FunctionID.WINAGAINST_SPOCK,
            new(){new AdditionalWin(CardSymbol.SPOCK)},
            "Additional win",
            "Win against spock"),
        [FunctionID.DMGANDHEAL] = new FunctionInfo( FunctionID.DMGANDHEAL,
            new(){new AdditionalDamage(), new Lifesteal()},
            "Empowered Healing",
            "Do /x/ extra damage and heal /x/% of damage dealt"
        )
    };
    

    /*
    public static Function CreateFunction(FunctionType functionType, bool isPlayer, string stringParam) {
        Function oldFunction = functionLibrary[functionType];
        Function newFunction = oldFunction.Copy();
        if (isPlayer) {
            newFunction.Init(TableSideName.PLAYER, stringParam);
        } else {
            newFunction.Init(TableSideName.ENEMY, stringParam);
        }
        return newFunction;
    }
     */

}