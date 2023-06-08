using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[RequireComponent(typeof(CardInPlay))]
public class CardUI : MonoBehaviour
{
    public GameObject displacement;
    public GameObject detailsObject;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI prowess;
    public TextMeshProUGUI defence;
    public TextMeshProUGUI movement;
    public Button button;
    public Image icon;
    public CanvasGroup canvasGroup;
    public CanvasGroup nextCanvasGroup;

    private bool isShownWithCity = false;
    private int cardPositionAtHex = 0;
    private int totalCardsAtHex = 1;
    private List<CardInPlay> allCards;

    public string id = Guid.NewGuid().ToString(); 
    
    private bool isOpen = false;
    
    private Board board;
    private CardInPlay card;

    private bool isMoving = false;
    private bool initialized = false;
    private bool isVisible = true;

    private Tilemap t;
    private SelectedItems selectedItems;
    private Game game;

    private Vector3 currentDisplacementPosition = Vector3.zero;
    private Vector3 currentPosition = Vector3.zero;

    void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        game = GameObject.Find("Game").GetComponent<Game>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        detailsObject.SetActive(false);
        t = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
    }

    public void Initialize()
    {
        card = GetComponent<CardInPlay>();
        button.interactable = card.owner == game.GetHumanPlayer().GetNation();

        if(card == null)
            return;
        if (!card.IsInitialized())
            return;

        cardName.text = card.GetDetails().cardName;
        icon.sprite = card.GetDetails().cardSprite;

        if (card.GetDetails().cardClass == CardClass.Character)
        {
            short prowessValue = card.GetCharacterDetails().prowess;
            short defenceValue = card.GetCharacterDetails().defence;
            short movementLeft = MovementConstants.characterMovement;

            prowess.text = Sprites.prowess + prowessValue.ToString();
            defence.text = Sprites.defence + defenceValue.ToString();
            movement.text = Sprites.movement + movementLeft.ToString();
        }

        RefreshAtHex();
        initialized = true;
        Debug.Log("Initialized " + gameObject.name);
    }

    public void RefreshAtHex()
    {
        Vector3 cellWorldCenter = t.GetCellCenterWorld(new Vector3Int(card.hex.x, card.hex.y, 0));
        if (currentPosition != cellWorldCenter)
        {
            gameObject.transform.position = cellWorldCenter;
            currentPosition = cellWorldCenter;
        }

        isShownWithCity = board.GetTile(card.GetHex()).HasCity() && !isMoving;
        allCards = board.GetTile(card.GetHex()).GetCards();
        totalCardsAtHex = allCards.Count();
        cardPositionAtHex = allCards.IndexOf(card);

        short expectedHorizontalDisplacement = isShownWithCity ? DisplacementPixels.right : DisplacementPixels.NONE;
        Vector3 expectedDisplacementPosition = new Vector3(expectedHorizontalDisplacement, DisplacementPixels.down, 0);
        if (expectedDisplacementPosition != currentDisplacementPosition)
        {
            Debug.Log("Correcting displacement of " + gameObject.name + ". Before: " + currentDisplacementPosition + " after: " + expectedDisplacementPosition);
            displacement.transform.localPosition = expectedDisplacementPosition;
            currentDisplacementPosition = expectedDisplacementPosition;
        }
    }

    void Update()
    {
        if (!initialized)
        {
            Initialize();
            return;
        }

        if (isOpen && selectedItems.GetOpenGUID() != id)
            isOpen = false;

        if (isMoving)
            isOpen = false;

        if (detailsObject.activeSelf != isOpen)
            detailsObject.SetActive(isOpen);

        if (game.GetHumanPlayer().SeesTile(card.hex) != isVisible)
            isVisible = game.GetHumanPlayer().SeesTile(card.hex);

        isVisible = cardPositionAtHex > 0 ? false : isVisible;

        if (isVisible)
        {
            short movementLeft = (short)(MovementConstants.characterMovement - card.moved);
            movement.text = Sprites.movement + movementLeft.ToString();

            nextCanvasGroup.alpha = totalCardsAtHex > 1 ? 1 : 0;
            nextCanvasGroup.interactable = totalCardsAtHex > 1 ? true : false;
            nextCanvasGroup.blocksRaycasts = totalCardsAtHex > 1 ? true : false;
        }
        canvasGroup.alpha = isVisible ? 1 : 0;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;

        RefreshAtHex();
    }

    public void Toggle()
    {
        if(game.GetHumanPlayer().GetNation() != card.owner)
        {
            isOpen = false;
            return;
        }

        isOpen = !isOpen;
        if (isOpen)
        {
            board.SelectHex(card.hex);
            if (card.GetDetails().cardClass == CardClass.Character)
            {
                Debug.Log("Selected " + card.GetDetails().cardName + " (character) at *" + card.hex.x + "," + card.hex.y + "*");
                selectedItems.SelectCardUI(this);
            }                
            else
                selectedItems.UnselectCardDetails();
        }            
        else if (selectedItems.GetOpenGUID() == id)
        {
            selectedItems.SetOpenGUID(string.Empty);
            board.SelectHex(Board.NULL);
            selectedItems.UnselectCardUI();
        }            
    }

    public CardInPlay GetCard()
    {
        return card;
    }

    public void Moving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    public void Next()
    {
        int newPosition = allCards.Count - 1;
        allCards.Remove(card);
        allCards.Insert(newPosition, card);
    }
}
