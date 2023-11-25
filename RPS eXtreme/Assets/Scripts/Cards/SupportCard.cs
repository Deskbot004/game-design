using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SupportCard : Card
{
    public bool isAttached;
    public List<Action> functions;

    public List<Action> GetFunctions() {
        return functions;
    }
}
