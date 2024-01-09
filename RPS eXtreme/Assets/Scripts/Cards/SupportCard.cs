using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SupportCard : Card
{
    public bool isAttached;
    public List<Action> functions = new List<Action>();

    public List<Action> GetFunctions() {
        return functions;
    }

}
