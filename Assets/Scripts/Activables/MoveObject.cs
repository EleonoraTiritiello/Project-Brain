using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : Activable
{
    [Header("Movement")]
    [SerializeField] float speedMovement = 1;
    [SerializeField] Vector3 positionOnActive = Vector3.down * 0.25f;

    Vector3 positionOnDeactive;
    Coroutine movementCoroutine;

    void Start()
    {
        //save position when deactivate
        positionOnDeactive = objectToControl.transform.position;
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
        Vector3 endPosition = active ? positionOnActive : positionOnDeactive;

        while(true)
        {
            float distance = Vector3.Distance(objectToControl.transform.position, endPosition);

            //move
            objectToControl.transform.position = Vector3.MoveTowards(objectToControl.transform.position, endPosition, speedMovement * Time.deltaTime);

            //if passed end position, stop movement
            if(Vector3.Distance(objectToControl.transform.position, endPosition) > distance)
            {
                objectToControl.transform.position = endPosition;
                break;
            }

            yield return null;
        }
    }
}
