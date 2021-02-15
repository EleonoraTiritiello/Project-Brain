using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchNeuron : Neuron
{
    [Header("Switch")]
    [SerializeField] Activable[] objectsToActivate = default;

    //to determine if active or deactive objects in the list
    bool toggle = true;

    public override void ActiveInteractable(bool active, Interactable interactable)
    {
        base.ActiveInteractable(active, interactable);

        //do things only when active
        if (active)
        {
            //foreach object in the list, try activate or deactivate
            foreach (Activable activable in objectsToActivate)
                activable.ToggleObject(this, toggle);

            //invert toggle
            toggle = !toggle;
        }
    }
}
