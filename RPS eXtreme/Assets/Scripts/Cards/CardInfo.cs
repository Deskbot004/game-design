using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;

public class CardInfo
{
    // TODO: Change after GameLogic
    public List<FunctionType> functions;
    public string title;
    public string description;

    public CardInfo() {}

    public CardInfo(List<FunctionType> functions, string title, string description) {
        this.functions = functions;
        this.title = title;
        this.description = description;
    }

    public string CreateDescription(List<string> stringParam) {
        string modifiedDescrition = description;
        var regex = new Regex(Regex.Escape("/x/"));
        foreach (string value in stringParam) {
            modifiedDescrition = regex.Replace(modifiedDescrition, value, 1);
        }
        return modifiedDescrition;
    }
}