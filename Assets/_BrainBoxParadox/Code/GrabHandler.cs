using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GrabHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SnapPoint targetedSnapPoint;
    [SerializeField] private Grabable currentGrabable;

    private Vector3 targetPosition;
    private Quaternion rotationOffset;

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
        targetedSnapPoint = CheckForSnapPoint();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Pressed");
            if (currentGrabable)
            {
                currentGrabable.Drop();
                if (targetedSnapPoint)
                {
                    targetedSnapPoint.SetOccupyingObject(currentGrabable);
                }
                else
                {
                }
                currentGrabable = null;
            }
            else
            {
                currentGrabable = TryGrab();
            }
        }
        
        HandleMove();
    }

    public Grabable TryGrab()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, grabRange))
        {
            //Instantiate(HitDebugPoint, hit.point, Quaternion.identity);   //Instantiate a HitDebugPoint at Raycast-Hit position to visualize it
            Grabable grabable = hit.collider.gameObject.GetComponent<Grabable>();
            if (grabable != null)
            {
                grabable.Grab();
                rotationOffset = Quaternion.Inverse(mainCamera.transform.rotation) * grabable.transform.rotation;
                rotationOffset = Quaternion.Euler(0, rotationOffset.eulerAngles.y, 0);
                return grabable;
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
        if (currentGrabable)
        {
            //Move Position of Grabbed Object
            targetPosition = mainCamera.transform.position + mainCamera.transform.forward * grabbedDistance;
            float totalDistance = (targetPosition - currentGrabable.transform.position).magnitude;
            float moveSpeed = Mathf.Lerp(dampMinSpeed, dampMaxSpeed, dampCurve.Evaluate(totalDistance / maxDampDistance));
            Vector3 newPosition = Vector3.MoveTowards(currentGrabable.transform.position, targetPosition, Time.deltaTime * moveSpeed);
            currentGrabable.rb.MovePosition(newPosition);
            
            //Change Rotation of Grabbed Object relative to camera
            var newRotation = mainCamera.transform.rotation.eulerAngles + rotationOffset.eulerAngles;
            newRotation.x = currentGrabable.transform.rotation.eulerAngles.x;
            currentGrabable.rb.rotation = Quaternion.Euler(newRotation);
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

    SnapPoint CheckForSnapPoint()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, grabRange))
        {
            var snapPoint = hit.collider.gameObject.GetComponent<SnapPoint>();
            return snapPoint;
        }

        return null;
    }
}
