using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Ins { get; private set; }

    public event Action OnGrabPressed;
    public bool grabButtonState;
    public bool lastGrabButtonState;

    public event Action OnRoomStateSwitch;
    public bool lastRoomSwitchState;

    public event Action OnMorphSliderChange;
    public int morphSliderValue;

    public event Action OnKnock;
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

    public void HandleKnock()
    {
        if (Time.time > lastKnockTime)
        {
            OnKnock?.Invoke();
            lastKnockTime = Time.time;
        }
    }
}
