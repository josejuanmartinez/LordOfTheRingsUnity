using System.Collections.Generic;

public enum NationsEnum
{
    NONE,
    UVATHA,
    KHAMUL,
    ADUNAPHEL,
    BEORN_THE_GOBLIN_BASHER,
    BARD_BLACK_ARROW,
    THRANDUIL_OF_MIRKWOOD,
    HUZ,
    THE_MOUTH,
    //SARUMAN,
    SARUMAN_OF_MANY_COLOURS,
    THEODEN_KING,
    THRAIN,
    THE_WITCH_KING,
    //RADAGAST,
    RADAGAST_SHADOWED_WANDERER,
    LADY_GALADRIEL,
    THE_BALROG,
    SMAUG_THE_GOLDEN
}

public enum NationRegionsEnum
{
    SEA_OF_RHUN,
    SOUTHERN_MIRKWOOD,
    CHELKAR,
    ANDUIN_VALES,
    NORTHERN_RHOVANION,
    WOODLAND_REALM,
    HORSE_PLAINS,
    GORGOROTH,
    //GAP_OF_ISEN,
    GAP_OF_ISEN,
    ROHAN,
    IRON_HILLS,
    ANGMAR,
    //WESTERN_MIRKWOOD,
    WESTERN_MIRKWOOD,
    WOLD_AND_FOOTHILLS,
    REDHORN_GATE,
    WITHERED_HEATH
}

public enum AlignmentsEnum
{
    FREE_PEOPLE,
    DARK_SERVANTS,
    RENEGADE,
    NEUTRAL,
    CHAOTIC,
    NONE
}

public static class Nations
{
    public static Dictionary<NationsEnum, AlignmentsEnum> alignments = new Dictionary<NationsEnum, AlignmentsEnum>()
    {
        { NationsEnum.UVATHA, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.KHAMUL, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.ADUNAPHEL, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.BEORN_THE_GOBLIN_BASHER , AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.BARD_BLACK_ARROW , AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.THRANDUIL_OF_MIRKWOOD, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.HUZ, AlignmentsEnum.NEUTRAL },
        { NationsEnum.THE_MOUTH, AlignmentsEnum.DARK_SERVANTS },
        //{ NationsEnum.SARUMAN, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.SARUMAN_OF_MANY_COLOURS, AlignmentsEnum.RENEGADE },
        { NationsEnum.THEODEN_KING, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.THRAIN, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.THE_WITCH_KING, AlignmentsEnum.DARK_SERVANTS },
        //{ NationsEnum.RADAGAST, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.RADAGAST_SHADOWED_WANDERER, AlignmentsEnum.RENEGADE },
        { NationsEnum.LADY_GALADRIEL, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.THE_BALROG, AlignmentsEnum.CHAOTIC },
        { NationsEnum.SMAUG_THE_GOLDEN, AlignmentsEnum.CHAOTIC },
    };

    public static Dictionary<NationsEnum, NationRegionsEnum> regions = new Dictionary<NationsEnum, NationRegionsEnum>()
    {
        { NationsEnum.UVATHA, NationRegionsEnum.SEA_OF_RHUN },
        { NationsEnum.KHAMUL, NationRegionsEnum.SOUTHERN_MIRKWOOD },
        { NationsEnum.ADUNAPHEL, NationRegionsEnum.CHELKAR },
        { NationsEnum.BEORN_THE_GOBLIN_BASHER , NationRegionsEnum.ANDUIN_VALES},
        { NationsEnum.BARD_BLACK_ARROW , NationRegionsEnum.NORTHERN_RHOVANION },
        { NationsEnum.THRANDUIL_OF_MIRKWOOD, NationRegionsEnum.WOODLAND_REALM },
        { NationsEnum.HUZ, NationRegionsEnum.HORSE_PLAINS },
        { NationsEnum.THE_MOUTH, NationRegionsEnum.GORGOROTH },
        //{ NationsEnum.SARUMAN, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.SARUMAN_OF_MANY_COLOURS, NationRegionsEnum.GAP_OF_ISEN },
        { NationsEnum.THEODEN_KING, NationRegionsEnum.ROHAN },
        { NationsEnum.THRAIN, NationRegionsEnum.IRON_HILLS },
        { NationsEnum.THE_WITCH_KING, NationRegionsEnum.ANGMAR },
        //{ NationsEnum.RADAGAST, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.RADAGAST_SHADOWED_WANDERER, NationRegionsEnum.WESTERN_MIRKWOOD },
        { NationsEnum.LADY_GALADRIEL, NationRegionsEnum.WOLD_AND_FOOTHILLS },
        { NationsEnum.THE_BALROG, NationRegionsEnum.REDHORN_GATE },
        { NationsEnum.SMAUG_THE_GOLDEN, NationRegionsEnum.WITHERED_HEATH }
    };

    public static short INFLUENCE = 20;

}
