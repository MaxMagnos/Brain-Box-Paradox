using System;
using UnityEngine;

public class GrabHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Grabable currentGrabable;
    private SnapPoint targetedSnapPoint;

    private Vector3 targetPosition;
    private Quaternion rotationOffset;

    // For SmoothDamp functionality
    private Vector3 currentMoveVelocity;
    private Quaternion currentRotationVelocity;


    [Header("Grab Settings")]
    [Tooltip("Max distance from which an object can be grabbed.")]
    public float grabRange = 5f;
    [Tooltip("How far the object is held from the camera.")]
    public float grabbedDistance = 3f;
    [Tooltip("The radius of the sphere used to detect grabbable objects.")]
    public float grabRadius = 0.5f;
    [Tooltip("The layer your grabbable objects are on.")]
    public LayerMask grabbableLayer;

    [Header("Movement Smoothing")]
    [Tooltip("Time it takes for the grabbed object to reach the target position. Smaller values are faster.")]
    public float moveSmoothTime = 0.08f;
    [Tooltip("Time it takes for the grabbed object to match the target rotation. Smaller values are faster.")]
    public float rotationSmoothTime = 0.08f;


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

    private void FixedUpdate()
    {
        // Physics-related code should be in FixedUpdate for consistency.
        HandleMove();
    }

    public void Grab()
    {
        if (currentGrabable) return;

        currentGrabable = GetGrabable();
        if (currentGrabable)
        {
            currentGrabable.Grab();
            // Calculate the rotation difference between the camera and the object.
            rotationOffset = Quaternion.Inverse(mainCamera.transform.rotation) * currentGrabable.transform.rotation;
        }
    }

    public void Drop()
    {
        if (!currentGrabable) return;

        targetedSnapPoint = CheckForSnapPoint();
        currentGrabable.Drop();
        if (targetedSnapPoint)
        {
            targetedSnapPoint.SetOccupyingObject(currentGrabable);
        }
        currentGrabable = null;
    }

    private Grabable GetGrabable()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        if (Physics.SphereCast(ray, grabRadius, out RaycastHit hit, grabRange, grabbableLayer))
        {
            if (hit.collider.TryGetComponent<Grabable>(out Grabable grabable))
            {
                return grabable;
            }
        }
        return null;
    }

    private void HandleMove()
    {
        if (!currentGrabable) return;

        // 1. Calculate Target Position & Rotation
        targetPosition = mainCamera.transform.position + mainCamera.transform.forward * grabbedDistance;
        Quaternion targetRotation = mainCamera.transform.rotation * rotationOffset;

        // 2. Smoothly move the Rigidbody to the target position
        Vector3 newPosition = Vector3.SmoothDamp(
            currentGrabable.transform.position,
            targetPosition,
            ref currentMoveVelocity,
            moveSmoothTime
        );
        currentGrabable.rb.MovePosition(newPosition);

        // 3. Smoothly rotate the Rigidbody to the target rotation
        // We use Slerp (Spherical Linear Interpolation) for smooth rotation.
        Quaternion newRotation = Slerp(
            currentGrabable.rb.rotation,
            targetRotation,
            ref currentRotationVelocity,
            rotationSmoothTime
        );
        currentGrabable.rb.MoveRotation(newRotation);
    }

    /// <summary>
    /// A custom Slerp function that works like SmoothDamp for Quaternions.
    /// </summary>
    private Quaternion Slerp(Quaternion current, Quaternion target, ref Quaternion velocity, float smoothTime)
    {
        // This is a simplified approach to quaternion damping.
        // For more complex physics, a full PID controller might be needed.
        float t = 1f - Mathf.Exp(-1f / smoothTime * Time.fixedDeltaTime);
        return Quaternion.Slerp(current, target, t);
    }


    private SnapPoint CheckForSnapPoint()
    {
        int layerMask = 1 << LayerMask.NameToLayer("SnapPoint");
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, grabRange, layerMask))
        {
            if (hit.collider.TryGetComponent<SnapPoint>(out SnapPoint snapPoint))
            {
                Debug.Log("SnapPoint Hit on Drop: " + snapPoint.name);
                return snapPoint;
            }
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        if (mainCamera == null) return;

        Gizmos.color = Color.yellow;
        Vector3 startPoint = mainCamera.transform.position;
        Vector3 endPoint = startPoint + mainCamera.transform.forward * grabRange;

        Gizmos.DrawWireSphere(startPoint, grabRadius);
        Gizmos.DrawWireSphere(endPoint, grabRadius);

        Gizmos.DrawLine(startPoint + mainCamera.transform.right * grabRadius, endPoint + mainCamera.transform.right * grabRadius);
        Gizmos.DrawLine(startPoint - mainCamera.transform.right * grabRadius, endPoint - mainCamera.transform.right * grabRadius);
        Gizmos.DrawLine(startPoint + mainCamera.transform.up * grabRadius, endPoint + mainCamera.transform.up * grabRadius);
        Gizmos.DrawLine(startPoint - mainCamera.transform.up * grabRadius, endPoint - mainCamera.transform.up * grabRadius);
    }

    public GameObject GetGrabbedObject()
    {
        return currentGrabable ? currentGrabable.gameObject : null;
    }
    
    // Add this method to your existing GrabHandler.cs script
    public void ResetGrabVelocity()
    {
        currentMoveVelocity = Vector3.zero;
    }
}