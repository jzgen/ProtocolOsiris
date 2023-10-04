using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimSystem : MonoBehaviour
{
    playerCtrl player;
    PlayerInput input;
    
    public bool isAiming;
    bool leftTrigger;

    public CinemachineVirtualCamera standVC;
    public CinemachineVirtualCamera aimVC;
    public CinemachineVirtualCamera AimcoverVC;

    private void Awake()
    {
        input = new PlayerInput();
        input.characterControls.Aim.performed += ctx => leftTrigger = true;
        input.characterControls.Aim.canceled += ctx => leftTrigger = false;
    }
    void Start()
    {
        player = GetComponent<playerCtrl>();


    }
    void Update()
    {
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

    private void OnEnable()
    {
        input.characterControls.Enable();
    }
    private void OnDisable()
    {
        input.characterControls.Disable();
    }
}
