using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : Interactable
{
    RoomGame roomParent;

    private void Start()
    {
        //generator is active by default
        ActiveInteractable(true, null);

        //save room parent
        roomParent = GetComponentInParent<RoomGame>();
    }

    protected override bool CanBeAttach(Interactable interactable)
    {
        //can't attach to generator
        return false;
    }

    public override bool CreateRope()
    {
        bool pickRopeFromGenerator = base.CreateRope();

        //when pick rope, force closing enter door
        if(pickRopeFromGenerator)
        {
            roomParent.ForceOpenCloseEnterDoor(false);
        }

        return pickRopeFromGenerator;
    }
}
