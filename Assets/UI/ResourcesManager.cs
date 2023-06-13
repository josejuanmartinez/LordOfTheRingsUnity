using System;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    public short storesMultiplier = 5;
    public TextMeshProUGUI food;
    public TextMeshProUGUI prod;
    public TextMeshProUGUI gold;
    public TextMeshProUGUI vp;
    public TextMeshProUGUI influence;

    private Board board;
    private Turn turn;

    private Dictionary<NationsEnum, CityProduction> stores = new Dictionary<NationsEnum, CityProduction>();
    private Dictionary<NationsEnum, CityProduction> productions = new Dictionary<NationsEnum, CityProduction>();
    private Dictionary<NationsEnum, CityProduction> consumptions = new Dictionary<NationsEnum, CityProduction>();
    private Dictionary<NationsEnum, short> influences = new Dictionary<NationsEnum, short>();

    void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
    }

    public void Initialize(NationsEnum nation, CityProduction cityProduction)
    {
        if (!stores.ContainsKey(nation))
        {
            stores[nation] = new CityProduction(0, 0, cityProduction.prod);
            stores[nation] *= storesMultiplier;
        }            
        else
            stores[nation] = stores[nation] + cityProduction;

        RecalculateCityProduction(nation);
        RecalculateNationConsumptions(nation);
        RecalculateInfluences();
    }

    public void RecalculateCityProduction(NationsEnum nation)
    {
        List<CityInPlay> cities = board.GetCityManager().GetCitiesOfPlayer(nation);
        short foodProd = 0;
        short prodProd = 0;
        short goldProd = 0;
        foreach(CityInPlay cityInPlay in cities)
        {
            CityDetails details = cityInPlay.GetDetails();
            foodProd += details.production.food;
            prodProd += details.production.prod;
            goldProd += details.production.gold;
        }
        productions[nation] = new CityProduction(foodProd, goldProd, prodProd);

        RefreshCityProductionStats(nation);
    }

    public void RecalculateNationConsumptions(NationsEnum nation)
    {
        List<CardInPlay> hazardCreatures = board.GetHazardCreaturesManager().GetHazardCreaturesOfPlayer(nation);
        List<CardInPlay> characters = board.GetCharacterManager().GetCharactersOfPlayerNonAvatar(nation);
        short foodConsumption = (short) hazardCreatures.Count;
        short goldConsumption = (short) characters.Count;

        consumptions[nation] = new CityProduction(foodConsumption, goldConsumption, 0);

        RefreshCityProductionStats(nation);
    }

    public void RefreshCityProductionStats(NationsEnum nation)
    {
        if(!productions.ContainsKey(nation))
            RecalculateCityProduction(nation);
        
        short foodProd = productions[nation].food;
        short goldProd = productions[nation].gold;
        short prodProd = productions[nation].prod;

        if (!consumptions.ContainsKey(nation))
            RecalculateNationConsumptions(nation);

        short foodConsumption = consumptions[nation].food;
        short goldConsumption = consumptions[nation].gold;

        //food.text = stores[turn.GetCurrentPlayer()].food.ToString() + (foodProd > 0 ? " (+" + foodProd + ")" : "");
        //gold.text = stores[turn.GetCurrentPlayer()].gold.ToString() + (goldProd > 0 ? " (+" + goldProd + ")" : "");
        //prod.text = stores[turn.GetCurrentPlayer()].prod.ToString() + (prodProd > 0 ? " (+" + prodProd + ")" : "");
        food.text = foodConsumption.ToString() + " / " + foodProd.ToString();
        gold.text = goldConsumption.ToString() + " / " + goldProd.ToString();
        prod.text = prodProd.ToString();
    }

    public void RecalculateInfluences()
    {
        for(int i = 0; i<Enum.GetValues(typeof(NationsEnum)).Length; i++)
        {
            if ((NationsEnum)i == NationsEnum.NONE)
                continue;
            influences[(NationsEnum)i] = GetFreeInfluence((NationsEnum)i, true);
        }
        RefreshInfluence();
    }

    public void RecalculateInfluence(NationsEnum nation)
    {
        short freeInfluence = Nations.INFLUENCE;
        List<CardInPlay> characters = board.GetCharacterManager().GetCharactersOfPlayer(nation);
        foreach (CardInPlay character in characters)
            freeInfluence -= character.GetCharacterDetails().mind;
        
        influences[nation] = freeInfluence;
    }

    public void RefreshInfluence()
    {
        influence.text = influences[turn.GetCurrentPlayer()].ToString();
    }

    public short GetFreeInfluence(NationsEnum nation, bool dirty)
    {
        if (!influences.ContainsKey(nation) || dirty)
            RecalculateInfluence(nation);
        return influences[nation];
    }

    public float GetCharacterSuccessProbability(CardDetails cardDetails)
    {
        if (cardDetails.GetCharacterCardDetails() == null)
            return 0f;

        short influence = GetFreeInfluence(turn.GetCurrentPlayer(), false);

        if (cardDetails.GetCharacterCardDetails().mind > influence)
            return 0f;

        influence -= cardDetails.GetCharacterCardDetails().mind;

        return 1 - (1f*influence / Nations.INFLUENCE);
    }
    
}
