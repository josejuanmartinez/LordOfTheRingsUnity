using System;
using TMPro;
using UnityEngine;

public class Turn : MonoBehaviour
{
    public TextMeshProUGUI turnText;
    
    private short turnNumber = 0;
    private NationsEnum currentTurnPlayer = NationsEnum.UVATHA;

    private bool isDirty = true;

    private Board board;
    private Game game;
    private bool isNewTurn = true;
    private bool isInitialized = false;
    
    private CameraController cameraController;
    private FOWManager fowManager;

    private void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        game = GameObject.Find("Game").GetComponent<Game>();
        cameraController = Camera.main.GetComponent<CameraController>();
        fowManager = GameObject.Find("FOWManager").GetComponent<FOWManager>();
    }

    public void Initialize()
    {
        if(board.IsInitialized() && game.IsInitialized())
        {
            AddTurn();
            turnText.text = turnNumber.ToString();
            currentTurnPlayer = game.GetHumanPlayer().GetNation();
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
        
        if (isNewTurn)
        {
            fowManager.UpdateCitiesFOW();
            fowManager.UpdateCardsFOW();
            CardInPlay avatar = board.GetCharacterManager().GetAvatar(GetCurrentPlayer());
            if (avatar)
                cameraController.LookToCard(avatar);
            isNewTurn = false;

            
        }
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    public void AddTurn()
    {
        turnNumber++;
        isDirty = true;
        isNewTurn = true;
    }

    public NationsEnum GetCurrentPlayer()
    {
        return currentTurnPlayer;
    }

    public void NextPlayer()
    {
        currentTurnPlayer = (NationsEnum)(((int)currentTurnPlayer + 1) % Enum.GetValues(typeof(NationsEnum)).Length);
        if (currentTurnPlayer == NationsEnum.NONE)
        {
            AddTurn();
            currentTurnPlayer = NationsEnum.UVATHA;
        }
    }

}
