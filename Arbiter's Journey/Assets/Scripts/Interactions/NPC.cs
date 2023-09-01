using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable
{
    protected override void InteractableStart()
    {
        base.InteractableStart();

        defaultDescription = "An NPC!";
        name = "npc_" + name;
    }
    public override bool IsNPC()
    {
        return true;
    }
}
