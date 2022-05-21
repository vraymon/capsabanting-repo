using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public enum CardState
{
    IDLE,
    SELECTED
}

public class PlayerCard : MonoBehaviour
{
    public Card card;
    public SpriteAtlas Atlas;
    public SpriteRenderer Renderer;

    const string CardBack = "card_list_54";
    const string JokerCard = "card_list_52";

    public CardState cardstate = CardState.IDLE;

    Suite suite;
    CardValue value;

    public bool HideCard;
    public void SetSprite()
    {
        suite = card.suite;
        value = card.cardvalue;

        if (!HideCard)
            Renderer.sprite = Atlas.GetSprite(GetSpriteName(suite, value));
        else
            Renderer.sprite = Atlas.GetSprite(CardBack);
    }
    string GetSpriteName(Suite type, CardValue val)
    {
        if (type == Suite.Spade)
        {
            switch (val)
            {
                case CardValue.TWO: return "card_list_1";
                case CardValue.THREE: return "card_list_2";
                case CardValue.FOUR: return "card_list_3";
                case CardValue.FIVE: return "card_list_4";
                case CardValue.SIX: return "card_list_5";
                case CardValue.SEVEN: return "card_list_6";
                case CardValue.EIGHT: return "card_list_7";
                case CardValue.NINE: return "card_list_8";
                case CardValue.TEN: return "card_list_9";
                case CardValue.JACK: return "card_list_10";
                case CardValue.QUEEN: return "card_list_11";
                case CardValue.KING: return "card_list_12";
                case CardValue.ACE: return "card_list_0";
            }
        }
        else if (type == Suite.Diamond)
        {
            switch (val)
            {
                case CardValue.TWO: return "card_list_14";
                case CardValue.THREE: return "card_list_15";
                case CardValue.FOUR: return "card_list_16";
                case CardValue.FIVE: return "card_list_17";
                case CardValue.SIX: return "card_list_18";
                case CardValue.SEVEN: return "card_list_19";
                case CardValue.EIGHT: return "card_list_20";
                case CardValue.NINE: return "card_list_21";
                case CardValue.TEN: return "card_list_22";
                case CardValue.JACK: return "card_list_23";
                case CardValue.QUEEN: return "card_list_24";
                case CardValue.KING: return "card_list_25";
                case CardValue.ACE: return "card_list_13";
            }
        }
        else if (type == Suite.Heart)
        {
            switch (val)
            {
                case CardValue.TWO: return "card_list_27";
                case CardValue.THREE: return "card_list_28";
                case CardValue.FOUR: return "card_list_29";
                case CardValue.FIVE: return "card_list_30";
                case CardValue.SIX: return "card_list_31";
                case CardValue.SEVEN: return "card_list_32";
                case CardValue.EIGHT: return "card_list_33";
                case CardValue.NINE: return "card_list_34";
                case CardValue.TEN: return "card_list_35";
                case CardValue.JACK: return "card_list_36";
                case CardValue.QUEEN: return "card_list_37";
                case CardValue.KING: return "card_list_38";
                case CardValue.ACE: return "card_list_26";
            }
        }
        else if (type == Suite.Club)
        {
            switch (val)
            {
                case CardValue.TWO: return "card_list_40";
                case CardValue.THREE: return "card_list_41";
                case CardValue.FOUR: return "card_list_42";
                case CardValue.FIVE: return "card_list_43";
                case CardValue.SIX: return "card_list_44";
                case CardValue.SEVEN: return "card_list_45";
                case CardValue.EIGHT: return "card_list_46";
                case CardValue.NINE: return "card_list_47";
                case CardValue.TEN: return "card_list_48";
                case CardValue.JACK: return "card_list_49";
                case CardValue.QUEEN: return "card_list_50";
                case CardValue.KING: return "card_list_51";
                case CardValue.ACE: return "card_list_39";
            }
        }
        else if (type == Suite.Undefined)
        {
            if (val == CardValue.JOKER)
                return JokerCard;
        }
        else
        {
            return CardBack;
        }
        return CardBack;
    }
}
