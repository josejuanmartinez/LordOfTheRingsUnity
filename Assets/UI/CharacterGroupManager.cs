using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterGroupManager : MonoBehaviour
{
    public Image companyLeaderSprite;
    public Image companyLeaderAlignment;
    public Image companyLeaderFrame;
    public TextMeshProUGUI companyLeaderName;
    public TextMeshProUGUI companyLeaderDetails;
    public GameObject allyPrefab;
    public GameObject objPregab;

    public void Initialize(CardInPlay card)
    {
        companyLeaderName.text = card.GetDetails().cardName;
        companyLeaderSprite.sprite = card.GetDetails().cardSprite;
        companyLeaderAlignment.sprite = card.GetCardUI().GetSprite();
        short prowessValue = card.GetCharacterDetails().prowess;
        short defenceValue = card.GetCharacterDetails().defence;
        short movementLeft = (short)(MovementConstants.characterMovement - card.moved);
        companyLeaderDetails.text = Sprites.prowess + prowessValue + "\n" + Sprites.defence + defenceValue + "\n" + Sprites.movement + movementLeft;
    }
}
