using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSprites : MonoBehaviour
{
    [UDictionary.Split(30, 70)]
    public UDictionary1 symbolSprites;

    [UDictionary.Split(30, 70)]
    public UDictionary1 supportWindowSprites;

    [UDictionary.Split(30, 70)]
    public UDictionary1 supportIconSprites;



    [Serializable]
    public class UDictionary1 : UDictionary<string, Sprite> { }

    //public Dictionary<string, Sprite> symbolSprites;
}
