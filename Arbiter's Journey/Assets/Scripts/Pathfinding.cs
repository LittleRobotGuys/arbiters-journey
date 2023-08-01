using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding
{
    private PathNode pathNode;
   
}


public class PathNode
{
    private Grid grid;
    private int x;
    private int y;


    public PathNode(Grid grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y; 
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}