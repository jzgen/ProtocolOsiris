using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverState : BaseStates
{
    float speed = 4;
    float borderOffset = 0.5f;

    Transform borderRayOrigin;

    CoverSystem _coverSystem;
    ShootSystem _shootSystem;
    CinemachineFreeLook coverCamera;

    public override void EnterState(PlayerCharacterController player)
    {
        _coverSystem = player.GetComponent<CoverSystem>();
        borderRayOrigin = _coverSystem.borderRayOrigin;

        _shootSystem = player.GetComponent<ShootSystem>();

        //Camera Settings
        coverCamera = player.cameraVC.FreeLookCamera;
        coverCamera.Priority = 2;
        coverCamera.m_YAxis.Value = 0.5f;
    }

    public override void UpdateState(PlayerCharacterController player)
    {
        if (player.gamePad.leftTrigger)
        {
            player.animator.SetBool("IsAiming", true);
            _shootSystem.HandleShot();
        }
        else
        {
            MoveAlongCover(player);
            player.animator.SetBool("IsAiming", false);
            _shootSystem.HandleReload();
        }

        HandleCamera(player);

    }
    public override void ExitState(PlayerCharacterController player)
    {
        //Camera Settings
        coverCamera.m_Priority = 0;
    }

    void HandleCamera(PlayerCharacterController player)
    {
        float speedX = 120f;
        float speedY = 2f;
        coverCamera.m_XAxis.Value += player.gamePad.rightJoystick.x * speedX * Time.deltaTime;
        coverCamera.m_YAxis.Value += -player.gamePad.rightJoystick.y * speedY * Time.deltaTime;
    }

    void MoveAlongCover(PlayerCharacterController player)
    {
        bool borderReached = false;

        //Change border position based on the player direction in cover
        Vector3 rayOrigin = borderRayOrigin.transform.position;
        if (player.gamePad.leftJoystick.x > 0.1f)
        {
            rayOrigin += borderRayOrigin.transform.TransformDirection(Vector3.right) * borderOffset;
        }
        else if (player.gamePad.leftJoystick.x < -0.1f)
        {
            rayOrigin += borderRayOrigin.transform.TransformDirection(Vector3.left) * borderOffset;
        }

        //Detect the border 
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, player.transform.forward, out hit, 1))
        {
            //Add the surface orientation void
        }
        else
        {
            borderReached = true;
        }

        //Allow or block the walk along the cover
        if (borderReached)
        {
            player.animator.SetFloat("AxisX", 0);
        }
        else
        {
            Vector3 move = Vector3.right * player.gamePad.leftJoystick.x * Time.deltaTime * speed;
            player.transform.Translate(move, Space.Self);
            player.animator.SetFloat("AxisX", player.gamePad.leftJoystick.x);
        }
    }
}
