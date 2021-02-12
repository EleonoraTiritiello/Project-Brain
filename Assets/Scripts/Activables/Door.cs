using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Activable
{
    protected override void Active()
    {
        //open door
        gameObject.SetActive(false);
    }

    protected override void Deactive()
    {
        //close door
        gameObject.SetActive(true);
    }
}
