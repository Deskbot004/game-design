using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
 * A DeckManager is used to save a Deck into a Jsonfile.
 */

[Serializable]
public class DeckManager 
{

    public List<string> CardSymbols = new List<string>();
    public List<bool> types = new List<bool>();
    public List<string> functions = new List<string>();

}
