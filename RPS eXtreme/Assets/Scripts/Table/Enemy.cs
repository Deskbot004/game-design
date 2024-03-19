using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyPrefs {
    ROCK,
    PAPER,
    SCISSORS,
    LIZARD,
    SPOCK,
    SUPPORT,
    RESOURCING,
    RIGHT,
    RANDOM
}

enum AmountOf {
    ROCK,
    PAPER,
    SCISSORS,
    LIZARD,
    SPOCK,
    SLOTS,
    ALLCARDS,
    NORMALCARDS,
    SUPPORTCARDS
}
[Serializable] public class PrefDict : UDictionary<EnemyPrefs, float> { }

public class Enemy : TableSide
{
    [Header("Enemy Specific")]
    [UDictionary.Split(50, 50)] public PrefDict preferences;
    
    private List<EnemyPrefs> normalCardPrefs = new() {
        EnemyPrefs.ROCK, 
        EnemyPrefs.PAPER, 
        EnemyPrefs.SCISSORS, 
        EnemyPrefs.LIZARD, 
        EnemyPrefs.SPOCK, 
        EnemyPrefs.RANDOM
    };

    #region Setup -------------------------------------------------------------------------------------------
        public override void Init(Table table) {
        AddMissingPrefs();
        NormalizePrefs();
        SetMinValues();
        base.Init(table);
        isPlayer = false;
    }

    void AddMissingPrefs() {
        foreach(EnemyPrefs pref in Enum.GetValues(typeof(EnemyPrefs))) {
            if(!preferences.ContainsKey(pref)) {
                preferences.Add(pref, 0);
            }
        }
    } 

    void NormalizePrefs() {
        float prefSum = preferences.Where(p => normalCardPrefs.Contains(p.Key)).Select(p => p.Value).ToList().Sum();
        if(prefSum == 0) {
            preferences[EnemyPrefs.RANDOM] = 1;
            return;
        }
        foreach (EnemyPrefs pref in normalCardPrefs) {
            preferences[pref] /= prefSum;
        }
    }

    void SetMinValues(){
        foreach(EnemyPrefs pref in normalCardPrefs) {
            if (preferences[pref] < 0.1f) {
                preferences[pref] = 0.1f;
            }
        }
        this.NormalizePrefs();
    }
    #endregion

    #region Playing during the Game -------------------------------------------------------------------------
    // Not yet refactored, because the whole thing will change
    public void PlayCards() {
        List<Card> cardsInHand = GetCardsInHand();
        Dictionary<AmountOf, int> stats = AnalyzeHand(cardsInHand);
        List<Card> playedCards = new();

        var wantedSlots = new Queue<int>();
        int numOfSlotsToFill = stats[AmountOf.SLOTS];
        int numOfNormalCards = stats[AmountOf.NORMALCARDS];

        if (numOfNormalCards < numOfSlotsToFill) {
            numOfSlotsToFill = numOfNormalCards;
        }

        /*bool beResourceful = false;

        if(stats["numBasic"] <= 3)
        {
            beResourceful = true;
        }*/

        // TODO Rework Resourcing after Turn change functionality
        if(UnityEngine.Random.Range(0.0f,1.0f) <= this.preferences[EnemyPrefs.RIGHT]) {
            for (int i = 0; i <= numOfSlotsToFill - 1; i++) {
                /*
                float decisionPrefResource = Random.Range(0.0f, 1.0f);
                if (beResourceful && decisionPrefResource <= this.preferences["resourcing"])
                {
                    continue;
                }*/
                wantedSlots.Enqueue(i);
            }
        } else {
            for (int i = numOfSlotsToFill - 1; i >= 0; i--) {
                /*
                float decisionPrefResource = Random.Range(0.0f, 1.0f);
                if (beResourceful && decisionPrefResource <= this.preferences["resourcing"])
                {
                    continue;
                }*/
                wantedSlots.Enqueue(i);
            }
        }
        
        // Handpicking cards
        while(wantedSlots.Count > 0) {
            CardSymbol symbolToPlay = SelectCardToPlay(stats);
            List<Card> cards = hand.GetCardsBySymbol(symbolToPlay);

            foreach(Card card in cards) {
                if (wantedSlots.Count > 0){
                    float decisionSupp = UnityEngine.Random.Range(0.0f, 1.0f);
                    if (decisionSupp <= preferences[EnemyPrefs.SUPPORT]) {
                        foreach(Card support in cardsInHand) {
                            if (playedCards.Contains(support)){
                                continue;
                            }
                            if(!support.IsNormal()){
                                NormalCard normal = (NormalCard)card;
                                if(support.GetComponent<Draggable>().DropInto(normal)){
                                    // yield return new WaitForSeconds(speed); TODO
                                    playedCards.Add(support);
                                }
                            }
                        }
                    }
                    playNormalCard(card, wantedSlots.Dequeue(), playedCards);
                    // yield return new WaitForSeconds(speed); TODO
                }
            }
        }
        // yield return new WaitForSeconds(speed); TODO
        table.ui.endTurnButton.GetComponent<Button>().interactable = true;

        //yield return new WaitForSeconds(0.5f); TODO
        FlipCardAnim anim = AnimationHandler.CreateAnim<FlipCardAnim>();
        anim.Init(playedCards);
        AnimationHandler.QueueAnimation(anim, AnimationQueueName.ENEMY);
    }

    Dictionary<AmountOf, int> AnalyzeHand(List<Card> cardsInHand) {
        Dictionary<AmountOf, int> stats = new();
        foreach(AmountOf thing in Enum.GetValues(typeof(AmountOf))) {
            stats.Add(thing, 0);
        }
        stats[AmountOf.SLOTS] = this.slots.Count;
        stats[AmountOf.ALLCARDS] = cardsInHand.Count;

        foreach(Card card in cardsInHand){
            if(!card.IsNormal()){
                stats[AmountOf.SUPPORTCARDS] += 1;
            } else {
                stats[AmountOf.NORMALCARDS] += 1;
                switch (card.GetSymbol()) {
                    case CardSymbol.ROCK:
                        stats[AmountOf.ROCK] += 1;
                        break;
                    case CardSymbol.PAPER:
                        stats[AmountOf.PAPER] += 1;
                        break;
                    case CardSymbol.SCISSORS:
                        stats[AmountOf.SCISSORS] += 1;
                        break;
                    case CardSymbol.LIZARD:
                        stats[AmountOf.LIZARD] += 1;
                        break;
                    case CardSymbol.SPOCK:
                        stats[AmountOf.SPOCK] += 1;
                        break;
                    default:
                        Debug.Log("AnalyzeHand: Unexpected card symbol: " + card.GetSymbol());
                        break;
                }
            }
        }
        // TODO Rework expected Cards
        // The opponent ressource decisions depend on the expected card amount on next turn. (Always Draw, Support Draw etc...)
        return stats;
    }

    CardSymbol SelectCardToPlay(Dictionary<AmountOf, int> stats) {
        float prefSum = preferences.Where(p => normalCardPrefs.Contains(p.Key)).Select(p => p.Value).ToList().Sum();
        float randomCardFloat = UnityEngine.Random.Range(0.0f, prefSum);
        CardSymbol cardToPlay;
        AmountOf numOfCard;
        EnemyPrefs prefOfCard;
        float currentprefsum = 0.0f;
        float previousprefsum = 0.0f;
        for (int i = 0; i <= 5; i++) {
            cardToPlay = (CardSymbol) i; // TODO: Create classes die erben for that stuff
            numOfCard = (AmountOf) i;
            prefOfCard = (EnemyPrefs) i;
            previousprefsum = currentprefsum;

            currentprefsum += preferences[prefOfCard];
            if(randomCardFloat <= currentprefsum && randomCardFloat > previousprefsum && stats[numOfCard] > 0) {
                stats[numOfCard] -= 1;
                return cardToPlay;
            }
        }
        List<CardSymbol> symbols = Enum.GetValues(typeof(CardSymbol)).Cast<CardSymbol>().ToList();
        CardSymbol randomCard = symbols[UnityEngine.Random.Range(0, symbols.Count)];
        numOfCard = (AmountOf) randomCard;
        stats[numOfCard] -= 1;
        return randomCard;
    }


    public int playNormalCard(Card card, int slot, List<Card> playedCards)
    {
        if (this.slots[slot].GetNormalAndSuppCards().Count == 0) 
        {
            NormalCard norm = (NormalCard)card;
            norm.GetComponent<Draggable>().DropInto(slots[slot]);
            playedCards.Add(card);

            /*
            MoveCardAnim anim = animHandler.CreateAnim<MoveCardAnim>();
            anim.cards = new() {card};
            anim.destinationObject = slots[slot].transform;
            anim.draggableOnArrival = false;
            animHandler.QueueAnimation(anim);
             */
            return 0;
        }
        else
        {
            return 1;
        }
    }
    #endregion

    public override void DrawCards(int amount) {
        base.DrawCards(amount);
        // Wait for 1 second // TODO
        PlayCards();
    }

    public void SetPreferences(List<(EnemyPrefs, float)> preferences) {
        foreach((EnemyPrefs, float) prefEntry in preferences) {
            this.preferences[prefEntry.Item1] = prefEntry.Item2;
        }
        AddMissingPrefs();
        NormalizePrefs();
        SetMinValues();
    }
}
