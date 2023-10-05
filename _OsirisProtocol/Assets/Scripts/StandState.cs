using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandState : BaseStates
{
    Vector2 leftJoystick;
    Vector2 rightJoystick;

    public float rotationX;

    AimSystem playerAim;

    public override void EnterState(playerCtrl player)
    {
        if(player.lastSate == player.coverstate)
        {
            rotationX = 0;
        }

        playerAim = player.GetComponent<AimSystem>();
    }
    public override void UpdateState(playerCtrl player)
    {
        //Listen and store the inputs
        leftJoystick = player.leftJoystick; 
        rightJoystick=player.rightJoystick;

        if (playerAim.isAiming)
        {
            leftJoystick = Vector2.ClampMagnitude(leftJoystick, 0.25f);
        }

        //Move the player with Left Joystick Input
        OnMove(player);

        //Rotate the player
        OnRotate(player);

    }
    public override void ExitState(playerCtrl player)
    {

    }
    void OnMove(playerCtrl player)
    {   
        //Update the animator variables
        if(leftJoystick != Vector2.zero)
        {
            player.animator.SetBool("IsWalking", true);

            player.animator.SetFloat("AxisX", leftJoystick.x);
            player.animator.SetFloat("AxisY", leftJoystick.y);
        }
        else
        {
            player.animator.SetBool("IsWalking", false);
        }
    }
    void OnRotate(playerCtrl player)
    {
        float rotatationY = rightJoystick.x * player.rotationSensitivity * Time.deltaTime;
        player.transform.Rotate(Vector3.up, rotatationY);

        rotationX -= rightJoystick.y * player.rotationSensitivity * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -45, 65);
        
        playerAim.standAimController.localRotation = Quaternion.Euler(rotationX,0,0);
        playerAim.cameraFollower.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }
}
