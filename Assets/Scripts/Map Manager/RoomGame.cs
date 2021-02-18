using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RoomAlternativesStruct
{
    public RoomGame room;
    public int numberDoors;
}

public class RoomGame : Room
{
    [Header("Camera Position")]
    public Transform cameraPosition = default;
    public float timeToMoveCamera = 1;

    [Header("This room alternatives")]
    [SerializeField] List<RoomAlternativesStruct> roomAlternatives = new List<RoomAlternativesStruct>();

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
        foreach(RoomAlternativesStruct alternative in roomAlternatives)
        {
            //find one with same number of doors
            if(alternative.numberDoors == usedDoors.Count)
            {
                RegenRoom(alternative.room);
                break;
            }
        }
    }

    void RegenRoom(RoomGame newRoom)
    {
        //instantiate new room
        RoomGame room = Instantiate(newRoom, transform.parent);
        room.transform.position = transform.position;
        room.transform.rotation = transform.rotation;

        //and destroy this one
        Destroy(gameObject);
    }
}
