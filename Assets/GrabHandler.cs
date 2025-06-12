using System;
using UnityEngine;

public class GrabHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject HitDebugPoint;
    [SerializeField] private GameObject grabbedObject;

    private Vector3 targetPosition;

    [Header("Variables to Change")]
    public float grabRange;
    public float grabbedDistance;

    public float maxDampDistance;
    public float dampMinSpeed;
    public float dampMaxSpeed;
    
    [Tooltip("Curve of damp-speed for grabbed objects.\n X-Axis: How far away object is from target-position (0 = at target position).\n Y-Axis: speed-multiplier (0 = minSpeed, 1 = maxSpeed")]
    public AnimationCurve dampCurve;
    
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
        if (Physics.Raycast(ray, out hit, grabRange))
        {
            //Instantiate(HitDebugPoint, hit.point, Quaternion.identity);   //Instantiate a HitDebugPoint at Raycast-Hit position to visualize it
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

    /// <summary>
    /// Moves Rigidbody of grabbed Object towards desired position (look direction).
    /// Uses Dampening with a variety of factors
    /// </summary>
    void HandleMove()
    {
        if (grabbedObject)
        {
            targetPosition = mainCamera.transform.position + mainCamera.transform.forward * grabbedDistance;
            float totalDistance = (targetPosition - grabbedObject.transform.position).magnitude;
            float moveSpeed = Mathf.Lerp(dampMinSpeed, dampMaxSpeed, dampCurve.Evaluate(totalDistance / maxDampDistance));
            Vector3 newPosition = Vector3.MoveTowards(grabbedObject.transform.position, targetPosition, Time.deltaTime * moveSpeed);
            grabbedObject.GetComponent<Rigidbody>().MovePosition(newPosition);
        }
    }
    
    
    void OnDrawGizmos()
    {
        if (mainCamera != null)
        {
            // Set the color of the Gizmo ray to yellow.
            Gizmos.color = Color.yellow;
            // Draw a line that matches the raycast's path and distance.
            Gizmos.DrawLine(mainCamera.transform.position, mainCamera.transform.position + mainCamera.transform.forward * grabRange);
        }
    }
}
