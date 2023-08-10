using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PathNode
{
    private SmartTileMap tilemap;
    private Vector2Int location;
    private int movementCost = 10;

    public int gCost { get; set; } = int.MaxValue;
    public int hCost { get; set; }
    public int fCost  { get; set; }

    private PathNode prevNode = null;


    public Vector2Int GetLocation()
    {
        return location;
    }    

    public PathNode(SmartTile tile, SmartTileMap tilemap)
    {
        this.tilemap = tilemap;
        location = new Vector2Int(tile.x, tile.y);
        movementCost = tile.cost;
    }

    public override string ToString()
    {
        return location.x + "," + location.y;
    }

    internal void CalcFCost()
    {
        fCost = gCost + hCost;
    }

    internal void SetPreviousNode(PathNode node) 
    { 
        this.prevNode = node; 
    }

    internal int GetMoveCost()
    {
        return movementCost;
    }

    public List<PathNode> GetNeighbors()
    {
        List<PathNode> returnNodes = new List<PathNode>();

        // Left
        SmartTile botleft = tilemap.GetTile(location.x - 1, location.y - 1);
        if (botleft != null) returnNodes.Add(botleft.GetPathNode());
        SmartTile midleft = tilemap.GetTile(location.x - 1, location.y);
        if (midleft != null) returnNodes.Add(midleft.GetPathNode());
        SmartTile topleft = tilemap.GetTile(location.x - 1, location.y + 1);
        if (topleft != null) returnNodes.Add(topleft.GetPathNode());

        // Top and Bottom
        SmartTile top = tilemap.GetTile(location.x, location.y - 1);
        if (top != null) returnNodes.Add(top.GetPathNode());
        SmartTile bot = tilemap.GetTile(location.x, location.y + 1);
        if (bot != null) returnNodes.Add(bot.GetPathNode());

        // Right
        SmartTile botright = tilemap.GetTile(location.x + 1, location.y - 1);
        if (botright != null) returnNodes.Add(botright.GetPathNode());
        SmartTile midright = tilemap.GetTile(location.x + 1, location.y);
        if (midright != null) returnNodes.Add(midright.GetPathNode());
        SmartTile topright = tilemap.GetTile(location.x + 1, location.y + 1);
        if (topright != null) returnNodes.Add(topright.GetPathNode());

        return returnNodes;
    }

    internal PathNode getPreviousNode()
    {
        return prevNode;
    }

    internal Vector3 GetLocationAsV3()
    {
        return new Vector3(this.location.x, this.location.y, 0);
    }
}
