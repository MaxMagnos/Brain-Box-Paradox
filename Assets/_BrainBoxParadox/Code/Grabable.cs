using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabable : MonoBehaviour
{
    public Rigidbody rb;
    public Collider grabCollider;
    public bool grabbed = false;

    private FloatEffect floatEffect;
    
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
    }

    public void Grab()
    {
        grabbed = true;
        rb.isKinematic = true;
        grabCollider.enabled = false;

        //Disable Float Effect upon being picked up
        if (floatEffect)
            floatEffect.enabled = false;
    }

    public void Drop()
    {
        grabbed = false;
        rb.isKinematic = false;
        grabCollider.enabled = true;
    }
}
