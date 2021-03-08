using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Speed Rotation")]
    [SerializeField] float rotationSpeedX = 300;
    [SerializeField] float rotationSpeedY = 300;
    [SerializeField] float smooth = 20;

    [Header("Limits Rotation")]
    [SerializeField] float limitX = 170;
    [SerializeField] float limitY = 40;

    //rotation
    Transform cam;
    Transform cameraTarget;
    Transform pivot;

    float rotationX;
    float rotationY;

    void Start()
    {
        //get camera and create a target to move smooth camera
        cam = Camera.main.transform;
        cameraTarget = new GameObject("Camera Target").transform;

        //confined mouse
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        //do only if there is pivot
        if (pivot == null)
            return;

        //get rotation by input
        GetInputRotation();

        //be sure can move and is not clamped
        if (Clamped())
            return;

        //rotate target around pivot
        RotateTransformAround(cameraTarget, pivot.position, Vector3.up, rotationX);
        RotateTransformAround(cameraTarget, pivot.position, cameraTarget.right, rotationY);

        //smooth movement camera to target
        LerpCamera();
    }

    #region private API

    void GetInputRotation()
    {
        //calculate inputs
        rotationX = -Input.GetAxis("Mouse X") * rotationSpeedX * Time.deltaTime;
        rotationY = Input.GetAxis("Mouse Y") * rotationSpeedY * Time.deltaTime;
    }

    bool Clamped()
    {
        //clamp rotation
        float x = NegativeAngle(cameraTarget.eulerAngles.y) + rotationX;
        float y = NegativeAngle(cameraTarget.eulerAngles.x) + rotationY;

        return x < -limitX || x > limitX || y < -limitY || y > limitY;
    }

    void RotateTransformAround(Transform transformToRotate, Vector3 pivot, Vector3 axis, float angle)
    {
        Vector3 direction = transformToRotate.position - pivot;     //get direction and distance from pivot
        direction = Quaternion.Euler(axis * angle) * direction;     //rotate it by angle
        Vector3 newPosition = pivot + direction;                    //from pivot, add new direction
        //rotation = Quaternion.LookRotation(pivot - newPosition);  //look at pivot

        transformToRotate.position = newPosition;                                                   //set position
        transformToRotate.rotation = Quaternion.Euler(axis * angle) * transformToRotate.rotation;   //add rotation
    }

    void LerpCamera()
    {
        //smooth movement camera to target
        cam.position = Vector3.Lerp(cam.position, cameraTarget.position, Time.deltaTime * smooth);
        cam.rotation = Quaternion.Lerp(cam.rotation, cameraTarget.rotation, Time.deltaTime * smooth);
    }

    float NegativeAngle(float angle)
    {
        //show positive and negative angle (instead of 270, show -90)
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    #endregion

    /// <summary>
    /// Set pivot to rotate around
    /// </summary>
    public void SetPivot(Transform pivot)
    {
        this.pivot = pivot;

        //reset target because camera was moved by a coroutine
        cameraTarget.position = cam.position;
        cameraTarget.rotation = cam.rotation;
    }

    /// <summary>
    /// Remove pivot and stop camera movement
    /// </summary>
    public void RemovePivot()
    {
        pivot = null;
    }
}
