using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadHandler : MonoBehaviour
{
    [Header("Joysticks Read")]
    public Vector2 leftJoystick;
    public Vector2 rightJoystick;

    [Header("Triggers Read")]
    public bool rightTrigger;
    public bool leftTrigger;

    [Header("Triggers Motor")]
    public float vibrationIntensity = 0.8f;

    public PlayerInput input;
    Gamepad gamepad;

    void Awake()
    {
        input = new PlayerInput();
        gamepad = Gamepad.current; //Get the current gamepad

        //Return the left and right joysticks input and store it in a public Vector2
        input.characterControls.Movement.performed += ctx => leftJoystick = ctx.ReadValue<Vector2>();
        input.characterControls.View.performed += ctx => rightJoystick = ctx.ReadValue<Vector2>();

    }
    private void Update()
    {
        //Update if the right trigger is being perfomed
        input.characterControls.Fire.performed += ctx => rightTrigger = true;
        input.characterControls.Fire.canceled += ctx => rightTrigger = false;

        //Update if the left trigger is being perfomed
        input.characterControls.Aim.performed += ctx => leftTrigger = true;
        input.characterControls.Aim.canceled += ctx => leftTrigger = false;
    }
    public void vibrateController(bool perform)
    {
        if(gamepad != null)
        {
            if (perform == true) //If perform is true, turn on the controller vibration for both gamepad's motors
            {
                gamepad.SetMotorSpeeds(vibrationIntensity, vibrationIntensity);
            }
            else  //If perform is false, turn off the controller vibration for both gamepad's motors
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
