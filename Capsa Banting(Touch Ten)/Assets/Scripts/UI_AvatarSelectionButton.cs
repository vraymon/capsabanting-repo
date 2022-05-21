using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_AvatarSelectionButton : MonoBehaviour
{
    public PlayerID avatarID;
    public TMP_Text charName;
    public Button button;

    public CharacterAvatar characterava;

    private void Start()
    {
        charName.text = avatarID.ToString();
        characterava.HappySprite = characterava.AvatarSprite[avatarID][0];
        characterava.AngrySprite = characterava.AvatarSprite[avatarID][1];
        characterava.SetAvatar(Emotion.Angry);
    }

    public void SelectCharacter()
    {
        characterava.SetAvatar(Emotion.Happy);
        GameLogic.Instance.selectedCharacter = avatarID;
    }

    public void ResetButton()
    {
        characterava.SetAvatar(Emotion.Angry);
    }
}
