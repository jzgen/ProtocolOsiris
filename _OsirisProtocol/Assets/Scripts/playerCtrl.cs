using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class playerCtrl : MonoBehaviour
{
    PlayerInput input;

    BaseStates currentStates;
    StandState standstate = new StandState();
    CrouchState crouchstate = new CrouchState();

    public float rotationSpeed = 100f;
    [HideInInspector] public float xRotation = 0f;
    [HideInInspector] public float yRotation = -8f;

    //Global Joysticks Value
    [HideInInspector] public Animator animator;
    [HideInInspector] public Vector2 leftJoystick;
    [HideInInspector] public Vector2 rightJoystick;

    public Transform cameraPivot;
    [HideInInspector] public CinemachineVirtualCamera crouchVC;

    void Awake()
    {
        input = new PlayerInput();

        input.characterControls.Movement.performed += ctx => leftJoystick = ctx.ReadValue<Vector2>();
        input.characterControls.View.performed += ctx => rightJoystick = ctx.ReadValue<Vector2>();
        input.characterControls.Crouch.performed += OnCrouchPerformed;
    }
    void Start()
    {
        crouchVC = GameObject.Find("Crouch VC").GetComponent<CinemachineVirtualCamera>();

        animator = GetComponent<Animator>();
        SetState(standstate);
    }
    void Update()
    {
        if (currentStates != null)
        {
            currentStates.UpdateState(this);
        }
    }
    void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        if (currentStates == standstate)
        {
            SetState(crouchstate);
        }
        else
        {
            SetState(standstate);
        }
    }
    public void SetState(BaseStates state)
    {
        if (currentStates != null)
        {
            currentStates.ExitState(this);
        }

        currentStates = state;
        currentStates.EnterState(this);
    }
    void OnEnable()
    {
        input.characterControls.Enable();
    }
    void OnDisable()
    {
        input.characterControls.Disable();
    }
}
