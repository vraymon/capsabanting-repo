using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public enum Suite
{
    Spade = 40,
    Heart = 30,
    Club = 20,
    Diamond = 10,
    Undefined
}

public enum CardValue
{
    THREE = 1,
    FOUR = 2,
    FIVE = 3,
    SIX = 4,
    SEVEN = 5,
    EIGHT = 6,
    NINE = 7,
    TEN = 8,
    JACK = 9,
    QUEEN = 10,
    KING = 11,
    ACE = 12,
    TWO = 13,
    JOKER = 999
}

public class Card
{
    protected Suite Suite;
    protected CardValue Value;

    protected int TotalValue;
    protected bool PartOfGroup;

    public bool IsPartOfGroup
    {
        get
        {
            return PartOfGroup;
        }
    }

    public Suite suite
    {
        get
        {
            return Suite;
        }
        set
        {
            Suite = value;
        }
    }

    public CardValue cardvalue
    {
        get
        {
            return Value;
        }
        set
        {
            Value = value;
        }
    }

    public int OveralValue
    {
        get
        {
            return (int)Value + (int)Suite;
        }
    }

    public int totalCardValue
    {
        set
        {
            TotalValue = value;
        }
        get
        {
            return TotalValue;
        }
    }
}
