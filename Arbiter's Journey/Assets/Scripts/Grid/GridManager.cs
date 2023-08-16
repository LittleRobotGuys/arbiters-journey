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

    [SerializeField]
    private bool DrawingDebugPaths = true;

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
            if (c.IsSelected()) c.MouseClicked(terrainMap.GetTileSansOffset(gridPos));
        }

        if (DrawingDebugPaths)
        {
            DebugDraw.X(new Vector2Int(gridPos.x, gridPos.y));
            DebugPath(debugCreature, gridPos);
        }
    }

    private void DebugPath(Creature debugCreature, Vector3Int target)
    {
        if (debugCreature == null)
        {
            Debug.LogError("Creature is null");
            return;
        }
        if (!terrainMap.ContainsTileAt(target))
        {
            Debug.LogError("Target is not in the terrainMap");
        }
        if (obstacleMap.ContainsTileAt(target))
        {
            Debug.LogError("Target is in the obstacleMap");
            return;
        }

        Pathfinding p = debugCreature.GetPathfinding();

        List<PathNode> debugPath = GetPath(debugCreature, p, target);

        if (debugPath != null)
        {
            foreach (PathNode pathNode in debugPath)
            {
                DebugDraw.X(pathNode.GetLocation());
            }
        }

        else
        {
            Debug.Log("No path found to " + target.ToString());
        }
    }

    private List<PathNode> GetPath(Creature c, Pathfinding p, Vector3Int target)
    {
        List<PathNode> path = p.FindPath(c.GetTile(), terrainMap.GetTile(target));
        if (path != null)
            p.ClearPathNodes(path);

        return path;
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
}
