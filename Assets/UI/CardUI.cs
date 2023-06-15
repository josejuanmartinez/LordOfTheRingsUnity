using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[RequireComponent(typeof(CardInPlay))]
public class CardUI : MonoBehaviour
{
    public GameObject displacement;
    public TextMeshProUGUI details;
    public TextMeshProUGUI cardName;

    public Sprite freeSprite;
    public Sprite darkSprite;
    public Sprite renegadeSprite;
    public Sprite neutralSprite;
    public Sprite chaosSprite;

    public Button button;
    public Image icon;
    public Image frame;
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
    private MovementManager moveOnTilemap;
    private ColorManager colorManager;
    private PopupManager popupManager;

    private bool isMoving = false;
    private bool initialized = false;
    private bool isVisible = true;

    private Tilemap t;
    private SelectedItems selectedItems;
    private Game game;
    private Turn turn;

    private Vector3 currentDisplacementPosition = Vector3.zero;
    private Vector3 currentPosition = Vector3.zero;
    
    private CardInPlay potentialLeader;
    private HashSet<CardInPlay> rejectedMerges = new HashSet<CardInPlay>();

    void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        game = GameObject.Find("Game").GetComponent<Game>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        //detailsObject.SetActive(false);
        t = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
        moveOnTilemap = GameObject.Find("MovementManager").GetComponent<MovementManager>();
        colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();
        popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();

        details.text = "";
    }

    public void Initialize()
    {
        if (!game.IsInitialized() || !turn.IsInitialized())
            return;

        card = GetComponent<CardInPlay>();

        if(card == null)
            return;
        if (!card.IsInitialized())
            return;

        button.interactable = card.owner == turn.GetCurrentPlayer();

        cardName.text = card.GetDetails().cardName;
        icon.sprite = card.GetDetails().cardSprite;
        frame.color = colorManager.GetColor(card.owner);

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
        totalCardsAtHex = 0;
        foreach(CardInPlay c in allCards)
            if(c.GetCompanyLeader() == null)
                totalCardsAtHex++;
        
        //totalCardsAtHex = allCards.Select(x => x.GetCompanyLeader() == null).Count();
        cardPositionAtHex = allCards.IndexOf(card);

        short expectedHorizontalDisplacement = isShownWithCity ? DisplacementPixels.right : DisplacementPixels.NONE;
        Vector3 expectedDisplacementPosition = new Vector3(expectedHorizontalDisplacement, DisplacementPixels.down, 0);
        if (expectedDisplacementPosition != currentDisplacementPosition)
        {
            //Debug.Log("Correcting displacement of " + gameObject.name + ". Before: " + currentDisplacementPosition + " after: " + expectedDisplacementPosition);
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

        if (Input.GetKeyUp(KeyCode.Escape))
            isOpen = false;

        if (isOpen && selectedItems.GetOpenGUID() != id)
            isOpen = false;

        if (isMoving)
            isOpen = false;

        RecalculateIsVisible();

        if(CanJoin())
        {
            CardInPlay charWithInflunce = GetMergeCandidate();
            if (charWithInflunce != null)
                AskToJoinCompanies(card, charWithInflunce);
        }

        RefreshAtHex();
    }

    public void RecalculateIsVisible()
    {
        if (game.GetHumanPlayer().SeesTile(card.hex) != isVisible)
            isVisible = game.GetHumanPlayer().SeesTile(card.hex);

        isVisible = cardPositionAtHex > 0 ? false : isVisible;
        isVisible = card.GetCompanyLeader() != null ? false : isVisible;

        if (isVisible)
        {
            nextCanvasGroup.alpha = totalCardsAtHex > 1 ? 1 : 0;
            nextCanvasGroup.interactable = totalCardsAtHex > 1 ? true : false;
            nextCanvasGroup.blocksRaycasts = totalCardsAtHex > 1 ? true : false;
        }
        canvasGroup.alpha = isVisible ? 1 : 0;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }

    public bool CanJoin()
    {
        return
            selectedItems.IsCharSelected() &&
            selectedItems.GetSelectedCardUI() == this &&
            !GetCard().IsAvatar() &&
            totalCardsAtHex > 0 &&
            !popupManager.IsShown() &&
            isOpen &&
            isVisible &&
            !GetCard().IsInCompany();
    }

    public CardInPlay GetMergeCandidate()
    {
        return allCards.DefaultIfEmpty(null).FirstOrDefault(
            x => x.IsCharacter() && 
            x.GetCharacterDetails().influence >= card.GetCharacterDetails().mind &&
            x != card && 
            !rejectedMerges.Contains(x));
    }

    public void AskToJoinCompanies(CardInPlay selected, CardInPlay leader)
    {

        potentialLeader = leader;
        okOption option1 = new()
        {
            text = "Merge",
            cardBoolFunc = Merge
        };
        cancelOption cancel = new()
        {
            text = "Ignore this turn",
            cardBoolFunc = Cancel
        };
        List<okOption> options = new() { option1 };

        popupManager.Initialize("Merge companies?", "Do you want " + selected.GetDetails().cardName + " to join the company of " + leader.GetDetails().cardName + "?\n\nThey will move and fight together.", selected.GetDetails().cardSprite, leader.GetDetails().cardSprite, options, cancel);
        button.enabled = false;
    }

    public void Merge()
    {
        popupManager.HidePopup();
        card.SetCompanyLeader(potentialLeader.GetDetails().cardName);
        button.enabled = true;
        potentialLeader.GetCardUI().SetFirstAtHex();
    }

    public void Cancel()
    {
        popupManager.HidePopup();
        button.enabled = true;
        rejectedMerges.Add(potentialLeader);
    }

    public Sprite GetSprite()
    {
        switch (Nations.alignments[card.owner])
        {
            case AlignmentsEnum.DARK_SERVANTS:
                return darkSprite;
            case AlignmentsEnum.FREE_PEOPLE:
                return freeSprite;
            case AlignmentsEnum.NEUTRAL:
                return neutralSprite;
            case AlignmentsEnum.CHAOTIC:
                return chaosSprite;
            case AlignmentsEnum.RENEGADE:
                return renegadeSprite;
            default:
                return null;
        }
    }
    public void Toggle()
    {
        if(turn.GetCurrentPlayer() != card.owner)
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
        selectedItems.SelectCardDetails(allCards[newPosition].GetCardUI(), allCards[newPosition].GetDetails(), false);
        allCards.Remove(card);
        allCards.Insert(newPosition, card);
    }

    public void SetFirstAtHex()
    {
        int newPosition = 0;
        selectedItems.SelectCardDetails(card.GetDetails(), false);
        allCards.Remove(card);
        allCards.Insert(newPosition, card);
    }
    public bool IsMoving()
    {
        return isMoving;
    }
}
