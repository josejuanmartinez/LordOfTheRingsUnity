using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class DeckManager : MonoBehaviour
{
    public GameObject hand;
    public Image placeCard;

    public short lastCardDrawn = -1;
        
    public short cardShown = -1;

    public bool isInitialized = false;

    private List<GameObject> cardsObjects;

    public List<CardDetails> discardPile;

    private ManaPool pool;
    private Board board;
    private PlaceDeck placeDeck;
    private Turn turn;
    private ResourcesManager resourcesManager;
    private MovementManager movementManager;
    private CardDetailsRepo cardRepo;  
    private Game game;
    private SelectedItems selectedItems;
    private Tilemap tilemap;
    private TerrainManager terrainManager;

    void Awake()
    {
        pool = GameObject.Find("ManaPool").GetComponent<ManaPool>();
        board = GameObject.Find("Board").GetComponent<Board>();
        placeDeck = GameObject.Find("PlaceDeck").GetComponent<PlaceDeck>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        movementManager = GameObject.Find("MovementManager").GetComponent<MovementManager>();
        cardRepo = GameObject.Find("CardDetailsRepo").GetComponent<CardDetailsRepo>();
        game = GameObject.Find("Game").GetComponent<Game>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        tilemap = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
        terrainManager = GameObject.Find("TerrainManager").GetComponent<TerrainManager>();
    }

    void Initialize()
    {
        if (!cardRepo.IsInitialized())
            return;
        List<string> cardNames = cardRepo.GetCardNamesOfNation(turn.GetCurrentPlayer());
        cardsObjects = cardNames.Select(x => cardRepo.GetCardDetails(x)).ToList();
        Debug.Log("Loaded " + cardsObjects.Count + " cards for " + turn.GetCurrentPlayer().ToString());
        Shuffle();
        DrawHand();
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized)
        {
            Initialize();
            return;
        }
        if(cardShown != -1)
            placeDeck.SetHoverCard(GetHandCard(cardShown).GetComponent<CardDetails>());
    }

    public int GetHandSize()
    {
        List<CardInPlay> charsWithExtraAtHome = board.GetCharacterManager().GetCharactersOfPlayer(turn.GetCurrentPlayer()).FindAll(x => x.GetCharacterDetails().abilities.Contains(CharacterAbilitiesEnum.OneAdditionalCardAtHome)).ToList();
        int counter = 0;
        foreach(CardInPlay c in charsWithExtraAtHome)
        {
            if(c.GetCharacterDetails() != null)
            {
                CharacterCardDetails cardDetails = c.GetCharacterDetails();
                foreach(string ht in cardDetails.homeTown)
                {
                    CityInPlay city = board.GetCityManager().GetCityOfPlayer(c.owner, ht);
                    if(city != null)
                    {
                        if (city.hex == c.hex)
                        {
                            counter++;
                        }
                    }
                }
            }
            
        }
        return CardsConstants.minHand + counter;
    }

    private GameObject GetHandCard(int cardShown)
    {
        if (hand.transform.childCount <= cardShown || cardShown < 0)
            return null;

        return hand.transform.GetChild(hand.transform.childCount - 1 - cardShown).gameObject;
    }

    private void Shuffle()
    {
        for (int i = 0; i < cardsObjects.Count; i++)
        {
            int rnd = Random.Range(0, cardsObjects.Count);
            GameObject tempGO = cardsObjects[rnd];
            cardsObjects[rnd] = cardsObjects[i];
            cardsObjects[i] = tempGO;
        }
    }

    public void DrawHand()
    {
        for(int i= 0; i < GetHandSize(); i++)
            Draw();
    }

    public void Draw()
    {
        for (int i = GetHandSize() - 1; i > 0; i--)
            if (GetHandCard(i - 1) != null)
                GetHandCard(i - 1).GetComponent<DeckCardUI>().increaseHandPosition();

        // This is the counter of cards drawn from the Deck (all cards)
        lastCardDrawn++;

        CreateCard(cardsObjects[lastCardDrawn], hand.transform);
        
    }

    public void CreateCard(GameObject card, Transform parentTransform)
    {
        Assert.IsTrue(card.GetComponent<CardDetails>() != null);

        GameObject instantiatedObject = Instantiate(card);
        instantiatedObject.transform.SetParent(parentTransform);
        instantiatedObject.layer = LayerMask.NameToLayer("ScreenOverlayUILayer");
        instantiatedObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        Image image = instantiatedObject.AddComponent<Image>();
        //image.preserveAspect = true;
        image.sprite = instantiatedObject.GetComponent<CardDetails>().cardSprite;
        Button button = instantiatedObject.AddComponent<Button>();
        button.targetGraphic = image;
        instantiatedObject.AddComponent<DeckCardUI>();
    }

    public void DiscardAndDraw(CardDetails card)
    {
        int index = -1;
        short handPos = -1;
        for(int i = 0; i < GetHandSize(); i++)
        {
            if (GetHandCard(i) == null)
                continue;
            string cardId = GetHandCard(i).GetComponent<CardDetails>().cardId;
            if (cardId == card.cardId)
            {
                index = i;
                handPos = GetHandCard(i).GetComponent<DeckCardUI>().handPos;
                break;
            }                    
        }
        if(index == -1 || handPos == -1)
        {
            Debug.LogError("Unable to discard card " + card.cardId);
            Hide(cardShown);
            return;
        }
        
        // Destroy the card from hand
        Destroy(GetHandCard(index).gameObject);

        // Add it to discard pile
        discardPile.Add(card);

        //For all the cards before, I increase the counter
        for (int i = handPos - 1; i >= 0; i--)
            GetHandCard(i).GetComponent<DeckCardUI>().increaseHandPosition();
        
        // This is the counter of cards drawn from the Deck (all cards)
        lastCardDrawn++;

        CreateCard(cardsObjects[lastCardDrawn], hand.transform);
        Hide(cardShown);
    }

    public void Show(short card)
    {
        cardShown = card;
    }

    public void Hide(short card)
    {
        if (card == -1)
        {
            cardShown = -1;
            return;
        }
         
        if (cardShown == card)
        {
            placeDeck.RemoveHoverCard(GetHandCard(cardShown).GetComponent<CardDetails>());
            cardShown = -1;
        }
            
    }

    public bool IsHazardCreaturePlayable(CardDetails creatureCardDetails, NationsEnum owner)
    {
        HazardCreatureCardDetails hazard = creatureCardDetails.gameObject.GetComponent<HazardCreatureCardDetails>();
        if (hazard == null)
            return false;
        return pool.HasEnoughMana(hazard.cardTypes) && (CanSpawnAtHaven(owner) || CanSpawnAtLastHex());
    }

    public bool IsCharacterCardPlayable(CardDetails cardDetails, NationsEnum owner)
    {
        CharacterCardDetails character = cardDetails.GetCharacterCardDetails();
        
        if (character == null)
            return false;
        
        if (resourcesManager.GetFreeInfluence(turn.GetCurrentPlayer(), false) < character.mind)
            return false;

        if (cardDetails.IsInPlay())
            return false;

        
        if (CanSpawnAtHomeTown(cardDetails, owner))
            return true;
        if (CanSpawnAtHaven(owner))
            return true;
        
        return false;
    }
    public bool IsGoldRingCardPlayable(CardDetails cardDetails, NationsEnum owner)
    {
        return false;
    }

    public bool IsUndiscoveredRingCardPlayable(ObjectCardDetails cardDetails, NationsEnum owner)
    {
        return false;
    }

    public bool IsObjectCardPlayable(CardDetails cardDetails, NationsEnum owner)
    {
        ObjectCardDetails details = cardDetails.GetObjectCardDetails();
        if (details == null)
            return false;
        
        if (details.IsUndiscoveredRing())
            return IsUndiscoveredRingCardPlayable(details, owner);

        CardDetails charDetails = selectedItems.GetLastSelectedChar();
        if (charDetails == null)
            return false;
        if (charDetails.GetCardInPlay().IsMoving())
            return false;

        CityInPlay city = board.GetCityManager().GetCityAtHex(charDetails.GetCardInPlay().GetHex());
        if (city == null)
            return false;
        //List<CityInPlay> cities = board.GetCityManager().GetCitiesWithCharactersOfPlayer(owner);
        //foreach(CityInPlay city in cities)
        //{
        CityDetails cityDetails = city.GetDetails();
        if(cityDetails != null)
        {
            if (cityDetails.IsTapped(owner))
                return false;
            AlignmentsEnum alignment = Nations.alignments[owner];
            switch(alignment)
            {
                case AlignmentsEnum.DARK_SERVANTS:
                case AlignmentsEnum.CHAOTIC:
                    foreach(ObjectTypes obj in cityDetails.objectTypesDark)
                        if (obj == details.itemType) 
                            return true;
                    break;
                case AlignmentsEnum.FREE_PEOPLE:
                case AlignmentsEnum.NEUTRAL:
                    foreach (ObjectTypes obj in cityDetails.objectTypesFree)
                        if (obj == details.itemType)
                            return true;
                    break;
                case AlignmentsEnum.RENEGADE:
                    if(cityDetails.objectTypesRenegade.Count() > 0 || cityDetails.renegadeSprite != null)
                    {
                        foreach (ObjectTypes obj in cityDetails.objectTypesRenegade)
                            if (obj == details.itemType)
                                return true;
                    } 
                    else
                    {
                        foreach (ObjectTypes obj in cityDetails.objectTypesFree)
                            if (obj == details.itemType)
                                return true;
                    }
                    break;
            }
        }
        //}
        return false;
    }

    public bool CanSpawnAtHomeTown(CardDetails cardDetails, NationsEnum owner)
    {
        CharacterCardDetails character = cardDetails.GetCharacterCardDetails();

        if (character  == null) 
            return false;

        foreach(string homeTown in character.homeTown)
            if (board.GetCityManager().GetCityOfPlayer(owner, homeTown) != null)
                return true;

        /*if (board.GetCharacterManager().GetCityStringsWithCharactersOfPlayer(owner).Contains(character.GetCharacterCardDetails().homeTown))
            return true;*/
        return false;
    }

    public bool CanSpawnAtHaven(NationsEnum owner)
    {
        if (board.GetCityManager().GetHavenOfPlayer(owner) != null)
            return true;
        return false;
    }

    public bool CanSpawnAtLastHex()
    {
        CardDetails characterDetails = selectedItems.GetLastSelectedChar();
        if (characterDetails == null)
            return false;

        HazardCreatureCardDetails hazardDetails = selectedItems.GetHazardCreatureCardDetails();
        if (hazardDetails == null)
            return false;

        CardTypesEnum cardType = terrainManager.GetTileAndMovementCost(characterDetails.GetCardInPlay().hex).cardInfo.cardType;
        return hazardDetails.cardTypes.Contains(cardType);
    }
}
