using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class ProceduralMapManagerGame : ProceduralMapManager
{
    [Header("Player")]
    [SerializeField] Player playerPrefab = default;

    public override IEnumerator EndGeneration()
    {
        yield return base.EndGeneration();

        //instantiate player
        Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        //wait
        yield return new WaitForFixedUpdate();

        //foreach room, connect doors
        foreach(RoomGame room in GetComponentsInChildren<RoomGame>())
        {
            ConnectDoors(room);
        }
    }

    void ConnectDoors(RoomGame room)
    {
        //foreach door struct do overlap and get activable doors
        foreach (DoorStruct door in room.doors)
        {
            Collider[] colliders = Physics.OverlapSphere(door.doorTransform.position, 2);
            List<Door> activableDoors = new List<Door>();

            foreach (Collider col in colliders)
            {
                Door activableDoor = col.GetComponentInParent<Door>();
                if (activableDoor && activableDoors.Contains(activableDoor) == false)       //be sure is not already in the list
                {
                    activableDoors.Add(activableDoor);
                }
            }

            //save connections in every activable door
            foreach (Door activableDoor in activableDoors)
            {
                activableDoor.AddConnectedDoors(activableDoors);
            }
        }
    }
}
