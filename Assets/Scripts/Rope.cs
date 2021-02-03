using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField] Rope ropePrefab = default;
    [SerializeField] float distanceNewRope = 0.4f;
    [SerializeField] float forceToIncreaseLength = 15000f;
    [SerializeField] int limitPiecesOfRope = 10;

    [Header("DEBUG")]
    [SerializeField] float currentForce;

    Player player;
    ConfigurableJoint joint;

    //start with one piece (this one)
    int piecesOfRope = 1;

    void Awake()
    {
        //get reference
        joint = GetComponent<ConfigurableJoint>();
    }

    public void Pick(Player player)
    {
        this.player = player;
    }

    void FixedUpdate()
    {
        //if picked by a player
        if(player != null && piecesOfRope < limitPiecesOfRope)
        {
            currentForce = joint.currentForce.magnitude;

            //check distance and if necessary, update length
            if(currentForce > forceToIncreaseLength)
            {
                UpdateRopeLength();
            }
        }
    }

    void UpdateRopeLength()
    {
        //instantiate new rope at current position
        Rope newRope = Instantiate(ropePrefab, transform.parent);
        newRope.transform.position = transform.position;
        newRope.transform.rotation = transform.rotation;

        //get joint and remove new rope script
        ConfigurableJoint newRopeJoint = newRope.GetComponent<ConfigurableJoint>();
        Destroy(newRope);

        //move forward THIS rope
        transform.position += transform.up * distanceNewRope;

        //set new rope to our current connected body (generator by default), and connect us to new rope
        newRopeJoint.connectedBody = joint.connectedBody;
        joint.connectedBody = newRopeJoint.GetComponent<Rigidbody>();

        //set our anchor point (our anchor, but in local for connected body)
        Vector3 worldAnchorPosition = transform.TransformPoint(joint.anchor);
        joint.connectedAnchor = joint.connectedBody.transform.InverseTransformPoint(worldAnchorPosition);

        //set new rope anchor point (new rope anchor, but in local for connected body)
        Vector3 newRopeWorldAnchorPosition = newRopeJoint.transform.TransformPoint(newRopeJoint.anchor);
        newRopeJoint.connectedAnchor = newRopeJoint.connectedBody.transform.InverseTransformPoint(newRopeWorldAnchorPosition);

        //increase pieces of rope
        piecesOfRope++;
    }
}
