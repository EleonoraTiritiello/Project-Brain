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

    [SerializeField] List<Vector3> ropePositions = new List<Vector3>();
    [SerializeField] List<float> ropeLengths = new List<float>();
    bool anglePositive;
    Vector3 ultimaRope => ropePositions[ropePositions.Count - 1];
    Vector3 penultimaRope => ropePositions[ropePositions.Count - 2];
    float ultimaLength => ropeLengths[ropeLengths.Count - 1];

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

        ropePositions.Clear();
    }

    #region private API

    void CreateSpringJoint()
    {
        //just for debug
        GameObject go = new GameObject(ultimaRope.ToString());
        go.transform.position = ultimaRope;

        //add joint
        joint = stateMachine.gameObject.AddComponent<SpringJoint>();

        //set joint connected anchor
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = ultimaRope;

        //set joint distances
        joint.maxDistance = ropeLengths[ropeLengths.Count -1];
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
                ropePositions.Remove(ultimaRope);
                ropeLengths.Remove(ultimaLength);
            }

            //if hit something, add point
            RaycastHit hit;
            Vector3 direction = (ultimaRope - handPosition).normalized;//+ direction to be sure to hit something
            if (Physics.Linecast(handPosition, ultimaRope + direction * 0.2f, out hit, redd096.CreateLayer.LayerAllExcept("Player")) && Vector3.Distance(ultimaRope, hit.point) > 1)
            {
                ropePositions.Add(hit.point);
                ropeLengths.Add(ultimaLength - Vector3.Distance(penultimaRope, hit.point));
                Object.Destroy(joint);
                CreateSpringJoint();

                //set how to check angle
                Vector3 directionFromHand = ultimaRope - handPosition;
                Vector3 directionNext = ultimaRope - penultimaRope;
                anglePositive = Vector3.SignedAngle(directionFromHand, directionNext, Vector3.up) > 0;
            }
            //else, if no hit neither previous positition, remove point until come back to connected point
            else 
            {
                hit.point = handPosition;   //for add last length just for debug

                if (ropePositions.Count > 1)
                {
                    RaycastHit newHit;
                    if (!Physics.Linecast(handPosition, penultimaRope, out newHit, redd096.CreateLayer.LayerAllExcept("Player")) || Vector3.Distance(penultimaRope, newHit.point) < 1)
                    {
                        //calculate angle instead of line to penultima - if greater than 180 remove last point
                        Vector3 directionFromHand = ultimaRope - handPosition;
                        Vector3 directionNext = ultimaRope - penultimaRope;
                        float angle = Vector3.SignedAngle(directionFromHand, directionNext, Vector3.up);
                        if (angle > 0 && anglePositive == false || angle < 0 && anglePositive)
                        {
                            ropePositions.Remove(ultimaRope);
                            ropeLengths.Remove(ultimaLength);
                            Object.Destroy(joint);
                            CreateSpringJoint();
                        }
                    }
                    //RaycastHit newHit;
                    //if (!Physics.Linecast(handPosition, penultimaRope, out newHit, redd096.CreateLayer.LayerAllExcept("Player")) || Vector3.Distance(penultimaRope, newHit.point) < 1)
                    //{
                    //    ropePositions.Remove(ultimaRope);
                    //    ropeLengths.Remove(ultimaLength);
                    //    Object.Destroy(joint);
                    //    CreateSpringJoint();
                    //}
                }
            }

            //add hand position
            ropePositions.Add(handPosition);
            ropeLengths.Add(ultimaLength - Vector3.Distance(penultimaRope, hit.point));

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
        //do nothing
    }

    #endregion
}
