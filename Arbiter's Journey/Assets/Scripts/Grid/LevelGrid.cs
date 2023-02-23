using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    [SerializeField]
    private Transform gridDebugObjectPrefab;


    private GridSystem gridSystem;

    [SerializeField]
    private Vector2 gridSize = new Vector2(10,10);

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("More than one LevelGrid active!");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystem = new GridSystem(gridSize);
    }

    public void AddCreatureAtGridPosition(Vector2 gridPosition, Creature creature)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddCreature(creature);
    }

    public List<Creature> GetCreatureListAtGridPosition(Vector2 gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).GetCreatures();
    }

    public void ClearCreatureAtGridPosition(Vector2 gridPosition)
    {
        gridSystem.ClearCreatures(gridPosition);
    }
    public Vector2 GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
}
