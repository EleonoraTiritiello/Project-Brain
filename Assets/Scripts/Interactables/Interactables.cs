using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField] LineRenderer ropePrefab = default;
    public float ropeLength = 10;

    protected bool isActive;
    protected Interactable alreadyAttached;

    LineRenderer rope;

    /// <summary>
    /// Create rope from line prefab
    /// </summary>
    public virtual bool CreateRope()
    {
        //check if can create
        if (CanCreateRope())
        {
            //if there isn't a rope (and there is a prefab)
            if (rope == null && ropePrefab != null)
            {
                //instantiate rope and set positions number
                rope = Instantiate(ropePrefab, transform);
                rope.positionCount = 2;

                //set first position
                rope.SetPosition(0, transform.position);
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Set new position for the rope
    /// </summary>
    public void UpdateRope(Vector3 position)
    {
        rope.SetPosition(1, position);
    }

    /// <summary>
    /// Attach rope to something
    /// </summary>
    public bool AttachRope(Interactable interactable)
    {
        //check if can attach
        if (CanAttach(interactable))
        {
            //active new interactable, and set this already attached
            interactable.isActive = true;
            alreadyAttached = interactable;

            //set rope position
            rope.SetPosition(1, interactable.transform.position);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Detach rope from this interactable
    /// </summary>
    public bool DetachRope()
    {
        //check if can detach
        if(CanDetachRope())
        {
            //our attached interactable is no more active, and this is not attached to nothing
            alreadyAttached.isActive = false;
            alreadyAttached = null;

            return true;
        }

        return false;
    }

    #region private API

    bool CanCreateRope()
    {
        bool thisIsActive = isActive;                               //be sure this interactable is active
        bool thisIsNotAlreadyAttached = alreadyAttached == null;    //be sure is not attached to something

        //return if can create rope
        return thisIsActive && thisIsNotAlreadyAttached;
    }

    bool CanAttach(Interactable interactable)
    {
        bool isNotItSelf = interactable != this;            //check if attach to another interactable and not itself
        bool IsNotActive = interactable.isActive == false;  //check if interactable is not already active

        //return if can attach
        return isNotItSelf && IsNotActive;
    }

    bool CanDetachRope()
    {
        bool isAttachedToSomething = alreadyAttached != null;       //be sure is already attached to something
        if(isAttachedToSomething)
        {
            bool isNotAttachedAgain = alreadyAttached.alreadyAttached == null;  //be sure our attached interactable is not attached to another thing

            //return if can detach
            return isNotAttachedAgain;
        }

        return false;
    }

    #endregion
}
