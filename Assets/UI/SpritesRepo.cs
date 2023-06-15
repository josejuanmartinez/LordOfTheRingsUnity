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
}
