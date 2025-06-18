using System;
using UnityEngine;

/// <summary>
/// Immitates Arduino-Controls with the keyboard for faster prototyping
/// </summary>
public class FauxInput : MonoBehaviour
{
    private InputHandler inputHandler;


    private void OnEnable()
    {
        inputHandler = InputHandler.Ins;
    }

    private void Update()
    {
        InputHandler.Ins.grabButtonState = Input.GetKey(KeyCode.Space);
        
        if (Input.GetKeyDown(KeyCode.F1))
        {
            InputHandler.Ins.HandleRoomSwitch(!InputHandler.Ins.lastRoomSwitchState);   //Just switches room-state when S is pressed
        }
    }
}
