using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board: MonoBehaviour
{
    public GameObject cardObject;
    public GameObject cityObject;

    public static Vector2Int NULL = Vector2Int.one * int.MinValue;

    private Dictionary<Vector2Int, BoardTile> tiles = new Dictionary<Vector2Int, BoardTile>();
    private Vector2Int selectedHex = NULL;

    private CardManager cardManager;
    private CityManager cityManager;
    private CharacterManager characterManager;
    private HazardCreaturesManager hazardCreaturesManager;
    private ResourcesManager resourcesManager;
    private Turn turn;
    private Tilemap t;
    private FOWManager fowManager;
    private Game game;

    private Transform cardsCanvasTransform;
    private SelectedItems selectedItems;
    bool initialized = false;

    void Awake()
    {
        cardManager = new CardManager(this);
        cityManager = new CityManager(this);
        characterManager = new CharacterManager(this);
        hazardCreaturesManager = new HazardCreaturesManager(this);
        game = GameObject.Find("Game").GetComponent<Game>();
        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        cardsCanvasTransform = GameObject.Find("CardsCanvas").transform;
        t = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        fowManager = GameObject.Find("FOWManager").GetComponent<FOWManager>();
        initialized = true;
    }

    public bool IsInitialized()
    {
        return initialized;
    }

    public void AddCity(Vector2Int hex, CityInPlay city)
    {
        if (!tiles.ContainsKey(hex))
        {
            tiles[hex] = new BoardTile(hex, city);
        } 
        else
        {
            BoardTile bt = tiles[hex];
            bt.AddCity(city);
        }
        resourcesManager.Initialize(city.owner, city.GetDetails().production);
        if(city.owner == game.GetHumanPlayer().GetNation())
            fowManager.UpdateCityFOW(city);
     }
    public void AddCard(Vector2Int hex, CardInPlay card)
    {
        if (!tiles.ContainsKey(hex))
        {
            tiles[hex] = new BoardTile(hex, card);
        }
        else
        {
            BoardTile bt = tiles[hex];
            bt.AddCard(card);
        }
        
        if (card.owner == game.GetHumanPlayer().GetNation())
            fowManager.UpdateCardFOW(card);
    }

    public Dictionary<Vector2Int, BoardTile> GetTiles()
    {
        return tiles;
    }
    
    public BoardTile GetTile(Vector2Int hex)
    {
        if(!tiles.ContainsKey(hex))
            tiles[hex] = new BoardTile(hex);
        return tiles[hex];            
    }

    public void SelectHex(Vector2Int hex)
    {
        selectedHex = hex;
    }

    public Vector2Int GetSelectedHex()
    {
        return selectedHex;
    }

    public bool IsHexSelected()
    {
        return selectedHex != NULL;
    }

    public CityManager GetCityManager()
    {
        return cityManager;
    }

    public CharacterManager GetCharacterManager()
    {
        return characterManager;
    }

    public HazardCreaturesManager GetHazardCreaturesManager()
    {
        return hazardCreaturesManager;
    }
    public CardManager GetCardManager()
    {
        return cardManager;
    }
    public bool CreateCardUI(CardDetails cardDetails, SpawnCardLocation spawnCardLocation)
    {
        string cardId = cardDetails.cardId;

        GameObject instantiatedObject = Instantiate(cardObject);
        instantiatedObject.name = cardId;
        instantiatedObject.transform.SetParent(cardsCanvasTransform);
        instantiatedObject.layer = LayerMask.NameToLayer("UI");
        instantiatedObject.transform.localScale = Vector3.one;

        bool success = false;

        Vector2Int hex = Vector2Int.one * int.MinValue;
        CityInPlay city;
        switch (spawnCardLocation)
        {
            case SpawnCardLocation.AtHaven:
                city = cityManager.GetHavenOfPlayer(turn.GetCurrentPlayer());
                hex = city.hex;
                success = true;
                break;
            case SpawnCardLocation.AtHomeTown:
                city = cityManager.GetCityOfPlayer(turn.GetCurrentPlayer(), cardDetails.GetCharacterCardDetails().homeTown);
                hex = city.hex;
                success = true;
                break;
            case SpawnCardLocation.AtLastCell:
                hex = selectedItems.GetSelectedCharacter().GetCard().hex;
                success = true;
                break;
        }
        if (!success || hex == Vector2.one * int.MinValue)
            return false;

        Vector3 cellWorldCenter = t.GetCellCenterWorld(new Vector3Int(hex.x, hex.y, 0));

        gameObject.transform.position = cellWorldCenter;

        CardInPlay cardInPlay = instantiatedObject.GetComponent<CardInPlay>();
        // Change OWNER
        cardInPlay.owner = turn.GetCurrentPlayer();
        // Change Moved
        cardInPlay.moved = 0;
        // HEX
        cardInPlay.hex = hex;
        // Finally this will trigger Initialize in CardInPLay, and then CardUI
        cardInPlay.cardId = cardId;

        return true;
    }
}
