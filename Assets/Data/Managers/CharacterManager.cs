using System.Collections.Generic;
using System.Linq;

public class CharacterManager
{
    Board board;
    public CharacterManager(Board board)
    {
        this.board = board;
    }

    public List<CardInPlay> GetCharactersOfPlayer(NationsEnum owner)
    {
        return board.GetTiles().Values.SelectMany(x => x.GetCards()).Where(y => y.GetCardClass() == CardClass.Character && y.owner == owner).ToList();
    }

    public List<CardInPlay> GetCharactersOfPlayerNonAvatar(NationsEnum owner)
    {
        return board.GetTiles().Values.SelectMany(x => x.GetCards()).Where(y => y.GetCardClass() == CardClass.Character && y.owner == owner && !y.IsAvatar()).ToList();
    }

    public List<CityInPlay> GetCitiesWithCharactersOfPlayer(NationsEnum owner)
    {
        List<CardInPlay> character = GetCharactersOfPlayer(owner);
        return character.Select(x => x.hex).Where(x => board.GetTile(x).HasCity()).Select(x => board.GetTile(x).GetCity()).ToList();
    }

    public List<string> GetCityStringsWithCharactersOfPlayer(NationsEnum owner)
    {
        List<CardInPlay> character = GetCharactersOfPlayer(owner);
        return character.Select(x => x.hex).Where(x => board.GetTile(x).HasCity()).Select(x => board.GetTile(x).GetCity().GetDetails().cityName).ToList();
    }
    public CardInPlay GetAvatar(NationsEnum owner)
    {
        List<CardInPlay> character = GetCharactersOfPlayer(owner);
        return character.First(x => x.IsAvatar());
    }
}
