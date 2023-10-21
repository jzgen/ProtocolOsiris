using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(GamepadHandler))]
public class PlayerCharacterController : MonoBehaviour
{
    [Header("General")]
   
    [Tooltip("Adjust the fall speed")]
    public float gravity;
    
    [Tooltip("Adjust the player rotation on the Y axis")]
    public float rotationSpeed;

    [Header("References")]

    [Tooltip("Put the rotation controller associated to the torso from the animation rigging")]
    public Transform torsoRotationController;

    [Tooltip("Put the camera follower object")]
    public Transform cameraFollower;

    //Player components
    [HideInInspector] public Animator animator;
    [HideInInspector] public GamepadHandler gamePad;
    private CharacterController characterController;

    //Camera Components
    public VirtualCameraController cameraVC;

    //States
    public BaseStates currentStates;
    public StandState standstate = new StandState();

    void Start()
    {
        gamePad = GetComponent<GamepadHandler>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        cameraVC = Camera.main.GetComponent<VirtualCameraController>();

        SetState(standstate);
    }

    void Update()
    {
        //Apply gravity
        if (!characterController.isGrounded)
        {
            characterController.Move(Vector3.down * gravity * Time.deltaTime);
        }

        if(currentStates != null)
        {
            currentStates.UpdateState(this);
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
}
