using System;
using UnityEngine;
using DG.Tweening;

public class SnapPoint : MonoBehaviour
{
    [SerializeField] private Grabable occupyingObject;
    
    private void OnTriggerStay(Collider other)
    {
        if (occupyingObject)    //If SnapPoint is occupied do nothing
        {
            return;
        }
        
        var otherGrabable = other.GetComponent<Grabable>();

        if (otherGrabable.grabbed)
        {
            return;
        }
        
        var rb = other.GetComponent<Rigidbody>();
        rb?.DOMove(transform.position, 0.5f).SetEase(Ease.OutQuint);
        rb.isKinematic = true;
    }

    private void FixedUpdate()
    {
        if (!occupyingObject)
        {
            return;
        }
        
        if (occupyingObject.grabbed)
        {
            occupyingObject = null;
        }
        else
        {
            
        }
    }
}
