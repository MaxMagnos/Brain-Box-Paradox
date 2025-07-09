using System;
using UnityEngine;
using DG.Tweening;

public class SnapPoint : MonoBehaviour
{
    [SerializeField] private Collider snapCollider;
    [SerializeField] private Grabable occupyingObject;
    [SerializeField] private int occupyingObjectShapeID;

    public bool isLightbulb;
    
    public event Action<int> OnObjectPlaced;
    
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
        occupyingObject.rb.DOMove(transform.position, 0.5f).SetEase(Ease.OutQuint);
        occupyingObject.rb.isKinematic = true;
        
        
        occupyingObjectShapeID = occupyingObject.gameObject.GetComponent<MorphHandler>()?.GetShapeID() ?? -1;   //Returns -1 if MorphHandler isn't found
        
        OnObjectPlaced?.Invoke(occupyingObjectShapeID);
    }

    public GameObject GetOccupyingObject()
    {
        return occupyingObject.gameObject;
    }

    private void ClearOccupyingObject()
    {
        occupyingObject = null;
        occupyingObjectShapeID = -1;
        snapCollider.enabled = true;
    }
}
