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

    // Update is called once per frame
    void Update()
    {
        HandleMove();
    }

    /// <summary>
    /// Main Function handling the high-level grab logic executed when Grab Button is pressed.
    /// </summary>
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
    
    public Grabable GetGrabable()
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
}
