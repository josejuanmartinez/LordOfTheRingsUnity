using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardDetails))]
public class CharacterCardDetails : MonoBehaviour
{
    public bool isAvatar;
    public RacesEnum race;
    public SubRacesEnum subRace = SubRacesEnum.None;
    public List<CharacterClassEnum> classes;
    public List<CharacterAbilitiesEnum> abilities;
    public List<string> homeTown;
    public bool isAgent;
    public bool isWinged;
    public bool isLeader;
    public bool isElvenLord;
    public bool isDwarfLord;
    public bool isInmovable = false;
    public short globalInfluenceModifier = 0;

    public short prowess;
    public short defence;

    public short mind;
    public short influence;

    private CardDetails cardDetails;

    private void Awake()
    {
        cardDetails = GetComponent<CardDetails>();
    }

    public CardDetails GetCardDetails()
    {
        return cardDetails;
    }
}
