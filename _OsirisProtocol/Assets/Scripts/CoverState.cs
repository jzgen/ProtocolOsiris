using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CoverState : BaseStates
{
    bool canMove = false;
    float speed = 4;

    public float rotationX;
    public float rotationY;

    //Border values
    float maxBorder;
    float minBorder;
    float currentPosition;
    float borderOfsset = 1;

    //Interpolation variables
    Vector3 lastPosition;
    Vector3 fixedPosition;
    Quaternion lastRotation;
    Quaternion fixedRotation;

    float duration = 0.4f;
    float timeElapsed = 0;

    // Joysticks Input stores
    Vector2 leftJoystick;
    Vector2 rightJoystick;

    CinemachineVirtualCamera virtualCamera;
    AimSystem aimSystem;

    public override void EnterState(playerCtrl player)
    {
        debugNullErrors(player);

        aimSystem = player.GetComponent<AimSystem>();
        aimSystem.standWeightConstraint = 0;

        //Store the last raycast info and player transforms to avoid variations on the interpolation
        fixedPosition = player.fixedPosition;
        lastPosition = player.lastPosition;

        fixedRotation = player.fixedRotation;
        lastRotation = player.lastRotation;

        //Initialize the time in 0 to avoid conflicts
        timeElapsed = 0;

        //Change the StandVC to CoverVC 
        virtualCamera = GameObject.Find("CoverVC").GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Priority = 3;

        //Get the borders in right and left direction and add an offset to both sides
        maxBorder = player.lengthCollider / 2 - borderOfsset;
        minBorder = -player.lengthCollider / 2 + borderOfsset;
    }
    public override void UpdateState(playerCtrl player)
    {
        //Listen and store the inputs
        leftJoystick = player.leftJoystick;
        rightJoystick = player.rightJoystick;

        if (aimSystem.isAiming)
        {
            speed = 1;
        }
        else
        {
            speed = 4;
        }

        if (!canMove)
        {
            leftJoystick = Vector2.zero; 
            rightJoystick = Vector2.zero;
            Interpolate(player);
        }
        else
        {
            MoveAlongCover(player);
            handleRotation(player);
        }
    }
    public override void ExitState(playerCtrl player)
    {
        rotationY = 0;

        //Return the current VC  to StandVC
        virtualCamera.Priority = 0;

        //Clean the variable that allow the playerMove
        canMove = false;
    }

    //Move the the player from last position to the initial cover position
    void Interpolate(playerCtrl player)
    {
        timeElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(timeElapsed / duration);

        Vector3 positionInterpolated = Vector3.Lerp(lastPosition,fixedPosition, t);
        Quaternion rotationInterpolated = Quaternion.Slerp(lastRotation, fixedRotation, t);

        player.transform.position = positionInterpolated;
        player.transform.rotation = rotationInterpolated;

        if (t >= 1)
        {
            player.transform.position = fixedPosition;
            player.transform.rotation = fixedRotation;
            canMove = true;
        }
    }

    //Handle the camera cover rotation
    void handleRotation(playerCtrl player)
    {
        rotationY += rightJoystick.x * player.rotationSensitivity * Time.deltaTime;
        player.gimbalY.localRotation = Quaternion.Euler(0, rotationY, 0);

        if (aimSystem.isAiming)
        {
            //Horizontal Aim Rotation
            aimSystem.coverWeightConstraint = 1;
            rotationY = Mathf.Clamp(rotationY, -75, 75);

            //Horizontal Aim Rotation
            rotationX -= rightJoystick.y *player.rotationSensitivity * Time.deltaTime;
            player.gimbalX.localRotation = Quaternion.Euler(rotationX, 0, 0);
            rotationX = Mathf.Clamp(rotationX, -55, 10);

            aimSystem.coverAimController.localRotation = player.gimbalY.localRotation * player.gimbalX.localRotation;

        }
        else
        {
            aimSystem.coverWeightConstraint = 0;
        }
    }

    //Move the player along the local X axis
    public void MoveAlongCover(playerCtrl player)
    {
        //Get the current position along the cover X axis
        currentPosition = player.positionRelativeToCollider;

        //If the cover is turned, change the border interpretation
        if (!player.isFlipped)
        {
            if (currentPosition >= maxBorder && leftJoystick.x >= 0)
            {
                Vector3 lastPosition = player.transform.position;
                player.transform.position = lastPosition;
                player.animator.SetFloat("AxisX",0);
            }
            else if (currentPosition <= minBorder && leftJoystick.x <= 0)
            {
                Vector3 lastPosition = player.transform.position;
                player.transform.position = lastPosition;
                player.animator.SetFloat("AxisX", 0);
            }
            else if (leftJoystick.x > 0.1 || leftJoystick.x < -0.1)
            {
                Vector3 move = Vector3.right * leftJoystick.x * Time.deltaTime * speed;
                player.transform.Translate(move, Space.Self);
                player.animator.SetFloat("AxisX", leftJoystick.x);
            }
            else
            {
                player.animator.SetFloat("AxisX", leftJoystick.x);
            }
        }
        else
        {
            if (currentPosition >= maxBorder && leftJoystick.x <= 0)
            {
                Vector3 lastPosition = player.transform.position;
                player.transform.position = lastPosition;
                player.animator.SetFloat("AxisX", 0);
            }
            else if (currentPosition <= minBorder && leftJoystick.x >= 0)
            {
                Vector3 lastPosition = player.transform.position;
                player.transform.position = lastPosition;
                player.animator.SetFloat("AxisX", 0);
            }
            else if (leftJoystick.x > 0.1 || leftJoystick.x < -0.1)
            {
                Vector3 move = Vector3.right * leftJoystick.x * Time.deltaTime * speed;
                player.transform.Translate(move, Space.Self);
                player.animator.SetFloat("AxisX", leftJoystick.x);
            }
            else
            {
                player.animator.SetFloat("AxisX", leftJoystick.x);
            }
        }
    }

    void debugNullErrors(playerCtrl player)
    {
        if (player.gimbalX == null)
            Debug.LogError("Missing " + player.gimbalX + ". Check the Cover Camera Follower component in the Aim System");

        if (player.gimbalY == null)
            Debug.LogError("Missing " + player.gimbalY + ". Check the Cover Camera Follower component in the Aim System");


    }
}