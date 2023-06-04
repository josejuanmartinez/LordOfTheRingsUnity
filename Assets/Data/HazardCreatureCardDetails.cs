using System.Collections.Generic;
using UnityEngine;

public class HazardCreatureCardDetails: MonoBehaviour
{
    public RacesEnum race;

    public short strikes;
    public short prowess;
    public short defence;

    public List<CardTypesEnum> cardTypes;
    public List<HazardAbilitiesEnum> hazardAbilities;
}
