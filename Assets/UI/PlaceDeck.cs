using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlaceDeck : MonoBehaviour
{
    public GameObject place;
    public Image placeImage;
    public Button button;
    public Image playbuttonImage;
    public short hideTime = 2;


    private SelectedItems selectedItems;
    private DiceManager diceManager;
    private Sprite hoverCard;
    private PopupManager popupManager;

    private string openGuid = string.Empty;
    private Color colorBefore;

    private short diceResults = 0;

    private ManaPool manaPool;
    private ResourcesManager resourcesManager;
    private Turn turn;
    private Board board;

    // Start is called before the first frame update
    void Awake()
    {
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        diceManager = GameObject.Find("DiceManager").GetComponent<DiceManager>();
        popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
        colorBefore = playbuttonImage.color;
        manaPool = GameObject.Find("ManaPool").GetComponent<ManaPool>();
        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        board = GameObject.Find("Board").GetComponent<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedItems.GetSelectedCityDetails() != null)
        {
            if (placeImage.sprite != selectedItems.GetSelectedCityDetails().darkSprite)
                placeImage.sprite = selectedItems.GetSelectedCityDetails().darkSprite;
            place.SetActive(true);
        }
        else if (selectedItems.GetSelectedCardDetails() != null)
        {
            if (placeImage.sprite != selectedItems.GetSelectedCardDetails().cardSprite)
                placeImage.sprite = selectedItems.GetSelectedCardDetails().cardSprite;
            place.SetActive(true);
        }
        else if (hoverCard != null)
        {
            placeImage.sprite = hoverCard;
            place.SetActive(true);

        }
        else
        {
            place.SetActive(false);
        }

        //if (place.activeSelf)
        //    button.gameObject.SetActive(selectedItems.IsDeckCardSelected());
    }

    public void RemoveHoverCard(CityDetails card)
    {
        if (hoverCard == card.darkSprite)
        {
            hoverCard = null;
            place.SetActive(false);
        }

    }

    public void RemoveHoverCard(CardDetails details)
    {
        if (hoverCard == details.cardSprite)
        {
            hoverCard = null;
            place.SetActive(false);
        }

    }

    public void SetHoverCard(CardDetails details)
    {
        hoverCard = details.cardSprite;
    }
    public void SetHoverCity(CityDetails details)
    {
        switch(Nations.alignments[turn.GetCurrentPlayer()])
        {
            case AlignmentsEnum.FREE_PEOPLE:
            case AlignmentsEnum.NEUTRAL:
                hoverCard = details.freeSprite;
                break;
            case AlignmentsEnum.DARK_SERVANTS:
            case AlignmentsEnum.RENEGADE:
                hoverCard = details.darkSprite;
                break;
        }
    }

    public void RemoveHoverCard(CardInPlay card)
    {
        if (hoverCard == card.GetDetails().cardSprite)
        {
            hoverCard = null;
            place.SetActive(false);
        }
    }
    public void RemoveHoverCity(CityInPlay city)
    {
        if (hoverCard == city.GetDetails().darkSprite)
        {
            hoverCard = null;
            place.SetActive(false);
        }
    }

    public void RemoveHoverCity(CityDetails cityDetails)
    {
        if (hoverCard == cityDetails.darkSprite)
        {
            hoverCard = null;
            place.SetActive(false);
        }
    } 

    public void SetHoverCard(CardInPlay card)
    {
        hoverCard = card.GetDetails().cardSprite;
    }

    public void SetHoverCard(CityInPlay city)
    {
        hoverCard = city.GetDetails().darkSprite;
    }

    public string GetOpenGUID()
    {
        return openGuid;
    }

    public void SetOpenGUID(string openGuid)
    {
        this.openGuid = openGuid;
    }

    public void PlayCard()
    {
        if (!selectedItems.IsCardInPlay())
        {
            if (selectedItems.IsCharSelected())
                PlayCharacter();
            if (selectedItems.IsHazardCreatureSelected())
                PlayHazardCreature();
        }
    }

    public void GatherDiceResults(int diceValue, SpawnCardLocation spawnCardLocation, CardDetails cardDetails)
    {
        diceResults = (short)diceValue;
        
        float prob = 0f;
        if (cardDetails.cardClass == CardClass.Character)
            prob = resourcesManager.GetCharacterSuccessProbability(cardDetails);
        else if (cardDetails.cardClass == CardClass.HazardCreature)
            prob = manaPool.GetHazardCreatureSuccessProbability(cardDetails);
        
        float result = (1f * diceValue / DiceManager.D10);
        if (result >= prob)
        {
            bool success = board.CreateCardUI(cardDetails, spawnCardLocation);
            if (success)
            {
                playbuttonImage.color = Color.green;
                // Update resources
                // Remove card from deck
            }                
            else
            {
                Debug.LogError("Something went wrong drawing a card");
            }
        }
        else
            playbuttonImage.color = Color.red;

        StartCoroutine(HideDices());
    }

    IEnumerator HideDices()
    {
        yield return new WaitForSeconds(hideTime);
        playbuttonImage.color = colorBefore;

        selectedItems.UnselectAll();
        hoverCard = null;

        button.enabled = true;
    }

    public void CharacterAtHaven()
    {
        popupManager.HidePopup();
        StartCoroutine(diceManager.Roll(DiceRollEnum.CharacterRoll, SpawnCardLocation.AtHaven, selectedItems.GetSelectedCardDetails()));
    }

    public void CharacterAtHomeTown()
    {
        popupManager.HidePopup();
        StartCoroutine(diceManager.Roll(DiceRollEnum.CharacterRoll, SpawnCardLocation.AtHomeTown, selectedItems.GetSelectedCardDetails()));
    }

    public void Cancel()
    {
        popupManager.HidePopup();
        button.enabled = true;
    }

    public void HazardCreatureAtHaven()
    {
        popupManager.HidePopup();
        StartCoroutine(diceManager.Roll(DiceRollEnum.HazardCreatureRoll, SpawnCardLocation.AtHaven, selectedItems.GetSelectedCardDetails()));
    }

    public void HazardCreatureAtLastCell()
    {
        popupManager.HidePopup();
        StartCoroutine(diceManager.Roll(DiceRollEnum.HazardCreatureRoll, SpawnCardLocation.AtLastCell, selectedItems.GetSelectedCardDetails()));
    }

    public void PlayCharacter()
    {
        okOption option1 = new()
        {
            text = "At Haven",
            cardBoolFunc = CharacterAtHaven
        };
        okOption option2 = new()
        {
            text = "At Home Town",
            cardBoolFunc = CharacterAtHomeTown
        };
        cancelOption cancel = new()
        {
            text = "Cancel",
            cardBoolFunc = Cancel
        };
        List<okOption> options = new() { option1, option2 };

        popupManager.Initialize("Where do you want to recruit your character?", options, cancel);
        button.enabled = false;
    }
    public void PlayHazardCreature()
    {
        okOption option1 = new()
        {
            text = "At the last visited cell",
            cardBoolFunc = HazardCreatureAtLastCell
        };
        okOption option2 = new()
        {
            text = "At Haven",
            cardBoolFunc = HazardCreatureAtHaven
        };
        cancelOption cancel = new()
        {
            text = "Cancel",
            cardBoolFunc = Cancel
        };
        List<okOption> options = new() { option1, option2 };

        popupManager.Initialize("Where do you want to spawn a hazard creature?", options, cancel);
        button.enabled = false;
    }

    public short GetDiceResults()
    {
        return diceResults;
    }
}
