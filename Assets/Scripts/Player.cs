using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] float speed = 5;
    [SerializeField] float rotationSpeed = 5;

    [Header("Rope")]
    [SerializeField] LineRenderer linePrefab = default;
    [SerializeField] float radiusPick = 3;
    [SerializeField] KeyCode pickRopeInput = KeyCode.E;
    [SerializeField] KeyCode changeHand = KeyCode.Space;
    [SerializeField] Transform rightHand = default;
    [SerializeField] Transform leftHand = default;

    Generator connectedPoint;
    LineRenderer line;
    bool usingRightHand = true;

    Rigidbody rb;
    Transform cam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;

        //lock rigidbody rotation
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void OnDrawGizmosSelected()
    {
        //draw radius pick
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusPick);
    }

    void Update()
    {
        Movement();
        Rotate();
        FindRope();
        UpdateLine();

        //change hand
        if(Input.GetKeyDown(changeHand))
        {
            usingRightHand = !usingRightHand;
        }

        if (connectedPoint)
        {
            Vector3 handPosition = usingRightHand ? rightHand.position : leftHand.position;
            Debug.Log(Vector3.Distance(connectedPoint.transform.position, handPosition));
        }
    }

    #region private API

    void Movement()
    {
        //get direction by input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        //direction to local
        direction = transform.TransformDirection(direction);

        //check if no reach rope limit
        if(connectedPoint != null)
        {
            Vector3 handPosition = usingRightHand ? rightHand.position : leftHand.position;
            Vector3 nextPosition = handPosition + direction * speed * Time.deltaTime;
            if(Vector3.Distance(nextPosition, connectedPoint.transform.position) > connectedPoint.ropeLength)
            {
                //if reach limit, stop movement
                rb.velocity = Vector3.zero;
                return;
            }
        }

        //move player
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
        if(Input.GetKeyDown(pickRopeInput) && connectedPoint == null)
        {
            //check every collider in area
            Collider[] colliders = Physics.OverlapSphere(transform.position, radiusPick);
            foreach(Collider col in colliders)
            {
                //if found generator, pick it
                Generator generator = col.GetComponentInParent<Generator>();
                if(generator)
                {
                    connectedPoint = generator;
                }
            }
        }
    }

    void UpdateLine()
    {
        //line renderer
        if (connectedPoint != null && linePrefab != null)
        {
            //instantiate line if null
            if (line == null)
            {
                line = Instantiate(linePrefab);
                line.positionCount = 2;
            }

            //update line position
            Vector3 handPosition = usingRightHand ? rightHand.position : leftHand.position;
            line.SetPosition(0, connectedPoint.transform.position);
            line.SetPosition(1, handPosition);
        }
    }

    #endregion
}
