using UnityEngine;

public class SelectedItems : MonoBehaviour
{
    private CardDetails selectedCardDetails;
    private CityDetails selectedCityDetails;

    private MoveOnTilemap moveOnTilemap;
    private PlaceDeck placeDeck;

    private bool isDeck = false;

    private void Awake()
    {
        moveOnTilemap = GameObject.Find("Movement").GetComponent<MoveOnTilemap>();
        placeDeck = GameObject.Find("PlaceDeck").GetComponent<PlaceDeck>();
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

    public void SelectCardDetails(CardDetails card, bool isDeck)
    {
        selectedCityDetails = null;
        selectedCardDetails = card;
        this.isDeck = isDeck;
    }
    public void SelectCardDetails(CardDetails card)
    {
        selectedCityDetails = null;
        selectedCardDetails = card;
        isDeck = false;
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
        SelectCardDetails(cardUI.GetCard().GetDetails());
        moveOnTilemap.SetSelectedCardUIForMovement(cardUI);
        placeDeck.SetOpenGUID(cardUI.id);
    }

    public void UnselectCardUI()
    {
        UnselectCardDetails();
        moveOnTilemap.SetSelectedCardUIForMovement(null);
    }

    public void UnselectCityUI()
    {
        UnselectCityDetails();
    }

    public CardUI GetSelectedCharacter()
    {
        return moveOnTilemap.GetSelectedCardUIForMovement();
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
}
