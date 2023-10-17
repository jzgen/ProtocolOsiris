using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class AimSystem : MonoBehaviour
{
    PlayerCharacterController player;
    PlayerInput input;
    
    public enum PlayerState
    {
        Stand,
        Cover
    }

    private PlayerState currentState = PlayerState.Stand;

    bool leftTrigger;
    
    [HideInInspector] public bool isAiming;
    [HideInInspector] public bool isCover;

    [Header("Virtual Aim Cameras")]
    public CinemachineVirtualCamera aimVC;
    public CinemachineVirtualCamera coverAimVC;

    [HideInInspector] public float standWeightConstraint;
    [HideInInspector] public float coverWeightConstraint;

    [Header("Animation Rigging Components")]
    public Transform standAimController;
    public Transform coverAimController;
    public MultiRotationConstraint rotationConstraint;

    [Header("Cameras Pivot")]
    public Transform cameraFollower;
    public Transform coverCameraFollower;

    Transform bone;
    private void Awake()
    {
        input = new PlayerInput();
        input.characterControls.Aim.performed += ctx => leftTrigger = true;
        input.characterControls.Aim.canceled += ctx => leftTrigger = false;
    }
    void Start()
    {
        bone = GameObject.FindGameObjectWithTag("Player").transform;

        standWeightConstraint = 1;
        coverWeightConstraint = 0;
        
        isCover = false;

        player = GetComponent<PlayerCharacterController>();
        rotationConstraint = rotationConstraint.GetComponent<MultiRotationConstraint>();
    }
    void Update()
    {
        Vector3 newPosition = new Vector3(bone.position.x, coverCameraFollower.position.y, bone.position.z);
        coverCameraFollower.position = newPosition;

        if (rotationConstraint!=null)
        {
            SetRotationConstraintWeight(0, standWeightConstraint); //Adjust in RunTime the torso stand rotation constraint
            SetRotationConstraintWeight(1, coverWeightConstraint);
        }


        if (leftTrigger)
        {   
            isAiming = true;
            player.animator.SetBool("IsAiming", true);
        }
        else
        {   
            isAiming = false;
            player.animator.SetBool("IsAiming", false);
        }

        if(isCover)
        {
            currentState = PlayerState.Cover;
        }
        else
        {
            currentState= PlayerState.Stand;
        }

        FSM();

    }
    void FSM()
    {
        switch(currentState)
        {
            case PlayerState.Stand:
                standState();
                break;

            case PlayerState.Cover:
                coverState();
                break;
        }
    }

    void standState()
    {
        if (isAiming)
        {
            aimVC.Priority = 2;
        }
        else
        {
            aimVC.Priority = 0;
        }
    }

    void coverState()
    {
        if (isAiming)
        {
            coverAimVC.Priority = 4;
        }
        else
        {
            coverAimVC.Priority = 0;
        }
    }

    //Animation Event - CoverLo_CoverR2Idle, CoverLo_CoverL2Idle
    void SetStandWeight()
    {
        player.animator.SetLayerWeight(1, 0.85f);
        standWeightConstraint = 1;
        isCover = false;
    }
    //Void to change the weight of the source rotation constraint
    public void SetRotationConstraintWeight(int index, float weight)
    {
        var sources = rotationConstraint.data.sourceObjects;
        sources.SetWeight(index, weight);
        rotationConstraint.data.sourceObjects = sources;
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
