using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Cinemachine;

public class WalkingState : BaseStates
{
    public enum PlayerState
    {
        stand,
        stealth
    }

    public PlayerState currentState;

    PlayerInput input;

    float xRotation = 0f;
    float yRotation = -8f;
    float rotationSpeed;

    bool isCrouching;
    bool onWalking;

    Vector2 leftJoystick;
    Vector2 rightJoystick;

    Transform playerTransform;
    Transform cameraPivot;

    Animator animator;

    CinemachineVirtualCamera crouchVC;

    public override void AwakeState(playerCtrl player)
    {
        input = new PlayerInput();
        input.Enable();

        //Joystticks detector
        input.characterControls.Movement.performed += ctx => leftJoystick = ctx.ReadValue<Vector2>();
        input.characterControls.View.performed += ctx => rightJoystick = ctx.ReadValue<Vector2>();

        //Button detector
        input.characterControls.Crouch.performed += ctx => isCrouching = !isCrouching;
    }
    public override void EnterState(playerCtrl player)
    {
        Debug.Log("Stand State On");
        
        isCrouching = false;

        animator = player.GetComponent<Animator>();
        animator.SetBool("IsCrouch", false);

        playerTransform = player.transform;
        rotationSpeed = player.rotationSpeed;
        animator = player.GetComponent<Animator>();
        cameraPivot = player.cameraPivot;

        crouchVC = GameObject.Find("Crouch VC").GetComponent<CinemachineVirtualCamera>();
        crouchVC.Priority = 0;

        currentState = PlayerState.stand;

    }
    public override void UpdateState(playerCtrl player)
    {
        FSM();
        
        if (isCrouching)
        {
            currentState = PlayerState.stealth;
            animator.SetBool("IsCrouch", true);
        }
        else
        {
            currentState = PlayerState.stand;
            animator.SetBool("IsCrouch", false);
        }

    }
    public override void ExitState(playerCtrl player) 
    {

    }
    public void FSM()
    {
        switch (currentState)
        {
            case PlayerState.stand:
                handleRotation();
                onMove();
                break;
            case PlayerState.stealth:
                handleRotation();
                onMove();
                break;
        }
    }

    public void handleRotation()
    {
        float joystickR_AxisX = rightJoystick.x * rotationSpeed * Time.deltaTime;
        float joystickR_AxisY = rightJoystick.y * rotationSpeed * Time.deltaTime;

        xRotation -= joystickR_AxisY;
        xRotation = Mathf.Clamp(xRotation, -25f, 60f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        playerTransform.Rotate(Vector3.up, joystickR_AxisX);

        animator.SetFloat("ViewY", -(xRotation));
    }
    public void onMove()
    {
        if (leftJoystick != Vector2.zero)
        {
            onWalking=true;
            animator.SetBool("IsWalking", onWalking);
            animator.SetFloat("AxisY", leftJoystick.y);
            animator.SetFloat("AxisX", leftJoystick.x);
        }
        else
        {
            onWalking=false;
            animator.SetBool("IsWalking", onWalking);
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
