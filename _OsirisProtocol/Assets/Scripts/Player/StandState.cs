using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandState : BaseStates
{
    public bool isWalking;
    float rotationX;

    ShootSystem _shootSystem;

    public override void EnterState(PlayerCharacterController player)
    {
        _shootSystem = player.GetComponent<ShootSystem>();
        player.animator.applyRootMotion = true;
    }
    public override void UpdateState(PlayerCharacterController player)
    {
        _shootSystem.HandleShot();

        //Handle Aim
        if (player.gamePad.leftTrigger)
        {
            player.cameraVC.setCameraVC(1);
        }
        else
        {
            player.cameraVC.setCameraVC(0);
        }

        //Root motion moves Axis X,Z
        if (player.gamePad.leftJoystick != Vector2.zero)
        {
            player.animator.SetBool("IsWalking", true);
            player.animator.SetFloat("AxisX", player.gamePad.leftJoystick.x);
            player.animator.SetFloat("AxisY", player.gamePad.leftJoystick.y);
        }
        else
        {
            player.animator.SetBool("IsWalking", false);
        }

        //Player rotation along Y axis
        float rotationY = player.gamePad.rightJoystick.x * player.rotationSpeed * Time.deltaTime;
        player.transform.Rotate(Vector3.up, rotationY);

        //Rotate the torso and the camera along the X axis
        rotationX -= player.gamePad.rightJoystick.y * player.rotationSpeed * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -45, 65);

        player.cameraFollower.localRotation = Quaternion.Euler(rotationX, 0, 0);
        player.torsoRotationController.localRotation = Quaternion.Euler(rotationX, 0, 0);

    }
    public override void ExitState(PlayerCharacterController player)
    {
        player.animator.SetBool("IsWalking", false);
        player.animator.SetFloat("AxisX", 0);
        player.animator.SetFloat("AxisY", 0);
    }
}
