using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : StateMachine
{
    [Header("States")]
    public NormalState normalState;
    public DraggingRopeState draggingRopeState;

    [HideInInspector] public Interactable connectedPoint;

    public System.Action<Transform> onChangeHand { get; set; }

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
}
