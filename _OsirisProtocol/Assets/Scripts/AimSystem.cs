using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class AimSystem : MonoBehaviour
{
    playerCtrl player;
    PlayerInput input;
    
    public bool isAiming;
    bool leftTrigger;
    bool check;

    public CinemachineVirtualCamera standVC;
    public CinemachineVirtualCamera aimVC;
    public CinemachineVirtualCamera AimcoverVC;

    public Transform cameraFollower;
    public Transform standAimController;
    public Transform coverAimController;

    MultiRotationConstraint rotationConstraint;
    private void Awake()
    {
        input = new PlayerInput();
        input.characterControls.Aim.performed += ctx => leftTrigger = true;
        input.characterControls.Aim.canceled += ctx => leftTrigger = false;
        rotationConstraint = standAimController.parent.GetComponent<MultiRotationConstraint>();
    }
    void Start()
    {
        player = GetComponent<playerCtrl>();
    }
    void Update()
    {
        if (player.currentStates == player.coverstate)
        {
            check = false;
            rotationConstraint.weight = 0f;
        }
        else if (check)
        {
            rotationConstraint.weight = 1f;
        }

        if(leftTrigger)
        {
            if(player.currentStates == player.standstate)
            {
                aimVC.Priority = 2;
            }
            else
            {
                AimcoverVC.Priority = 4;
            }
            
            isAiming = true;
        }
        else
        {
            if (player.currentStates == player.standstate)
            {
                aimVC.Priority = 0;
            }
            else
            {
                AimcoverVC.Priority = 0;
            }
            
            isAiming = false;
        }

        player.animator.SetBool("IsAiming", isAiming);
    }
    void SetAniamtorWeightLayer()
    {
        check = true;
        player.animator.SetLayerWeight(1, 0.85f);
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
