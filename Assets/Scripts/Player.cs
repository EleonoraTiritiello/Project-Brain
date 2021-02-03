using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] float speed = 5;
    [SerializeField] float rotationSpeed = 5;

    [Header("Rope")]
    [SerializeField] float radiusPick = 3;
    [SerializeField] KeyCode pickRopeInput = KeyCode.E;
    [SerializeField] Vector3 anchorPoint = Vector3.zero;

    [Header("Line Renderer")]
    [SerializeField] LineRenderer linePrefab = default;
    Transform connectedPoint;
    LineRenderer line;

    Rigidbody rb;
    ConfigurableJoint joint;
    Transform cam;

    Rope currentRope;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();
        cam = Camera.main.transform;

        //lock rigidbody rotation
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void OnDrawGizmosSelected()
    {
        //draw radius pick
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusPick);

        //draw anchor point
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + anchorPoint, 0.1f);
    }

    void Update()
    {
        Movement();
        Rotate();
        FindRope();

        //line renderer
        if(connectedPoint != null && linePrefab != null)
        {
            //instantiate line if null
            if (line == null)
            {
                line = Instantiate(linePrefab);
                line.positionCount = 2;
            }

            //update line position
            line.SetPosition(0, connectedPoint.position);
            line.SetPosition(1, transform.position);
        }
    }

    #region private API

    void Movement()
    {
        //get direction by input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        //direction to local, then move player
        direction = transform.TransformDirection(direction);
        rb.velocity = direction * speed;
    }

    void Rotate()
    {
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = -Input.GetAxis("Mouse Y");

        //rotate player on Y axis and camera on X axis
        transform.rotation = Quaternion.AngleAxis(horizontal * rotationSpeed * Time.deltaTime, Vector3.up) * transform.rotation;
        cam.rotation = Quaternion.AngleAxis(vertical * rotationSpeed * Time.deltaTime, transform.right) * cam.rotation;
    }

    void FindRope()
    {
        //when press input and there is no rope in hand
        if(Input.GetKeyDown(pickRopeInput) && currentRope == null)
        {
            //check every collider in area
            Collider[] colliders = Physics.OverlapSphere(transform.position, radiusPick);
            foreach(Collider col in colliders)
            {
                //if found rope, pick it
                if(col.GetComponent<Rope>())
                {
                    PickRope(col.GetComponent<Rope>());
                }
            }
        }
    }

    void PickRope(Rope rope)
    {
        //set rope in hand
        currentRope = rope;
        rope.Pick(this);

        //add joint and set it
        joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.anchor = anchorPoint;
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;
        joint.projectionMode = JointProjectionMode.PositionAndRotation;

        //add rope to joint
        joint.connectedBody = rope.GetComponent<Rigidbody>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = Vector3.up;

        //line renderer
        connectedPoint = rope.GetComponent<Joint>().connectedBody.transform;
    }

    #endregion

    public void UpdateAnchorPoint()
    {
        Vector3 worldAnchorPosition = transform.TransformPoint(anchorPoint);
        joint.connectedAnchor = joint.connectedBody.transform.InverseTransformPoint(worldAnchorPosition);
    }
}
