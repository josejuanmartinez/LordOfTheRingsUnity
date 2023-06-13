using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(CityUI))]
public class CityInPlay : MonoBehaviour
{
    public string cityId;

    public NationsEnum owner;
    public Vector2Int hex;

    Board board;
    CityDetails details;

    bool initialized = false;

    private CardDetailsRepo cardDetailsRepo;
    private ResourcesManager resourcesManager;
    private PlaceDeck placeDeck;
    private CityUI cityUI;
    private Game game;

    void Awake()
    {
        cardDetailsRepo = GameObject.Find("CardDetailsRepo").GetComponent<CardDetailsRepo>();
        board = GameObject.Find("Board").GetComponent<Board>();
        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        placeDeck = GameObject.Find("PlaceDeck").GetComponent<PlaceDeck>();
        cityUI = GetComponent<CityUI>();
        game = GameObject.Find("Game").GetComponent<Game>();
    }
    void Initialize()
    {
        if (board.IsInitialized() && cardDetailsRepo.IsInitialized() && !string.IsNullOrEmpty(cityId))
        {
            GameObject cityObject = Instantiate(cardDetailsRepo.GetCityDetails(cityId));
            cityObject.name = cityId + "_details";
            cityObject.transform.SetParent(transform);
            details = cityObject.GetComponent<CityDetails>();
            details.Initialize();

            board.AddCity(hex, this);

            initialized = true;

            resourcesManager.Initialize(owner, details.GetCityProduction());

            //Debug.Log(details.name + " registered itself in Board at " + HexTranslator.GetDebugTileInfo(hex) + " " + HexTranslator.GetNormalizedCellPosString(hex));
        }
    }

    void Update()
    {
        if (!initialized)
            Initialize();
    }

    public void SetHoverCity()
    {
        placeDeck.SetHoverCity(details);
    }
    public void RemoveHoverCity()
    {
        placeDeck.RemoveHoverCity(details);
    }

    public CityInPlay(CityDetails details, NationsEnum owner, Vector2Int hex)
    {
        this.details = details;
        this.owner = owner;
        this.hex = hex;
    }

    public CityDetails GetDetails()
    {
        return details;
    }

    public Vector2Int GetHex()
    {
        return hex;
    }

    public bool IsInitialized()
    {
        return initialized;
    }

    public void Tap(NationsEnum nation)
    {
        details.Tap(nation);
        if(nation == game.GetHumanPlayer().GetNation())
            cityUI.Tap();
    }
}
