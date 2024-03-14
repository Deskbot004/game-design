using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SupportCardSprites : CardSprites
{
    [UDictionary.Split(60, 40)]
    public FunctionDict supportWindowSprites;

    [UDictionary.Split(60, 40)]
    public FunctionDict supportIconSprites;
}

