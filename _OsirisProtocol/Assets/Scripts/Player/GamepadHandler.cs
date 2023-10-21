using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadHandler : MonoBehaviour
{
    public PlayerInput input;

    [Header("Joysticks Read")]
    public Vector2 leftJoystick;
    public Vector2 rightJoystick;

    [Header("Triggers Read")]
    public bool rightTrigger;
    public bool leftTrigger;

    [Header("Triggers Motor")]
    public float vibrationIntensity = 0.8f;

    Gamepad gamepad;

    void Awake()
    {
        input = new PlayerInput();
        input.characterControls.Movement.performed += ctx => leftJoystick = ctx.ReadValue<Vector2>();
        input.characterControls.View.performed += ctx => rightJoystick = ctx.ReadValue<Vector2>();

        gamepad = Gamepad.current;
    }
    private void Update()
    {
        input.characterControls.Fire.performed += ctx => rightTrigger = true;
        input.characterControls.Fire.canceled += ctx => rightTrigger = false;

        input.characterControls.Aim.performed += ctx => leftTrigger = true;
        input.characterControls.Aim.canceled += ctx => leftTrigger = false;
    }
    public void vibrateController(bool perform)
    {
        if(gamepad != null)
        {
            if (perform == true)
            {
                gamepad.SetMotorSpeeds(vibrationIntensity, vibrationIntensity);
            }
            else
            {
                gamepad.SetMotorSpeeds(0, 0);
            }
        }
    }

    private void OnEnable()
    {
        input.characterControls.Enable();
    }

    private void OnDisable()
    {
        input.characterControls.Disable();
    }
}
