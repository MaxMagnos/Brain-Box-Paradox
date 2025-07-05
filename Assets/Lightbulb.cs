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
        if (other.CompareTag("LightOrb"))
        {
            morphHandler.TurnOnLightbulb();
            Destroy(other.transform.parent.gameObject);
        }
    }
}
