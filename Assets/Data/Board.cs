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
    private MovementManager movementManager;
    private Game game;
    private DeckManager deckManager;

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
        movementManager = GameObject.Find("MovementManager").GetComponent<MovementManager>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
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
        
        if (card.owner == turn.GetCurrentPlayer())
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

        GameObject instantiatedObject = Instantiate(cardObject, cardsCanvasTransform.transform);
        instantiatedObject.name = cardId;
        instantiatedObject.layer = LayerMask.NameToLayer("UI");
        //instantiatedObject.transform.localScale = Vector3.one;
        CardInPlay card = instantiatedObject.GetComponent<CardInPlay>();

        // Change OWNER
        card.owner = turn.GetCurrentPlayer();
        // Change Moved
        card.moved = MovementConstants.characterMovement;
        // Finally this will trigger Initialize in CardInPLay, and then CardUI
        card.cardId = cardId;

        bool success = false;
        
        Vector2Int hex = Vector2Int.one * int.MinValue;
        CityInPlay city;
        switch (spawnCardLocation)
        {
            case SpawnCardLocation.AtHaven:
                city = cityManager.GetHavenOfPlayer(turn.GetCurrentPlayer());
                if(city != null)
                {
                    hex = city.hex;
                    card.Initialize(hex);
                    success = true;
                }
                else
                {
                    Debug.Log("Trying to instantiate at haven but player does not have a haven");
                    success = false;
                }                
                break;
            case SpawnCardLocation.AtHomeTown:
                if (cardDetails.GetCharacterCardDetails() != null)
                {
                    CharacterCardDetails character = cardDetails.GetCharacterCardDetails();
                    success = false;
                    // THIS WILL ALREADY TAKE INTO ACCOUNT "ANY" hometowns
                    foreach(string homeTown in character.homeTown)
                    {
                        city = cityManager.GetCityOfPlayer(turn.GetCurrentPlayer(), homeTown);
                        if (city != null)
                        {
                            hex = city.hex;
                            card.Initialize(hex);
                            success = true;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Card does not have CharacteDetails but it is trying to instantiate at hometown.");
                    success = false;
                }
                    
                break;
            case SpawnCardLocation.AtLastCell:
                if(movementManager.GetLastHex() != MovementManager.NULL2)
                {
                    hex = movementManager.GetLastHex();
                    card.Initialize(hex);
                    success = true;
                }
                else
                {
                    Debug.LogError("Unable to instantiate at last hex.");
                    if (instantiatedObject)
                        DestroyImmediate(instantiatedObject);
                    success = false;
                }
                    
                break;
        }
        if (!success || hex == Vector2.one * int.MinValue)
        {
            DestroyImmediate(instantiatedObject);
            if (tiles[hex].GetCards().Contains(card))
                tiles[hex].RemoveCard(card);
            return false;
        }            

        // HEX
        card.hex = hex;

        Vector3 cellWorldCenter = t.GetCellCenterWorld(new Vector3Int(hex.x, hex.y, 0));

        gameObject.transform.position = cellWorldCenter;


        return true;
    }
}
