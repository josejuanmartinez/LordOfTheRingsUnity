using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCardDetails : MonoBehaviour
{
    public ObjectTypes itemType;
    public ObjectSlot objectSlot;
    public List<CharacterClassEnum> requiredClass;
    public List<ObjectAbilities> abilities;

    public bool cannotBeDuplicated = false;

    public short prowess;
    public short maxProwess = -1;

    public short defence;
    public short maxDefence = -1;

    public short mind;
    public short minMind = -1;

    public short influence;
    public short maxInfluence = -1;

    public short bodyCheckModifier = 0;

    private CardDetails cardDetails;

    public bool givesSage;
    public bool givesWarrior;
    public bool givesDiplomat;
    public bool givesScout;
    public bool givesRanger;

    private void Awake()
    {
        cardDetails = GetComponent<CardDetails>();
    }

    public CardDetails GetCardDetails()
    {
        return cardDetails;
    } 

    public bool IsBattleItem()
    {
        return objectSlot == ObjectSlot.Shield || objectSlot == ObjectSlot.Weapon || objectSlot == ObjectSlot.Armor || objectSlot == ObjectSlot.Helmet;
    }

    public bool IsUndiscoveredRing()
    {
        return objectSlot == ObjectSlot.MagicRing ||
            objectSlot == ObjectSlot.SpiritRing ||
            objectSlot == ObjectSlot.LesserRing ||
            objectSlot == ObjectSlot.DwarvenRing ||
            objectSlot == ObjectSlot.TheOneRing;
    }
}
