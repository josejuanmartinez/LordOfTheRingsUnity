using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainBonusesEnum {
    VERY_HIGH_GOLD,
    HIGH_GOLD,
    MID_GOLD,
    LOW_GOLD,
    VERY_LOW_GOLD,
    VERY_HIGH_PROD,
    HIGH_PROD,
    MID_PROD,
    LOW_PROD,
    VERY_LOW_PROD,
    VERY_HIGH_FOOD,
    HIGH_FOOD,
    MID_FOOD,
    LOW_FOOD,
    VERY_LOW_FOOD
}


public static class TerrainBonuses
{
    public static Dictionary<TerrainBonusesEnum, short> maxBonuses = new Dictionary<TerrainBonusesEnum, short>() {
        { TerrainBonusesEnum.VERY_HIGH_GOLD, 4 },
        { TerrainBonusesEnum.HIGH_GOLD, 3 },
        { TerrainBonusesEnum.MID_GOLD, 2 },
        { TerrainBonusesEnum.LOW_GOLD, 1 },
        { TerrainBonusesEnum.VERY_LOW_GOLD, 0 },
        { TerrainBonusesEnum.VERY_HIGH_PROD, 5 },
        { TerrainBonusesEnum.HIGH_PROD, 4 },
        { TerrainBonusesEnum.MID_PROD, 3 },
        { TerrainBonusesEnum.LOW_PROD, 2 },
        { TerrainBonusesEnum.VERY_LOW_PROD, 1 },
        { TerrainBonusesEnum.VERY_HIGH_FOOD, 5 },
        { TerrainBonusesEnum.HIGH_FOOD, 4 },
        { TerrainBonusesEnum.MID_FOOD, 3 },
        { TerrainBonusesEnum.LOW_FOOD, 2 },
        { TerrainBonusesEnum.VERY_LOW_FOOD, 1 },
    };

    public static Dictionary<TerrainBonusesEnum, short> minBonuses = new Dictionary<TerrainBonusesEnum, short>() {
        { TerrainBonusesEnum.VERY_HIGH_GOLD, 2 },
        { TerrainBonusesEnum.HIGH_GOLD, 1 },
        { TerrainBonusesEnum.MID_GOLD, 1 },
        { TerrainBonusesEnum.LOW_GOLD, 0 },
        { TerrainBonusesEnum.VERY_LOW_GOLD, 0 },
        { TerrainBonusesEnum.VERY_HIGH_PROD, 2 },
        { TerrainBonusesEnum.HIGH_PROD, 2 },
        { TerrainBonusesEnum.MID_PROD, 1 },
        { TerrainBonusesEnum.LOW_PROD, 1 },
        { TerrainBonusesEnum.VERY_LOW_PROD, 1 },
        { TerrainBonusesEnum.VERY_HIGH_FOOD, 2 },
        { TerrainBonusesEnum.HIGH_FOOD, 2 },
        { TerrainBonusesEnum.MID_FOOD, 1 },
        { TerrainBonusesEnum.LOW_FOOD, 1 },
        { TerrainBonusesEnum.VERY_LOW_FOOD, 0 },
    };

}
