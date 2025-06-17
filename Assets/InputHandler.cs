using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Ins { get; private set; }

    public event Action OnGrabPressed;
    public bool grabButtonState;
    public bool lastGrabButtonState;

    public event Action OnRoomStateSwitch;
    public bool roomSwitchState;

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
        HandleGrab();
    }

    private void HandleGrab()
    {
        if (lastGrabButtonState == grabButtonState) return;

        if (grabButtonState && !lastGrabButtonState)
        {
            OnGrabPressed?.Invoke();
        }
        
        lastGrabButtonState = grabButtonState;
    }
}
