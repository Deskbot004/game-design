using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

// Stores all relevant info about the functions from a functionID
public class FunctionInfo
{
    //public List<FunctionType> functions; // Old Version (//TODO: Remove)
    // Allgemeine Infos
    public FunctionID functionID;
    public List<Function> functions;
    public string title;
    private string descriptionTemplate;

    // Specifische Infos
    public List<string> functionParams;
    public string description;

    public FunctionInfo(FunctionID functionID, List<Function> functions, string title, string descriptionTemplate) {
        this.functionID = functionID;
        this.functions = new(functions); // TODO: Does this also create new Function Objects?
        this.title = title;
        this.descriptionTemplate = descriptionTemplate;
    }

    public FunctionInfo CreateInitialized(bool belongsToPlayer, List<string> functionParams) {
        FunctionInfo newFunctionInfo = new(functionID, functions, title, descriptionTemplate);
        newFunctionInfo.Init(belongsToPlayer, functionParams);
        return newFunctionInfo;
    }

    void Init(bool belongsToPlayer, List<string> functionParams) {
        this.functionParams = functionParams;
        InitFunctions(belongsToPlayer, functionParams);
        SetDescription(functionParams);
    }

    void InitFunctions(bool belongsToPlayer, List<string> functionParams) {
        Debug.Assert(functions.Count == functionParams.Count, "Error initializing Functions: amount of entries in functionParams List doesn't match amounts of functions");

        TableSideName tableSide = belongsToPlayer? TableSideName.PLAYER : TableSideName.ENEMY;
        for(int i=0; i<functions.Count; i++) {
            functions[i].Init(tableSide, functionParams[i]);
        }
    }

    void SetDescription(List<string> functionParams) {
        description = descriptionTemplate;
        var regex = new Regex(Regex.Escape("/x/"));
        foreach (string value in functionParams) {
            description = regex.Replace(description, value, 1);
        }
    }
}