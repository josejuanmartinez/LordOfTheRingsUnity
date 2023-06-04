using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapSelector : MonoBehaviour
{
    public Tilemap[] terrainTilemaps;
    public Sprite selectedSprite;

    public static Vector3Int NULL = Vector3Int.one * int.MinValue;
    public Vector3Int hoverPos = NULL;

    private Tilemap movementTilemap;
    private Tile selectedTile, unselectedTile;
    private CameraController cameraController;

    private EventSystem eventSystem;
    private DiceManager diceManager;

    private void Awake()
    {
        selectedTile = ScriptableObject.CreateInstance<Tile>();
        selectedTile.sprite = selectedSprite;
        unselectedTile = ScriptableObject.CreateInstance<Tile>();        
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        movementTilemap = GameObject.Find("MovementTilemap").GetComponent<Tilemap>();
        cameraController = Camera.main.GetComponent<CameraController>();
        diceManager = GameObject.Find("DiceManager").GetComponent<DiceManager>();
    }

    void Update()
    {
        if (isOverUI())
        {
            Reset();
            return;
        }

        if(cameraController.IsDragging())
        {
            Reset();
            return;
        }

        if (diceManager.IsDicing())
        {
            Reset();
            return;
        }
         

        if (Input.GetKeyUp(KeyCode.Escape))
            Reset();

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cardTilePos = movementTilemap.WorldToCell(mouseWorldPos);
        Vector3 cardCellCenter = movementTilemap.CellToWorld(cardTilePos);
        cardCellCenter = new Vector3(cardCellCenter.x, cardCellCenter.y, 0);
            
        //cardTilePos = new Vector3Int(cardTilePos.x, cardTilePos.y, 0);
        cardTilePos = movementTilemap.WorldToCell(cardCellCenter);
        if (cardTilePos != hoverPos)
        {
            if(hoverPos  != NULL)
                movementTilemap.SetTile(hoverPos, unselectedTile);
            if (cardTilePos != NULL)
            {
                hoverPos = cardTilePos;
                movementTilemap.SetTile(cardTilePos, selectedTile);
            }
        }
    }

    public void Reset()
    {
        if (hoverPos!= NULL)
            movementTilemap.SetTile(hoverPos, unselectedTile);
        hoverPos = NULL;
    }

    public bool isOverUI()
    {
        // Create a PointerEventData object
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        // Create a list to store the results
        List<RaycastResult> results = new List<RaycastResult>();

        // Raycast using the EventSystem
        eventSystem.RaycastAll(pointerEventData, results);

        // Check if any UI objects were hit
        if (results.Count > 0)
        {
            // UI objects were hit
            //Debug.Log("UI objects are under the mouse.");
            return true;
        }
        else
        {
            // No UI objects were hit
            //Debug.Log("No UI objects under the mouse.");
            return false;
        }
    }
}