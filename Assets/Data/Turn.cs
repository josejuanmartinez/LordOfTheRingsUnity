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
    private CameraController cameraController;
    private void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        game = GameObject.Find("Game").GetComponent<Game>();
        cameraController = Camera.main.GetComponent<CameraController>();
        isNewTurn = true;
    }

    void Update()
    {
        if (isDirty)
            turnText.text = turnNumber.ToString();
        if (board.IsInitialized() && game.IsInitialized())
        {
            if (isNewTurn)
            {
                CardInPlay avatar = board.GetCharacterManager().GetAvatar(game.GetHumanPlayer().GetNation());
                if (avatar)
                    cameraController.LookToCard(avatar);
                currentTurnPlayer = game.GetHumanPlayer().GetNation();
                isNewTurn = false;
            }
        }
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
