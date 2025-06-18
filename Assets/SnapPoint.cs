using System;
using UnityEngine;
using DG.Tweening;

public class SnapPoint : MonoBehaviour
{
    [SerializeField] private Collider snapCollider;
    [SerializeField] private Grabable occupyingObject;

    private void Awake()
    {
        snapCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (occupyingObject)    //If SnapPoint is occupied do nothing
        {
            return;
        }
        
        var otherGrabable = other.GetComponent<Grabable>();

        if (otherGrabable.grabbed || !otherGrabable)    //Do nothing if grabable is already grabbed or returned empty
        {
            return;
        }
        
        SetOccupyingObject(otherGrabable);
    }

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
        occupyingObject = grabable;
        occupyingObject.rb.DOMove(transform.position, 0.5f).SetEase(Ease.OutQuint);
        occupyingObject.rb.isKinematic = true;
        snapCollider.enabled = false;       //Disable collider for SnapPoint since it's not needed while it has an occupying object.
    }

    private void ClearOccupyingObject()
    {
        occupyingObject = null;
        snapCollider.enabled = true;
    }
}
