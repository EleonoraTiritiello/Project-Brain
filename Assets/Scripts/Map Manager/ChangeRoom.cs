using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRoom : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] RoomGame roomToDeactivate = default;
    [SerializeField] RoomGame roomToActivate = default;

    Camera cam;
    Coroutine changeRoomCoroutine;

    private void Start()
    {
        cam = Camera.main;
    }


    private void OnTriggerEnter(Collider other)
    {
        //stop if there is already a coroutine running
        if(changeRoomCoroutine != null)
        {
            StopCoroutine(changeRoomCoroutine);
        }

        //start new coroutine
        changeRoomCoroutine = StartCoroutine(ChangeRoomCoroutine());
        
    }

    IEnumerator ChangeRoomCoroutine()
    {
        //active new room
        roomToActivate.gameObject.SetActive(true);

        //move cam to position and rotation
        float delta = 0;
        while(delta < 1)
        {
            delta += Time.deltaTime / roomToActivate.timeToMoveCamera;
            cam.transform.position = Vector3.Lerp(roomToDeactivate.cameraPosition.position, roomToActivate.cameraPosition.position, delta);
            cam.transform.rotation = Quaternion.Lerp(roomToDeactivate.cameraPosition.rotation, roomToActivate.cameraPosition.rotation, delta);

            yield return null;
        }

        //deactive old room
        roomToDeactivate.gameObject.SetActive(false);
    }   
}
