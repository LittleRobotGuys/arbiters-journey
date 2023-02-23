using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{
    private List<Creature> creatures;
    private TerrainObject terrain;
    private Vector2 gridPosition;
    private GridSystem gridSystem;

    public GridObject(Vector2 gridPosition, GridSystem gridSystem)
    {
        this.gridPosition = gridPosition;
        this.gridSystem = gridSystem;
        this.creatures = new List<Creature>();
    }

    public override string ToString()
    {
        return "(" + gridPosition.x + "," + gridPosition.y + ")\n";
    }

    public void AddCreature(Creature creature)
    {
        this.creatures.Add(creature);
    }

    public List<Creature> GetCreatures()
    {
        return this.creatures;
    }
}
