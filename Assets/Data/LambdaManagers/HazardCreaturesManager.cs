using System.Collections.Generic;
using System.Linq;

public class HazardCreaturesManager
{
    Board board;
    public HazardCreaturesManager(Board board)
    {
        this.board = board;
    }

    public List<CardInPlay> GetHazardCreaturesOfPlayer(NationsEnum owner)
    {
        return board.GetTiles().Values.SelectMany(x => x.GetCards()).Where(y => y.GetCardClass() == CardClass.HazardCreature && y.owner == owner).ToList();
    }
}
