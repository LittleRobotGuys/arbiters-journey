using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem
{
    private int width;
    private int height;
    private GridObject[,] gridObjects;
    private float defaultCellSize = .25f;
    private float cellSize = 1;
    private Vector2 gridSize;

    public GridSystem(Vector2 size) : this((int)size.x, (int)size.y, 0) { }

    public GridSystem(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize > 0 ? cellSize : defaultCellSize;

        gridObjects = new GridObject[width, height];
        for(int x =0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 gridPosition = new Vector2(x, y);
                gridObjects[x, y] = new GridObject(gridPosition, this);
            }
        }
    }

    /**
     * Testing and Debug
     */
    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(x, y), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(x,y));
            }
        }

        Vector2 a = new Vector2(1f, 2f);
        Vector2 b = new Vector2(1f, 2f);
        Vector2 c = new Vector2(2f, 2f);
        Debug.Log("Vector2 equality and inequalities:\n" + a + " == " + b + "? " + (a == b) + "\n" + b + " != " + c + "? " + (b != c));
    }


    /**
     * Grid Objects
     */
    internal void ClearCreatures(Vector2 gridPosition)
    {
        GridObject gridObject = GetGridObject(gridPosition);
        foreach (Creature c in gridObject.GetCreatures())
        {
            GameObject.Destroy(c);
        }
    }

    public GridObject GetGridObject(int x, int y)
    {
        return gridObjects[x, y];
    }

    public GridObject GetGridObject(Vector2 gridPosition)
    {
        return GetGridObject((int)gridPosition.x, (int)gridPosition.y);
    }

    public Vector3 GetGridPosition(int x, int y)
    {
        return GetGridPosition(new Vector2(x, y));
    }
    
    /**
     * Grid Position
     */
    public Vector3 GetGridPosition(Vector3 grid3Pos)
    {
        return GetGridPosition(new Vector2(grid3Pos.x, grid3Pos.y));
    }

    public Vector3 GetGridPosition(Vector2 worldPos)
    {
        return new Vector2(
            Mathf.RoundToInt(worldPos.x / cellSize),
            Mathf.RoundToInt(worldPos.y / cellSize));
    }

    /**
     * World Position
     */
    public Vector3 GetWorldPosition(int x, int y)
    {
        return GetWorldPosition(new Vector2(x, y));
    }

    public Vector3 GetWorldPosition(Vector2 gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y) * cellSize;
    }
}
