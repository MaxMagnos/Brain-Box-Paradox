using System;
using UnityEngine;

public class GrabHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject HitDebugPoint;
    [SerializeField] private GameObject grabbedObject;

    [Header("Variables to Change")]
    public float grabDistance;
    
    private void Awake()
    {
        if (!mainCamera)
        {
            mainCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Pressed");
            if (grabbedObject)
            {
                grabbedObject.GetComponent<Grabable>().Drop();
                grabbedObject = null;
            }
            else
            {
                grabbedObject = TryGrab();
            }
        }
        
        HandleMove();
    }

    public GameObject TryGrab()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        Debug.Log("Attempting to Grab");
        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            Instantiate(HitDebugPoint, hit.point, Quaternion.identity);
            Grabable grabable = hit.collider.gameObject.GetComponent<Grabable>();
            if (grabable != null)
            {
                grabable.Grab();
                return grabable.gameObject;
            }
        }
        
        //Return Null if no Grabable Object was hit
        return null;
    }

    void HandleMove()
    {
        if (grabbedObject != null)
        {
            grabbedObject.transform.position = mainCamera.transform.position + mainCamera.transform.forward * 3;
        }
    }
    
    
    void OnDrawGizmos()
    {
        if (mainCamera != null)
        {
            // Set the color of the Gizmo ray to yellow.
            Gizmos.color = Color.yellow;
            // Draw a line that matches the raycast's path and distance.
            Gizmos.DrawLine(mainCamera.transform.position, mainCamera.transform.position + mainCamera.transform.forward * grabDistance);
        }
    }
}
