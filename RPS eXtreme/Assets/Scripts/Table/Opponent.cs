using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Opponent : TablePlayer
{
    [Header("Opponent Specific")]
    public float speed;

    [System.Serializable]
    public class UDictionary1 : UDictionary<string, float> { }
    [UDictionary.Split(50, 50)]
    public UDictionary1 preferences;
    
    public override void Init(Table table, AnimationHandler animHandler) {
        CheckPreferencesAllSet();
        NormalizePrefs();
        base.Init(table, animHandler);
        isPlayer = false;
    }

    public void CheckPreferencesAllSet() {
        List<string> prefOptions = new() {"resourcing", "right", "rock", "scissors", "paper", "random", "support"};
        foreach (string prefOption in prefOptions) {
            Debug.Assert(preferences.ContainsKey(prefOption));
        }
    }

    private void NormalizePrefs() {
        float total = 0f;
        total += this.preferences["rock"];
        total += this.preferences["scissors"];
        total += this.preferences["paper"];
        total += this.preferences["random"];
        if(total == 0) {
            Debug.Log("can't have all 0 preferences");
            this.preferences["rock"] = 1f / 3f;
            this.preferences["scissors"] = 1f / 3f;
            this.preferences["paper"] = 1f / 3f;
        }
        this.preferences["rock"] /= total;
        this.preferences["scissors"] /= total;
        this.preferences["paper"] /= total;
        this.preferences["random"] /= total;
    }

    public void SetPreferences(List<float> preferences){ // TODO: Refactor after Deck
        if(preferences.Count != this.preferences.Keys.Count){
            Debug.Log("Number of preferences of deck doesn't match number of preferences of opponent!");
            return;
        }
        this.preferences["resourcing"] = preferences[0];
        this.preferences["right"] = preferences[1];
        this.preferences["rock"] = preferences[2];
        this.preferences["scissors"] = preferences[3];
        this.preferences["paper"] = preferences[4];
        this.preferences["random"] = preferences[5];
        this.preferences["support"] = preferences[6];
        this.NormalizePrefs();
        //Make sure no Card has a probability of much less than 10% // TODO: Should this also be checked in the init? Maybe in CheckPreferencesAllSet()?
        if(this.preferences["rock"] < 0.1)
        {
            this.preferences["rock"] = 0.1f;
        }
        if (this.preferences["scissors"] < 0.1)
        {
            this.preferences["scissors"] = 0.1f;
        }
        if (this.preferences["paper"] < 0.1)
        {
            this.preferences["paper"] = 0.1f;
        }
        this.NormalizePrefs();
    }

    private Dictionary<string,int> AnalyzeHand(List<Card> cards) {  
        Dictionary<string,int> stats = new Dictionary<string, int>();
        stats["numSlots"] = this.slots.Count;
        stats["numCards"] = cards.Count;
        
        int basic = 0;
        int support = 0;
        int rock = 0;
        int paper = 0;
        int scissor = 0;
        int lizard = 0;
        int spock = 0;
        foreach(Card card in cards){
            if(!card.IsBasic()){
                support += 1;
            } else {
                switch (card.symbol) {
                    // "scissors", "rock", "paper", "lizard", "spock"
                    case "rock":
                        rock+=1;
                        break;
                    case "paper":
                        paper+=1;
                        break;
                    case "scissors":
                        scissor+=1;
                        break;
                    case "lizard":
                        lizard+=1;
                        break;
                    case "spock":
                        spock+=1;
                        break;
                    default:
                        Debug.Log("AnalyzeHand: Unexpected card symbol: " + card.symbol);break;
                }

                basic += 1;
            }
        }
        stats["numRock"] = rock;
        stats["numPaper"] = paper;
        stats["numScissors"] = scissor;
        stats["numLizard"] = lizard;
        stats["numSpock"] = spock;
        stats["numBasic"] = basic;
        stats["numSupport"] = support;
        // TODO Rework expected Cards
        //stats["numExpectedCards"] = stats["numCards"] + this.table.logic.turnDraw;
        return stats;
    }

    public void PlayCards() {
        List<Card> cardsInHand = GetCardsInHand();
        Dictionary<string,int> stats = AnalyzeHand(cardsInHand);
        List<Card> playedCards = new();

        var wantedSlots = new Queue<int>();
        var slotfill = stats["numSlots"];
        /*bool beResourceful = false;

        if(stats["numBasic"] <= 3)
        {
            beResourceful = true;
        }*/


        if (stats["numBasic"] < stats["numSlots"]) {
            slotfill = stats["numBasic"];
        }

        // TODO Rework Resourcing after Turn change functionality
        float decisionPrefRight = Random.Range(0.0f,1.0f);
        if(decisionPrefRight <= this.preferences["right"])
        {
            for (int i = 0; i <= slotfill - 1; i++)
            {
                /*
                float decisionPrefResource = Random.Range(0.0f, 1.0f);
                if (beResourceful && decisionPrefResource <= this.preferences["resourcing"])
                {
                    continue;
                }*/
                wantedSlots.Enqueue(i);
            }
        }
        else
        {
            for (int i = slotfill - 1; i >= 0; i--)
            {
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
            float decisionCardToPlay = Random.Range(0.0f, preferences["rock"]+preferences["scissors"]+preferences["paper"]+preferences["random"]);
            string cardToPlay = "random";
            if(decisionCardToPlay <= preferences["rock"] && stats["numRock"] > 0){
                stats["numRock"] -= 1;
                cardToPlay = "rock";
            } else if(decisionCardToPlay <= preferences["rock"] + preferences["scissors"] && stats["numScissors"] > 0 && decisionCardToPlay > preferences["rock"]){
                stats["numScissors"] -= 1;
                cardToPlay = "scissors";
            } else if(decisionCardToPlay <= preferences["rock"] + preferences["scissors"] + preferences["paper"] && stats["numPaper"] > 0 && decisionCardToPlay > preferences["rock"] + preferences["scissors"])
            {
                stats["numPaper"] -= 1;
                cardToPlay = "paper";
            }

            foreach(Card card in cardsInHand) {
                if (card.symbol == cardToPlay && wantedSlots.Count > 0){
                    float decisionSupp = Random.Range(0.0f, 1.0f);
                    if (decisionSupp <= preferences["support"]) {
                        foreach(Card support in cardsInHand) {
                            if (playedCards.Contains(support)){
                                continue;
                            }
                            if(!support.IsBasic()){
                                NormalCard normal = (NormalCard)card;
                                if(support.GetComponent<Draggable>().DropInto(normal)){
                                    //normal.OnDrop(support.GetComponent<Draggable>())
                                    // yield return new WaitForSeconds(speed); TODO
                                    //support.transform.localPosition = new Vector3(0,0,0.5f);
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
        foreach(Card card in playedCards)
        {
            //RemoveFromHandWithAnimation(card);
        }
        
        table.ui.endTurnButton.GetComponent<Button>().interactable = true;

        //yield return new WaitForSeconds(0.5f); TODO
        FlipCardAnim anim = animHandler.CreateAnim<FlipCardAnim>();
        anim.flippedCards = playedCards;
        animHandler.QueueAnimation(anim, (int) AnimationOffQueues.OPPONENT);
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

    public override void DrawCards(int amount) {
        base.DrawCards(amount);
        // Wait for 1 second // TODO
        PlayCards();
    }
}
