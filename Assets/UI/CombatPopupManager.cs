using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatPopupManager : MonoBehaviour
{
    public GameObject popup;

    public Image citySprite;
    public TextMeshProUGUI title;
    public GameObject defendersPrefab;
    public GameObject attackersPrefab;
    public GameObject strikePrefab;

    public GridLayoutGroup defenders;
    public GridLayoutGroup attackers;

    public List<AutomaticAttackEnum> attacksDebug = new List<AutomaticAttackEnum>();
    public GameObject cardInPlayPrefabDebug;
    public CityDetails cityDetailsDebug;

    public bool debug = false;
    public bool isInitialized = false;

    private Board board;
    private SpritesRepo spriteRepo;

    void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        spriteRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
    }

    public void Initialize(List<AutomaticAttackEnum> attacks, CardInPlay card, CityDetails cityDetails)
    {
        if (card.GetDetails() == null)
            return;
        CardDetails cardDetails = card.GetDetails();
        if (cardDetails.GetCharacterCardDetails() == null)
            return;
        title.text = "Encounter at " + cityDetails.cityName;
        
        citySprite.sprite = cityDetails.GetSprite(cardDetails.GetCardInPlay().owner);

        for(int i=defenders.transform.childCount-1; i>=0; i--)
            DestroyImmediate(defenders.transform.GetChild(i).gameObject);
        
        for (int i = attackers.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(attackers.transform.GetChild(i).gameObject);

        foreach (AutomaticAttackEnum attack in attacks)
        {
            GameObject attacker = Instantiate(attackersPrefab, attackers.transform);
            attacker.GetComponent<Attacker>().Initialize(spriteRepo.GetRaceSprite(AutomaticAttack.automaticAttacks[attack].race));
            int strikes = AutomaticAttack.automaticAttacks[attack].strikes;
            int prowess = AutomaticAttack.automaticAttacks[attack].prowess;
            for(int i=0; i<strikes; i++)
            {
                GameObject strike = Instantiate(strikePrefab, attacker.transform.Find("StrikeGrids"));
                strike.GetComponent<DragDrop>().Initialize(prowess);
            }
        }
                
        List<CardInPlay> characters = board.GetCharacterManager().GetCharactersInCompanyOf(cardDetails);
        characters.Add(cardDetails.GetCardInPlay());

        foreach(CardInPlay character in characters)
        {
            GameObject defender = Instantiate(defendersPrefab, defenders.transform);
            defender.GetComponent<CharacterUIInitializer>().Initialize(character);
        }
        isInitialized = true;

    }

    void Update()
    {
        if(debug)
        {
            debug = false;
            GameObject cardDebugCloned = Instantiate(cardInPlayPrefabDebug);
            CardInPlay cardInPlayDebug = cardDebugCloned.GetComponent<CardInPlay>();
            cardInPlayDebug.Initialize(Vector2Int.zero);
            Initialize(attacksDebug, cardInPlayDebug, cityDetailsDebug);
            
        }    
    }

    public void ShowPopup()
    {
        popup.gameObject.SetActive(true);
    }

    public void HidePopup()
    {
        popup.gameObject.SetActive(false);
    }

    public bool IsShown()
    {
        return popup.gameObject.activeSelf;
    }
}
