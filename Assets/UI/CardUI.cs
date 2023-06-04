using System;
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
    public Image icon;
    public bool isShownWithOtherCard = false;

    public string id = Guid.NewGuid().ToString(); 
    
    private bool isOpen = false;
    
    private Board board;
    private CardInPlay card;

    private bool isMoving = false;
    private bool initialized = false;

    private Tilemap t;
    private SelectedItems selectedItems;

    private Vector3 currentDisplacementPosition = Vector3.zero;
    private Vector3 currentPosition = Vector3.zero;

    void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        detailsObject.SetActive(false);
        t = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
    }

    public void Initialize()
    {
        card = GetComponent<CardInPlay>();

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

            prowess.text = Sprites.prowess + prowessValue.ToString();
            defence.text = Sprites.defence + defenceValue.ToString();

        }

        RefreshAtHex();
        initialized = true;
    }

    public void RefreshAtHex()
    {
        Vector3 cellWorldCenter = t.GetCellCenterWorld(new Vector3Int(card.hex.x, card.hex.y, 0));
        if (currentPosition != cellWorldCenter)
        {
            gameObject.transform.position = cellWorldCenter;
            currentPosition = cellWorldCenter;
        }

        isShownWithOtherCard = board.GetTile(card.GetHex()).HasCity() && !isMoving;

        short expectedHorizontalDisplacement = isShownWithOtherCard ? DisplacementPixels.right : DisplacementPixels.NONE;
        Vector3 expectedDisplacementPosition = new Vector3(expectedHorizontalDisplacement, DisplacementPixels.down, 0);
        if (expectedDisplacementPosition != currentDisplacementPosition)
        {
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

        RefreshAtHex();
    }

    public void Toggle()
    {
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
}
