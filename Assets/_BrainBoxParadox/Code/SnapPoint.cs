using System;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class SnapPoint : MonoBehaviour
{
    [SerializeField] private Collider snapCollider;
    [SerializeField] private Grabable occupyingObject;
    [SerializeField] private int occupyingObjectShapeID;

    public bool isLightbulb;
    
    public event Action<int, GameObject> OnObjectPlaced;
    
    private void Awake()
    {
        snapCollider = GetComponent<SphereCollider>();
    }

    // NOTE: Currently Deactivated as there is no reason in current game design for objects to snap on contact
    //
    // private void OnTriggerStay(Collider other)
    // {
    //     if (occupyingObject)    //If SnapPoint is occupied do nothing
    //     {
    //         return;
    //     }
    //     
    //     var otherGrabable = other.GetComponent<Grabable>();
    //
    //     if (!otherGrabable || otherGrabable.grabbed)    //Do nothing if grabable is already grabbed or returned empty
    //     {
    //         return;
    //     }
    //     
    //     SetOccupyingObject(otherGrabable);
    // }

    private void FixedUpdate()
    {
        if (!occupyingObject)
        {
            return;
        }
        
        if (occupyingObject.grabbed)
        {
            ClearOccupyingObject();
        }
    }

    public void SetOccupyingObject(Grabable grabable)
    {
        
        if (isLightbulb)
        {
            if (grabable.GetComponent<MorphHandler>()?.GetShapeID() != 0)
            {
                return;
            }
            snapCollider.enabled = true; //Re-Enable collider for the Lightbulb Trigger to work         //TODO: This entire thing could/should be replaced by a coroutine of some sort. Using a physical-check to turn on the lightbulb is not very efficient.
        }
        else
        {
            snapCollider.enabled = false;       //Disable collider for SnapPoint since it's not needed while it has an occupying object.
        }
        
        occupyingObject = grabable;
        
        occupyingObjectShapeID = occupyingObject.gameObject.GetComponent<MorphHandler>()?.GetShapeID() ?? -1;   //Returns -1 if MorphHandler isn't found
        
        occupyingObject.rb.isKinematic = true;
        occupyingObject.rb.DOMove(transform.position, 0.5f).SetEase(Ease.OutQuint)
            .OnComplete(ObjectPlaced);

    }

    private void ObjectPlaced()
    {
        Debug.Log("ObjectPlaced");
        OnObjectPlaced?.Invoke(occupyingObjectShapeID, occupyingObject.gameObject);
    }
    
    public GameObject GetOccupyingObject()
    {
        if (occupyingObject != null)
        {
            return occupyingObject.gameObject;
        }
        return null;
    }

    private void ClearOccupyingObject()
    {
        occupyingObject = null;
        occupyingObjectShapeID = -1;
        snapCollider.enabled = true;
    }
}
