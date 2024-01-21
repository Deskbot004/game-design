using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : TablePlayer
{
    public Dictionary<string,float> preferences;
    [Header("[resourcing,right,rock,paper,scissors,random,support]")]
    public List<float> preferenceList;

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
        this.normalizeDict();
        this.SetPreferenceList();
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
        this.normalizeDict();
        //Make sure no Card has a probability of much less than 10%
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
        this.normalizeDict();
        this.SetPreferenceList();
    }
    //Only for looking at the Dictionary values in the editor
    public void SetPreferenceList()
    {
        this.preferenceList.Clear();
        foreach(KeyValuePair<string, float> pref in this.preferences)
        {
            this.preferenceList.Add(pref.Value);
        }
    }

    private void normalizeDict()
    {
        float total = 0f;
        total += this.preferences["rock"];
        total += this.preferences["scissors"];
        total += this.preferences["paper"];
        total += this.preferences["random"];
        if(total == 0)
        {
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
        stats["numExpectedCards"] = stats["numCards"] + this.table.logic.turnDraw;
        return stats;
    }

    public override IEnumerator playCards() {
        List<Card> cards = this.hand.GetCards();
        Dictionary<string,int> stats = AnalyzeHand(cards);
        List<Card> playedCards = new List<Card>();

        var wantedSlots = new Queue<int>();
        var slotfill = stats["numSlots"];
        bool beResourceful = false;

        if(stats["numBasic"] <= 3)
        {
            beResourceful = true;
        }


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

        yield return new WaitForSeconds(0.5f);
        float animationLength = table.TurnEnemySlotCards();
        if(table.quickResolve) animationLength = 0;
        yield return new WaitForSecondsRealtime(animationLength + table.waitTimer);
    }


    public int playNormalCard(Card card, int slot, List<Card> playedCards)
    {
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
