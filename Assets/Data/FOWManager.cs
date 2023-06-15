using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FOWManager : MonoBehaviour
{
    public short cityVisionLevel = 5;
    public short cardVisionLevel = 3;
    public Tile fowTile;
    
    Tilemap fow;
    Board board;
    Game game;
    Turn turn;

    bool isInitialized = false;

    void Initialize()
    {
        fow = GameObject.Find("FOWTilemap").GetComponent<Tilemap>();
        board = GameObject.Find("Board").GetComponent<Board>();
        game = GameObject.Find("Game").GetComponent<Game>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        isInitialized = true;
    }

    // Update is called once per frame
    public void UpdateCitiesFOW()
    {
        if(!isInitialized)
            Initialize();
        if (!isInitialized)
            return;

        List <CityInPlay> cities = board.GetCityManager().GetCitiesOfPlayer(turn.GetCurrentPlayer()); 
        foreach (CityInPlay city in cities)
        {
            UpdateCityFOW(city);
        }
    }

    public void UpdateCityFOW(CityInPlay city)
    {
        if (!isInitialized)
            Initialize();
        if (!isInitialized)
            return;

        Vector3Int cityHex = new Vector3Int(city.hex.x, city.hex.y, 0);
        HashSet<Vector3Int> hexesToClean = new HashSet<Vector3Int>() { cityHex };
        for (int i = 0; i < cityVisionLevel; i++)
        {
            HashSet<Vector3Int> moreHexesToClean = new HashSet<Vector3Int>();
            foreach (Vector3Int hex in hexesToClean)
            {
                List<Vector3Int> v3Surroundings = HexTranslator.GetSurroundings(hex);
                moreHexesToClean.UnionWith(v3Surroundings);
            }
            hexesToClean = moreHexesToClean;
        }
        foreach (Vector3Int surrounding in hexesToClean)
        {
            fow.SetTile(surrounding, null);
            fow.RefreshTile(surrounding);
            game.GetHumanPlayer().SetCitySeesTile(new Vector2Int(surrounding.x, surrounding.y));
        }
    }

    public void UpdateCardsFOW()
    {
        if (!isInitialized)
            Initialize();
        if (!isInitialized)
            return;

        List<CardInPlay> cards = board.GetCardManager().GetCardsOfPlayer(turn.GetCurrentPlayer());
        foreach (CardInPlay card in cards)
        {
            UpdateCardFOW(card);
        }
    }

    public void UpdateCardFOW(CardInPlay card)
    {
        if (!isInitialized)
            Initialize();
        if (!isInitialized)
            return;

        Vector3Int cardHex = new Vector3Int(card.hex.x, card.hex.y, 0);
        HashSet<Vector3Int> hexesToClean = new HashSet<Vector3Int>() { cardHex };
        for (int i = 0; i < cardVisionLevel; i++)
        {
            HashSet<Vector3Int> moreHexesToClean = new HashSet<Vector3Int>();
            foreach (Vector3Int hex in hexesToClean)
            {
                List<Vector3Int> v3Surroundings = HexTranslator.GetSurroundings(hex);
                moreHexesToClean.UnionWith(v3Surroundings);
            }
            hexesToClean.UnionWith(moreHexesToClean);
        }
        foreach (Vector3Int surrounding in hexesToClean)
        {
            fow.SetTile(surrounding, null);
            fow.RefreshTile(surrounding);
            game.GetHumanPlayer().SetCardSeesTile(new Vector2Int(surrounding.x, surrounding.y));
        }
    }

    public void UpdateCardFOW(Vector3Int newHex, Vector3Int oldHex)
    {
        if (!isInitialized)
            Initialize();
        if (!isInitialized)
            return;

        Vector3Int cardHex = new Vector3Int(oldHex.x, oldHex.y, 0);
        HashSet<Vector3Int> hexesToClean = new HashSet<Vector3Int>() { cardHex };
        for (int i = 0; i < cardVisionLevel; i++)
        {
            HashSet<Vector3Int> moreHexesToClean = new HashSet<Vector3Int>();
            foreach (Vector3Int hex in hexesToClean)
            {
                List<Vector3Int> v3Surroundings = HexTranslator.GetSurroundings(hex);
                moreHexesToClean.UnionWith(v3Surroundings);
            }
            hexesToClean.UnionWith(moreHexesToClean);
        }
        foreach (Vector3Int surrounding in hexesToClean)
        {
            if(!game.GetHumanPlayer().CitySeesTile(new Vector2Int(surrounding.x, surrounding.y))) {
                fow.SetTile(surrounding, fowTile);
                fow.RefreshTile(surrounding);
            }
            game.GetHumanPlayer().UnsetCardSeesTile(new Vector2Int(surrounding.x, surrounding.y));
        }

        cardHex = new Vector3Int(newHex.x, newHex.y, 0);
        hexesToClean = new HashSet<Vector3Int>() { cardHex };
        for (int i = 0; i < cardVisionLevel; i++)
        {
            HashSet<Vector3Int> moreHexesToClean = new HashSet<Vector3Int>();
            foreach (Vector3Int hex in hexesToClean)
            {
                List<Vector3Int> v3Surroundings = HexTranslator.GetSurroundings(hex);
                moreHexesToClean.UnionWith(v3Surroundings);
            }
            hexesToClean.UnionWith(moreHexesToClean);
        }
        foreach (Vector3Int surrounding in hexesToClean)
        {
            fow.SetTile(surrounding, null);
            fow.RefreshTile(surrounding);
            game.GetHumanPlayer().SetCardSeesTile(new Vector2Int(surrounding.x, surrounding.y));
        }
    }
}
