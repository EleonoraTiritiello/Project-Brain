using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Activable
{
    bool calledFromConnectedDoor;
    public List<Door> connectedDoors = new List<Door>();

    protected override void Active()
    {
        //open door
        ObjectToControl.SetActive(false);

        //active every connected door
        if (calledFromConnectedDoor == false)       //only if not already called from another connected door
        {
            CallConnectedDoors(true);
        }
    }

    protected override void Deactive()
    {
        //close door
        ObjectToControl.SetActive(true);

        //deactive every connected door
        if (calledFromConnectedDoor == false)       //only if not already called from another connected door
        {
            CallConnectedDoors(false);
        }
    }

    #region API

    public void AddConnectedDoors(List<Door> doors)
    {
        //clear list (if this function is called, then a room is destroyed with its doors, so there is a null reference in the list)
        connectedDoors.Clear();

        foreach (Door door in doors)
        {
            //be sure is not this door
            if (door != this)
            {
                connectedDoors.Add(door);
            }
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
                calledFromConnectedDoor = true;

                //active or deactive
                if (active)
                    door.Active();
                else
                    door.Deactive();

                //stop calling
                calledFromConnectedDoor = false;
            }
        }
    }

    #endregion
}
