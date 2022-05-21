using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerType
{
    Player,
    AI
}

public enum Emotion
{
    Happy,
    Angry
}

public enum PlayerID
{
    JACK,
    ANDREAS,
    JANE,
    ANE,
    INVALIDPLAYER
}

public enum PlayerState
{
    WAITING,
    DOINGTURN
}

public class Player : MonoBehaviour
{
    public List<PlayerCard> playerCard;
    public List<Card> HandCard = new List<Card>();
    public List<CardGroup> GroupedCard = new List<CardGroup>();

    public PlayerType Type = PlayerType.Player;
    public Emotion emotion = Emotion.Happy;
    public PlayerID ID = PlayerID.INVALIDPLAYER;

    public PlayerState playerState = PlayerState.WAITING; 

    public CharacterAvatar avatar;

    public List<Card> SelectedCard = new List<Card>();
    public CardGroup SelectedCardGroup;

    public bool PassTurn = false;

    public void InitPlayer()
    {
        avatar.HappySprite = avatar.AvatarSprite[ID][0];
        avatar.AngrySprite = avatar.AvatarSprite[ID][1];

        avatar.SetAvatar(emotion);
    }
}
