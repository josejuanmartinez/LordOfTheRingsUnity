using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TerrainsEnum
{
    SEA,
    COAST,
    WASTE,
    PLAINS,
    GRASS,
    ASHES,
    MOUNTAIN,
    HILLS,
    SNOWHILLS,
    ICE,
    DESERT,
    OTHER_HILLS_MOUNTAIN,
    FOREST,
    SWAMP
}

public static class Terrains
{
    public static Dictionary<TerrainsEnum, TerrainBonusesEnum> goldBonuses = new Dictionary<TerrainsEnum, TerrainBonusesEnum>()
    {
        { TerrainsEnum.SEA, TerrainBonusesEnum.LOW_GOLD },
        { TerrainsEnum.COAST, TerrainBonusesEnum.MID_GOLD },
        { TerrainsEnum.WASTE, TerrainBonusesEnum.MID_GOLD },
        { TerrainsEnum.PLAINS, TerrainBonusesEnum.MID_GOLD },
        { TerrainsEnum.GRASS, TerrainBonusesEnum.MID_GOLD },
        { TerrainsEnum.ASHES, TerrainBonusesEnum.LOW_GOLD },
        { TerrainsEnum.MOUNTAIN, TerrainBonusesEnum.HIGH_GOLD },
        { TerrainsEnum.HILLS, TerrainBonusesEnum.MID_GOLD },
        { TerrainsEnum.SNOWHILLS, TerrainBonusesEnum.VERY_HIGH_GOLD },
        { TerrainsEnum.ICE, TerrainBonusesEnum.LOW_GOLD },
        { TerrainsEnum.DESERT, TerrainBonusesEnum.MID_GOLD },
        { TerrainsEnum.OTHER_HILLS_MOUNTAIN, TerrainBonusesEnum.HIGH_GOLD },
        { TerrainsEnum.FOREST, TerrainBonusesEnum.LOW_GOLD },
        { TerrainsEnum.SWAMP, TerrainBonusesEnum.LOW_GOLD },
    };

    public static Dictionary<TerrainsEnum, TerrainBonusesEnum> prodBonuses = new Dictionary<TerrainsEnum, TerrainBonusesEnum>()
    {
        { TerrainsEnum.SEA, TerrainBonusesEnum.MID_PROD}, 
        { TerrainsEnum.COAST, TerrainBonusesEnum.MID_PROD }, 
        { TerrainsEnum.WASTE, TerrainBonusesEnum.HIGH_PROD }, 
        { TerrainsEnum.PLAINS, TerrainBonusesEnum.MID_PROD }, 
        { TerrainsEnum.GRASS, TerrainBonusesEnum.LOW_PROD}, 
        { TerrainsEnum.ASHES, TerrainBonusesEnum.VERY_HIGH_PROD }, 
        { TerrainsEnum.MOUNTAIN, TerrainBonusesEnum.HIGH_PROD }, 
        { TerrainsEnum.HILLS, TerrainBonusesEnum.HIGH_PROD }, 
        { TerrainsEnum.SNOWHILLS, TerrainBonusesEnum.MID_PROD }, 
        { TerrainsEnum.ICE, TerrainBonusesEnum.HIGH_PROD },
        { TerrainsEnum.DESERT, TerrainBonusesEnum.VERY_HIGH_PROD },
        { TerrainsEnum.OTHER_HILLS_MOUNTAIN, TerrainBonusesEnum.HIGH_PROD },
        { TerrainsEnum.FOREST, TerrainBonusesEnum.MID_PROD },
        { TerrainsEnum.SWAMP, TerrainBonusesEnum.LOW_PROD},
    };

    public static Dictionary<TerrainsEnum, TerrainBonusesEnum> foodBonuses = new Dictionary<TerrainsEnum, TerrainBonusesEnum>()
    {
                                                             // GOLD PROD FOOD
        { TerrainsEnum.SEA, TerrainBonusesEnum.HIGH_FOOD},   // LOW MID HIGH (9)
        { TerrainsEnum.COAST, TerrainBonusesEnum.MID_FOOD }, // MID MID MID (9)
        { TerrainsEnum.WASTE, TerrainBonusesEnum.LOW_FOOD }, // MID HIGH LOW (9)
        { TerrainsEnum.PLAINS, TerrainBonusesEnum.MID_FOOD }, // MID MID MID (9)
        { TerrainsEnum.GRASS, TerrainBonusesEnum.HIGH_FOOD}, // MID LOW HIGH (9)
        { TerrainsEnum.ASHES, TerrainBonusesEnum.LOW_FOOD }, // LOW VERY_HIGH LOW (9)
        { TerrainsEnum.MOUNTAIN, TerrainBonusesEnum.VERY_LOW_FOOD }, // HIGH HIGH VERY_LOW (9)
        { TerrainsEnum.HILLS, TerrainBonusesEnum.LOW_FOOD }, //MID HIGH LOW (9)
        { TerrainsEnum.SNOWHILLS, TerrainBonusesEnum.VERY_LOW_FOOD }, //VERY_HIGH MID VERY_LOW (9)
        { TerrainsEnum.ICE, TerrainBonusesEnum.MID_FOOD }, // LOW HIGH MID (9)
        { TerrainsEnum.DESERT, TerrainBonusesEnum.VERY_LOW_FOOD }, // MID VERY_HIGH VERY_LOW (9)
        { TerrainsEnum.OTHER_HILLS_MOUNTAIN, TerrainBonusesEnum.VERY_LOW_FOOD }, // HIGH HIGH VERY_LOW (9)
        { TerrainsEnum.FOREST, TerrainBonusesEnum.HIGH_FOOD}, // LOW MID HIGH (9)
        { TerrainsEnum.SWAMP, TerrainBonusesEnum.VERY_HIGH_FOOD } // LOW LOW VERY_HIGH (9)
    };

    public static Dictionary<TerrainsEnum, short> movementCost = new Dictionary<TerrainsEnum, short>()
    {
        { TerrainsEnum.SEA, 1},
        { TerrainsEnum.COAST, 2}, 
        { TerrainsEnum.WASTE, 1}, 
        { TerrainsEnum.PLAINS, 1},
        { TerrainsEnum.GRASS, 1}, 
        { TerrainsEnum.ASHES, 2}, 
        { TerrainsEnum.MOUNTAIN, 3}, 
        { TerrainsEnum.HILLS, 2}, 
        { TerrainsEnum.SNOWHILLS, 3}, 
        { TerrainsEnum.ICE, 2},
        { TerrainsEnum.DESERT, 2},
        { TerrainsEnum.OTHER_HILLS_MOUNTAIN, 3},
        { TerrainsEnum.FOREST, 2},
        { TerrainsEnum.SWAMP, 3},
    };
}