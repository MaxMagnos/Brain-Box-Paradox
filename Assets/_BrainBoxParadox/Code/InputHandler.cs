using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Ins { get; private set; }

    public event Action OnGrabPressed;
    public event Action OnGrabReleased;
    public bool grabButtonAState;
    public bool grabButtonBState;
    public bool currentlyGrabbing;

    public event Action OnRoomStateSwitch;
    public bool lastRoomSwitchState;

    public event Action OnMorphSliderChange;
    public int morphSliderValue;
    public float wheelInput;
    public int wheelValue;
    public int virtualSliderValue;

    public event Action OnConverterShake;
    public bool knockButtonState;
    public float lastKnockTime = float.NegativeInfinity;
    
    
    [Header("Shake Detection Settings")]
    [Tooltip("How much 'shake energy' is required to trigger the event.")]
    public float shakeThreshold = 200f;

    [Tooltip("How fast the shake bar depletes per second.")]
    public float shakeDecay = 150f;

    [Tooltip("Minimum mouse speed required to register as a shake stroke. Filters out small jitters.")]
    public float minShakeSpeed = 2f;

    // Internal State
    [SerializeField]private float currentShakeLevel = 0f;
    [SerializeField]private float currentStrokeAccumulator = 0f; // Stores distance of current movement
    private float lastFrameMouseX = 0f;          // Used to detect direction flips

    private void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Ins = this;
        DontDestroyOnLoad(gameObject);

        wheelValue = 1000000;
    }

    private void Update()
    {
        //Input.GetKey methods used for simple communication from Arduino as HID device to Unity. Not elegant but foolproof (i am the fool).
        
        //Setting grabButtonStates based on the Touchpads
        grabButtonAState = Input.GetKey(KeyCode.Mouse0);
        grabButtonBState = Input.GetKey(KeyCode.Mouse1);
        HandleGrab();

        
        //Mouse Implementation for MorphSlider:
        wheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (wheelInput > 0) //Scrolled up
        {
            wheelValue++;
        }
        else if (wheelInput < 0) //Scrolled down
        {
            wheelValue--;
        }
        virtualSliderValue = wheelValue % 3 + 1;
        HandleMorphSlider(virtualSliderValue);
        Debug.Log("MorphSlider: " + virtualSliderValue);
        
        
        //Calling HandleMorphSlider based on F1-3 key pressed.
        if (Input.GetKeyDown(KeyCode.F1))
        {
            HandleMorphSlider(1);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            HandleMorphSlider(2);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            HandleMorphSlider(3);
        }
        
        //Switching Room based on the Input from F4
        //HandleRoomSwitch(Input.GetKey(KeyCode.F4));
        //Alternative Method more optimized for Keyboard use
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            HandleRoomSwitchStatic();
        }

        UpdateMouseShake();
        
        // if (Input.GetKeyDown(KeyCode.F7))
        // {
        //     HandleConverterShake();
        // }
    }

    private void HandleGrab()
    {
        if (!currentlyGrabbing)
        {
            if (grabButtonAState && grabButtonBState)
            {
                currentlyGrabbing = true;
                OnGrabPressed?.Invoke();
            }
        }
        else
        {
            if (!grabButtonAState && !grabButtonBState)
            {
                currentlyGrabbing = false;
                OnGrabReleased?.Invoke();
            }
        }
    }

    public void HandleRoomSwitch(bool state)
    {
        if (state == lastRoomSwitchState)
            return;

        lastRoomSwitchState = state;
        OnRoomStateSwitch?.Invoke();
    }

    /// <summary>
    /// Alternative RoomSwitch method that works based on a single Impulse instead of the actual state of the switch (optimal for keyboard use)
    /// </summary>
    public void HandleRoomSwitchStatic()
    {
        lastRoomSwitchState = !lastRoomSwitchState;
        OnRoomStateSwitch?.Invoke();
    }

    public void HandleMorphSlider(int value)
    {
        if (morphSliderValue == value) return;
        
        morphSliderValue = value;
        OnMorphSliderChange?.Invoke();
    }

    public void HandleConverterShake()
    {
        if (Time.time > lastKnockTime + 1f)
        {
            OnConverterShake?.Invoke();
            lastKnockTime = Time.time;
        }
    }
    
    private void UpdateMouseShake()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime <= 0) return; // Safety check

        // 1. Get raw distance for this frame
        float mouseDelta = Input.GetAxisRaw("Mouse X");

        // 2. Calculate Velocity (Distance / Time) for the threshold check
        // This ensures 5 pixels at 200fps is treated the same as 30 pixels at 30fps
        float currentVelocity = Mathf.Abs(mouseDelta) / deltaTime;

        // 3. Logic: Accumulate Stroke vs. Bank Energy
        // We check VELOCITY for the filter, but we sum DELTA (distance) for the progress
        if (currentVelocity > minShakeSpeed)
        {
            // Detect Direction Flip
            bool isMovingOpposite = Mathf.Sign(mouseDelta) != Mathf.Sign(lastFrameMouseX);

            if (isMovingOpposite && lastFrameMouseX != 0)
            {
                // Flip detected: Bank the pending stroke into the actual meter
                currentShakeLevel += currentStrokeAccumulator;
                currentStrokeAccumulator = 0f;
            }

            // Add the physical distance moved this frame to the pending stroke
            currentStrokeAccumulator += Mathf.Abs(mouseDelta);
        }
        else
        {
            // Movement too slow? Reset the pending stroke (can't cheat by moving slowly)
            currentStrokeAccumulator = 0f;
        }

        // 4. Decay and Clamp
        currentShakeLevel -= shakeDecay * deltaTime;
        currentShakeLevel = Mathf.Clamp(currentShakeLevel, 0f, shakeThreshold);

        // 5. Trigger
        if (currentShakeLevel >= shakeThreshold)
        {
            OnShakeCompleted();
            currentShakeLevel = 0f; 
            currentStrokeAccumulator = 0f;
        }

        // 6. Update History
        if (Mathf.Abs(mouseDelta) > 0.01f)
        {
            lastFrameMouseX = mouseDelta;
        }
    }

    // The method that runs when the shake is done
    private void OnShakeCompleted()
    {
        Debug.Log("SHAKE DETECTED!");
        HandleConverterShake();
    }
}
