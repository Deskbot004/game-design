using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : TablePlayer
{
    Dictionary<string,float> preferences;
    public float speed;
    public override void init(Table table)
    {
        this.preferences = new Dictionary<string, float>();
        this.preferences["resourcing"] = 0;
        this.preferences["right"] = 1;
        this.preferences["rock"] = 1;
        this.preferences["scissors"] = 1;
        this.preferences["paper"] = 1;
        this.preferences["random"] = 0;
        this.preferences["support"] = 1;
        base.init(table);
        this.isPlayer = false;
    }

    public void SetPreferences(List<float> preferences){
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

                        break;
                    case "spock":

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
        stats["numBasic"] = basic;
        stats["numSupport"] = support;
        stats["numExpectedCards"] = stats["numCards"] + this.table.logic.turnDraw;
        //Debug.Log("I have "+rock+" Rocks "+scissor+" Scissors "+paper+" Papers =>"+basic+" Basics "+support+" Support "+stats["numExpectedCards"]+" Cards next turn!");
        return stats;
    }

    public override IEnumerator playCards() {
        List<Card> cards = this.hand.GetCards();
        Dictionary<string,int> stats = AnalyzeHand(cards);
        List<Card> playedCards = new List<Card>();

        var wantedSlots = new Queue<int>();
        var slotfill = stats["numSlots"];
        if (stats["numBasic"] < stats["numSlots"]) {
            slotfill = slotfill - stats["numSlots"] + stats["numBasic"];
        }

        // -> pref right und pref resourcing
        //-> Anpassen wie viel wollen wir spielen
        for (int i = 0;i<=slotfill-1;i++) {
            wantedSlots.Enqueue(i);
        }

        // Handpicking cards
        while(wantedSlots.Count > 0) {
            float decision = Random.Range(0.0f, preferences["rock"]+preferences["scissors"]+preferences["paper"]+preferences["random"]);
            string cardToPlay = "random";
            if(decision <= preferences["rock"] && stats["numRock"] > 0){
                stats["numRock"] -= 1;
                cardToPlay = "rock";
            } else if(decision <= preferences["rock"] + preferences["scissors"] && stats["numScissors"] > 0){
                stats["numScissors"] -= 1;
                cardToPlay = "scissors";
            } else if(decision <= preferences["rock"] + preferences["scissors"] + preferences["paper"] && stats["numPaper"] > 0){
                stats["numPaper"] -= 1;
                cardToPlay = "paper";
            } 


            foreach(Card card in cards) {
                if (card.symbol == cardToPlay && wantedSlots.Count > 0){
                    float decisionSupp = Random.Range(0.0f, 1.0f);
                    if (decisionSupp <= preferences["support"]) {
                        foreach(Card support in cards) {
                            if (playedCards.Contains(support)){
                                continue;
                            }
                            if(!support.IsBasic()){
                                NormalCard normal = (NormalCard)card;
                                if(normal.OnDrop(support.GetComponent<Draggable>())){
                                    yield return new WaitForSeconds(speed);
                                    support.transform.localPosition = new Vector3(0,0,0.5f);
                                    playedCards.Add(support);
                                }
                            }
                        }
                    }
                    playNormalCard(card, wantedSlots.Dequeue(), playedCards);
                    yield return new WaitForSeconds(speed);
                }
            }
        }
        yield return new WaitForSeconds(speed);
        foreach(Card card in playedCards)
        {
            this.hand.RemoveCard(card);
        }
        this.hand.ArrangeHand();
        yield return null;

    }


    public int playNormalCard(Card card, int slot, List<Card> playedCards)
    {
        //Debug.Log("playing Normal Card with symbol " + card.GetSymbol());
        if (this.slots[slot].GetNormalAndSuppCards().Count == 0) 
        {
            NormalCard norm = (NormalCard)card;
            this.slots[slot].SetCard(norm);
            playedCards.Add(card);
            card.SetWorldTargetPosition(slots[slot].transform.TransformPoint(Vector3.zero));
            card.SetTargetRotation(Vector3.zero);
            StartCoroutine(card.MoveToTarget(1));
            return 0;
        }
        else
        {
            return 1;
        }
    }

    protected override IEnumerator DealCards(List<Card> cards, float timeOffset)
    {
        yield return base.DealCards(cards, timeOffset);
        yield return new WaitForSeconds(1);
        StartCoroutine(playCards());
        yield return null;
    }
}
