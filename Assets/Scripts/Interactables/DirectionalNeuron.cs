using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FieldOfView3D))]
public class DirectionalNeuron : Neuron
{
    FieldOfView3D fieldOfView3D;

    protected override void Start()
    {
        base.Start();

        //get reference to field of view
        fieldOfView3D = GetComponent<FieldOfView3D>();
    }

    protected override bool CanAttach(Interactable interactable)
    {
        //check if interactable is inside our area of vision
        bool isInsideAreaOfVision = false;
        foreach(Transform target in fieldOfView3D.VisibleTargets)
        {
            //foreach target (collider transform) check if parent (where there is script Interactable) is the same of interactable parameter
            Interactable i = target.GetComponentInParent<Interactable>();
            if(i != null && i == interactable)
            {
                isInsideAreaOfVision = true;
                break;
            }
        }

        //check can attach only if interactable is inside our area of vision
        if(isInsideAreaOfVision)
        {
            return base.CanAttach(interactable);
        }

        return false;
    }
}
