using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class StandState : BaseStates
{
    Vector2 leftJoystick;
    Vector2 rightJoystick;

    AimSystem playerAim;
    public override void EnterState(playerCtrl player)
    {
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

        //Rotate th player
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

    }
}
