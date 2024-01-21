using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSprites : MonoBehaviour
{
    [UDictionary.Split(30, 70)]
    public UDictionary1 symbolSprites;

    [UDictionary.Split(50, 50)]
    public UDictionary1 supportWindowSprites;

    [UDictionary.Split(30, 70)]
    public UDictionary1 supportIconSprites;

    [UDictionary.Split(30, 70)]
    public UDictionary2 colors;



    [Serializable]
    public class UDictionary1 : UDictionary<string, Sprite> { }

    [Serializable]
    public class UDictionary2 : UDictionary<string, Color> { }
}
