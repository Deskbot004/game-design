using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FunctionHandler
{
    private Gamelogic logic;
    private List<Function> allFunctions = new();
    private Stack<Function> doneFunctions = new();

    public FunctionHandler(Gamelogic logic) {
        this.logic = logic;
    }

    public void AddFunctions(List<Function> function) {
        allFunctions.AddRange(function);
    }

    public void HandleBRFunctions() {
        List<Function> brFunctions = allFunctions.Where(f => f is BRFunction).ToList();
        HandleFunctions(brFunctions);
    }

    public void HandleARFunctions(DictKeys caller) {
        List<Function> arFunctions = allFunctions.Where(f => f is ARFunction && f.caller == caller).ToList();
        HandleFunctions(arFunctions);
    }

    public void HandleDCFunctions() {
        List<Function> dcFunctions = allFunctions.Where(f => f is DCFunction).ToList();
        HandleFunctions(dcFunctions);
    }

    void HandleFunctions(List<Function> functionList) {
        foreach(Function func in functionList) {
            func.DoEffect(logic);
            doneFunctions.Push(func);
        }
    }

    public void ResetEverything() {
        while(doneFunctions.Count > 0) {
            Function func = doneFunctions.Pop();
            func.ResetEffect(logic);
        }
        allFunctions.Clear();
        doneFunctions.Clear();
    }
}
