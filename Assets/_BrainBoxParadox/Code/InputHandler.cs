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

    public event Action OnConverterShake;
    public bool knockButtonState;
    public float lastKnockTime = float.NegativeInfinity;

    private void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Ins = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        //Input.GetKey methods used for simple communication from Arduino as HID device to Unity. Not elegant but foolproof (i am the fool).
        
        //Setting grabButtonStates based on the Touchpads
        grabButtonAState = Input.GetKey(KeyCode.F5);
        grabButtonBState = Input.GetKey(KeyCode.F6);
        HandleGrab();

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
        HandleRoomSwitch(Input.GetKey(KeyCode.F4));

        if (Input.GetKeyDown(KeyCode.F7))
        {
            HandleConverterShake();
        }
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
}
