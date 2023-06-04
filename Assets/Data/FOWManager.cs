using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FOWManager : MonoBehaviour
{
    public short visionLevels = 2;
    Tilemap fow;
    Board board;
    Game game;

    bool isInitialized = false;
    void Initialize()
    {
        fow = GameObject.Find("FOWTilemap").GetComponent<Tilemap>();
        board = GameObject.Find("Board").GetComponent<Board>();
        game = GameObject.Find("Game").GetComponent<Game>();
        isInitialized = true;
    }

    // Update is called once per frame
    public void UpdateCitiesFOW()
    {
        if(!isInitialized)
            Initialize();
        if (!isInitialized)
            return;

        List <CityInPlay> cities = board.GetCityManager().GetCitiesOfPlayer(game.GetHumanPlayer().GetNation()); 
        foreach (CityInPlay city in cities)
        {
            Vector3Int cityHex = new Vector3Int(city.hex.x, city.hex.y, 0);
            List<Vector3Int> hexesToClean = new List<Vector3Int>() { cityHex };
            for(int i=0; i<visionLevels; i++) {
                List<Vector3Int> moreHexesToClean = new List<Vector3Int>();
                foreach (Vector3Int hex in hexesToClean)
                {
                    List<Vector3Int> v3Surroundings = HexTranslator.GetSurroundings(hex);
                    moreHexesToClean.AddRange(v3Surroundings);
                }
                hexesToClean = moreHexesToClean.Distinct().ToList();
            }
            foreach (Vector3Int surrounding in hexesToClean.Distinct())
            {
                fow.SetTile(surrounding, null);
                fow.RefreshTile(surrounding);
            }
        }
    }

    public void UpdateCardFOW(Vector3Int hex)
    {
        if (!isInitialized)
            Initialize();
        if (!isInitialized)
            return;

        List<Vector3Int> v3Surroundings = HexTranslator.GetSurroundings(hex);
        foreach (Vector3Int surrounding in v3Surroundings.Distinct())
        {
            fow.SetTile(surrounding, null);
            fow.RefreshTile(surrounding);
        }
    }
}
