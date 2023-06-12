using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CardInPlay : MonoBehaviour
{
    public string cardId;
    public NationsEnum owner;

    public short moved = 0;
    public Vector2Int hex;

    CardDetails details;

    Board board;

    bool initialized = false;

    private CardDetailsRepo cardDetailsRepo;
    private ResourcesManager resourcesManager;
    private PlaceDeck placeDeck;

    private CardUI cardUI;
    private string inCompanyOf;

    void Awake()
    {
        cardDetailsRepo = GameObject.Find("CardDetailsRepo").GetComponent<CardDetailsRepo>();
        board = GameObject.Find("Board").GetComponent<Board>();
        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        placeDeck = GameObject.Find("PlaceDeck").GetComponent<PlaceDeck>();
        cardUI = GetComponent<CardUI>();
    }

    void Initialize()
    {
        if (board.IsInitialized() && cardDetailsRepo.IsInitialized() && !string.IsNullOrEmpty(cardId))
        {
            board.AddCard(hex, this);

            GameObject cardObject = Instantiate(cardDetailsRepo.GetCardDetails(cardId));
            cardObject.name = cardId + "_details";
            cardObject.transform.SetParent(transform);            
            details = cardObject.GetComponent<CardDetails>();
            details.Play();

            board = GameObject.Find("Board").GetComponent<Board>();
            
            initialized = true;
            
            if (IsCharacter())
                resourcesManager.RecalculateInfluence(owner);

            Debug.Log(details.cardName + " registered itself in Board at " + HexTranslator.GetDebugTileInfo(hex) + " " + HexTranslator.GetNormalizedCellPosString(hex));
        }
    }

    void Update()
    {
        if (!initialized)
            Initialize();
    }

    public bool IsInitialized()
    {
        return initialized;
    }

    public Vector2Int GetHex()
    {
        return hex;
    }

    public void SetHoverCard()
    {
        placeDeck.SetHoverCard(details);
    }
    public void RemoveHoverCard()
    {
        placeDeck.RemoveHoverCard(details);
    }

    public void AddToHex(Vector2Int newHex)
    {
        board.GetTile(hex).RemoveCard(this);
        hex = newHex;
        board.GetTile(newHex).AddCard(this);
    }

    public CardDetails GetDetails()
    {
        if (!initialized)
            Debug.LogError("Calling to `GetDetails` but CardInPlay still not initialized!");
        return details;
    }
    public CharacterCardDetails GetCharacterDetails()
    {
        if (!initialized)
            Debug.LogError("Calling to `GetCharacterDetails` but CardInPlay still not initialized!");
        if (GetComponentInChildren<CharacterCardDetails>() != null)
            return GetComponentInChildren<CharacterCardDetails>();
        else
            return null;
    }

    public CardClass GetCardClass()
    {
        if(!initialized)
            Debug.LogError("Calling to `GetCardClass` but CardInPlay still not initialized!");
        if(!details)
            Debug.LogError("Calling to `GetCardClass` but details is null");
        return details.cardClass;
    
    }

    public bool IsCharacter()
    {
        if (!initialized)
            Debug.LogError("Calling to `IsCharacter` but CardInPlay still not initialized!");
        return GetCardClass() == CardClass.Character;
    }

    public bool IsAvatar()
    {
        if (!initialized)
            Debug.LogError("Calling to `IsAvatar` but CardInPlay still not initialized!");
        if (IsCharacter() && GetComponentInChildren<CharacterCardDetails>() != null)
            return GetComponentInChildren<CharacterCardDetails>().isAvatar;
        else
            return false;
    }

    public void AddMovement(short movement)
    {
        moved += movement;
    }

    public string GetCompanyLeader()
    {
        return inCompanyOf;
    }
    
    public void SetCompanyLeader(string companyLeader)
    {
        inCompanyOf = companyLeader;
    }

    public void RemoveCompanyLeader()
    {
        inCompanyOf = null;
    }

    public bool IsInCompany()
    {
        return inCompanyOf != null;
    }

    public CardUI GetCardUI()
    {
        return cardUI;
    }

    public void SetHex(Vector2Int hex)
    {
        this.hex = hex;
        List<CardInPlay> company = board.GetCharacterManager().GetCharactersInCompanyOf(details);
        company.ForEach(x => x.hex = hex);
    }
}
