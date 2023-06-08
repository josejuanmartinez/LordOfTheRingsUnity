using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Aoiti.Pathfinding;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class MoveOnTilemap : MonoBehaviour
{
    public static Vector3Int[] directionsEvenY = new Vector3Int[6] {Vector3Int.left, Vector3Int.left + Vector3Int.up, Vector3Int.up,
                                                 Vector3Int.right,Vector3Int.down, Vector3Int.left + Vector3Int.down};

    public static Vector3Int[] directionsUnevenY = new Vector3Int[6] {Vector3Int.left, Vector3Int.up, Vector3Int.right + Vector3Int.up,
                                                 Vector3Int.right,Vector3Int.right + Vector3Int.down, Vector3Int.down};

    public Tilemap[] terrainTilemaps;    
    public LineRenderer lineRenderer;
    public Image[] rightMovementSprites;
    public TextMeshProUGUI[] rightMovementCosts;
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
    // ****************************************
    
    private CardUI selectedCardUIForMovement;

    private List<Vector3> positions = new List<Vector3>();
    private short totalMovement = 0;

    private TileAndMovementCost[] tiles;

    private Pathfinder<Vector3Int> pathfinder;

    private static Vector3Int NULL = Vector3Int.one * int.MinValue;
    
    private Vector3Int showingPathDestination = NULL;

    private CameraController cameraController;

    private List<Vector3Int> path;

    

    [System.Serializable]
    public struct TileAndMovementCost
    {
        public Vector3Int cellPosition;
        public bool movable;
        public float movementCost;
    }

    [Range(0.001f,1f)]
    public float stepTime;


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
            TileAndMovementCost tmc = tiles[destinationPos];
            if (tmc.movable)
                result.Add(destination, tmc.movementCost);
        }
        return result;
    }

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

        BoundsInt bounds = terrainTilemaps[0].cellBounds;
        TileBase[][] allTiles = new TileBase[terrainTilemaps.Length][];

        for (int i = 0; i < terrainTilemaps.Length; i++)
            allTiles[i] = terrainTilemaps[i].GetTilesBlock(bounds);

        tiles = new TileAndMovementCost[bounds.size.x*bounds.size.y];

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                bool found = false;

                for (int i = 0; i < terrainTilemaps.Length;i++)
                {
                    
                    Vector3Int tilePosition = new Vector3Int(x, y, -1);
                    Tile tile = (Tile) allTiles[i][(y - bounds.yMin) * bounds.size.x + (x - bounds.xMin)];
                    if (tile != null)
                    {
                        TerrainInfo terrainInfo = tile.gameObject.GetComponent<TerrainInfo>();
                        if (terrainInfo != null )
                        {
                            short movement = Terrains.movementCost[terrainInfo.terrainType];
                            TileAndMovementCost structValue = new TileAndMovementCost() { cellPosition = tilePosition, movable = true, movementCost = movement };

                            int pos = HexTranslator.GetNormalizedCellPosInt(new Vector3Int(x,y,0));
                            tiles[pos] = structValue;
                            found = true;
                            break;
                        }
                    }
                }
                
                if(!found)
                    throw new System.Exception("Tile " + x.ToString() + "," + y.ToString() + " does not have any sprite in any layer");
            }
        }
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
            Vector2Int hexSelected = board.GetSelectedHex();
            Vector3Int currentCellPos = new Vector3Int(hexSelected.x, hexSelected.y, 0);
            Vector3Int target = cellHover.last;

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

    public CardUI GetSelectedCardUIForMovement()
    {
        return selectedCardUIForMovement;
    }

    public void SetSelectedCardUIForMovement(CardUI cardUI)
    {
        selectedCardUIForMovement = cardUI;
    }


    IEnumerator MovePath()
    {
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

                bool exceeded = false;
                for (int i = 0; i < terrainTilemaps.Length; i++)
                {
                    Tile terrainTile = (Tile)terrainTilemaps[i].GetTile(targetCell);
                    if (terrainTile != null)
                    {
                        TerrainInfo terrainTileInfo = terrainTile.gameObject.GetComponent<TerrainInfo>();
                        if (terrainTileInfo != null)
                        {
                            TerrainsEnum terrainEnum = terrainTileInfo.terrainType;
                            short movement = Terrains.movementCost[terrainEnum];

                            if (selectedCardUIForMovement.GetCard().moved + movement > MovementConstants.characterMovement)
                            {
                                exceeded = true;
                                break;
                            }

                            selectedCardUIForMovement.GetCard().AddMovement(movement);
                            break;
                        }
                    }
                }

                if (exceeded)
                    break;

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

                CardInfo ci = ((Tile)cardTilemap.GetTile(targetCell)).gameObject.GetComponent<CardInfo>();
                if (ci != null)
                {
                    if (ci.cardType <= CardTypesEnum.SEA)
                    {
                        manaPool.AddMana(ci.cardType);
                    }
                }
                else
                {
                    Debug.LogError("No card info at tile " + HexTranslator.GetDebugTileInfo(targetCell));
                }
                manaPool.ToggleDirty();
            }

            // LAST TILE
            /*
            for (int i = 0; i < terrainTilemaps.Length; i++)
            {
                Tile terrainTile = (Tile)terrainTilemaps[i].GetTile(path[currentPointIndex]);
                if (terrainTile != null)
                {
                    TerrainInfo terrainTileInfo = terrainTile.gameObject.GetComponent<TerrainInfo>();
                    if (terrainTileInfo != null)
                    {
                        TerrainsEnum terrainEnum = terrainTileInfo.terrainType;
                        short movement = (currentPointIndex != 0) ? Terrains.movementCost[terrainEnum] : (short)0;

                        selectedCardUIForMovement.GetCard().AddMovement(movement);
                        break;
                    }
                }
            }*/

            // Ensure the GameObject stays at the last position in the path
            Vector3 destination = cardTilemap.CellToWorld(path[currentPointIndex]);
            selectedCardUIForMovement.gameObject.transform.position = new Vector3(destination.x, destination.y, 0);
            selectedCardUIForMovement.StopMoving();
            selectedItems.UnselectCardDetails();
            selectedCardUIForMovement.GetCard().hex = new Vector2Int(path[path.Count - 1].x, path[path.Count - 1].y);
        } else
        {
            Reset();
        }
    }

    IEnumerator RenderPath()
    {
        totalMovement = selectedCardUIForMovement.GetCard().moved;
        for (int p=0;p<path.Count;p++)
        {
            //Movement Tilemap
            Vector3Int cardTilePos = path[p];

            Vector3 cardCellCenter = cardTilemap.CellToWorld(cardTilePos);
            cardCellCenter = new Vector3(cardCellCenter.x, cardCellCenter.y, -1);
            positions.Add(cardCellCenter);

            //Terrain Tilemap
            Vector3Int terrainTilePos = cardTilePos;
            bool found = false;
            bool exceeded = false;
            for (int i = 0; i < terrainTilemaps.Length; i++)
            {
                Tile terrainTile = (Tile)terrainTilemaps[i].GetTile(terrainTilePos);
                if (terrainTile != null)
                {
                    TerrainInfo terrainTileInfo = terrainTile.gameObject.GetComponent<TerrainInfo>();
                    if (terrainTileInfo != null)
                    {
                        TerrainsEnum terrainEnum = terrainTileInfo.terrainType;
                        short movement = (p != 0) ? Terrains.movementCost[terrainEnum] : (short) 0;
                        found = true;
                        if (totalMovement + movement > MovementConstants.characterMovement)
                        {
                            exceeded = true;
                            break;
                        }
                        totalMovement += movement;
                        rightMovementCosts[positions.Count - 1].text = totalMovement.ToString();
                        rightMovementCosts[positions.Count - 1].enabled = true;                        
                        break;
                    }
                }
            }
            if (!found)
                Debug.LogError("Terrain not found for " + terrainTilePos);
            
            if (exceeded)
            {
                StopAllCoroutines();
                positions = positions.GetRange(0, p);
                path = path.GetRange(0, p);
                lineRenderer.positionCount = positions.Count;
                lineRenderer.SetPositions(positions.ToArray());
                break;
            }


            Tile cardTile = (Tile)cardTilemap.GetTile(cardTilePos);
            if (cardTile == null)
            {
                Reset();
                StopAllCoroutines();
                break;
            }
            CardInfo cardTileInfo = cardTile.gameObject.GetComponent<CardInfo>();
            if(cardTileInfo == null)
            {
                Reset();
                StopAllCoroutines();
                break;
            }

            Sprite cardTileSprite = cardTilemap.GetSprite(cardTilePos);
            Tile newTile = ScriptableObject.CreateInstance<Tile>();
            newTile.sprite = cardTileSprite;
            movementTilemap.SetTile(cardTilePos, newTile);

            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());

            // Right Canvas Trail
            Sprite spriteMovement = cardTileInfo.spriteMovement;
            if(spriteMovement == null)
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
            rightMovementSprites[positions.Count - 1].sprite = spriteMovement;
            rightMovementSprites[positions.Count - 1].enabled = true;

            
        }
        yield return new WaitForSeconds(stepTime);
    }
}

