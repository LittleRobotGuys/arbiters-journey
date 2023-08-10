using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SmartTileMap : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private int tilemapCost;

    [SerializeField]
    private Dictionary<string, SmartTile> allTiles;

    public Vector2Int size { get; internal set; }


    // Start is called before the first frame update
    void Awake()
    {
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
        }    

        allTiles = new Dictionary<string, SmartTile>();
        TileBase[] tileArray = tilemap.GetTilesBlock(tilemap.cellBounds);

        Vector3Int cellCurrent = new Vector3Int(tilemap.cellBounds.xMin, tilemap.cellBounds.yMin, tilemap.cellBounds.zMin);
        

        StringBuilder sb = new StringBuilder();
        for (int k = 0; k < tileArray.Length; k++)
        {
            // Skip null tiles
            if (tileArray[k] == null)
            {
                cellCurrent = UpdateCurrentCell(cellCurrent);
                continue;
            }
            // Transform each tile to a smart tile
            SmartTile tile = new SmartTile(this, (Tile)tileArray[k], cellCurrent);
            tile.cost = tilemapCost;
            allTiles.Add(tile.x + "," + tile.y, tile);
            sb.Append(tile.ToString() + ", ");

            cellCurrent = UpdateCurrentCell(cellCurrent);
        }

        Debug.Log(name + "'s tiles:\n\n" + sb.ToString());
        tilemap.CompressBounds();
    }

    private Vector3Int UpdateCurrentCell(Vector3Int cellCurrent)
    {
        cellCurrent.x++;
        if (cellCurrent.x >= tilemap.cellBounds.xMax)
        {
            cellCurrent.x = tilemap.cellBounds.xMin;
            cellCurrent.y++;
        }
        if (cellCurrent.y >= tilemap.cellBounds.yMax)
        {
            cellCurrent.y = tilemap.cellBounds.yMin;
        }
        // cellCurrent.z++;
        // if (cellCurrent.z >= tilemap.cellBounds.zMax)
        // {
        //     cellCurrent.z = tilemap.cellBounds.zMin;
        // }

        return cellCurrent;
    }

    public SmartTile GetTile(Vector3Int position)
    {
        string key = position.x + "," + position.y;
        
        if (allTiles.ContainsKey(key))
        {
            return allTiles[key];
        }
        
        return null;
    }

    // Needs tested
    public PathNode GetPathNode(Vector2Int vector2Int)
    {
        Tile tile = (Tile) tilemap.GetTile(new Vector3Int(vector2Int.x, vector2Int.y, 0));
        return tile.gameObject.GetComponent<PathNode>();
    }

    public bool HasTile(Vector3Int vector2Int)
    {
        return tilemap.HasTile(new Vector3Int(vector2Int.x, vector2Int.y, (int) transform.position.z));
    }

    internal BoundsInt GetCellBounds()
    {
        return this.tilemap.cellBounds;
    }

    internal Vector3Int WorldToCell(Vector3 world)
    {
        return this.tilemap.WorldToCell(world);
    }

    internal TileBase[] GetTilesBlock(BoundsInt boundsInt)
    {
        return tilemap.GetTilesBlock(boundsInt);
    }

    internal List<SmartTile> GetAllTiles()
    {
        return allTiles.Values.ToList<SmartTile>();
    }

    internal List<string> GetAllTilesKeys()
    {
        return allTiles.Keys.ToList<string>();
    }

    internal void Remove(string t)
    {
        allTiles.Remove(t);
    }

    internal SmartTile GetTile(float x, float y)
    {
        return allTiles.GetValueOrDefault((int)x + "," + (int)y);
    }

    internal bool ContainsTileAt(Vector3Int target)
    {
        return GetTile(target) != null;
    }
}
