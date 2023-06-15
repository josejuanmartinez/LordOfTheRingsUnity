using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using TMPro;
using UnityEditor.Experimental.GraphView;
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

    public TextMeshProUGUI probabilityText;
    public CanvasGroup probabilityCanvasGroup;

    private SelectedItems selectedItems;
    private DiceManager diceManager;
    private Sprite hoverCard;
    private PopupManager popupManager;
    private CombatPopupManager combatPopupManager;

    private string openGuid = string.Empty;
    private Color colorBefore;

    private short diceResults = 0;

    private ManaPool manaPool;
    private ResourcesManager resourcesManager;
    private Turn turn;
    private Board board;
    private Game game;
    private MovementManager movementManager;
    private DeckManager deckManager;
    private SpritesRepo spritesRepo;

    // Start is called before the first frame update
    void Awake()
    {
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        diceManager = GameObject.Find("DiceManager").GetComponent<DiceManager>();
        popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
        combatPopupManager = GameObject.Find("CombatPopupManager").GetComponent<CombatPopupManager>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        colorBefore = playbuttonImage.color;
        manaPool = GameObject.Find("ManaPool").GetComponent<ManaPool>();
        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        board = GameObject.Find("Board").GetComponent<Board>();
        game = GameObject.Find("Game").GetComponent<Game>();
        movementManager = GameObject.Find("MovementManager").GetComponent<MovementManager>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        HideProbability();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedItems.GetSelectedCityDetails() != null)
        {
            if (placeImage.sprite != selectedItems.GetSelectedCityDetails().darkSprite && 
                placeImage.sprite != selectedItems.GetSelectedCityDetails().freeSprite &&
                placeImage.sprite != selectedItems.GetSelectedCityDetails().renegadeSprite &&
                placeImage.sprite != selectedItems.GetSelectedCityDetails().balrogSprite &&
                placeImage.sprite != selectedItems.GetSelectedCityDetails().lordSprite)
            {
                CityDetails cityDetails = selectedItems.GetSelectedCityDetails();
                placeImage.sprite = cityDetails.GetSprite(turn.GetCurrentPlayer());
            }
            HideProbability();
            place.SetActive(true);
        }
        else if (selectedItems.GetSelectedCardDetails() != null)
        {
            CardDetails details = selectedItems.GetSelectedCardDetails();
            ShowProbability(details);
            placeImage.sprite = details.cardSprite;
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
        ShowProbability(details);
    }

    public void SetHoverCity(CityDetails details)
    {
        hoverCard = details.GetSprite(turn.GetCurrentPlayer());
        HideProbability();
    }

    private void HideProbability()
    {
        probabilityCanvasGroup.alpha = 0;
        probabilityText.text = "";
    }

    private void ShowProbability(CardDetails details)
    {
        if (details.IsInPlay())
        {
            HideProbability();
            return;
        }            
        probabilityCanvasGroup.alpha = 1;
        probabilityText.text = "";
        switch (details.cardClass)
        {
            case CardClass.Character:
                probabilityText.text = Math.Truncate(10 * resourcesManager.GetCharacterSuccessProbability(details)).ToString();
                break;
            case CardClass.HazardCreature:
                probabilityText.text = Math.Truncate(10 * manaPool.GetHazardCreatureSuccessProbability(details)).ToString();
                break;
            default:
                HideProbability();
                break;
        }
    }
    
    public void RemoveHoverCard(CardInPlay card)
    {
        RemoveHoverCard(card.GetDetails());
    }
    public void RemoveHoverCity(CityInPlay city)
    {
        RemoveHoverCity(city.GetDetails());
    }
    
    public void RemoveHoverCity(CityDetails cityDetails)
    {
        if (hoverCard == cityDetails.darkSprite ||
                hoverCard == cityDetails.freeSprite ||
                hoverCard == cityDetails.renegadeSprite ||
                hoverCard == cityDetails.balrogSprite ||
                hoverCard == cityDetails.lordSprite)
        {
            hoverCard = null;
            place.SetActive(false);
        }
    } 

    public void SetHoverCard(CardInPlay card)
    {
        SetHoverCard(card.GetDetails());
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
            if (selectedItems.IsObjectSelected())
                PlayObject();
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
                if (cardDetails.cardClass == CardClass.Character)
                {
                    resourcesManager.RecalculateInfluence(turn.GetCurrentPlayer());
                    resourcesManager.RefreshInfluence();
                }                    
                // Update mana
                if(cardDetails.cardClass == CardClass.HazardCreature)
                {
                    HazardCreatureCardDetails hazard = cardDetails.GetHazardCreatureCardDetails();
                    foreach(CardTypesEnum cardType in hazard.cardTypes)
                    {
                        int newRes = manaPool.manaPool[cardType] > 0 ? manaPool.manaPool[cardType] - 1 : 0;
                        manaPool.manaPool[cardType] = (short)newRes;
                    }
                    manaPool.ToggleDirty();
                }
                
            }                
            else
            {
                Debug.LogError("Something went wrong drawing a card");
            }
        }
        else
            playbuttonImage.color = Color.red;

        deckManager.DiscardAndDraw(cardDetails);
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

    public void TapToGetObject()
    {
        popupManager.HidePopup();

        CardDetails objectDetails = selectedItems.GetSelectedCardDetails();
        if(objectDetails != null)
        {
            CardDetails characterDetails = selectedItems.GetLastSelectedChar();
            if(characterDetails != null && characterDetails.GetCardInPlay() != null)
            {
                CardInPlay cardInPlay = characterDetails.GetCardInPlay();
                CityInPlay city = board.GetCityManager().GetCityAtHex(cardInPlay.GetHex());
                if(city != null && city.GetDetails() !=null)
                {
                    CityDetails cityDetails = city.GetDetails();
                    List<AutomaticAttackEnum> attacks = cityDetails.GetAutomaticAttacks(turn.GetCurrentPlayer());
                    combatPopupManager.ShowPopup();
                    combatPopupManager.Initialize(attacks, cardInPlay, cityDetails);
                }
            }
        }
    }

    public void PlayCharacter()
    {
        if (selectedItems.GetSelectedCardDetails() == null)
            return;

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
        List<okOption> options = new() { };

        CardDetails cc = selectedItems.GetSelectedCardDetails();
        if(cc != null)
        {
            if (deckManager.CanSpawnAtHomeTown(cc, turn.GetCurrentPlayer()))
                options.Add(option2);

            if (deckManager.CanSpawnAtHaven(turn.GetCurrentPlayer()))
                options.Add(option1);
        }        

        popupManager.Initialize("Recruit Character", "Where do you want to recruit <b>" + cc.cardName + "</b>?", cc.cardSprite, spritesRepo.town, options, cancel);
        button.enabled = false;
    }
    public void PlayHazardCreature()
    {
        if (selectedItems.GetSelectedCardDetails() == null)
            return;

        okOption option1 = new()
        {
            text = "Last visited cell",
            cardBoolFunc = HazardCreatureAtLastCell
        };
        okOption option2 = new()
        {
            text = "Haven",
            cardBoolFunc = HazardCreatureAtHaven
        };
        cancelOption cancel = new()
        {
            text = "Cancel",
            cardBoolFunc = Cancel
        };
        List<okOption> options = new() {  };

        CardDetails cc = selectedItems.GetSelectedCardDetails();
        if (cc != null)
        {
            if (deckManager.CanSpawnAtLastHex())
                options.Add(option1);
            if (deckManager.CanSpawnAtHaven(turn.GetCurrentPlayer()))
                options.Add(option2);            
        }

        string message = "Where do you want to spawn <b>" + cc.cardName + "</b>?\n\n";
        if(options.Contains(option1))
            message += "By selecting <i>Last visited cell</i>, the creature will appear at " + selectedItems.GetLastSelectedChar().cardName + " position.";

        Sprite sprite = spritesRepo.wild;
        if (!options.Contains(option1))
            sprite = spritesRepo.town;

        popupManager.Initialize("Spawn a creature?", message, cc.cardSprite, sprite, options, cancel);
        button.enabled = false;
    }

    public void PlayObject()
    {
        if (selectedItems.GetSelectedCardDetails() == null)
            return;

        CardDetails card = selectedItems.GetSelectedCardDetails();
        if (card == null)
            return;

        CardDetails characterDetails = selectedItems.GetLastSelectedChar();
        if (characterDetails == null)
            return;
        if (characterDetails.GetCardInPlay() == null)
            return;

        CityInPlay cityInPlay = board.GetCityManager().GetCityAtHex(characterDetails.GetCardInPlay().GetHex());
        if (cityInPlay == null)
            return;
        
        CityDetails cityDetails = cityInPlay.GetDetails();
        if (cityDetails == null)
            return;
        
        okOption option1 = new()
        {
            text = "Tap",
            cardBoolFunc = TapToGetObject
        };
        cancelOption cancel = new()
        {
            text = "Cancel",
            cardBoolFunc = Cancel
        };
        List<okOption> options = new() { option1 };

        popupManager.Initialize("Tap place?", "Do you want to tap the place to obtain the object?\n\nTapping makes the site <i>visited</i>, what means you will not be able to obtain anything else in here.\n\n By tapping, you will trigger any automatic attacks towards your company. If the places follows your alignment, the attacks will be of <i>detaintment</i> and your company will not be hurt.", card.cardSprite, cityDetails.GetSprite(turn.GetCurrentPlayer()), options, cancel);
        button.enabled = false;
    }

    public short GetDiceResults()
    {
        return diceResults;
    }
}
