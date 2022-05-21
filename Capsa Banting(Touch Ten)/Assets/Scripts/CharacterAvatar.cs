using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CharacterAvatar : MonoBehaviour
{
    public Image UIcharacterrenderer;
    public SpriteRenderer characterRenderer;
    public SpriteAtlas characterAtlas;

    string HappySpriteName;
    string AngrySpriteName;

    public bool RenderInUI;

    public Dictionary<PlayerID, List<string>> AvatarSprite = new Dictionary<PlayerID, List<string>>()
    {
        { PlayerID.INVALIDPLAYER, new List<string>{ "character_invalid", "character_invalid" } },
        { PlayerID.ANDREAS, new List<string>{ "BoyFace_2_Smile", "BoyFace_2_Angry" } },
        { PlayerID.JACK, new List<string>{ "BoyFace_Smile", "BoyFace_Angry" } },
        { PlayerID.JANE, new List<string>{ "GirlFace_2_Smile", "GirlFace_2_Angry" } },
        { PlayerID.ANE, new List<string>{ "GirlFace_Smile", "GirlFace_Angry" } },
    };

    public string HappySprite
    {
        set
        {
            HappySpriteName = value;
        }
    }

    public string AngrySprite
    {
        set
        {
            AngrySpriteName = value;
        }
    }

    public void SetAvatar(Emotion emo)
    {
        switch (emo)
        {
            case Emotion.Happy:
                if(RenderInUI)
                    UIcharacterrenderer.sprite = characterAtlas.GetSprite(HappySpriteName);
                else
                    characterRenderer.sprite = characterAtlas.GetSprite(HappySpriteName);

                break;
            case Emotion.Angry:
                if (RenderInUI)
                    UIcharacterrenderer.sprite = characterAtlas.GetSprite(AngrySpriteName);
                else
                    characterRenderer.sprite = characterAtlas.GetSprite(AngrySpriteName);

                break;
        }
    }
}
