using System.Collections.Generic;
using System.Linq;

public class CardManager
{
    
    Board board;
    public CardManager(Board board)
    {
        this.board = board;
    }

    public List<CardInPlay> GetCardsOfPlayer(NationsEnum owner)
    {
        return board.GetTiles().Values.SelectMany(x => x.GetCards()).Where(y => y.owner == owner).ToList();
    }
}
