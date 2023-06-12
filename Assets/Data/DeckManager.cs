using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
public class DeckManager : MonoBehaviour
{
    public GameObject hand;
    public Image placeCard;

    public short lastCardDrawn = -1;
        
    public short handSize = 6;
    public GameObject[] handCards;
    

    public short cardsInHand = 0;

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
    }

    void Initialize()
    {
        if (!cardRepo.IsInitialized())
            return;
        List<string> cardNames = cardRepo.GetCardNamesOfNation(game.GetHumanPlayer().GetNation());
        cardsObjects = cardNames.Select(x => cardRepo.GetCardDetails(x)).ToList();
        Debug.Log("Loaded " + cardsObjects.Count + " cards for " + game.GetHumanPlayer().GetNation().ToString());
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
            placeDeck.SetHoverCard(handCards[cardShown].transform.GetChild(0).GetComponent<CardDetails>());
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
        for(int i= 0; i < handSize; i++)
            Draw();
    }

    public void Draw()
    {
        if (cardsInHand == handSize)
        {
            Debug.LogError("THe player is trying to draw more cards than available in hand!");
            return;
        }

        for (int i = handSize-1; i > 0; i--)
        {
            if (handCards[i-1].transform.childCount > 0)
            {
                GameObject cardToMove = handCards[i - 1].transform.GetChild(0).gameObject;
                cardToMove.GetComponent<DeckCardUI>().increaseHandPosition();
                cardToMove.transform.SetParent(handCards[i].transform, false);
            }   
        }

        if (handCards[0].transform.childCount > 0)
            Destroy(handCards[0].transform.GetChild(0).gameObject);

        // This is the counter of cards drawn from the Deck (all cards)
        lastCardDrawn++;

        CreateCard(cardsObjects[lastCardDrawn], handCards[0].transform);
        
        //This is the counter of cards I have in my hand
        cardsInHand++;
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
        for(int i = 0; i < handCards.Length; i++)
        {
            if (handCards[i].transform.childCount > 0)
            {
                string cardId = handCards[i].transform.GetChild(0).gameObject.GetComponent<CardDetails>().cardId;
                if (cardId == card.cardId)
                {
                    index = i;
                    break;
                }                    
            }
        }
        if(index == -1)
        {
            Debug.Log("Unable to discard card " + card.cardId);
            return;
        }
        if (handCards[index].transform.childCount > 0)
            Destroy(handCards[index].transform.GetChild(0).gameObject);

        discardPile.Add(card);

        for (int i = index - 1; i > 0; i--)
        {
            if (handCards[i - 1].transform.childCount > 0)
            {
                GameObject cardToMove = handCards[i - 1].transform.GetChild(0).gameObject;
                cardToMove.GetComponent<DeckCardUI>().increaseHandPosition();
                cardToMove.transform.SetParent(handCards[i].transform, false);
            }
        }
        // This is the counter of cards drawn from the Deck (all cards)
        lastCardDrawn++;

        CreateCard(cardsObjects[lastCardDrawn], handCards[0].transform);
    }

    public void Show(short card)
    {
        cardShown = card;
    }

    public void Hide(short card)
    {
        if (cardShown == card)
        {
            placeDeck.RemoveHoverCard(handCards[cardShown].transform.GetChild(0).GetComponent<CardDetails>());
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
        
        if (resourcesManager.GetFreeInfluence(turn.GetCurrentPlayer()) < character.mind)
            return false;

        if (cardDetails.IsInPlay())
            return false;

        
        if (CanSpawnAtHomeTown(cardDetails, owner))
            return true;
        if (CanSpawnAtHaven(owner))
            return true;
        
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
        return movementManager.GetLastHex() != MovementManager.NULL2;
    }
}
