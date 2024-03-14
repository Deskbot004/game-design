using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using UnityEngine;

public class BRFunction : Function
{
    
}

public class AdditionalWin : BRFunction
{
    CardSymbol winsAgainst;
    int[,] oldWinMatrix;

    public override Function Copy() {
        return new AdditionalWin();
    }

    public override void Init(DictKeys caller, string stringParam) {
        this.caller = caller;
        // TODO: convert string
        Enum.TryParse(stringParam, out this.winsAgainst); // idk if this works
    }

    public override void DoEffect(Gamelogic logic) {
        oldWinMatrix = (int[,]) logic.GetwinMatrix().Clone();
        int[,] winMatrix = logic.GetwinMatrix();
        
        int change = 0;
        if(caller == DictKeys.PLAYER) {
            change = 3;
        } else {
            change = -3;
        }

        var entry = -1;
        switch (winsAgainst) {
            case CardSymbol.SCISSORS:
                entry = 0;
                break;
            case CardSymbol.ROCK:
                entry = 1;
                break;
            case CardSymbol.PAPER:
                entry = 2;
                break;
            case CardSymbol.SPOCK:
                entry = 3;
                break;
            case CardSymbol.LIZARD:
                entry = 4;
                break;
            default:
                Debug.Log("Error: No additional win mentioned " + entry);
                break;
        }
        for (int i = 0; i < winMatrix.GetLength(0); i++) {
            winMatrix[i, entry] += change;
        }
    }

    public override void ResetEffect(Gamelogic logic) {
        logic.SetwinMatrix(oldWinMatrix);
    }
}

public class WinOnDraw : BRFunction
{
    int[,] oldWinMatrix;

    public override Function Copy() {
        return new WinOnDraw();
    }

    public override void Init(DictKeys caller, string stringParam) {
        this.caller = caller;
    }

    public override void DoEffect(Gamelogic logic) {
        oldWinMatrix = (int[,]) logic.GetwinMatrix().Clone();
        int[,] winMatrix = logic.GetwinMatrix();
        
        int change = 0;
        if(caller == DictKeys.PLAYER) {
            change = 3;
        } else {
            change = -3;
        }
        
        for (int i = 0; i < winMatrix.GetLength(0); i++)
        {
            winMatrix[i, i] += change;
        }
    }

    public override void ResetEffect(Gamelogic logic) {
        logic.SetwinMatrix(oldWinMatrix);
    }
}

