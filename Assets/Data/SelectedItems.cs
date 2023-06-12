using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SelectedItems : MonoBehaviour
{
    private CardDetails selectedCardDetails;
    private CityDetails selectedCityDetails;

    private List<CardInPlay> selectedCompany;

    private MovementManager moveOnTilemap;
    private PlaceDeck placeDeck;
    private Board board;

    private bool isDeck = false;

    private void Awake()
    {
        moveOnTilemap = GameObject.Find("MovementManager").GetComponent<MovementManager>();
        placeDeck = GameObject.Find("PlaceDeck").GetComponent<PlaceDeck>();
        board = GameObject.Find("Board").GetComponent<Board>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            UnselectAll();
    }

    public void SelectCityDetails(CityDetails city)
    {
        selectedCityDetails = city;
        selectedCardDetails = null;
    }

    public void UnselectCityDetails()
    {
        selectedCityDetails = null;
    }

    public void SelectCardDetails(CardDetails cardDetails, bool isDeck)
    {
        SelectCardDetails(null, cardDetails, isDeck);
    }

    public void SelectCardDetails(CardUI cardUI, CardDetails cardDetails, bool isDeck)
    {
        selectedCityDetails = null;
        selectedCardDetails = cardDetails;

        // COMPANY
        selectedCompany = board.GetCharacterManager().GetCharactersInCompanyOf(cardDetails);

        this.isDeck = isDeck;
    }

    public void UnselectCardDetails()
    {
        selectedCardDetails = null;
        isDeck = false;
    }
    public bool IsDeckCardSelected()
    {
        return isDeck;
    }

    public bool IsCardInPlay()
    {
        if (selectedCardDetails != null)
        {
            return selectedCardDetails.IsInPlay();
        }
        else
        {
            return false;
        }
    }
    public bool IsCharSelected()
    {
        if (selectedCardDetails != null) {
            return selectedCardDetails.gameObject.GetComponent<CharacterCardDetails>() != null;
        } else {
            return false;
        }        
    }
    public bool IsHazardCreatureSelected()
    {
        if (selectedCardDetails != null)
        {
            return selectedCardDetails.gameObject.GetComponent<HazardCreatureCardDetails>() != null;
        } else
        {
            return false;
        }
    }

    public bool IsCitySelected()
    {
        return selectedCityDetails != null;
    }

    public void SelectCityUI(CityUI cityUI)
    {
        SelectCityDetails(cityUI.GetCity().GetDetails());
        placeDeck.SetOpenGUID(cityUI.id);
    }

    public void SelectCardUI(CardUI cardUI)
    {
        SelectCardDetails(cardUI, cardUI.GetCard().GetDetails(), false);
        placeDeck.SetOpenGUID(cardUI.id);
    }

    public void UnselectCardUI()
    {
        UnselectCardDetails();
    }

    public void UnselectCityUI()
    {
        UnselectCityDetails();
    }

    public CardUI GetSelectedCardUI()
    {
        // There may not be a selected OR there may be a selected card in Deck (not in play)
        if (selectedCardDetails != null && selectedCardDetails.GetCardInPlay() != null)
            return selectedCardDetails.GetCardInPlay().GetCardUI();
        else return null;
    }

    public HazardCreatureCardDetails GetHazardCreatureCardDetails()
    {
        if (!IsHazardCreatureSelected())
            return null;
        return selectedCardDetails.gameObject.GetComponent<HazardCreatureCardDetails>();
    }

    public string GetOpenGUID()
    {
        return placeDeck.GetOpenGUID();
    }

    public void SetOpenGUID(string id)
    {
        placeDeck.SetOpenGUID(id);
    }

    public CardDetails GetSelectedCardDetails()
    {
        return selectedCardDetails;
    }

    public CityDetails GetSelectedCityDetails()
    {
        return selectedCityDetails;
    }

    public void UnselectAll()
    {
        selectedCardDetails = null;
        selectedCityDetails = null;
        isDeck = false;
    }

    public List<CardInPlay> GetCompany()
    {
        return selectedCompany;
    }
}
