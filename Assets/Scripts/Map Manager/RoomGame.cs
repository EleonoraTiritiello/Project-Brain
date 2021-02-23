using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGame : Room
{
    [Header("Camera Position")]
    public Transform cameraPosition = default;
    public float timeToMoveCamera = 1;

    [Header("This room alternatives")]
    [SerializeField] float precisionPosition = 0.1f;
    [SerializeField] List<RoomGame> roomAlternatives = new List<RoomGame>();

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

    public override void CompleteRoom()
    {
        //foreach alternative
        foreach(RoomGame alternative in roomAlternatives)
        {
            //find one with same doors
            if(SameDoors(alternative.doors))
            {
                RegenRoom(alternative);
                break;
            }
        }
    }

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
                if(Vector3.Distance(alternativeDoor.door.localPosition, door.door.localPosition) < precisionPosition &&         //check door transform has same local position
                    alternativeDoor.direction == door.direction &&                                                              //check same direction
                    alternativeDoor.isOnlyExit == door.isOnlyExit)                                                              //check same bool
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

    void RegenRoom(RoomGame newRoom)
    {
        //instantiate new room
        RoomGame room = Instantiate(newRoom, transform.parent);
        room.transform.position = transform.position;
        room.transform.rotation = transform.rotation;

        //register room (no set adjacent room and so on, cause also other rooms will be destroyed)
        room.Register(id, teleported);

        //and destroy this one
        Destroy(gameObject);
    }
}
