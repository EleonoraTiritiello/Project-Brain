using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Activable
{
    bool calledFromConnectedDoor;
    bool forceDoor;
    List<Door> connectedDoors = new List<Door>();

    RoomGame RoomParent;

    void Awake()
    {
        //save room parent
        RoomParent = GetComponentInParent<RoomGame>();
    }

    protected override void Active()
    {
        //if player opened a door that was already the enter door - call in the room parent to force open enter door, and stop
        if(calledFromConnectedDoor == false && forceDoor == false && RoomParent.enterDoor == this)
        {
            RoomParent.ForceOpenCloseEnterDoor(true);
            return;
        }

        //open door
        OpenCloseDoor(true);

        //only if not called from connected door and not forced
        if (calledFromConnectedDoor == false && forceDoor == false)
        {
            //active every connected door
            CallConnectedDoors(true);

            //activate next room
            ActiveDeactiveConnectedRooms(true);

            //when open a door, force open also room's enter door
            RoomParent.ForceOpenCloseEnterDoor(true);

            //add this door to opened doors of this room
            RoomParent.openedDoors.Add(this);
        }

        //if called from connected door
        if(calledFromConnectedDoor)
        {
            //if no enter door, now this one is the enter door
            if(RoomParent.enterDoor == null)
            {
                RoomParent.enterDoor = this;
            }

            //if this one is the enter door
            if (RoomParent.enterDoor == this)
            {
                //reactive every other opened door in this room
                foreach (Door openedDoor in RoomParent.openedDoors)
                {
                    openedDoor.Active();
                }
            }
        }
    }

    protected override void Deactive()
    {
        //close door
        OpenCloseDoor(false);

        //only if not called from connected door and not forced
        if (calledFromConnectedDoor == false && forceDoor == false)
        {
            //deactive every connected door
            CallConnectedDoors(false);

            //deactive other rooms
            ActiveDeactiveConnectedRooms(false);

            //remove this door from opened doors of this room
            RoomParent.openedDoors.Remove(this);

            //force closing enter door of this room (like when pick rope from generator, because player is resolving puzzle again)
            RoomParent.ForceOpenCloseEnterDoor(false);
        }

        //if called from connected door
        if(calledFromConnectedDoor)
        {
            //if is the enter door of other room
            if (RoomParent.enterDoor == this)
            {
                //close every other opened door in this room
                foreach(Door openedDoor in RoomParent.openedDoors)
                {
                    openedDoor.Deactive();
                }

                //this one is no more the enter door
                RoomParent.enterDoor = null;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        //if hit player
        Player player = other.GetComponentInParent<Player>();
        if (player)
        {
            //check if enter in room, then call EnterRoom and deactivate previous rooms
            if (CheckTargetIsEnteringRoom(player.transform))
            {
                RoomParent.EnterRoom();
                ActiveDeactiveConnectedRooms(false);
            }
            //if exit from room, be sure connected rooms are active (if player come back to previous room, without connect door, it must to reactivate)
            else
            {
                ActiveDeactiveConnectedRooms(true);
            }
        }
    }

    #region private API

    void OpenCloseDoor(bool open)
    {
        foreach (Renderer rend in ObjectToControl.GetComponentsInChildren<Renderer>())
        {
            //disable/enable renderer
            rend.enabled = !open;
        }
        foreach (Collider col in ObjectToControl.GetComponentsInChildren<Collider>())
        {
            //set collider trigger/NOTTrigger
            col.isTrigger = open;
        }
    }

    void CallConnectedDoors(bool active)
    {
        //foreach connected door
        foreach (Door door in connectedDoors)
        {
            if (door != null)
            {
                //set we are calling it
                door.calledFromConnectedDoor = true;

                //active or deactive
                if (active)
                    door.Active();
                else
                    door.Deactive();

                //stop calling
                door.calledFromConnectedDoor = false;
            }
        }
    }

    bool CheckTargetIsEnteringRoom(Transform target)
    {
        //get door distance and target distance from room center
        float doorDistanceFromRoom = Vector3.Distance(transform.position, RoomParent.transform.position);
        float targetDistanceFromRoom = Vector3.Distance(target.position, RoomParent.transform.position);

        //if target is near than door, then is entering
        return targetDistanceFromRoom < doorDistanceFromRoom;
    }

    void ActiveDeactiveConnectedRooms(bool active)
    {
        //foreach connected door, activate/deactive room
        foreach (Door door in connectedDoors)
        {
            if (door != null)
            {
                door.RoomParent.gameObject.SetActive(active);
            }
        }
    }

    #endregion

    #region public API

    public void AddConnectedDoors(List<Door> doors)
    {
        foreach (Door door in doors)
        {
            //be sure is not this door and is not already in the list
            if (door != this && connectedDoors.Contains(door) == false)
            {
                connectedDoors.Add(door);
            }
        }
    }

    public void ForceOpenCloseDoor(bool open)
    {
        forceDoor = true;

        //force to open or close door
        if (open)
            Active();
        else
            Deactive();

        forceDoor = false;
    }

    #endregion
}
