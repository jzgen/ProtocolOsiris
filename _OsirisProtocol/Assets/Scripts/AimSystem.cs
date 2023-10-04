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
            aimVC.Priority = 2;
            isAiming = true;
        }
        else
        {
            isAiming = false;
            aimVC.Priority = 0;
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
