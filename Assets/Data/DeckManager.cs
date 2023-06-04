using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
public class DeckManager : MonoBehaviour
{
    public GameObject hand;
    public Image placeCard;

    public GameObject[] cardsObjects;
    public short lastCardDrawn = -1;
        
    public short handSize = 6;
    public GameObject[] handCards;

    public short cardsInHand = 0;

    public short cardShown = -1;

    public bool isShuffled = false;

    private ManaPool pool;
    private Board board;
    private PlaceDeck placeDeck;
    void Awake()
    {
        Shuffle();
        DrawHand();
        isShuffled = true;

        pool = GameObject.Find("ManaPool").GetComponent<ManaPool>();
        board = GameObject.Find("Board").GetComponent<Board>();
        placeDeck = GameObject.Find("PlaceDeck").GetComponent<PlaceDeck>();
    }

    void Update()
    {
        if(cardShown != -1)
        {
            placeDeck.SetHoverCard(handCards[cardShown].transform.GetChild(0).GetComponent<CardDetails>());
        } 
    }

    private void Shuffle()
    {
        for (int i = 0; i < cardsObjects.Length; i++)
        {
            int rnd = Random.Range(0, cardsObjects.Length);
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

    public bool IsHazardCreaturePlayable(HazardCreatureCardDetails creatureCardDetails)
    {
        return pool.HasEnoughMana(creatureCardDetails.cardTypes);
    }

    public bool IsCharacterCardPlayable(CharacterCardDetails character, NationsEnum owner)
    {
        if (character.GetCardDetails().IsInPlay())
            return false;

        if (character.homeTown == CitiesStringConstants.ANY)
            return true;

        switch (Nations.alignments[owner])
        {
            case AlignmentsEnum.DARK_SERVANTS:
                if (character.homeTown == CitiesStringConstants.ANY_DARK)
                    return true;
                break;
            case AlignmentsEnum.FREE_PEOPLE:
                if (character.homeTown == CitiesStringConstants.ANY_FREE)
                    return true;
                break;
            case AlignmentsEnum.NEUTRAL:
                if (character.homeTown == CitiesStringConstants.ANY_NEUTRAL)
                    return true;
                break;
            case AlignmentsEnum.RENEGADE:
                if (character.homeTown == CitiesStringConstants.ANY_RENEGADE)
                    return true;
                break;
        }
        if (board.GetCityManager().GetCitiesStringsOfPlayer(owner).Contains(character.homeTown))
            return true;
        if (board.GetCharacterManager().GetCityStringsWithCharactersOfPlayer(owner).Contains(character.homeTown))
            return true;
        return false;
    }
}
