using System.Collections.Generic;
using UnityEngine;

public struct CityProduction
{
    public short food;
    public short gold;
    public short prod;

    public CityProduction(short food, short gold, short prod)
    {
        this.food = food;
        this.gold = gold;
        this.prod = prod;
    }
    public CityProduction(CityProduction a)
    {
        food = a.food;
        gold = a.gold;
        prod = a.prod;        
    }
    public static CityProduction operator +(CityProduction a, CityProduction b)
       => new CityProduction((short)(a.food + b.food), (short)(a.gold + b.gold), (short)(a.prod + b.prod));

    public static CityProduction operator *(CityProduction a, short b)
       => new CityProduction((short)(a.food * b), (short)(a.gold * b), (short)(a.prod * b));
}

public class CityDetails: MonoBehaviour
{
    public CitySizesEnum size;
    public List<string> nearestHavenFree;
    public List<string> nearestHavenDark;
    public List<string> nearestHavenRenegade;
    
    public TerrainsEnum terrain;
    public CardTypesEnum cardType;
    public List<FactionsEnum> faction;

    public List<ObjectTypes> objectTypesFree;
    public List<ObjectTypes> objectTypesDark;
    public List<ObjectTypes> objectTypesRenegade;
    public List<AutomaticAttackEnum> automaticAttacksFree;
    public List<AutomaticAttackEnum> automaticAttacksDark;
    public List<AutomaticAttackEnum> automaticAttacksRenegade;
    public bool isHidden = false;
    public bool hasPort = false;
    public bool hasHoard = false;
    public bool isUnderground = false;
    public short iDrawFree;
    public short youDrawFree;
    public short iDrawDark;
    public short youDrawDark;
    public short iDrawRenegade;
    public short youDrawRenegade;

    public short corruption;
    public List<CitySpecials> citySpecials = new List<CitySpecials>();

    public string cityId;
    public string regionId;
    public string cityName;
    public bool isHaven;

    public CityProduction production;

    public Sprite freeSprite;
    public Sprite darkSprite;
    public Sprite renegadeSprite;
    public Sprite balrogSprite;
    public Sprite lordSprite;

    private HashSet<NationsEnum> tappers = new HashSet<NationsEnum>();
        
    public void Initialize()
    {
        production = CityDataGenerator.Generate(terrain, size);
    }

    public CityProduction GetCityProduction()
    {
        return production;
    }

    public bool IsHaven()
    {
        return isHaven;
    }
    public void Tap(NationsEnum nation)
    {
        tappers.Add(nation);
    }

    public bool IsTapped(NationsEnum nationEnum)
    {
        return tappers.Contains(nationEnum);
    }

    public Sprite GetSprite(NationsEnum nation)
    {
        switch (Nations.alignments[nation])
        {
            case AlignmentsEnum.DARK_SERVANTS:
                return darkSprite;
            case AlignmentsEnum.FREE_PEOPLE:
            case AlignmentsEnum.NEUTRAL:
                return freeSprite;
            case AlignmentsEnum.RENEGADE:
                if (renegadeSprite != null)
                    return renegadeSprite;
                else
                    return darkSprite;
            case AlignmentsEnum.CHAOTIC:
                if (balrogSprite != null)
                    return balrogSprite;
                else
                    return darkSprite;
            default:
                return freeSprite;
        }
    }

    public List<AutomaticAttackEnum> GetAutomaticAttacks(NationsEnum nation)
    {
        switch (Nations.alignments[nation])
        {
            case AlignmentsEnum.FREE_PEOPLE:
            case AlignmentsEnum.NEUTRAL:
                return automaticAttacksFree;
            case AlignmentsEnum.CHAOTIC:
            case AlignmentsEnum.DARK_SERVANTS:
                return automaticAttacksDark;
            case AlignmentsEnum.RENEGADE:
                if (automaticAttacksRenegade.Count > 0)
                    return automaticAttacksRenegade;
                else
                    return automaticAttacksDark;
            default:
                return automaticAttacksFree;
        }
    }
}

public static class CityDataGenerator
{
    public static CityProduction Generate(TerrainsEnum terrain, CitySizesEnum size)
    {
        System.Random rd = new System.Random();
        short food = (short) rd.Next(TerrainBonuses.minBonuses[Terrains.foodBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.foodBonuses[terrain]]);
        short gold= (short)rd.Next(TerrainBonuses.minBonuses[Terrains.goldBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.goldBonuses[terrain]]);
        short prod = (short)rd.Next(TerrainBonuses.minBonuses[Terrains.prodBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.prodBonuses[terrain]]);

        switch (CitySizesCritical.sizeBonuses[size])
        {
            case TerrainBonusesEnum.VERY_HIGH_GOLD:
            case TerrainBonusesEnum.HIGH_GOLD:
            case TerrainBonusesEnum.MID_GOLD:
            case TerrainBonusesEnum.LOW_GOLD:
            case TerrainBonusesEnum.VERY_LOW_GOLD:
                gold += (short)rd.Next(
                    TerrainBonuses.minBonuses[CitySizesCritical.sizeBonuses[size]],
                    TerrainBonuses.maxBonuses[CitySizesCritical.sizeBonuses[size]]
                    );
                break;


            case TerrainBonusesEnum.VERY_HIGH_PROD: 
            case TerrainBonusesEnum.HIGH_PROD: 
            case TerrainBonusesEnum.MID_PROD: 
            case TerrainBonusesEnum.LOW_PROD: 
            case TerrainBonusesEnum.VERY_LOW_PROD:
                prod += (short)rd.Next(
                    TerrainBonuses.minBonuses[CitySizesCritical.sizeBonuses[size]],
                    TerrainBonuses.maxBonuses[CitySizesCritical.sizeBonuses[size]]
                    );
                break;

            case TerrainBonusesEnum.VERY_HIGH_FOOD:
            case TerrainBonusesEnum.HIGH_FOOD: 
            case TerrainBonusesEnum.MID_FOOD: 
            case TerrainBonusesEnum.LOW_FOOD: 
            case TerrainBonusesEnum.VERY_LOW_FOOD:
                food += (short)rd.Next(
                    TerrainBonuses.minBonuses[CitySizesCritical.sizeBonuses[size]],
                    TerrainBonuses.maxBonuses[CitySizesCritical.sizeBonuses[size]]
                    );
                break;

        }
        
        
        return new CityProduction(food, gold, prod);
    }
}
