using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private bool isDoor = false;
    [SerializeField]
    private bool isNPC = false;
    [SerializeField]
    private bool isCollectible = false;
    [SerializeField]
    private bool isChest = false;
    [SerializeField]
    private SmartTileMap tilemap;

    [SerializeField]
    private List<Interaction> interactions;
    [SerializeField] 
    private List<GameObject> inventory;

    // Description
    [SerializeField]
    private string description = null;
    private string defaultDescription = "Something";

    private void Start()
    {
        inventory = new List<GameObject>();
        if (isCollectible)
        {
            defaultDescription = "You can pick this thing up!";
            name = "collectible_" + name;
        }
        if (isDoor)
        {
            defaultDescription = "A door!";
            name = "door_" + name;
        }
        if (isNPC)
        {
            defaultDescription = "A person!";
            name = "npc_" + name;
        }
        if (isChest)
        {
            defaultDescription = "A chest!";
            name = "chest_" + name;
        }
        if (tilemap == null)
        {
            tilemap = (FindObjectOfType<GridManager>() as GridManager).GetWalkableTileMap();

            // In case the item is placed on the Collision map instead
            if (tilemap == null)
            {
                tilemap = (FindObjectOfType<GridManager>() as GridManager).GetCollisionTileMap();
            }
        }

        var tile = tilemap.GetTile(transform.position);
        Debug.Log(name + " is now being added to the inventory of " + tile.ToString());
        tile.AddInventory(gameObject);
    }

    internal void Interact()
    {
        Debug.Log(name + " interacted with!");
    }

    internal bool IsDoor()
    {
        return isDoor;
    }

    internal bool IsNPC()
    {
        return isNPC;
    }

    internal bool IsCollectible()
    {
        return isCollectible;
    }

    internal bool IsChest()
    {
        return isChest;
    }

    internal void SetDefaultNPC()
    {
        this.isNPC = true;
        // Set some standard dialog options
    }

    internal void ClearInventory()
    {
        this.inventory = new List<GameObject>();
    }

    internal void AddInventory(GameObject obj)
    {
        if (this.inventory == null) 
        {
            this.inventory = new List<GameObject>();
        }
        this.inventory.Add(obj);
    }

    public string GetDescription()
    {
        if (description != null)
        {
            return description;
        }

        return defaultDescription;
    }
}
