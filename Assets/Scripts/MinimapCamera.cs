using System.Collections;
using UnityEngine;
using redd096;

public class MinimapCamera : MonoBehaviour
{
    [Header("Layer")]
    [SerializeField] string minimapLayer = "Minimap";

    [Header("Movement")]
    [SerializeField] float durationMovement = 1;
    [SerializeField] bool followRotation = true;

    Camera cam;
    Coroutine movementCoroutine;
    RoomGame previousRoom;

    void Awake()
    {
        //remove minimap layer from main camera
        cam = Camera.main;
        cam.cullingMask = LayerUtility.RemoveLayer(cam.cullingMask, LayerUtility.NameToLayer(minimapLayer));

        //set layer to this camera
        GetComponent<Camera>().cullingMask = CreateLayer.LayerOnly(minimapLayer);
    }

    void LateUpdate()
    {
        //if current roomm != previous room, move to new position
        if (GameManager.instance.levelManager.currentRoom && GameManager.instance.levelManager.currentRoom != previousRoom)
        {
            Movement();
        }

        //if must follow rotation, rotate with main camera
        if (followRotation)
        {
            Rotation();
        }
    }

    #region private API

    void Movement()
    {
        //move to new position
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        movementCoroutine = StartCoroutine(MovementCoroutine());
    }

    IEnumerator MovementCoroutine()
    {
        //set vars
        Vector3 startPosition = transform.position;
        Vector3 endPosition = GameManager.instance.levelManager.currentRoom.transform.position + Vector3.up * 20;   //current room position + up 

        //movement animation
        float delta = 0;
        while(delta < 1)
        {
            delta += Time.deltaTime / durationMovement;

            transform.position = Vector3.Lerp(startPosition, endPosition, delta);

            yield return null;
        }
    }

    void Rotation()
    {
        //follow main camera rotation on Y axis
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, cam.transform.eulerAngles.y, transform.eulerAngles.z);
    }

    #endregion
}
