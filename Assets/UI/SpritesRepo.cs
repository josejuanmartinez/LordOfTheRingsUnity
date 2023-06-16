using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpritesRepo : MonoBehaviour
{
    public Sprite wild;
    public Sprite town;
    public Sprite darkFortress;
    public Sprite character;
    public Sprite item;

    public Sprite free;
    public Sprite dark;
    public Sprite renegade;
    public Sprite neutral;
    public Sprite chaos;
    public Sprite poi;

    public List<RacesEnum> races;
    public List<Sprite> racesSprites;

    public Sprite raceDefaultSprite;

    void Awake()
    {
        Assert.AreEqual(races.Count, racesSprites.Count);
    }

    public Sprite GetRaceSprite(RacesEnum race)
    {
        int position = races.FindIndex(x => x == race);
        if (position == -1)
            return raceDefaultSprite;
        else
            return racesSprites[position];
    }
    public Sprite GetAlignmentSprite(NationsEnum nation)
    {
        if (nation == NationsEnum.NONE)
            return poi;

        switch (Nations.alignments[nation])
        {
            case AlignmentsEnum.NEUTRAL:
                return neutral;
            case AlignmentsEnum.FREE_PEOPLE:
                return free;
            case AlignmentsEnum.DARK_SERVANTS:
                return dark;
            case AlignmentsEnum.RENEGADE:
                return renegade;
            case AlignmentsEnum.CHAOTIC:
                return chaos;
            case AlignmentsEnum.NONE:
                return poi;
        }
        return poi;
    }
}
