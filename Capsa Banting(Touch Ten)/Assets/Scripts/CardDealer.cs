using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum HandRank
{
    Single = 0,
    Pair = 20,
    Triple = 30,
    FourofKind = 40,
    Straight = 50,
    Flush = 60,
    FullHouse = 70,
    StraightFlush = 80,
    Dragon = 90,
    Undefined
}

public class CardDealer : MonoBehaviour
{
    public List<Card> Deck = new List<Card>();

    List<Card> buffer = new List<Card>();

    const int TotalPlayedCard = 52;

    public void GetDeck()
    {
        foreach(Suite suit in (Suite[]) Enum.GetValues(typeof(Suite)))
        {
            if (suit == Suite.Undefined)
                continue;
            foreach(CardValue val in (CardValue[])Enum.GetValues(typeof(CardValue)))
            {
                if (val == CardValue.JOKER)
                    continue;

                Card c = new Card();

                c.suite = suit;
                c.cardvalue = val;
                Deck.Add(c);
            }
        }

        for(int i = 0; i < UnityEngine.Random.Range(0,4); i++)
        {
            Card c = new Card();

            c.suite = Suite.Undefined;
            c.cardvalue = CardValue.JOKER;
            Deck.Add(c);
        }
    }

    public void Shuffle(List<Card> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int shuffled = UnityEngine.Random.Range(0, deck.Count);
            deck[i] = deck[shuffled];
            deck[shuffled] = temp;
        }
    }

    public bool GetFlush(List<Card> cards)
    {
        var query = cards.GroupBy(x => x.suite).Where(g => g.Count() == 5).Select(y => y.Key).ToList();

        if (query.Count != 0)
            return true;
        else
            return false;
    }

    public bool GetPair(List<Card> cards)
    {
        var query = cards.GroupBy(x => x.cardvalue).Where(g => g.Count() == 2).Select(y => y.Key).ToList();

        if (query.Count != 0)
            return true;
        else
            return false;
    }

    public List<Card> GetPairCard(List<Card> cards)
    {
        var query = cards.GroupBy(x => x.cardvalue).Where(g => g.Count() == 2).Select(y => y.Key).ToList();

        List<Card> c = new List<Card>();

        foreach(Card x in cards)
        {
            if (query.Contains(x.cardvalue))
                c.Add(x);
        }

        return c;
    }

    public bool GetTriple(List<Card> cards)
    {
        var query = cards.GroupBy(x => x.cardvalue).Where(g => g.Count() == 3).Select(y => y.Key).ToList();

        if (query.Count != 0)
            return true;
        else
            return false;
    }

    public List<Card> GetTripleCard(List<Card> cards)
    {
        var query = cards.GroupBy(x => x.cardvalue).Where(g => g.Count() == 3).Select(y => y.Key).ToList();

        List<Card> c = new List<Card>();

        foreach (Card x in cards)
        {
            if (query.Contains(x.cardvalue))
                c.Add(x);
        }

        return c;
    }

    public bool GetFourOfKind(List<Card> cards)
    {
        var query = cards.GroupBy(x => x.cardvalue).Where(g => g.Count() == 4).Select(y => y.Key).ToList();

        if (query.Count != 0)
        {
            if (cards.Count < 5)
                return false;
            else
                return true;
        }
        else
            return false;
    }

    public List<Card> GetFourCard(List<Card> cards)
    {
        var query = cards.GroupBy(x => x.cardvalue).Where(g => g.Count() == 4).Select(y => y.Key).ToList();

        List<Card> c = new List<Card>();

        foreach (Card x in cards)
        {
            if (query.Contains(x.cardvalue))
                c.Add(x);
        }

        return c;
    }

    public bool GetFullHouse(List<Card> cards)
    {
        if (GetPair(cards) && GetTriple(cards))
            return true;
        else
            return false;
    }

    public bool GetStraighFlush(List<Card> cards)
    {
        if (GetFlush(cards) && GetStraight(cards))
            return true;
        else
            return false;
    }

    public bool GetDragon(List<Card> cards)
    {
        var sort = cards.OrderByDescending(x => x.cardvalue).ToList();
        sort.Reverse();

        for (int i = 0; i < sort.Count; i++)
        {
            if (sort[i] == sort.Last())
            {
                if (Mathf.Abs(GetDelta(sort[i].cardvalue, sort[i - 1].cardvalue)) != 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (GetDelta(sort[i].cardvalue, sort[i + 1].cardvalue) != 1)
            {
                return false;
            }
        }

        return false;
    }

    public bool GetStraight(List<Card> cards)
    {
        var sort = cards.OrderByDescending(x => x.cardvalue).ToList();
        sort.Reverse();

        for(int i = 0; i < sort.Count; i++)
        {
            if (sort[i] == sort.Last())
            {
                if (Mathf.Abs(GetDelta(sort[i].cardvalue, sort[i - 1].cardvalue)) != 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (GetDelta(sort[i].cardvalue, sort[i + 1].cardvalue) != 1)
            {
                return false;
            }
        }

        return false;
    }

    int GetDelta(CardValue a, CardValue b)
    {
        int x = (int)b - (int)a;
        return x;
    }

    public void Update()
    {
        /*if(Input.GetKeyUp(KeyCode.D))
        {
            GetDeck();
        }
        if(Input.GetKeyUp(KeyCode.S))
        {
            Shuffle(Deck);
            List<Card> handcard = new List<Card>();

            for(int i = 0; i < 13; i++)
            {
                handcard.Add(Deck[i]);
            }

            var hc = handcard.OrderByDescending(x => x.cardvalue).ToList();
            hc.Reverse();
            handcard = hc;

            string HandCardContent = "";
            foreach(Card c in handcard)
            {
                string s = c.cardvalue.ToString() + c.suite.ToString() + " | ";
                HandCardContent += s;
            }

            Debug.Log(HandCardContent);

            List<CardGroup> possibleCardGroup = new List<CardGroup>();

            if(GetFlush(handcard) != null)
                possibleCardGroup.AddRange(GetFlush(handcard));

            if(GetPair(handcard) != null)
                possibleCardGroup.AddRange(GetPair(handcard));

            if(GetStraight(handcard) != null)
                possibleCardGroup.AddRange(GetStraight(handcard));

            string possibleGroup = "";
            foreach (CardGroup g in possibleCardGroup)
            {
                string s = "";
                if (g.HandRank  == HandRank.Straight)
                    s = g.HandRank.ToString() + " | ";
                else if (g.HandRank == HandRank.Flush)
                    s = g.HandRank.ToString() + g.GroupSuite.ToString() + " | ";
                else
                    s = g.HandRank.ToString() + g.cardvalue.ToString() + " | ";
                possibleGroup += s;
            }

            Debug.Log(possibleGroup);
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            Card c1 = new Card();
            c1.cardvalue = CardValue.THREE;
            c1.suite = Suite.Spade;
            Card c2 = new Card();
            c2.cardvalue = CardValue.FOUR;
            c2.suite = Suite.Spade;
            Card c3 = new Card();
            c3.cardvalue = CardValue.FIVE;
            c3.suite = Suite.Spade;
            Card c4 = new Card();
            c4.cardvalue = CardValue.SIX;
            c4.suite = Suite.Spade;
            Card c5 = new Card();
            c5.cardvalue = CardValue.SEVEN;
            c5.suite = Suite.Spade;
            Card c6 = new Card();
            c6.cardvalue = CardValue.NINE;
            c6.suite = Suite.Heart;
            Card c7 = new Card();
            c7.cardvalue = CardValue.NINE;
            c7.suite = Suite.Heart;
            Card c8 = new Card();
            c8.cardvalue = CardValue.TEN;
            c8.suite = Suite.Diamond;
            Card c9 = new Card();
            c9.cardvalue = CardValue.JACK;
            c9.suite = Suite.Diamond;
            Card c10 = new Card();
            c10.cardvalue = CardValue.QUEEN;
            c10.suite = Suite.Club;
            Card c11 = new Card();
            c11.cardvalue = CardValue.KING;
            c11.suite = Suite.Club;
            Card c12 = new Card();
            c12.cardvalue = CardValue.ACE;
            c12.suite = Suite.Club;
            Card c13 = new Card();
            c13.cardvalue = CardValue.TWO;
            c13.suite = Suite.Diamond;


            List<Card> c = new List<Card> { c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13 };

            var hc = c.OrderByDescending(x => x.OveralValue).ToList();
            hc.Reverse();
            c = hc;

            List<CardGroup> possibleCardGroup = new List<CardGroup>();

            if (GetFlush(c) != null)
                possibleCardGroup.AddRange(GetFlush(c));

            if (GetPair(c) != null)
                possibleCardGroup.AddRange(GetPair(c));

            if (GetTriple(c) != null)
                possibleCardGroup.AddRange(GetPair(c));

            if (GetFourOfKind(c) != null)
                possibleCardGroup.AddRange(GetPair(c));

            if (GetStraight(c) != null)
                possibleCardGroup.AddRange(GetStraight(c));

            string HandCardContent = "";
            foreach (Card ca in c)
            {
                string s = ca.cardvalue.ToString() + ca.suite.ToString() + " | ";
                HandCardContent += s;
            }

            Debug.Log(HandCardContent);

            string possibleGroup = "";
            foreach (CardGroup g in possibleCardGroup)
            {
                string s = "";
                if (g.HandRank == HandRank.Straight || g.HandRank == HandRank.StraightFlush)
                    s = g.HandRank.ToString() + " | ";
                else if (g.HandRank == HandRank.Flush)
                    s = g.HandRank.ToString() + g.GroupSuite.ToString() + " | ";
                else
                    s = g.HandRank.ToString() + g.cardvalue.ToString() + g.GroupSuite.ToString() + " | ";
                possibleGroup += s;
            }

            Debug.Log(possibleGroup);
        }*/
    }
}
