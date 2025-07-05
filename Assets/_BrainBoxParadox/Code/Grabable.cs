using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabable : MonoBehaviour
{
    public Rigidbody rb;
    public Collider grabCollider;
    public bool grabbed = false;

    public event Action OnGrabbed;
    public event Action OnDropped;

    private FloatEffect floatEffect;
    
    //Layer Related
    private int originalLayer;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            Debug.LogWarning("No Rigidbody found on " + gameObject.name + ". Adding one automatically.");
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        grabCollider = GetComponent<Collider>();
        floatEffect = GetComponent<FloatEffect>();
        
        originalLayer = gameObject.layer;
    }
    
    public void Grab()
    {
        grabbed = true;
        rb.isKinematic = true;
        grabCollider.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        //Disable Float Effect upon being picked up
        if (floatEffect)
            floatEffect.enabled = false;
        
        OnGrabbed?.Invoke();
        
    }

    public void Drop()
    {
        grabbed = false;
        rb.isKinematic = false;
        grabCollider.enabled = true;
        gameObject.layer = originalLayer;
        
        OnDropped?.Invoke();
    }
}
