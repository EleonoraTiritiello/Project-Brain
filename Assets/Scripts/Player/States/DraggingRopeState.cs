using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DraggingRopeState : NormalState
{
    [Header("Joint")]
    [SerializeField] float spring = 200;
    [SerializeField] float damper = 200;
    [SerializeField] float massScale = 100;

    [Header("Rope")]
    [SerializeField] KeyCode changeHand = KeyCode.Space;
    [SerializeField] Transform rightHand = default;
    [SerializeField] Transform leftHand = default;

    SpringJoint joint;
    bool usingRightHand = true;

    public override void Enter()
    {
        base.Enter();

        //create spring joint from connected point        
        CreateSpringJoint();
    }

    public override void Update()
    {
        base.Update();

        //change hand
        if (Input.GetKeyDown(changeHand))
        {
            usingRightHand = !usingRightHand;
        }

        //show line renderer
        UpdateRope();
    }

    #region private API

    void CreateSpringJoint()
    {
        Player player = stateMachine as Player;

        //linecast to generator
        RaycastHit hit;
        Physics.Linecast(stateMachine.transform.position, player.connectedPoint.transform.position, out hit);

        //add joint
        joint = stateMachine.gameObject.AddComponent<SpringJoint>();

        //set joint connected anchor
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = hit.point;

        //set joint distances
        joint.maxDistance = player.connectedPoint.ropeLength;
        joint.minDistance = 0;

        //set joint spring
        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;
    }

    void UpdateRope()
    {
        Player player = stateMachine as Player;

        //if player is connected to something
        if (player.connectedPoint != null)
        {
            //get hand position
            Vector3 handPosition = usingRightHand ? rightHand.position : leftHand.position;

            //update rope position
            player.connectedPoint.UpdateRope(handPosition);
        }
    }

    protected override void Interact(Interactable interactable)
    {
        //if attach rope
        Player player = stateMachine as Player;
        if (interactable && player.connectedPoint.AttachRope(interactable))
        {
            //remove player connected point and joint
            player.connectedPoint = null;
            Object.Destroy(joint);

            //back to normal state
            player.SetState(player.normalState);
        }
    }

    protected override void DetachRope(Interactable interactable)
    {
        //do nothing
    }

    #endregion
}
