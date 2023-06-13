using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldRingDetails : MonoBehaviour
{
    public List<string> hometown;
    public List<CardTypesEnum> placeTypes;

    public bool onlyDragonLair;

    public short theOneRingMax = 12;
    public short theOneRingMin = 11;

    public short spiritRingMax = 12;
    public short spiritRingMin = 9;

    public short dwarvenRingMax = 12;
    public short dwarvenRingMin = 9;

    public short magicRingMax = 6;
    public short magicRingMin = 1;

    public bool canGetLesserFromDiscard;

}
