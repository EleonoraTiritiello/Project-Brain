using System.Collections;
using UnityEngine;
using redd096;

public class MinimapCamera : MonoBehaviour
{
    [Header("Layer")]
    [SerializeField] string minimapLayer = "Minimap";

    [Header("Movement")]
    [SerializeField] float durationMovement = 1;

    Coroutine movementCoroutine;
    RoomGame previousRoom;

    void Awake()
    {
        //remove minimap layer from main camera
        Camera cam = Camera.main;
        cam.cullingMask = LayerUtility.RemoveLayer(cam.cullingMask, LayerUtility.NameToLayer(minimapLayer));

        //set layer to this camera
        GetComponent<Camera>().cullingMask = CreateLayer.LayerOnly(minimapLayer);
    }

    void LateUpdate()
    {
        //if current room != previous room
        if (GameManager.instance.levelManager.currentRoom && GameManager.instance.levelManager.currentRoom != previousRoom)
        {
            //move to new position
            if (movementCoroutine != null)
                StopCoroutine(movementCoroutine);

            movementCoroutine = StartCoroutine(MovementCoroutine());
        }
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
}
