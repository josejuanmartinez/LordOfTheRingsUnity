using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class HexNumGridCreator : MonoBehaviour
{
    public GameObject hexNum;
    
    void Awake()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        Transform worldSpaceCanvas = GameObject.Find("HexNums").transform;
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                TileBase tile = (TileBase) allTiles[(x - bounds.xMin) + (y - bounds.yMin) * bounds.size.x];
                Vector3Int tilePosition = new Vector3Int(x, y, 0);                
                GameObject go = Instantiate(hexNum, worldSpaceCanvas, true);
                Vector3 center = tilemap.GetCellCenterWorld(tilePosition);
                Vector3 cellSize = tilemap.cellSize;
                go.transform.position = new Vector3(center.x, center.y + (cellSize.y / 2), 0);
                go.GetComponent<TextMeshProUGUI>().text = x + "," + y;
            }
        }
    }
}
