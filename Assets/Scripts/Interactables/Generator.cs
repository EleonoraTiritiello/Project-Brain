using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : Interactable
{
    private void Start()
    {
        //generator is active by default
        ActiveInteractable(true, null);
    }

    protected override bool CanBeAttach(Interactable interactable)
    {
        //can't attach to generator
        return false;
    }
}
