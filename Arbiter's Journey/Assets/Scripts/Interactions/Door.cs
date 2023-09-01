using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    protected override void InteractableStart()
    {
        base.InteractableStart();

        defaultDescription = "A door!";
        name = "door_" + name;
    }
    public override bool IsDoor()
    {
        return true;
    }
}
