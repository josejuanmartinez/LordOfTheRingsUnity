using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIInitializer : MonoBehaviour
{
    public TextMeshProUGUI details;
    public Image cardImage;
    public TextMeshProUGUI cardName;
    public Image alignment;
    private SpritesRepo spritesRepo;

    private void Awake()
    {
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
    }

    public void Initialize(CardInPlay card)
    {
        if (card.GetDetails() == null)
        {
            Debug.LogError("Unable to get CardDetails from " + card.name);
            return;
        }
        CardDetails cardDetails = card.GetDetails();
        this.cardImage.sprite = cardDetails.cardSprite;
        this.cardName.text = cardDetails.cardName;

        alignment.sprite = spritesRepo.GetAlignmentSprite(cardDetails.GetCardInPlay().owner);

        if (card.IsCharacter() && card.GetCharacterDetails() != null)
        {
            CharacterCardDetails character = card.GetCharacterDetails();
            details.text = "<sprite name=\"prowess\">" + character.prowess + "\n<sprite name=\"defence\">" + character.defence + "\n<sprite name=\"movement\">" + (MovementConstants.characterMovement - card.moved).ToString();
        } else if (card.IsCreature() && card.GetHazardCreatureDetails() != null)
        {
            HazardCreatureCardDetails creature = card.GetHazardCreatureDetails();
            details.text = "<sprite name=\"prowess\">" + creature.prowess + "\n<sprite name=\"defence\">" + creature.defence + "\n<sprite name=\"movement\">" + (MovementConstants.characterMovement - card.moved).ToString();
        }
    }

}
