using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSprites : MonoBehaviour
{
    [UDictionary.Split(30, 70)]
    public ColorDict backgroundColor;

    [UDictionary.Split(30, 70)]
    public ColorDict windowColor;

    // Dict Definition
    [Serializable] public class ColorDict : UDictionary<CardSymbol, Color> { }
    [Serializable] public class SpriteDict : UDictionary<CardSymbol, Sprite> { }    
    [Serializable] public class FunctionDict : UDictionary<FunctionID, Sprite> { }
}
