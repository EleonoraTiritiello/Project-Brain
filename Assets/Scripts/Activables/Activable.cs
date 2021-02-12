using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activable : MonoBehaviour
{
    [Header("Activable")]
    [Tooltip("Need every object in the list to activate, or only one?")] [SerializeField] bool needEveryObjectInTheList = true;
    [Tooltip("List of objects necessary to activate this activable")] [SerializeField] List<Interactable> objectsForActivate = new List<Interactable>();

    bool isActive;
    List<Interactable> alreadyActiveObjects = new List<Interactable>();

    /// <summary>
    /// Function to activate or deactivate object
    /// </summary>
    /// <param name="interactable">interactable used to call this function</param>
    /// <param name="active">try activate object when true, or deactivate when false</param>
    public virtual void ToggleObject(Interactable interactable, bool active)
    {
        //if interactable is inside the list
        if(objectsForActivate.Contains(interactable))
        {
            if (active)
            {
                //add to the list and try activate
                AddElementInTheList(interactable);
                TryActivate();
            }
            else
            {
                //remove from the list and try deactivate
                RemoveElementFromTheList(interactable);
                TryDeactivate();
            }
        }
    }

    protected abstract void Active();

    protected abstract void Deactive();

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

        if( (needEveryObjectInTheList == false && alreadyActiveObjects.Count > 0)   //if doesn't need every element and there is at least one
            || alreadyActiveObjects.Count >= objectsForActivate.Count)               //or if there are all the elements in the list
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

        if((needEveryObjectInTheList && alreadyActiveObjects.Count < objectsForActivate.Count)   //if need every element in the list but there aren't all
            || alreadyActiveObjects.Count <= 0)                                                 //or if there isn't neither one
        {
            isActive = false;
            Deactive();
        }
    }

    #endregion
}
