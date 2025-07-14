using System;
using UnityEngine;

public class Lightbulb : MonoBehaviour
{
    private MorphHandler morphHandler;

    private Collider snapCollider;
    
    private Grabable grabable;

    private void Awake()
    {
        morphHandler = gameObject.GetComponentInParent<MorphHandler>();
        snapCollider = gameObject.GetComponent<Collider>();
        grabable = gameObject.GetComponentInParent<Grabable>();
    }

    private void OnEnable()
    {
        if (grabable != null)
        {
            grabable.OnGrabbed += DisableSnapCollider;
            grabable.OnDropped += EnableSnapCollider;
        }
        else
        {
            
        }
    }

    private void OnDisable()
    {
        if (grabable != null)
        {
            grabable.OnGrabbed -= DisableSnapCollider;
            grabable.OnDropped -= EnableSnapCollider;
        }
    }

    private void DisableSnapCollider()
    {
        snapCollider.enabled = false;
    }

    private void EnableSnapCollider()
    {
        snapCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter: " + other.gameObject.name);
        if (other.CompareTag("LightOrb"))
        {
            Debug.Log("LightOrb entered");
            morphHandler.TurnOnLightbulb();
            Destroy(other.transform.parent.gameObject);
        }
    }
}
