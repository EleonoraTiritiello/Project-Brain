using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorNeuron : Neuron
{
    [Header("Activator")]
    public List<Activable> ObjectsToActivate = new List<Activable>();

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //draw a line to every object to activate
        foreach (Activable activable in ObjectsToActivate)
            if (activable)
                Gizmos.DrawLine(activable.ObjectToControl.transform.position, ObjectToControl.transform.position);
    }

    public override void ActiveInteractable(bool active, Interactable interactable)
    {
        base.ActiveInteractable(active, interactable);

        //foreach object in the list, try activate or deactivate
        foreach (Activable activable in ObjectsToActivate)
            if (activable)
                activable.ToggleObject(this, active);
    }
}
