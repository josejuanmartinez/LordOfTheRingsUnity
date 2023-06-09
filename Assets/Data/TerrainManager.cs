using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public struct TileAndMovementCost
{
    public Vector3Int cellPosition;
    public TerrainInfo terrain;
    public CardInfo cardInfo;
    public bool movable;
    public float movementCost;
}

public class TerrainManager : MonoBehaviour
{
    public Tilemap[] terrainTilemaps;
    public Tilemap cardTilemap;
    public Tilemap movementTilemap;

    private TileAndMovementCost[] tiles;
    public void Awake()
    {
        BoundsInt bounds = terrainTilemaps[0].cellBounds;
        TileBase[][] allTiles = new TileBase[terrainTilemaps.Length][];

        for (int i = 0; i < terrainTilemaps.Length; i++)
            allTiles[i] = terrainTilemaps[i].GetTilesBlock(bounds);

        tiles = new TileAndMovementCost[bounds.size.x * bounds.size.y];

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                bool found = false;

                for (int i = 0; i < terrainTilemaps.Length; i++)
                {

                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    Tile tile = (Tile)allTiles[i][(y - bounds.yMin) * bounds.size.x + (x - bounds.xMin)];
                    if (tile != null)
                    {
                        TerrainInfo terrainInfo = tile.gameObject.GetComponent<TerrainInfo>();
                        if (terrainInfo != null)
                        {
                            CardInfo cardInfo = GetCardInfo(tilePosition);
                            if(cardInfo != null)
                            {
                                short movement = Terrains.movementCost[terrainInfo.terrainType];
                                TileAndMovementCost structValue = new TileAndMovementCost() { cellPosition = tilePosition, terrain = terrainInfo, cardInfo = cardInfo, movable = true, movementCost = movement };

                                int pos = HexTranslator.GetNormalizedCellPosInt(new Vector3Int(x, y, 0));
                                tiles[pos] = structValue;
                                found = true;
                                break;
                            }                            
                        }
                    }
                }

                if (!found)
                    Debug.Log("Tile " + x.ToString() + "," + y.ToString() + " does not have any sprite in any layer");
            }
        }
    }
    public TileAndMovementCost GetTileAndMovementCost(int tileNum)
    {
        return tiles[tileNum];
    }

    public TileAndMovementCost GetTileAndMovementCost(Vector2Int tile)
    {
        int pos = HexTranslator.GetNormalizedCellPosInt(new Vector3Int(tile.x, tile.y, 0));
        return GetTileAndMovementCost(pos);
    }

    public short GetMovementCost(Vector3Int targetCell)
    {
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
                    return movement;
                }
            }
        }
        return 0;
    }

    public Sprite GetSpriteMovement(Vector3Int cardTilePos)
    {
        Tile cardTile = (Tile)cardTilemap.GetTile(cardTilePos);
        if (cardTile == null)
            return null;

        CardInfo cardTileInfo = GetCardInfo(cardTilePos);
        if (cardTileInfo == null)
            return null;

        Sprite cardTileSprite = cardTilemap.GetSprite(cardTilePos);
        Tile newTile = ScriptableObject.CreateInstance<Tile>();
        newTile.sprite = cardTileSprite;
        movementTilemap.SetTile(cardTilePos, newTile);

        // Right Canvas Trail
        return cardTileInfo.spriteMovement;
    }

    public CardInfo GetCardInfo(Vector3Int targetCell)
    {
        Tile tile = ((Tile)cardTilemap.GetTile(targetCell));
        if (tile == null)
        {
            Debug.LogError("Unable to get cell tile at " + targetCell);
            return null;
        }
        GameObject go = tile.gameObject;
        if( go == null)
        {
            Debug.LogError("Unable to get cell tile gameobject at " + targetCell);
            return null;
        }
        if(go.GetComponent<CardInfo>() == null)
        {
            Debug.LogError("Unable to get CardInfo from tile at " + targetCell);
            return null;
        }
        return go.GetComponent<CardInfo>();
    }
}
