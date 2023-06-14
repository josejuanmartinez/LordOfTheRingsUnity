using System;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CityUI : MonoBehaviour
{
    public GameObject displacement;
    public GameObject detailsObject;
    public TextMeshProUGUI detailsText;
    public TextMeshProUGUI cityName;
    public Button button;
    public Image health;
    public Image citySprite;
    public Image alignment;

    public CanvasGroup tapped;

    public Sprite freeSprite;
    public Sprite darkSprite;
    public Sprite renegadeSprite;
    public Sprite neutralSprite;
    public Sprite poiSprite;

    public bool isOpen = false;
    public bool isShownWithOtherCard = false;

    public string id = Guid.NewGuid().ToString();

    private Board board;
    private CityInPlay city;
    private SelectedItems selectedItems;
    private Tilemap t;
    private Game game;

    private bool isInitialized = false;

    private void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        t = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
        game = GameObject.Find("Game").GetComponent<Game>();
    }
    public void Initialize()
    {
        if (!game.IsInitialized())
            return;
        
        city = GetComponent<CityInPlay>();
        
        button.interactable = city.owner == game.GetHumanPlayer().GetNation();

        if (city == null)
            return;
        if (!city.IsInitialized())
            return;
        

        CityDetails details = city.GetDetails();

        Vector3 cellWorldCenter = t.GetCellCenterWorld(new Vector3Int(city.hex.x, city.hex.y, 0));

        gameObject.transform.position = cellWorldCenter;

        detailsText.text = Sprites.gold + details.production.gold.ToString() + "\n" +  Sprites.prod + details.production.prod.ToString() + "\n" + Sprites.food + details.production.food.ToString();

        switch(game.GetHumanPlayer().GetAlignment())
        {
            case AlignmentsEnum.FREE_PEOPLE:
            case AlignmentsEnum.NEUTRAL:
                citySprite.sprite = details.freeSprite;
                break;
            case AlignmentsEnum.DARK_SERVANTS:
                citySprite.sprite = details.darkSprite;
                break;
            case AlignmentsEnum.RENEGADE:
                citySprite.sprite = details.renegadeSprite != null ? details.renegadeSprite : details.darkSprite;
                break;
        }

        if(city.owner == NationsEnum.NONE)
        {
            alignment.sprite = poiSprite;
        }
        else
        {
            switch (Nations.alignments[city.owner])
            {
                case AlignmentsEnum.NEUTRAL:
                    alignment.sprite = neutralSprite;
                    break;
                case AlignmentsEnum.FREE_PEOPLE:
                    alignment.sprite = freeSprite;
                    break;
                case AlignmentsEnum.DARK_SERVANTS:
                    alignment.sprite = darkSprite;
                    break;
                case AlignmentsEnum.RENEGADE:
                    alignment.sprite = renegadeSprite;
                    break;
                case AlignmentsEnum.NONE:
                    alignment.sprite = poiSprite;
                    break;
            }
        }

        cityName.text = details.cityName + (details.isHaven ? "<sprite name=\"haven\">" : "");
        detailsObject.SetActive(false);

        isInitialized = true;

        tapped.alpha = 0;
    }

    void Update()
    {
        if (!isInitialized)
            Initialize();
        
        if (!isInitialized)
            return;

        if (Input.GetKeyUp(KeyCode.Escape))
            isOpen = false;

        if (isOpen && selectedItems.GetOpenGUID() != id)
            isOpen = false;

        if(detailsObject.activeSelf != isOpen)
            detailsObject.SetActive(isOpen);

        isShownWithOtherCard = board.GetTile(city.GetHex()).HasCards() && game.GetHumanPlayer().SeesTile(city.GetHex());

        short expectedHorizontalDisplacement = isShownWithOtherCard ? DisplacementPixels.left : DisplacementPixels.NONE;
        displacement.transform.localPosition = new Vector3(expectedHorizontalDisplacement, DisplacementPixels.down, 0);
     }

    public void Toggle()
    {
        if (game.GetHumanPlayer().GetNation() != city.owner)
        {
            isOpen = false;
            return;
        }

        isOpen = !isOpen;
        if (isOpen)
        {
            board.SelectHex(city.hex);
            selectedItems.SelectCityUI(this);
        }            
        else if (selectedItems.GetOpenGUID() == id)
        {
            board.SelectHex(Board.NULL);
            selectedItems.UnselectCityUI();
        }            
    }

    public void ToggleShownWithOtherCard()
    {
        isShownWithOtherCard = !isShownWithOtherCard;
    }

    public CityInPlay GetCity()
    {
        return city;
    }

    public void Tap()
    {
        tapped.alpha = 1;
    }
}
