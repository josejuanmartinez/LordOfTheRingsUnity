using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    private NationsEnum nation;
    private AlignmentsEnum aligment;

    public Player(NationsEnum nation)
    {
        this.nation = nation;
        aligment = Nations.alignments[nation];
    }

    public NationsEnum GetNation()
    {
        return nation;
    }

    public AlignmentsEnum GetAlignment()
    {
        return aligment;
    }
}