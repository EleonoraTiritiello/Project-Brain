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
    [SerializeField] float distanceBetweenPoints = 0.5f;

    SpringJoint joint;
    bool usingRightHand = true;

    int anglePositive;
    [SerializeField] List<Vector3> ropePositions = new List<Vector3>();
    [SerializeField] List<float> ropeLengths = new List<float>();
    Vector3 lastRope => ropePositions[ropePositions.Count - 1];
    Vector3 penultimaRope => ropePositions[ropePositions.Count - 2];
    float lastLength => ropeLengths[ropeLengths.Count - 1];

    public override void Enter()
    {
        base.Enter();

        //start with position at connected point
        Player player = stateMachine as Player;
        ropePositions.Add(player.connectedPoint.transform.position);
        ropeLengths.Add(player.connectedPoint.ropeLength);

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

    public override void Exit()
    {
        base.Exit();

        //clear vars
        ropePositions.Clear();
        ropeLengths.Clear();
        anglePositive = -1;
    }

    #region private API

    void CreateSpringJoint()
    {
        //add joint
        joint = stateMachine.gameObject.AddComponent<SpringJoint>();

        //set joint connected anchor
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = lastRope;

        //set joint distances
        joint.maxDistance = lastLength;
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

            //remove previous hand position
            if (ropePositions.Count > 1)
            {
                ropePositions.Remove(lastRope);
                ropeLengths.Remove(lastLength);
            }

            //if hit something, add point
            RaycastHit hit;
            Vector3 direction = (lastRope - handPosition).normalized;//+ direction to be sure to hit something
            if (Physics.Linecast(handPosition, lastRope + direction * 0.2f, out hit, redd096.CreateLayer.LayerAllExcept("Player")) 
                && Vector3.Distance(hit.point, handPosition) < Vector3.Distance(lastRope, handPosition)     //check is near then last point
                && Vector3.Distance(hit.point, lastRope) > distanceBetweenPoints)                           //check distance to not hit always same point
            {
                //add point, calculate new length, and recreate joint
                ropePositions.Add(hit.point);
                ropeLengths.Add(lastLength - Vector3.Distance(penultimaRope, hit.point));
                Object.Destroy(joint);
                CreateSpringJoint();

                //reset angle
                anglePositive = -1;
            }
            //else, if no hit neither previous positition, remove point until come back to connected point
            else 
            {
                hit.point = handPosition;   //for add handPosition as last length just for debug

                if (ropePositions.Count > 1)
                {
                    //if angle is not set, calculate if is positive or negative
                    if(anglePositive < 0)
                    {
                        Vector3 directionFromHand = lastRope - handPosition;
                        Vector3 directionNext = lastRope - penultimaRope;
                        int angle = Mathf.RoundToInt( Vector3.SignedAngle(directionFromHand, directionNext, Vector3.up) );

                        if (angle > 0 && angle < 180)
                            anglePositive = 1;
                        else if (angle < 0 && angle > -180)
                            anglePositive = 0;
                    }

                    RaycastHit newHit;
                    if (Physics.Linecast(handPosition, penultimaRope, out newHit, redd096.CreateLayer.LayerAllExcept("Player")) == false 
                        || Vector3.Distance(newHit.point, penultimaRope) < distanceBetweenPoints)   //check if hit near to point, in this case calculate as same point
                    {
                        //calculate angle - if greater than 180 remove last point
                        Vector3 directionFromHand = lastRope - handPosition;
                        Vector3 directionNext = lastRope - penultimaRope;
                        float angle = Vector3.SignedAngle(directionFromHand, directionNext, Vector3.up);

                        //greater than 180 if now is positive and before was negative or viceversa
                        if (angle > 0 && anglePositive == 0 || angle < 0 && anglePositive == 1)
                        {
                            //remove last point, remove last length, and recreate joint
                            ropePositions.Remove(lastRope);
                            ropeLengths.Remove(lastLength);
                            Object.Destroy(joint);
                            CreateSpringJoint();

                            //reset angle
                            anglePositive = -1;
                        }
                    }
                }
            }

            //add hand position for lineRenderer
            ropePositions.Add(handPosition);
            ropeLengths.Add(lastLength - Vector3.Distance(penultimaRope, hit.point));

            //update rope position
            player.connectedPoint.UpdateRope(ropePositions);
        }
    }

    protected override void Interact()
    {
        Interactable interactable = FindInteractable();

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

    protected override void RewindRope()
    {
        //do nothing
    }

    protected override void DetachRope()
    {
        Player player = stateMachine as Player;

        //hide rope
        player.connectedPoint.UpdateRope(new List<Vector3>());

        //remove player connected point and joint
        player.connectedPoint = null;
        Object.Destroy(joint);

        //back to normal state
        player.SetState(player.normalState);
    }

    #endregion
}
