using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : Interactable
{
    protected override void InteractableStart()
    {
        base.InteractableStart();

        defaultDescription = "You can pick this thing up!";
        name = "collectible_" + name;
    }

    public override bool IsCollectible()
    {
        return true;
    }
}
