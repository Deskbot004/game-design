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

    public List<string> cardSymbols = new List<string>();
    public List<int> cardTypes = new List<int>();
    public List<int> slotTypes = new List<int>();
    public List<string> functions = new List<string>();

}
