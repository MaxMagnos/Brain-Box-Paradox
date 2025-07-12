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

    [Header("Grab Tuning")]
    public float grabRange = 5f; // Max distance you can grab from
    public float grabbedDistance = 3f; // How far the object is held from you

    [Header("Grab Detection")]
    [Tooltip("The radius of the sphere used to detect grabbable objects.")]
    public float grabRadius = 0.5f;
    [Tooltip("Set this to the layer your grabbable objects are on.")]
    public LayerMask grabbableLayer;

    [Header("Movement Damping")]
    public float maxDampDistance;
    public float dampMinSpeed;
    public float dampMaxSpeed;
    
    [Tooltip("Curve of damp-speed for grabbed objects.\n X-Axis: How far away object is from target-position (0 = at target position).\n Y-Axis: speed-multiplier (0 = minSpeed, 1 = maxSpeed")]
    public AnimationCurve dampCurve;

    private void OnEnable()
    {
        if (!mainCamera)
        {
            mainCamera = Camera.main;
        }

        InputHandler.Ins.OnGrabPressed += Grab;
        InputHandler.Ins.OnGrabReleased += Drop;
    }

    private void OnDisable()
    {
        InputHandler.Ins.OnGrabPressed -= Grab;
        InputHandler.Ins.OnGrabReleased -= Drop;
    }

    void Update()
    {
        HandleMove();
    }
    
    public void Grab()
    {
        if (!currentGrabable)
        {
            currentGrabable = GetGrabable();
        }
    }

    public void Drop()
    {
        if (currentGrabable)
        {
            targetedSnapPoint = CheckForSnapPoint();
            currentGrabable.Drop();
            if (targetedSnapPoint)
            {
                targetedSnapPoint.SetOccupyingObject(currentGrabable);
            }
            currentGrabable = null;
        }
    }
    
    /// <summary>
    /// Uses a SphereCast to find a grabbable object in front of the camera.
    /// This is more reliable than a Raycast for grabbing, as it detects close objects.
    /// </summary>
    public Grabable GetGrabable()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        // Use SphereCast instead of Raycast. It's like a thick ray.
        if (Physics.SphereCast(ray, grabRadius, out hit, grabRange, grabbableLayer))
        {
            Grabable grabable = hit.collider.gameObject.GetComponent<Grabable>();
            if (grabable != null)
            {
                grabable.Grab();
                // This rotation logic remains exactly as you had it.
                rotationOffset = Quaternion.Inverse(mainCamera.transform.rotation) * grabable.transform.rotation;
                rotationOffset = Quaternion.Euler(0, rotationOffset.eulerAngles.y, 0);
                return grabable;
            }
        }
        
        // Return null if no grabbable object was hit
        return null;
    }
    
    void HandleMove()
    {
        if (currentGrabable)
        {
            targetPosition = mainCamera.transform.position + mainCamera.transform.forward * grabbedDistance;
            float totalDistance = (targetPosition - currentGrabable.transform.position).magnitude;
            float moveSpeed = Mathf.Lerp(dampMinSpeed, dampMaxSpeed, dampCurve.Evaluate(totalDistance / maxDampDistance));
            Vector3 newPosition = Vector3.MoveTowards(currentGrabable.transform.position, targetPosition, Time.deltaTime * moveSpeed);
            currentGrabable.rb.MovePosition(newPosition);
            
            var newRotation = mainCamera.transform.rotation.eulerAngles + rotationOffset.eulerAngles;
            newRotation.x = currentGrabable.transform.rotation.eulerAngles.x;
            currentGrabable.rb.rotation = Quaternion.Euler(newRotation);
        }
    }
    
    void OnDrawGizmos()
    {
        if (mainCamera != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 startPoint = mainCamera.transform.position;
            Vector3 endPoint = startPoint + mainCamera.transform.forward * grabRange;
            
            // Draw a wire sphere at the start and end of the cast to visualize the radius
            Gizmos.DrawWireSphere(startPoint, grabRadius);
            Gizmos.DrawWireSphere(endPoint, grabRadius);
            
            // Draw lines to connect the spheres, representing the path of the SphereCast
            Gizmos.DrawLine(startPoint + mainCamera.transform.right * grabRadius, endPoint + mainCamera.transform.right * grabRadius);
            Gizmos.DrawLine(startPoint - mainCamera.transform.right * grabRadius, endPoint - mainCamera.transform.right * grabRadius);
            Gizmos.DrawLine(startPoint + mainCamera.transform.up * grabRadius, endPoint + mainCamera.transform.up * grabRadius);
            Gizmos.DrawLine(startPoint - mainCamera.transform.up * grabRadius, endPoint - mainCamera.transform.up * grabRadius);
        }
    }

    SnapPoint CheckForSnapPoint()
    {
        int layerMask = 1 << LayerMask.NameToLayer("SnapPoint");
        
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, grabRange, layerMask))
        {
            var snapPoint = hit.collider.gameObject.GetComponent<SnapPoint>();
            if(snapPoint != null) {Debug.Log("Snappoint Hit on Drop: " + snapPoint.name);}
            return snapPoint;
        }

        return null;
    }

    public GameObject GetGrabbedObject()
    {
        if (currentGrabable)
        {
            return currentGrabable.gameObject;
        }
        else
        {
            return null;
        }
    }
}