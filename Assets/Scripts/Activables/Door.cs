using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Activable
{
    protected override void Active()
    {
        //open door
        objectToControl.SetActive(false);
    }

    protected override void Deactive()
    {
        //close door
        objectToControl.SetActive(true);
    }
}
