using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Important")]
    [Tooltip("The object to interact with")] [SerializeField] GameObject objectToControl = default;
    public GameObject ObjectToControl => objectToControl != null ? objectToControl : gameObject;

    [Header("Rope")]
    [SerializeField] LineRenderer ropePrefab = default;
    [SerializeField] RopeColliderInteraction colliderPrefab = default;
    public float RopeLength = 10;

    protected bool isActive;
    protected Interactable attachedTo;
    protected Interactable attachedFrom;

    LineRenderer rope;
    Pooling<RopeColliderInteraction> colliders = new Pooling<RopeColliderInteraction>();
    List<RopeColliderInteraction> collidersInOrder = new List<RopeColliderInteraction>();

    #region public API

    /// <summary>
    /// Active or Deactive interactable
    /// </summary>
    /// <param name="active">try activate object when true, or deactivate when false</param>
    /// <param name="interactable">object attached from</param>
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
            //there is not prefab, so can't create rope
            else if (ropePrefab == null)
                return false;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Set new positions for the rope
    /// </summary>
    public void UpdateRope(List<Vector3> positions)
    {
        //update rope positions
        rope.positionCount = positions.Count;
        rope.SetPositions(positions.ToArray());

        //if greater than colliders count, add collider (not to last one, cause that is hand position)
        if (positions.Count -1 > colliders.PooledObjects.Count && positions.Count > 2)
            CreateCollider(positions[positions.Count - 2], positions[positions.Count - 3]);
        //if lower than colliders count, remove last collider
        else if (positions.Count -1 < colliders.PooledObjects.Count)
            DestroyLastCollider();
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
            interactable.ActiveInteractable(true, this);
            attachedTo = interactable;

            //set last rope position (and collider)
            rope.SetPosition(rope.positionCount -1, interactable.ObjectToControl.transform.position);
            CreateCollider(rope.GetPosition(rope.positionCount - 2), rope.GetPosition(rope.positionCount - 1));

            return true;
        }

        return false;
    }

    /// <summary>
    /// Detach rope, return rope's owner
    /// </summary>
    /// <param name="interactable">object this interactable was attached from, the rope's owner</param>
    /// <param name="ropePositions">positions already in this rope</param>
    /// <returns></returns>
    public bool DetachRope(out Interactable interactable, out List<Vector3> ropePositions)
    {
        if (CanDetachRope())
        {
            //return rope's owner and set is not attached to this one anymore
            interactable = attachedFrom;
            attachedFrom.attachedTo = null;

            //add every rope position to the list (not last one because was the attach to this object)
            ropePositions = new List<Vector3>();
            for (int i = 0; i < attachedFrom.rope.positionCount - 1; i++)
                ropePositions.Add(attachedFrom.rope.GetPosition(i));

            //remove last collider and deactive this object
            attachedFrom.DestroyLastCollider();
            ActiveInteractable(false, null);

            return true;
        }

        interactable = null;
        ropePositions = new List<Vector3>();
        return false;
    }

    /// <summary>
    /// Detach rope from this interactable (used for rewind)
    /// </summary>
    public bool DetachRewindRope()
    {
        //check if can detach
        if(CanDetachRewindRope())
        {
            //our attached interactable is no more active, and this is not attached to nothing
            attachedTo.ActiveInteractable(false, null);
            attachedTo = null;

            //hide rope and destroy every collider
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

    /// <summary>
    /// Create collider from start point to end point
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    public void CreateCollider(Vector3 startPoint, Vector3 endPoint)
    {
        //create collider
        RopeColliderInteraction collider = colliders.Instantiate(colliderPrefab);
        collidersInOrder.Add(collider);

        //set rope vars
        collider.Init(this);

        //set position
        Vector3 center = Vector3.Lerp(startPoint, endPoint, 0.5f);
        collider.transform.position = center;

        //set rotation
        Vector3 direction = startPoint - endPoint;
        collider.transform.rotation = Quaternion.LookRotation(direction);

        //set scale
        float widthRope = rope.startWidth;
        collider.transform.localScale = new Vector3(widthRope, widthRope, direction.magnitude);
    }

    /// <summary>
    /// Destroy last collider
    /// </summary>
    public void DestroyLastCollider()
    {
        //do only if there are colliders
        if (collidersInOrder.Count <= 0)
            return;

        //get last collider
        RopeColliderInteraction collider = collidersInOrder[collidersInOrder.Count - 1];

        //remove from the list and destroy
        collidersInOrder.Remove(collider);
        Pooling<Collider>.Destroy(collider.gameObject);
    }

    /// <summary>
    /// Destroy every collider of this rope
    /// </summary>
    public void DestroyAllColliders()
    {
        //destroy every collider in the list
        colliders.DestroyAll();

        //and clear list
        colliders.Clear();
        collidersInOrder.Clear();
    }

    #endregion

    #region private API

    bool CanCreateRope()
    {
        bool thisIsActive = isActive;                                   //be sure this interactable is active
        bool thisIsNotAlreadyAttached = attachedTo == null;             //be sure is not attached to something

        //return if can create rope
        return thisIsActive && thisIsNotAlreadyAttached;
    }

    protected virtual bool CanAttach(Interactable interactable)
    {
        bool isNotItSelf = interactable != this;                        //check if attach to another interactable and not itself
        bool IsNotAlreadyAttached = interactable.attachedTo == null;    //check if interactable is not already attached to something
        bool isNotGenerator = interactable is Generator == false;       //check if interactable is not a generator

        //return if can attach
        return isNotItSelf && IsNotAlreadyAttached && isNotGenerator;
    }

    bool CanDetachRope()
    {
        bool isAttachedToSomething = attachedFrom != null;              //be sure is already attached from something
        if (isAttachedToSomething)
        {
            bool isNotAttachedAgain = attachedTo == null;               //be sure this one is NOT attached to another thing

            //return if can detach
            return isNotAttachedAgain;
        }

        return false;
    }

    bool CanDetachRewindRope()
    {
        bool isAttachedToSomething = attachedTo != null;                //be sure is already attached to something
        if(isAttachedToSomething)
        {
            bool isNotAttachedAgain = attachedTo.attachedTo == null;    //be sure our attached interactable is NOT attached to another thing

            //return if can detach
            return isNotAttachedAgain;
        }

        return false;
    }

    bool CanRewindRope()
    {
        bool isAttachedToSomething = attachedTo != null;                //be sure is already attached to something

        return isAttachedToSomething;
    }

    #endregion
}
