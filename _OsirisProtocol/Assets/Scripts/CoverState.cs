using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CoverState : BaseStates
{
    bool onPosition = false;
    RaycastHit hit;

    float speed = 4;

    float maxBorder;
    float minBorder;
    float currentPosition;
    float borderOfsset = 1;

    CinemachineVirtualCamera virtualCamera;

    public override void EnterState(playerCtrl player)
    {
        virtualCamera = GameObject.Find("CoverVC").GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Priority = 3;
        maxBorder = player.lengthCollider / 2 - borderOfsset;
        minBorder = -player.lengthCollider / 2 + borderOfsset;
    }
    public override void UpdateState(playerCtrl player)
    {
        if (!onPosition)
        {
            if (player.transform.position != player.fixedPosition || player.transform.rotation != player.fixedRotation)
            {
                player.transform.position = player.fixedPosition;
                player.transform.rotation = player.fixedRotation;
                onPosition = true;
            }
        }
        else
        {
            MoveAlongCover(player);
        }
    }
    public override void ExitState(playerCtrl player)
    {
        virtualCamera.Priority = 0;
        onPosition = false;
    }

    public void MoveAlongCover(playerCtrl player)
    {
        currentPosition = player.positionRelativeToCollider;

        if (!player.isFlipped)
        {
            if (currentPosition > maxBorder && player.leftJoystick.x > 0)
            {
                Vector3 lastPosition = player.transform.position;
                player.transform.position = lastPosition;
            }
            else if (currentPosition < minBorder && player.leftJoystick.x < 0)
            {
                Vector3 lastPosition = player.transform.position;
                player.transform.position = lastPosition;
            }
            else
            {
                Vector3 move = Vector3.right * player.leftJoystick.x * Time.deltaTime * speed;
                player.transform.Translate(move, Space.Self);
                player.animator.SetFloat("AxisX", player.leftJoystick.x);
            }
        }
        else
        {
            if (currentPosition > maxBorder && player.leftJoystick.x < 0)
            {
                Vector3 lastPosition = player.transform.position;
                player.transform.position = lastPosition;
            }
            else if (currentPosition < minBorder && player.leftJoystick.x > 0)
            {
                Vector3 lastPosition = player.transform.position;
                player.transform.position = lastPosition;
            }
            else
            {
                Vector3 move = Vector3.right * player.leftJoystick.x * Time.deltaTime * speed;
                player.transform.Translate(move, Space.Self);
                player.animator.SetFloat("AxisX", player.leftJoystick.x);
            }
        }
    }
}
