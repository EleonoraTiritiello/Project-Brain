using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class RoomGame : Room
{
    [Header("Camera Position")]
    public Transform cameraPosition = default;
    public float timeToMoveCamera = 1;

    [Header("This room alternatives")]
    [SerializeField] float precisionPosition = 0.1f;
    [SerializeField] List<RoomGame> roomAlternatives = new List<RoomGame>();

    Transform cam;

    public Door enterDoor { get; set; }
    public List<Door> openedDoors { get; set; } = new List<Door>();

    public override IEnumerator EndRoom()
    {
        //foreach alternative
        foreach (RoomGame alternative in roomAlternatives)
        {
            //find one with same doors
            if (SameDoors(alternative.doors))
            {
                RegenRoom(alternative);
                break;
            }
        }

        //wait next frame (so room is already instatiated)
        yield return null;
    }

    #region select alternative

    bool SameDoors(List<DoorStruct> alternativeDoors)
    {
        //do only if same number of doors
        if (alternativeDoors.Count != usedDoors.Count)
            return false;

        //copy used doors
        List<DoorStruct> doorsToCheck = new List<DoorStruct>(usedDoors);

        //foreach alternative door, check if there is the same door in doorsToCheck
        foreach(DoorStruct alternativeDoor in alternativeDoors)
        {
            foreach(DoorStruct door in doorsToCheck)
            {
                if(Vector3.Distance(alternativeDoor.doorTransform.localPosition, door.doorTransform.localPosition) < precisionPosition &&       //check door transform has same local position
                    alternativeDoor.direction == door.direction &&                                                                              //check same direction
                    alternativeDoor.typeOfDoor == door.typeOfDoor)                                                                              //check same type
                {
                    //remove from doorsToCheck and go to next alternativeDoor
                    doorsToCheck.Remove(door);
                    break;
                }
            }
        }

        //if no doors to check, all the doors are the same
        return doorsToCheck.Count <= 0;
    }

    RoomGame RegenRoom(RoomGame roomPrefab)
    {
        //instantiate new room
        RoomGame room = Instantiate(roomPrefab, transform.parent);
        room.transform.position = transform.position;
        room.transform.rotation = transform.rotation;

        //register room (no set adjacent room and so on, cause also other rooms will be destroyed)
        room.Register(id, teleported);

        //and destroy this room
        Destroy(gameObject);

        return room;
    }

    #endregion

    #region public API

    public void EnterRoom()
    {
        //start coroutine
        StartCoroutine(MoveCameraCoroutine());
    }

    public void ForceOpenCloseEnterDoor(bool open)
    {
        if (enterDoor)
        {
            //force opening/closing enter door
            enterDoor.ForceOpenCloseDoor(open);

            //force opening/closing also other opened doors in the room
            foreach (Door door in openedDoors)
                door.ForceOpenCloseDoor(open);
        }
    }

    IEnumerator MoveCameraCoroutine()
    {
        //be sure there is cameraPosition
        if (cameraPosition == null)
        {
            Debug.LogWarning($"Manca la camera position nella camera {gameObject.name}");
            yield break;
        }

        //get cam if null
        if (cam == null)
            cam = Camera.main.transform;

        //set vars
        Vector3 startPosition = cam.position;
        Quaternion startRotation = cam.rotation;

        //move cam smooth to position and rotation
        float delta = 0;
        while (delta < 1)
        {
            delta += Time.deltaTime / timeToMoveCamera;
            cam.transform.position = Vector3.Lerp(startPosition, cameraPosition.position, delta);
            cam.transform.rotation = Quaternion.Lerp(startRotation, cameraPosition.rotation, delta);

            yield return null;
        }
    }

    #endregion
}
