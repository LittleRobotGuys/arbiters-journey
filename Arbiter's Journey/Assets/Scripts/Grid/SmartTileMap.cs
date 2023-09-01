using DebugUtils;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class SmartTileMap : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private int tilemapCost;

    [SerializeField]
    private Dictionary<string, SmartTile> allTiles;
    private Vector3 origin = Vector3.zero;

    // Number of full screens away from origin (0,0).  So a cameraOffset of (2,-3) is two full screens right, 3 sceens up from origin. 
    private Vector3 cameraOffset = Vector3.zero;
    private Vector2Int tileRadius = new Vector2Int(15,8);

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
            DebugDraw.X(new Vector2Int(tile.x, tile.y), Color.green);
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

    // Uses cameraOffset.  All QOL versions should refer to this method.
    public SmartTile GetTile(float x, float y)
    {
        return allTiles.GetValueOrDefault(((int)x + cameraOffset.x) + "," + ((int)y + cameraOffset.y));
    }

    public SmartTile GetTile(Vector3 position)
    {
        return GetTile((int)position.x, (int)position.y);
    }
    public SmartTile GetTile(Vector3Int position)
    {
        return GetTile(position.x, position.y);
    }

    // Ignores cameraOffset.  Useful for direct input.
    public SmartTile GetTileSansOffset(float x, float y)
    {
        return allTiles.GetValueOrDefault((int)x + "," + (int)y);
    }

    internal SmartTile GetTileSansOffset(Vector3Int gridPos)
    {
        return GetTileSansOffset(gridPos.x, gridPos.y);
    }



    public bool ContainsTileAt(Vector3Int target)
    {
        return GetTile(target) != null;
    }

    public void MoveCameraEast(Creature character)
    {
        cameraOffset.x++;
        Camera.main.transform.DOMoveX(cameraOffset.x * tileRadius.x * 2, 1f, false).OnComplete(() => character.UpdateCreatureTile());
        character.transform.DOMoveX(character.transform.position.x + 3f, 1f, false).OnComplete(() => character.UpdateCreatureTile());
    }

    public void MoveCameraWest(Creature character)
    {
        cameraOffset.x--;
        Camera.main.transform.DOMoveX(cameraOffset.x * tileRadius.x * -2, 1f, false).OnComplete(() => character.UpdateCreatureTile());
        character.transform.DOMoveX(character.transform.position.x - 3f, 1f, false).OnComplete(() => character.UpdateCreatureTile());
    }

    public void MoveCameraNorth(Creature character)
    {
        cameraOffset.y++;
    }

    public void MoveCameraSouth(Creature character)
    {
        cameraOffset.y--;
    }

    internal Vector3 GetOrigin()
    {
        return origin;
    }

    internal Vector3 GetOffset()
    {
        return cameraOffset;
    }

    internal int GetXRadius()
    {
        return this.tileRadius.x;
    }
}
