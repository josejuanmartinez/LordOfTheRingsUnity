using System.Collections.Generic;
using System.Linq;

public class CityManager
{
    Board board;
    public CityManager(Board b)
    {
        board = b;
    }

    public List<CityInPlay> GetCitiesOfPlayer(NationsEnum owner)
    {
        List<CityInPlay> res = board.GetTiles().Values.Where(x => x.GetCity() != null && x.GetCity().owner == owner).Select(x => x.GetCity()).ToList();
        return res;
    }

    public List<string> GetCitiesStringsOfPlayer(NationsEnum owner)
    {
        List<CityInPlay> cities = GetCitiesOfPlayer(owner);
        return cities.Select(x => x.GetDetails().cityName).Union(cities.Select(x => x.GetDetails().regionId)).ToList();
    }

    public List<string> GetCityStringsWithCharactersOfPlayer(NationsEnum owner)
    {
        List<CityInPlay> cities = GetCitiesOfPlayer(owner);
        return cities.Select(x => x.GetDetails().cityName).ToList();
    }

    public CityInPlay GetHavenOfPlayer(NationsEnum owner)
    {
        List<CityInPlay> cities = GetCitiesOfPlayer(owner);
        return cities.First(x => x.GetDetails().IsHaven());
    }
    public CityInPlay GetCityOfPlayer(NationsEnum owner, string cityName)
    {
        List<CityInPlay> cities = GetCitiesOfPlayer(owner);
        return cities.First(x => x.GetDetails().cityName == cityName);
    }
}
