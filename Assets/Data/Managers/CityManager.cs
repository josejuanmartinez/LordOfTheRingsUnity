using System;
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
        return cities.Select(x => x.cityId).Union(cities.Select(x => x.GetDetails().regionId)).ToList();
    }

    /*public List<string> GetCityStringsWithCharactersOfPlayer(NationsEnum owner)
    {
        List<CityInPlay> cities = GetCitiesOfPlayer(owner);
        return cities.Select(x => x.cityId).ToList();
    }*/

    public CityInPlay GetHavenOfPlayer(NationsEnum owner)
    {
        List<CityInPlay> cities = GetCitiesOfPlayer(owner);
        return cities.First(x => x.GetDetails().IsHaven());
    }
    public CityInPlay GetCityOfPlayer(NationsEnum owner, string cityName)
    {
        List<CityInPlay> cities = GetCitiesOfPlayer(owner);
        CityInPlay exactMatch = cities.DefaultIfEmpty(null).FirstOrDefault(x => x.cityId == cityName);
        if (exactMatch != null)
            return exactMatch;
        Random random = new Random();
        if (exactMatch == null)
        {

            if (cityName == CitiesStringConstants.ANY)
            {
                int index = random.Next(cities.Count);
                return cities[index];
            }

            switch (Nations.alignments[owner])
            {
                case AlignmentsEnum.DARK_SERVANTS:
                    if (cityName == CitiesStringConstants.ANY_DARK)
                    {
                        int index = random.Next(cities.Count);
                        return cities[index];
                    }                        
                    break;
                case AlignmentsEnum.FREE_PEOPLE:
                    if (cityName == CitiesStringConstants.ANY_FREE)
                    {
                        int index = random.Next(cities.Count);
                        return cities[index];
                    }
                    break;
                case AlignmentsEnum.NEUTRAL:
                    if (cityName == CitiesStringConstants.ANY_NEUTRAL)
                    {
                        int index = random.Next(cities.Count);
                        return cities[index];
                    }
                    break;
                case AlignmentsEnum.RENEGADE:
                    if (cityName == CitiesStringConstants.ANY_RENEGADE)
                    {
                        int index = random.Next(cities.Count);
                        return cities[index];
                    }
                    break;
            }
        }
        return null;
    }
}
