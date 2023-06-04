using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CitySizesEnum {
    VERY_BIG,
    BIG,
    MEDIUM,
    SMALL,
    VERY_SMALL
}

public static class CitySizesCritical
{
    public static Dictionary<CitySizesEnum, TerrainBonusesEnum> sizeBonuses = new Dictionary<CitySizesEnum, TerrainBonusesEnum>() {
        { CitySizesEnum.VERY_BIG, TerrainBonusesEnum.MID_GOLD },
        { CitySizesEnum.BIG, TerrainBonusesEnum.LOW_GOLD },
        { CitySizesEnum.MEDIUM, TerrainBonusesEnum.LOW_PROD },
        { CitySizesEnum.SMALL, TerrainBonusesEnum.MID_PROD },
        { CitySizesEnum.VERY_SMALL, TerrainBonusesEnum.HIGH_FOOD},
    };
}
