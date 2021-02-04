using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NormalState : State
{
    [Header("Movement")]
    [SerializeField] float speed = 5;
    [SerializeField] float rotationSpeed = 300;

    [Header("Interact")]
    public float radiusInteract = 1.5f;
    [SerializeField] KeyCode interactInput = KeyCode.E;
    [SerializeField] KeyCode detachRopeInput = KeyCode.Q;

    Rigidbody rb;
    Transform cam;

    public override void Enter()
    {
        base.Enter();

        rb = stateMachine.GetComponent<Rigidbody>();
        cam = Camera.main.transform;

        //lock rigidbody rotation
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public override void Update()
    {
        base.Update();

        Movement();
        Rotate();

        //interact
        if (Input.GetKeyDown(interactInput))
        {
            Interactable interactable = FindInteractable();
            Interact(interactable);
        }
        //detach rope
        else if(Input.GetKeyDown(detachRopeInput))
        {
            Interactable interactable = FindInteractable();
            DetachRope(interactable);
        }
    }

    #region private API

    void Movement()
    {
        //get direction by input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        //direction to local
        direction = stateMachine.transform.TransformDirection(direction);

        //move player
        rb.velocity = direction * speed;
    }

    void Rotate()
    {
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = -Input.GetAxis("Mouse Y");

        //rotate player on Y axis and camera on X axis
        stateMachine.transform.rotation = Quaternion.AngleAxis(horizontal * rotationSpeed * Time.deltaTime, Vector3.up) * stateMachine.transform.rotation;
        cam.RotateAround(stateMachine.transform.position, stateMachine.transform.right, vertical * rotationSpeed * Time.deltaTime);
    }

    Interactable FindInteractable()
    {
        List<Interactable> listInteractables = new List<Interactable>();

        //check every collider in area
        Collider[] colliders = Physics.OverlapSphere(stateMachine.transform.position, radiusInteract);
        foreach (Collider col in colliders)
        {
            //if found interactable
            Interactable interactable = col.GetComponentInParent<Interactable>();
            if (interactable)
            {
                //add to list
                listInteractables.Add(interactable);
            }
        }

        //only if found something
        if (listInteractables.Count > 0)
        {
            //find nearest
            return FindNearest(listInteractables, stateMachine.transform.position);
        }

        return null;
    }

    /// <summary>
    /// Find nearest to position
    /// </summary>
    Interactable FindNearest(List<Interactable> list, Vector3 position)
    {
        Interactable nearest = null;
        float distance = Mathf.Infinity;

        //foreach element in the list
        foreach (Interactable element in list)
        {
            //check distance to find nearest
            float newDistance = Vector3.Distance(element.transform.position, position);
            if (newDistance < distance)
            {
                distance = newDistance;
                nearest = element;
            }
        }

        return nearest;
    }

    protected virtual void Interact(Interactable interactable)
    {
        //if create rope
        if (interactable && interactable.CreateRope())
        {
            //connect to interactable
            Player player = stateMachine as Player;
            player.connectedPoint = interactable;

            //change state to dragging rope
            player.SetState(player.draggingRopeState);
        }
    }

    protected virtual void DetachRope(Interactable interactable)
    {
        //if detach rope
        if(interactable && interactable.DetachRope())
        {
            //connect to interactable
            Player player = stateMachine as Player;
            player.connectedPoint = interactable;

            //change state to dragging rope
            player.SetState(player.draggingRopeState);
        }
    }

    #endregion
}
