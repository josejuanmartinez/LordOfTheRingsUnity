using System.Collections.Generic;

public enum AutomaticAttackEnum
{
    Nazgul115,
    Orcs27,
    Men28,
    Men29,
    Men36,
    Men38,
    MenAll8,
    ElvesAll9,
    AnimalsAll9,
    Men18,
    MenAll9,
    MenAll7,
    Men49,
    Animals55,
    Men37,
    Men48,
    MenAll5,
    Orcs38,
    Trap19,
    RandomNeutral,
    RandomFrontier,
    Undead16,
    Wolves37,
    Plant110EntCanTapDefenderChoses,
    NoAttacks,
    Dwarves410DetaintmentIfDwarfDiscardTresureToCancel,
    DwarvesAll7CancelIfOnlyDwarves,
    MenAll10,
    MenAll6,
    Spider27,
    Orcs37,
    Trolls28,
    Orcs35,
    Trolls110,
    Elves28,
    Elves310,
    ChosenByOpponentLairWildernessNeutral,
    Men110,
    Orcs36,
    Orcs28,
    OrcsAll7,
    Bears26,
    Maia113,
    Men27,
    Animals210,
    Undead18,
    Undead18CorruptionMinusTwoCheckIfWounded,
    Elves48,
    Elves39,
    Elves210,
    Elves310CancelledIfLessThanSixDetainmentIfLessThan12,
    Orcs47,
    Traps210DetaintmentIfElf,
    Dragon114,
    Dwarves28,
    Dwarves39,
    Undead28CorruptionMinusTwoCheckIfWounded,
}

public struct AutomaticAttackStruct
{
    public RacesEnum race;
    public short strikes;
    public short prowess;
    public short defence;
    public AutomaticAttackStruct(RacesEnum race, short strikes, short prowess, short defence)
    {
        this.race = race;
        this.strikes = strikes;
        this.prowess = prowess;
        this.defence = defence;
    }
}

public static class AutomaticAttack
{
    public static Dictionary<AutomaticAttackEnum, AutomaticAttackStruct> automaticAttacks = new Dictionary<AutomaticAttackEnum, AutomaticAttackStruct>()
    {
        { AutomaticAttackEnum.Nazgul115, new AutomaticAttackStruct( RacesEnum.Ringwraith, 1, 15, 0) },
        { AutomaticAttackEnum.Men28, new AutomaticAttackStruct( RacesEnum.Man, 2, 8, 0) },
        { AutomaticAttackEnum.Men29, new AutomaticAttackStruct( RacesEnum.Man, 2, 9, 0) },
        { AutomaticAttackEnum.Men36, new AutomaticAttackStruct( RacesEnum.Man, 3, 6, 0) },
        { AutomaticAttackEnum.Men38, new AutomaticAttackStruct( RacesEnum.Man, 3, 8, 0) },
        { AutomaticAttackEnum.Orcs27, new AutomaticAttackStruct( RacesEnum.Orc, 2, 7, 0) },
        { AutomaticAttackEnum.MenAll8, new AutomaticAttackStruct( RacesEnum.Man, -1, 7, 0) },
        { AutomaticAttackEnum.ElvesAll9, new AutomaticAttackStruct( RacesEnum.Elf, -1, 9, 0) },
        { AutomaticAttackEnum.AnimalsAll9, new AutomaticAttackStruct( RacesEnum.Animal, -1, 9, 0) },
        { AutomaticAttackEnum.Men18, new AutomaticAttackStruct( RacesEnum.Man, 1, 8, 0) },
        { AutomaticAttackEnum.MenAll9, new AutomaticAttackStruct( RacesEnum.Man, -1, 9, 0) },
        { AutomaticAttackEnum.MenAll7, new AutomaticAttackStruct( RacesEnum.Man, -1, 7, 0) },
        { AutomaticAttackEnum.MenAll7, new AutomaticAttackStruct( RacesEnum.Man, 4, 9, 0) },
        { AutomaticAttackEnum.Animals55, new AutomaticAttackStruct( RacesEnum.Animal, 5, 5, 0) },
        { AutomaticAttackEnum.Men37, new AutomaticAttackStruct( RacesEnum.Man, 3, 7, 0) },
        { AutomaticAttackEnum.Men48, new AutomaticAttackStruct( RacesEnum.Man, 4, 8, 0) },
        { AutomaticAttackEnum.MenAll5, new AutomaticAttackStruct( RacesEnum.Man, -1, 5, 0) },
        { AutomaticAttackEnum.Orcs38, new AutomaticAttackStruct( RacesEnum.Orc, 3, 8, 0) },
        { AutomaticAttackEnum.Trap19, new AutomaticAttackStruct( RacesEnum.Trap, 1, 9, 0) },
        { AutomaticAttackEnum.Undead16, new AutomaticAttackStruct( RacesEnum.Undead, 1, 6, 0) },
        { AutomaticAttackEnum.Wolves37, new AutomaticAttackStruct( RacesEnum.Wolf, 3, 7, 0) },
        { AutomaticAttackEnum.Plant110EntCanTapDefenderChoses, new AutomaticAttackStruct( RacesEnum.Plant, 1, 10, 0) },
        { AutomaticAttackEnum.MenAll10, new AutomaticAttackStruct( RacesEnum.Man, -1, 10, 0) },
        { AutomaticAttackEnum.MenAll6, new AutomaticAttackStruct( RacesEnum.Man, -1, 6, 0) },
        { AutomaticAttackEnum.Spider27, new AutomaticAttackStruct( RacesEnum.Spider, 2, 7, 0) },
        { AutomaticAttackEnum.Orcs37, new AutomaticAttackStruct( RacesEnum.Orc, 3, 7, 0) },
        { AutomaticAttackEnum.Trolls28, new AutomaticAttackStruct( RacesEnum.Troll, 2, 8, 0) },
        { AutomaticAttackEnum.Orcs35, new AutomaticAttackStruct( RacesEnum.Orc, 3, 5, 0) },
        { AutomaticAttackEnum.Trolls110, new AutomaticAttackStruct( RacesEnum.Troll, 1, 10, 0) },
        { AutomaticAttackEnum.Elves28, new AutomaticAttackStruct( RacesEnum.Elf, 2, 8, 0) },
        { AutomaticAttackEnum.Elves310, new AutomaticAttackStruct( RacesEnum.Elf, 3, 10, 0) },
        { AutomaticAttackEnum.Men110, new AutomaticAttackStruct( RacesEnum.Man, 1, 10, 0) },
        { AutomaticAttackEnum.Orcs36, new AutomaticAttackStruct( RacesEnum.Orc, 3, 6, 0) },
        { AutomaticAttackEnum.Orcs28, new AutomaticAttackStruct( RacesEnum.Orc, 2, 8, 0) },
        { AutomaticAttackEnum.OrcsAll7, new AutomaticAttackStruct( RacesEnum.Orc, -1, 7, 0) },
        { AutomaticAttackEnum.Bears26, new AutomaticAttackStruct( RacesEnum.Bear, 2, 6, 0) },
        { AutomaticAttackEnum.Maia113, new AutomaticAttackStruct( RacesEnum.Maia, 1, 13, 0) },
        { AutomaticAttackEnum.Men27, new AutomaticAttackStruct( RacesEnum.Man, 2, 7, 0) },
        { AutomaticAttackEnum.Animals210, new AutomaticAttackStruct( RacesEnum.Animal, 2, 10, 0) },
        { AutomaticAttackEnum.Undead18, new AutomaticAttackStruct(RacesEnum.Undead, 1, 8, 0) },
        { AutomaticAttackEnum.Undead18CorruptionMinusTwoCheckIfWounded, new AutomaticAttackStruct(RacesEnum.Undead, 1, 8, 0) },
        { AutomaticAttackEnum.Undead28CorruptionMinusTwoCheckIfWounded, new AutomaticAttackStruct(RacesEnum.Undead, 2, 8, 0) },
        { AutomaticAttackEnum.Elves48, new AutomaticAttackStruct(RacesEnum.Elf, 4, 8, 0) },
        { AutomaticAttackEnum.Elves39, new AutomaticAttackStruct(RacesEnum.Elf, 3, 9, 0) },
        { AutomaticAttackEnum.Elves210, new AutomaticAttackStruct(RacesEnum.Elf, 2, 10, 0) },
        { AutomaticAttackEnum.Elves310CancelledIfLessThanSixDetainmentIfLessThan12, new AutomaticAttackStruct(RacesEnum.Elf, 3, 10, 0) },
        { AutomaticAttackEnum.Orcs47, new AutomaticAttackStruct(RacesEnum.Orc, 4, 7, 0) },
        { AutomaticAttackEnum.Traps210DetaintmentIfElf, new AutomaticAttackStruct(RacesEnum.Orc, 2, 10, 0) },
        { AutomaticAttackEnum.Dragon114, new AutomaticAttackStruct(RacesEnum.Dragon, 1, 14, 0) },
        { AutomaticAttackEnum.Dwarves28, new AutomaticAttackStruct(RacesEnum.Dragon, 2, 8, 0) },
        { AutomaticAttackEnum.Dwarves39, new AutomaticAttackStruct(RacesEnum.Dragon, 3, 9, 0) },
    };
}