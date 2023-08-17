using System;
using System.Collections.Generic;
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
    private List<GameObject> inventory;

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
        // Debug.Log("Tile in " + tilemap.name + " created: " + x + ',' + y);
    }

    public bool hasPathNode()
    {
        return pathNode != null;
    }

    public PathNode getPreviousPathNode()
    {
        return pathNode.GetPreviousNode();
    }

    internal PathNode GetPathNode()
    {
        if (pathNode == null)
        {
            pathNode = new PathNode(this, this.tilemap);
        }

        return pathNode;
    }

    internal Interactable GetObjectOnIt()
    {
        if (inventory == null) return null;
        foreach (GameObject go in inventory)
        {
            if (go != null) 
            {
                return go.GetComponent<Interactable>();
            }
        }
        return null;
    }

    internal void ClearInventory()
    {
        this.inventory = new List<GameObject>();
    }

    internal void AddInventory(GameObject obj)
    {
        if (inventory == null)
        {
            inventory = new List<GameObject>();
        }
        this.inventory.Add(obj);
    }

    internal TileBase GetTile()
    {
        return tile;
    }
}
