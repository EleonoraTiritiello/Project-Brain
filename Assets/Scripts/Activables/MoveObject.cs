using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : Activable
{
    [Header("Movement")]
    [SerializeField] float speedMovement = 1;
    [SerializeField] Vector3 localPositionOnActive = Vector3.down * 0.25f;

    Vector3 localPositionOnDeactive;
    Coroutine movementCoroutine;

    protected override void Start()
    {
        base.Start();

        //save positions when active or deactive
        localPositionOnDeactive = ObjectToControl.transform.localPosition;
    }

    void OnDisable()
    {
        //if deactivate while coroutine is running, set last position
        if(movementCoroutine != null)
        {
            Vector3 endPosition = isActive ? localPositionOnActive : localPositionOnDeactive;
            ObjectToControl.transform.localPosition = endPosition;
        }
    }

    protected override void Active()
    {
        //be sure there is no a coroutine running
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        //start coroutine with active true
        if(gameObject.activeInHierarchy)
            movementCoroutine = StartCoroutine(MovementCoroutine());
    }

    protected override void Deactive()
    {
        //be sure there is no a coroutine running
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        //start coroutine with active false
        if (gameObject.activeInHierarchy)
            movementCoroutine = StartCoroutine(MovementCoroutine());
    }

    IEnumerator MovementCoroutine()
    {
        //set vars
        Vector3 endPosition = isActive ? localPositionOnActive : localPositionOnDeactive;

        while(true)
        {
            float distance = Vector3.Distance(ObjectToControl.transform.localPosition, endPosition);

            //move
            ObjectToControl.transform.localPosition = Vector3.MoveTowards(ObjectToControl.transform.localPosition, endPosition, speedMovement * Time.deltaTime);

            //if passed end position, stop movement
            if(Vector3.Distance(ObjectToControl.transform.localPosition, endPosition) > distance)
            {
                ObjectToControl.transform.localPosition = endPosition;
                break;
            }

            yield return null;
        }

        movementCoroutine = null;
    }

    void OnCollisionEnter(Collision collision)
    {
        //if hit rope collider
        RopeColliderInteraction ropeColliderInteraction = collision.gameObject.GetComponentInParent<RopeColliderInteraction>();
        if(ropeColliderInteraction)
        {
            //destroy rope
            ropeColliderInteraction.DestroyRope();
        }
    }
}
