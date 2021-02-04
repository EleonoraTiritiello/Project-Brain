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
    public bool CreateRope()
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

            //hide rope by default (will be updated by player)
            UpdateRope(rope.GetPosition(0));

            return true;
        }

        return false;
    }

    /// <summary>
    /// Rewind rope from every interactable to this one
    /// </summary>
    public bool RewindRope()
    {
        //check if can rewind
        if(CanRewindRope())
        {
            List<Interactable> everyAttachedThings = new List<Interactable>();
            Interactable lastAttached = this;

            //while there is a thing attached
            while(lastAttached.alreadyAttached != null)
            {
                //add this interactable to the list, and go to next thing attached
                everyAttachedThings.Add(lastAttached);
                lastAttached = lastAttached.alreadyAttached;
            }

            //reverse detach from last one to this interactable
            for(int i = everyAttachedThings.Count -1; i >= 0; i--)
            {
                if(everyAttachedThings[i].DetachRope() == false)
                {
                    Debug.LogWarning("impossible to detach " + everyAttachedThings[i].name);
                }
            }

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
        bool isNotItSelf = interactable != this;                            //check if attach to another interactable and not itself
        bool IsNotAlreadyAttached = interactable.alreadyAttached == null;   //check if interactable is not already attached to something
        bool isNotGenerator = interactable is Generator == false;           //check if interactable is not a generator

        //return if can attach
        return isNotItSelf && IsNotAlreadyAttached && isNotGenerator;
    }

    bool CanDetachRope()
    {
        bool isAttachedToSomething = alreadyAttached != null;                   //be sure is already attached to something
        if(isAttachedToSomething)
        {
            bool isNotAttachedAgain = alreadyAttached.alreadyAttached == null;  //be sure our attached interactable is NOT attached to another thing

            //return if can detach
            return isNotAttachedAgain;
        }

        return false;
    }

    bool CanRewindRope()
    {
        bool isAttachedToSomething = alreadyAttached != null;                   //be sure is already attached to something
        if (isAttachedToSomething)
        {
            bool isAttachedAgain = alreadyAttached.alreadyAttached != null;     //be sure our attached interactable is attached to another thing

            //return if can detach
            return isAttachedAgain;
        }

        return false;
    }

    #endregion
}
