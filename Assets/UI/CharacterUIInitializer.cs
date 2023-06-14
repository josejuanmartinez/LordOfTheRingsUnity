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
    
    public Sprite free;
    public Sprite dark;
    public Sprite neutral;
    public Sprite renegade;
    public Sprite chaotic;

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

        switch (Nations.alignments[cardDetails.GetCardInPlay().owner])
        {
            case AlignmentsEnum.FREE_PEOPLE:
                alignment.sprite = free;
                break;
            case AlignmentsEnum.NEUTRAL:
                alignment.sprite = neutral;
                break;
            case AlignmentsEnum.DARK_SERVANTS:
                alignment.sprite = dark;
                break;
            case AlignmentsEnum.RENEGADE:
                alignment.sprite = renegade;
                break;
            case AlignmentsEnum.CHAOTIC:
                alignment.sprite = chaotic;
                break;
        }
    }

}
