using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : StateMachine
{
    [Header("States")]
    public NormalState normalState;
    public DraggingRopeState draggingRopeState;

    [HideInInspector] public Interactable connectedPoint;
    [HideInInspector] public bool usingRightHand = true;

    public System.Action onChangeHand;

    private void Start()
    {
        //set default state to normalState
        SetState(normalState);
    }

    private void OnDrawGizmosSelected()
    {
        //draw radius pick of normalState
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, normalState.radiusInteract);
    }

    public void ChangeHand()
    {
        //change hand
        usingRightHand = !usingRightHand;

        //call event
        onChangeHand?.Invoke();
    }
}
