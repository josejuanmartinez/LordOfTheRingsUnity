using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Aoiti.Pathfinding;
using UnityEngine.UI;
using TMPro;

public class MovementManager : MonoBehaviour
{
    public static Vector3Int[] directionsEvenY = new Vector3Int[6] {Vector3Int.left, Vector3Int.left + Vector3Int.up, Vector3Int.up,
                                                 Vector3Int.right,Vector3Int.down, Vector3Int.left + Vector3Int.down};

    public static Vector3Int[] directionsUnevenY = new Vector3Int[6] {Vector3Int.left, Vector3Int.up, Vector3Int.right + Vector3Int.up,
                                                 Vector3Int.right,Vector3Int.right + Vector3Int.down, Vector3Int.down};

    public LineRenderer lineRenderer;
    public Image[] rightMovementSprites;
    public TextMeshProUGUI[] rightMovementCosts;
    public GameObject charactersGameObject;
    
    public GameObject characterGroupPrefab;
    public CharacterGroupManager leaderCharacterGroup;    
    
    public float speed = 1f;

    

    // **** REFERENCES TO STATIC OBJECTS *****
    private Board board;
    private CellHover cellHover;
    private Tilemap movementTilemap;
    private ManaPool manaPool;
    private Tilemap cardTilemap;
    private TilemapSelector tilemapSelector;
    private SelectedItems selectedItems;
    private FOWManager fow;
    private TerrainManager terrainManager;
    // ****************************************
    
    private List<Vector3> positions = new List<Vector3>();
    private short totalMovement = 0;

    private Pathfinder<Vector3Int> pathfinder;

    public static Vector3Int NULL = Vector3Int.one * int.MinValue;
    public static Vector2Int NULL2 = Vector2Int.one * int.MinValue;

    private Vector3Int showingPathDestination = NULL;

    private CameraController cameraController;

    private List<Vector3Int> path;

    private CardUI lastSelected = null;
    private Vector2Int lastHex = NULL2;


    [Range(0.001f,1f)]
    public float stepTime;


    void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        movementTilemap = GameObject.Find("MovementTilemap").GetComponent<Tilemap>();
        cardTilemap = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
        board = GameObject.Find("Board").GetComponent<Board>();
        manaPool = GameObject.Find("ManaPool").GetComponent<ManaPool>();
        tilemapSelector = GameObject.Find("TilemapSelector").GetComponent<TilemapSelector>();
        cellHover = tilemapSelector.GetComponent<CellHover>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        fow = GameObject.Find("FOWManager").GetComponent<FOWManager>();
        terrainManager = GameObject.Find("TerrainManager").GetComponent<TerrainManager>();

        charactersGameObject.SetActive(false);
    }

    public float DistanceFunc(Vector3Int a, Vector3Int b)
    {
        return (a-b).sqrMagnitude;
    }

    public Dictionary<Vector3Int,float> connectionsAndCosts(Vector3Int a)
    {
        Dictionary<Vector3Int, float> result= new Dictionary<Vector3Int, float>();
        bool even = (a.y % 2 == 0);
        Vector3Int[] directions = even ? directionsEvenY : directionsUnevenY;

        foreach (Vector3Int dir in directions)
        {
            Vector3Int destination = dir + a;
            destination = new Vector3Int(destination.x, destination.y, 0);
            int destinationPos = HexTranslator.GetNormalizedCellPosInt(destination);
            TileAndMovementCost tmc = terrainManager.GetTileAndMovementCost(destinationPos);
            if (tmc.movable)
                result.Add(destination, tmc.movementCost);
        }
        return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = new Pathfinder<Vector3Int>(DistanceFunc, connectionsAndCosts);
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedItems.IsCharSelected())
        {
            CardUI cardUI = selectedItems.GetSelectedCardUI();
            if (cardUI == null)
                return;
            CardInPlay card = cardUI.GetCard();
            Vector2Int hexSelected = board.GetSelectedHex();
            Vector3Int currentCellPos = new Vector3Int(hexSelected.x, hexSelected.y, 0);
            Vector3Int target = cellHover.last;

            if (cardUI != lastSelected)
            {
                charactersGameObject.SetActive(true);
                // LEADER: Already painted, just update
                leaderCharacterGroup.Initialize(card);
                lastSelected = cardUI;

                // COMPANY: Paint
                //1. I clean all except leader (child==0)
                int childs = charactersGameObject.transform.childCount;
                for (int i = childs - 1; i > 0; i--)
                    Destroy(charactersGameObject.transform.GetChild(i).gameObject);
                //2. I add characters
                foreach(CardInPlay charInCompany in selectedItems.GetCompany())
                {
                    GameObject charInstantiated = Instantiate(characterGroupPrefab, charactersGameObject.transform);
                    CharacterGroupManager charGroup = charInstantiated.GetComponent<CharacterGroupManager>();
                    charGroup.Initialize(charInCompany);
                }

                //TODO: Add the rest (ally, objects)
            }

            if (currentCellPos == target)
            {
                //Debug.Log("Same hex!");
                Reset();
                return;
            }

            if (Input.GetMouseButton(1))
            {
                cameraController.preventDrag = true;

                pathfinder.GenerateAstarPath(currentCellPos, target, out path);
                path.Insert(0, currentCellPos);
                if (showingPathDestination != path[path.Count - 1])
                {
                    Reset();
                    StopAllCoroutines();
                    StartCoroutine(RenderPath());
                    showingPathDestination = path[path.Count - 1];
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {   
                StopAllCoroutines();
                StartCoroutine(MovePath());
                Reset();
                cameraController.preventDrag = false;

            }
        } else
        {
            charactersGameObject.SetActive(false);
            lastSelected = null;
        }
    }

    public void Reset()
    {
        positions = new List<Vector3>();
        movementTilemap.ClearAllTiles();
        for(int i=0;i<rightMovementSprites.Length;i++)
        {
            rightMovementSprites[i].enabled = false;
            rightMovementCosts[i].enabled = false;
        }            
        positions.Clear();
        totalMovement = 0;
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }

    IEnumerator MovePath()
    {
        CardUI selectedCardUIForMovement = selectedItems.GetSelectedCardUI();
        if(selectedCardUIForMovement == null)
        {
            Reset();
            yield return null;
        }

        if (selectedCardUIForMovement != null)
        {
            if (selectedCardUIForMovement.GetCard().moved >= MovementConstants.characterMovement)
            {
                Reset();
                yield return null;
            }
            selectedCardUIForMovement.Moving();
            int currentPointIndex = 0;

            int maxPath = positions.Count;

            while (currentPointIndex + 1 < maxPath)
            {
                Vector3 startPosition = cardTilemap.CellToWorld(path[currentPointIndex]);
                startPosition = new Vector3(startPosition.x, startPosition.y, 0);
                Vector3 targetPosition = cardTilemap.CellToWorld(path[(currentPointIndex + 1)]);
                targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0);
                
                float currentLerpTime = 0f;

                Vector3Int startCell = cardTilemap.WorldToCell(startPosition);
                Vector3Int targetCell = cardTilemap.WorldToCell(targetPosition);

                short movement = terrainManager.GetMovementCost(targetCell);
                if (selectedCardUIForMovement.GetCard().moved + movement > MovementConstants.characterMovement)
                    break;

                selectedCardUIForMovement.GetCard().AddMovement(movement);

                while (currentLerpTime < 1f)
                {
                    currentLerpTime += Time.deltaTime * speed;
                    Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, currentLerpTime);
                    selectedCardUIForMovement.gameObject.transform.position = currentPosition;
                    yield return null;
                }

                currentPointIndex++;
                selectedCardUIForMovement.GetCard().AddToHex(new Vector2Int(targetCell.x, targetCell.y));

                fow.UpdateCardFOW(targetCell, startCell);

                CardInfo ci = terrainManager.GetCardInfo(targetCell);
                if (ci != null)
                    if (ci.cardType <= CardTypesEnum.SEA)
                        manaPool.AddMana(ci.cardType);
                else
                    Debug.LogError("No card info at tile " + HexTranslator.GetDebugTileInfo(targetCell));
                manaPool.ToggleDirty();
            }

            // Ensure the GameObject stays at the last position in the path
            Vector3 destination = cardTilemap.CellToWorld(path[currentPointIndex]);
            selectedCardUIForMovement.gameObject.transform.position = new Vector3(destination.x, destination.y, 0);
            selectedCardUIForMovement.StopMoving();
            selectedItems.UnselectCardDetails();
            lastHex = new Vector2Int(path[path.Count - 1].x, path[path.Count - 1].y);
            selectedCardUIForMovement.GetCard().SetHex(lastHex);
        } 
        else
            Reset();
    }

    IEnumerator RenderPath()
    {
        CardUI selectedCardUIForMovement = selectedItems.GetSelectedCardUI();
        if (selectedCardUIForMovement == null)
        {
            Reset();
            yield return null;
        }

        totalMovement = selectedCardUIForMovement.GetCard().moved;
        for (int p=0;p<path.Count;p++)
        {
            //Movement Tilemap
            Vector3Int cardTilePos = path[p];

            Vector3 cardCellCenter = cardTilemap.CellToWorld(cardTilePos);
            cardCellCenter = new Vector3(cardCellCenter.x, cardCellCenter.y, -1);
            positions.Add(cardCellCenter);
                        
            short movement = (p != 0) ? terrainManager.GetMovementCost(cardTilePos) : (short)0;
            
            if (totalMovement + movement > MovementConstants.characterMovement)
            {
                StopAllCoroutines();
                positions = positions.GetRange(0, p);
                path = path.GetRange(0, p);
                lineRenderer.positionCount = positions.Count;
                lineRenderer.SetPositions(positions.ToArray());
                break;
            }

            totalMovement += movement;
            rightMovementCosts[positions.Count - 1].text = totalMovement.ToString();
            rightMovementCosts[positions.Count - 1].enabled = true;


            Sprite spriteMovement = terrainManager.GetSpriteMovement(cardTilePos);
            
            if (spriteMovement == null)
            {
                Reset();
                StopAllCoroutines();
                break;
            }
            if (rightMovementSprites.Length < positions.Count)
            {
                Reset();
                StopAllCoroutines();
                break;
            }

            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());

            rightMovementSprites[positions.Count - 1].sprite = spriteMovement;
            rightMovementSprites[positions.Count - 1].enabled = true;            
        }
        yield return new WaitForSeconds(stepTime);
    }
    public Vector2Int GetLastHex()
    {
        return lastHex;
    }
}

