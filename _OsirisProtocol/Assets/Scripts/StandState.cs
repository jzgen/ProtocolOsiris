using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandState : BaseStates
{
    public Vector2 leftJoystick;
    public Vector2 rightJoystick;
    public override void EnterState(playerCtrl player)
    {

    }
    public override void UpdateState(playerCtrl player)
    {
        leftJoystick = player.leftJoystick;
        rightJoystick = player.rightJoystick;
        handleRotation(player);
        onMove(player);
    }
    public override void ExitState(playerCtrl player)
    {

    }
    public void handleRotation(playerCtrl player)
    {
        float joystickR_AxisX = rightJoystick.x * player.rotationSpeed * Time.deltaTime;
        float joystickR_AxisY = rightJoystick.y * player.rotationSpeed * Time.deltaTime;

        player.xRotation -= joystickR_AxisY;
        player.xRotation = Mathf.Clamp(player.xRotation, -25f, 60f);

        player.cameraPivot.localRotation = Quaternion.Euler(player.xRotation, player.yRotation, 0f);
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