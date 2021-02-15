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
        if(changeRoomCoroutine != null)
        {
            StopCoroutine(changeRoomCoroutine);
        }

        changeRoomCoroutine = StartCoroutine(ChangeRoomCoroutine());
        
    }

    IEnumerator ChangeRoomCoroutine()
    {
        roomToActivate.gameObject.SetActive(true);

        float delta = 0;

        while(delta < 1)
        {
            delta += Time.deltaTime / roomToActivate.timeToMoveCamera;
            cam.transform.position = Vector3.Lerp(roomToDeactivate.cameraPosition.position, roomToActivate.cameraPosition.position, delta);
            cam.transform.rotation = Quaternion.Lerp(roomToDeactivate.cameraPosition.rotation, roomToActivate.cameraPosition.rotation, delta);

            yield return null;
        }

        roomToDeactivate.gameObject.SetActive(false);

    }

   
}
