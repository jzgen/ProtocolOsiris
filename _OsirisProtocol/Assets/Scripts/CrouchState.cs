using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : BaseStates
{
    ShootSystem shootSystem;

    public Vector2 leftJoystick;
    public Vector2 rightJoystick;

    CinemachineVirtualCamera virtualCamera;

    public override void EnterState(playerCtrl player)
    {
        virtualCamera = GameObject.Find("Crouch VC").GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Priority = 1;

        shootSystem = player.GetComponent<ShootSystem>();

        player.animator.SetBool("IsCrouch", true);

        shootSystem.noise = shootSystem.crouchVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    }
    public override void UpdateState(playerCtrl player)
    {
        leftJoystick = player.leftJoystick;
        rightJoystick = player.rightJoystick;

        //handleRotation(player);
        onMove(player);
    }
    public override void ExitState(playerCtrl player)
    {
        player.animator.SetBool("IsCrouch", false);
        virtualCamera.Priority = 0;

        shootSystem.noise = shootSystem.standVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    }
    public void handleRotation(playerCtrl player)
    {
        float joystickR_AxisX = rightJoystick.x * player.rotationSpeed * Time.deltaTime;
        float joystickR_AxisY = rightJoystick.y * player.rotationSpeed * Time.deltaTime;

        player.xRotation -= joystickR_AxisY;
        player.xRotation = Mathf.Clamp(player.xRotation, -25f, 60f);

        player.standCamPivot.localRotation = Quaternion.Euler(player.xRotation, player.yRotation, 0f);
        player.transform.Rotate(Vector3.up, joystickR_AxisX);

        player.animator.SetFloat("ViewY", -(player.xRotation));
    }
    public void onMove(playerCtrl player)
    {

        if (leftJoystick != Vector2.zero)
        {
            player.animator.SetBool("IsWalking", true);
            player.animator.SetFloat("AxisY", leftJoystick.y);
            player.animator.SetFloat("AxisX", leftJoystick.x);
        }
        else
        {
            player.animator.SetBool("IsWalking", false);
        }
    }
}
