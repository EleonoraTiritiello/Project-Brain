using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField] LineRenderer ropePrefab = default;
    public float ropeLength = 10;

    protected bool isActive;
    protected Interactable attachedTo;
    protected Interactable attachedFrom;

    List<BoxCollider> colliders = new List<BoxCollider>();

    LineRenderer rope;

    #region public API

    /// <summary>
    /// Active or Deactive interactable
    /// </summary>
    /// <param name="active">try activate object when true, or deactivate when false</param>
    public virtual void ActiveInteractable(bool active, Interactable interactable)
    {
        isActive = active;
        attachedFrom = interactable;
    }

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
                //instantiate rope
                rope = Instantiate(ropePrefab, transform);
            }
            else if (ropePrefab == null)
                Debug.LogWarning("Non è stato inserito il prefab della corda");

            return true;
        }

        return false;
    }

    /// <summary>
    /// Set new position for the rope
    /// </summary>
    public void UpdateRope(List<Vector3> positions)
    {
        rope.positionCount = positions.Count;
        rope.SetPositions(positions.ToArray());
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
            interactable.ActiveInteractable(true,this );
            attachedTo = interactable;

            //set last rope position
            rope.SetPosition(rope.positionCount -1, interactable.transform.position);

            return true;
        }

        return false;
    }

    public bool DetachRope(out Interactable interactable)
    {
        if (CanDetachRope())
        {
            interactable = attachedFrom;
            attachedFrom.DestroyCollider();
            attachedFrom = null;

            return true;
        }

        interactable = null;

        return false;
    }

    /// <summary>
    /// Detach rope from this interactable
    /// </summary>
    public bool DetachRewindRope()
    {
        //check if can detach
        if(CanDetachRewindRope())
        {
            //our attached interactable is no more active, and this is not attached to nothing
            attachedTo.ActiveInteractable(false, null);
            attachedTo = null;

            //hide rope by default (will be updated by player)
            rope.positionCount = 0;

            DestroyAllColliders();

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
            while(lastAttached.attachedTo != null)
            {
                //add this interactable to the list, and go to next thing attached
                everyAttachedThings.Add(lastAttached);
                lastAttached = lastAttached.attachedTo;
            }

            //reverse detach from last one to this interactable
            for(int i = everyAttachedThings.Count -1; i >= 0; i--)
            {
                if(everyAttachedThings[i].DetachRewindRope() == false)
                {
                    Debug.LogWarning("impossible to detach " + everyAttachedThings[i].name);
                }
            }

            return true;
        }

        return false;
    }

    public void CreateCollider(Vector3 startPoint, Vector3 endPoint)
    {
        GameObject colliderGo = new GameObject();

        colliders.Add(colliderGo.AddComponent<BoxCollider>());

        Vector3 direction = startPoint - endPoint;
        Vector3 center = Vector3.Lerp(startPoint, endPoint, 0.5f);
        colliderGo.transform.position = center;

        colliderGo.transform.rotation = Quaternion.LookRotation(direction);

        float widthRope = rope.startWidth;

        colliderGo.transform.localScale = new Vector3(widthRope, widthRope, direction.magnitude);
    }

    public void DestroyCollider()
    {
        BoxCollider boxcollider = colliders[colliders.Count - 1];

        colliders.Remove(boxcollider);

        Destroy(boxcollider.gameObject);

    }

    public void DestroyAllColliders()
    {
        foreach(BoxCollider box in colliders)
        {
            Destroy(box.gameObject);
        }

        colliders.Clear();
    }

    #endregion

    #region private API

    bool CanCreateRope()
    {
        bool thisIsActive = isActive;                               //be sure this interactable is active
        bool thisIsNotAlreadyAttached = attachedTo == null;    //be sure is not attached to something

        //return if can create rope
        return thisIsActive && thisIsNotAlreadyAttached;
    }

    protected virtual bool CanAttach(Interactable interactable)
    {
        bool isNotItSelf = interactable != this;                            //check if attach to another interactable and not itself
        bool IsNotAlreadyAttached = interactable.attachedTo == null;   //check if interactable is not already attached to something
        bool isNotGenerator = interactable is Generator == false;           //check if interactable is not a generator

        //return if can attach
        return isNotItSelf && IsNotAlreadyAttached && isNotGenerator;
    }

    bool CanDetachRope()
    {
        bool isAttachedToSomething = attachedFrom != null;                   //be sure is already attached to something
        if (isAttachedToSomething)
        {
            bool isNotAttachedAgain = attachedTo == null;  //be sure our attached interactable is NOT attached to another thing

            //return if can detach
            return isNotAttachedAgain;
        }

        return false;
    }

    bool CanDetachRewindRope()
    {
        bool isAttachedToSomething = attachedTo != null;                   //be sure is already attached to something
        if(isAttachedToSomething)
        {
            bool isNotAttachedAgain = attachedTo.attachedTo == null;  //be sure our attached interactable is NOT attached to another thing

            //return if can detach
            return isNotAttachedAgain;
        }

        return false;
    }

    bool CanRewindRope()
    {
        bool isAttachedToSomething = attachedTo != null;                   //be sure is already attached to something
        if (isAttachedToSomething)
        {
            bool isAttachedAgain = attachedTo.attachedTo != null;     //be sure our attached interactable is attached to another thing

            //return if can detach
            return isAttachedAgain;
        }

        return false;
    }

    #endregion
}
