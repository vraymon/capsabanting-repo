using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGroup : Card
{
    public List<Card> GroupContent;

    public HandRank HandRank;

    public CardValue HighCard;

    public Suite GroupSuite
    {
        get
        {
            return Suite;
        }
    }

    public CardValue GroupValue
    {
        get
        {
            return Value;
        }
    }

    public CardGroup(List<Card> content, HandRank rank, Suite suite = default(Suite), CardValue value = default(CardValue), CardValue highcard = default(CardValue))
    {
        GroupContent = content;
        HandRank = rank;
        Suite = suite;
        Value = value;
        TotalValue = (int)rank + (int)highcard + OveralValue;
    }
}
