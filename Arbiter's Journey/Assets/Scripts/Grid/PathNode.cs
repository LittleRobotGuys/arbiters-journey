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
        SmartTile botleft = tilemap.GetTileSansOffset(location.x - 1, location.y - 1);
        if (botleft != null && botleft.IsValidNeighbor()) returnNodes.Add(botleft.GetPathNode());
        SmartTile midleft = tilemap.GetTileSansOffset(location.x - 1, location.y);
        if (midleft != null && midleft.IsValidNeighbor()) returnNodes.Add(midleft.GetPathNode());
        SmartTile topleft = tilemap.GetTileSansOffset(location.x - 1, location.y + 1);
        if (topleft != null && topleft.IsValidNeighbor()) returnNodes.Add(topleft.GetPathNode());

        // Top and Bottom
        SmartTile top = tilemap.GetTileSansOffset(location.x, location.y - 1);
        if (top != null &&  top.IsValidNeighbor()) returnNodes.Add(top.GetPathNode());
        SmartTile bot = tilemap.GetTileSansOffset(location.x, location.y + 1);
        if (bot != null && bot.IsValidNeighbor()) returnNodes.Add(bot.GetPathNode());

        // Right
        SmartTile botright = tilemap.GetTileSansOffset(location.x + 1, location.y - 1);
        if (botright != null && botright.IsValidNeighbor()) returnNodes.Add(botright.GetPathNode());
        SmartTile midright = tilemap.GetTileSansOffset(location.x + 1, location.y);
        if (midright != null && midright.IsValidNeighbor()) returnNodes.Add(midright.GetPathNode());
        SmartTile topright = tilemap.GetTileSansOffset(location.x + 1, location.y + 1);
        if (topright != null && topright.IsValidNeighbor()) returnNodes.Add(topright.GetPathNode());

        return returnNodes;
    }

    internal PathNode GetPreviousNode()
    {
        return prevNode;
    }

    internal Vector3 GetLocationAsV3()
    {
        return new Vector3(this.location.x, this.location.y, 0);
    }
}
