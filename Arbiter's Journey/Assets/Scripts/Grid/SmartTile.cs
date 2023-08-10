using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SmartTile
{
    public SmartTileMap tilemap;
    public int x, y, z;
    public Tile tile;
    public int cost;
    public string name;
    private PathNode pathNode = null;

    public override string ToString()
    {
        return "(" + x + "," + y + ")";
    }

    public SmartTile(SmartTileMap tilemap, Tile tileBase, Vector3 position)
    {
        this.tilemap = tilemap;
        this.tile = tileBase;
        this.name = tileBase.name;

        x = tilemap.WorldToCell(position).x;
        y = tilemap.WorldToCell(position).y;
        z = tilemap.WorldToCell (position).z;
    }

    public bool hasPathNode()
    {
        return pathNode != null;
    }

    public PathNode getPreviousPathNode()
    {
        return pathNode.getPreviousNode();
    }

    internal PathNode GetPathNode()
    {
        if (pathNode == null)
        {
            pathNode = new PathNode(this, this.tilemap);
        }

        return pathNode;
    }
}
