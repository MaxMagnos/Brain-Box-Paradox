using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Multiplier for mouse sensitivity. Default is 1.")]
    public float mouseSpeed = 1f;
    
    [Tooltip("Maximum angle (in degrees) the player can look up or down.")]
    public float verticalClamp = 90f;

    [Header("References")]
    [Tooltip("Assign the parent Player object here to rotate the body horizontally. If left empty, the camera will rotate itself.")]
    public Transform playerBody;

    // Internal state
    private float xRotation = 0f; // Pitch (Up/Down)
    private float yRotation = 0f; // Yaw (Left/Right) - Used only if no body is assigned
    private bool isInputEnabled = true;
    private Quaternion initialRotation;

    void Start()
    {
        // Save the starting rotation for the Reset function
        initialRotation = transform.localRotation;
        
        // Initialize internal rotation values based on start orientation
        Vector3 startEuler = transform.localEulerAngles;
        xRotation = startEuler.x;
        yRotation = startEuler.y;

        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isInputEnabled) return;

        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        // 1. Get raw input
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;

        // 2. Calculate Vertical Rotation (Pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

        // 3. Apply Rotation
        if (playerBody != null)
        {
            // Standard FPS: Rotate Camera vertically, Body horizontally
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
        else
        {
            // Turret/Stationary: Rotate Camera both ways
            yRotation += mouseX;
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }

    /// <summary>
    /// Call this to enable or disable mouse control.
    /// </summary>
    public void SetInputActive(bool active)
    {
        isInputEnabled = active;
        
        // Optional: Unlock cursor when input is disabled
        Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !active;
    }

    /// <summary>
    /// Resets the camera to its starting rotation.
    /// </summary>
    public void ResetCamera()
    {
        // Reset internal variables
        xRotation = 0f;
        yRotation = 0f; // Or initial values if you want to support non-zero start

        // Apply original rotation
        transform.localRotation = initialRotation;
        
        // If using a body, you might want to reset that too, 
        // strictly speaking this resets the Camera's relation to its parent.
    }
}