using System.Collections.Generic;

public enum NationsEnum
{
    NONE,
    UVATHA,
    KHAMUL,
    ADUNAPHEL,
    ELROND,
    BEORN_THE_GOBLIN_BASHER,
    //ULRED,
    //ARYEN,
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
    GALADRIEL,
    THE_BALROG,
    SMAUG_THE_GOLDEN
}

public enum AlignmentsEnum
{
    FREE_PEOPLE,
    DARK_SERVANTS,
    RENEGADE,
    NEUTRAL,
    CHAOTIC
}

public static class Nations
{
    public static Dictionary<NationsEnum, AlignmentsEnum> alignments = new Dictionary<NationsEnum, AlignmentsEnum>()
    {
        { NationsEnum.UVATHA, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.KHAMUL, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.ADUNAPHEL, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.ELROND , AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.BEORN_THE_GOBLIN_BASHER , AlignmentsEnum.FREE_PEOPLE },
        //{ NationsEnum.ULRED , AlignmentsEnum.FREE_PEOPLE },
        //{ NationsEnum.ARYEN , AlignmentsEnum.FREE_PEOPLE },
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
        { NationsEnum.GALADRIEL, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.THE_BALROG, AlignmentsEnum.CHAOTIC },
        { NationsEnum.SMAUG_THE_GOLDEN, AlignmentsEnum.CHAOTIC },
    };

    public static short INFLUENCE = 20;

}
