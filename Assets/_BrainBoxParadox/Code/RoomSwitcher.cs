using UnityEngine;

// Enum should be defined outside the class or in its own file
enum RoomVariant
{
    A,
    B
}

public class RoomSwitcher : MonoBehaviour
{
    [SerializeField] private Vector3 anchor_A;
    [SerializeField] private Vector3 anchor_B;

    private GameObject xrOrigin;
    [SerializeField] private GrabHandler grabHandler;
    private RoomVariant currentVariant = RoomVariant.A;

    private void Awake()
    {
        // It's safer to get references in Awake or Start
        xrOrigin = this.gameObject;
    }

    private void OnEnable()
    {
        // Assuming InputHandler is a singleton you've created
        InputHandler.Ins.OnRoomStateSwitch += SwitchRoom;
    }

    private void OnDisable()
    {
        InputHandler.Ins.OnRoomStateSwitch -= SwitchRoom;
    }
    private void SwitchRoom()
    {
        var grabbedObject = grabHandler.GetGrabbedObject();
        Rigidbody grabbedRb = null;
        RigidbodyInterpolation originalInterpolation = RigidbodyInterpolation.None;
        Vector3? grabbedObjectLocalOffset = null; 

        // --- PREPARE ---
        if (grabbedObject != null)
        {
            // 1. Get the Rigidbody and store its current interpolation setting
            grabbedRb = grabbedObject.GetComponent<Rigidbody>();
            originalInterpolation = grabbedRb.interpolation;

            // 2. Disable interpolation to allow for an instant teleport
            grabbedRb.interpolation = RigidbodyInterpolation.None;

            // 3. Store the object's position relative to the player
            grabbedObjectLocalOffset = xrOrigin.transform.InverseTransformPoint(grabbedObject.transform.position);
        }

        // --- TELEPORT THE PLAYER ---
        Vector3 targetPosition = (currentVariant == RoomVariant.A) ? anchor_B : anchor_A;
        xrOrigin.transform.position = targetPosition;
        currentVariant = (currentVariant == RoomVariant.A) ? RoomVariant.B : RoomVariant.A;

        // --- TELEPORT THE GRABBED OBJECT AND RESET STATE ---
        if (grabbedObject != null && grabbedObjectLocalOffset.HasValue)
        {
            // 4. Move the object using its Rigidbody for better physics handling
            Vector3 newObjectPosition = xrOrigin.transform.TransformPoint(grabbedObjectLocalOffset.Value);
            grabbedRb.position = newObjectPosition;

            // 5. CRITICAL: Reset the grab handler's velocity to prevent "catch-up"
            grabHandler.ResetGrabVelocity();

            // 6. Restore the original interpolation setting
            grabbedRb.interpolation = originalInterpolation;
        }
    }

    // SetAnchors method remains the same
    public void SetAnchors(Vector3 newAnchorA, Vector3? newAnchorB = null)
    {
        anchor_A = newAnchorA;
        anchor_B = newAnchorB.HasValue ? newAnchorB.Value : newAnchorA;

        // Start at B so the first switch goes to A
        currentVariant = RoomVariant.B;
        SwitchRoom();
    }
}