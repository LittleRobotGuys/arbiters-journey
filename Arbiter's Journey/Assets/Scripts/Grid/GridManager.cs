using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using DebugUtils;
using UnityEditor;

public class GridManager : MonoBehaviour
{
    public SmartTileMap terrainMap;
    public SmartTileMap obstacleMap;
    new Camera camera;
    private Movement movementControls;
    [SerializeField]
    private List<Creature> creatureList;

    public Vector2Int mouseTarget;

    [SerializeField]
    private Creature debugCreature;

    private void Awake()
    {
        movementControls = new Movement();
    }

    private void OnEnable()
    {
        movementControls.Enable();
    }

    private void OnDisable()
    {
        movementControls.Disable();
    }
    void Start()
    {
        if (terrainMap == null || obstacleMap == null)
        {
            Debug.LogError("Unable to initialize GridManager - Need to assign SmartTileMaps.  Did you break the prefab?");
        }
        camera = Camera.main;

        RemoveTerrainMapTilesDueToObstacles();

        movementControls.MovementMap.Movement.performed += _ => GetMouseTile();
    }

    private void RemoveTerrainMapTilesDueToObstacles()
    {
        List<string> obstacleKeyList = obstacleMap.GetAllTilesKeys();
        foreach (string key in terrainMap.GetAllTilesKeys())
        {
            if (obstacleKeyList.Contains(key))
            {
                terrainMap.Remove(key);
            }
        }
    }

    private void GetMouseTile()
    {
        if (terrainMap == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 world = camera.ScreenToWorldPoint(mousePos);
        Vector3Int gridPos = terrainMap.WorldToCell(world);

        foreach (Creature c in creatureList)
        {
            if (c.IsSelected()) c.TileSelected(terrainMap.GetTileSansOffset(gridPos));
        }
    }

    internal SmartTileMap GetCollisionTileMap()
    {
        return obstacleMap;
    }

    internal SmartTileMap GetWalkableTileMap()
    {
        return terrainMap;
    }

    internal void LazyAddCreature(Creature creature)
    {
        if (this.creatureList.Contains(creature)) return;
        
        this.creatureList.Add(creature);
    }

    // Use this to find a path between two tiles.  The player's Pathfinding can be passed in if needed;
    // the reason there's a pathfinding var is to be able to use different logic if needed (say, for a flyer).
    private List<PathNode> GetPath(SmartTile tile, Pathfinding p, Vector3Int target)
    {
        List<PathNode> path = p.FindPath(tile, terrainMap.GetTile(target));
        if (path != null)
            p.ClearPathNodes(path);

        return path;
    }
}
