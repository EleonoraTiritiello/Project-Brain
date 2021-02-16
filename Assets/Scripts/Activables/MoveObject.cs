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

    void Start()
    {
        //save positions when active or deactive
        localPositionOnDeactive = ObjectToControl.transform.localPosition;
    }

    protected override void Active()
    {
        //be sure there is no a coroutine running
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        //start coroutine with active true
        movementCoroutine = StartCoroutine(MovementCoroutine(true));
    }

    protected override void Deactive()
    {
        //be sure there is no a coroutine running
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        //start coroutine with active false
        movementCoroutine = StartCoroutine(MovementCoroutine(false));
    }

    IEnumerator MovementCoroutine(bool active)
    {
        //set vars
        Vector3 endPosition = active ? localPositionOnActive : localPositionOnDeactive;

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
    }
}
