using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckCardUI : MonoBehaviour
{
    public short handPos = 0;
    
    private Button button;

    private CardDetails cardDetails;
    private DeckManager deck;
    private ManaPool pool;
    private PlaceDeck placeDeck;

    private NationsEnum owner;

    private bool isSelected = false;

    private SelectedItems selectedItems;

    private string id = Guid.NewGuid().ToString();

    void Awake()
    {
        deck = GameObject.Find("Deck").GetComponent<DeckManager>();
        pool = GameObject.Find("ManaPool").GetComponent<ManaPool>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        placeDeck = GameObject.Find("PlaceDeck").GetComponent<PlaceDeck>();
        owner = GameObject.Find("Turn").GetComponent<Turn>().GetCurrentPlayer();
        
        button = GetComponent<Button>();
        button.onClick.AddListener(Toggle);
    }

    void Update()
    {
        if (IsPointerOverUIElement())
            deck.Show(handPos);
        else
            deck.Hide(handPos);

        cardDetails = GetComponent<CardDetails>();

        MeetRequirements();
    }

    public void MeetRequirements()
    {
        switch(cardDetails.cardClass)
        {
            case CardClass.HazardCreature:
                HazardCreatureCardDetails hazard = GetComponent<HazardCreatureCardDetails>();
                button.interactable = pool.HasEnoughMana(hazard.cardTypes);
                break;
            case CardClass.Character:
                CharacterCardDetails character = GetComponent<CharacterCardDetails>();
                button.interactable = deck.IsCharacterCardPlayable(character, owner);
                break;
        }
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("ScreenOverlayUILayer") && curRaysastResult.gameObject == gameObject)
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    public void increaseHandPosition()
    {
        handPos++;
    }

    public void Toggle()
    {
        isSelected = !isSelected;
        if (isSelected)
        {
            placeDeck.SetOpenGUID(id);
            selectedItems.SelectCardDetails(cardDetails, true);
        }            
        else if (placeDeck.GetOpenGUID() == id)
        {
            placeDeck.SetOpenGUID(string.Empty);
            selectedItems.UnselectCardDetails();
        }            
    }
}
