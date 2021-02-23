using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : StateMachine
{
    [Header("States")]
    public NormalState normalState;
    public DraggingRopeState draggingRopeState;

    [HideInInspector] public Interactable connectedPoint;
    [HideInInspector] public bool usingRightHand = true;

    public System.Action onChangeHand;

    //check room
    RoomGame currentRoom;
    Coroutine changeRoomCoroutine;
    Camera cam;

    private void Start()
    {
        //set default state to normalState
        SetState(normalState);

        //get cam for check room
        cam = Camera.main;
    }

    private void OnDrawGizmosSelected()
    {
        //draw radius pick of normalState
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, normalState.radiusInteract);
    }

    protected override void Update()
    {
        base.Update();

        //check current room
        CheckRoom();
    }

    public void ChangeHand()
    {
        //change hand
        usingRightHand = !usingRightHand;

        //call event
        onChangeHand?.Invoke();
    }

    #region check room

    void CheckRoom()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            //check room
            RoomGame room = hit.transform.GetComponentInParent<RoomGame>();

            //if different from current room
            if(room != currentRoom)
            {
                if (changeRoomCoroutine != null)
                    StopCoroutine(changeRoomCoroutine);

                //start coroutine to change room
                changeRoomCoroutine = StartCoroutine(ChangeRoomCoroutine(room, currentRoom));

                //change current room
                currentRoom = room;
            }
        }
    }

    IEnumerator ChangeRoomCoroutine(RoomGame roomToActivate, RoomGame roomToDeactivate)
    {
        //active new room
        roomToActivate.gameObject.SetActive(true);

        //if there is no room to deactivate, set final cam position and stop coroutine
        if(roomToDeactivate == null)
        {
            cam.transform.position = roomToActivate.cameraPosition.position;
            cam.transform.rotation = roomToActivate.cameraPosition.rotation;

            yield break;
        }

        //else move cam smooth to position and rotation
        float delta = 0;
        while (delta < 1)
        {
            delta += Time.deltaTime / roomToActivate.timeToMoveCamera;
            cam.transform.position = Vector3.Lerp(roomToDeactivate.cameraPosition.position, roomToActivate.cameraPosition.position, delta);
            cam.transform.rotation = Quaternion.Lerp(roomToDeactivate.cameraPosition.rotation, roomToActivate.cameraPosition.rotation, delta);

            yield return null;
        }

        //deactive old room
        roomToDeactivate.gameObject.SetActive(false);
    }

    #endregion
}
