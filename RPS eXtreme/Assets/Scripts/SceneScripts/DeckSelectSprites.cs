using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelectSprites : MonoBehaviour
{
    [UDictionary.Split(40, 60)]
    public SpriteDict windowSprites;

    [UDictionary.Split(40, 60)]
    public ColorDict backgroundColors;


    // Dicts definitions
    [Serializable]
    public class SpriteDict : UDictionary<string, Sprite> { }

    [Serializable]
    public class ColorDict : UDictionary<string, Color> { }
}
