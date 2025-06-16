using System;
using UnityEngine;

public class Grabable : MonoBehaviour
{
    public Rigidbody rb;
    public Collider grabCollider;
    public bool grabbed = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabCollider = GetComponent<Collider>();
    }

    public void Grab()
    {
        grabbed = true;
        rb.isKinematic = true;
        grabCollider.enabled = false;
    }

    public void Drop()
    {
        grabbed = false;
        rb.isKinematic = false;
        grabCollider.enabled = true;
    }
}
