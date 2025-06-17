using System;
using UnityEngine;

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
    }
}
