using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private SmartTileMap tilemap;

    [SerializeField]
    private List<Interaction> interactions;
    [SerializeField] 
    private List<InventoryItem> inventory;

    protected AudioSource interactableAudio;

    // Description
    [SerializeField]
    protected string description = null;
    protected string defaultDescription = "Something";

    private void Start()
    {
        InteractableStart();
    }

    protected virtual void InteractableStart()
    {
        inventory = new List<InventoryItem>();
        
        if (tilemap == null)
        {
            tilemap = (FindObjectOfType<GridManager>() as GridManager).GetWalkableTileMap();

            // In case the item is placed on the Collision map instead
            if (tilemap == null)
            {
                tilemap = (FindObjectOfType<GridManager>() as GridManager).GetCollisionTileMap();
            }
        }

        if (interactableAudio == null)
        {
            interactableAudio = gameObject.GetComponent<AudioSource>();
        }

        var tile = tilemap.GetTile(transform.position);
        tile.AddInventory(gameObject);
    }

    internal void Interact()
    {
        Debug.Log(name + " interacted with!");
        PlayAudioDefault();
    }

    protected void PlayAudioDefault()
    {
        if (interactableAudio != null) interactableAudio.Play();
    }

    public virtual bool IsDoor()
    {
        return false;
    }

    public virtual bool IsNPC()
    {
        return false;
    }

    public virtual bool IsCollectible()
    {
        return false;
    }

    public virtual bool IsChest()
    {
        return false;
    }

    internal void ClearInventory()
    {
        this.inventory = new List<InventoryItem>();
    }

    internal void AddInventory(InventoryItem obj)
    {
        if (this.inventory == null) 
        {
            this.inventory = new List<InventoryItem>();
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
