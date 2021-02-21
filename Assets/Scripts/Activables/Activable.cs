using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activable : MonoBehaviour
{
    [Header("Important")]
    [Tooltip("The object to activate or deactivate")] [SerializeField] GameObject objectToControl = default;

    [Header("Activable")]
    [Tooltip("How many objects of the list to activate? (-1 means all the list)")] [SerializeField] int howManyObjectsToActivate = -1;
    [Tooltip("List of objects necessary to activate this activable")] public List<Interactable> ObjectsForActivate = new List<Interactable>();

    public GameObject ObjectToControl => objectToControl != null ? objectToControl : gameObject;


    bool isActive;
    List<Interactable> alreadyActiveObjects = new List<Interactable>();
    int necessaryToActivate => howManyObjectsToActivate < 0 ? ObjectsForActivate.Count : howManyObjectsToActivate;  //necessary number or all the list

    /// <summary>
    /// Function to activate or deactivate object
    /// </summary>
    /// <param name="interactable">interactable used to call this function</param>
    /// <param name="active">try activate object when true, or deactivate when false</param>
    public virtual void ToggleObject(Interactable interactable, bool active)
    {
        //if interactable is inside the list
        if(ObjectsForActivate.Contains(interactable))
        {
            if (active)
            {
                //add to the list of already active and try activate
                AddElementInTheList(interactable);
                TryActivate();
            }
            else
            {
                //remove from the list of already active and try deactivate
                RemoveElementFromTheList(interactable);
                TryDeactivate();
            }
        }
    }

    protected abstract void Active();

    protected abstract void Deactive();

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        //draw a line to every object necessary to activate this
        foreach (Interactable interactable in ObjectsForActivate)
            Gizmos.DrawLine(interactable.ObjectToControl.transform.position + Vector3.right * 0.05f, ObjectToControl.transform.position + Vector3.right * 0.05f);   //a bit moved, to not override Interactable gizmos
    }

    #region private API

    void AddElementInTheList(Interactable interactable)
    {
        //add if not already inside the list
        if (alreadyActiveObjects.Contains(interactable) == false)
        {
            alreadyActiveObjects.Add(interactable);
        }
    }

    void RemoveElementFromTheList(Interactable interactable)
    {
        //remove interactable if inside the list of already active
        if (alreadyActiveObjects.Contains(interactable))
        {
            alreadyActiveObjects.Remove(interactable);
        }
    }

    void TryActivate()
    {
        //do only if not already active
        if (isActive)
            return;

        //if reach necessary
        if(alreadyActiveObjects.Count >= necessaryToActivate)
        {
            isActive = true;
            Active();
        }
    }

    void TryDeactivate()
    {
        //do only if not already deactive
        if (isActive == false)
            return;

        //if not reach necessary
        if(alreadyActiveObjects.Count < necessaryToActivate)
        {
            isActive = false;
            Deactive();
        }
    }

    #endregion
}
