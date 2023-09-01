using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    protected override void InteractableStart()
    {
        base.InteractableStart();

        AddInventory(InventoryItemFactory.MakeRandomGold(100));
        defaultDescription = "A chest!";
        name = "chest_" + name;
    }
    public override bool IsChest()
    {
        return true;
    }
}
