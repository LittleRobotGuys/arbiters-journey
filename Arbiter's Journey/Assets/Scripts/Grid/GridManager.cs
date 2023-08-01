using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap obstacleMap;
    public Vector3Int[,] Locations;
    Astar astar;
    List<Location> roadPath = new List<Location>();
    new Camera camera;
    BoundsInt bounds;
    private Movement movementControls;
    [SerializeField]
    private List<Creature> creatureList;

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
        tilemap.CompressBounds();
        obstacleMap.CompressBounds();
        bounds = tilemap.cellBounds;
        camera = Camera.main;


        CreateMovementGrid();
        astar = new Astar(Locations, bounds.size.x, bounds.size.y);

        movementControls.MovementMap.Movement.performed += ctx => Move(ctx.ReadValue<float>());
    }

    private void Move(float direction)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 world = camera.ScreenToWorldPoint(mousePos);
        Vector3Int gridPos = tilemap.WorldToCell(world);
        target = new Vector2Int(gridPos.x, gridPos.y);

        foreach (Creature creature in creatureList)
        {
            creature.Move(target);
        }

        Debug.Log("Grid Position: " + target.x + ", " + target.y);
    }

    internal Tilemap GetCollisionTileMap()
    {
        return obstacleMap;
    }

    internal Tilemap GetWalkableTileMap()
    {
        return tilemap;
    }

    public void CreateMovementGrid()
    {
        Locations = new Vector3Int[bounds.size.x, bounds.size.y];
        for (int x = bounds.xMin, i = 0; i < (bounds.size.x); x++, i++)
        {
            for (int y = bounds.yMin, j = 0; j < (bounds.size.y); y++, j++)
            {
                if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    Locations[i, j] = new Vector3Int(x, y, 0);
                }
                else
                {
                    Locations[i, j] = new Vector3Int(x, y, 1);
                }
            }
        }
    }
 
    public Vector2Int target;
    void Update()
    {
        Vector3 world = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3Int gridPos = tilemap.WorldToCell(world);

        if (roadPath != null && roadPath.Count > 0)
            roadPath.Clear();

        roadPath = astar.CreatePath(Locations, target, new Vector2Int(gridPos.x, gridPos.y), 1000);
        if (roadPath == null)
            return;

        target = new Vector2Int(roadPath[0].X, roadPath[0].Y);
    }

}
