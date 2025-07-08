using System;
using UnityEngine;

public class Lightbulb : MonoBehaviour
{
    private MorphHandler morphHandler;

    private void Start()
    {
        morphHandler = gameObject.GetComponentInParent<MorphHandler>();
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
