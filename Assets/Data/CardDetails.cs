using UnityEngine;

public class CardDetails : MonoBehaviour
{
    public Sprite cardSprite;
    public string cardId;
    public string groupId;
    public string cardName;
    public CardClass cardClass;
    public AlignmentsEnum alignment;

    public bool isUnique;
    public short victoryPoints;
    public short victoryPointsModifierIfEliminated;
    public short corruption;

    private bool isInPlay = false;

    void Awake()
    {
        if (GetComponent<CharacterCardDetails>() != null)
            cardClass = CardClass.Character;
        if (GetComponent<HazardCreatureCardDetails>() != null)
            cardClass = CardClass.HazardCreature;
    }
    public void Play()
    {
        isInPlay = true;
    }
    public bool IsInPlay() 
    { 
        return isInPlay; 
    }

    public CardInPlay GetCardInPlay()
    {
        if (!isInPlay)
            return null;
        else
            return gameObject.GetComponent<CardInPlay>();
    }

    public HazardCreatureCardDetails GetHazardCreatureCardDetails()
    {
        if (cardClass != CardClass.HazardCreature)
            return null;
        else
            return gameObject.GetComponent<HazardCreatureCardDetails>();
    }

    public CharacterCardDetails GetCharacterCardDetails()
    {
        if (cardClass != CardClass.Character)
            return null;
        else
            return gameObject.GetComponent<CharacterCardDetails>();
    }
}