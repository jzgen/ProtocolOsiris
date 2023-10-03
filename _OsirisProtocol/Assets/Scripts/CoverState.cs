using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CoverState : BaseStates
{
    bool onPsotion = false;
    RaycastHit hit;

    float speed = 6;

    float maxBorder;
    float minBorder;
    float currentPosition;
    float borderOfsset = 1;

    public override void EnterState(playerCtrl player)
    {
        maxBorder = player.lengthCollider / 2 - borderOfsset;
        minBorder = -player.lengthCollider / 2 + borderOfsset;
    }
    public override void UpdateState(playerCtrl player)
    {
        if (!onPsotion)
        {
            if (player.transform.position != player.fixedPosition || player.transform.rotation != player.fixedRotation)
            {
                player.transform.position = player.fixedPosition;
                player.transform.rotation = player.fixedRotation;
                onPsotion = true;
            }
        }
        else
        {
            MoveAlongCover(player);
        }
    }
    public override void ExitState(playerCtrl player)
    {
        onPsotion = false;
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
            }
        }
    }
}
