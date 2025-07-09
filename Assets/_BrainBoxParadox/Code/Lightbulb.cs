using System;
using UnityEngine;

public class Lightbulb : MonoBehaviour
{
    private MorphHandler morphHandler;

    private Collider snapCollider;

    private void Start()
    {
        morphHandler = gameObject.GetComponentInParent<MorphHandler>();
        snapCollider = gameObject.GetComponent<Collider>();
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
