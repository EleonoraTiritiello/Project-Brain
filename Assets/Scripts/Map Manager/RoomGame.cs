using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGame : Room
{
    [Header("Camera Position")]
    public Transform cameraPosition = default;
    public float timeToMoveCamera = 1;

    Transform cam;

    private void OnEnable()
    {
        if (cameraPosition == null)
        {
            Debug.LogWarning($"Manca la camera position nella camera {gameObject.name}");
            return;
        }

        //get cam if null
        if (cam == null)
            cam = Camera.main.transform;

        //set cam position and rotation
        cam.position = cameraPosition.position;
        cam.rotation = cameraPosition.rotation;
    }
}
