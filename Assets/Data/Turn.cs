using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Turn : MonoBehaviour
{
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI playerText;
    public Image alignment;

    private short turnNumber = 0;
    private NationsEnum currentTurnPlayer = NationsEnum.UVATHA;

    private bool isDirty = true;

    private Board board;
    private Game game;
    private bool isInitialized = false;
    
    private CameraController cameraController;
    private FOWManager fowManager;
    private SpritesRepo spritesRepo;

    private void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        game = GameObject.Find("Game").GetComponent<Game>();
        cameraController = Camera.main.GetComponent<CameraController>();
        fowManager = GameObject.Find("FOWManager").GetComponent<FOWManager>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
    }

    public void Initialize()
    {
        if(board.IsInitialized() && game.IsInitialized())
        {
            NewTurn();
            isInitialized = true;
        }
        
    }

    void Update()
    {
        if (!isInitialized)
        {
            Initialize();
            return;
        }            
        
        if (isDirty)
            turnText.text = turnNumber.ToString();
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    public void NewTurn()
    {
        turnNumber++;

        currentTurnPlayer = game.GetHumanPlayer().GetNation();

        isDirty = true;
        fowManager.UpdateCitiesFOW();
        fowManager.UpdateCardsFOW();

        RefreshTurnInfo();

        board.GetCharacterManager().GetCharactersOfPlayer(currentTurnPlayer).ForEach(x => x.moved = 0);
        
        CardInPlay avatar = board.GetCharacterManager().GetAvatar(GetCurrentPlayer());
        if (avatar)
            cameraController.LookToCard(avatar);
    }

    public void RefreshTurnInfo()
    {
        turnText.text = turnNumber.ToString();
        playerText.text = LocalizationEN.Localize(currentTurnPlayer.ToString()) + " [" + LocalizationEN.Localize(Nations.regions[currentTurnPlayer].ToString()) + "]";
        alignment.sprite = spritesRepo.GetAlignmentSprite(currentTurnPlayer);
    }

    public NationsEnum GetCurrentPlayer()
    {
        return currentTurnPlayer;
    }
    public void PlayIATurn()
    {
        RefreshTurnInfo();
        NextPlayer();
    }

    public void NextPlayer()
    {
        currentTurnPlayer = (NationsEnum)(((int)currentTurnPlayer + 1) % Enum.GetValues(typeof(NationsEnum)).Length);
        if (currentTurnPlayer == NationsEnum.NONE || currentTurnPlayer == game.GetHumanPlayer().GetNation())
            NewTurn();
        else
            PlayIATurn();
    }

}
